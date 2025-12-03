namespace LaserMacsaUser.Views
{
    partial class SplashForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.PictureBox pictureLogo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblCopy;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Timer timer1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
            pictureLogo = new PictureBox();
            lblTitle = new Label();
            progressBar1 = new ProgressBar();
            lblCopy = new Label();
            lblVersion = new Label();
            lblStatus = new Label();
            lblPercent = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureLogo).BeginInit();
            SuspendLayout();
            // 
            // pictureLogo
            // 
            pictureLogo.Image = Properties.Resources.MacsaLogo;
            resources.ApplyResources(pictureLogo, "pictureLogo");
            pictureLogo.Name = "pictureLogo";
            pictureLogo.TabStop = false;
            // 
            // lblTitle
            // 
            resources.ApplyResources(lblTitle, "lblTitle");
            lblTitle.Name = "lblTitle";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            progressBar1.Style = ProgressBarStyle.Continuous;
            // 
            // lblCopy
            // 
            resources.ApplyResources(lblCopy, "lblCopy");
            lblCopy.Name = "lblCopy";
            // 
            // lblVersion
            // 
            resources.ApplyResources(lblVersion, "lblVersion");
            lblVersion.Name = "lblVersion";
            // 
            // lblStatus
            // 
            resources.ApplyResources(lblStatus, "lblStatus");
            lblStatus.Name = "lblStatus";
            // 
            // lblPercent
            // 
            resources.ApplyResources(lblPercent, "lblPercent");
            lblPercent.Name = "lblPercent";
            // 
            // timer1
            // 
            timer1.Interval = 80;
            timer1.Tick += timer1_Tick;
            // 
            // SplashForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            Controls.Add(pictureLogo);
            Controls.Add(lblTitle);
            Controls.Add(progressBar1);
            Controls.Add(lblCopy);
            Controls.Add(lblVersion);
            Controls.Add(lblStatus);
            Controls.Add(lblPercent);
            FormBorderStyle = FormBorderStyle.None;
            Name = "SplashForm";
            Load += SplashForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureLogo).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
