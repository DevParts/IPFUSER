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
        private readonly int _bufferSize; // Tamaño del buffer del láser
        private readonly int _waitTimeBufferFull; // Tiempo de espera cuando buffer está lleno

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

        public QueueService(IDatabaseService databaseService, ILaserService laserService, Promotion promotion, int bufferSize = 100, int waitTimeBufferFull = 50)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _laserService = laserService ?? throw new ArgumentNullException(nameof(laserService));
            _promotion = promotion ?? throw new ArgumentNullException(nameof(promotion));
            _bufferSize = bufferSize;
            _waitTimeBufferFull = waitTimeBufferFull;
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

            // Esperar a que los hilos terminen (máximo 3 segundos)
            if (_producerThread != null && _producerThread.IsAlive)
            {
                _producerThread.Join(3000);
                if (_producerThread.IsAlive)
                {
                    System.Diagnostics.Debug.WriteLine("Advertencia: ProducerThread no terminó en el tiempo esperado");
                }
            }

            if (_consumerThread != null && _consumerThread.IsAlive)
            {
                _consumerThread.Join(3000);
                if (_consumerThread.IsAlive)
                {
                    System.Diagnostics.Debug.WriteLine("Advertencia: ConsumerThread no terminó en el tiempo esperado");
                }
            }

            // Limpiar colas
            lock (_queueLock)
            {
                _queue1.Clear();
                _queue2.Clear();
            }

            // Liberar recursos
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
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
                        // Validar que el código no esté vacío después de trim
                        string trimmedCode = code.Trim();
                        if (!string.IsNullOrEmpty(trimmedCode))
                        {
                            queue.Enqueue(trimmedCode);
                            // Según CAMBIOS_BD_CODIGOS.md: La tabla Codes tiene columnas Id, Code, Consumed
                            // El campo Consumed indica si el código ha sido usado (0=No, 1=Sí)
                            row["Consumed"] = 1;

                            // Capturar IDs
                            int currentId = Convert.ToInt32(row["Id"]);
                            if (idConsumedInitial == 0)
                            {
                                idConsumedInitial = currentId;
                            }
                            idConsumedFinal = currentId;
                        }
                        else
                        {
                            // Código vacío después de trim - loggear pero no agregar a la cola
                            System.Diagnostics.Debug.WriteLine($"Código vacío detectado y omitido: '{code}' (Id: {row["Id"]})");
                        }
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
            int errorCount = 0; // Contador de errores consecutivos
            const int maxErrorCount = 80; // Máximo de reintentos antes de verificar estado

            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    string? code = null;

                    // Tomar código de la cola activa (Peek en lugar de Dequeue para no perderlo si falla)
                    lock (_queueLock)
                    {
                        if (_activeQueue.Count > 0)
                        {
                            code = _activeQueue.Peek();
                        }
                    }

                    if (code != null)
                    {
                        // Verificar buffer antes de enviar (opcional, para optimización)
                        int bufferCount = _laserService.GetBufferCount();
                        if (bufferCount >= _bufferSize)
                        {
                            // Buffer lleno, esperar un poco
                            Thread.Sleep(_waitTimeBufferFull);
                            continue;
                        }

                        // Enviar al láser según UserFields
                        int result = SendCodeToLaser(code, _promotion);

                        if (result == 0)
                        {
                            // Éxito - remover código de la cola
                            lock (_queueLock)
                            {
                                if (_activeQueue.Count > 0 && _activeQueue.Peek() == code)
                                {
                                    _activeQueue.Dequeue();
                                }
                            }

                            _lastSentCode = code;
                            CodeSent?.Invoke(this, code);
                            errorCount = 0; // Resetear contador de errores
                        }
                        else if (result == 8)
                        {
                            // Buffer lleno - esperar y reintentar
                            errorCount++;
                            Thread.Sleep(_waitTimeBufferFull);

                            // Si el error se repite muchas veces, verificar estado del láser
                            if (errorCount >= maxErrorCount)
                            {
                                errorCount = 0;
                                var status = _laserService.GetStatus();
                                if (status.AlarmCode != 0)
                                {
                                    ErrorOccurred?.Invoke(this, $"Error del láser detectado. Código de alarma: {status.AlarmCode}");
                                    // No remover el código de la cola, se reintentará
                                }
                            }
                            // No remover el código de la cola, se reintentará en la siguiente iteración
                        }
                        else
                        {
                            // Otro error
                            ErrorOccurred?.Invoke(this, $"Error al enviar código: {code}. Código de error: {result}");
                            errorCount++;

                            // Remover código de la cola solo si es un error crítico
                            // (depende de la lógica de negocio)
                            if (result != -1) // -1 es error de inicialización, no remover
                            {
                                lock (_queueLock)
                                {
                                    if (_activeQueue.Count > 0 && _activeQueue.Peek() == code)
                                    {
                                        _activeQueue.Dequeue();
                                    }
                                }
                            }
                        }
                    }

                    // Alternar cola activa solo si la cola actual está vacía
                    lock (_queueLock)
                    {
                        if (_activeQueue.Count == 0)
                        {
                            _activeQueue = _activeQueue == _queue1 ? _queue2 : _queue1;
                        }
                    }

                    Thread.Sleep(10); // Pequeña espera para no sobrecargar
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Error en ConsumerLoop: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        private int SendCodeToLaser(string code, Promotion promo)
        {
            try
            {
                // VALIDACIÓN CRÍTICA: Verificar que el código no esté vacío o solo espacios
                if (string.IsNullOrWhiteSpace(code))
                {
                    ErrorOccurred?.Invoke(this, $"Intento de enviar código vacío al láser. Código: '{code}'");
                    return -1;
                }

                // Validar que el código tenga contenido válido
                string trimmedCode = code.Trim();
                if (string.IsNullOrEmpty(trimmedCode))
                {
                    ErrorOccurred?.Invoke(this, $"Código solo contiene espacios en blanco: '{code}'");
                    return -1;
                }

                if (promo.UserFields == 1)
                {
                    // Un solo campo - enviar código completo
                    return _laserService.SendUserMessage(0, trimmedCode);
                }
                else if (promo.UserFields == 2)
                {
                    // Dos campos - dividir según Split1 y Split2
                    if (trimmedCode.Length < promo.Split1 + promo.Split2)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {trimmedCode}");
                        return -1;
                    }

                    string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                    string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();

                    // VALIDAR que las partes no estén vacías
                    if (string.IsNullOrEmpty(part1) || string.IsNullOrEmpty(part2))
                    {
                        ErrorOccurred?.Invoke(this, $"Parte del código está vacía después de dividir. Part1: '{part1}', Part2: '{part2}'");
                        return -1;
                    }

                    int r1 = _laserService.SendUserMessage(0, part1);
                    if (r1 != 0) return r1;

                    int r2 = _laserService.SendUserMessage(1, part2);
                    return r2;
                }
                else if (promo.UserFields == 3)
                {
                    // Tres campos
                    if (trimmedCode.Length < promo.Split1 + promo.Split2 + promo.Split3)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {trimmedCode}");
                        return -1;
                    }

                    string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                    string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();
                    string part3 = trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3).Trim();

                    // VALIDAR que las partes no estén vacías
                    if (string.IsNullOrEmpty(part1) || string.IsNullOrEmpty(part2) || string.IsNullOrEmpty(part3))
                    {
                        ErrorOccurred?.Invoke(this, $"Parte del código está vacía después de dividir. Part1: '{part1}', Part2: '{part2}', Part3: '{part3}'");
                        return -1;
                    }

                    int r1 = _laserService.SendUserMessage(0, part1);
                    if (r1 != 0) return r1;

                    int r2 = _laserService.SendUserMessage(1, part2);
                    if (r2 != 0) return r2;

                    int r3 = _laserService.SendUserMessage(2, part3);
                    return r3;
                }
                else if (promo.UserFields == 4)
                {
                    // Cuatro campos
                    if (trimmedCode.Length < promo.Split1 + promo.Split2 + promo.Split3 + promo.Split4)
                    {
                        ErrorOccurred?.Invoke(this, $"Código demasiado corto para dividir: {trimmedCode}");
                        return -1;
                    }

                    string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                    string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();
                    string part3 = trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3).Trim();
                    string part4 = trimmedCode.Substring(promo.Split1 + promo.Split2 + promo.Split3, promo.Split4).Trim();

                    // VALIDAR que las partes no estén vacías
                    if (string.IsNullOrEmpty(part1) || string.IsNullOrEmpty(part2) || 
                        string.IsNullOrEmpty(part3) || string.IsNullOrEmpty(part4))
                    {
                        ErrorOccurred?.Invoke(this, $"Parte del código está vacía después de dividir. Part1: '{part1}', Part2: '{part2}', Part3: '{part3}', Part4: '{part4}'");
                        return -1;
                    }

                    int r1 = _laserService.SendUserMessage(0, part1);
                    if (r1 != 0) return r1;

                    int r2 = _laserService.SendUserMessage(1, part2);
                    if (r2 != 0) return r2;

                    int r3 = _laserService.SendUserMessage(2, part3);
                    if (r3 != 0) return r3;

                    int r4 = _laserService.SendUserMessage(3, part4);
                    return r4;
                }

                return -1;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error al enviar código al láser: {ex.Message}");
                return -1;
            }
        }
    }
}

