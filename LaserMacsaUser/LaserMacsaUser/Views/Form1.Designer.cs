namespace LaserMacsaUser.Views
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pnlArtwork = new Panel();
            cmboxtxtCodes = new ComboBox();
            lblHeaderArtwork = new Label();
            lblArtwork = new Label();
            txtArtwork = new TextBox();
            lblLaser = new Label();
            txtLaser = new TextBox();
            picLogo = new PictureBox();
            lblPromotion = new Label();
            txtPromotion = new TextBox();
            lblCodes = new Label();
            txtCodes = new TextBox();
            progressCodes = new ProgressBar();
            lblProgressCodes = new Label();
            pnlProduction = new Panel();
            lblHeaderProd = new Label();
            lblStoppers = new Label();
            txtStoppers = new TextBox();
            lblPending = new Label();
            txtPending = new TextBox();
            lblProduced = new Label();
            txtProduced = new TextBox();
            lblOrder = new Label();
            txtOrder = new TextBox();
            lblCode = new Label();
            txtCode = new TextBox();
            lblPromoIndex = new Label();
            txtPromoIndex = new TextBox();
            progressProd = new ProgressBar();
            lblProgressProd = new Label();
            btnStart = new Button();
            btnStop = new Button();
            btnExit = new Button();
            menuStrip1 = new MenuStrip();
            configurationToolStripMenuItem = new ToolStripMenuItem();
            configPruebaToolStripMenuItem = new ToolStripMenuItem();
            pnlArtwork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            pnlProduction.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // pnlArtwork
            // 
            pnlArtwork.BackColor = Color.White;
            pnlArtwork.BorderStyle = BorderStyle.FixedSingle;
            pnlArtwork.Controls.Add(cmboxtxtCodes);
            pnlArtwork.Controls.Add(lblHeaderArtwork);
            pnlArtwork.Controls.Add(lblArtwork);
            pnlArtwork.Controls.Add(txtArtwork);
            pnlArtwork.Controls.Add(lblLaser);
            pnlArtwork.Controls.Add(txtLaser);
            pnlArtwork.Controls.Add(picLogo);
            pnlArtwork.Controls.Add(lblPromotion);
            pnlArtwork.Controls.Add(txtPromotion);
            pnlArtwork.Controls.Add(lblCodes);
            pnlArtwork.Controls.Add(txtCodes);
            pnlArtwork.Controls.Add(progressCodes);
            pnlArtwork.Controls.Add(lblProgressCodes);
            pnlArtwork.Location = new Point(10, 45);
            pnlArtwork.Name = "pnlArtwork";
            pnlArtwork.Size = new Size(970, 220);
            pnlArtwork.TabIndex = 1;
            // 
            // cmboxtxtCodes
            // 
            cmboxtxtCodes.DropDownStyle = ComboBoxStyle.DropDown;
            cmboxtxtCodes.FormattingEnabled = true;
            cmboxtxtCodes.Location = new Point(722, 3);
            cmboxtxtCodes.Name = "cmboxtxtCodes";
            cmboxtxtCodes.Size = new Size(233, 23);
            cmboxtxtCodes.TabIndex = 12;
            // 
            // lblHeaderArtwork
            // 
            lblHeaderArtwork.BackColor = Color.DarkRed;
            lblHeaderArtwork.Dock = DockStyle.Top;
            lblHeaderArtwork.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderArtwork.ForeColor = Color.White;
            lblHeaderArtwork.Location = new Point(0, 0);
            lblHeaderArtwork.Name = "lblHeaderArtwork";
            lblHeaderArtwork.Size = new Size(968, 30);
            lblHeaderArtwork.TabIndex = 0;
            lblHeaderArtwork.Text = "Active Artwork";
            lblHeaderArtwork.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblArtwork
            // 
            lblArtwork.Font = new Font("Segoe UI", 10F);
            lblArtwork.Location = new Point(20, 50);
            lblArtwork.Name = "lblArtwork";
            lblArtwork.Size = new Size(74, 23);
            lblArtwork.TabIndex = 1;
            lblArtwork.Text = "Artwork :";
            // 
            // txtArtwork
            // 
            txtArtwork.BackColor = Color.Yellow;
            txtArtwork.Location = new Point(110, 48);
            txtArtwork.Name = "txtArtwork";
            txtArtwork.ReadOnly = true;
            txtArtwork.Size = new Size(150, 23);
            txtArtwork.TabIndex = 2;
            // 
            // lblLaser
            // 
            lblLaser.Font = new Font("Segoe UI", 10F);
            lblLaser.Location = new Point(280, 50);
            lblLaser.Name = "lblLaser";
            lblLaser.Size = new Size(74, 23);
            lblLaser.TabIndex = 3;
            lblLaser.Text = "Laser File:";
            // 
            // txtLaser
            // 
            txtLaser.Location = new Point(360, 48);
            txtLaser.Name = "txtLaser";
            txtLaser.ReadOnly = true;
            txtLaser.Size = new Size(310, 23);
            txtLaser.TabIndex = 4;
            // 
            // picLogo
            // 
            picLogo.Image = Properties.Resources.MacsaLogo;
            picLogo.Location = new Point(722, 83);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(233, 70);
            picLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            picLogo.TabIndex = 5;
            picLogo.TabStop = false;
            // 
            // lblPromotion
            // 
            lblPromotion.Font = new Font("Segoe UI", 10F);
            lblPromotion.Location = new Point(20, 90);
            lblPromotion.Name = "lblPromotion";
            lblPromotion.Size = new Size(87, 23);
            lblPromotion.TabIndex = 6;
            lblPromotion.Text = "Promotion:";
            // 
            // txtPromotion
            // 
            txtPromotion.Location = new Point(110, 88);
            txtPromotion.Name = "txtPromotion";
            txtPromotion.ReadOnly = true;
            txtPromotion.Size = new Size(560, 23);
            txtPromotion.TabIndex = 7;
            // 
            // lblCodes
            // 
            lblCodes.Font = new Font("Segoe UI", 10F);
            lblCodes.Location = new Point(20, 130);
            lblCodes.Name = "lblCodes";
            lblCodes.Size = new Size(111, 23);
            lblCodes.TabIndex = 8;
            lblCodes.Text = "Available Codes:";
            lblCodes.Click += lblCodes_Click;
            // 
            // txtCodes
            // 
            txtCodes.Location = new Point(137, 130);
            txtCodes.Name = "txtCodes";
            txtCodes.ReadOnly = true;
            txtCodes.Size = new Size(533, 23);
            txtCodes.TabIndex = 9;
            // 
            // progressCodes
            // 
            progressCodes.Location = new Point(20, 175);
            progressCodes.Maximum = 100;
            progressCodes.Name = "progressCodes";
            progressCodes.Size = new Size(520, 25);
            progressCodes.TabIndex = 10;
            // 
            // lblProgressCodes
            // 
            lblProgressCodes.Font = new Font("Segoe UI", 9F);
            lblProgressCodes.Location = new Point(546, 177);
            lblProgressCodes.Name = "lblProgressCodes";
            lblProgressCodes.Size = new Size(47, 23);
            lblProgressCodes.TabIndex = 11;
            lblProgressCodes.Text = "00 %";
            // 
            // pnlProduction
            // 
            pnlProduction.BackColor = Color.White;
            pnlProduction.BorderStyle = BorderStyle.FixedSingle;
            pnlProduction.Controls.Add(lblHeaderProd);
            pnlProduction.Controls.Add(lblStoppers);
            pnlProduction.Controls.Add(txtStoppers);
            pnlProduction.Controls.Add(lblPending);
            pnlProduction.Controls.Add(txtPending);
            pnlProduction.Controls.Add(lblProduced);
            pnlProduction.Controls.Add(txtProduced);
            pnlProduction.Controls.Add(lblOrder);
            pnlProduction.Controls.Add(txtOrder);
            pnlProduction.Controls.Add(lblCode);
            pnlProduction.Controls.Add(txtCode);
            pnlProduction.Controls.Add(lblPromoIndex);
            pnlProduction.Controls.Add(txtPromoIndex);
            pnlProduction.Controls.Add(progressProd);
            pnlProduction.Controls.Add(lblProgressProd);
            pnlProduction.Controls.Add(btnStart);
            pnlProduction.Controls.Add(btnStop);
            pnlProduction.Controls.Add(btnExit);
            pnlProduction.Location = new Point(10, 280);
            pnlProduction.Name = "pnlProduction";
            pnlProduction.Size = new Size(970, 280);
            pnlProduction.TabIndex = 2;
            // 
            // lblHeaderProd
            // 
            lblHeaderProd.BackColor = Color.DarkRed;
            lblHeaderProd.Dock = DockStyle.Top;
            lblHeaderProd.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblHeaderProd.ForeColor = Color.White;
            lblHeaderProd.Location = new Point(0, 0);
            lblHeaderProd.Name = "lblHeaderProd";
            lblHeaderProd.Size = new Size(968, 30);
            lblHeaderProd.TabIndex = 0;
            lblHeaderProd.Text = "Production";
            lblHeaderProd.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStoppers
            // 
            lblStoppers.Font = new Font("Segoe UI", 10F);
            lblStoppers.Location = new Point(20, 60);
            lblStoppers.Name = "lblStoppers";
            lblStoppers.Size = new Size(100, 23);
            lblStoppers.TabIndex = 1;
            lblStoppers.Text = "Stoppers to Produce";
            // 
            // txtStoppers
            // 
            txtStoppers.Location = new Point(149, 59);
            txtStoppers.Name = "txtStoppers";
            txtStoppers.Size = new Size(303, 23);
            txtStoppers.TabIndex = 2;
            // 
            // lblPending
            // 
            lblPending.Font = new Font("Segoe UI", 10F);
            lblPending.Location = new Point(520, 40);
            lblPending.Name = "lblPending";
            lblPending.Size = new Size(100, 23);
            lblPending.TabIndex = 3;
            lblPending.Text = "Pending";
            // 
            // txtPending
            // 
            txtPending.BackColor = Color.LightCyan;
            txtPending.Location = new Point(520, 65);
            txtPending.Name = "txtPending";
            txtPending.Size = new Size(190, 23);
            txtPending.TabIndex = 4;
            // 
            // lblProduced
            // 
            lblProduced.Font = new Font("Segoe UI", 10F);
            lblProduced.Location = new Point(763, 39);
            lblProduced.Name = "lblProduced";
            lblProduced.Size = new Size(100, 23);
            lblProduced.TabIndex = 5;
            lblProduced.Text = "Produced";
            // 
            // txtProduced
            // 
            txtProduced.Location = new Point(763, 65);
            txtProduced.Name = "txtProduced";
            txtProduced.Size = new Size(190, 23);
            txtProduced.TabIndex = 6;
            // 
            // lblOrder
            // 
            lblOrder.Font = new Font("Segoe UI", 10F);
            lblOrder.Location = new Point(20, 110);
            lblOrder.Name = "lblOrder";
            lblOrder.Size = new Size(63, 23);
            lblOrder.TabIndex = 7;
            lblOrder.Text = "Order:";
            // 
            // txtOrder
            // 
            txtOrder.Location = new Point(149, 108);
            txtOrder.Name = "txtOrder";
            txtOrder.Size = new Size(303, 23);
            txtOrder.TabIndex = 8;
            // 
            // lblCode
            // 
            lblCode.Font = new Font("Segoe UI", 10F);
            lblCode.Location = new Point(520, 133);
            lblCode.Name = "lblCode";
            lblCode.Size = new Size(100, 23);
            lblCode.TabIndex = 9;
            lblCode.Text = "Code:";
            // 
            // txtCode
            // 
            txtCode.Enabled = false;
            txtCode.Location = new Point(520, 159);
            txtCode.Name = "txtCode";
            txtCode.Size = new Size(433, 23);
            txtCode.TabIndex = 10;
            // 
            // lblPromoIndex
            // 
            lblPromoIndex.Font = new Font("Segoe UI", 10F);
            lblPromoIndex.Location = new Point(20, 159);
            lblPromoIndex.Name = "lblPromoIndex";
            lblPromoIndex.Size = new Size(123, 23);
            lblPromoIndex.TabIndex = 11;
            lblPromoIndex.Text = "Promotion Index:";
            // 
            // txtPromoIndex
            // 
            txtPromoIndex.Enabled = false;
            txtPromoIndex.Location = new Point(149, 158);
            txtPromoIndex.Name = "txtPromoIndex";
            txtPromoIndex.Size = new Size(303, 23);
            txtPromoIndex.TabIndex = 12;
            txtPromoIndex.Visible = false;
            // 
            // progressProd
            // 
            progressProd.Location = new Point(20, 217);
            progressProd.Name = "progressProd";
            progressProd.Size = new Size(520, 25);
            progressProd.TabIndex = 13;
            // 
            // lblProgressProd
            // 
            lblProgressProd.Font = new Font("Segoe UI", 9F);
            lblProgressProd.Location = new Point(546, 219);
            lblProgressProd.Name = "lblProgressProd";
            lblProgressProd.Size = new Size(47, 23);
            lblProgressProd.TabIndex = 14;
            lblProgressProd.Text = "00 %";
            // 
            // btnStart
            // 
            btnStart.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStart.ImageAlign = ContentAlignment.TopCenter;
            btnStart.Location = new Point(613, 196);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(100, 70);
            btnStart.TabIndex = 15;
            btnStart.Text = "Start";
            btnStart.TextAlign = ContentAlignment.BottomCenter;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnStop.ImageAlign = ContentAlignment.TopCenter;
            btnStop.Location = new Point(733, 196);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(100, 70);
            btnStop.TabIndex = 16;
            btnStop.Text = "Stop";
            btnStop.TextAlign = ContentAlignment.BottomCenter;
            // 
            // btnExit
            // 
            btnExit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnExit.ImageAlign = ContentAlignment.TopCenter;
            btnExit.Location = new Point(853, 196);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(100, 70);
            btnExit.TabIndex = 17;
            btnExit.Text = "Exit";
            btnExit.TextAlign = ContentAlignment.BottomCenter;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { configurationToolStripMenuItem, configPruebaToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1000, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // configurationToolStripMenuItem
            // 
            configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            configurationToolStripMenuItem.Size = new Size(93, 20);
            configurationToolStripMenuItem.Text = "Configuration";
            configurationToolStripMenuItem.Click += configurationToolStripMenuItem_Click;
            // 
            // configPruebaToolStripMenuItem
            // 
            configPruebaToolStripMenuItem.Name = "configPruebaToolStripMenuItem";
            configPruebaToolStripMenuItem.Size = new Size(95, 20);
            configPruebaToolStripMenuItem.Text = "Config Prueba";
            configPruebaToolStripMenuItem.Click += configPruebaToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1000, 600);
            Controls.Add(pnlArtwork);
            Controls.Add(pnlProduction);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Integra CSI - Code Marker";
            pnlArtwork.ResumeLayout(false);
            pnlArtwork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            pnlProduction.ResumeLayout(false);
            pnlProduction.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        private Panel pnlArtwork;
        private Label lblHeaderArtwork;
        private Label lblArtwork;
        private TextBox txtArtwork;
        private Label lblLaser;
        private TextBox txtLaser;
        private PictureBox picLogo;
        private Label lblPromotion;
        private TextBox txtPromotion;
        private Label lblCodes;
        private TextBox txtCodes;
        private ProgressBar progressCodes;
        private Panel pnlProduction;
        private Label lblHeaderProd;
        private Label lblStoppers;
        private TextBox txtStoppers;
        private Label lblPending;
        private TextBox txtPending;
        private Label lblProduced;
        private TextBox txtProduced;
        private Label lblOrder;
        private TextBox txtOrder;
        private Label lblCode;
        private TextBox txtCode;
        private Label lblPromoIndex;
        private TextBox txtPromoIndex;
        private ProgressBar progressProd;
        private Label lblProgressProd;
        private Button btnStart;
        private Button btnStop;
        private Button btnExit;

        #endregion

        private Label lblProgressCodes;
        private ComboBox cmboxtxtCodes;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem configurationToolStripMenuItem;
        private ToolStripMenuItem configPruebaToolStripMenuItem;
    }
}
