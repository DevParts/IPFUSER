using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Tag RFID leído por Speedway
    /// </summary>
    public class RfidTag
    {
        public string EPC { get; set; } = string.Empty;
        public int Antenna { get; set; }
        public int Rssi { get; set; }
        public DateTime Timestamp { get; set; }
    }
    
    /// <summary>
    /// Interfaz para el servicio de Speedway (lecturas rápidas de 40ms)
    /// </summary>
    public interface ISpeedwayService
    {
        /// <summary>
        /// Conecta al lector Speedway
        /// </summary>
        bool Connect(string ipAddress, int port = 49380);
        
        /// <summary>
        /// Desconecta del lector
        /// </summary>
        void Disconnect();
        
        /// <summary>
        /// Inicia inventario continuo (lectura rápida)
        /// </summary>
        bool StartInventory();
        
        /// <summary>
        /// Detiene inventario
        /// </summary>
        bool StopInventory();
        
        /// <summary>
        /// Obtiene tags del buffer (lectura rápida)
        /// </summary>
        List<RfidTag> GetBufferedTags();
        
        /// <summary>
        /// Limpia el buffer
        /// </summary>
        void ClearBuffer();
        
        /// <summary>
        /// Evento cuando se lee un tag (asíncrono para no bloquear)
        /// </summary>
        event EventHandler<RfidTag>? TagRead;
        
        /// <summary>
        /// Verifica si está conectado
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Configura para lectura rápida (40ms)
        /// </summary>
        void ConfigureFastReading();
    }
}

