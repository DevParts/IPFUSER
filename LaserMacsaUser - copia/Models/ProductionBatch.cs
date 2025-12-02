using System;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Modelo que representa un lote de producción
    /// </summary>
    public class ProductionBatch
    {
        public string Order { get; set; } = string.Empty; // 11 caracteres exactos
        public int StoppersToProduce { get; set; } // Cantidad a producir
        public int Produced { get; set; } // Cantidad producida
        public int Pending { get; set; } // Cantidad pendiente
        public string? LastCode { get; set; } // Último código enviado
        public int? PromotionIndex { get; set; } // Índice en el archivo de texto
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}

