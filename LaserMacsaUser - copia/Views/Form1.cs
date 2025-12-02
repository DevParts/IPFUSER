using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using LaserMacsaUser.Controllers;
using LaserMacsaUser.Models;
using LaserMacsaUser.Views;
using LaserMacsaUser.Services;
using LaserMacsaUser.Configuration;
using System.Drawing;

namespace LaserMacsaUser.Views
{
    public partial class Form1 : Form
    {
        // Services
        private IDatabaseService? _databaseService;
        private ILaserService? _laserService;
        private IQueueService? _queueService;
        private ISpeedwayService? _speedwayService;

        // Controllers
        private ArtworkController? _artworkController;
        private PromotionController? _promotionController;
        private ProductionController? _productionController;

        // Current state
        private Promotion? _currentPromotion;
        private ProductionBatch? _currentBatch;
        private int _firstArtworkId = 0;
        private System.Timers.Timer? _updateTimer;
        private bool _isClosing = false;
        private string _messagePath = string.Empty;  // Directorio donde están los archivos láser
        
        // Sistema de gestión de alarmas en la UI
        private readonly Dictionary<string, AlarmForm> _activeAlarmForms = new Dictionary<string, AlarmForm>();
        private readonly Dictionary<string, bool> _manuallyClosedAlarms = new Dictionary<string, bool>();

        // Settings
        private AppSettings _settings = new AppSettings();

        public Form1()
        {
            InitializeComponent();
            InitializeServices();
            SetupEventHandlers();
            InitializeUI();
        }

        private void InitializeServices()
        {
            // Database Service
            _databaseService = new DatabaseService
            {
                DbName = _settings.Catalog,
                DataSource = _settings.DataSource,
                UseWindowsAuthentication = _settings.UseWindowsAuthentication,
                User = _settings.User,
                Password = _settings.Password,
                ConnectionTimeout = 30
            };

            // Laser Service
            _laserService = new LaserService();

            // Queue Service
            _queueService = new QueueService(_databaseService);

            // Speedway Service (opcional)
            _speedwayService = new SpeedwayService();

            // Controllers
            _artworkController = new ArtworkController(_databaseService);
            _promotionController = new PromotionController(_databaseService);
            _productionController = new ProductionController(_laserService, _queueService, _speedwayService);

            // Suscribirse a eventos del ProductionController
            _productionController.StatusUpdated += ProductionController_StatusUpdated;
            _productionController.ErrorOccurred += ProductionController_ErrorOccurred;
            _productionController.AlarmDetected += ProductionController_AlarmDetected;
        }

        private void SetupEventHandlers()
        {
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnExit.Click += BtnExit_Click;
        }

        private void InitializeUI()
        {
            // Deshabilitar controles inicialmente
            btnStart.Enabled = false;
            btnStop.Enabled = false;
            txtStoppers.Enabled = false;
            txtOrder.Enabled = false;

            // Iniciar flujo de selección de artwork
            Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Iniciar proceso de selección de artwork (Step 1 y 2)
            GetArtwork();
        }

