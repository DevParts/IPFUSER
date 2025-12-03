namespace LaserMacsaUser.Views.Login
{
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
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
            resources.ApplyResources(lblTitle, "lblTitle");
            lblTitle.ForeColor = Color.FromArgb(150, 0, 0);
            lblTitle.Name = "lblTitle";
            // 
            // pictureLogo
            // 
            resources.ApplyResources(pictureLogo, "pictureLogo");
            pictureLogo.Image = Properties.Resources.MacsaLogo;
            pictureLogo.Name = "pictureLogo";
            pictureLogo.TabStop = false;
            // 
            // lblPassword
            // 
            resources.ApplyResources(lblPassword, "lblPassword");
            lblPassword.Name = "lblPassword";
            // 
            // txtPassword
            // 
            resources.ApplyResources(txtPassword, "txtPassword");
            txtPassword.Name = "txtPassword";
            txtPassword.KeyDown += txtPassword_KeyDown;
            // 
            // btnLogin
            // 
            resources.ApplyResources(btnLogin, "btnLogin");
            btnLogin.BackColor = Color.Firebrick;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.ForeColor = Color.White;
            btnLogin.Name = "btnLogin";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // LoginForm
            // 
            resources.ApplyResources(this, "$this");
            BackColor = Color.White;
            Controls.Add(lblTitle);
            Controls.Add(pictureLogo);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
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