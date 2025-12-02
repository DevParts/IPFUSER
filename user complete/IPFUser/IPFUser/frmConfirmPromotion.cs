using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IPFUser.My;
using IPFUser.My.Resources;
using MacsaDevicesNet;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[DesignerGenerated]
public class frmConfirmPromotion : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Button1")]
	private Button _Button1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Button2")]
	private Button _Button2;

	[field: AccessedThroughProperty("lblIntroArt")]
	internal virtual Label lblIntroArt
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblDescription")]
	internal virtual Label lblDescription
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label1")]
	internal virtual Label Label1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblPromo")]
	internal virtual Label lblPromo
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button Button1
	{
		[CompilerGenerated]
		get
		{
			return _Button1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = Button1_Click;
			Button button = _Button1;
			if (button != null)
			{
				button.Click -= value2;
			}
			_Button1 = value;
			button = _Button1;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	internal virtual Button Button2
	{
		[CompilerGenerated]
		get
		{
			return _Button2;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = Button2_Click;
			Button button = _Button2;
			if (button != null)
			{
				button.Click -= value2;
			}
			_Button2 = value;
			button = _Button2;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	[field: AccessedThroughProperty("lblLoadingCodes")]
	internal virtual Label lblLoadingCodes
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblPLCAd")]
	internal virtual Label lblPLCAd
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public frmConfirmPromotion()
	{
		base.Load += ConfirmPromotion_Load;
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}

	[System.Diagnostics.DebuggerStepThrough]
	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPFUser.frmConfirmPromotion));
		this.lblIntroArt = new System.Windows.Forms.Label();
		this.lblDescription = new System.Windows.Forms.Label();
		this.Label1 = new System.Windows.Forms.Label();
		this.lblPromo = new System.Windows.Forms.Label();
		this.lblLoadingCodes = new System.Windows.Forms.Label();
		this.Button2 = new System.Windows.Forms.Button();
		this.Button1 = new System.Windows.Forms.Button();
		this.lblPLCAd = new System.Windows.Forms.Label();
		base.SuspendLayout();
		resources.ApplyResources(this.lblIntroArt, "lblIntroArt");
		this.lblIntroArt.Name = "lblIntroArt";
		this.lblDescription.BackColor = System.Drawing.Color.PapayaWhip;
		this.lblDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.lblDescription, "lblDescription");
		this.lblDescription.Name = "lblDescription";
		resources.ApplyResources(this.Label1, "Label1");
		this.Label1.Name = "Label1";
		this.lblPromo.BackColor = System.Drawing.Color.FromArgb(255, 255, 128);
		this.lblPromo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.lblPromo, "lblPromo");
		this.lblPromo.Name = "lblPromo";
		this.lblLoadingCodes.BackColor = System.Drawing.Color.Yellow;
		this.lblLoadingCodes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.lblLoadingCodes, "lblLoadingCodes");
		this.lblLoadingCodes.Name = "lblLoadingCodes";
		resources.ApplyResources(this.Button2, "Button2");
		this.Button2.Image = IPFUser.My.Resources.Resources.Delete;
		this.Button2.Name = "Button2";
		this.Button2.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.Button1, "Button1");
		this.Button1.Image = IPFUser.My.Resources.Resources.Check;
		this.Button1.Name = "Button1";
		this.Button1.UseVisualStyleBackColor = true;
		this.lblPLCAd.BackColor = System.Drawing.Color.Orange;
		this.lblPLCAd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		resources.ApplyResources(this.lblPLCAd, "lblPLCAd");
		this.lblPLCAd.Name = "lblPLCAd";
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lblPLCAd);
		base.Controls.Add(this.lblLoadingCodes);
		base.Controls.Add(this.Button2);
		base.Controls.Add(this.Button1);
		base.Controls.Add(this.lblPromo);
		base.Controls.Add(this.Label1);
		base.Controls.Add(this.lblDescription);
		base.Controls.Add(this.lblIntroArt);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmConfirmPromotion";
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void Button1_Click(object sender, EventArgs e)
	{
		Cursor.Current = Cursors.WaitCursor;
		Button1.Enabled = false;
		Button2.Enabled = false;
		lblLoadingCodes.Visible = true;
		lblLoadingCodes.Refresh();
		MyProject.Forms.frmMain.PreparePromotion();
		base.Tag = true;
		Cursor.Current = Cursors.Default;
		Close();
	}

	private void Button2_Click(object sender, EventArgs e)
	{
		base.Tag = false;
		Close();
	}

	private void ConfirmPromotion_Load(object sender, EventArgs e)
	{
		lblPromo.Text = Conversions.ToString(AppCSIUser.PossibleArtwork);
		SqlDataReader Dr = AppCSIUser.Db.GetDataReader("Select * from Jobs inner join Artworks on JobId=IdJob where Artwork=" + AppCSIUser.PossibleArtwork);
		Dr.Read();
		lblDescription.Text = Conversions.ToString(Dr["JobName"]);
		Common.MACSALog("Select Promotion " + Dr["JobName"].ToString(), TraceEventType.Information);
		if (Conversions.ToBoolean(Operators.AndObject(Conversions.ToDouble(AppCSIUser.LastJobId) > 0.0, Operators.CompareObjectNotEqual(Dr["JobId"], AppCSIUser.LastJobId, TextCompare: false))))
		{
			Common.MACSALog("Dettach DB due to promotion change " + AppCSIUser.LastJobId.ToString() + " to " + Dr["JobId"].ToString(), TraceEventType.Information);
			SqlDataReader Dr2 = AppCSIUser.Db.GetDataReader("Select CodesDB from Jobs where JobId=" + AppCSIUser.LastJobId.ToString());
			if (Dr2.HasRows)
			{
				Dr2.Read();
				try
				{
					Common.MACSALog(Conversions.ToString(Operators.ConcatenateObject("Detach Db ", Dr2["CodesDb"])), TraceEventType.Information);
					AppCSIUser.Db.DetachDB(Path.GetFileNameWithoutExtension(Conversions.ToString(Dr2["CodesDb"])));
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Common.MACSALog(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ERROR Detach Db ", Dr2["CodesDb"]), ":"), ex2.Message)), TraceEventType.Error);
					ProjectData.ClearProjectError();
				}
			}
			Dr2.Close();
		}
		AppCSIUser.LastJobId = Conversions.ToString(Dr["JobId"]);
		AppCSIUser.Promo = new Promotion();
		AppCSIUser.Promo.Load(Conversions.ToString(Dr["JobName"]));
		Dr.Close();
		if (AppCSIUser.Promo.Layers > 1)
		{
			lblPLCAd.Visible = true;
		}
		else
		{
			lblPLCAd.Visible = false;
		}
	}
}
