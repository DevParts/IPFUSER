namespace LaserMacsaUser.Views
{
    partial class AppConfigForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PropertyGrid propertyGridConfig;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnRunTests;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.propertyGridConfig = new System.Windows.Forms.PropertyGrid();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.btnRunTests = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Form Settings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 500);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Application Setup";
            this.BackColor = System.Drawing.Color.White;
            // 
            // propertyGridConfig
            // 
            this.propertyGridConfig.Location = new System.Drawing.Point(10, 10);
            this.propertyGridConfig.Name = "propertyGridConfig";
            this.propertyGridConfig.Size = new System.Drawing.Size(430, 380);
            this.propertyGridConfig.ToolbarVisible = false;
            this.propertyGridConfig.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridConfig.HelpVisible = true;
            // Configurar fuente consistente para evitar problemas de visualización
            this.propertyGridConfig.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Text = "Probar Conexi�n";
            this.btnTestConnection.Location = new System.Drawing.Point(10, 400);
            this.btnTestConnection.Size = new System.Drawing.Size(120, 28);
            this.btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.System;
            // 
            // btnRunTests
            // 
            this.btnRunTests.Text = "Ejecutar Tests";
            this.btnRunTests.Location = new System.Drawing.Point(140, 400);
            this.btnRunTests.Size = new System.Drawing.Size(120, 28);
            this.btnRunTests.FlatStyle = System.Windows.Forms.FlatStyle.System;
            // 
            // btnOk
            // 
            this.btnOk.Text = "Ok";
            this.btnOk.Location = new System.Drawing.Point(355, 455);
            this.btnOk.Size = new System.Drawing.Size(85, 28);
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            // 
            // Add Controls
            // 
            this.Controls.Add(this.propertyGridConfig);
            this.Controls.Add(this.btnTestConnection);
            this.Controls.Add(this.btnRunTests);
            this.Controls.Add(this.btnOk);
            this.ResumeLayout(false);
        }
    }
}

