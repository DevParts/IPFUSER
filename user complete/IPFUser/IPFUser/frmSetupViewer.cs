using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IPFUser.My;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[DesignerGenerated]
public class frmSetupViewer : Form
{
	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("Button1")]
	private Button _Button1;

	[field: AccessedThroughProperty("PropertyGrid1")]
	internal virtual PropertyGrid PropertyGrid1
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

	public frmSetupViewer()
	{
		base.Load += Configuracion_Load;
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	[System.Diagnostics.DebuggerStepThrough]
	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPFUser.frmSetupViewer));
		this.PropertyGrid1 = new System.Windows.Forms.PropertyGrid();
		this.Button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		resources.ApplyResources(this.PropertyGrid1, "PropertyGrid1");
		this.PropertyGrid1.Name = "PropertyGrid1";
		resources.ApplyResources(this.Button1, "Button1");
		this.Button1.Name = "Button1";
		this.Button1.UseVisualStyleBackColor = true;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.Button1);
		base.Controls.Add(this.PropertyGrid1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frmSetupViewer";
		base.ResumeLayout(false);
	}

	private void Configuracion_Load(object sender, EventArgs e)
	{
		SetupAplicacio AppSet = new SetupAplicacio();
		PropertyGrid1.SelectedObject = AppSet;
	}

	private void Button1_Click(object sender, EventArgs e)
	{
		MySettingsProperty.Settings.Save();
		Close();
		base.Tag = "1";
	}
}
