using System;
using System.Drawing;
using System.Windows.Forms;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Views
{
    public partial class ConfirmPromotionForm : Form
    {
        public Promotion? ConfirmedPromotion { get; private set; }
        public bool IsConfirmed { get; private set; }

        private Label lblIntro = null!;
        private Label lblArtworkId = null!;
        private Label lblPromotionName = null!;
        private Button btnOk = null!;
        private Button btnCancel = null!;
        private Label lblLoading = null!;

        public ConfirmPromotionForm(int artworkId, string promotionName)
        {
            InitializeComponent();
            lblArtworkId!.Text = artworkId.ToString();
            lblPromotionName!.Text = promotionName;
        }

        private void InitializeComponent()
        {
            lblIntro = new Label();
            lblArtworkId = new Label();
            lblPromotionName = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            lblLoading = new Label();
            SuspendLayout();

            // lblIntro
            lblIntro.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            lblIntro.Location = new System.Drawing.Point(30, 20);
            lblIntro.Size = new System.Drawing.Size(400, 30);
            lblIntro.Text = "Sure to activate this";
            lblIntro.TextAlign = ContentAlignment.MiddleCenter;

            // lblArtworkId
            lblArtworkId.BackColor = Color.Yellow;
            lblArtworkId.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblArtworkId.Location = new Point(30, 70);
            lblArtworkId.Size = new Size(400, 40);
            lblArtworkId.TextAlign = ContentAlignment.MiddleCenter;

            // lblPromotionName
            lblPromotionName.BackColor = Color.Orange;
            lblPromotionName.Font = new Font("Segoe UI", 12F);
            lblPromotionName.Location = new Point(30, 120);
            lblPromotionName.Size = new Size(400, 40);
            lblPromotionName.TextAlign = ContentAlignment.MiddleCenter;

            // lblLoading
            lblLoading.Font = new System.Drawing.Font("Segoe UI", 10F);
            lblLoading.Location = new System.Drawing.Point(30, 180);
            lblLoading.Size = new System.Drawing.Size(400, 30);
            lblLoading.Text = "Loading codes...";
            lblLoading.Visible = false;

            // btnOk
            btnOk.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
            btnOk.Location = new System.Drawing.Point(100, 230);
            btnOk.Size = new System.Drawing.Size(120, 50);
            btnOk.Text = "OK";
            btnOk.Click += BtnOk_Click;

            // btnCancel
            btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.Location = new System.Drawing.Point(250, 230);
            btnCancel.Size = new System.Drawing.Size(120, 50);
            btnCancel.Text = "Cancel";
            btnCancel.Click += BtnCancel_Click;

            // Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(460, 300);
            Controls.Add(lblIntro);
            Controls.Add(lblArtworkId);
            Controls.Add(lblPromotionName);
            Controls.Add(lblLoading);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Confirm Promotion";
            ResumeLayout(false);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            btnOk.Enabled = false;
            btnCancel.Enabled = false;
            lblLoading.Visible = true;
            Application.DoEvents();

            IsConfirmed = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            IsConfirmed = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

