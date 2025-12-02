using System;
using System.Data;
using System.Data.SqlClient;
using LaserMacsaUser.Models;
using LaserMacsaUser.Services;

namespace LaserMacsaUser.Controllers
{
    /// <summary>
    /// Controller para el flujo de selección de Artwork (Step 1 y 2 del manual)
    /// </summary>
    public class ArtworkController
    {
        private readonly IDatabaseService _databaseService;
        
        public ArtworkController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        /// <summary>
        /// Step 1: Primera entrada de Artwork ID
        /// Valida que el artwork exista y pertenezca a una promoción válida
        /// </summary>
        public Artwork ValidateFirstArtwork(int artworkId)
        {
            var artwork = new Artwork { ArtworkId = artworkId };
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"Validando artwork: {artworkId}");
                
                // Buscar artwork en base de datos
                string sql = $"SELECT * FROM Artworks WHERE Artwork={artworkId}";
                System.Diagnostics.Debug.WriteLine($"Ejecutando SQL: {sql}");
                
                SqlDataReader? reader = null;
                try
                {
                    reader = _databaseService.GetDataReader(sql);
                    
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int idJob = Convert.ToInt32(reader["IdJob"]);
                        System.Diagnostics.Debug.WriteLine($"Artwork encontrado. IdJob: {idJob}");
                        
                        // Cerrar el reader antes de hacer otra consulta
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                        
                        // Validar que pertenezca a una promoción válida
                        string? validationError = null;
                        bool isValid = IsValidPromotion(idJob, out validationError);
                        
                        if (isValid)
                        {
                            artwork.IdJob = idJob;
                            artwork.IsValid = true;
                            System.Diagnostics.Debug.WriteLine($"Artwork válido. IdJob: {idJob}");
                        }
                        else
                        {
                            artwork.IsValid = false;
                            artwork.ValidationError = validationError ?? "Artwork no válido para esta promoción";
                            System.Diagnostics.Debug.WriteLine($"Artwork inválido: {artwork.ValidationError}");
                        }
                    }
                    else
                    {
                        artwork.IsValid = false;
                        artwork.ValidationError = $"Artwork no encontrado: {artworkId}";
                        System.Diagnostics.Debug.WriteLine($"Artwork no encontrado en base de datos: {artworkId}");
                    }
                }
                finally
                {
                    reader?.Close();
                    reader?.Dispose();
                }
            }
            catch (Exception ex)
            {
                artwork.IsValid = false;
                artwork.ValidationError = $"Error al validar artwork: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Excepción al validar artwork: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            return artwork;
        }
        
        /// <summary>
        /// Step 2: Segunda entrada de Artwork ID (confirmación)
        /// Compara con la primera entrada
        /// </summary>
        public bool ValidateSecondArtwork(int firstArtworkId, int secondArtworkId)
        {
            return firstArtworkId == secondArtworkId;
        }
        
        /// <summary>
        /// Valida que una promoción tenga todos los campos requeridos
        /// </summary>
        private bool IsValidPromotion(int jobId, out string? errorMessage)
        {
            errorMessage = null;
            
            try
            {
                System.Diagnostics.Debug.WriteLine($"Validando promoción JobId: {jobId}");
                string sql = $"SELECT * FROM Jobs WHERE JobId={jobId}";
                
                // Usar GetDataTable en lugar de GetDataReader para evitar conflictos de conexión
                var table = _databaseService.GetDataTable(sql, "Jobs");
                
                if (table.Rows.Count == 0)
                {
                    errorMessage = $"Promoción no encontrada (JobId: {jobId})";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                var row = table.Rows[0];
                
                // Validar campos requeridos
                if (row["RecordLength"] == DBNull.Value)
                {
                    errorMessage = "La promoción no tiene RecordLength definido";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                if (row["Split1"] == DBNull.Value ||
                    row["Split2"] == DBNull.Value ||
                    row["Split3"] == DBNull.Value ||
                    row["Split4"] == DBNull.Value)
                {
                    errorMessage = "La promoción no tiene todos los campos Split definidos (Split1, Split2, Split3, Split4)";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                // Validar que la suma de splits coincida con RecordLength
                int recordLength = Convert.ToInt32(row["RecordLength"]);
                int split1 = Convert.ToInt32(row["Split1"]);
                int split2 = Convert.ToInt32(row["Split2"]);
                int split3 = Convert.ToInt32(row["Split3"]);
                int split4 = Convert.ToInt32(row["Split4"]);
                int sumSplits = split1 + split2 + split3 + split4;
                
                System.Diagnostics.Debug.WriteLine($"RecordLength: {recordLength}, Splits: {split1}+{split2}+{split3}+{split4}={sumSplits}");
                
                if (recordLength == 0)
                {
                    errorMessage = "RecordLength no puede ser 0";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                if (sumSplits != recordLength)
                {
                    errorMessage = $"La suma de los Splits ({sumSplits}) no coincide con RecordLength ({recordLength})";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                // Validar que tenga archivo láser
                if (row["LaserFile"] == DBNull.Value || string.IsNullOrEmpty(row["LaserFile"]?.ToString()))
                {
                    errorMessage = "La promoción no tiene archivo láser (LaserFile) definido";
                    System.Diagnostics.Debug.WriteLine(errorMessage);
                    return false;
                }
                
                System.Diagnostics.Debug.WriteLine($"Promoción válida. JobId: {jobId}, LaserFile: {row["LaserFile"]}");
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al validar promoción: {ex.Message}";
                System.Diagnostics.Debug.WriteLine(errorMessage);
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}

