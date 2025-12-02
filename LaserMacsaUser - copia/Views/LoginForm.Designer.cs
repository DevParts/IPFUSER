
namespace LaserMacsaUser.Views {
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            pictureLogo = new PictureBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureLogo).BeginInit();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(150, 0, 0);
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(360, 60);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ACCESS PANEL";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureLogo
            // 
            pictureLogo.Image = Properties.Resources.MacsaLogo;
            pictureLogo.Location = new Point(80, 80);
            pictureLogo.Name = "pictureLogo";
            pictureLogo.Size = new Size(200, 120);
            pictureLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureLogo.TabIndex = 1;
            pictureLogo.TabStop = false;
            // 
            // lblPassword
            // 
            lblPassword.Font = new Font("Segoe UI", 11F);
            lblPassword.Location = new Point(40, 230);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(280, 22);
            lblPassword.TabIndex = 2;
            lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.Location = new Point(40, 260);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '?';
            txtPassword.Size = new Size(280, 27);
            txtPassword.TabIndex = 3;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);

            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.Firebrick;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI Semibold", 11F);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(40, 310);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(280, 40);
            btnLogin.TabIndex = 4;
            btnLogin.Text = "ENTER";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // 
            // LoginForm
            // 
            BackColor = Color.White;
            ClientSize = new Size(360, 420);
            Controls.Add(lblTitle);
            Controls.Add(pictureLogo);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LaserMacsa Login";
            ((System.ComponentModel.ISupportInitialize)pictureLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pictureLogo;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
    }
}

