namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Información de un archivo de códigos importado (basado en CodesIndex)
    /// </summary>
    public class CodeFileInfo
    {
        /// <summary>
        /// ID del archivo en CodesIndex
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del Job asociado
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Nombre del archivo
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Registro inicial (Id mínimo)
        /// </summary>
        public int FromRecord { get; set; }

        /// <summary>
        /// Registro final (Id máximo)
        /// </summary>
        public int ToRecord { get; set; }

        /// <summary>
        /// Total de códigos en el archivo
        /// </summary>
        public int TotalCodes { get; set; }

        /// <summary>
        /// Códigos consumidos
        /// </summary>
        public int Consumed { get; set; }
    }
}

