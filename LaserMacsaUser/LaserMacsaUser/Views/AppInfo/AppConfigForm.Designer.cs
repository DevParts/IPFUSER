namespace LaserMacsaUser.Views
{
    partial class AppConfigForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PropertyGrid propertyGridConfig;
        private System.Windows.Forms.Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppConfigForm));
            propertyGridConfig = new PropertyGrid();
            btnOk = new Button();
            SuspendLayout();
            // 
            // propertyGridConfig
            // 
            propertyGridConfig.Location = new Point(10, 10);
            propertyGridConfig.Name = "propertyGridConfig";
            propertyGridConfig.PropertySort = PropertySort.Categorized;
            propertyGridConfig.Size = new Size(430, 440);
            propertyGridConfig.TabIndex = 0;
            propertyGridConfig.ToolbarVisible = false;
            // 
            // btnOk
            // 
            btnOk.FlatStyle = FlatStyle.System;
            btnOk.Location = new Point(355, 455);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(85, 28);
            btnOk.TabIndex = 1;
            btnOk.Text = "Ok";
            // 
            // AppConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(450, 500);
            Controls.Add(propertyGridConfig);
            Controls.Add(btnOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AppConfigForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Application Setup";
            ResumeLayout(false);
        }
    }
}