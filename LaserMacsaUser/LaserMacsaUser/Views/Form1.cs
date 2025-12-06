using LaserMacsaUser.Views.AppInfoPrueba;
using LaserMacsaUser.Views.Login;
using LaserMacsaUser.Views.AppInfo;
using LaserMacsaUser.Resources;
using LaserMacsaUser.Services;
using LaserMacsaUser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserMacsaUser.Views
{
    public partial class Form1 : Form
    {
        // Servicios
        private readonly IDatabaseService _databaseService;
        private readonly ILaserService _laserService;
        private IQueueService? _queueService;
        private readonly IPromotionService _promotionService;
        private readonly IHistoryService _historyService;

        // Estado de la aplicación
        private Promotion? _currentPromotion;
        private System.Windows.Forms.Timer? _syncTimer;
        private string _dbPath = string.Empty;
        private string _drive = string.Empty;
        private string _pedido = string.Empty;
        private string _artwork = string.Empty;
        private int _totalToProduce = 0;
        private int _initialRecord = 0;
        private int _finalRecord = 0;
        private bool _isRunning = false;

        // Control de alarmas y advertencias
        private readonly HashSet<int> _activeAlarms = new HashSet<int>();
        private bool _lowLevelWarningShown = false;

        public Form1()
        {
            InitializeComponent();

            // Inicializar servicios
            _databaseService = new DatabaseService();
            _laserService = new LaserService();
            _laserService.AlarmDetected += LaserService_AlarmDetected;
            _promotionService = new PromotionService(_databaseService);
            _historyService = new HistoryService(_databaseService);

            // Inicializar timer de sincronización
            InitializeSyncTimer();

            // Conectar eventos
            this.Load += Form1_Load;
            btnStart.Click += btnStart_Click;
            btnStop.Click += btnStop_Click;
            cmboxtxtCodes.SelectedIndexChanged += cmboxtxtCodes_SelectedIndexChanged;
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.ShowDialog();

            if (login.IsAuthenticated)
            {
                AppConfigForm formAppInfo = new AppConfigForm();
                formAppInfo.ShowDialog();
            }
        }

        private void lblCodes_Click(object sender, EventArgs e)
        {

        }
        public enum LaserIndicator
        {
            Ocupado,        // 🔴 Rojo
            Listo,          // 🟢 Verde
            Desconectado    // ⚪ Gris
        }

        // Colores apagados
        Color redOff = Color.FromArgb(180, 160, 160);   // Gris con toque rojo
        Color greenOff = Color.FromArgb(160, 180, 160);  // Gris con toque verde
        Color yellowOff = Color.FromArgb(190, 190, 150);


        // Colores encendidos
        Color redOn = Color.FromArgb(255, 0, 0);
        Color greenOn = Color.FromArgb(0, 255, 0);
        Color yellowOn = Color.FromArgb(255, 230, 80);

        // Timer para animación
        private System.Windows.Forms.Timer fadeTimer;
        private Panel currentLed;
        private Color startColor;
        private Color targetColor;
        private int fadeStep = 0;

        private void StartFade(Panel led, Color offColor, Color onColor)
        {
            // Apagar visualmente antes de animar
            led.BackColor = offColor;

            currentLed = led;
            startColor = offColor;
            targetColor = onColor;
            fadeStep = 0;

            if (fadeTimer == null)
            {
                fadeTimer = new System.Windows.Forms.Timer();
                fadeTimer.Interval = 15; // velocidad del fade (15 ms suave)
                fadeTimer.Tick += FadeTick;
            }

            fadeTimer.Start();
        }

        private void FadeTick(object sender, EventArgs e)
        {
            fadeStep++;
            float t = fadeStep / 20f; // 20 pasos → 300 ms aprox.

            if (t > 1f) t = 1f;

            int r = (int)(startColor.R + (targetColor.R - startColor.R) * t);
            int g = (int)(startColor.G + (targetColor.G - startColor.G) * t);
            int b = (int)(startColor.B + (targetColor.B - startColor.B) * t);

            currentLed.BackColor = Color.FromArgb(r, g, b);

            if (t >= 1f)
                fadeTimer.Stop();
        }

        public void SetLaserStatus(LaserIndicator status)
        {
            // Todos apagados (no se ocultan)
            pnlLedRed.BackColor = redOff;
            pnlLedGreen.BackColor = greenOff;
            pnlLedGray.BackColor = yellowOff;

            switch (status)
            {
                case LaserIndicator.Ocupado:
                    StartFade(pnlLedRed, redOff, redOn);
                    break;

                case LaserIndicator.Listo:
                    StartFade(pnlLedGreen, greenOff, greenOn);
                    break;

                case LaserIndicator.Desconectado:
                    StartFade(pnlLedGray, yellowOff, yellowOn);
                    break;
            }
        }



        private void configPruebaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfirFormPrueba formAppInfoPrueba = new AppConfirFormPrueba();
            formAppInfoPrueba.ShowDialog();
        }

        #region Inicialización

        private void InitializeSyncTimer()
        {
            _syncTimer = new System.Windows.Forms.Timer();
            _syncTimer.Interval = 2000; // 2 segundos
            _syncTimer.Tick += SyncTimer_Tick;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            MakeCircle(pnlLedRed);
            MakeCircle(pnlLedGreen);
            MakeCircle(pnlLedGray);

            SetLaserStatus(LaserIndicator.Desconectado);
            try
            {
                // 1. Preparar base de datos
                PrepareDataBase();

                // 2. Buscar y adjuntar BD IPFEu
                if (!SearchDb())
                {
                    MessageBox.Show(
                        "No se encontró la base de datos IPFEu.\n\n" +
                        "Por favor, asegúrese de que el archivo IPF.mdf esté en una unidad accesible.",
                        "Error de Base de Datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // 3. Seleccionar Artwork/Promoción
                GetArtwork();

                // Configurar estado inicial de UI
                UpdateUIState(isRunning: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inicializar la aplicación:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void MakeCircle(Panel p)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, p.Width - 1, p.Height - 1);
            p.Region = new Region(gp);
        }
        private void PrepareDataBase()
        {
            try
            {
                // Cargar configuración desde AppSettings
                var settings = LoadAppSettings();

                // Conectar a la base de datos principal
                _databaseService.Connect(
                    settings.DataSource,
                    settings.Catalog,
                    settings.UseWindowsAuthentication,
                    settings.User,
                    settings.Password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al preparar base de datos: {ex.Message}", ex);
            }
        }

        private bool SearchDb()
        {
            try
            {
                // Buscar IPFEu en todas las unidades (como en frmMain.SearchDb())
                string[] drives = Directory.GetLogicalDrives();
                string? dbPath = null;

                foreach (string drive in drives)
                {
                    try
                    {
                        string ipfeuPath = Path.Combine(drive, "IPFEu");
                        if (Directory.Exists(ipfeuPath))
                        {
                            string mdfFilePath = Path.Combine(ipfeuPath, "IPF.mdf");
                            if (File.Exists(mdfFilePath))
                            {
                                dbPath = ipfeuPath;
                                _drive = drive;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        // Continuar buscando en otras unidades
                    }
                }

                if (string.IsNullOrEmpty(dbPath))
                {
                    return false;
                }

                _dbPath = dbPath;
                System.Diagnostics.Debug.WriteLine($"Base de datos encontrada en: {dbPath}");

                // Desadjuntar si ya está adjunta
                if (_databaseService.IsDatabaseAttached("IPFEu"))
                {
                    _databaseService.DetachDatabase("IPFEu");
                }

                // Adjuntar base de datos
                string mdfPath = Path.Combine(dbPath, "IPF.mdf");
                string ldfPath = Path.Combine(dbPath, "IPF_log.ldf");

                if (!File.Exists(ldfPath))
                {
                    // Si no existe el .ldf, SQL Server lo recreará
                    ldfPath = string.Empty;
                }

                bool attached = _databaseService.AttachDatabase("IPFEu", mdfPath, ldfPath);
                return attached;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SearchDb: {ex.Message}");
                return false;
            }
        }

        private void GetArtwork()
        {
            try
            {
                while (true)
                {
                    // Paso 1: Selección inicial de artwork (Activate)
                    var artworkForm = new ArtworkSelection(ArtworkSelection.Mode.Activate);
                    if (artworkForm.ShowDialog() != DialogResult.OK)
                    {
                        // Usuario canceló - cerrar aplicación
                        this.Close();
                        return;
                    }

                    // Validar artwork ingresado
                    if (!int.TryParse(artworkForm.ArtworkNumber, out int artwork))
                    {
                        MessageBox.Show(
                            "Por favor, ingrese un número de artwork válido.",
                            "Artwork Inválido",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    // Validar que el artwork exista y pertenezca a una promoción válida
                    int? jobId = _promotionService.ValidateArtwork(artwork);
                    if (jobId == null)
                    {
                        MessageBox.Show(
                            $"El artwork {artwork} no existe o no pertenece a una promoción válida.\n\n" +
                            "Por favor, verifique que:\n" +
                            "- El artwork exista en la base de datos\n" +
                            "- La promoción tenga configuración completa (RecordLength, Splits, LaserFile)",
                            "Artwork Inválido",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    // Paso 2: Confirmación de artwork (Repeat)
                    var artworkRepeatForm = new ArtworkSelection(ArtworkSelection.Mode.Repeat);
                    if (artworkRepeatForm.ShowDialog() != DialogResult.OK)
                    {
                        continue; // Volver a pedir artwork
                    }

                    if (artworkRepeatForm.ArtworkNumber != artwork.ToString())
                    {
                        MessageBox.Show(
                            "Los números de artwork no coinciden. Por favor, ingrese el mismo número.",
                            "Artwork No Coincide",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    // Paso 3: Cargar promoción
                    _currentPromotion = _promotionService.LoadPromotionByArtwork(artwork);
                    if (_currentPromotion == null)
                    {
                        MessageBox.Show(
                            $"Error al cargar la promoción para el artwork {artwork}.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        continue;
                    }

                    // Paso 4: Mostrar confirmación de promoción
                    var confirmForm = new PromotionConfirmation(artwork, _currentPromotion);
                    if (confirmForm.ShowDialog() == DialogResult.OK && confirmForm.IsConfirmed)
                    {
                        // Promoción confirmada - adjuntar BD de códigos y actualizar UI
                        _artwork = artwork.ToString();
                        LoadPromotionData();
                        break;
                    }
                    // Si no confirma, volver a pedir artwork
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al seleccionar artwork:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadPromotionData()
        {
            try
            {
                if (_currentPromotion == null)
                    return;

                // Adjuntar base de datos de códigos si es necesario
                if (!string.IsNullOrEmpty(_currentPromotion.CodesDb))
                {
                    if (!_promotionService.AttachCodesDatabase(_currentPromotion, _dbPath))
                    {
                        MessageBox.Show(
                            $"Advertencia: No se pudo adjuntar la base de datos de códigos '{_currentPromotion.CodesDb}'.\n" +
                            "La producción puede no funcionar correctamente.",
                            "Advertencia",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }

                // Actualizar controles del panel principal
                UpdatePromotionUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar datos de promoción:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdatePromotionUI()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdatePromotionUI));
                return;
            }

            if (_currentPromotion == null)
            {
                System.Diagnostics.Debug.WriteLine("UpdatePromotionUI: _currentPromotion es null");
                return;
            }

            // Recalcular ConsumedCodes desde FilesImported para asegurar que esté actualizado
            _currentPromotion.ConsumedCodes = _currentPromotion.FilesImported.Sum(f => f.Consumed);

            // Actualizar panel de Artwork
            txtArtwork.Text = _artwork ?? string.Empty;
            txtLaser.Text = _currentPromotion.LaserFile ?? string.Empty;
            txtPromotion.Text = _currentPromotion.JobName ?? string.Empty;

            // No actualizar txtCodes aquí, se actualizará cuando se seleccione un archivo en el ComboBox
            // txtCodes se actualizará en UpdateFieldsForSelectedFile()

            System.Diagnostics.Debug.WriteLine($"UpdatePromotionUI: TotalCodes={_currentPromotion.TotalCodes}, ConsumedCodes={_currentPromotion.ConsumedCodes}");

            // Configurar barra de progreso
            if (progressCodes.Maximum != 100)
            {
                progressCodes.Maximum = 100;
            }

            // Cargar archivos en el ComboBox (esto también actualizará los campos)
            LoadCodesToComboBox();

            // Limpiar campos de producción
            txtOrder.Text = string.Empty;
            txtStoppers.Text = string.Empty;
            txtProduced.Text = "0";
            txtPending.Text = "0";
            txtCode.Text = string.Empty;
            progressProd.Value = 0;
            lblProgressProd.Text = "0 %";
        }

        private void LoadCodesToComboBox()
        {
            try
            {
                // Asegurar que se ejecute en el hilo de UI
                if (InvokeRequired)
                {
                    Invoke(new Action(LoadCodesToComboBox));
                    return;
                }

                // Limpiar ComboBox
                cmboxtxtCodes.Items.Clear();
                cmboxtxtCodes.Text = string.Empty;

                if (_currentPromotion == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadCodesToComboBox: _currentPromotion es null");
                    return;
                }

                // Cargar nombres de archivos desde FilesImported
                if (_currentPromotion.FilesImported == null || _currentPromotion.FilesImported.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LoadCodesToComboBox: No hay archivos importados");
                    cmboxtxtCodes.Text = "No hay archivos disponibles";
                    return;
                }

                // Desconectar evento temporalmente para evitar actualizaciones durante la carga
                cmboxtxtCodes.SelectedIndexChanged -= cmboxtxtCodes_SelectedIndexChanged;

                cmboxtxtCodes.BeginUpdate();
                try
                {
                    foreach (var fileInfo in _currentPromotion.FilesImported)
                    {
                        if (!string.IsNullOrEmpty(fileInfo.FileName))
                        {
                            cmboxtxtCodes.Items.Add(fileInfo.FileName);
                        }
                    }
                }
                finally
                {
                    cmboxtxtCodes.EndUpdate();
                    // Reconectar evento
                    cmboxtxtCodes.SelectedIndexChanged += cmboxtxtCodes_SelectedIndexChanged;
                }

                // Si hay archivos, seleccionar el primero y actualizar campos
                if (cmboxtxtCodes.Items.Count > 0)
                {
                    cmboxtxtCodes.SelectedIndex = 0;
                    System.Diagnostics.Debug.WriteLine($"LoadCodesToComboBox: {cmboxtxtCodes.Items.Count} archivos cargados en ComboBox");
                    // Actualizar campos con el primer archivo
                    UpdateFieldsForSelectedFile();
                }
                else
                {
                    cmboxtxtCodes.Text = "No hay archivos disponibles";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar archivos en ComboBox: {ex.Message}\n{ex.StackTrace}");
                cmboxtxtCodes.Items.Clear();
                cmboxtxtCodes.Text = $"Error: {ex.Message}";
            }
        }

        private void cmboxtxtCodes_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                UpdateFieldsForSelectedFile();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en cmboxtxtCodes_SelectedIndexChanged: {ex.Message}");
            }
        }

        private void UpdateFieldsForSelectedFile()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(UpdateFieldsForSelectedFile));
                    return;
                }

                if (_currentPromotion == null || cmboxtxtCodes.SelectedIndex < 0)
                {
                    return;
                }

                string? selectedFileName = cmboxtxtCodes.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedFileName))
                {
                    return;
                }

                // Buscar el archivo seleccionado en FilesImported
                var selectedFile = _currentPromotion.FilesImported.FirstOrDefault(f => f.FileName == selectedFileName);
                if (selectedFile == null)
                {
                    return;
                }

                // Actualizar txtCodes con el TotalCodes del archivo seleccionado
                txtCodes.Text = selectedFile.TotalCodes.ToString();

                // Actualizar barra de progreso de códigos basado en el archivo seleccionado
                if (selectedFile.TotalCodes > 0)
                {
                    double consumedPercent = (selectedFile.Consumed / (double)selectedFile.TotalCodes) * 100.0;
                    int percentValue = (int)Math.Round(consumedPercent);
                    percentValue = Math.Max(0, Math.Min(100, percentValue)); // Asegurar que esté entre 0 y 100

                    // Asegurar que Maximum esté configurado
                    if (progressCodes.Maximum != 100)
                    {
                        progressCodes.Maximum = 100;
                    }

                    progressCodes.Value = percentValue;
                    lblProgressCodes.Text = $"{percentValue} %";

                    System.Diagnostics.Debug.WriteLine($"UpdateFieldsForSelectedFile: Archivo={selectedFileName}, TotalCodes={selectedFile.TotalCodes}, Consumed={selectedFile.Consumed}, Porcentaje={percentValue}%");
                }
                else
                {
                    progressCodes.Value = 0;
                    lblProgressCodes.Text = "0 %";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UpdateFieldsForSelectedFile: {ex.Message}");
            }
        }

        private AppSettings LoadAppSettings()
        {
            // Cargar configuración desde AppSettings (clase de configuración)
            // Por ahora, valores por defecto - se pueden leer desde archivo de configuración
            return new AppSettings
            {
                DataSource = "(local)\\SQLEXPRESS",
                Catalog = "IPFEu",
                UseWindowsAuthentication = true,
                User = string.Empty,
                Password = string.Empty
            };
        }

        #endregion

        #region Producción

        private void btnStart_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_currentPromotion == null)
                {
                    MessageBox.Show(
                        "No hay una promoción seleccionada.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 1. Validar cantidad a producir
                if (!int.TryParse(txtStoppers.Text, out _totalToProduce) || _totalToProduce <= 0)
                {
                    MessageBox.Show(
                        "Por favor, ingrese una cantidad válida a producir.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 2. Validar pedido (11 caracteres)
                _pedido = txtOrder.Text.Trim();
                if (_pedido.Length != 11)
                {
                    MessageBox.Show(
                        "El pedido debe tener exactamente 11 caracteres.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 3. Adjuntar base de datos de códigos si es necesario
                if (!string.IsNullOrEmpty(_currentPromotion.CodesDb))
                {
                    if (!_promotionService.AttachCodesDatabase(_currentPromotion, _dbPath))
                    {
                        MessageBox.Show(
                            $"No se pudo adjuntar la base de datos de códigos: {_currentPromotion.CodesDb}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                // 4. Inicializar láser
                string laserIP = GetLaserIPFromSettings();
                if (!_laserService.Initialize(laserIP, ".\\"))
                {
                    string error = _laserService.GetLastError();
                    MessageBox.Show(
                        $"Error al inicializar el láser:\n{error}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // 5. Copiar archivo .msf desde la PC al disco duro del láser
                string messagePath = _dbPath + "\\";  // C:\IPFEu\
                if (!_laserService.CopyMessageFile(_currentPromotion.LaserFile, messagePath))
                {
                    string error = _laserService.GetLastError();
                    MessageBox.Show(
                        $"Error al copiar archivo de mensaje al láser:\n{error}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _laserService.Stop();
                    return;
                }

                // 6. Establecer el archivo como mensaje por defecto en el láser
                if (!_laserService.SetDefaultMessage(_currentPromotion.LaserFile))
                {
                    string error = _laserService.GetLastError();
                    MessageBox.Show(
                        $"Error al establecer mensaje por defecto:\n{error}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _laserService.Stop();
                    return;
                }

                // 7. Iniciar impresión
                if (!_laserService.StartPrint(_currentPromotion.LaserFile, 1))
                {
                    string error = _laserService.GetLastError();
                    MessageBox.Show(
                        $"Error al iniciar la impresión:\n{error}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    _laserService.Stop();
                    return;
                }

                // 8. Crear e iniciar QueueService
                _queueService = new QueueService(_databaseService, _laserService, _currentPromotion);
                if (_queueService != null)
                {
                    _queueService.CodeSent += OnCodeSent;
                    _queueService.ErrorOccurred += OnQueueError;
                    _queueService.Start(_totalToProduce);
                }

                // 9. Iniciar timer de sincronización
                _syncTimer?.Start();

                // 10. Actualizar UI
                _isRunning = true;
                UpdateUIState(isRunning: true);

                // 11. Inicializar registros
                _initialRecord = 0;
                _finalRecord = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar producción:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object? sender, EventArgs e)
        {
            try
            {
                // 1. Detener timer
                _syncTimer?.Stop();

                // 2. Detener QueueService
                _queueService?.Stop();

                // 3. Detener láser
                _laserService.Stop();

                // 4. Actualizar contadores finales
                UpdateFinalCounters();

                // 5. Generar histórico
                if (_initialRecord > 0 && _finalRecord > 0)
                {
                    GenerateHistoric();
                }

                // 6. Actualizar UI
                _isRunning = false;
                UpdateUIState(isRunning: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al detener producción:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Timer de Sincronización

        private void SyncTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                if (!_isRunning || _currentPromotion == null)
                {
                    return;
                }

                // 1. Actualizar contadores
                var status = _laserService.GetStatus();
                UpdateCounters(status);

                // 2. Actualizar barras de progreso
                UpdateProgressBars();

                // 3. Actualizar porcentaje de códigos consumidos del archivo seleccionado
                UpdateCodesConsumedPercentage();

                // 4. Actualizar último código enviado
                if (_queueService != null)
                {
                    txtCode.Text = _queueService.LastSentCode;
                }

                // 5. Verificar errores de láser
                CheckLaserErrors(status);

                // 6. Verificar si producción completa
                if (IsProductionComplete())
                {
                    btnStop_Click(this, EventArgs.Empty);
                }

                // 7. Advertencias de bajo nivel
                WarningLowLevelCodes();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SyncTimer_Tick: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Auxiliares

        private void UpdateUIState(bool isRunning)
        {
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
            txtOrder.Enabled = !isRunning;
            txtStoppers.Enabled = !isRunning;
        }

        private void UpdateCounters(LaserStatus status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<LaserStatus>(UpdateCounters), status);
                return;
            }

            txtProduced.Text = status.OkCounter.ToString();
            txtPending.Text = _queueService?.PendingCodes.ToString() ?? "0";
        }

        private void UpdateProgressBars()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateProgressBars));
                return;
            }

            if (_totalToProduce > 0 && _queueService != null)
            {
                int produced = _queueService.ProducedCodes;
                int progress = (int)((produced / (double)_totalToProduce) * 100);
                progressProd.Value = Math.Min(progress, 100);
                lblProgressProd.Text = $"{progress} %";
            }
        }

        private void UpdateFinalCounters()
        {
            if (_queueService != null)
            {
                var status = _laserService.GetStatus();
                txtProduced.Text = status.OkCounter.ToString();
            }
        }

        private void CheckLaserErrors(LaserStatus status)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<LaserStatus>(CheckLaserErrors), status);
                    return;
                }

                // Verificar si hay códigos de alarma
                if (status.AlarmCodes.Count > 0)
                {
                    foreach (int alarmCode in status.AlarmCodes)
                    {
                        if (alarmCode != 0)
                        {
                            string alarmMessage = GetAlarmDescription(alarmCode);
                            System.Diagnostics.Debug.WriteLine($"Alarma detectada: {alarmCode} - {alarmMessage}");

                            // Mostrar mensaje de alarma (solo una vez por tipo de alarma)
                            if (!_activeAlarms.Contains(alarmCode))
                            {
                                _activeAlarms.Add(alarmCode);
                                ShowAlarmMessage(alarmCode, alarmMessage);
                            }
                        }
                    }
                }
                else if (status.AlarmCode != 0)
                {
                    string alarmMessage = GetAlarmDescription((int)status.AlarmCode);
                    System.Diagnostics.Debug.WriteLine($"Alarma detectada: {status.AlarmCode} - {alarmMessage}");

                    if (!_activeAlarms.Contains((int)status.AlarmCode))
                    {
                        _activeAlarms.Add((int)status.AlarmCode);
                        ShowAlarmMessage((int)status.AlarmCode, alarmMessage);
                    }
                }
                else
                {
                    // Limpiar alarmas si no hay errores
                    _activeAlarms.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CheckLaserErrors: {ex.Message}");
            }
        }

        private void LaserService_AlarmDetected(object? sender, LaserAlarmEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object?, LaserAlarmEventArgs>(LaserService_AlarmDetected), sender, e);
                return;
            }

            try
            {
                string alarmMessage = $"Alarma del láser: {e.AlarmCode} - {GetAlarmDescription(e.AlarmCode)}";
                System.Diagnostics.Debug.WriteLine(alarmMessage);
                ShowAlarmMessage(e.AlarmCode, GetAlarmDescription(e.AlarmCode));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en LaserService_AlarmDetected: {ex.Message}");
            }
        }

        private string GetAlarmDescription(int alarmCode)
        {
            // Mapeo básico de códigos de alarma comunes
            return alarmCode switch
            {
                0 => "Sin errores",
                1 => "Error de comunicación",
                2 => "Error de comunicación con el láser",
                3 => "Error de hardware",
                4 => "Error de archivo",
                5 => "Error de parámetros",
                9 => "Buffer lleno",
                10 => "Temperatura alta",
                16 => "Error de archivo láser",
                _ => $"Alarma desconocida: {alarmCode}"
            };
        }

        private void ShowAlarmMessage(int alarmCode, string message)
        {
            try
            {
                // Mostrar mensaje de alarma en un MessageBox
                MessageBox.Show(
                    $"Alarma del láser detectada:\n\nCódigo: {alarmCode}\nDescripción: {message}\n\nPor favor, verifique el estado del láser.",
                    "Alarma del Láser",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al mostrar alarma: {ex.Message}");
            }
        }

        private bool IsProductionComplete()
        {
            if (_queueService == null || _currentPromotion == null)
                return false;

            int produced = _queueService.ProducedCodes;
            int bufferCount = _laserService.GetBufferCount();

            return produced >= _totalToProduce && bufferCount == 0;
        }

        private void UpdateCodesConsumedPercentage()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(UpdateCodesConsumedPercentage));
                    return;
                }

                if (_currentPromotion == null || cmboxtxtCodes.SelectedIndex < 0)
                    return;

                string? selectedFileName = cmboxtxtCodes.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedFileName))
                    return;

                // Recargar datos de CodesIndex desde la BD para obtener valores actualizados
                if (_promotionService != null)
                {
                    _promotionService.LoadPromotionData(_currentPromotion);
                }

                var selectedFile = _currentPromotion.FilesImported.FirstOrDefault(f => f.FileName == selectedFileName);
                if (selectedFile == null)
                    return;

                // Usar el valor actualizado de Consumed desde la BD
                if (selectedFile.TotalCodes > 0)
                {
                    double consumedPercent = (selectedFile.Consumed / (double)selectedFile.TotalCodes) * 100.0;
                    int percentValue = (int)Math.Round(consumedPercent);
                    percentValue = Math.Max(0, Math.Min(100, percentValue));

                    if (progressCodes.Maximum != 100)
                    {
                        progressCodes.Maximum = 100;
                    }

                    progressCodes.Value = percentValue;
                    lblProgressCodes.Text = $"{percentValue} %";

                    // Actualizar también txtCodes con el total
                    txtCodes.Text = selectedFile.TotalCodes.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en UpdateCodesConsumedPercentage: {ex.Message}");
            }
        }

        private void WarningLowLevelCodes()
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(WarningLowLevelCodes));
                    return;
                }

                if (_currentPromotion == null)
                    return;

                // Obtener archivo seleccionado
                if (cmboxtxtCodes.SelectedIndex < 0)
                    return;

                string? selectedFileName = cmboxtxtCodes.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedFileName))
                    return;

                var selectedFile = _currentPromotion.FilesImported.FirstOrDefault(f => f.FileName == selectedFileName);
                if (selectedFile == null)
                    return;

                // Calcular códigos restantes del archivo seleccionado
                int remaining = selectedFile.TotalCodes - selectedFile.Consumed;

                // Obtener umbrales de configuración (por defecto)
                int lowLevelWarning = 1000;  // Advertencia cuando quedan menos de 1000 códigos
                int veryLowLevelWarning = 100; // Advertencia crítica cuando quedan menos de 100 códigos

                // TODO: Leer estos valores desde configuración (AppSettings)
                // lowLevelWarning = Properties.Settings.Default.LowLevelWarning;
                // veryLowLevelWarning = Properties.Settings.Default.VeryLowLevelWarning;

                // Verificar y mostrar advertencias
                if (remaining <= veryLowLevelWarning && remaining > 0)
                {
                    // Advertencia crítica - mostrar solo una vez
                    if (!_lowLevelWarningShown)
                    {
                        MessageBox.Show(
                            $"¡ADVERTENCIA CRÍTICA!\n\nQuedan solo {remaining} códigos disponibles en el archivo '{selectedFileName}'.\n\nPor favor, importe más códigos pronto.",
                            "Códigos Muy Bajo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        _lowLevelWarningShown = true;
                    }
                }
                else if (remaining <= lowLevelWarning && remaining > veryLowLevelWarning)
                {
                    // Advertencia normal - mostrar solo una vez
                    if (!_lowLevelWarningShown)
                    {
                        MessageBox.Show(
                            $"Advertencia: Quedan {remaining} códigos disponibles en el archivo '{selectedFileName}'.\n\nConsidere importar más códigos.",
                            "Códigos Bajo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        _lowLevelWarningShown = true;
                    }
                }
                else if (remaining > lowLevelWarning)
                {
                    // Resetear la bandera si hay suficientes códigos
                    _lowLevelWarningShown = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en WarningLowLevelCodes: {ex.Message}");
            }
        }

        private void GenerateHistoric()
        {
            if (_currentPromotion == null || _initialRecord == 0 || _finalRecord == 0)
            {
                return;
            }

            try
            {
                string artwork = txtArtwork.Text.Trim();
                _historyService.GenerateHistoric(
                    _initialRecord,
                    _finalRecord,
                    _currentPromotion,
                    _pedido,
                    artwork,
                    _drive);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al generar histórico: {ex.Message}");
            }
        }

        private string GetLaserIPFromSettings()
        {
            // Por ahora, valor por defecto
            // TODO: Leer desde AppSettings
            return "192.168.0.180";
        }

        private void OnCodeSent(object? sender, string code)
        {
            // Evento cuando se envía un código
            // Actualizar registros inicial y final
            if (_queueService != null)
            {
                // TODO: Obtener IDs reales de los códigos enviados
                // Por ahora, usar contadores aproximados
                if (_initialRecord == 0)
                {
                    _initialRecord = _queueService.ProducedCodes;
                }
                _finalRecord = _queueService.ProducedCodes;
            }
        }

        private void OnQueueError(object? sender, string error)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object?, string>(OnQueueError), sender, error);
                return;
            }

            // Mostrar error en UI
            System.Diagnostics.Debug.WriteLine($"Error en cola: {error}");
        }

        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Restart();

        }

        private void txtProduced_TextChanged(object sender, EventArgs e)
        {

        }

        private void Ocupado_Click(object sender, EventArgs e)
        {
            SetLaserStatus(LaserIndicator.Ocupado);
        }

        private void lblReady_Click(object sender, EventArgs e)
        {
            SetLaserStatus(LaserIndicator.Listo);
        }

        private void lblDisconnected_Click(object sender, EventArgs e)
        {
            SetLaserStatus(LaserIndicator.Desconectado);
        }
    }
}

