using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace LaserMacsaUser.Views
{
    public partial class SplashForm : Form
    {
        int progress = 0;
        private ComponentResourceManager RM;

        public SplashForm()
        {
            InitializeComponent();

            // Cargar recursos del formulario según idioma seleccionado
            RM = new ComponentResourceManager(typeof(SplashForm));

            // Inicia con valores del idioma actual
            lblPercent.Text = "0 %";
            lblStatus.Text = RM.GetString("LoadingStart");
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progress += 2;
            progressBar1.Value = progress;
            lblPercent.Text = $"{progress} %";

            // Cambiar textos según progreso usando recursos del RESX del formulario
            if (progress == 10) lblStatus.Text = RM.GetString("LoadingInit");
            if (progress == 30) lblStatus.Text = RM.GetString("LoadingModules");
            if (progress == 60) lblStatus.Text = RM.GetString("LoadingEnv");
            if (progress == 80) lblStatus.Text = RM.GetString("LoadingReady");

            // Al llegar a 100%
            if (progress >= 100)
            {
                timer1.Stop();

                Form1 main = new Form1();
                main.Show();

                this.Hide();
            }
        }
    }
}
