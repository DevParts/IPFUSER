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
            InitializeComponent();
            _mode = mode;
            ConfigureMode();
            this.FormClosing += ArtworkSelection_FormClosing;
        }

        private void ArtworkSelection_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Si el usuario cierra con la X y no ha presionado ningún botón, cancelar
            if (DialogResult == DialogResult.None)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void ConfigureMode()
        {
            if (_mode == Mode.Activate)
            {
                lblTitle.Text = "Artwork Number to Activate";
            }
            else
            {
                lblTitle.Text = "Repeat Artwork Number";
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtArtworkNumber.Text))
            {
                MessageBox.Show("Please enter the artwork number.", "Missing Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArtworkNumber.Focus();
                return;
            }

            // Validar que sea numérico
            if (!int.TryParse(txtArtworkNumber.Text, out _))
            {
                MessageBox.Show("Please enter a valid numeric artwork number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtArtworkNumber.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
