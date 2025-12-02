using System;
using System.Threading;
using System.Threading.Tasks;
using LaserMacsaUser.Models;
using LaserMacsaUser.Services;

namespace LaserMacsaUser.Controllers
{
    /// <summary>
    /// Controller para Step 4: Start Production
    /// Orquesta todo el flujo de producción con láser y colas
    /// </summary>
    public class ProductionController
    {
        private readonly ILaserService _laserService;
        private readonly IQueueService _queueService;
        private readonly ISpeedwayService? _speedwayService;
        
        private CancellationTokenSource? _consumerCancellationToken;
        private Task? _consumerTask;
        private bool _isProducing = false;
        
        public ProductionBatch? CurrentBatch { get; private set; }
        public event EventHandler<ProductionStatusEventArgs>? StatusUpdated;
        public event EventHandler<string>? ErrorOccurred;
        public event EventHandler<LaserAlarmEventArgs>? AlarmDetected;
        
        public ProductionController(
            ILaserService laserService, 
            IQueueService queueService,
            ISpeedwayService? speedwayService = null)
        {
            _laserService = laserService;
            _queueService = queueService;
            _speedwayService = speedwayService;
            
            // Suscribirse a eventos
            _queueService.CodeProduced += OnCodeProduced;
            _laserService.AlarmDetected += LaserService_AlarmDetected;
        }
        
        /// <summary>
        /// Maneja alarmas del láser
        /// </summary>
        private void LaserService_AlarmDetected(object? sender, LaserAlarmEventArgs e)
        {
            // Disparar evento de alarma
            AlarmDetected?.Invoke(this, e);
            
            // Si la alarma requiere detener producción, hacerlo
            if (LaserAlarmService.ShouldStopProduction(e.Alarm))
            {
                ErrorOccurred?.Invoke(this, 
                    $"{e.Alarm.Code}: {e.Alarm.Description}\nSolución: {e.Alarm.Solution}");
                
                // Detener producción automáticamente en errores críticos
                if (e.Alarm.Severity == LaserAlarmService.AlarmSeverity.Critical)
                {
                    StopProduction();
                }
            }
        }
        
        /// <summary>
        /// Inicia la producción según Step 4 del manual
        /// </summary>
        public bool StartProduction(ProductionBatch batch, Promotion promotion, string laserIP, string messagePath)
        {
            if (_isProducing)
            {
                return false;
            }
            
            try
            {
                CurrentBatch = batch;
                _isProducing = true;
                
                // 1. Inicializar láser
                if (!_laserService.Initialize(laserIP, messagePath))
                {
                    ErrorOccurred?.Invoke(this, "Error al inicializar láser");
                    return false;
                }
                
                // 2. Copiar archivo de mensaje al láser
                // Usar el nombre completo del archivo (con extensión) como en el código original
                string fullMessageName = promotion.LaserFile;
                if (!_laserService.CopyMessageFile(fullMessageName, messagePath))
                {
                    string errorMsg = "Error al copiar archivo de mensaje";
                    // Intentar obtener más detalles del error
                    var copyStatus = _laserService.GetStatus();
                    if (!string.IsNullOrEmpty(copyStatus.ErrorDescription))
                    {
                        errorMsg += $": {copyStatus.ErrorDescription}";
                    }
                    ErrorOccurred?.Invoke(this, errorMsg);
                    return false;
                }
                
                // 3. Establecer mensaje por defecto
                // Pasar el nombre completo (con extensión) como en el código original
                // SetDefaultMessage intentará primero sin extensión y luego con extensión si es necesario
                if (!_laserService.SetDefaultMessage(fullMessageName))
                {
                    string errorMsg = "Error al establecer mensaje por defecto";
                    var defaultStatus = _laserService.GetStatus();
                    if (!string.IsNullOrEmpty(defaultStatus.ErrorDescription))
                    {
                        errorMsg += $": {defaultStatus.ErrorDescription}";
                    }
                    ErrorOccurred?.Invoke(this, errorMsg);
                    return false;
                }
                
                // 4. Habilitar buffer para campos de usuario
                bool isDataString = promotion.DatamatrixType >= 0;
                // Obtener tamaño del buffer desde configuración
                int bufferSize = LaserMacsaUser.Properties.Settings.Default.LaserBufferSize;
                if (bufferSize <= 0) bufferSize = 100; // Valor por defecto si no está configurado
                
                if (!_laserService.EnableBufferedUserFields(promotion.UserFields, bufferSize, isDataString))
                {
                    ErrorOccurred?.Invoke(this, "Error al habilitar buffer");
                    return false;
                }
                
                // 5. Verificar estado del láser
                var status = _laserService.GetStatus();
                if (status.State == LaserState.Errors)
                {
                    ErrorOccurred?.Invoke(this, $"Error en láser: {status.ErrorDescription}");
                    return false;
                }
                
                // 6. Iniciar sistema de colas
                _queueService.Start(promotion, batch.StoppersToProduce);
                
                // 7. Iniciar consumer (envío de códigos al láser)
                _consumerCancellationToken = new CancellationTokenSource();
                _consumerTask = Task.Run(() => ConsumerLoop(promotion, _consumerCancellationToken.Token), 
                    _consumerCancellationToken.Token);
                
                // 8. Iniciar marcado en el láser
                if (!_laserService.StartPrinting(promotion.LaserFile, 0))
                {
                    ErrorOccurred?.Invoke(this, "Error al iniciar marcado");
                    StopProduction();
                    return false;
                }
                
                // 9. Iniciar Speedway si está disponible (para lecturas rápidas de 40ms)
                if (_speedwayService != null && !_speedwayService.IsConnected)
                {
                    // Configurar Speedway para lectura rápida
                    _speedwayService.ConfigureFastReading();
                    _speedwayService.StartInventory();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, $"Error al iniciar producción: {ex.Message}");
                StopProduction();
                return false;
            }
        }
        
