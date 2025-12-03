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
            this.components = new System.ComponentModel.Container();
            this.pictureLogo = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblCopy = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);

            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
            this.SuspendLayout();

            // 
            // pictureLogo
            // 
            this.pictureLogo.Location = new System.Drawing.Point(150, 20);
            this.pictureLogo.Name = "pictureLogo";
            this.pictureLogo.Size = new System.Drawing.Size(300, 200);
            this.pictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureLogo.Image = Properties.Resources.MacsaLogo;
            // 🔥 Asegúrate de agregar la imagen a Resources y nombrarla macsa_logo

            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 230);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(600, 40);
            this.lblTitle.Text = "Integra Promotions Full User";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(50, 300);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(500, 20);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;

            // 
            // lblCopy
            // 
            this.lblCopy.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCopy.Location = new System.Drawing.Point(50, 330);
            this.lblCopy.Name = "lblCopy";
            this.lblCopy.Size = new System.Drawing.Size(200, 20);
            this.lblCopy.Text = "Copyright © 2012";

            // 
            // lblVersion
            // 
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVersion.Location = new System.Drawing.Point(350, 330);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(200, 20);
            this.lblVersion.Text = "Version 1.00";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;

            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStatus.Location = new System.Drawing.Point(50, 360);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(300, 20);
            this.lblStatus.Text = "Data Access...";

            // 
            // lblPercent
            // 
            this.lblPercent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPercent.Location = new System.Drawing.Point(450, 360);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(100, 20);
            this.lblPercent.Text = "0 %";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // timer1
            // 
            this.timer1.Interval = 80;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 420);
            this.Controls.Add(this.pictureLogo);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblCopy);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblPercent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.White;
            this.Name = "SplashForm";
            this.Text = "SplashForm";
            this.Load += new System.EventHandler(this.SplashForm_Load);

            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
