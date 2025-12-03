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
            resources.ApplyResources(lblTitle, "lblTitle");
            lblTitle.Name = "lblTitle";
            lblTitle.Click += lblTitle_Click;
            // 
            // txtArtworkNumber
            // 
            resources.ApplyResources(txtArtworkNumber, "txtArtworkNumber");
            txtArtworkNumber.Name = "txtArtworkNumber";
            // 
            // btnOk
            // 
            resources.ApplyResources(btnOk, "btnOk");
            btnOk.Image = Properties.Resources.ok30x30_png;
            btnOk.Name = "btnOk";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnExit
            // 
            resources.ApplyResources(btnExit, "btnExit");
            btnExit.Image = Properties.Resources.exit30x30;
            btnExit.Name = "btnExit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // ArtworkSelection
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            Controls.Add(btnExit);
            Controls.Add(btnOk);
            Controls.Add(txtArtworkNumber);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ArtworkSelection";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}