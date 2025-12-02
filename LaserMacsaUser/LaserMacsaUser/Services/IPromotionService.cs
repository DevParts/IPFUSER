using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de promociones
    /// </summary>
    public interface IPromotionService
    {
        /// <summary>
        /// Carga una promoción por nombre de trabajo
        /// </summary>
        /// <param name="jobName">Nombre del trabajo</param>
        /// <returns>Promoción cargada o null si no existe</returns>
        Promotion? LoadPromotion(string jobName);

        /// <summary>
        /// Carga una promoción por artwork
        /// </summary>
        /// <param name="artwork">Número de artwork</param>
        /// <returns>Promoción cargada o null si no existe</returns>
        Promotion? LoadPromotionByArtwork(int artwork);

        /// <summary>
        /// Obtiene lista de trabajos disponibles
        /// </summary>
        /// <returns>Lista de nombres de trabajos</returns>
        List<string> GetAvailableJobs();

        /// <summary>
        /// Obtiene lista de artworks disponibles para un trabajo
        /// </summary>
        /// <param name="jobName">Nombre del trabajo</param>
        /// <returns>Lista de números de artwork</returns>
        List<int> GetAvailableArtworks(string jobName);

        /// <summary>
        /// Carga datos relacionados de la promoción (CodesIndex, etc.)
        /// </summary>
        /// <param name="promotion">Promoción a la que cargar datos</param>
        void LoadPromotionData(Promotion promotion);

        /// <summary>
        /// Adjunta la base de datos de códigos de la promoción si es necesario
        /// </summary>
        /// <param name="promotion">Promoción con la base de datos a adjuntar</param>
        /// <param name="dbPath">Ruta donde buscar los archivos .mdf/.ldf</param>
        /// <returns>True si se adjuntó exitosamente</returns>
        bool AttachCodesDatabase(Promotion promotion, string dbPath);

        /// <summary>
        /// Valida que un artwork exista y pertenezca a una promoción válida
        /// </summary>
        /// <param name="artwork">Número de artwork</param>
        /// <returns>ID del Job si es válido, null si no</returns>
        int? ValidateArtwork(int artwork);
    }
}

