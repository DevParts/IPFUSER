using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MacsaDevicesNet.My;
using MacsaDevicesNet.My.Resources;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

[DesignerGenerated]
public class frmAbout : Form
{
	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private IContainer components;

	[AccessedThroughProperty("LogoPictureBox")]
	private PictureBox _LogoPictureBox;

	[AccessedThroughProperty("LabelProductName")]
	private Label _LabelProductName;

	[AccessedThroughProperty("LabelVersion")]
	private Label _LabelVersion;

	[AccessedThroughProperty("LabelCopyright")]
	private Label _LabelCopyright;

	[AccessedThroughProperty("TextBoxDescription")]
	private TextBox _TextBoxDescription;

	internal virtual PictureBox LogoPictureBox
	{
		[DebuggerNonUserCode]
		get
		{
			return _LogoPictureBox;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_LogoPictureBox = value;
		}
	}

	internal virtual Label LabelProductName
	{
		[DebuggerNonUserCode]
		get
		{
			return _LabelProductName;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_LabelProductName = value;
		}
	}

	internal virtual Label LabelVersion
	{
		[DebuggerNonUserCode]
		get
		{
			return _LabelVersion;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_LabelVersion = value;
		}
	}

	internal virtual Label LabelCopyright
	{
		[DebuggerNonUserCode]
		get
		{
			return _LabelCopyright;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_LabelCopyright = value;
		}
	}

	internal virtual TextBox TextBoxDescription
	{
		[DebuggerNonUserCode]
		get
		{
			return _TextBoxDescription;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_TextBoxDescription = value;
		}
	}

	[DebuggerNonUserCode]
	public frmAbout()
	{
		base.Load += AboutBox_Load;
		__ENCAddToList(this);
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	private static void __ENCAddToList(object value)
	{
		checked
		{
			lock (__ENCList)
			{
				if (__ENCList.Count == __ENCList.Capacity)
				{
					int num = 0;
					int num2 = __ENCList.Count - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						int num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						WeakReference weakReference = __ENCList[num3];
						if (weakReference.IsAlive)
						{
							if (num3 != num)
							{
								__ENCList[num] = __ENCList[num3];
							}
							num++;
						}
						num3++;
					}
					__ENCList.RemoveRange(num, __ENCList.Count - num);
					__ENCList.Capacity = __ENCList.Count;
				}
				__ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
			}
		}
	}

	[DebuggerNonUserCode]
	protected override void Dispose(bool disposing)
	{
		try
		{
			if ((disposing && components != null) ? true : false)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacsaDevicesNet.frmAbout));
		this.LogoPictureBox = new System.Windows.Forms.PictureBox();
		this.LabelProductName = new System.Windows.Forms.Label();
		this.LabelVersion = new System.Windows.Forms.Label();
		this.LabelCopyright = new System.Windows.Forms.Label();
		this.TextBoxDescription = new System.Windows.Forms.TextBox();
		((System.ComponentModel.ISupportInitialize)this.LogoPictureBox).BeginInit();
		this.SuspendLayout();
		this.LogoPictureBox.Image = MacsaDevicesNet.My.Resources.Resources.logo_macsa_2011;
		resources.ApplyResources(this.LogoPictureBox, "LogoPictureBox");
		this.LogoPictureBox.Name = "LogoPictureBox";
		this.LogoPictureBox.TabStop = false;
		resources.ApplyResources(this.LabelProductName, "LabelProductName");
		System.Windows.Forms.Label labelProductName = this.LabelProductName;
		System.Drawing.Size maximumSize = new System.Drawing.Size(0, 17);
		labelProductName.MaximumSize = maximumSize;
		this.LabelProductName.Name = "LabelProductName";
		resources.ApplyResources(this.LabelVersion, "LabelVersion");
		System.Windows.Forms.Label labelVersion = this.LabelVersion;
		maximumSize = new System.Drawing.Size(0, 17);
		labelVersion.MaximumSize = maximumSize;
		this.LabelVersion.Name = "LabelVersion";
		resources.ApplyResources(this.LabelCopyright, "LabelCopyright");
		System.Windows.Forms.Label labelCopyright = this.LabelCopyright;
		maximumSize = new System.Drawing.Size(0, 17);
		labelCopyright.MaximumSize = maximumSize;
		this.LabelCopyright.Name = "LabelCopyright";
		resources.ApplyResources(this.TextBoxDescription, "TextBoxDescription");
		this.TextBoxDescription.Name = "TextBoxDescription";
		this.TextBoxDescription.ReadOnly = true;
		this.TextBoxDescription.TabStop = false;
		resources.ApplyResources(this, "$this");
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this.LabelProductName);
		this.Controls.Add(this.LabelVersion);
		this.Controls.Add(this.LabelCopyright);
		this.Controls.Add(this.TextBoxDescription);
		this.Controls.Add(this.LogoPictureBox);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		this.Name = "frmAbout";
		this.TopMost = true;
		((System.ComponentModel.ISupportInitialize)this.LogoPictureBox).EndInit();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void AboutBox_Load(object sender, EventArgs e)
	{
		string arg = ((Operators.CompareString(MyProject.Application.Info.Title, "", TextCompare: false) == 0) ? Path.GetFileNameWithoutExtension(MyProject.Application.Info.AssemblyName) : MyProject.Application.Info.Title);
		Text = $"About {arg}";
		LabelProductName.Text = MyProject.Application.Info.ProductName;
		LabelVersion.Text = $"Version {MyProject.Application.Info.Version.ToString()}";
		LabelCopyright.Text = MyProject.Application.Info.Copyright;
		TextBoxDescription.Text = MyProject.Application.Info.Description;
	}
}
