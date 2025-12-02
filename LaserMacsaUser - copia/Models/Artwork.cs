namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Modelo que representa un Artwork
    /// </summary>
    public class Artwork
    {
        public int ArtworkId { get; set; }
        public int IdJob { get; set; }
        
        public bool IsValid { get; set; }
        public string? ValidationError { get; set; }
    }
}

