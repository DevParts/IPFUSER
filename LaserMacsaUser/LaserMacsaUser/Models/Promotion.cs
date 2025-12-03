using System.Collections.Generic;
using System.Text;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Representa una promoción/trabajo de marcado láser (basado en Promotion del sistema original)
    /// </summary>
    public class Promotion
    {
        /// <summary>
        /// ID del trabajo
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Nombre del trabajo
        /// </summary>
        public string JobName { get; set; } = string.Empty;

        /// <summary>
        /// Archivo láser asociado
        /// </summary>
        public string LaserFile { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la base de datos de códigos (sin extensión)
        /// </summary>
        public string CodesDb { get; set; } = string.Empty;

        /// <summary>
        /// Número de capas (1-25)
        /// </summary>
        public int Layers { get; set; }

        /// <summary>
        /// Elementos por ciclo
        /// </summary>
        public int CycleElements { get; set; }

        /// <summary>
        /// Número de campos de usuario (1-4)
        /// </summary>
        public int UserFields { get; set; }

        /// <summary>
        /// Longitud de división 1
        /// </summary>
        public int Split1 { get; set; }

        /// <summary>
        /// Longitud de división 2
        /// </summary>
        public int Split2 { get; set; }

        /// <summary>
        /// Longitud de división 3
        /// </summary>
        public int Split3 { get; set; }

        /// <summary>
        /// Longitud de división 4
        /// </summary>
        public int Split4 { get; set; }

        /// <summary>
        /// Tipo de DataMatrix (-1 si no aplica)
        /// </summary>
        public int DatamatrixType { get; set; } = -1;

        /// <summary>
        /// Total de códigos disponibles
        /// </summary>
        public int TotalCodes { get; set; }

        /// <summary>
        /// Códigos consumidos
        /// </summary>
        public int ConsumedCodes { get; set; }

        /// <summary>
        /// Archivos de códigos importados (CodesIndex)
        /// </summary>
        public List<CodeFileInfo> FilesImported { get; set; } = new List<CodeFileInfo>();

        /// <summary>
        /// Cantidades por capa (LayerQty1 - LayerQty25)
        /// </summary>
        public int[] LayerQty { get; set; } = new int[25];

        /// <summary>
        /// Uso de códigos por capa (LayerUseCodes1 - LayerUseCodes25)
        /// </summary>
        public int[] LayerUseCodes { get; set; } = new int[25];

        /// <summary>
        /// Indica si es absoluta o relativa
        /// </summary>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// Genera SQL para obtener códigos disponibles (basado en GetSqlCodes del sistema original)
        /// </summary>
        /// <param name="records">Número máximo de registros a obtener</param>
        /// <returns>Consulta SQL</returns>
        public string GetSqlCodes(int records)
        {
            if (FilesImported.Count == 0)
            {
                return $"SELECT TOP {records} * FROM Codes WHERE Sent=0";
            }

            var sql = new StringBuilder($"SELECT TOP {records} * FROM Codes WHERE Sent=0 AND (");

            for (int i = 0; i < FilesImported.Count; i++)
            {
                var file = FilesImported[i];
                if (i == FilesImported.Count - 1)
                {
                    sql.Append($"(Id >= {file.FromRecord} AND Id <= {file.ToRecord})");
                }
                else
                {
                    sql.Append($"(Id >= {file.FromRecord} AND Id <= {file.ToRecord}) OR ");
                }
            }

            sql.Append(")");
            return sql.ToString();
        }

        /// <summary>
        /// Calcula el número de campos de usuario basado en los splits
        /// </summary>
        public void CalculateUserFields()
        {
            UserFields = 0;
            if (Split1 > 0) UserFields++;
            if (Split2 > 0) UserFields++;
            if (Split3 > 0) UserFields++;
            if (Split4 > 0) UserFields++;
        }
    }
}

