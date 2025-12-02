using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using MacsaDevicesNet.My;
using MacsaDevicesNet.My.Resources;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

[DesignerGenerated]
public class frmSplash : Form
{
	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private IContainer components;

	[AccessedThroughProperty("PictureBox1")]
	private PictureBox _PictureBox1;

	[AccessedThroughProperty("progressBar1")]
	private ProgressBar _progressBar1;

	[AccessedThroughProperty("lblApplicationTitle")]
	private Label _lblApplicationTitle;

	[AccessedThroughProperty("lblVersion")]
	private Label _lblVersion;

	[AccessedThroughProperty("lblCopyright")]
	private Label _lblCopyright;

	[AccessedThroughProperty("PictureBox2")]
	private PictureBox _PictureBox2;

	[AccessedThroughProperty("lblStatus")]
	private Label _lblStatus;

	[AccessedThroughProperty("lblPercent")]
	private Label _lblPercent;

	public string Version;

	protected Thread m_threadProgress;

	private object ThreadLockObject;

	private long m_lDuration;

	private int m_iTargetPerc;

	internal virtual PictureBox PictureBox1
	{
		[DebuggerNonUserCode]
		get
		{
			return _PictureBox1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_PictureBox1 = value;
		}
	}

	internal virtual ProgressBar progressBar1
	{
		[DebuggerNonUserCode]
		get
		{
			return _progressBar1;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_progressBar1 = value;
		}
	}

	internal virtual Label lblApplicationTitle
	{
		[DebuggerNonUserCode]
		get
		{
			return _lblApplicationTitle;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_lblApplicationTitle = value;
		}
	}

	internal virtual Label lblVersion
	{
		[DebuggerNonUserCode]
		get
		{
			return _lblVersion;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_lblVersion = value;
		}
	}

	internal virtual Label lblCopyright
	{
		[DebuggerNonUserCode]
		get
		{
			return _lblCopyright;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_lblCopyright = value;
		}
	}

	internal virtual PictureBox PictureBox2
	{
		[DebuggerNonUserCode]
		get
		{
			return _PictureBox2;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_PictureBox2 = value;
		}
	}

	internal virtual Label lblStatus
	{
		[DebuggerNonUserCode]
		get
		{
			return _lblStatus;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_lblStatus = value;
		}
	}

	internal virtual Label lblPercent
	{
		[DebuggerNonUserCode]
		get
		{
			return _lblPercent;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_lblPercent = value;
		}
	}

