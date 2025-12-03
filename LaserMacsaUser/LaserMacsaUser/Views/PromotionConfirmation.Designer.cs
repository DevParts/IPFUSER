namespace LaserMacsaUser.Views
{
    partial class PromotionConfirmation
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.TextBox txtPromotionId;

        private System.Windows.Forms.Label labelPromotionName;
        private System.Windows.Forms.TextBox txtPromotionName;

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromotionConfirmation));
            labelTitle = new Label();
            txtPromotionId = new TextBox();
            labelPromotionName = new Label();
            txtPromotionName = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // labelTitle
            // 
            resources.ApplyResources(labelTitle, "labelTitle");
            labelTitle.Name = "labelTitle";
            // 
            // txtPromotionId
            // 
            resources.ApplyResources(txtPromotionId, "txtPromotionId");
            txtPromotionId.BackColor = Color.FromArgb(255, 255, 200);
            txtPromotionId.Name = "txtPromotionId";
            txtPromotionId.ReadOnly = true;
            // 
            // labelPromotionName
            // 
            resources.ApplyResources(labelPromotionName, "labelPromotionName");
            labelPromotionName.Name = "labelPromotionName";
            // 
            // txtPromotionName
            // 
            resources.ApplyResources(txtPromotionName, "txtPromotionName");
            txtPromotionName.BackColor = Color.FromArgb(255, 240, 215);
            txtPromotionName.Name = "txtPromotionName";
            txtPromotionName.ReadOnly = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(btnOk, "btnOk");
            btnOk.BackColor = Color.WhiteSmoke;
            btnOk.Image = Properties.Resources.ok30x30_png;
            btnOk.Name = "btnOk";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Image = Properties.Resources.cancel30x30;
            btnCancel.Name = "btnCancel";
            // 
            // PromotionConfirmation
            // 
            resources.ApplyResources(this, "$this");
            BackColor = Color.WhiteSmoke;
            Controls.Add(labelTitle);
            Controls.Add(txtPromotionId);
            Controls.Add(labelPromotionName);
            Controls.Add(txtPromotionName);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromotionConfirmation";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}