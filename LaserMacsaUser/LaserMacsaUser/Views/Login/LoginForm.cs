using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserMacsaUser.Views.Login
{
    public partial class LoginForm : Form
    {
        // Resultado de autenticación
        public bool IsAuthenticated { get; private set; } = false;

        public LoginForm()
        {
            InitializeComponent();

            // Para que Enter ejecute el login
            this.AcceptButton = btnLogin;

            // Opcional: limpiar y enfocar
            txtPassword.Clear();
            txtPassword.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            ValidatePassword();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidatePassword();
                e.SuppressKeyPress = true; // Evita beep
            }
        }

        private void ValidatePassword()
        {
            string lang = Properties.Settings.Default.Language ?? "English";

            string msgIncorrect;
            string titleIncorrect;

            if (lang == "Español")
            {
                msgIncorrect = "Contraseña incorrecta.";
                titleIncorrect = "Acceso denegado";
            }
            else
            {
                msgIncorrect = "Incorrect password.";
                titleIncorrect = "Access Denied";
            }

            if (txtPassword.Text.Trim() == Properties.Settings.Default.AppPassword)
            {
                IsAuthenticated = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(msgIncorrect, titleIncorrect,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtPassword.Clear();
                txtPassword.Focus();
                IsAuthenticated = false;
            }
        }


    }
}