using System;
using System.Collections.Generic;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de colas (Producer-Consumer optimizado para 40ms)
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Inicia el sistema de colas
        /// </summary>
        void Start(Promotion promotion, int totalToProduce);
        
        /// <summary>
        /// Detiene el sistema de colas
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Obtiene el siguiente código de la cola (Consumer)
        /// </summary>
        string? DequeueCode();
        
        /// <summary>
        /// Obtiene el siguiente DataMatrix de la cola (Consumer)
        /// </summary>
        byte[]? DequeueDataMatrix();
        
        /// <summary>
        /// Cantidad de códigos producidos
        /// </summary>
        int Produced { get; }
        
        /// <summary>
        /// Cantidad de códigos pendientes
        /// </summary>
        int Pending { get; }
        
        /// <summary>
        /// Último código enviado
        /// </summary>
        string? LastSentCode { get; }
        
        /// <summary>
        /// Posición del último código enviado
        /// </summary>
        string? LastSentCodePosition { get; }
        
        /// <summary>
        /// Indica si hay error en la cola
        /// </summary>
        bool HasError { get; }
        
        /// <summary>
        /// Evento cuando se produce un código
        /// </summary>
        event EventHandler<CodeProducedEventArgs>? CodeProduced;
    }
    
    /// <summary>
    /// Argumentos del evento de código producido
    /// </summary>
    public class CodeProducedEventArgs : EventArgs
    {
        public string Code { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}

