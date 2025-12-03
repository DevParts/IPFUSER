using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio para gestión de promociones (basado en Promotion.Load() del sistema original)
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly IDatabaseService _databaseService;
        private string _lastJobId = string.Empty;

        public PromotionService(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public Promotion? LoadPromotion(string jobName)
        {
            try
            {
                string sql = $"SELECT * FROM Jobs WHERE JobName='{jobName.Replace("'", "''")}'";
                DataTable jobsTable = _databaseService.GetDataTable(sql, "Jobs");

                if (jobsTable.Rows.Count == 0)
                {
                    return null;
                }

                DataRow jobRow = jobsTable.Rows[0];
                int jobId = GetIntValue(jobRow["JobId"]);

                // Si cambió de promoción, desadjuntar base de datos anterior
                if (!string.IsNullOrEmpty(_lastJobId) && _lastJobId != jobId.ToString())
                {
                    DetachPreviousDatabase(_lastJobId);
                }

                _lastJobId = jobId.ToString();

                // Crear promoción
                var promotion = new Promotion
                {
                    JobId = jobId,
                    JobName = GetStringValue(jobRow["JobName"]),
                    LaserFile = GetStringValue(jobRow["LaserFile"]),
                    CodesDb = GetCodesDbName(GetStringValue(jobRow["CodesDb"])),
                    Layers = GetIntValue(jobRow["Layers"]),
                    CycleElements = GetIntValue(jobRow["CycleElements"]),
                    Split1 = GetIntValue(jobRow["Split1"]),
                    Split2 = GetIntValue(jobRow["Split2"]),
                    Split3 = GetIntValue(jobRow["Split3"]),
                    Split4 = GetIntValue(jobRow["Split4"]),
                    DatamatrixType = GetIntValue(jobRow["Datamatrix"], -1),
                    IsAbsolute = GetBoolValue(jobRow["IsAbsolute"])
                };

                // Calcular UserFields basado en splits
                promotion.CalculateUserFields();

                // Cargar LayerQty (LayerQty1 - LayerQty25)
                for (int i = 0; i < 25; i++)
                {
                    string columnName = $"LayerQty{i + 1}";
                    if (jobsTable.Columns.Contains(columnName))
                    {
                        promotion.LayerQty[i] = GetIntValue(jobRow[columnName]);
                    }
                }

                // Cargar LayerUseCodes (LayerUseCodes1 - LayerUseCodes25)
                for (int i = 0; i < 25; i++)
                {
                    string columnName = $"LayerUseCodes{i + 1}";
                    if (jobsTable.Columns.Contains(columnName))
                    {
                        promotion.LayerUseCodes[i] = GetIntValue(jobRow[columnName]);
                    }
                }

                // Cargar datos relacionados (CodesIndex, etc.)
                LoadPromotionData(promotion);

                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar promoción '{jobName}': {ex.Message}", ex);
            }
        }

        public Promotion? LoadPromotionByArtwork(int artwork)
        {
            try
            {
                // Buscar trabajo por artwork
                string sql = $@"
                    SELECT j.* 
                    FROM Jobs j
                    INNER JOIN Artworks a ON j.JobId = a.IdJob
                    WHERE a.Artwork = {artwork}";

                DataTable jobsTable = _databaseService.GetDataTable(sql, "Jobs");

                if (jobsTable.Rows.Count == 0)
                {
                    return null;
                }

                string jobName = GetStringValue(jobsTable.Rows[0]["JobName"]);
                return LoadPromotion(jobName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar promoción por artwork {artwork}: {ex.Message}", ex);
            }
        }

        public List<string> GetAvailableJobs()
        {
            try
            {
                string sql = "SELECT JobName FROM Jobs ORDER BY JobName";
                DataTable table = _databaseService.GetDataTable(sql, "Jobs");

                return table.Rows.Cast<DataRow>()
                    .Select(row => row["JobName"]?.ToString() ?? string.Empty)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener trabajos disponibles: {ex.Message}", ex);
            }
        }

        public List<int> GetAvailableArtworks(string jobName)
        {
            try
            {
                string sql = $@"
                    SELECT DISTINCT a.Artwork
                    FROM Artworks a
                    INNER JOIN Jobs j ON a.IdJob = j.JobId
                    WHERE j.JobName = '{jobName.Replace("'", "''")}'
                    ORDER BY a.Artwork";

                DataTable table = _databaseService.GetDataTable(sql, "Artworks");

                return table.Rows.Cast<DataRow>()
                    .Select(row => GetIntValue(row["Artwork"]))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artworks disponibles: {ex.Message}", ex);
            }
        }

        public void LoadPromotionData(Promotion promotion)
        {
            try
            {
                // Cargar CodesIndex (archivos importados)
                string sql = $"SELECT * FROM CodesIndex WHERE IdJob = {promotion.JobId}";
                DataTable codesIndexTable = _databaseService.GetDataTable(sql, "CodesIndex");

                promotion.FilesImported.Clear();
                int totalCodes = 0;

                foreach (DataRow row in codesIndexTable.Rows)
                {
                    var fileInfo = new CodeFileInfo
                    {
                        Id = GetIntValue(row["IdJob"]),
                        JobId = promotion.JobId,
                        FileName = GetStringValue(row["FileName"]),
                        FromRecord = GetIntValue(row["FromRecord"]),
                        ToRecord = GetIntValue(row["ToRecord"]),
                        TotalCodes = GetIntValue(row["TotalCodes"]),
                        Consumed = GetIntValue(row["Consumed"])
                    };

                    promotion.FilesImported.Add(fileInfo);
                    totalCodes += fileInfo.TotalCodes;
                }

                promotion.TotalCodes = totalCodes;
                promotion.ConsumedCodes = promotion.FilesImported.Sum(f => f.Consumed);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar datos de promoción: {ex.Message}", ex);
            }
        }

        public bool AttachCodesDatabase(Promotion promotion, string dbPath)
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
                    System.Diagnostics.Debug.WriteLine($"Base de datos '{promotion.CodesDb}' ya está adjunta");
                    return true;
                }

                // Construir rutas de archivos
                string mdfPath = Path.Combine(dbPath, $"{promotion.CodesDb}.mdf");
                string ldfPath = Path.Combine(dbPath, $"{promotion.CodesDb}_log.ldf");

                // Verificar que existan los archivos
                if (!File.Exists(mdfPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Archivo .mdf no encontrado: {mdfPath}");
                    return false;
                }

                // Adjuntar base de datos
                bool attached = _databaseService.AttachDatabase(promotion.CodesDb, mdfPath, ldfPath);

                if (attached)
                {
                    // Conectar a la base de datos de códigos
                    _databaseService.ConnectCodesDatabase(promotion.CodesDb);
                }

                return attached;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al adjuntar base de datos de códigos: {ex.Message}");
                return false;
            }
        }

        private string GetCodesDbName(string? codesDbValue)
        {
            if (string.IsNullOrEmpty(codesDbValue))
            {
                return string.Empty;
            }

            // Extraer nombre sin extensión (como en el código original)
            return Path.GetFileNameWithoutExtension(codesDbValue);
        }

        private void DetachPreviousDatabase(string previousJobId)
        {
            try
            {
                // Obtener CodesDb del trabajo anterior
                string sql = $"SELECT CodesDb FROM Jobs WHERE JobId = {previousJobId}";
                DataTable table = _databaseService.GetDataTable(sql, "Jobs");

                if (table.Rows.Count > 0)
                {
                    string? codesDb = table.Rows[0]["CodesDb"]?.ToString();
                    if (!string.IsNullOrEmpty(codesDb))
                    {
                        string dbName = Path.GetFileNameWithoutExtension(codesDb);
                        if (_databaseService.IsDatabaseAttached(dbName))
                        {
                            _databaseService.DetachDatabase(dbName);
                            System.Diagnostics.Debug.WriteLine($"Base de datos desadjuntada: {dbName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al desadjuntar base de datos anterior: {ex.Message}");
            }
        }

        public int? ValidateArtwork(int artwork)
        {
            try
            {
                // Verificar que el artwork exista
                string sql = $@"
                    SELECT j.JobId, j.RecordLength, j.Split1, j.Split2, j.Split3, j.Split4, j.LaserFile
                    FROM Jobs j
                    INNER JOIN Artworks a ON j.JobId = a.IdJob
                    WHERE a.Artwork = {artwork}";

                DataTable table = _databaseService.GetDataTable(sql, "Jobs");

                if (table.Rows.Count == 0)
                {
                    return null; // Artwork no existe
                }

                DataRow row = table.Rows[0];
                int jobId = GetIntValue(row["JobId"]);

                // Validar que la promoción sea válida (basado en GoodPromotion del código original)
                // 1. Verificar que RecordLength y Splits no sean null
                if (row["RecordLength"] == DBNull.Value ||
                    row["Split1"] == DBNull.Value ||
                    row["Split2"] == DBNull.Value ||
                    row["Split3"] == DBNull.Value ||
                    row["Split4"] == DBNull.Value)
                {
                    return null; // Datos incompletos
                }

                // 2. Verificar que RecordLength no sea 0 y que la suma de splits coincida
                int recordLength = GetIntValue(row["RecordLength"]);
                int split1 = GetIntValue(row["Split1"]);
                int split2 = GetIntValue(row["Split2"]);
                int split3 = GetIntValue(row["Split3"]);
                int split4 = GetIntValue(row["Split4"]);

                if (recordLength == 0 || (split1 + split2 + split3 + split4) != recordLength)
                {
                    return null; // Splits no coinciden con RecordLength
                }

                // 3. Verificar que LaserFile no sea null
                string laserFile = GetStringValue(row["LaserFile"]);
                if (string.IsNullOrEmpty(laserFile))
                {
                    return null; // No hay archivo láser
                }

                return jobId; // Artwork válido
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al validar artwork: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene un valor entero de forma segura desde un objeto de base de datos
        /// </summary>
        private int GetIntValue(object? value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Obtiene un valor string de forma segura desde un objeto de base de datos
        /// </summary>
        private string GetStringValue(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }
            return value.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Obtiene un valor booleano de forma segura desde un objeto de base de datos
        /// </summary>
        private bool GetBoolValue(object? value, bool defaultValue = false)
        {
            if (value == null || value == DBNull.Value)
            {
                return defaultValue;
            }
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}

