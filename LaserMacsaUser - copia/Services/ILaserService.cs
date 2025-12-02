using LaserMacsaUser.Models;
using System.Threading.Tasks;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de comunicación con el láser
    /// </summary>
    public interface ILaserService
    {
        /// <summary>
        /// Inicializa conexión con el láser
        /// </summary>
        bool Initialize(string ipAddress, string messagePath);
        
        /// <summary>
        /// Copia archivo de mensaje al láser
        /// </summary>
        bool CopyMessageFile(string messageName, string messagePath);
        
        /// <summary>
        /// Establece mensaje por defecto
        /// </summary>
        bool SetDefaultMessage(string messageName);
        
        /// <summary>
        /// Habilita buffer para campos de usuario
        /// </summary>
        bool EnableBufferedUserFields(int userFields, int bufferSize, bool isDataString);
        
        /// <summary>
        /// Inicia marcado
        /// </summary>
        bool StartPrinting(string messageName, int copies = 0);
        
        /// <summary>
        /// Detiene marcado
        /// </summary>
        bool StopPrinting(int timeout = 2000);
        
        /// <summary>
        /// Envía código al láser (modo rápido UTF-8)
        /// </summary>
        int SendCode(string code, int field = 0);
        
        /// <summary>
        /// Envía DataMatrix al láser
        /// </summary>
        int SendDataMatrix(byte[] dataMatrix, int field = 0);
        
        /// <summary>
        /// Envía un código dividido según los splits de la promoción
        /// Divide el código según Split1, Split2, Split3, Split4 y envía cada parte al campo correspondiente
        /// </summary>
        int SendCodeWithSplits(string code, int split1, int split2, int split3, int split4, bool isDataString = false);
        
        /// <summary>
        /// Obtiene estado del láser
        /// </summary>
        LaserStatus GetStatus();
        
        /// <summary>
        /// Obtiene cantidad de códigos en buffer
        /// </summary>
        int GetBufferCount(bool isDataString = false);
        
        /// <summary>
        /// Limpia buffer
        /// </summary>
        bool ClearBuffer();
        
        /// <summary>
        /// Cierra conexión
        /// </summary>
        void Disconnect();
        
        /// <summary>
        /// Verifica si está conectado
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// Evento cuando se detecta una alarma
        /// </summary>
        event EventHandler<LaserAlarmEventArgs>? AlarmDetected;
    }
    
    /// <summary>
    /// Argumentos del evento de alarma
    /// </summary>
    public class LaserAlarmEventArgs : EventArgs
    {
        public LaserAlarmService.AlarmInfo Alarm { get; set; } = null!;
        public LaserStatus Status { get; set; } = null!;
    }
}

