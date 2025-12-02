using System;
using System.Data;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Modelo que representa una Promoción/Job
    /// </summary>
    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LaserFile { get; set; } = string.Empty;
        public string CodesDb { get; set; } = string.Empty;
        public int Split1 { get; set; }
        public int Split2 { get; set; }
        public int Split3 { get; set; }
        public int Split4 { get; set; }
        public int DatamatrixType { get; set; } // -1 = texto, >=0 = DataMatrix
        public int RecordLength { get; set; }
        
        /// <summary>
        /// Calcula UserFields basado en cuántos campos Split tienen valor > 0
        /// Según userfield.md: UserFields = cantidad de Split > 0 (1-4)
        /// </summary>
        public int UserFields
        {
            get
            {
                int count = 0;
                if (Split1 > 0) count++;
                if (Split2 > 0) count++;
                if (Split3 > 0) count++;
                if (Split4 > 0) count++;
                return count > 0 ? count : 1; // Mínimo 1 campo
            }
        }
        
        // Datos relacionados
        public DataTable? FilesImported { get; set; } // CodesIndex
        public DataTable? Artworks { get; set; }
        public DataTable? Data { get; set; } // Jobs table
        
        // Propiedades calculadas
        public int TotalCodes
        {
            get
            {
                if (FilesImported == null) return 0;
                int total = 0;
                foreach (DataRow row in FilesImported.Rows)
                {
                    total += Convert.ToInt32(row["TotalCodes"] ?? 0);
                }
                return total;
            }
        }
        
        public int ConsumedCodes
        {
            get
            {
                if (FilesImported == null) return 0;
                int consumed = 0;
                foreach (DataRow row in FilesImported.Rows)
                {
                    consumed += Convert.ToInt32(row["Consumed"] ?? 0);
                }
                return consumed;
            }
        }
        
        /// <summary>
        /// Genera query SQL para obtener códigos disponibles
        /// </summary>
        public string GetSqlCodes(int records)
        {
            if (FilesImported == null || FilesImported.Rows.Count == 0)
            {
                return $"SELECT TOP {records} * FROM Codes WHERE Sent=0 ORDER BY Id";
            }
            
            string sql = $"SELECT TOP {records} * FROM Codes WHERE Sent=0 AND (";
            
            for (int i = 0; i < FilesImported.Rows.Count; i++)
            {
                int fromRecord = Convert.ToInt32(FilesImported.Rows[i]["FromRecord"]);
                int toRecord = Convert.ToInt32(FilesImported.Rows[i]["ToRecord"]);
                
                if (i < FilesImported.Rows.Count - 1)
                {
                    sql += $"(Id >= {fromRecord} AND Id <= {toRecord}) OR ";
                }
                else
                {
                    sql += $"(Id >= {fromRecord} AND Id <= {toRecord})";
                }
            }
            
            return sql + ") ORDER BY Id";
        }
    }
}

