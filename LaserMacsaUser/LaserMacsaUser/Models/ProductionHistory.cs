using System;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Registro histórico de producción (basado en tabla Historico)
    /// </summary>
    public class ProductionHistory
    {
        /// <summary>
        /// ID del registro histórico
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Número de pedido
        /// </summary>
        public string Pedido { get; set; } = string.Empty;

        /// <summary>
        /// Artwork utilizado
        /// </summary>
        public string Artwork { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del fichero
        /// </summary>
        public string Fichero { get; set; } = string.Empty;

        /// <summary>
        /// Registro inicial
        /// </summary>
        public int Desde { get; set; }

        /// <summary>
        /// Registro final
        /// </summary>
        public int Hasta { get; set; }

        /// <summary>
        /// Volumen (etiqueta de unidad)
        /// </summary>
        public string Volumen { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp de la producción
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Sesión de producción
        /// </summary>
        public string Sesion { get; set; } = string.Empty;

        /// <summary>
        /// Número de capas
        /// </summary>
        public int Layers { get; set; }

        /// <summary>
        /// Cantidades por capa (LayerQty1 - LayerQty25)
        /// </summary>
        public int[] LayerQty { get; set; } = new int[25];
    }
}

