using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio simplificado de Speedway para lecturas rápidas (40ms)
    /// Adaptado para .NET 8
    /// </summary>
    public class SpeedwayService : ISpeedwayService
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private readonly List<RfidTag> _buffer = new();
        private readonly object _bufferLock = new();
        private bool _isInventoryRunning = false;
        private CancellationTokenSource? _cancellationTokenSource;
        private Task? _inventoryTask;
        
        public bool IsConnected => _tcpClient?.Connected ?? false;
        
        public event EventHandler<RfidTag>? TagRead;
        
        public bool Connect(string ipAddress, int port = 49380)
        {
            try
            {
                Disconnect();
                
                _tcpClient = new TcpClient();
                _tcpClient.Connect(ipAddress, port);
                _networkStream = _tcpClient.GetStream();
                _networkStream.ReadTimeout = 300; // 300ms timeout para lecturas rápidas
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error conectando Speedway: {ex.Message}");
                return false;
            }
        }
        
        public void Disconnect()
        {
            StopInventory();
            
            _networkStream?.Close();
            _networkStream?.Dispose();
            _tcpClient?.Close();
            _tcpClient?.Dispose();
            
            _networkStream = null;
            _tcpClient = null;
            
            ClearBuffer();
        }
        
        public void ConfigureFastReading()
        {
            // Configuración para lecturas rápidas de 40ms
            // Timeouts cortos, buffer habilitado
            if (_networkStream != null)
            {
                _networkStream.ReadTimeout = 40; // 40ms para lecturas rápidas
                _networkStream.WriteTimeout = 40;
            }
        }
        
        public bool StartInventory()
        {
            if (!IsConnected || _isInventoryRunning) return false;
            
            try
            {
                _isInventoryRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();
                
                // Iniciar tarea asíncrona para lectura continua
                _inventoryTask = Task.Run(() => InventoryLoop(_cancellationTokenSource.Token), 
                    _cancellationTokenSource.Token);
                
                return true;
            }
            catch
            {
                _isInventoryRunning = false;
                return false;
            }
        }
        
        public bool StopInventory()
        {
            if (!_isInventoryRunning) return false;
            
            try
            {
                _isInventoryRunning = false;
                _cancellationTokenSource?.Cancel();
                _inventoryTask?.Wait(1000);
                _cancellationTokenSource?.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void InventoryLoop(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[2048];
            
            while (!cancellationToken.IsCancellationRequested && _isInventoryRunning)
            {
                try
                {
                    if (_networkStream == null || !_networkStream.DataAvailable)
                    {
                        Thread.Sleep(1); // Esperar 1ms antes de verificar de nuevo
                        continue;
                    }
                    
                    // Leer datos disponibles (máximo 40ms)
                    int bytesRead = _networkStream.Read(buffer, 0, buffer.Length);
                    
                    if (bytesRead > 0)
                    {
                        // Procesar tags leídos (simplificado - aquí iría el protocolo completo)
                        ProcessInventoryData(buffer, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error en InventoryLoop: {ex.Message}");
                    }
                    Thread.Sleep(10);
                }
            }
        }
        
        private void ProcessInventoryData(byte[] data, int length)
        {
            // Aquí se procesaría el protocolo completo de Speedway
            // Por ahora, creamos un tag de ejemplo para la estructura
            // NOTA: Esto necesita implementación completa del protocolo Speedway
            
            var tag = new RfidTag
            {
                EPC = Convert.ToHexString(data, 0, Math.Min(12, length)), // Simplificado
                Antenna = 1,
                Rssi = -60,
                Timestamp = DateTime.Now
            };
            
            // Agregar al buffer
            lock (_bufferLock)
            {
                // Evitar duplicados
                if (!_buffer.Exists(t => t.EPC == tag.EPC))
                {
                    _buffer.Add(tag);
                }
            }
            
            // Disparar evento (asíncrono para no bloquear)
            TagRead?.Invoke(this, tag);
        }
        
        public List<RfidTag> GetBufferedTags()
        {
            lock (_bufferLock)
            {
                var tags = new List<RfidTag>(_buffer);
                return tags;
            }
        }
        
        public void ClearBuffer()
        {
            lock (_bufferLock)
            {
                _buffer.Clear();
            }
        }
    }
}

