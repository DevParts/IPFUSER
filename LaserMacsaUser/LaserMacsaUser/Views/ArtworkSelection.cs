using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LaserMacsaUser.Helpers;
namespace LaserMacsaUser.Resources
{
    public partial class ArtworkSelection : Form
    {
        public enum Mode
        {
            Activate,
            Repeat
        }

        public string ArtworkNumber => txtArtworkNumber.Text.Trim();
        private readonly Mode _mode;

        public ArtworkSelection(Mode mode)
        {
            try
            {
                InitializeComponent();
                _mode = mode;
                ConfigureMode();

                ApplicationLogger.Log("ArtworkSelection iniciado correctamente.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Error al inicializar ArtworkSelection", ex);
            }
        }

        private void ConfigureMode()
        {
            try
            {
                if (_mode == Mode.Activate)
                {
                    lblTitle.Text = "Artwork Number to Activate";
                    ApplicationLogger.Log("Modo configurado: Activate. Título establecido correctamente.");
                }
                else
                {
                    lblTitle.Text = "Repeat Artwork Number";
                    ApplicationLogger.Log("Modo configurado: Repeat. Título establecido correctamente.");
                }

                ApplicationLogger.Log($"ArtworkSelection configurado correctamente en modo: {_mode}.");
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Error al configurar ArtworkSelection", ex);
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {


                {
                    if (string.IsNullOrWhiteSpace(txtArtworkNumber.Text))
                    {
                        MessageBox.Show("Please enter the artwork number.", "Missing Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }

                    ApplicationLogger.Log($"Artwork confirmado: {txtArtworkNumber.Text.Trim()}");

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError("Error al confirmar artwork ", ex);
                MessageBox.Show("Ha ocurrido un error al procesar la operación.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ApplicationLogger.Log("ArtworkSelection: Usuario canceló la selección.");
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
