using System;
using System.Data;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio para generación de histórico (basado en AppCSIUser.GenerateHistoric)
    /// </summary>
    public class HistoryService : IHistoryService
    {
        private readonly IDatabaseService _databaseService;

        public HistoryService(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public void GenerateHistoric(int initRecord, int finalRecord, Promotion promotion, string pedido, string artwork, string drive)
        {
            if (finalRecord == 0 || initRecord == 0)
            {
                return;
            }

            try
            {
                bool writePLCData = true;

                // Procesar cada archivo importado que esté en el rango
                foreach (var fileInfo in promotion.FilesImported)
                {
                    // Verificar si el rango de registros está dentro de este archivo
                    if (initRecord < fileInfo.FromRecord || initRecord >= fileInfo.ToRecord)
                    {
                        continue;
                    }

                    int desde = 0;
                    int hasta = 0;

                    if (finalRecord <= fileInfo.ToRecord)
                    {
                        // El rango completo está en este archivo
                        desde = initRecord - fileInfo.FromRecord + 1;
                        hasta = finalRecord - fileInfo.FromRecord + 1;
                    }
                    else
                    {
                        // El rango se extiende más allá de este archivo
                        desde = initRecord - fileInfo.FromRecord + 1;
                        hasta = fileInfo.ToRecord - fileInfo.FromRecord + 1;
                    }

                    // Agregar registro histórico
                    AddHistoric(
                        fileInfo.FileName,
                        desde,
                        hasta,
                        writePLCData,
                        promotion,
                        pedido,
                        artwork,
                        drive);

                    writePLCData = false; // Solo escribir datos de PLC en el primer registro

                    // Si el rango se extiende más allá, continuar con el siguiente archivo
                    int currentIndex = promotion.FilesImported.IndexOf(fileInfo);
                    if (finalRecord > fileInfo.ToRecord && currentIndex >= 0 && currentIndex < promotion.FilesImported.Count - 1)
                    {
                        int nextIndex = currentIndex + 1;
                        if (nextIndex < promotion.FilesImported.Count)
                        {
                            initRecord = promotion.FilesImported[nextIndex].FromRecord;
                        }
                    }
                    else
                    {
                        break; // No hay más archivos que procesar
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar histórico: {ex.Message}", ex);
            }
        }

        private void AddHistoric(string fichero, int desde, int hasta, bool writePLCData, Promotion promotion, string pedido, string artwork, string drive)
        {
            try
            {
                DataTable table = _databaseService.GetDataTable("SELECT * FROM Historico", "Historico");
                DataRow row = table.NewRow();

                row["Pedido"] = pedido;
                row["Artwork"] = artwork;
                row["Fichero"] = fichero;
                row["Desde"] = desde;
                row["Hasta"] = hasta;
                row["Volumen"] = GetVolumeLabel(drive);
                row["Timestamp"] = DateTime.Now;
                row["Sesion"] = GetSessionId(); // TODO: Implementar generación de sesión
                row["Layers"] = promotion.Layers;

                // Agregar LayerQty (LayerQty1 - LayerQty25)
                // Por ahora, usar valores de la promoción (en producción multicapa se actualizarían desde PLC)
                for (int i = 0; i < 25; i++)
                {
                    string columnName = $"LayerQty{i + 1}";
                    if (table.Columns.Contains(columnName))
                    {
                        if (promotion.Layers > 1 && writePLCData)
                        {
                            // TODO: Obtener valores desde PLC si está disponible
                            row[columnName] = promotion.LayerQty[i];
                        }
                        else
                        {
                            row[columnName] = 0;
                        }
                    }
                }

                table.Rows.Add(row);
                _databaseService.UpdateTable(table);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar registro histórico: {ex.Message}", ex);
            }
        }

        private string GetVolumeLabel(string drive)
        {
            try
            {
                if (string.IsNullOrEmpty(drive))
                    return string.Empty;

                if (drive.Length >= 2 && drive[1] == ':')
                {
                    var driveInfo = new System.IO.DriveInfo(drive.Substring(0, 2));
                    return driveInfo.VolumeLabel;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetSessionId()
        {
            // Generar ID de sesión único
            // Por ahora, usar timestamp
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}

