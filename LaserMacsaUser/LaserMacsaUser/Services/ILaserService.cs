using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de comunicación con el láser
    /// </summary>
    public interface ILaserService
    {
        /// <summary>
        /// Indica si el láser está conectado
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Inicializa la conexión con el láser
        /// </summary>
        /// <param name="ipAddress">Dirección IP del láser</param>
        /// <param name="messagePath">Ruta local (por defecto ".\\")</param>
        /// <returns>True si la inicialización fue exitosa</returns>
        bool Initialize(string ipAddress, string messagePath = ".\\");

        /// <summary>
        /// Obtiene el estado actual del láser
        /// </summary>
        LaserStatus GetStatus();

        /// <summary>
        /// Inicia la impresión con un archivo específico
        /// </summary>
        /// <param name="filename">Nombre del archivo (sin extensión)</param>
        /// <param name="copies">Número de copias</param>
        /// <returns>True si el comando fue exitoso</returns>
        bool StartPrint(string filename, int copies);

        /// <summary>
        /// Envía un mensaje de usuario a un campo específico (alta velocidad UTF-8)
        /// </summary>
        /// <param name="fieldIndex">Índice del campo (0-3)</param>
        /// <param name="message">Mensaje a enviar</param>
        /// <returns>0 = éxito, 8 = buffer lleno, otros = error</returns>
        int SendUserMessage(int fieldIndex, string message);

        /// <summary>
        /// Detiene la impresión
        /// </summary>
        void Stop();

        /// <summary>
        /// Obtiene el número de elementos en el buffer del láser
        /// </summary>
        int GetBufferCount();

        /// <summary>
        /// Obtiene el último error ocurrido
        /// </summary>
        string GetLastError();

        /// <summary>
        /// Copia un archivo de mensaje (.msf) desde la PC al disco duro del láser
        /// </summary>
        /// <param name="messageName">Nombre del archivo (con extensión, ej: "88888888.msf")</param>
        /// <param name="messagePath">Ruta local donde está el archivo (ej: "C:\IPFEu\")</param>
        /// <returns>True si la copia fue exitosa</returns>
        bool CopyMessageFile(string messageName, string messagePath);

        /// <summary>
        /// Establece un archivo de mensaje como el mensaje por defecto en el láser
        /// </summary>
        /// <param name="messageName">Nombre del archivo (con o sin extensión)</param>
        /// <returns>True si se estableció correctamente</returns>
        bool SetDefaultMessage(string messageName);

        /// <summary>
        /// Evento que se dispara cuando se detecta una alarma
        /// </summary>
        event EventHandler<LaserAlarmEventArgs>? AlarmDetected;
    }

    /// <summary>
    /// Argumentos del evento de alarma del láser
    /// </summary>
    public class LaserAlarmEventArgs : EventArgs
    {
        public int AlarmCode { get; set; }
        public string AlarmDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