	public frmSplash()
	{
		base.Load += Form2_Load;
		__ENCAddToList(this);
		Version = "";
		m_threadProgress = new Thread(ThreadProgress);
		ThreadLockObject = RuntimeHelpers.GetObjectValue(new object());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacsaDevicesNet.frmSplash));
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.lblApplicationTitle = new System.Windows.Forms.Label();
		this.lblVersion = new System.Windows.Forms.Label();
		this.lblCopyright = new System.Windows.Forms.Label();
		this.lblStatus = new System.Windows.Forms.Label();
		this.lblPercent = new System.Windows.Forms.Label();
		this.PictureBox1 = new System.Windows.Forms.PictureBox();
		this.PictureBox2 = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox2).BeginInit();
		this.SuspendLayout();
		resources.ApplyResources(this.progressBar1, "progressBar1");
		this.progressBar1.ForeColor = System.Drawing.SystemColors.ControlDark;
		this.progressBar1.Name = "progressBar1";
		resources.ApplyResources(this.lblApplicationTitle, "lblApplicationTitle");
		this.lblApplicationTitle.BackColor = System.Drawing.Color.Transparent;
		this.lblApplicationTitle.Name = "lblApplicationTitle";
		resources.ApplyResources(this.lblVersion, "lblVersion");
		this.lblVersion.BackColor = System.Drawing.Color.Transparent;
		this.lblVersion.Name = "lblVersion";
		resources.ApplyResources(this.lblCopyright, "lblCopyright");
		this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
		this.lblCopyright.Name = "lblCopyright";
		resources.ApplyResources(this.lblStatus, "lblStatus");
		this.lblStatus.Name = "lblStatus";
		resources.ApplyResources(this.lblPercent, "lblPercent");
		this.lblPercent.BackColor = System.Drawing.Color.Transparent;
		this.lblPercent.Name = "lblPercent";
		resources.ApplyResources(this.PictureBox1, "PictureBox1");
		this.PictureBox1.Image = MacsaDevicesNet.My.Resources.Resources.logo_macsa_2011;
		this.PictureBox1.Name = "PictureBox1";
		this.PictureBox1.TabStop = false;
		resources.ApplyResources(this.PictureBox2, "PictureBox2");
		this.PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.PictureBox2.Name = "PictureBox2";
		this.PictureBox2.TabStop = false;
		resources.ApplyResources(this, "$this");
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this.lblPercent);
		this.Controls.Add(this.lblStatus);
		this.Controls.Add(this.lblApplicationTitle);
		this.Controls.Add(this.lblVersion);
		this.Controls.Add(this.lblCopyright);
		this.Controls.Add(this.progressBar1);
		this.Controls.Add(this.PictureBox1);
		this.Controls.Add(this.PictureBox2);
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		this.Name = "frmSplash";
		this.ShowInTaskbar = false;
		this.TopMost = true;
		((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox2).EndInit();
		this.ResumeLayout(false);
		this.PerformLayout();
	}

	[DllImport("uxtheme.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern int SetWindowTheme(IntPtr hWnd, [Optional][DefaultParameterValue("")][MarshalAs(UnmanagedType.AnsiBStr)] ref string pszSubAppName, [Optional][DefaultParameterValue("")][MarshalAs(UnmanagedType.AnsiBStr)] ref string pszSubIdList);

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void Form2_Load(object sender, EventArgs e)
	{
		IntPtr handle = progressBar1.Handle;
		string pszSubAppName = "";
		string pszSubIdList = "";
		SetWindowTheme(handle, ref pszSubAppName, ref pszSubIdList);
		if (Operators.CompareString(MyProject.Application.Info.Title, "", TextCompare: false) != 0)
		{
			lblApplicationTitle.Text = MyProject.Application.Info.Title;
		}
		else
		{
			lblApplicationTitle.Text = Path.GetFileNameWithoutExtension(MyProject.Application.Info.AssemblyName);
		}
		if (Operators.CompareString(Version, "", TextCompare: false) == 0)
		{
			lblVersion.Text = string.Format(lblVersion.Text, MyProject.Application.Info.Version.Major, MyProject.Application.Info.Version.Minor);
		}
		else
		{
			lblVersion.Text = "Version " + Version;
		}
		lblCopyright.Text = MyProject.Application.Info.Copyright;
	}

	public void SetBarState(int iPercentage, string sText, long lDuration)
	{
		checked
		{
			int millisecondsTimeout = (int)Math.Round((double)lDuration / (double)(iPercentage - progressBar1.Value));
			lblStatus.Text = sText;
			m_iTargetPerc = iPercentage;
			m_lDuration = lDuration;
			while (m_iTargetPerc > progressBar1.Value)
			{
				progressBar1.Value += 1;
				Application.DoEvents();
				Thread.Sleep(millisecondsTimeout);
				lblPercent.Text = Conversions.ToString(progressBar1.Value) + " %";
			}
		}
	}

	private void ThreadProgress()
	{
		checked
		{
			int millisecondsTimeout = (int)Math.Round((double)m_lDuration / (double)(m_iTargetPerc - progressBar1.Value));
			object threadLockObject = ThreadLockObject;
			ObjectFlowControl.CheckForSyncLockOnValueType(threadLockObject);
			bool lockTaken = false;
			try
			{
				Monitor.Enter(threadLockObject, ref lockTaken);
				while (m_iTargetPerc > progressBar1.Value)
				{
					progressBar1.Value += 1;
					Thread.Sleep(millisecondsTimeout);
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(threadLockObject);
				}
			}
		}
	}
}
