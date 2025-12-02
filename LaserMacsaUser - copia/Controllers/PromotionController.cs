using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using LaserMacsaUser.Models;
using LaserMacsaUser.Services;

namespace LaserMacsaUser.Controllers
{
    /// <summary>
    /// Controller para Step 3: Confirmación de Promoción
    /// </summary>
    public class PromotionController
    {
        private readonly IDatabaseService _databaseService;
        private string _lastJobId = string.Empty;
        
        public PromotionController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        /// <summary>
        /// Carga información de la promoción basada en el artwork
        /// </summary>
        public Promotion? LoadPromotion(int artworkId)
        {
            try
            {
                // Obtener información de la promoción
                string sql = "SELECT * FROM Jobs " +
                           "INNER JOIN Artworks ON JobId=IdJob " +
                           $"WHERE Artwork={artworkId}";
                
                var reader = _databaseService.GetDataReader(sql);
                
                if (!reader.HasRows)
                {
                    reader.Close();
                    return null;
                }
                
                reader.Read();
                
                // Función helper para obtener valor de columna de forma segura
                object? GetColumnValue(string columnName, object? defaultValue = null)
                {
                    try
                    {
                        // Verificar si la columna existe
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                            {
                                var value = reader[columnName];
                                return value == DBNull.Value ? defaultValue : value;
                            }
                        }
                        return defaultValue;
                    }
                    catch
                    {
                        return defaultValue;
                    }
                }
                
                // Normalizar LaserFile: si viene como ruta completa, extraer solo el nombre
                string laserFileRaw = reader["LaserFile"].ToString() ?? string.Empty;
                string laserFileNormalized = NormalizeLaserFile(laserFileRaw);
                
                var promotion = new Promotion
                {
                    Id = Convert.ToInt32(reader["JobId"]),
                    Name = reader["JobName"].ToString() ?? string.Empty,
                    LaserFile = laserFileNormalized,  // Solo nombre del archivo
                    CodesDb = reader["CodesDb"].ToString() ?? string.Empty,
                    Split1 = Convert.ToInt32(GetColumnValue("Split1", 0) ?? 0),
                    Split2 = Convert.ToInt32(GetColumnValue("Split2", 0) ?? 0),
                    Split3 = Convert.ToInt32(GetColumnValue("Split3", 0) ?? 0),
                    Split4 = Convert.ToInt32(GetColumnValue("Split4", 0) ?? 0),
                    RecordLength = Convert.ToInt32(GetColumnValue("RecordLength", 0) ?? 0),
                    DatamatrixType = Convert.ToInt32(GetColumnValue("Datamatrix", -1) ?? -1)
                    // UserFields se calcula automáticamente basado en Split1-4
                };
                
                string jobId = reader["JobId"].ToString() ?? string.Empty;
                reader.Close();
                
                // Si cambió de promoción, desvincular base de datos anterior
                if (!string.IsNullOrEmpty(_lastJobId) && 
                    Convert.ToInt32(_lastJobId) > 0 &&
                    jobId != _lastJobId)
                {
                    DetachPreviousDatabase(_lastJobId);
                }
                
                _lastJobId = jobId;
                
                // Cargar datos relacionados
                LoadPromotionData(promotion);
                
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar promoción: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Carga datos relacionados de la promoción
        /// </summary>
        private void LoadPromotionData(Promotion promotion)
        {
            // Cargar archivos importados (CodesIndex)
            string sql = $"SELECT * FROM CodesIndex WHERE IdJob={promotion.Id}";
            promotion.FilesImported = _databaseService.GetDataTable(sql, "CodesIndex");
            
            // Cargar artworks
            sql = $"SELECT * FROM Artworks WHERE IdJob={promotion.Id}";
            promotion.Artworks = _databaseService.GetDataTable(sql, "Artworks");
            
            // Cargar datos de Jobs
            sql = $"SELECT * FROM Jobs WHERE JobId={promotion.Id}";
            promotion.Data = _databaseService.GetDataTable(sql, "Jobs");
        }
        
        /// <summary>
        /// Desvincula la base de datos de la promoción anterior
        /// </summary>
        private void DetachPreviousDatabase(string lastJobId)
        {
            try
            {
                string sql = $"SELECT CodesDB FROM Jobs WHERE JobId={lastJobId}";
                var reader = _databaseService.GetDataReader(sql);
                
                if (reader.HasRows)
                {
                    reader.Read();
                    string codesDb = reader["CodesDB"].ToString() ?? string.Empty;
                    reader.Close();
                    
                    if (!string.IsNullOrEmpty(codesDb))
                    {
                        string dbName = Path.GetFileNameWithoutExtension(codesDb);
                        _databaseService.DetachDatabase(dbName);
                    }
                }
                else
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Log error pero no bloquear el proceso
                System.Diagnostics.Debug.WriteLine($"Error al desvincular base de datos: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Normaliza LaserFile: extrae solo el nombre del archivo si viene como ruta completa
        /// </summary>
        private string NormalizeLaserFile(string laserFile)
        {
            if (string.IsNullOrEmpty(laserFile))
            {
                return string.Empty;
            }
            
            // Si es una ruta completa (tiene directorio), extraer solo el nombre
            if (Path.IsPathRooted(laserFile))
            {
                return Path.GetFileName(laserFile);
            }
            
            // Si ya es solo un nombre, retornarlo tal cual
            return laserFile;
        }
        
        /// <summary>
        /// Carga la base de datos de códigos de la promoción
        /// </summary>
        public bool LoadCodesDatabase(Promotion promotion, string dbPath)
        {
            try
            {
                if (string.IsNullOrEmpty(promotion.CodesDb))
                {
                    return false;
                }
                
                // Verificar si ya está adjunta
                if (_databaseService.IsDatabaseAttached(promotion.CodesDb))
                {
                    return true;
                }
                
                // Adjuntar base de datos
                string mdfPath = Path.Combine(dbPath, promotion.CodesDb + ".mdf");
                string ldfPath = Path.Combine(dbPath, promotion.CodesDb + "_log.ldf");
                
                if (!File.Exists(mdfPath))
                {
                    return false;
                }
                
                return _databaseService.AttachDatabase(promotion.CodesDb, mdfPath, ldfPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar base de códigos: {ex.Message}", ex);
            }
        }
    }
}

