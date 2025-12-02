using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Views
{
    public partial class PromotionConfirmation : Form
    {
        public bool IsConfirmed { get; private set; } = false;
        public Promotion? Promotion { get; private set; }
        public int Artwork { get; private set; }

        private readonly int _artwork;
        private readonly Promotion? _promotion;

        public PromotionConfirmation(int artwork, Promotion? promotion)
        {
            InitializeComponent();
            _artwork = artwork;
            _promotion = promotion;
            
            // Conectar eventos
            this.Load += PromotionConfirmation_Load;
            btnOk.Click += btnOk_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void PromotionConfirmation_Load(object? sender, EventArgs e)
        {
            if (_promotion != null)
            {
                txtPromotionId.Text = _artwork.ToString();
                txtPromotionName.Text = _promotion.JobName;
            }
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            IsConfirmed = true;
            Promotion = _promotion;
            Artwork = _artwork;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            IsConfirmed = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
