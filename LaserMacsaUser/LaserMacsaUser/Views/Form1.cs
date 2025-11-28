using LaserMacsaUser.Views.AppInfoPrueba;
using LaserMacsaUser.Views.Login;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void configPruebaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppConfirFormPrueba formAppInfoPrueba = new AppConfirFormPrueba();
            formAppInfoPrueba.ShowDialog();
        }
    }
}
