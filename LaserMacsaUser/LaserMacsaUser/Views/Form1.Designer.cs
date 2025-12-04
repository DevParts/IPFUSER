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
            resources.ApplyResources(pnlArtwork, "pnlArtwork");
            pnlArtwork.Name = "pnlArtwork";
            // 
            // cmboxtxtCodes
            // 
            cmboxtxtCodes.FormattingEnabled = true;
            resources.ApplyResources(cmboxtxtCodes, "cmboxtxtCodes");
            cmboxtxtCodes.Name = "cmboxtxtCodes";
            // 
            // lblHeaderArtwork
            // 
            lblHeaderArtwork.BackColor = Color.DarkRed;
            resources.ApplyResources(lblHeaderArtwork, "lblHeaderArtwork");
            lblHeaderArtwork.ForeColor = Color.White;
            lblHeaderArtwork.Name = "lblHeaderArtwork";
            // 
            // lblArtwork
            // 
            resources.ApplyResources(lblArtwork, "lblArtwork");
            lblArtwork.Name = "lblArtwork";
            // 
            // txtArtwork
            // 
            txtArtwork.BackColor = Color.Yellow;
            resources.ApplyResources(txtArtwork, "txtArtwork");
            txtArtwork.Name = "txtArtwork";
            txtArtwork.ReadOnly = true;
            // 
            // lblLaser
            // 
            resources.ApplyResources(lblLaser, "lblLaser");
            lblLaser.Name = "lblLaser";
            // 
            // txtLaser
            // 
            resources.ApplyResources(txtLaser, "txtLaser");
            txtLaser.Name = "txtLaser";
            txtLaser.ReadOnly = true;
            // 
            // picLogo
            // 
            picLogo.Image = Properties.Resources.MacsaLogo;
            resources.ApplyResources(picLogo, "picLogo");
            picLogo.Name = "picLogo";
            picLogo.TabStop = false;
            // 
            // lblPromotion
            // 
            resources.ApplyResources(lblPromotion, "lblPromotion");
            lblPromotion.Name = "lblPromotion";
            // 
            // txtPromotion
            // 
            resources.ApplyResources(txtPromotion, "txtPromotion");
            txtPromotion.Name = "txtPromotion";
            txtPromotion.ReadOnly = true;
            // 
            // lblCodes
            // 
            resources.ApplyResources(lblCodes, "lblCodes");
            lblCodes.Name = "lblCodes";
            lblCodes.Click += lblCodes_Click;
            // 
            // txtCodes
            // 
            resources.ApplyResources(txtCodes, "txtCodes");
            txtCodes.Name = "txtCodes";
            txtCodes.ReadOnly = true;
            // 
            // progressCodes
            // 
            resources.ApplyResources(progressCodes, "progressCodes");
            progressCodes.Name = "progressCodes";
            // 
            // lblProgressCodes
            // 
            resources.ApplyResources(lblProgressCodes, "lblProgressCodes");
            lblProgressCodes.Name = "lblProgressCodes";
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
            resources.ApplyResources(pnlProduction, "pnlProduction");
            pnlProduction.Name = "pnlProduction";
            // 
            // lblHeaderProd
            // 
            lblHeaderProd.BackColor = Color.DarkRed;
            resources.ApplyResources(lblHeaderProd, "lblHeaderProd");
            lblHeaderProd.ForeColor = Color.White;
            lblHeaderProd.Name = "lblHeaderProd";
            // 
            // lblStoppers
            // 
            resources.ApplyResources(lblStoppers, "lblStoppers");
            lblStoppers.Name = "lblStoppers";
            // 
            // txtStoppers
            // 
            resources.ApplyResources(txtStoppers, "txtStoppers");
            txtStoppers.Name = "txtStoppers";
            // 
            // lblPending
            // 
            resources.ApplyResources(lblPending, "lblPending");
            lblPending.Name = "lblPending";
            // 
            // txtPending
            // 
            txtPending.BackColor = Color.LightCyan;
            resources.ApplyResources(txtPending, "txtPending");
            txtPending.Name = "txtPending";
            // 
            // lblProduced
            // 
            resources.ApplyResources(lblProduced, "lblProduced");
            lblProduced.Name = "lblProduced";
            // 
            // txtProduced
            // 
            resources.ApplyResources(txtProduced, "txtProduced");
            txtProduced.Name = "txtProduced";
            // 
            // lblOrder
            // 
            resources.ApplyResources(lblOrder, "lblOrder");
            lblOrder.Name = "lblOrder";
            // 
            // txtOrder
            // 
            resources.ApplyResources(txtOrder, "txtOrder");
            txtOrder.Name = "txtOrder";
            // 
            // lblCode
            // 
            resources.ApplyResources(lblCode, "lblCode");
            lblCode.Name = "lblCode";
            // 
            // txtCode
            // 
            resources.ApplyResources(txtCode, "txtCode");
            txtCode.Name = "txtCode";
            // 
            // lblPromoIndex
            // 
            resources.ApplyResources(lblPromoIndex, "lblPromoIndex");
            lblPromoIndex.Name = "lblPromoIndex";
            // 
            // txtPromoIndex
            // 
            resources.ApplyResources(txtPromoIndex, "txtPromoIndex");
            txtPromoIndex.Name = "txtPromoIndex";
            // 
            // progressProd
            // 
            resources.ApplyResources(progressProd, "progressProd");
            progressProd.Name = "progressProd";
            // 
            // lblProgressProd
            // 
            resources.ApplyResources(lblProgressProd, "lblProgressProd");
            lblProgressProd.Name = "lblProgressProd";
            // 
            // btnStart
            // 
            resources.ApplyResources(btnStart, "btnStart");
            btnStart.Image = Properties.Resources.start30x30;
            btnStart.Name = "btnStart";
            // 
            // btnStop
            // 
            resources.ApplyResources(btnStop, "btnStop");
            btnStop.Image = Properties.Resources.stop30x301;
            btnStop.Name = "btnStop";
            // 
            // btnExit
            // 
            resources.ApplyResources(btnExit, "btnExit");
            btnExit.Image = Properties.Resources.exit30x30;
            btnExit.Name = "btnExit";
            btnExit.Click += btnExit_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { configurationToolStripMenuItem, configPruebaToolStripMenuItem });
            resources.ApplyResources(menuStrip1, "menuStrip1");
            menuStrip1.Name = "menuStrip1";
            // 
            // configurationToolStripMenuItem
            // 
            configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            resources.ApplyResources(configurationToolStripMenuItem, "configurationToolStripMenuItem");
            configurationToolStripMenuItem.Click += configurationToolStripMenuItem_Click;
            // 
            // configPruebaToolStripMenuItem
            // 
            configPruebaToolStripMenuItem.Name = "configPruebaToolStripMenuItem";
            resources.ApplyResources(configPruebaToolStripMenuItem, "configPruebaToolStripMenuItem");
            configPruebaToolStripMenuItem.Click += configPruebaToolStripMenuItem_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            Controls.Add(pnlArtwork);
            Controls.Add(pnlProduction);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
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
