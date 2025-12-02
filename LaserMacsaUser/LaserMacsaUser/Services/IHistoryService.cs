using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Interfaz para el servicio de generación de histórico
    /// </summary>
    public interface IHistoryService
    {
        /// <summary>
        /// Genera un registro histórico de producción
        /// </summary>
        /// <param name="initRecord">Registro inicial</param>
        /// <param name="finalRecord">Registro final</param>
        /// <param name="promotion">Promoción utilizada</param>
        /// <param name="pedido">Número de pedido</param>
        /// <param name="artwork">Artwork utilizado</param>
        /// <param name="drive">Unidad donde se encuentra la BD</param>
        void GenerateHistoric(int initRecord, int finalRecord, Promotion promotion, string pedido, string artwork, string drive);
    }
}

