namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Estado del láser
    /// </summary>
    public enum LaserState
    {
        Stopped,
        Marking,
        Errors,
        Warning
    }
    
    /// <summary>
    /// Modelo de estado del láser
    /// </summary>
    public class LaserStatus
    {
        public LaserState State { get; set; }
        public uint OkPrints { get; set; } // d_counter
        public uint NokPrints { get; set; } // s_counter
        public uint TotalPrints { get; set; } // t_counter
        public uint ErrorCode { get; set; }
        public string ErrorDescription { get; set; } = string.Empty;
        public string MessageName { get; set; } = string.Empty;
        public bool IsPrinting { get; set; }
        public int BufferFillStatus { get; set; } // Cantidad de códigos en buffer
    }
}

