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
            labelTitle.AutoSize = true;
            labelTitle.Font = new Font("Segoe UI", 20F);
            labelTitle.Location = new Point(25, 20);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(249, 37);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Sure to activate this";
            // 
            // txtPromotionId
            // 
            txtPromotionId.BackColor = Color.FromArgb(255, 255, 200);
            txtPromotionId.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtPromotionId.Location = new Point(310, 23);
            txtPromotionId.Name = "txtPromotionId";
            txtPromotionId.ReadOnly = true;
            txtPromotionId.Size = new Size(245, 36);
            txtPromotionId.TabIndex = 2;
            txtPromotionId.TextAlign = HorizontalAlignment.Center;
            // 
            // labelPromotionName
            // 
            labelPromotionName.AutoSize = true;
            labelPromotionName.Font = new Font("Segoe UI", 11F);
            labelPromotionName.Location = new Point(25, 83);
            labelPromotionName.Name = "labelPromotionName";
            labelPromotionName.Size = new Size(79, 20);
            labelPromotionName.TabIndex = 3;
            labelPromotionName.Text = "Promotion";
            // 
            // txtPromotionName
            // 
            txtPromotionName.BackColor = Color.FromArgb(255, 240, 215);
            txtPromotionName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            txtPromotionName.Location = new Point(25, 122);
            txtPromotionName.Name = "txtPromotionName";
            txtPromotionName.ReadOnly = true;
            txtPromotionName.Size = new Size(560, 36);
            txtPromotionName.TabIndex = 4;
            txtPromotionName.TextAlign = HorizontalAlignment.Center;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.White;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 10F);
            btnOk.ImageAlign = ContentAlignment.TopCenter;
            btnOk.Location = new Point(158, 168);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(125, 80);
            btnOk.TabIndex = 5;
            btnOk.Text = "ok";
            btnOk.TextAlign = ContentAlignment.BottomCenter;
            btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.ImageAlign = ContentAlignment.TopCenter;
            btnCancel.Location = new Point(310, 168);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(125, 80);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.TextAlign = ContentAlignment.BottomCenter;
            // 
            // PromotionConfirmation
            // 
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(600, 260);
            Controls.Add(labelTitle);
            Controls.Add(txtPromotionId);
            Controls.Add(labelPromotionName);
            Controls.Add(txtPromotionName);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromotionConfirmation";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Promotion Confirmation";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}