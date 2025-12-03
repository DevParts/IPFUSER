using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de gestión de colas de códigos con sistema de doble cola
    /// Basado en el sistema original de IPFUser (QueueFiller/QueueConsumer)
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILaserService _laserService;
        private readonly Promotion _promotion;

        // Colas principales (texto)
        private readonly Queue<string> _queue1 = new Queue<string>();
        private readonly Queue<string> _queue2 = new Queue<string>();
        private Queue<string> _activeQueue;

        // Colas para DataMatrix (si aplica) - Reservado para uso futuro
        // private readonly Queue<byte[]> _dataMatrixQueue1 = new Queue<byte[]>();
        // private readonly Queue<byte[]> _dataMatrixQueue2 = new Queue<byte[]>();
        // private Queue<byte[]>? _activeDataMatrixQueue;

        private readonly object _queueLock = new object();
        private Thread? _producerThread;
        private Thread? _consumerThread;
        private bool _isRunning = false;
        private CancellationTokenSource? _cancellationTokenSource;

        private int _producedCodes = 0;
        private int _totalToProduce = 0;
        private string _lastSentCode = string.Empty;

        public bool IsRunning => _isRunning;
        public int ProducedCodes => _producedCodes;
        public int PendingCodes
        {
            get
            {
                lock (_queueLock)
                {
                    return _queue1.Count + _queue2.Count;
                }
            }
        }
        public string LastSentCode => _lastSentCode;

        public event EventHandler<string>? ErrorOccurred;
        public event EventHandler<string>? CodeSent;

        public QueueService(IDatabaseService databaseService, ILaserService laserService, Promotion promotion)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _laserService = laserService ?? throw new ArgumentNullException(nameof(laserService));
            _promotion = promotion ?? throw new ArgumentNullException(nameof(promotion));
            _activeQueue = _queue1;
        }

        public void Start(int totalToProduce)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("El servicio de colas ya está ejecutándose");
            }

            _totalToProduce = totalToProduce;
            _producedCodes = 0;
            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            // Hilo productor (llena colas)
            _producerThread = new Thread(() => ProducerLoop(_cancellationTokenSource.Token))
            {
                Name = "QueueProducer",
                IsBackground = true
            };
            _producerThread.Start();

            // Hilo consumidor (envía al láser)
            _consumerThread = new Thread(() => ConsumerLoop(_cancellationTokenSource.Token))
            {
                Name = "QueueConsumer",
                IsBackground = true
            };
            _consumerThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();

            // Esperar a que los hilos terminen (máximo 2 segundos)
            _producerThread?.Join(2000);
            _consumerThread?.Join(2000);

            // Limpiar colas
            lock (_queueLock)
            {
                _queue1.Clear();
                _queue2.Clear();
                // DataMatrix queues reservadas para uso futuro
                // _dataMatrixQueue1.Clear();
                // _dataMatrixQueue2.Clear();
            }
        }

        private void ProducerLoop(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Llenar Queue1 si está vacía
                    lock (_queueLock)
                    {
                        if (_queue1.Count == 0 && _producedCodes < _totalToProduce)
                        {
                            FillQueue(_queue1);
                        }
                    }

                    // Llenar Queue2 si está vacía
                    lock (_queueLock)
                    {
                        if (_queue2.Count == 0 && _producedCodes < _totalToProduce)
                        {
                            FillQueue(_queue2);
                        }
                    }

                    Thread.Sleep(100); // Esperar antes de verificar nuevamente
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Error en ProducerLoop: {ex.Message}");
                    Thread.Sleep(500); // Esperar más tiempo en caso de error
                }
            }
        }

        private void FillQueue(Queue<string> queue)
        {
            try
            {
                // Calcular cuántos códigos cargar (máximo 50, como en el original)
                int remaining = _totalToProduce - _producedCodes;
                int records = Math.Min(remaining, 50);

                if (records <= 0)
                    return;

                // Obtener códigos de BD
                string sql = _promotion.GetSqlCodes(records);
                DataTable table = _databaseService.GetCodes(sql);

                if (table.Rows.Count == 0)
                    return;

                // Capturar IDs inicial y final para actualizar Consumed
                int idConsumedInitial = 0;
                int idConsumedFinal = 0;

                // Agregar a cola y marcar como enviados
                foreach (DataRow row in table.Rows)
                {
                    string? code = row["Code"]?.ToString();
                    if (!string.IsNullOrEmpty(code))
                    {
                        queue.Enqueue(code);
                        row["Sent"] = 1;
                        row["TimeStamp"] = DateTime.Now;

                        // Capturar IDs
                        int currentId = Convert.ToInt32(row["Id"]);
                        if (idConsumedInitial == 0)
                        {
                            idConsumedInitial = currentId;
                        }
                        idConsumedFinal = currentId;
                    }
                }

                // Actualizar BD - marcar códigos como enviados
                _databaseService.MarkCodesAsSent(table);

                // Actualizar Consumed en CodesIndex
                if (idConsumedInitial > 0 && idConsumedFinal > 0)
                {
                    _databaseService.UpdateConsumos(_promotion.JobId, idConsumedInitial, idConsumedFinal);
                }

                _producedCodes += table.Rows.Count;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error al llenar cola: {ex.Message}");
            }
        }

        private void ConsumerLoop(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    string? code = null;

                    // Tomar código de la cola activa
                    lock (_queueLock)
                    {
                        if (_activeQueue.Count > 0)
                        {
                            code = _activeQueue.Dequeue();
                        }
                    }

                    if (code != null)
                    {
                        // Enviar al láser según UserFields
                        bool success = SendCodeToLaser(code, _promotion);

                        if (success)
                        {
                            _lastSentCode = code;
                            CodeSent?.Invoke(this, code);
                        }
                        else
                        {
                            ErrorOccurred?.Invoke(this, $"Error al enviar código: {code}");
                            // Reintentar más tarde o manejar error según sea necesario
                        }
                    }

                    // Alternar cola activa
                    _activeQueue = _activeQueue == _queue1 ? _queue2 : _queue1;

                    Thread.Sleep(10); // Pequeña espera para no sobrecargar
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Error en ConsumerLoop: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        private bool SendCodeToLaser(string code, Promotion promo)
        {
            try
            {
                if (promo.UserFields == 1)
                {
                    // Un solo campo - enviar código completo
                    return _laserService.SendUserMessage(0, code);
                }
                else if (promo.UserFields == 2)
                {
                    // Dos campos - dividir según Split1 y Split2
                    if (code.Length < promo.Split1 + promo.Split2)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {code}");
                        return false;
                    }

                    string part1 = code.Substring(0, promo.Split1);
                    string part2 = code.Substring(promo.Split1, promo.Split2);

                    bool r1 = _laserService.SendUserMessage(0, part1);
                    if (!r1) return false;

                    bool r2 = _laserService.SendUserMessage(1, part2);
                    return r2;
                }
                else if (promo.UserFields == 3)
                {
                    // Tres campos
                    if (code.Length < promo.Split1 + promo.Split2 + promo.Split3)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {code}");
                        return false;
                    }

                    string part1 = code.Substring(0, promo.Split1);
                    string part2 = code.Substring(promo.Split1, promo.Split2);
                    string part3 = code.Substring(promo.Split1 + promo.Split2, promo.Split3);

                    bool r1 = _laserService.SendUserMessage(0, part1);
                    if (!r1) return false;

                    bool r2 = _laserService.SendUserMessage(1, part2);
                    if (!r2) return false;

                    bool r3 = _laserService.SendUserMessage(2, part3);
                    return r3;
                }
                else if (promo.UserFields == 4)
                {
                    // Cuatro campos
                    if (code.Length < promo.Split1 + promo.Split2 + promo.Split3 + promo.Split4)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {code}");
                        return false;
                    }

                    string part1 = code.Substring(0, promo.Split1);
                    string part2 = code.Substring(promo.Split1, promo.Split2);
                    string part3 = code.Substring(promo.Split1 + promo.Split2, promo.Split3);
                    string part4 = code.Substring(promo.Split1 + promo.Split2 + promo.Split3, promo.Split4);

                    bool r1 = _laserService.SendUserMessage(0, part1);
                    if (!r1) return false;

                    bool r2 = _laserService.SendUserMessage(1, part2);
                    if (!r2) return false;

                    bool r3 = _laserService.SendUserMessage(2, part3);
                    if (!r3) return false;

                    bool r4 = _laserService.SendUserMessage(3, part4);
                    return r4;
                }

                return false;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error al enviar código al láser: {ex.Message}");
                return false;
            }
        }
    }
}

