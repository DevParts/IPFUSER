using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de colas de códigos
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Indica si el servicio está ejecutándose
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Códigos producidos hasta el momento
        /// </summary>
        int ProducedCodes { get; }

        /// <summary>
        /// Códigos pendientes en las colas
        /// </summary>
        int PendingCodes { get; }

        /// <summary>
        /// Último código enviado
        /// </summary>
        string LastSentCode { get; }

        /// <summary>
        /// Inicia el servicio de colas
        /// </summary>
        /// <param name="totalToProduce">Total de códigos a producir</param>
        void Start(int totalToProduce);

        /// <summary>
        /// Detiene el servicio de colas
        /// </summary>
        void Stop();

        /// <summary>
        /// Evento que se dispara cuando se produce un error
        /// </summary>
        event EventHandler<string>? ErrorOccurred;

        /// <summary>
        /// Evento que se dispara cuando se envía un código
        /// </summary>
        event EventHandler<string>? CodeSent;
    }
}

