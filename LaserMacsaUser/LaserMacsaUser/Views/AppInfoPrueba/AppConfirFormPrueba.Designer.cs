namespace LaserMacsaUser.Views.AppInfoPrueba
{
    partial class AppConfirFormPrueba
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
            propertyGridConfig = new PropertyGrid();
            btnOk = new Button();
            label1 = new Label();
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 466);
            label1.Name = "label1";
            label1.Size = new Size(80, 25);
            label1.TabIndex = 2;
            label1.Text = "PRUEBA";
            label1.Click += label1_Click;
            // 
            // AppConfirFormPrueba
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(450, 500);
            Controls.Add(label1);
            Controls.Add(propertyGridConfig);
            Controls.Add(btnOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AppConfirFormPrueba";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PRUEBA Application Setup";
            ResumeLayout(false);
            PerformLayout();
        }
        private Label label1;
    }
}