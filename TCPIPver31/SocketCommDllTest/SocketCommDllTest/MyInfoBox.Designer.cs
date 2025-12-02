namespace SocketCommDllTest
{
    partial class MyInfoBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyInfoBox));
            this.inf_image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.inf_image)).BeginInit();
            this.SuspendLayout();
            // 
            // inf_image
            // 
            this.inf_image.Location = new System.Drawing.Point(0, 0);
            this.inf_image.Margin = new System.Windows.Forms.Padding(0);
            this.inf_image.Name = "inf_image";
            this.inf_image.Padding = new System.Windows.Forms.Padding(12);
            this.inf_image.Size = new System.Drawing.Size(100, 50);
            this.inf_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.inf_image.TabIndex = 0;
            this.inf_image.TabStop = false;
            // 
            // MyInfoBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(120, 71);
            this.Controls.Add(this.inf_image);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(800, 700);
            this.Name = "MyInfoBox";
            this.Text = "MyInfoBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MyInfoBox_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.inf_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox inf_image;
    }
}