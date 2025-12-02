using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de colas optimizado para lecturas rápidas (40ms)
    /// Implementa patrón Producer-Consumer con doble cola
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly IDatabaseService _databaseService;
        private readonly Queue<string> _queue1 = new();
        private readonly Queue<string> _queue2 = new();
        private readonly Queue<byte[]> _dataMatrixQueue1 = new();
        private readonly Queue<byte[]> _dataMatrixQueue2 = new();
        
        private Queue<string> _activeQueue = null!;
        private Queue<byte[]> _activeDataMatrixQueue = null!;
        
        private Promotion? _promotion;
        private int _totalToProduce;
        private int _produced = 0;
        private string? _lastSentCode;
        private string? _lastSentCodePosition;
        private bool _isRunning = false;
        private bool _hasError = false;
        
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _producerTask;
        private readonly object _queueLock = new();
        
        public int Produced => _produced;
        public int Pending => _totalToProduce - _produced;
        public string? LastSentCode => _lastSentCode;
        public string? LastSentCodePosition => _lastSentCodePosition;
        public bool HasError => _hasError;
        
        public event EventHandler<CodeProducedEventArgs>? CodeProduced;
        
        public QueueService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        public void Start(Promotion promotion, int totalToProduce)
        {
            if (_isRunning) Stop();
            
            _promotion = promotion;
            _totalToProduce = totalToProduce;
            _produced = 0;
            _hasError = false;
            _isRunning = true;
            
            // Seleccionar cola activa
            if (promotion.DatamatrixType < 0)
            {
                _activeQueue = _queue1;
            }
            else
            {
                _activeDataMatrixQueue = _dataMatrixQueue1;
            }
            
            // Iniciar producer
            _cancellationTokenSource = new CancellationTokenSource();
            _producerTask = Task.Run(() => ProducerLoop(_cancellationTokenSource.Token), 
                _cancellationTokenSource.Token);
        }
        
        public void Stop()
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _producerTask?.Wait(2000);
            _cancellationTokenSource?.Dispose();
            
            lock (_queueLock)
            {
                _queue1.Clear();
                _queue2.Clear();
                _dataMatrixQueue1.Clear();
                _dataMatrixQueue2.Clear();
            }
        }
        
        public string? DequeueCode()
        {
            if (_promotion == null || _promotion.DatamatrixType >= 0)
            {
                return null; // No es modo texto
            }
            
            lock (_queueLock)
            {
                // Intentar obtener de la cola activa
                if (_activeQueue.Count > 0)
                {
                    string code = _activeQueue.Dequeue();
                    _lastSentCode = code;
                    return code;
                }
                
                // Cambiar de cola si está vacía
                if (_activeQueue == _queue1 && _queue2.Count > 0)
                {
                    _activeQueue = _queue2;
                    return _activeQueue.Dequeue();
                }
                else if (_activeQueue == _queue2 && _queue1.Count > 0)
                {
                    _activeQueue = _queue1;
                    return _activeQueue.Dequeue();
                }
            }
            
            return null; // No hay códigos disponibles
        }
        
        public byte[]? DequeueDataMatrix()
        {
            if (_promotion == null || _promotion.DatamatrixType < 0)
            {
                return null; // No es modo DataMatrix
            }
            
            lock (_queueLock)
            {
                if (_activeDataMatrixQueue.Count > 0)
                {
                    return _activeDataMatrixQueue.Dequeue();
                }
                
                // Cambiar de cola
                if (_activeDataMatrixQueue == _dataMatrixQueue1 && _dataMatrixQueue2.Count > 0)
                {
                    _activeDataMatrixQueue = _dataMatrixQueue2;
                    return _activeDataMatrixQueue.Dequeue();
                }
                else if (_activeDataMatrixQueue == _dataMatrixQueue2 && _dataMatrixQueue1.Count > 0)
                {
                    _activeDataMatrixQueue = _dataMatrixQueue1;
                    return _activeDataMatrixQueue.Dequeue();
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Loop del Producer - Carga códigos de la BD y los agrega a las colas
        /// Optimizado para 40ms
        /// </summary>
        private void ProducerLoop(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested && _produced < _totalToProduce)
            {
                try
                {
                    // Calcular cuántos registros cargar (máximo 50 por lote)
                    int remaining = _totalToProduce - _produced;
                    int records = Math.Min(remaining, 50);
                    
                    if (records <= 0) break;
                    
                    // Obtener códigos de la base de datos
                    string sql = _promotion!.GetSqlCodes(records);
                    DataTable sqlTable = _databaseService.GetDataTable(sql, "Codes");
                    
                    if (sqlTable.Rows.Count == 0)
                    {
                        Thread.Sleep(10); // Reducido de 50ms a 10ms para optimizar
                        continue;
                    }
                    
                    int idFinal = 0;
                    Queue<string> targetQueue = _activeQueue == _queue1 ? _queue2 : _queue1;
                    Queue<byte[]> targetDataMatrixQueue = _activeDataMatrixQueue == _dataMatrixQueue1 ? 
                        _dataMatrixQueue2 : _dataMatrixQueue1;
                    
                    lock (_queueLock)
                    {
                        foreach (DataRow row in sqlTable.Rows)
                        {
                            string code = row["Code"].ToString() ?? string.Empty;
                            
                            if (_promotion.DatamatrixType < 0)
                            {
                                // Modo texto
                                targetQueue.Enqueue(code);
                            }
                            else
                            {
                                // Modo DataMatrix - aquí se calcularía el DataMatrix
                                // Por ahora, simplificado
                                byte[] dataMatrix = System.Text.Encoding.UTF8.GetBytes(code);
                                targetDataMatrixQueue.Enqueue(dataMatrix);
                            }
                            
                            // Marcar como enviado
                            row["Sent"] = 1;
                            row["TimeStamp"] = DateTime.Now;
                            
                            idFinal = Convert.ToInt32(row["Id"]);
                            _lastSentCodePosition = idFinal.ToString();
                        }
                    }
                    
                    // Actualizar base de datos
                    _databaseService.UpdateTable(sqlTable);
                    
                    // Actualizar contadores
                    Interlocked.Add(ref _produced, sqlTable.Rows.Count);
                    
                    // Disparar eventos
                    foreach (DataRow row in sqlTable.Rows)
                    {
                        string code = row["Code"].ToString() ?? string.Empty;
                        int position = Convert.ToInt32(row["Id"]);
                        CodeProduced?.Invoke(this, new CodeProducedEventArgs 
                        { 
                            Code = code, 
                            Position = position 
                        });
                    }
                    
                    // Alternar cola activa
                    if (_promotion.DatamatrixType < 0)
                    {
                        _activeQueue = targetQueue;
                    }
                    else
                    {
                        _activeDataMatrixQueue = targetDataMatrixQueue;
                    }
                    
                    // Esperar mínimo tiempo antes de cargar más (optimizado para <40ms)
                    // Solo esperar si las colas están llenas
                    int queueCount = _promotion.DatamatrixType < 0 ? 
                        (_activeQueue?.Count ?? 0) : 
                        (_activeDataMatrixQueue?.Count ?? 0);
                    
                    if (queueCount > 20) // Si hay más de 20 códigos en cola, esperar un poco
                    {
                        Thread.Sleep(10); // Reducido de 50ms a 10ms
                    }
                    else
                    {
                        Thread.Sleep(1); // Espera mínima si la cola no está llena
                    }
                }
                catch (Exception ex)
                {
                    _hasError = true;
                    System.Diagnostics.Debug.WriteLine($"Error en ProducerLoop: {ex.Message}");
                    Thread.Sleep(1000); // Esperar más tiempo en caso de error
                }
            }
        }
    }
}

