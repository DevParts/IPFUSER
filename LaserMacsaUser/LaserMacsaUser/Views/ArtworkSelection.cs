using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            InitializeComponent();
            _mode = mode;
            ConfigureMode();
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
