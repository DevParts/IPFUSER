using LaserMacsaUser.Views.AppInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserMacsaUser.Views
{
    public partial class AppConfigForm : Form
    {
        private AppSettings? _settingsBefore;

        public AppConfigForm()
        {
            InitializeComponent();
            LoadSettings();
            btnOk.Click += BtnOk_Click;
        }

        private void LoadSettings()
        {
            // Clonar valores actuales para detectar cambios
            _settingsBefore = new AppSettings()
            {
                AppPassword = Properties.Settings.Default.AppPassword
            };

            // Mostrar en el PropertyGrid
            propertyGridConfig.SelectedObject = new AppSettings()
            {
                AppPassword = Properties.Settings.Default.AppPassword
            };
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Obtener valores modificados
            var updated = (AppSettings)propertyGridConfig.SelectedObject;

            bool passwordChanged = updated.AppPassword != _settingsBefore?.AppPassword;

            // Guardar configuraciones
            Properties.Settings.Default.AppPassword = updated.AppPassword;
            Properties.Settings.Default.Save();

            // Si la contraseña cambió, reiniciar
            if (passwordChanged)
            {
                var result = MessageBox.Show(
                    "La contraseña se ha modificado.\nSe necesita reiniciar la aplicación para aplicar los cambios.\n\n¿Desea reiniciar ahora?",
                    "Reinicio necesario",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }

            this.Close();
        }
    }
}