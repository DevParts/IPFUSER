using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LaserMacsaUser.Helpers;
namespace LaserMacsaUser.Views
{
    public partial class SplashForm : Form
    {
        int progress = 0;

        public SplashForm()
        {
            InitializeComponent();

            ApplicationLogger.Log("Se ha iniciado la aplicación."); // Log de inicio de la app

            // Inicia con 0%
            lblPercent.Text = "0 %";
            lblStatus.Text = "Cargando...";
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progress += 2; // velocidad de carga
            progressBar1.Value = progress;
            lblPercent.Text = $"{progress} %";

            // Puedes ir cambiando el mensaje
            if (progress == 10) lblStatus.Text = "Inicializando...";
            if (progress == 30) lblStatus.Text = "Cargando módulos...";
            if (progress == 60) lblStatus.Text = "Preparando entorno...";
            if (progress == 80) lblStatus.Text = "Listo...";

            // Cuando llega al 100% abre el Form principal
            if (progress >= 100)
            {
                timer1.Stop();

                Form1 main = new Form1(); // tu form principal
                main.Show();

                this.Hide(); // oculta Splash
            }
        }
    }
}