        /// <summary>
        /// Step 1 y 2: Selección de Artwork (doble entrada)
        /// </summary>
        private void GetArtwork()
        {
            while (!_isClosing)
            {
                // Step 1: Primera entrada
                var artworkForm1 = new ArtworkSelection(ArtworkSelection.Mode.Activate);
                if (artworkForm1.ShowDialog() != DialogResult.OK)
                {
                    // Si el usuario canceló, cerrar la aplicación
                    if (!_isClosing)
                    {
                        _isClosing = true;
                        Application.Exit();
                    }
                    return;
                }

                if (!int.TryParse(artworkForm1.ArtworkNumber, out int artworkId1))
                {
                    MessageBox.Show("Please enter a valid artwork number.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                // Validar primera entrada
                var artwork1 = _artworkController!.ValidateFirstArtwork(artworkId1);
                if (!artwork1.IsValid)
                {
                    var result = MessageBox.Show(
                        artwork1.ValidationError ?? "Invalid artwork" + "\n\n¿Desea intentar con otro artwork?",
                        "Validation Error",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);
                    
                    if (result == DialogResult.No)
                    {
                        // Usuario quiere salir
                        _isClosing = true;
                        Application.Exit();
                        return;
                    }
                    continue; // Intentar de nuevo
                }

                _firstArtworkId = artworkId1;

                // Step 2: Segunda entrada (confirmación)
                var artworkForm2 = new ArtworkSelection(ArtworkSelection.Mode.Repeat);
                if (artworkForm2.ShowDialog() != DialogResult.OK)
                {
                    // Si el usuario canceló, volver al inicio
                    if (_isClosing)
                    {
                        return;
                    }
                    continue;
                }

                if (!int.TryParse(artworkForm2.ArtworkNumber, out int artworkId2))
                {
                    MessageBox.Show("Please enter a valid artwork number.", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                // Validar que coincidan
                if (!_artworkController.ValidateSecondArtwork(_firstArtworkId, artworkId2))
                {
                    MessageBox.Show("Artwork numbers do not match. Please try again.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                // Step 3: Confirmar promoción
                var promotion = _promotionController!.LoadPromotion(_firstArtworkId);
                if (promotion == null)
                {
                    MessageBox.Show("Error loading promotion.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                // Mostrar diálogo de confirmación
                var confirmForm = new ConfirmPromotionForm(_firstArtworkId, promotion.Name);
                if (confirmForm.ShowDialog() == DialogResult.OK && confirmForm.IsConfirmed)
                {
                    // Buscar y adjuntar base de datos principal (IPFEu) si es necesario
                    const string mainDbName = "IPFEu";
                    bool mainDbAttached = false;

                    // Verificar si ya está adjunta
                    if (_databaseService != null && _databaseService.IsDatabaseAttached(mainDbName))
                    {
                        mainDbAttached = true;
                        System.Diagnostics.Debug.WriteLine($"Base de datos '{mainDbName}' ya está adjunta");
                    }
                    else
                    {
                        // Buscar archivo IPFEu.mdf
                        string dbPath = SearchDatabasePath();
                        if (string.IsNullOrEmpty(dbPath))
                        {
                            // Intentar buscar también en ubicaciones comunes
                            string[] commonPaths = new[]
                            {
                                @"C:\IPFEu",
                                @"C:\LaserMacsa\bases de datos\IPFEu",
                                Path.Combine(Application.StartupPath, "..", "..", "..", "..", "user", "IPFEu")
                            };

                            foreach (string commonPath in commonPaths)
                            {
                                try
                                {
                                    string normalizedPath = Path.GetFullPath(commonPath);
                                    string mdfPath = Path.Combine(normalizedPath, "IPFEu.mdf");
                                    if (File.Exists(mdfPath))
                                    {
                                        dbPath = normalizedPath;
                                        break;
                                    }
                                }
                                catch { }
                            }
                        }

                        if (!string.IsNullOrEmpty(dbPath))
                        {
                            // Adjuntar base de datos principal
                            string mdfPath = Path.Combine(dbPath, "IPFEu.mdf");
                            string ldfPath = Path.Combine(dbPath, "IPFEu_log.ldf");

                            if (File.Exists(mdfPath))
                            {
                                try
                                {
                                    if (_databaseService != null)
                                    {
                                        mainDbAttached = _databaseService.AttachDatabase(mainDbName, mdfPath, ldfPath);
                                        if (mainDbAttached)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Base de datos '{mainDbName}' adjuntada exitosamente desde: {mdfPath}");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al adjuntar base de datos: {ex.Message}");
                                }
                            }
                        }

                        if (!mainDbAttached)
                        {
                            MessageBox.Show(
                                "Base de datos 'IPFEu' no encontrada o no se pudo adjuntar.\n\n" +
                                "Busque en todas las unidades lógicas (C:\\, D:\\, etc.) el archivo 'IPFEu.mdf'.\n\n" +
                                "Ubicaciones comunes:\n" +
                                "- C:\\IPFEu\\IPFEu.mdf\n" +
                                "- C:\\LaserMacsa\\bases de datos\\IPFEu\\IPFEu.mdf",
                                "Database Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                    }

                    // Cargar base de datos de códigos de la promoción
                    // Primero obtener la ruta de la base principal para buscar la de códigos
                    string codesDbPath = string.Empty;
                    if (_databaseService != null)
                    {
                        // Intentar obtener la ruta física de la base adjunta
                        try
                        {
                            using (var reader = _databaseService.GetDataReader(
                                "SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('IPFEu') AND type = 0"))
                            {
                                if (reader.Read())
                                {
                                    string? physicalPath = reader["physical_name"]?.ToString();
                                    if (!string.IsNullOrEmpty(physicalPath))
                                    {
                                        codesDbPath = Path.GetDirectoryName(physicalPath) ?? string.Empty;
                                    }
                                }
                                reader.Close();
                            }
                        }
                        catch
                        {
                            // Si no podemos obtener la ruta, usar ubicaciones comunes
                            codesDbPath = @"C:\IPFEu";
                        }
                    }

                    if (string.IsNullOrEmpty(codesDbPath))
                    {
                        codesDbPath = @"C:\IPFEu"; // Fallback
                    }

                    if (_promotionController.LoadCodesDatabase(promotion, codesDbPath))
                    {
                        // Guardar MessagePath (directorio de la BD) para uso posterior
                        _messagePath = codesDbPath;
                        if (!_messagePath.EndsWith("\\"))
                        {
                            _messagePath += "\\";
                        }
                        
                        _currentPromotion = promotion;
                        PreparePromotion();
                        break; // Salir del bucle
                    }
                    else
                    {
                        MessageBox.Show(
                            $"Error loading codes database.\n\n" +
                            $"Promotion: {promotion.Name}\n" +
                            $"CodesDb: {promotion.CodesDb}\n" +
                            $"Path: {codesDbPath}\n\n" +
                            $"Verifique que el archivo '{promotion.CodesDb}.mdf' existe en la ruta.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Prepara la interfaz después de confirmar la promoción
        /// Basado en CODIGO_COMPLETO_FUNCIONAMIENTO.md PreparePromotion()
        /// </summary>
        private void PreparePromotion()
        {
            if (_currentPromotion == null) return;

            // Limpiar campos según CODIGO_COMPLETO_FUNCIONAMIENTO.md línea 232
            txtStoppers.Text = "";
            txtOrder.Text = "";
            txtCode.Text = "";
            txtPromoIndex.Text = "";
            txtProduced.Text = "0";
            txtPending.Text = "0";
            progressProd.Value = 0;
            lblProgressProd.Text = "00%";

            // Mostrar información de artwork
            txtArtwork.Text = _firstArtworkId.ToString();
            txtArtwork.BackColor = Color.Yellow;

            // Mostrar información de promoción
            txtPromotion.Text = _currentPromotion.Name;
            txtLaser.Text = _currentPromotion.LaserFile;

            // Calcular y mostrar códigos disponibles (según CODIGO_COMPLETO_FUNCIONAMIENTO.md línea 248-250)
            int totalCodes = _currentPromotion.TotalCodes;
            int consumedCodes = _currentPromotion.ConsumedCodes;
            int availableCodes = totalCodes - consumedCodes;
            txtCodes.Text = availableCodes.ToString();

            // Actualizar barra de progreso de códigos
            if (totalCodes > 0)
            {
                int percent = (int)Math.Round((double)availableCodes / totalCodes * 100.0);
                progressCodes.Value = percent;
                lblProgressCodes.Text = $"{percent} %";
            }

            // Habilitar campos de producción según CODIGO_COMPLETO_FUNCIONAMIENTO.md línea 278
            txtStoppers.Enabled = true;
            txtOrder.Enabled = true;
            btnStart.Enabled = true;
            btnStart.Text = "Start";
        }


        private void BtnStart_Click(object? sender, EventArgs e)
        {
            if (_currentPromotion == null)
            {
                MessageBox.Show("No promotion selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // VALIDACIÓN 1: Validar cantidad
            if (!int.TryParse(txtStoppers.Text, out int stoppers) || stoppers <= 0)
            {
                MessageBox.Show("Debe indicar cantidad a producir (mayor que 0).", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStoppers.Focus();
                return;
            }

            // VALIDACIÓN 2: Validar pedido (debe tener exactamente 11 caracteres según CODIGO_COMPLETO_FUNCIONAMIENTO.md línea 303)
            if (txtOrder.Text.Length != 11)
            {
                MessageBox.Show("Pedido debe tener exactamente 11 caracteres.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOrder.Focus();
                return;
            }

            // VALIDACIÓN 3: Validar que no exceda códigos disponibles (solo si no es continuación)
            // Según CODIGO_COMPLETO_FUNCIONAMIENTO.md línea 297
            int availableCodes = _currentPromotion.TotalCodes - _currentPromotion.ConsumedCodes;
            if (btnStart.Text != "Continue" && stoppers > availableCodes)
            {
                MessageBox.Show(
                    $"Cantidad excede códigos disponibles.\n\n" +
                    $"Solicitado: {stoppers}\n" +
                    $"Disponibles: {availableCodes}",
                    "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStoppers.Focus();
                return;
            }


            // Validar que MessagePath esté configurado
            if (string.IsNullOrEmpty(_messagePath))
            {
                MessageBox.Show(
                    "No se pudo determinar la ubicación de los archivos láser.\n\n" +
                    "Por favor verifique que la base de datos esté correctamente configurada.",
                    "Error de Configuración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Validar que el archivo láser exista en MessagePath
            if (!string.IsNullOrEmpty(_currentPromotion.LaserFile))
            {
                string laserFilePath = Path.Combine(_messagePath, _currentPromotion.LaserFile);
                if (!File.Exists(laserFilePath))
                {
                    MessageBox.Show(
                        $"Archivo láser no encontrado:\n\n" +
                        $"Archivo: {_currentPromotion.LaserFile}\n" +
                        $"Ubicación esperada: {laserFilePath}\n\n" +
                        $"Por favor verifique que el archivo existe en el directorio de la base de datos.\n\n" +
                        $"Nota: En LaserMacsa, al seleccionar un archivo láser, este se copia automáticamente al directorio de la BD.",
                        "Archivo Láser No Encontrado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show(
                    "La promoción no tiene archivo láser configurado.\n\n" +
                    "Por favor configure un archivo láser (.msf o .mlf) en LaserMacsa.",
                    "Archivo Láser No Configurado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Crear batch
            _currentBatch = new ProductionBatch
            {
                Order = txtOrder.Text,
                StoppersToProduce = stoppers,
                StartTime = DateTime.Now,
                IsActive = true
            };

            // Iniciar producción usando MessagePath obtenido del directorio de la BD
            bool success = _productionController!.StartProduction(
                _currentBatch,
                _currentPromotion,
                _settings.Laser_IP,
                _messagePath  // Usar MessagePath obtenido del directorio de la BD
            );

            if (success)
            {
                // Deshabilitar controles
                txtStoppers.Enabled = false;
                txtOrder.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnExit.Enabled = false;

                // Iniciar timer de actualización
                StartUpdateTimer();
            }
            else
            {
                MessageBox.Show("Error starting production. Check laser connection.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            _productionController?.StopProduction();
            StopUpdateTimer();
            
            // Cerrar todos los formularios de alarma activos
            foreach (var form in _activeAlarmForms.Values.ToList())
            {
                if (!form.IsDisposed)
                {
                    form.Close();
                }
            }
            _activeAlarmForms.Clear();
            
            // Limpiar registro de alarmas cerradas manualmente
            _manuallyClosedAlarms.Clear();

            // Actualizar UI
            if (_currentBatch != null)
            {
                txtProduced.Text = _currentBatch.Produced.ToString();
                txtPending.Text = _currentBatch.Pending.ToString();
            }

            // Habilitar controles
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnExit.Enabled = true;
        }

        private void BtnExit_Click(object? sender, EventArgs e)
        {
            if (_productionController != null)
            {
                var status = _productionController.GetStatus();
                if (status.IsProducing)
                {
                    var result = MessageBox.Show("Production is running. Stop and exit?", "Confirm Exit",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        _productionController.StopProduction();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Application.Exit();
        }

        private void ProductionController_StatusUpdated(object? sender, Controllers.ProductionStatusEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ProductionController_StatusUpdated(sender, e)));
                return;
            }

            // Actualizar UI
            txtProduced.Text = e.Produced.ToString();
            txtPending.Text = e.Pending.ToString();
            txtCode.Text = e.LastCode ?? "";
            txtPromoIndex.Text = e.LastCodePosition?.ToString() ?? "";

            // Actualizar barra de progreso
            if (_currentBatch != null && _currentBatch.StoppersToProduce > 0)
            {
                int percent = (int)Math.Round((double)e.Produced / _currentBatch.StoppersToProduce * 100.0);
                progressProd.Value = percent;
                lblProgressProd.Text = $"{percent} %";
            }
        }

        private void ProductionController_ErrorOccurred(object? sender, string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ProductionController_ErrorOccurred(sender, error)));
                return;
            }

            MessageBox.Show(error, "Production Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            BtnStop_Click(null, EventArgs.Empty);
        }

        private void ProductionController_AlarmDetected(object? sender, LaserAlarmEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ProductionController_AlarmDetected(sender, e)));
                return;
            }

            string alarmKey = $"{e.Alarm.Code}_{e.Alarm.Type}";
            
            // Si el usuario cerró manualmente esta alarma, no volver a mostrarla
            if (_manuallyClosedAlarms.ContainsKey(alarmKey) && _manuallyClosedAlarms[alarmKey])
            {
                return; // Usuario cerró manualmente, no mostrar de nuevo
            }
            
            // Si ya hay un formulario de alarma activo para esta alarma, cerrarlo primero
            if (_activeAlarmForms.TryGetValue(alarmKey, out AlarmForm? existingForm))
            {
                if (!existingForm.IsDisposed && existingForm.Visible)
                {
                    // Cerrar el formulario existente (cierre automático)
                    existingForm.Close();
                    _activeAlarmForms.Remove(alarmKey);
                }
            }
            
            // Crear y mostrar nuevo formulario de alarma
            var alarmForm = new AlarmForm(e.Alarm);
            alarmForm.FormClosed += (s, args) =>
            {
                // Cuando se cierra el formulario, verificar si fue manual o automático
                if (alarmForm.WasClosedManually)
                {
                    // Marcar como cerrado manualmente para no volver a mostrar
                    _manuallyClosedAlarms[alarmKey] = true;
                }
                
                // Remover del diccionario de formularios activos
                _activeAlarmForms.Remove(alarmKey);
            };
            
            _activeAlarmForms[alarmKey] = alarmForm;
            alarmForm.Show(); // Mostrar sin bloquear (Show, no ShowDialog)

            // Si es crítica, ya se detuvo la producción automáticamente
            if (e.Alarm.Severity == LaserAlarmService.AlarmSeverity.Critical)
            {
                BtnStop_Click(null, EventArgs.Empty);
            }
        }

        private void StartUpdateTimer()
        {
            _updateTimer = new System.Timers.Timer(2000); // Actualizar cada 2 segundos
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
            _updateTimer.Start();
        }

        private void StopUpdateTimer()
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            _updateTimer = null;
        }

        private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_currentPromotion != null && _productionController != null)
            {
                // Actualizar códigos disponibles
                int totalCodes = _currentPromotion.TotalCodes;
                int consumedCodes = _currentPromotion.ConsumedCodes;
                int availableCodes = totalCodes - consumedCodes;

                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        txtCodes.Text = availableCodes.ToString();
                        if (totalCodes > 0)
                        {
                            int percent = (int)Math.Round((double)availableCodes / totalCodes * 100.0);
                            progressCodes.Value = percent;
                            lblProgressCodes.Text = $"{percent} %";
                        }
                    }));
                }
            }
        }

        private void lblProgressCodes_Click(object sender, EventArgs e)
        {
            // Evento existente
        }

        /// <summary>
        /// Busca la base de datos IPFEu.mdf en todas las unidades lógicas
        /// Similar al método FindMdfFiles() de LaserMacsa
        /// </summary>
        private string SearchDatabasePath()
        {
            try
            {
                const string dbName = "IPFEu";
                const string mdfFileName = "IPFEu.mdf";
                
                // Primero verificar si la base ya está adjunta en SQL Server
                if (_databaseService != null && _databaseService.IsDatabaseAttached(dbName))
                {
                    System.Diagnostics.Debug.WriteLine($"Base de datos '{dbName}' ya está adjunta en SQL Server");
                    // Retornar ruta vacía porque ya está adjunta, no necesitamos el archivo físico
                    return string.Empty; // Indica que está adjunta
                }

                // Buscar archivo IPFEu.mdf en todas las unidades lógicas
                string[] logicalDrives = Directory.GetLogicalDrives();
                string? foundMdfPath = null;

                foreach (string drive in logicalDrives)
                {
                    try
                    {
                        // Buscar archivo IPFEu.mdf en esta unidad (con profundidad limitada)
                        foundMdfPath = SearchMdfFile(drive, mdfFileName, maxDepth: 5, currentDepth: 0);
                        if (!string.IsNullOrEmpty(foundMdfPath))
                        {
                            System.Diagnostics.Debug.WriteLine($"Base de datos encontrada en: {foundMdfPath}");
                            break;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Ignorar unidades sin acceso
                        continue;
                    }
                    catch (Exception ex)
                    {
                        // Continuar con la siguiente unidad
                        System.Diagnostics.Debug.WriteLine($"Error accediendo a {drive}: {ex.Message}");
                        continue;
                    }
                }

                // Si encontramos el archivo, retornar el directorio que lo contiene
                if (!string.IsNullOrEmpty(foundMdfPath))
                {
                    return Path.GetDirectoryName(foundMdfPath) ?? string.Empty;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SearchDatabasePath: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Busca archivo .mdf recursivamente con límite de profundidad
        /// Similar al método SearchMdfFiles() de LaserMacsa
        /// </summary>
        private string? SearchMdfFile(string path, string fileName, int maxDepth, int currentDepth)
        {
            if (currentDepth >= maxDepth)
                return null;

            try
            {
                // Buscar archivo en el directorio actual
                string[] files = Directory.GetFiles(path, fileName, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[0]; // Retornar el primero encontrado
                }

                // Buscar en subdirectorios (con límite de profundidad)
                if (currentDepth < maxDepth - 1)
                {
                    string[] directories = Directory.GetDirectories(path);
                    foreach (string dir in directories)
                    {
                        try
                        {
                            // Saltar carpetas del sistema comunes para acelerar
                            string dirName = Path.GetFileName(dir).ToLower();
                            if (dirName.StartsWith("$") ||
                                dirName == "system volume information" ||
                                dirName == "recovery" ||
                                dirName == "windows" ||
                                (dirName == "program files" && !dir.ToLower().Contains("sql")) ||
                                (dirName == "program files (x86)" && !dir.ToLower().Contains("sql")))
                            {
                                continue;
                            }

                            string? found = SearchMdfFile(dir, fileName, maxDepth, currentDepth + 1);
                            if (!string.IsNullOrEmpty(found))
                            {
                                return found;
                            }
                        }
                        catch
                        {
                            // Continuar con el siguiente directorio
                            continue;
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // No tenemos acceso a este directorio
                return null;
            }
            catch
            {
                // Continuar con el siguiente
                return null;
            }

            return null;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.ShowDialog();

            if (login.IsAuthenticated)
            {
                AppConfigForm formAppInfo = new AppConfigForm();
                formAppInfo.ShowDialog();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Marcar que estamos cerrando para detener el bucle de artwork
            _isClosing = true;

            // Si el usuario está cerrando y hay producción activa, preguntar
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (_productionController != null)
                {
                    var status = _productionController.GetStatus();
                    if (status.IsProducing)
                    {
                        var result = MessageBox.Show("Production is running. Stop and exit?", "Confirm Exit",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            e.Cancel = true;
                            _isClosing = false; // Restaurar flag si canceló
                            return;
                        }
                        _productionController.StopProduction();
                    }
                }
            }

            // Limpiar recursos
            StopUpdateTimer();
            _productionController?.StopProduction();
            _laserService?.Disconnect();
            _speedwayService?.Disconnect();
            _databaseService?.CloseConnection();
            
            base.OnFormClosing(e);
        }
    }
}

