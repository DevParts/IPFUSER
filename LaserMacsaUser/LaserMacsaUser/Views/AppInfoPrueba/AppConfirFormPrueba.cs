using LaserMacsaUser.Views.AppInfo;
using LaserMacsaUser.Views.AppInfoPrueba;
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

        public AppConfirFormPrueba()
        {
            InitializeComponent();
            LoadSettings();
            btnOk.Click += BtnOk_Click;
        }

        private void LoadSettings()
        {
            // Clonar valores actuales para detectar cambios
            _settingsBefore = new AppSettingsPrueba()
            {
            };

            // Mostrar en el PropertyGrid
            propertyGridConfig.SelectedObject = new AppSettingsPrueba()
            {
            };


        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
  