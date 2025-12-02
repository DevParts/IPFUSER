using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IPFUser.My.Resources;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[DesignerGenerated]
public class frmSetArtwork : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("txtArtwork")]
	private TextBox _txtArtwork;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Button1")]
	private Button _Button1;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnExit")]
	private Button _btnExit;

	private int TmpArtwork;

	[field: AccessedThroughProperty("lblIntroArt")]
	internal virtual Label lblIntroArt
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual TextBox txtArtwork
	{
		[CompilerGenerated]
		get
		{
			return _txtArtwork;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			KeyPressEventHandler value2 = txtArtwork_KeyPress;
			TextBox textBox = _txtArtwork;
			if (textBox != null)
			{
				textBox.KeyPress -= value2;
			}
			_txtArtwork = value;
			textBox = _txtArtwork;
			if (textBox != null)
			{
				textBox.KeyPress += value2;
			}
		}
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

	internal virtual Button btnExit
	{
		[CompilerGenerated]
		get
		{
			return _btnExit;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = btnExit_Click;
			Button button = _btnExit;
			if (button != null)
			{
				button.Click -= value2;
			}
			_btnExit = value;
			button = _btnExit;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	public frmSetArtwork()
	{
		base.Activated += SetArtwork_Activated;
		base.Load += SetArtwork_Load;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPFUser.frmSetArtwork));
		this.lblIntroArt = new System.Windows.Forms.Label();
		this.txtArtwork = new System.Windows.Forms.TextBox();
		this.Button1 = new System.Windows.Forms.Button();
		this.btnExit = new System.Windows.Forms.Button();
		base.SuspendLayout();
		resources.ApplyResources(this.lblIntroArt, "lblIntroArt");
		this.lblIntroArt.Name = "lblIntroArt";
		resources.ApplyResources(this.txtArtwork, "txtArtwork");
		this.txtArtwork.Name = "txtArtwork";
		resources.ApplyResources(this.Button1, "Button1");
		this.Button1.Image = IPFUser.My.Resources.Resources.Check;
		this.Button1.Name = "Button1";
		this.Button1.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.btnExit, "btnExit");
		this.btnExit.Image = IPFUser.My.Resources.Resources.door_in;
		this.btnExit.Name = "btnExit";
		this.btnExit.UseVisualStyleBackColor = true;
		base.AcceptButton = this.Button1;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.btnExit);
		base.Controls.Add(this.Button1);
		base.Controls.Add(this.txtArtwork);
		base.Controls.Add(this.lblIntroArt);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmSetArtwork";
		base.ResumeLayout(false);
		base.PerformLayout();
	}

	private void SetArtwork_Activated(object sender, EventArgs e)
	{
		txtArtwork.Focus();
	}

	private void txtArtwork_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (!Versioned.IsNumeric(e.KeyChar) & (Strings.Asc(e.KeyChar) != 8))
		{
			e.Handled = true;
		}
	}

	private void Button1_Click(object sender, EventArgs e)
	{
		if (txtArtwork.Text.Length == 0)
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String57"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			txtArtwork.Focus();
		}
		else if (Operators.ConditionalCompareObjectEqual(base.Tag, 1, TextCompare: false))
		{
			SqlDataReader Dr = AppCSIUser.Db.GetDataReader("Select * from Artworks where Artwork=" + txtArtwork.Text);
			if (Dr.HasRows)
			{
				Dr.Read();
				if (GoodPromotion(Conversions.ToInteger(Dr["IdJob"])))
				{
					Dr.Close();
					AppCSIUser.PossibleArtwork = Conversions.ToInteger(txtArtwork.Text);
					Close();
				}
				else
				{
					MessageBox.Show(AppCSIUser.Rm.GetString("String86"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					Dr.Close();
					txtArtwork.Focus();
				}
			}
			else
			{
				Dr.Close();
				MessageBox.Show(AppCSIUser.Rm.GetString("String58") + " " + txtArtwork.Text + " " + AppCSIUser.Rm.GetString("String59"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				txtArtwork.Focus();
			}
		}
		else
		{
			if (Conversions.ToDouble(txtArtwork.Text) != (double)TmpArtwork)
			{
				MessageBox.Show(AppCSIUser.Rm.GetString("String60"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				AppCSIUser.PossibleArtwork = 0;
			}
			else
			{
				AppCSIUser.PossibleArtwork = TmpArtwork;
			}
			Close();
		}
	}

	private void SetArtwork_Load(object sender, EventArgs e)
	{
		if (Operators.ConditionalCompareObjectEqual(base.Tag, 1, TextCompare: false))
		{
			lblIntroArt.Text = AppCSIUser.Rm.GetString("String61");
			return;
		}
		TmpArtwork = AppCSIUser.PossibleArtwork;
		AppCSIUser.PossibleArtwork = 0;
		lblIntroArt.Text = AppCSIUser.Rm.GetString("String62");
	}

	private void btnExit_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show(AppCSIUser.Rm.GetString("String63"), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			AppCSIUser.PossibleArtwork = -1;
			Close();
		}
	}

	public bool GoodPromotion(int IdJob)
	{
		SqlDataReader Dr = AppCSIUser.Db.GetDataReader("Select * from Jobs where JobId=" + IdJob);
		Dr.Read();
		if ((Dr["RecordLength"] == DBNull.Value) | (Dr["Split1"] == DBNull.Value) | (Dr["Split2"] == DBNull.Value) | (Dr["Split3"] == DBNull.Value) | (Dr["Split4"] == DBNull.Value))
		{
			Dr.Close();
			return false;
		}
		if (Conversions.ToBoolean(Operators.OrObject(Operators.CompareObjectEqual(Dr["RecordLength"], 0, TextCompare: false), Operators.CompareObjectNotEqual(Operators.AddObject(Operators.AddObject(Operators.AddObject(Dr["Split1"], Dr["Split2"]), Dr["Split3"]), Dr["Split4"]), Dr["RecordLength"], TextCompare: false))))
		{
			Dr.Close();
			return false;
		}
		if (Dr["LaserFile"] == DBNull.Value)
		{
			Dr.Close();
			return false;
		}
		Dr.Close();
		return true;
	}
}