        /// <summary>
        /// Detiene la producción
        /// </summary>
        public void StopProduction()
        {
            if (!_isProducing) return;
            
            _isProducing = false;
            
            // Detener consumer
            _consumerCancellationToken?.Cancel();
            _consumerTask?.Wait(2000);
            _consumerCancellationToken?.Dispose();
            
            // Detener colas
            _queueService.Stop();
            
            // Detener láser
            _laserService.StopPrinting();
            
            // Detener Speedway
            _speedwayService?.StopInventory();
            
            // Actualizar batch
            if (CurrentBatch != null)
            {
                CurrentBatch.Produced = _queueService.Produced;
                CurrentBatch.Pending = _queueService.Pending;
                CurrentBatch.EndTime = DateTime.Now;
                CurrentBatch.IsActive = false;
            }
        }
        
        /// <summary>
        /// Loop del Consumer - Envía códigos al láser
        /// Optimizado para funcionar por debajo de 40ms
        /// </summary>
        private void ConsumerLoop(Promotion promotion, CancellationToken cancellationToken)
        {
            bool isDataString = promotion.DatamatrixType >= 0;
            int retryCount = 0;
            const int maxRetries = 80;
            // Obtener tamaño del buffer desde configuración
            int bufferSize = LaserMacsaUser.Properties.Settings.Default.LaserBufferSize;
            if (bufferSize <= 0) bufferSize = 100; // Valor por defecto
            
            // Usar SpinWait para esperas muy cortas (más eficiente que Thread.Sleep)
            System.Threading.SpinWait spinWait = new();
            int spinCount = 0;
            
            while (_isProducing && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Verificar buffer del láser (solo si no hay códigos en cola)
                    string? code = null;
                    byte[]? dataMatrix = null;
                    
                    // Intentar obtener código primero (más rápido)
                    if (!isDataString)
                    {
                        code = _queueService.DequeueCode();
                    }
                    else
                    {
                        dataMatrix = _queueService.DequeueDataMatrix();
                    }
                    
                    // Si no hay código disponible, usar SpinWait en lugar de Sleep
                    if (code == null && dataMatrix == null)
                    {
                        spinWait.SpinOnce();
                        spinCount++;
                        
                        // Cada 1000 spins (~1ms), verificar buffer y estado
                        if (spinCount >= 1000)
                        {
                            spinCount = 0;
                            int currentBufferCount = _laserService.GetBufferCount(isDataString);
                            
                            if (currentBufferCount >= bufferSize)
                            {
                                // Buffer lleno - esperar corto tiempo
                                retryCount++;
                                if (retryCount >= maxRetries)
                                {
                                    retryCount = 0;
                                    var status = _laserService.GetStatus();
                                    if (status.State == LaserState.Errors)
                                    {
                                        ErrorOccurred?.Invoke(this, $"Error en láser: {status.ErrorDescription}");
                                        break;
                                    }
                                }
                                // Usar Sleep solo cuando realmente es necesario (buffer lleno)
                                Thread.Sleep(10); // Reducido de 50ms a 10ms
                            }
                        }
                        continue;
                    }
                    
                    retryCount = 0;
                    spinCount = 0;
                    
                    // Verificar buffer solo antes de enviar (optimización)
                    int bufferCount = _laserService.GetBufferCount(isDataString);
                    if (bufferCount >= bufferSize)
                    {
                        // Buffer lleno - reintentar más tarde con SpinWait
                        // Re-encolar el código (si es posible) o simplemente esperar
                        Thread.Sleep(5); // Espera mínima cuando buffer está lleno
                        continue;
                    }
                    
                    // CONFIRMACIÓN: Verificar estado del láser antes de enviar
                    var laserStatus = _laserService.GetStatus();
                    if (laserStatus.State == LaserState.Errors)
                    {
                        // Error en láser - no enviar y detener producción
                        ErrorOccurred?.Invoke(this, 
                            $"Error en láser antes de enviar código: {laserStatus.ErrorDescription}");
                        break;
                    }
                    
                    if (!_laserService.IsConnected)
                    {
                        // Láser desconectado - no enviar
                        ErrorOccurred?.Invoke(this, "Láser desconectado. No se puede enviar código.");
                        break;
                    }
                    
                    // Enviar código
                    int result = 0;
                    
                    if (!isDataString && code != null)
                    {
                        // Modo texto - enviar código dividido según UserFields y Splits
                        result = _laserService.SendCodeWithSplits(
                            code, 
                            promotion.Split1, 
                            promotion.Split2, 
                            promotion.Split3, 
                            promotion.Split4, 
                            false);
                    }
                    else if (isDataString && dataMatrix != null)
                    {
                        // Modo DataMatrix
                        result = _laserService.SendDataMatrix(dataMatrix, 0);
                    }
                    
                    if (result == 0)
                    {
                        // Éxito - actualizar estado
                        UpdateStatus();
                        
                        // Verificar si se completó
                        if (_queueService.Produced >= CurrentBatch!.StoppersToProduce)
                        {
                            // Esperar a que el buffer se vacíe
                            while (_laserService.GetBufferCount(isDataString) > 0 && _isProducing)
                            {
                                Thread.Sleep(50); // Reducido de 100ms a 50ms
                            }
                            break;
                        }
                        
                        // NO esperar entre envíos exitosos - continuar inmediatamente
                        // Esto permite funcionar por debajo de 40ms
                        continue;
                    }
                    else if (result == 8)
                    {
                        // Buffer lleno - esperar corto tiempo
                        Thread.Sleep(5); // Reducido de 50ms a 5ms
                    }
                    else
                    {
                        // Error
                        ErrorOccurred?.Invoke(this, $"Error al enviar código: {result}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke(this, $"Error en ConsumerLoop: {ex.Message}");
                    break;
                }
            }
        }
        
        private void OnCodeProduced(object? sender, CodeProducedEventArgs e)
        {
            if (CurrentBatch != null)
            {
                CurrentBatch.LastCode = e.Code;
                CurrentBatch.PromotionIndex = e.Position;
            }
        }
        
        private void UpdateStatus()
        {
            if (CurrentBatch != null)
            {
                CurrentBatch.Produced = _queueService.Produced;
                CurrentBatch.Pending = _queueService.Pending;
                CurrentBatch.LastCode = _queueService.LastSentCode;
                CurrentBatch.PromotionIndex = int.TryParse(_queueService.LastSentCodePosition, out int pos) ? pos : null;
                
                StatusUpdated?.Invoke(this, new ProductionStatusEventArgs
                {
                    Produced = CurrentBatch.Produced,
                    Pending = CurrentBatch.Pending,
                    LastCode = CurrentBatch.LastCode,
                    LastCodePosition = CurrentBatch.PromotionIndex
                });
            }
        }
        
        /// <summary>
        /// Obtiene el estado actual de la producción
        /// </summary>
        public ProductionStatus GetStatus()
        {
            var laserStatus = _laserService.GetStatus();
            
            return new ProductionStatus
            {
                IsProducing = _isProducing,
                Produced = _queueService.Produced,
                Pending = _queueService.Pending,
                LaserState = laserStatus.State,
                LaserBufferCount = laserStatus.BufferFillStatus,
                HasError = _queueService.HasError || laserStatus.State == LaserState.Errors,
                ErrorMessage = laserStatus.ErrorDescription
            };
        }
    }
    
    /// <summary>
    /// Estado de producción
    /// </summary>
    public class ProductionStatus
    {
        public bool IsProducing { get; set; }
        public int Produced { get; set; }
        public int Pending { get; set; }
        public LaserState LaserState { get; set; }
        public int LaserBufferCount { get; set; }
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    /// <summary>
    /// Argumentos del evento de actualización de estado
    /// </summary>
    public class ProductionStatusEventArgs : EventArgs
    {
        public int Produced { get; set; }
        public int Pending { get; set; }
        public string? LastCode { get; set; }
        public int? LastCodePosition { get; set; }
    }
}

