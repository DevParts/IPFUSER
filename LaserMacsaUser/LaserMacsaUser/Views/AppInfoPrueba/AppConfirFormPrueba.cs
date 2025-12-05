using LaserMacsaUser.Views.AppInfo;
using LaserMacsaUser.Views.AppInfoPrueba;
using LaserMacsaUser.Services;
using LaserMacsaUser.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserMacsaUser.Views.AppInfoPrueba
{
    public partial class AppConfirFormPrueba : Form
    {
        private AppSettingsPrueba? _settingsBefore;
        private Promotion? _currentPromotion;
        private IDatabaseService? _databaseService;
        private IPromotionService? _promotionService;
        private string _dbPath = string.Empty;

        public AppConfirFormPrueba()
        {
            InitializeComponent();
            LoadSettings();
            btnOk.Click += BtnOk_Click;
            btnValidateCodes.Click += BtnValidateCodes_Click;
            
            // Suscribirse al evento de cambio de propiedad para guardar inmediatamente
            propertyGridConfig.PropertyValueChanged += PropertyGridConfig_PropertyValueChanged;
        }

        /// <summary>
        /// Establece la promoción actual y servicios necesarios para la validación
        /// </summary>
        public void SetPromotionAndServices(Promotion? promotion, IDatabaseService? databaseService, IPromotionService? promotionService, string dbPath = "")
        {
            _currentPromotion = promotion;
            _databaseService = databaseService;
            _promotionService = promotionService;
            _dbPath = dbPath;
            
            // Habilitar/deshabilitar botón según disponibilidad
            btnValidateCodes.Enabled = promotion != null && databaseService != null;
        }
        
        private void PropertyGridConfig_PropertyValueChanged(object? s, PropertyValueChangedEventArgs e)
        {
            // Cuando se cambia una propiedad en el PropertyGrid, guardar inmediatamente
            if (propertyGridConfig.SelectedObject is AppSettingsPrueba settings)
            {
                // Los setters ya escribieron en Properties.Settings.Default
                // Guardar inmediatamente para que los cambios se persistan
                Properties.Settings.Default.Save();
            }
        }

        private void LoadSettings()
        {
            // Cargar valores actuales desde Properties.Settings
            // Los valores se cargan automáticamente a través de los getters
            _settingsBefore = new AppSettingsPrueba();

            // Mostrar en el PropertyGrid con valores actuales
            // Los valores se cargan automáticamente desde Properties.Settings a través de los getters
            propertyGridConfig.SelectedObject = new AppSettingsPrueba();
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Guardar configuración cuando se presiona OK
            if (propertyGridConfig.SelectedObject is AppSettingsPrueba settings)
            {
                // Forzar la actualización de los valores desde el PropertyGrid
                // El PropertyGrid puede no haber llamado a los setters todavía
                propertyGridConfig.Refresh();
                
                // Leer los valores actuales del objeto y guardarlos explícitamente
                Properties.Settings.Default.LaserIP = settings.LaserIP;
                Properties.Settings.Default.LaserBufferSize = settings.LaserBufferSize;
                Properties.Settings.Default.WaitTimeBufferFull = settings.WaitTimeBufferFull;
                
                // Guardar todos los cambios
                Properties.Settings.Default.Save();
            }
            
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void BtnValidateCodes_Click(object? sender, EventArgs e)
        {
            try
            {
                // Verificar que haya promoción seleccionada
                if (_currentPromotion == null)
                {
                    MessageBox.Show(
                        "No hay una promoción seleccionada.\n\n" +
                        "Por favor, seleccione una promoción en la pantalla principal antes de validar códigos.",
                        "Validación de Códigos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Verificar que el servicio de base de datos esté disponible
                if (_databaseService == null)
                {
                    MessageBox.Show(
                        "El servicio de base de datos no está disponible.\n\n" +
                        "Por favor, asegúrese de que la conexión a la base de datos esté configurada correctamente.",
                        "Validación de Códigos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Verificar y conectar base de datos de códigos si es necesario
                if (!string.IsNullOrEmpty(_currentPromotion.CodesDb))
                {
                    bool isAttached = _databaseService.IsDatabaseAttached(_currentPromotion.CodesDb);
                    
                    if (!isAttached)
                    {
                        // Intentar adjuntar la base de datos
                        if (_promotionService != null && !string.IsNullOrEmpty(_dbPath))
                        {
                            var result = MessageBox.Show(
                                $"La base de datos de códigos '{_currentPromotion.CodesDb}' no está adjunta.\n\n" +
                                "¿Desea intentar adjuntarla ahora?",
                                "Base de Datos no Adjunta",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                if (!_promotionService.AttachCodesDatabase(_currentPromotion, _dbPath))
                                {
                                    MessageBox.Show(
                                        $"No se pudo adjuntar la base de datos '{_currentPromotion.CodesDb}'.\n\n" +
                                        "Por favor, verifique que la base de datos exista en la ruta configurada.",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    return;
                                }
                                
                                // Después de adjuntar, asegurar que esté conectada
                                // AttachCodesDatabase intenta conectar, pero si falla silenciosamente, 
                                // debemos intentar conectar explícitamente aquí
                                try
                                {
                                    System.Diagnostics.Debug.WriteLine($"Conectando a BD después de adjuntar: '{_currentPromotion.CodesDb}'");
                                    _databaseService.ConnectCodesDatabase(_currentPromotion.CodesDb);
                                    System.Diagnostics.Debug.WriteLine($"BD conectada después de adjuntar.");
                                }
                                catch (Exception connectEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error al conectar después de adjuntar: {connectEx.Message}");
                                    MessageBox.Show(
                                        $"La base de datos se adjuntó correctamente, pero no se pudo conectar.\n\n" +
                                        $"Error: {connectEx.Message}\n\n" +
                                        "Por favor, verifique la conexión al servidor SQL.",
                                        "Error de Conexión",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                $"La base de datos de códigos '{_currentPromotion.CodesDb}' no está adjunta.\n\n" +
                                "Por favor, adjunte la base de datos desde la pantalla principal primero.",
                                "Base de Datos no Adjunta",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    
                    // Asegurar que esté conectada (no solo adjunta)
                    // SIEMPRE intentar conectar explícitamente, incluso si IsCodesDatabaseConnected retorna true
                    // Esto asegura que _codesDbName esté establecido correctamente
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Intentando conectar a base de datos de códigos '{_currentPromotion.CodesDb}'...");
                        _databaseService.ConnectCodesDatabase(_currentPromotion.CodesDb);
                        System.Diagnostics.Debug.WriteLine($"Base de datos de códigos '{_currentPromotion.CodesDb}' conectada correctamente.");
                        
                        // Verificar que realmente se estableció la conexión
                        if (!_databaseService.IsCodesDatabaseConnected())
                        {
                            System.Diagnostics.Debug.WriteLine($"ERROR: ConnectCodesDatabase retornó true, pero IsCodesDatabaseConnected retorna false!");
                            MessageBox.Show(
                                $"Error: La conexión no se estableció correctamente después de ConnectCodesDatabase.\n\n" +
                                $"Por favor, intente nuevamente o verifique la configuración de la base de datos.",
                                "Error de Conexión",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        System.Diagnostics.Debug.WriteLine($"Verificación: IsCodesDatabaseConnected retorna true. Continuando...");
                    }
                    catch (Exception ex)
                    {
                        // Si falla la conexión, mostrar error y no continuar
                        System.Diagnostics.Debug.WriteLine($"Error al conectar a BD de códigos: {ex.Message}");
                        MessageBox.Show(
                            $"No se pudo conectar a la base de datos de códigos '{_currentPromotion.CodesDb}'.\n\n" +
                            $"Error: {ex.Message}\n\n" +
                            "Por favor, verifique que:\n" +
                            "- La base de datos esté correctamente adjunta\n" +
                            "- El servidor SQL esté ejecutándose\n" +
                            "- Las credenciales sean correctas",
                            "Error de Conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                // ⬇️ AGREGAR ESTA VERIFICACIÓN ADICIONAL ⬇️
                // Verificar que la promoción tenga una base de datos de códigos configurada
                if (string.IsNullOrEmpty(_currentPromotion.CodesDb))
                {
                    MessageBox.Show(
                        "La promoción seleccionada no tiene una base de datos de códigos configurada.\n\n" +
                        "Por favor, verifique la configuración de la promoción en la base de datos principal.",
                        "Configuración Incompleta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                System.Diagnostics.Debug.WriteLine($"Promoción tiene CodesDb: '{_currentPromotion.CodesDb}'");
                // ⬆️ FIN VERIFICACIÓN ADICIONAL ⬆️

                // ⬇️ VERIFICACIÓN FINAL ANTES DE VALIDAR ⬇️
                // Verificación final antes de validar - asegurar que la conexión esté establecida
                if (!_databaseService.IsCodesDatabaseConnected())
                {
                    System.Diagnostics.Debug.WriteLine($"ADVERTENCIA: Conexión perdida antes de validar. Reintentando conectar...");
                    
                    // ⬇️ AGREGAR ESTA VERIFICACIÓN CRÍTICA ⬇️
                    // Verificar que tenemos la información necesaria para reconectar
                    if (_currentPromotion == null || string.IsNullOrEmpty(_currentPromotion.CodesDb))
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR: No se puede reconectar - _currentPromotion es null o CodesDb está vacío.");
                        MessageBox.Show(
                            $"Error: No se puede reconectar a la base de datos porque la información de la promoción no está disponible.\n\n" +
                            $"Por favor, cierre este formulario y seleccione una promoción nuevamente en la pantalla principal.",
                            "Error de Configuración",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    // ⬆️ FIN VERIFICACIÓN CRÍTICA ⬆️
                    
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Reintentando conectar a '{_currentPromotion.CodesDb}'...");
                        _databaseService.ConnectCodesDatabase(_currentPromotion.CodesDb);
                        System.Diagnostics.Debug.WriteLine($"Reconexión exitosa.");
                    }
                    catch (Exception reconnectEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al reconectar: {reconnectEx.Message}");
                        MessageBox.Show(
                            $"Error: La conexión a la base de datos se perdió y no se pudo reconectar.\n\n" +
                            $"Error: {reconnectEx.Message}\n\n" +
                            "Por favor, verifique la configuración de la base de datos y vuelva a intentar.",
                            "Error de Conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Verificación final: Conexión OK antes de validar.");
                }
                // ⬆️ FIN VERIFICACIÓN FINAL ⬆️

                // Mostrar mensaje de "Validando..."
                Cursor = Cursors.WaitCursor;
                btnValidateCodes.Enabled = false;

                try
                {
                    // Validar los primeros 10 códigos
                    var validationResult = CodeValidator.ValidateCodes(_currentPromotion, _databaseService, 10);

                    // Mostrar resultado en MessageBox
                    string message = validationResult.Summary;
                    
                    MessageBoxIcon icon = validationResult.IsValid 
                        ? MessageBoxIcon.Information 
                        : MessageBoxIcon.Warning;

                    string title = validationResult.IsValid 
                        ? "Validación Exitosa" 
                        : "Validación con Errores";

                    MessageBox.Show(
                        message,
                        title,
                        MessageBoxButtons.OK,
                        icon);
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnValidateCodes.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al validar códigos:\n\n{ex.Message}",
                    "Error de Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
  