namespace LaserMacsaUser.Resources
{
    partial class ArtworkSelection
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtArtworkNumber;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnExit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArtworkSelection));
            lblTitle = new Label();
            txtArtworkNumber = new TextBox();
            btnOk = new Button();
            btnExit = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(30, 25);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(320, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Artwork Number to Activate";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtArtworkNumber
            // 
            txtArtworkNumber.Font = new Font("Segoe UI", 11F);
            txtArtworkNumber.Location = new Point(90, 70);
            txtArtworkNumber.MaxLength = 10;
            txtArtworkNumber.Name = "txtArtworkNumber";
            txtArtworkNumber.Size = new Size(200, 27);
            txtArtworkNumber.TabIndex = 1;
            txtArtworkNumber.TextAlign = HorizontalAlignment.Center;
            // 
            // btnOk
            // 
            btnOk.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOk.ImageAlign = ContentAlignment.TopCenter;
            btnOk.Location = new Point(90, 120);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(100, 60);
            btnOk.TabIndex = 2;
            btnOk.Text = "Ok";
            btnOk.TextAlign = ContentAlignment.BottomCenter;
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnExit
            // 
            btnExit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnExit.ImageAlign = ContentAlignment.TopCenter;
            btnExit.Location = new Point(210, 120);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(100, 60);
            btnExit.TabIndex = 3;
            btnExit.Text = "Exit";
            btnExit.TextAlign = ContentAlignment.BottomCenter;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // ArtworkSelection
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(380, 210);
            Controls.Add(btnExit);
            Controls.Add(btnOk);
            Controls.Add(txtArtworkNumber);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ArtworkSelection";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Artwork Selection";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}