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
using LaserMacsaUser.Helpers;
namespace LaserMacsaUser.Views;

public partial class PromotionConfirmation : Form
{1
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
        try
        {
            if (_promotion != null)
            {
                txtPromotionId.Text = _artwork.ToString();
                txtPromotionName.Text = _promotion.JobName;

                ApplicationLogger.Log($"[PromotionConfirmation_Load] Cargando promoción: Artwork={_artwork}, Nombre={_promotion.JobName}");
            }
            else
            {
                ApplicationLogger.Log($"[PromotionConfirmation_Load] No se recibió promoción. Solo artwork={_artwork}");
            }
        }
        catch (Exception ex)
        {
            ApplicationLogger.LogError("[PromotionConfirmation_Load] Error al cargar datos", ex);
        }
    }

    private void btnOk_Click(object? sender, EventArgs e)
    {
        try
        {
            IsConfirmed = true;
            Promotion = _promotion;
            Artwork = _artwork;

            ApplicationLogger.Log($"[btnOk_Click] Confirmado: Artwork={_artwork}, Promotion={(Promotion != null ? Promotion.JobName : "null")}");

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            ApplicationLogger.LogError("[btnOk_Click] Error al confirmar promoción", ex);
            MessageBox.Show("Ocurrió un error al confirmar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        try
        {
            IsConfirmed = false;

            ApplicationLogger.Log($"[btnCancel_Click] Cancelado por el usuario. Artwork={_artwork}");

            DialogResult = DialogResult.Cancel;
            Close();
        }
        catch (Exception ex)
        {
            ApplicationLogger.LogError("[btnCancel_Click] Error al cancelar", ex);
            MessageBox.Show("Ocurrió un error al cancelar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
