using LaserMacsaUser.Views.AppInfo;
using System;
using System.Windows.Forms;

namespace LaserMacsaUser.Views
{
    public partial class AppConfigForm : Form
    {
        private string? _originalLanguage;
        private string? _originalPassword;

        public AppConfigForm()
        {
            InitializeComponent();

            btnOk.Click += BtnOk_Click;

            LoadSettings();
            ApplyLanguage();
        }

        private void LoadSettings()
        {
            _originalLanguage = Properties.Settings.Default.Language;
            _originalPassword = Properties.Settings.Default.AppPassword;

            propertyGridConfig.SelectedObject = new AppSettings();
        }

        private void ApplyLanguage()
        {
            string lang = Properties.Settings.Default.Language;

            if (lang == "Español")
                this.Text = "Configuración de la Aplicación";
            else
                this.Text = "Application Settings";
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            string lang = Properties.Settings.Default.Language;

            if (propertyGridConfig.SelectedObject is not AppSettings)
            {
                if (lang == "Español")
                    MessageBox.Show("No se pudo leer la configuración. Intente de nuevo.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Could not read configuration. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            string? newLanguage = Properties.Settings.Default.Language;
            string? newPassword = Properties.Settings.Default.AppPassword;

            bool passwordChanged = !string.Equals(newPassword, _originalPassword, StringComparison.Ordinal);
            bool languageChanged = !string.Equals(newLanguage, _originalLanguage, StringComparison.Ordinal);

            Properties.Settings.Default.Save();

            // -------- CAMBIO DE IDIOMA --------
            if (languageChanged)
            {
                DialogResult result;

                if (lang == "Español")
                {
                    result = MessageBox.Show(
                        "El idioma se modificó.\nSe necesita reiniciar la aplicación para aplicar los cambios.\n\n¿Desea reiniciar ahora?",
                        "Reinicio necesario",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    result = MessageBox.Show(
                        "Language was changed.\nThe application must restart to apply the changes.\n\nDo you want to restart now?",
                        "Restart required",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                }

                if (result == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            // -------- CAMBIO DE CONTRASEÑA --------
            if (passwordChanged)
            {
                DialogResult result;

                if (lang == "Español")
                {
                    result = MessageBox.Show(
                        "La contraseña se ha modificado.\nSe necesita reiniciar la aplicación para aplicar los cambios.\n\n¿Desea reiniciar ahora?",
                        "Reinicio necesario",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    result = MessageBox.Show(
                        "The password has been modified.\nThe application must restart to apply the changes.\n\nDo you want to restart now?",
                        "Restart required",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );
                }

                if (result == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            this.Close();
        }
    }
}
