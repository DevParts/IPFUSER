using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IPFUser.DataMatrix;
using IPFUser.My;
using IPFUser.My.Resources;
using MacsaDevicesNet;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[DesignerGenerated]
public class frmMain : Form
{
	public struct CodeMatrix
	{
		public string Code;

		public byte[] Data;
	}

	/// <summary>
	/// Cambios de estado del PLC
	/// </summary>
	/// <param name="Sender"></param>
	/// <param name="State"></param>
	/// <param name="IsError"></param>
	/// <remarks></remarks>
	public delegate void dlgPLCOnStateChanged(object Sender, string State, bool IsError);

	/// <summary>
	/// Recepcion del estado del PLC
	/// </summary>
	/// <param name="Sender"></param>
	/// <remarks></remarks>
	public delegate void dlgPLCOnDataReceived(object Sender);

	/// <summary>
	/// Transmisión de parámetros correcta
	/// </summary>
	/// <param name="Sender"></param>
	/// <remarks></remarks>
	public delegate void dlgPLCOnDataSent(object Sender);

	public delegate void dlgPLCOnModbusException(object Sender, byte IdFunction, byte Exception);

	private CLaser Laser;

	private Queue<string> Queue1;

	private Queue<string> Queue2;

	private Queue<string> ActiveQueue;

	private Queue<CodeMatrix> QueueDataString1;

	private Queue<CodeMatrix> QueueDataString2;

	private Queue<CodeMatrix> ActiveQueueDataString;

	private bool LaserQueueError;

	private int QueueLaserItems;

	private bool MustResetQueues;

	private string MessagePath;

	private string LastSentCode;

	private string LastSentCodePosition;

	private bool FirstTime;

	private int PercentWarning;

	private int InitialRecord;

	private int FinalRecord;

	private bool UpdateInitialRecord;

	private bool DataLoaded;

	private HardwareLicense HardwareLicense;

	private EZCode oEZCoder;

	private IContainer components;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("ConfiguraciónToolStripMenuItem")]
	private ToolStripMenuItem _ConfiguraciónToolStripMenuItem;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("txtProducir")]
	private TextBox _txtProducir;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnStop")]
	private Button _btnStop;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnStart")]
	private Button _btnStart;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnExit")]
	private Button _btnExit;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("SincroMarkState")]
	private System.Windows.Forms.Timer _SincroMarkState;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnPercentWarning")]
	private Button _btnPercentWarning;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("TimerBlinkWarning")]
	private System.Windows.Forms.Timer _TimerBlinkWarning;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("btnFinOk")]
	private Button _btnFinOk;

	[SpecialName]
	private int _0024STATIC_0024SincroMarkState_Tick_002420211C1280A1_0024InputsHasp;

	[field: AccessedThroughProperty("StatusStrip1")]
	internal virtual StatusStrip StatusStrip1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("MenuStrip1")]
	internal virtual MenuStrip MenuStrip1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual ToolStripMenuItem ConfiguraciónToolStripMenuItem
	{
		[CompilerGenerated]
		get
		{
			return _ConfiguraciónToolStripMenuItem;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = ConfiguraciónToolStripMenuItem_Click;
			ToolStripMenuItem toolStripMenuItem = _ConfiguraciónToolStripMenuItem;
			if (toolStripMenuItem != null)
			{
				toolStripMenuItem.Click -= value2;
			}
			_ConfiguraciónToolStripMenuItem = value;
			toolStripMenuItem = _ConfiguraciónToolStripMenuItem;
			if (toolStripMenuItem != null)
			{
				toolStripMenuItem.Click += value2;
			}
		}
	}

	[field: AccessedThroughProperty("SplitContainer1")]
	internal virtual SplitContainer SplitContainer1
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

	[field: AccessedThroughProperty("PgbTotal")]
	internal virtual ProgressBar PgbTotal
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblAvailableCodes")]
	internal virtual Label lblAvailableCodes
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label9")]
	internal virtual Label Label9
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblPromotion")]
	internal virtual Label lblPromotion
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label2")]
	internal virtual Label Label2
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblArtwork")]
	internal virtual Label lblArtwork
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label3")]
	internal virtual Label Label3
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label10")]
	internal virtual Label Label10
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PgbLote")]
	internal virtual ProgressBar PgbLote
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label4")]
	internal virtual Label Label4
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("txtPedido")]
	internal virtual TextBox txtPedido
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label5")]
	internal virtual Label Label5
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual TextBox txtProducir
	{
		[CompilerGenerated]
		get
		{
			return _txtProducir;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			KeyPressEventHandler value2 = txtProducir_KeyPress;
			EventHandler value3 = txtProducir_TextChanged;
			TextBox textBox = _txtProducir;
			if (textBox != null)
			{
				textBox.KeyPress -= value2;
				textBox.TextChanged -= value3;
			}
			_txtProducir = value;
			textBox = _txtProducir;
			if (textBox != null)
			{
				textBox.KeyPress += value2;
				textBox.TextChanged += value3;
			}
		}
	}

	internal virtual Button btnStop
	{
		[CompilerGenerated]
		get
		{
			return _btnStop;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = btnStop_Click;
			Button button = _btnStop;
			if (button != null)
			{
				button.Click -= value2;
			}
			_btnStop = value;
			button = _btnStop;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	internal virtual Button btnStart
	{
		[CompilerGenerated]
		get
		{
			return _btnStart;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = btnStart_Click;
			Button button = _btnStart;
			if (button != null)
			{
				button.Click -= value2;
			}
			_btnStart = value;
			button = _btnStart;
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

	[field: AccessedThroughProperty("lblProducidos")]
	internal virtual Label lblProducidos
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label11")]
	internal virtual Label Label11
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblState")]
	internal virtual ToolStripStatusLabel lblState
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual System.Windows.Forms.Timer SincroMarkState
	{
		[CompilerGenerated]
		get
		{
			return _SincroMarkState;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = SincroMarkState_Tick;
			System.Windows.Forms.Timer timer = _SincroMarkState;
			if (timer != null)
			{
				timer.Tick -= value2;
			}
			_SincroMarkState = value;
			timer = _SincroMarkState;
			if (timer != null)
			{
				timer.Tick += value2;
			}
		}
	}

	[field: AccessedThroughProperty("pnlInfo")]
	internal virtual Panel pnlInfo
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblInfo")]
	internal virtual Label lblInfo
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("ImgInfo")]
	internal virtual PictureBox ImgInfo
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblLastCode")]
	internal virtual Label lblLastCode
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label12")]
	internal virtual Label Label12
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblLaserFile")]
	internal virtual Label lblLaserFile
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pnlPercentWarning")]
	internal virtual Panel pnlPercentWarning
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pbPercentWarning")]
	internal virtual PictureBox pbPercentWarning
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button btnPercentWarning
	{
		[CompilerGenerated]
		get
		{
			return _btnPercentWarning;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = btnPercentWarning_Click;
			Button button = _btnPercentWarning;
			if (button != null)
			{
				button.Click -= value2;
			}
			_btnPercentWarning = value;
			button = _btnPercentWarning;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	[field: AccessedThroughProperty("lblPercentWarning")]
	internal virtual Label lblPercentWarning
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblTotalPecent")]
	internal virtual Label lblTotalPecent
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label13")]
	internal virtual Label Label13
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblLotePercent")]
	internal virtual Label lblLotePercent
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual System.Windows.Forms.Timer TimerBlinkWarning
	{
		[CompilerGenerated]
		get
		{
			return _TimerBlinkWarning;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = TimerBlinkWarning_Tick;
			System.Windows.Forms.Timer timer = _TimerBlinkWarning;
			if (timer != null)
			{
				timer.Tick -= value2;
			}
			_TimerBlinkWarning = value;
			timer = _TimerBlinkWarning;
			if (timer != null)
			{
				timer.Tick += value2;
			}
		}
	}

	[field: AccessedThroughProperty("cmbPromoFiles")]
	internal virtual ComboBox cmbPromoFiles
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblPendientes")]
	internal virtual Label lblPendientes
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label15")]
	internal virtual Label Label15
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PictureBox1")]
	internal virtual PictureBox PictureBox1
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	internal virtual Button btnFinOk
	{
		[CompilerGenerated]
		get
		{
			return _btnFinOk;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			EventHandler value2 = btnFinOk_Click;
			Button button = _btnFinOk;
			if (button != null)
			{
				button.Click -= value2;
			}
			_btnFinOk = value;
			button = _btnFinOk;
			if (button != null)
			{
				button.Click += value2;
			}
		}
	}

	[field: AccessedThroughProperty("Label14")]
	internal virtual Label Label14
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblLastCodePosition")]
	internal virtual Label lblLastCodePosition
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pnlPLC")]
	internal virtual Panel pnlPLC
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("lblStatePLC")]
	internal virtual Label lblStatePLC
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Label16")]
	internal virtual Label Label16
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("Panel9")]
	internal virtual Panel Panel9
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pbLedLife")]
	internal virtual PictureBox pbLedLife
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PictureBox4")]
	internal virtual PictureBox PictureBox4
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("pbLedPLC")]
	internal virtual PictureBox pbLedPLC
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PictureBox3")]
	internal virtual PictureBox PictureBox3
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PictureBox2")]
	internal virtual PictureBox PictureBox2
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	[field: AccessedThroughProperty("PictureBox5")]
	internal virtual PictureBox PictureBox5
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	}

	public frmMain()
	{
		base.FormClosing += main_FormClosing;
		base.Load += main_Load;
		Laser = new CLaser();
		Queue1 = new Queue<string>();
		Queue2 = new Queue<string>();
		QueueDataString1 = new Queue<CodeMatrix>();
		QueueDataString2 = new Queue<CodeMatrix>();
		LaserQueueError = false;
		MustResetQueues = true;
		FirstTime = true;
		PercentWarning = 100;
		oEZCoder = new EZCode();
		AppCSIUser.InitCulture();
		InitializeComponent();
	}

	private void ConfiguraciónToolStripMenuItem_Click(object sender, EventArgs e)
	{
		frmPassword Pwd = new frmPassword("mlaser", this);
		Pwd.ShowDialog();
		Pwd.Dispose();
		if (Operators.ConditionalCompareObjectEqual(base.Tag, "1", TextCompare: false))
		{
			MyProject.Forms.frmSetupViewer.ShowDialog();
			MyProject.Forms.frmSetupViewer.Dispose();
			MessageBox.Show(AppCSIUser.Rm.GetString("String85"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public bool PrepareDataBase()
	{
		if (Operators.CompareString(MySettingsProperty.Settings.Catalog, "", TextCompare: false) == 0)
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String34"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			MyProject.Forms.frmSetupViewer.ShowDialog();
			MessageBox.Show(AppCSIUser.Rm.GetString("String35"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			ProjectData.EndApp();
		}
		if (Operators.CompareString(MySettingsProperty.Settings.DataServer, "", TextCompare: false) == 0)
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String36"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			MyProject.Forms.frmSetupViewer.ShowDialog();
			MessageBox.Show(AppCSIUser.Rm.GetString("String35"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			ProjectData.EndApp();
		}
		AppCSIUser.Db.PopupErrors = true;
		AppCSIUser.Db.DbName = MySettingsProperty.Settings.Catalog;
		AppCSIUser.Db.DataSource = MySettingsProperty.Settings.DataServer;
		AppCSIUser.Db.UseWindowsAuthentication = MySettingsProperty.Settings.UseWindowsAuthentication;
		AppCSIUser.Db.User = MySettingsProperty.Settings.SqlUser;
		AppCSIUser.Db.Password = MySettingsProperty.Settings.SqlPassword;
		bool PrepareDataBase = default(bool);
		return PrepareDataBase;
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void main_FormClosing(object sender, FormClosingEventArgs e)
	{
		DisconnectDB();
		AppCSIUser.oPLC.CloseConnection();
		Common.MACSALog("Final de la Aplicación", TraceEventType.Information);
		ProjectData.EndApp();
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void main_Load(object sender, EventArgs e)
	{
		if (Common.PrevInstance(MyProject.Application.Info.ProductName))
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String37"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			ProjectData.EndApp();
		}
		Common.MACSALog("Inicio de la Aplicación", TraceEventType.Information);
		string licPath = Path.Combine(Application.StartupPath, "license.lic");
		if (!LicenseManager.IsLicenseValid(licPath))
		{
			ProjectData.EndApp();
		}
		try
		{
			Directory.CreateDirectory(Application.StartupPath + "\\Tmp");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
		Text = "Macsa Integra - " + Application.ProductName + " 3.0.0.2";
		frmSplash frmSplash = new frmSplash();
		frmSplash.StartPosition = FormStartPosition.CenterScreen;
		frmSplash.Show();
		frmSplash.SetBarState(30, AppCSIUser.Rm.GetString("String39"), 1000L);
		frmSplash.SetBarState(60, AppCSIUser.Rm.GetString("String40"), 1000L);
		frmSplash.SetBarState(100, AppCSIUser.Rm.GetString("String41"), 1000L);
		frmSplash.Dispose();
		SplitContainer1.Enabled = false;
		PrepareDataBase();
		lblState.Text = "Buscando Datos...";
		Cursor.Current = Cursors.WaitCursor;
		if (SearchDb())
		{
			Cursor.Current = Cursors.Default;
			lblState.Text = AppCSIUser.Rm.GetString("String42");
			SplitContainer1.Enabled = true;
			base.Visible = false;
			GetArtwork();
			base.Visible = true;
		}
		else
		{
			ProjectData.EndApp();
		}
	}

	public bool SearchDb()
	{
		string[] Dbs = new string[1] { "" };
		string[] Drives = Directory.GetLogicalDrives();
		checked
		{
			int num = Drives.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				try
				{
					if (Directory.GetFiles(Drives[i] + "IPFEu").Length > 0)
					{
						Dbs[0] = Drives[i] + "IPFEu\\IPF.mdf";
						AppCSIUser.Drive = Drives[i];
						break;
					}
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					ProjectData.ClearProjectError();
				}
			}
			if (Operators.CompareString(Dbs[0], "", TextCompare: false) == 0)
			{
				MessageBox.Show(AppCSIUser.Rm.GetString("String43") + "\r\n" + AppCSIUser.Rm.GetString("String44"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Common.MACSALog("Base de Datos encontrada en " + Dbs[0], TraceEventType.Information);
			AppCSIUser.DbPath = Path.GetDirectoryName(Dbs[0]);
			AppCSIUser.Db.PopupErrors = false;
			Smo.DetachDb("IPFEu");
			try
			{
				Common.MACSALog("Attach Db CSIEu from " + Dbs[0], TraceEventType.Information);
				Smo.AttachDb("IPFEu", Dbs[0], Path.GetDirectoryName(Dbs[0]) + "\\IPF_log.ldf");
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				Common.MACSALog("ERROR Attach Db CSIEu from " + Dbs[0] + " " + ex4.Message, TraceEventType.Error);
				ProjectData.ClearProjectError();
			}
			AppCSIUser.Db.PopupErrors = true;
			AppCSIUser.Db.CreateAdapter("Historico");
			AppCSIUser.Db.CreateAdapter("CodesIndex");
			MessagePath = Path.GetDirectoryName(Dbs[0]) + "\\";
			AppCSIUser.DbConnected = true;
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public void GetArtwork()
	{
		while (true)
		{
			MyProject.Forms.frmSetArtwork.Tag = 1;
			MyProject.Forms.frmSetArtwork.ShowDialog();
			MyProject.Forms.frmSetArtwork.Dispose();
			if (AppCSIUser.PossibleArtwork > 0)
			{
				MyProject.Forms.frmSetArtwork.Dispose();
				MyProject.Forms.frmSetArtwork.Tag = 2;
				MyProject.Forms.frmSetArtwork.ShowDialog();
				MyProject.Forms.frmSetArtwork.Dispose();
				if (AppCSIUser.PossibleArtwork > 0)
				{
					MyProject.Forms.frmConfirmPromotion.ShowDialog();
					if (Operators.ConditionalCompareObjectEqual(MyProject.Forms.frmConfirmPromotion.Tag, true, TextCompare: false))
					{
						break;
					}
					MyProject.Forms.frmConfirmPromotion.Dispose();
				}
				else if (AppCSIUser.PossibleArtwork == -1)
				{
					Close();
				}
			}
			else if (AppCSIUser.PossibleArtwork == -1)
			{
				Close();
			}
		}
		MustResetQueues = true;
		QueueLaserItems = 0;
		Queue1.Clear();
		Queue2.Clear();
		ActiveQueue = null;
		ActiveQueueDataString = null;
		MyProject.Forms.frmConfirmPromotion.Dispose();
		txtProducir.Enabled = true;
		if (AppCSIUser.Promo.Layers > 1)
		{
			pnlPLC.Visible = true;
			pnlPercentWarning.Dock = DockStyle.None;
			AppCSIUser.oPLC.OnStateChanged += PLCStateChanged;
			AppCSIUser.oPLC.OnNewDataReceived += PLCDataReceived;
			AppCSIUser.oPLC.OnNewDataSent += PLCDataSent;
			AppCSIUser.oPLC.OnModbusException += PLCModbusException;
		}
		else
		{
			pnlPLC.Visible = false;
			pnlPercentWarning.Dock = DockStyle.Bottom;
			AppCSIUser.oPLC.OnStateChanged -= PLCStateChanged;
			AppCSIUser.oPLC.OnNewDataReceived -= PLCDataReceived;
			AppCSIUser.oPLC.OnNewDataSent -= PLCDataSent;
			AppCSIUser.oPLC.OnModbusException -= PLCModbusException;
		}
		if (AppCSIUser.Promo.Layers > 1)
		{
			if (!AppCSIUser.oPLC.Init())
			{
				DisconnectDB();
				MessageBox.Show(AppCSIUser.Rm.GetString("String95"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				MyProject.Forms.frmSetupViewer.ShowDialog();
				MyProject.Forms.frmSetupViewer.Dispose();
				ProjectData.EndApp();
			}
			else
			{
				lblStatePLC.Text = AppCSIUser.Rm.GetString("String100");
				AppCSIUser.oPLC.Rearm();
			}
		}
		else
		{
			AppCSIUser.oPLC.CloseConnection();
		}
	}

	public void PreparePromotion()
	{
		txtProducir.Text = "";
		txtPedido.Text = "";
		lblLastCode.Text = "";
		lblLastCodePosition.Text = "";
		lblProducidos.Text = Conversions.ToString(0);
		lblPendientes.Text = Conversions.ToString(0);
		PgbLote.Value = 0;
		PercentWarning = 100;
		pnlPercentWarning.Visible = false;
		pnlInfo.Visible = false;
		lblArtwork.Text = AppCSIUser.PossibleArtwork.ToString();
		lblPromotion.Text = AppCSIUser.Promo.Name;
		lblAvailableCodes.Text = Conversions.ToString(Operators.SubtractObject(AppCSIUser.Promo.TotalCodes, AppCSIUser.Promo.ConsumedCodes));
		lblLaserFile.Text = AppCSIUser.Promo.LaserFile;
		checked
		{
			PgbTotal.Value = Conversions.ToInteger(((int)Math.Round((double)Conversions.ToLong(lblAvailableCodes.Text) / (double)Conversions.ToLong(AppCSIUser.Promo.TotalCodes) * 100.0)).ToString());
			lblTotalPecent.Text = PgbTotal.Value + " %";
			WarningLowLevelCodes();
			cmbPromoFiles.Items.Clear();
			int num = AppCSIUser.Promo.FilesImported.Rows.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				cmbPromoFiles.Items.Add(RuntimeHelpers.GetObjectValue(AppCSIUser.Promo.FilesImported.Rows[i]["FileName"]));
			}
			cmbPromoFiles.SelectedIndex = 0;
			AppCSIUser.LoadCodes();
			txtPedido.Enabled = true;
		}
	}

	private void btnStart_Click(object sender, EventArgs e)
	{
		MessageBox.Show(AppCSIUser.Rm.GetString("String38"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		if (Operators.CompareString(txtProducir.Text, "", TextCompare: false) == 0 || Conversions.ToInteger(txtProducir.Text) == 0)
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String45"));
			return;
		}
		if (Operators.CompareString(btnStart.Text, "Continue", TextCompare: false) != 0 && Conversions.ToInteger(txtProducir.Text) > Conversions.ToInteger(lblAvailableCodes.Text))
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String46") + " " + lblAvailableCodes.Text);
			return;
		}
		if (txtPedido.Text.Length != 11)
		{
			MessageBox.Show(AppCSIUser.Rm.GetString("String47"));
			return;
		}
		checked
		{
			if (AppCSIUser.Promo.Layers > 1)
			{
				if (Conversions.ToBoolean(Operators.NotObject(AppCSIUser.oPLC.IsConnected)) && !AppCSIUser.oPLC.Init())
				{
					pbLedPLC.Image = Resources.red;
					lblStatePLC.Text = AppCSIUser.Rm.GetString("String99");
					return;
				}
				AppCSIUser.oPLC.WData.GoRearm = true;
				Common.Wait(2000L);
				if (!AppCSIUser.oPLC.RData.Rearmed)
				{
					MessageBox.Show(AppCSIUser.Rm.GetString("String101"));
					return;
				}
				int QueueElements = ((AppCSIUser.Promo.DatamatrixType >= 0) ? (QueueElements + (QueueDataString1.Count + QueueDataString2.Count)) : (QueueElements + (Queue1.Count + Queue2.Count)));
				AppCSIUser.oPLC.FillCycle(QueueElements + QueueLaserItems);
				AppCSIUser.oPLC.Start();
			}
			txtPedido.Enabled = false;
			txtProducir.Enabled = false;
			pnlInfo.Visible = false;
			if (Operators.CompareString(btnStart.Text, AppCSIUser.Rm.GetString("String48"), TextCompare: false) != 0)
			{
				Laser.TotalMark = Conversions.ToInteger(txtProducir.Text);
				Laser.TotalMarked = 0;
				lblPendientes.Text = Conversions.ToString(Laser.TotalMark);
				lblProducidos.Text = Conversions.ToString(0);
				PgbLote.Value = 0;
				lblLotePercent.Text = "00%";
			}
			Common.MACSALog("Inicia producción para el pedido: " + txtPedido.Text + " y Artwork: " + lblArtwork.Text, TraceEventType.Information);
			AppCSIUser.Pedido = txtPedido.Text;
			AppCSIUser.Artwork = lblArtwork.Text;
			if (Laser.Run(AppCSIUser.Promo.LaserFile, MessagePath, Conversions.ToInteger(AppCSIUser.Promo.UserFields), Conversions.ToBoolean(Interaction.IIf(AppCSIUser.Promo.DatamatrixType < 0, false, true)), MustResetQueues))
			{
				if (Laser.GetState() | (Laser.ErrorType == CLaser.ERROR_TYPES.LASER_WARNING))
				{
					MustResetQueues = false;
					QueueLaserItems = Laser.InBufferCount(Conversions.ToBoolean(Interaction.IIf(AppCSIUser.Promo.DatamatrixType < 0, false, true)));
					btnStop.Enabled = true;
					btnExit.Enabled = false;
					btnStart.Enabled = false;
					InitialRecord = FinalRecord;
					UpdateInitialRecord = true;
					if (AppCSIUser.Promo.DatamatrixType < 0)
					{
						if (ActiveQueue == null)
						{
							ActiveQueue = Queue1;
						}
					}
					else if (ActiveQueueDataString == null)
					{
						ActiveQueueDataString = QueueDataString1;
					}
					if (Laser.RunThread())
					{
						DataLoaded = false;
						MySettingsProperty.Settings.Session++;
						MySettingsProperty.Settings.Save();
						if (AppCSIUser.Promo.DatamatrixType < 0)
						{
							ThreadPool.QueueUserWorkItem(QueueFiller);
							ThreadPool.QueueUserWorkItem(QueueConsumer);
						}
						else
						{
							ThreadPool.QueueUserWorkItem(QueueFillerDataString);
							ThreadPool.QueueUserWorkItem(QueueConsumerDataString);
						}
						int sock = Laser.Sock;
						Promotion promo;
						string filename = (promo = AppCSIUser.Promo).LaserFile;
						int num = LaserDLL.MLaser_Start(sock, ref filename, 0);
						promo.LaserFile = filename;
						int i = num;
						SincroMarkState.Start();
					}
					else
					{
						AppCSIUser.oPLC.Stop();
						LaserDLL.SimuString Cadena = default(LaserDLL.SimuString);
						LaserDLL.MGetLastError(Laser.Sock2, ref Cadena);
						MessageBox.Show(AppCSIUser.Rm.GetString("String49") + " " + LaserDLL.SSView(Cadena.str_Renamed), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
				}
				else
				{
					AppCSIUser.oPLC.Stop();
					Common.MACSALog("Error en el Laser: " + Laser.ErrorDesc, TraceEventType.Information);
					InfoPanel(Enabled: true, Laser.ErrorDesc, IsWarning: true);
					MessageBox.Show(AppCSIUser.Rm.GetString("String49"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			else
			{
				AppCSIUser.oPLC.Stop();
				InfoPanel(Enabled: true, AppCSIUser.Rm.GetString("String50"), IsWarning: true);
			}
		}
	}

	private void btnStop_Click(object sender, EventArgs e)
	{
		Common.MACSALog("Detiene proceso por STOP", TraceEventType.Information);
		btnStart.Text = AppCSIUser.Rm.GetString("String48");
		btnStart.Enabled = true;
		btnStop.Enabled = false;
		btnExit.Enabled = true;
		SincroMarkState.Stop();
		QueueLaserItems = Laser.InBufferCount(Conversions.ToBoolean(Interaction.IIf(AppCSIUser.Promo.DatamatrixType < 0, false, true)));
		Laser.Stop();
		if (AppCSIUser.Promo.Layers > 1)
		{
			AppCSIUser.oPLC.Stop();
		}
		lblProducidos.Text = Laser.TotalMarked.ToString();
		checked
		{
			lblPendientes.Text = (Laser.TotalMark - Laser.TotalMarked).ToString();
			if (Laser.TotalMark > 0)
			{
				PgbLote.Value = (int)Math.Round((float)Laser.TotalMarked / (float)Laser.TotalMark * 100f);
				lblLotePercent.Text = PgbLote.Value + "%";
			}
			Common.Wait(2000L);
			AppCSIUser.GenerateHistoric(InitialRecord, FinalRecord);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void btnExit_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show(AppCSIUser.Rm.GetString("String51"), Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Common.MACSALog("Cierra la promoción en curso", TraceEventType.Information);
			base.Visible = false;
			GetArtwork();
			base.Visible = true;
		}
		else
		{
			Common.MACSALog("Cierra la Aplicación", TraceEventType.Information);
			DisconnectDB();
			Common.MACSALog("Final de la Aplicación", TraceEventType.Information);
			ProjectData.EndApp();
		}
	}

	private void SincroMarkState_Tick(object sender, EventArgs e)
	{
		checked
		{
			_0024STATIC_0024SincroMarkState_Tick_002420211C1280A1_0024InputsHasp++;
			if (_0024STATIC_0024SincroMarkState_Tick_002420211C1280A1_0024InputsHasp == 10)
			{
				_0024STATIC_0024SincroMarkState_Tick_002420211C1280A1_0024InputsHasp = 0;
				btnStop_Click(btnStop, new EventArgs());
				MessageBox.Show(AppCSIUser.Rm.GetString("String38"), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			lblAvailableCodes.Text = Conversions.ToString(Operators.SubtractObject(AppCSIUser.Promo.TotalCodes, AppCSIUser.Promo.ConsumedCodes));
			PgbTotal.Value = Conversions.ToInteger(((int)Math.Round((double)Conversions.ToLong(lblAvailableCodes.Text) / (double)Conversions.ToLong(AppCSIUser.Promo.TotalCodes) * 100.0)).ToString());
			lblTotalPecent.Text = PgbTotal.Value + " %";
			WarningLowLevelCodes();
			lblProducidos.Text = Laser.TotalMarked.ToString();
			lblPendientes.Text = (Laser.TotalMark - Laser.TotalMarked).ToString();
			if (Laser.TotalMark > 0)
			{
				PgbLote.Value = (int)Math.Round((float)Laser.TotalMarked / (float)Laser.TotalMark * 100f);
				lblLotePercent.Text = PgbLote.Value + "%";
			}
			if (Operators.CompareString(LastSentCode, "", TextCompare: false) != 0)
			{
				if (AppCSIUser.Promo.Split1 > 0)
				{
					lblLastCode.Text = LastSentCode.Substring(0, AppCSIUser.Promo.Split1);
				}
				if (AppCSIUser.Promo.Split2 > 0)
				{
					Label label;
					(label = lblLastCode).Text = label.Text + "-" + LastSentCode.Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2);
				}
				if (AppCSIUser.Promo.Split3 > 0)
				{
					Label label;
					(label = lblLastCode).Text = label.Text + "-" + LastSentCode.Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2, AppCSIUser.Promo.Split3);
				}
				if (AppCSIUser.Promo.Split4 > 0)
				{
					Label label;
					(label = lblLastCode).Text = label.Text + "-" + LastSentCode.Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2 + AppCSIUser.Promo.Split3, AppCSIUser.Promo.Split4);
				}
			}
			lblLastCodePosition.Text = LastSentCodePosition;
			if (LaserQueueError)
			{
				LaserQueueError = false;
				LaserDLL.SimuString Cadena = default(LaserDLL.SimuString);
				if (Operators.CompareString(Laser.ErrorDesc, "", TextCompare: false) == 0)
				{
					LaserDLL.MGetLastError(Laser.Sock, ref Cadena);
					InfoPanel(Enabled: true, "Error: " + LaserDLL.SSView(Cadena.str_Renamed), IsWarning: true);
					Common.MACSALog("Error en el Laser: " + LaserDLL.SSView(Cadena.str_Renamed), TraceEventType.Information);
				}
				else
				{
					InfoPanel(Enabled: true, Laser.ErrorDesc, IsWarning: true);
					Common.MACSALog("Error en el Laser: " + Laser.ErrorDesc, TraceEventType.Information);
				}
				Common.MACSALog("Fuerza STOP por error de laser", TraceEventType.Information);
				Laser.ResetBuffer(Conversions.ToInteger(AppCSIUser.Promo.UserFields), Conversions.ToBoolean(Interaction.IIf(AppCSIUser.Promo.DatamatrixType < 0, false, true)));
				btnStop_Click(null, null);
				Common.MACSALog("Reset de las colas de FastUserField", TraceEventType.Information);
			}
			if (AppCSIUser.Promo.Layers > 1 && AppCSIUser.oPLC.RData.GeneralError)
			{
				Common.MACSALog("Error General del PLC. Se detiene producción", TraceEventType.Information);
				InfoPanel(Enabled: true, AppCSIUser.Rm.GetString("String102"), IsWarning: true);
				btnStop_Click(null, null);
			}
			if (Laser.TotalMark == Laser.TotalMarked && Laser.InBufferCount(Conversions.ToBoolean(Interaction.IIf(AppCSIUser.Promo.DatamatrixType < 0, false, true))) == 0)
			{
				InfoPanel(Enabled: true, AppCSIUser.Rm.GetString("String52"), IsWarning: false);
				btnStop_Click(null, null);
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="Enabled">Si es visible</param>
	/// <param name="Text"></param>
	/// <param name="IsWarning">Si es Warning o en Error</param>
	/// <remarks></remarks>
	public void InfoPanel(bool Enabled, string Text, bool IsWarning)
	{
		pnlInfo.Visible = Enabled;
		lblInfo.Text = Text;
		if (IsWarning)
		{
			ImgInfo.Image = Resources.Warning;
			pnlInfo.BackColor = Color.Red;
			btnStart.Text = AppCSIUser.Rm.GetString("String48");
		}
		else
		{
			pnlInfo.BackColor = Color.Green;
			ImgInfo.Image = Resources.Check;
			btnStart.Text = AppCSIUser.Rm.GetString("String53");
			btnFinOk.Visible = true;
		}
	}

	/// <summary>
	/// Borra todas las colas de datos
	/// </summary>
	/// <remarks></remarks>
	public void ClearQueues()
	{
		Queue1.Clear();
		Queue2.Clear();
		QueueDataString1.Clear();
		QueueDataString2.Clear();
	}

	public void FillQueue(Queue<string> Cola)
	{
		checked
		{
			int Len = ((Laser.TotalMark - Laser.TotalMarked <= 50) ? (Laser.TotalMark - Laser.TotalMarked) : 50);
			DataTable Tb = AppCSIUser.DbCodes.GetSqlTable(AppCSIUser.Promo.get_GetSqlCodes(Len), "Codes");
			int IdConsumedInitial = 0;
			if (Tb.Rows.Count > 0)
			{
				int IdConsumedFinal = default(int);
				foreach (DataRow Row in Tb.Rows)
				{
					if (UpdateInitialRecord)
					{
						InitialRecord = Conversions.ToInteger(Row["Id"]);
						UpdateInitialRecord = false;
					}
					if (IdConsumedInitial == 0)
					{
						IdConsumedInitial = Conversions.ToInteger(Row["Id"]);
					}
					Cola.Enqueue(Conversions.ToString(Row["Code"]));
					Row["Sent"] = 1;
					Row["TimeStamp"] = DateTime.Now;
					IdConsumedFinal = Conversions.ToInteger(Row["Id"]);
					FinalRecord = Conversions.ToInteger(Row["Id"]);
				}
				LastSentCodePosition = IdConsumedFinal.ToString();
				AppCSIUser.DbCodes.UpdateTable(Tb);
				Laser.TotalMarked += Tb.Rows.Count;
				UpdateConsumos(IdConsumedInitial, IdConsumedFinal);
			}
			DataLoaded = true;
		}
	}

	public void FillQueueDataString(Queue<CodeMatrix> Cola)
	{
		checked
		{
			int Len = ((Laser.TotalMark - Laser.TotalMarked <= 50) ? (Laser.TotalMark - Laser.TotalMarked) : 50);
			DataTable Tb = AppCSIUser.DbCodes.GetSqlTable(AppCSIUser.Promo.get_GetSqlCodes(Len), "Codes");
			int IdConsumedInitial = 0;
			if (Tb.Rows.Count > 0)
			{
				int IdConsumedFinal = default(int);
				foreach (DataRow Row in Tb.Rows)
				{
					if (UpdateInitialRecord)
					{
						InitialRecord = Conversions.ToInteger(Row["Id"]);
						UpdateInitialRecord = false;
					}
					if (IdConsumedInitial == 0)
					{
						IdConsumedInitial = Conversions.ToInteger(Row["Id"]);
					}
					oEZCoder.CalculateCode(Encoding.Default.GetBytes(Row["Code"].ToString()), Row["Code"].ToString().Length);
					Cola.Enqueue(new CodeMatrix
					{
						Code = Row["Code"].ToString(),
						Data = oEZCoder.Code
					});
					Row["Sent"] = 1;
					Row["TimeStamp"] = DateTime.Now;
					IdConsumedFinal = Conversions.ToInteger(Row["Id"]);
					FinalRecord = Conversions.ToInteger(Row["Id"]);
				}
				LastSentCodePosition = IdConsumedFinal.ToString();
				AppCSIUser.DbCodes.UpdateTable(Tb);
				Laser.TotalMarked += Tb.Rows.Count;
				UpdateConsumos(IdConsumedInitial, IdConsumedFinal);
			}
			DataLoaded = true;
		}
	}

	public void QueueFiller(object StateInfo)
	{
		while (Laser.State == CLaser.LASER_STATES.MARKING)
		{
			lock (Queue1)
			{
				if (Queue1.Count == 0)
				{
					FillQueue(Queue1);
				}
			}
			lock (Queue2)
			{
				if (Queue2.Count == 0)
				{
					FillQueue(Queue2);
				}
			}
			Thread.Sleep(100);
		}
	}

	public void QueueFillerDataString(object StateInfo)
	{
		while (Laser.State == CLaser.LASER_STATES.MARKING)
		{
			lock (QueueDataString1)
			{
				if (QueueDataString1.Count == 0)
				{
					FillQueueDataString(QueueDataString1);
				}
			}
			lock (QueueDataString2)
			{
				if (QueueDataString2.Count == 0)
				{
					FillQueueDataString(QueueDataString2);
				}
			}
			Thread.Sleep(100);
		}
	}

	public void QueueConsumer(object StateInfo)
	{
		checked
		{
			int i = default(int);
			int ErrorCnt = default(int);
			while (Laser.State == CLaser.LASER_STATES.MARKING)
			{
				if (ActiveQueue.Equals(Queue1))
				{
					lock (Queue1)
					{
						if (Queue1.Count > 0)
						{
							LastSentCode = ActiveQueue.Peek();
							object userFields = AppCSIUser.Promo.UserFields;
							if (Operators.ConditionalCompareObjectEqual(userFields, 1, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek()));
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields, 2, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields, 3, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 2, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2, AppCSIUser.Promo.Split3)));
								}
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields, 4, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 2, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2, AppCSIUser.Promo.Split3)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 3, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2 + AppCSIUser.Promo.Split3, AppCSIUser.Promo.Split4)));
								}
							}
							switch (i)
							{
							case 0:
								ActiveQueue.Dequeue();
								ErrorCnt = 0;
								break;
							default:
								Common.MACSALog("ERROR USERFIELDS: " + i, TraceEventType.Error);
								LaserQueueError = true;
								return;
							case 8:
								ErrorCnt++;
								Thread.Sleep(MySettingsProperty.Settings.WaitTimeOnLaserQueueFull);
								if (ErrorCnt == 80)
								{
									ErrorCnt = 0;
									Laser.GetState();
									if (Operators.CompareString(Laser.ErrorDesc, "", TextCompare: false) != 0)
									{
										LaserQueueError = true;
										return;
									}
								}
								break;
							}
						}
						else
						{
							ActiveQueue = Queue2;
						}
					}
				}
				if (ActiveQueue.Equals(Queue2))
				{
					lock (Queue2)
					{
						if (Queue2.Count > 0)
						{
							LastSentCode = ActiveQueue.Peek();
							object userFields2 = AppCSIUser.Promo.UserFields;
							if (Operators.ConditionalCompareObjectEqual(userFields2, 1, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek()));
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields2, 2, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields2, 3, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 2, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2, AppCSIUser.Promo.Split3)));
								}
							}
							else if (Operators.ConditionalCompareObjectEqual(userFields2, 4, TextCompare: false))
							{
								i = LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 0, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(0, AppCSIUser.Promo.Split1)));
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 1, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1, AppCSIUser.Promo.Split2)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 2, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2, AppCSIUser.Promo.Split3)));
								}
								if (i == 0)
								{
									i |= LaserDLL.MLaser_FastUTF8Usermessage(Laser.Sock2, 3, Encoding.UTF8.GetBytes(ActiveQueue.Peek().Substring(AppCSIUser.Promo.Split1 + AppCSIUser.Promo.Split2 + AppCSIUser.Promo.Split3, AppCSIUser.Promo.Split4)));
								}
							}
							switch (i)
							{
							case 0:
								ActiveQueue.Dequeue();
								ErrorCnt = 0;
								break;
							default:
								Common.MACSALog("ERROR USERFIELDS: " + i, TraceEventType.Error);
								LaserQueueError = true;
								return;
							case 8:
								ErrorCnt++;
								Thread.Sleep(MySettingsProperty.Settings.WaitTimeOnLaserQueueFull);
								if (ErrorCnt == 80)
								{
									ErrorCnt = 0;
									Laser.GetState();
									if (Operators.CompareString(Laser.ErrorDesc, "", TextCompare: false) != 0)
									{
										LaserQueueError = true;
										return;
									}
								}
								break;
							}
						}
						else
						{
							ActiveQueue = Queue1;
						}
					}
				}
				Thread.Sleep(MySettingsProperty.Settings.WaitTime);
			}
		}
	}

	public void QueueConsumerDataString(object StateInfo)
	{
		checked
		{
			int i = default(int);
			int ErrorCnt = default(int);
			while (Laser.State == CLaser.LASER_STATES.MARKING)
			{
				if (ActiveQueueDataString.Equals(QueueDataString1))
				{
					lock (QueueDataString1)
					{
						if (QueueDataString1.Count > 0)
						{
							LastSentCode = ActiveQueueDataString.Peek().Code;
							object userFields = AppCSIUser.Promo.UserFields;
							if (Operators.ConditionalCompareObjectEqual(userFields, 1, TextCompare: false))
							{
								byte[] Data = ActiveQueueDataString.Peek().Data;
								i = LaserDLL.MLaser_FastDataString(Laser.Sock2, 0, Data, Data.Length);
							}
							switch (i)
							{
							case 0:
								ActiveQueueDataString.Dequeue();
								ErrorCnt = 0;
								break;
							default:
								Common.MACSALog("ERROR USERFIELDS: " + i, TraceEventType.Error);
								LaserQueueError = true;
								return;
							case 8:
								ErrorCnt++;
								Thread.Sleep(MySettingsProperty.Settings.WaitTimeOnLaserQueueFull);
								if (ErrorCnt == 80)
								{
									ErrorCnt = 0;
									Laser.GetState();
									if (Operators.CompareString(Laser.ErrorDesc, "", TextCompare: false) != 0)
									{
										LaserQueueError = true;
										return;
									}
								}
								break;
							}
						}
						else
						{
							ActiveQueueDataString = QueueDataString2;
						}
					}
				}
				if (ActiveQueueDataString.Equals(QueueDataString2))
				{
					lock (Queue2)
					{
						if (Queue2.Count > 0)
						{
							LastSentCode = ActiveQueueDataString.Peek().Code;
							object userFields2 = AppCSIUser.Promo.UserFields;
							if (Operators.ConditionalCompareObjectEqual(userFields2, 1, TextCompare: false))
							{
								byte[] Data2 = ActiveQueueDataString.Peek().Data;
								i = LaserDLL.MLaser_FastDataString(Laser.Sock2, 0, Data2, Data2.Length);
							}
							switch (i)
							{
							case 0:
								ActiveQueueDataString.Dequeue();
								ErrorCnt = 0;
								break;
							default:
								Common.MACSALog("ERROR USERFIELDS: " + i, TraceEventType.Error);
								LaserQueueError = true;
								return;
							case 8:
								ErrorCnt++;
								Thread.Sleep(MySettingsProperty.Settings.WaitTimeOnLaserQueueFull);
								if (ErrorCnt == 80)
								{
									ErrorCnt = 0;
									Laser.GetState();
									if (Operators.CompareString(Laser.ErrorDesc, "", TextCompare: false) != 0)
									{
										LaserQueueError = true;
										return;
									}
								}
								break;
							}
						}
						else
						{
							ActiveQueueDataString = QueueDataString1;
						}
					}
				}
				Thread.Sleep(MySettingsProperty.Settings.WaitTime);
			}
		}
	}

	private void btnPercentWarning_Click(object sender, EventArgs e)
	{
		TimerBlinkWarning.Stop();
		lblPercentWarning.ForeColor = Color.White;
		pnlPercentWarning.Visible = false;
	}

	private void TimerBlinkWarning_Tick(object sender, EventArgs e)
	{
		if (lblPercentWarning.ForeColor == Color.White)
		{
			lblPercentWarning.ForeColor = lblPercentWarning.BackColor;
		}
		else
		{
			lblPercentWarning.ForeColor = Color.White;
		}
	}

	public void WarningLowLevelCodes()
	{
		if (PgbTotal.Value < MySettingsProperty.Settings.LowCodes)
		{
			lblPercentWarning.Text = AppCSIUser.Rm.GetString("String54") + " " + PgbTotal.Value + "% " + AppCSIUser.Rm.GetString("String55");
			if (PercentWarning == 100)
			{
				PercentWarning = 50;
				if (MySettingsProperty.Settings.ShowLowCodes)
				{
					pnlPercentWarning.Visible = true;
					lblPercentWarning.BackColor = Color.Orange;
					pbPercentWarning.BackColor = Color.Orange;
					lblPercentWarning.ForeColor = Color.White;
					TimerBlinkWarning.Start();
				}
			}
			if ((double)PgbTotal.Value < Conversions.ToDouble(MySettingsProperty.Settings.VeryLowCodes) && PercentWarning != 25)
			{
				PercentWarning = 25;
				if (MySettingsProperty.Settings.ShowVeryLowCodes)
				{
					pnlPercentWarning.Visible = true;
					lblPercentWarning.BackColor = Color.Red;
					pbPercentWarning.BackColor = Color.Red;
					lblPercentWarning.ForeColor = Color.White;
					TimerBlinkWarning.Start();
				}
			}
		}
		else
		{
			pnlPercentWarning.Visible = false;
		}
	}

	private void txtProducir_KeyPress(object sender, KeyPressEventArgs e)
	{
		if ((e.KeyChar != '\b') & !Versioned.IsNumeric(e.KeyChar))
		{
			e.Handled = true;
		}
	}

	private void txtProducir_TextChanged(object sender, EventArgs e)
	{
		btnStart.Text = AppCSIUser.Rm.GetString("String53");
	}

	public void UpdateConsumos(int IdInicial, int IdFinal)
	{
		checked
		{
			int num = AppCSIUser.Promo.FilesImported.Rows.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!Operators.ConditionalCompareObjectGreaterEqual(IdInicial, AppCSIUser.Promo.FilesImported.Rows[i]["FromRecord"], TextCompare: false) || Operators.ConditionalCompareObjectGreaterEqual(IdInicial, AppCSIUser.Promo.FilesImported.Rows[i]["ToRecord"], TextCompare: false))
				{
					continue;
				}
				if (Operators.ConditionalCompareObjectLessEqual(IdFinal, AppCSIUser.Promo.FilesImported.Rows[i]["ToRecord"], TextCompare: false))
				{
					DataRow dataRow;
					(dataRow = AppCSIUser.Promo.FilesImported.Rows[i])["Consumed"] = Operators.AddObject(dataRow["Consumed"], IdFinal - IdInicial + 1);
				}
				else
				{
					DataRow dataRow;
					(dataRow = AppCSIUser.Promo.FilesImported.Rows[i])["Consumed"] = Operators.AddObject(dataRow["Consumed"], Operators.AddObject(Operators.SubtractObject(AppCSIUser.Promo.FilesImported.Rows[i]["ToRecord"], IdInicial), 1));
					if (i < AppCSIUser.Promo.FilesImported.Rows.Count - 1)
					{
						IdInicial = Conversions.ToInteger(AppCSIUser.Promo.FilesImported.Rows[i + 1]["FromRecord"]);
					}
				}
				AppCSIUser.Db.UpdateTable(AppCSIUser.Promo.FilesImported);
			}
		}
	}

	public void DisconnectDB()
	{
		Cursor.Current = Cursors.WaitCursor;
		Common.MACSALog("Detaching all Db's to exit application...", TraceEventType.Information);
		checked
		{
			try
			{
				if (AppCSIUser.DbConnected)
				{
					DataTable Tb = AppCSIUser.Db.GetSqlTable("Select * from Jobs", "Jobs");
					int num = Tb.Rows.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						if (Smo.IsAttached(Path.GetFileNameWithoutExtension(Conversions.ToString(Tb.Rows[i]["CodesDb"]))))
						{
							try
							{
								Common.MACSALog(Conversions.ToString(Operators.ConcatenateObject("Detach Db ", Tb.Rows[i]["CodesDb"])), TraceEventType.Information);
								Smo.DetachDb(Path.GetFileNameWithoutExtension(Conversions.ToString(Tb.Rows[i]["CodesDb"])));
							}
							catch (Exception ex)
							{
								ProjectData.SetProjectError(ex);
								Exception ex2 = ex;
								Common.MACSALog(Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ERROR Detach Db ", Tb.Rows[i]["CodesDb"]), " "), ex2.Message)), TraceEventType.Error);
								ProjectData.ClearProjectError();
							}
						}
					}
					Common.MACSALog("Detaching IPFEu", TraceEventType.Information);
					AppCSIUser.Db.CloseConnection();
					Smo.DetachDb("IPFEu");
				}
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				ProjectData.ClearProjectError();
			}
			Cursor.Current = Cursors.Default;
		}
	}

	private void btnFinOk_Click(object sender, EventArgs e)
	{
		Common.MACSALog("Finaliza la producción programada", TraceEventType.Information);
		btnFinOk.Visible = false;
		InfoPanel(Enabled: false, "", IsWarning: false);
		txtProducir.Text = "";
		txtProducir.Enabled = true;
	}

	public void PLCStateChanged(object Sender, string State, bool IsError)
	{
		if (base.InvokeRequired)
		{
			dlgPLCOnStateChanged Caller = PLCStateChanged;
			Invoke(Caller, Sender, State, IsError);
		}
		else
		{
			lblStatePLC.Text = State;
			if (!IsError)
			{
				pbLedPLC.Image = Resources.green;
			}
			else
			{
				pbLedPLC.Image = Resources.red;
			}
		}
	}

	public void PLCDataReceived(object Sender)
	{
		if (base.InvokeRequired)
		{
			dlgPLCOnDataReceived Caller = PLCDataReceived;
			Invoke(Caller, Sender);
			return;
		}
		if (AppCSIUser.oPLC.RData.Rearmed)
		{
			AppCSIUser.oPLC.WData.GoRearm = false;
		}
		if (AppCSIUser.oPLC.RData.Running)
		{
			AppCSIUser.oPLC.WData.GoRun = false;
		}
		if (AppCSIUser.oPLC.RData.Stopped)
		{
			AppCSIUser.oPLC.WData.GoStop = false;
		}
		pbLedLife.Visible = AppCSIUser.oPLC.RData.Life;
		if (AppCSIUser.oPLC.RData.Running)
		{
			pbLedPLC.Image = Resources.green;
			lblStatePLC.Text = AppCSIUser.Rm.GetString("String96");
		}
		else if (AppCSIUser.oPLC.RData.Stopped)
		{
			pbLedPLC.Image = Resources.green;
			lblStatePLC.Text = AppCSIUser.Rm.GetString("String97");
		}
		else if (AppCSIUser.oPLC.RData.Rearmed)
		{
			pbLedPLC.Image = Resources.green;
			lblStatePLC.Text = AppCSIUser.Rm.GetString("String98");
		}
		else if (AppCSIUser.oPLC.RData.GeneralError)
		{
			pbLedPLC.Image = Resources.red;
			lblStatePLC.Text = AppCSIUser.Rm.GetString("String99");
		}
	}

	public void PLCDataSent(object Sender)
	{
		if (base.InvokeRequired)
		{
			dlgPLCOnDataSent Caller = PLCDataSent;
			Invoke(Caller, Sender);
		}
	}

	public void PLCModbusException(object Sender, byte IdFunction, byte Exception)
	{
		if (base.InvokeRequired)
		{
			dlgPLCOnModbusException Caller = PLCModbusException;
			Invoke(Caller, Sender, IdFunction, Exception);
		}
		else
		{
			lblStatePLC.Text = $"Excepcion Modbus({IdFunction},{Exception})";
			pbLedPLC.Image = Resources.red;
			AppCSIUser.oPLC.RData.GeneralError = true;
		}
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPFUser.frmMain));
		this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
		this.PictureBox3 = new System.Windows.Forms.PictureBox();
		this.PictureBox2 = new System.Windows.Forms.PictureBox();
		this.pnlPLC = new System.Windows.Forms.Panel();
		this.lblStatePLC = new System.Windows.Forms.Label();
		this.Label16 = new System.Windows.Forms.Label();
		this.Panel9 = new System.Windows.Forms.Panel();
		this.pbLedLife = new System.Windows.Forms.PictureBox();
		this.PictureBox4 = new System.Windows.Forms.PictureBox();
		this.pbLedPLC = new System.Windows.Forms.PictureBox();
		this.PictureBox1 = new System.Windows.Forms.PictureBox();
		this.cmbPromoFiles = new System.Windows.Forms.ComboBox();
		this.lblTotalPecent = new System.Windows.Forms.Label();
		this.pnlPercentWarning = new System.Windows.Forms.Panel();
		this.pbPercentWarning = new System.Windows.Forms.PictureBox();
		this.btnPercentWarning = new System.Windows.Forms.Button();
		this.lblPercentWarning = new System.Windows.Forms.Label();
		this.Label12 = new System.Windows.Forms.Label();
		this.lblLaserFile = new System.Windows.Forms.Label();
		this.Label1 = new System.Windows.Forms.Label();
		this.PgbTotal = new System.Windows.Forms.ProgressBar();
		this.lblAvailableCodes = new System.Windows.Forms.Label();
		this.Label9 = new System.Windows.Forms.Label();
		this.lblPromotion = new System.Windows.Forms.Label();
		this.Label2 = new System.Windows.Forms.Label();
		this.lblArtwork = new System.Windows.Forms.Label();
		this.Label3 = new System.Windows.Forms.Label();
		this.PictureBox5 = new System.Windows.Forms.PictureBox();
		this.Label14 = new System.Windows.Forms.Label();
		this.lblLastCodePosition = new System.Windows.Forms.Label();
		this.lblPendientes = new System.Windows.Forms.Label();
		this.Label15 = new System.Windows.Forms.Label();
		this.Label13 = new System.Windows.Forms.Label();
		this.lblLotePercent = new System.Windows.Forms.Label();
		this.lblLastCode = new System.Windows.Forms.Label();
		this.pnlInfo = new System.Windows.Forms.Panel();
		this.btnFinOk = new System.Windows.Forms.Button();
		this.lblInfo = new System.Windows.Forms.Label();
		this.ImgInfo = new System.Windows.Forms.PictureBox();
		this.btnExit = new System.Windows.Forms.Button();
		this.lblProducidos = new System.Windows.Forms.Label();
		this.Label11 = new System.Windows.Forms.Label();
		this.btnStop = new System.Windows.Forms.Button();
		this.btnStart = new System.Windows.Forms.Button();
		this.Label10 = new System.Windows.Forms.Label();
		this.PgbLote = new System.Windows.Forms.ProgressBar();
		this.Label4 = new System.Windows.Forms.Label();
		this.txtPedido = new System.Windows.Forms.TextBox();
		this.Label5 = new System.Windows.Forms.Label();
		this.txtProducir = new System.Windows.Forms.TextBox();
		this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
		this.lblState = new System.Windows.Forms.ToolStripStatusLabel();
		this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
		this.ConfiguraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.SincroMarkState = new System.Windows.Forms.Timer(this.components);
		this.TimerBlinkWarning = new System.Windows.Forms.Timer(this.components);
		((System.ComponentModel.ISupportInitialize)this.SplitContainer1).BeginInit();
		this.SplitContainer1.Panel1.SuspendLayout();
		this.SplitContainer1.Panel2.SuspendLayout();
		this.SplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PictureBox3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox2).BeginInit();
		this.pnlPLC.SuspendLayout();
		this.Panel9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbLedLife).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pbLedPLC).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox1).BeginInit();
		this.pnlPercentWarning.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pbPercentWarning).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox5).BeginInit();
		this.pnlInfo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ImgInfo).BeginInit();
		this.StatusStrip1.SuspendLayout();
		this.MenuStrip1.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(this.SplitContainer1, "SplitContainer1");
		this.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.SplitContainer1.Name = "SplitContainer1";
		resources.ApplyResources(this.SplitContainer1.Panel1, "SplitContainer1.Panel1");
		this.SplitContainer1.Panel1.Controls.Add(this.PictureBox3);
		this.SplitContainer1.Panel1.Controls.Add(this.PictureBox2);
		this.SplitContainer1.Panel1.Controls.Add(this.pnlPLC);
		this.SplitContainer1.Panel1.Controls.Add(this.PictureBox1);
		this.SplitContainer1.Panel1.Controls.Add(this.cmbPromoFiles);
		this.SplitContainer1.Panel1.Controls.Add(this.lblTotalPecent);
		this.SplitContainer1.Panel1.Controls.Add(this.pnlPercentWarning);
		this.SplitContainer1.Panel1.Controls.Add(this.Label12);
		this.SplitContainer1.Panel1.Controls.Add(this.lblLaserFile);
		this.SplitContainer1.Panel1.Controls.Add(this.Label1);
		this.SplitContainer1.Panel1.Controls.Add(this.PgbTotal);
		this.SplitContainer1.Panel1.Controls.Add(this.lblAvailableCodes);
		this.SplitContainer1.Panel1.Controls.Add(this.Label9);
		this.SplitContainer1.Panel1.Controls.Add(this.lblPromotion);
		this.SplitContainer1.Panel1.Controls.Add(this.Label2);
		this.SplitContainer1.Panel1.Controls.Add(this.lblArtwork);
		this.SplitContainer1.Panel1.Controls.Add(this.Label3);
		resources.ApplyResources(this.SplitContainer1.Panel2, "SplitContainer1.Panel2");
		this.SplitContainer1.Panel2.Controls.Add(this.PictureBox5);
		this.SplitContainer1.Panel2.Controls.Add(this.Label14);
		this.SplitContainer1.Panel2.Controls.Add(this.lblLastCodePosition);
		this.SplitContainer1.Panel2.Controls.Add(this.lblPendientes);
		this.SplitContainer1.Panel2.Controls.Add(this.Label15);
		this.SplitContainer1.Panel2.Controls.Add(this.Label13);
		this.SplitContainer1.Panel2.Controls.Add(this.lblLotePercent);
		this.SplitContainer1.Panel2.Controls.Add(this.lblLastCode);
		this.SplitContainer1.Panel2.Controls.Add(this.pnlInfo);
		this.SplitContainer1.Panel2.Controls.Add(this.btnExit);
		this.SplitContainer1.Panel2.Controls.Add(this.lblProducidos);
		this.SplitContainer1.Panel2.Controls.Add(this.Label11);
		this.SplitContainer1.Panel2.Controls.Add(this.btnStop);
		this.SplitContainer1.Panel2.Controls.Add(this.btnStart);
		this.SplitContainer1.Panel2.Controls.Add(this.Label10);
		this.SplitContainer1.Panel2.Controls.Add(this.PgbLote);
		this.SplitContainer1.Panel2.Controls.Add(this.Label4);
		this.SplitContainer1.Panel2.Controls.Add(this.txtPedido);
		this.SplitContainer1.Panel2.Controls.Add(this.Label5);
		this.SplitContainer1.Panel2.Controls.Add(this.txtProducir);
		resources.ApplyResources(this.PictureBox3, "PictureBox3");
		this.PictureBox3.Image = IPFUser.My.Resources.Resources.green;
		this.PictureBox3.Name = "PictureBox3";
		this.PictureBox3.TabStop = false;
		resources.ApplyResources(this.PictureBox2, "PictureBox2");
		this.PictureBox2.Image = IPFUser.My.Resources.Resources.red;
		this.PictureBox2.Name = "PictureBox2";
		this.PictureBox2.TabStop = false;
		resources.ApplyResources(this.pnlPLC, "pnlPLC");
		this.pnlPLC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.pnlPLC.Controls.Add(this.lblStatePLC);
		this.pnlPLC.Controls.Add(this.Label16);
		this.pnlPLC.Controls.Add(this.Panel9);
		this.pnlPLC.Name = "pnlPLC";
		resources.ApplyResources(this.lblStatePLC, "lblStatePLC");
		this.lblStatePLC.BackColor = System.Drawing.Color.White;
		this.lblStatePLC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblStatePLC.Name = "lblStatePLC";
		resources.ApplyResources(this.Label16, "Label16");
		this.Label16.BackColor = System.Drawing.Color.SteelBlue;
		this.Label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.Label16.ForeColor = System.Drawing.Color.White;
		this.Label16.Name = "Label16";
		resources.ApplyResources(this.Panel9, "Panel9");
		this.Panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.Panel9.Controls.Add(this.pbLedLife);
		this.Panel9.Controls.Add(this.PictureBox4);
		this.Panel9.Controls.Add(this.pbLedPLC);
		this.Panel9.Name = "Panel9";
		resources.ApplyResources(this.pbLedLife, "pbLedLife");
		this.pbLedLife.Image = IPFUser.My.Resources.Resources.Heart16;
		this.pbLedLife.Name = "pbLedLife";
		this.pbLedLife.TabStop = false;
		resources.ApplyResources(this.PictureBox4, "PictureBox4");
		this.PictureBox4.Image = IPFUser.My.Resources.Resources.panasonic;
		this.PictureBox4.Name = "PictureBox4";
		this.PictureBox4.TabStop = false;
		resources.ApplyResources(this.pbLedPLC, "pbLedPLC");
		this.pbLedPLC.Image = IPFUser.My.Resources.Resources.red;
		this.pbLedPLC.Name = "pbLedPLC";
		this.pbLedPLC.TabStop = false;
		resources.ApplyResources(this.PictureBox1, "PictureBox1");
		this.PictureBox1.Image = IPFUser.My.Resources.Resources.logo_macsa_2011;
		this.PictureBox1.Name = "PictureBox1";
		this.PictureBox1.TabStop = false;
		resources.ApplyResources(this.cmbPromoFiles, "cmbPromoFiles");
		this.cmbPromoFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cmbPromoFiles.FormattingEnabled = true;
		this.cmbPromoFiles.Name = "cmbPromoFiles";
		resources.ApplyResources(this.lblTotalPecent, "lblTotalPecent");
		this.lblTotalPecent.Name = "lblTotalPecent";
		resources.ApplyResources(this.pnlPercentWarning, "pnlPercentWarning");
		this.pnlPercentWarning.Controls.Add(this.pbPercentWarning);
		this.pnlPercentWarning.Controls.Add(this.btnPercentWarning);
		this.pnlPercentWarning.Controls.Add(this.lblPercentWarning);
		this.pnlPercentWarning.Name = "pnlPercentWarning";
		resources.ApplyResources(this.pbPercentWarning, "pbPercentWarning");
		this.pbPercentWarning.BackColor = System.Drawing.Color.Orange;
		this.pbPercentWarning.Image = IPFUser.My.Resources.Resources.Warning;
		this.pbPercentWarning.Name = "pbPercentWarning";
		this.pbPercentWarning.TabStop = false;
		resources.ApplyResources(this.btnPercentWarning, "btnPercentWarning");
		this.btnPercentWarning.Name = "btnPercentWarning";
		this.btnPercentWarning.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.lblPercentWarning, "lblPercentWarning");
		this.lblPercentWarning.BackColor = System.Drawing.Color.Orange;
		this.lblPercentWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblPercentWarning.ForeColor = System.Drawing.Color.Black;
		this.lblPercentWarning.Name = "lblPercentWarning";
		resources.ApplyResources(this.Label12, "Label12");
		this.Label12.Name = "Label12";
		resources.ApplyResources(this.lblLaserFile, "lblLaserFile");
		this.lblLaserFile.BackColor = System.Drawing.Color.White;
		this.lblLaserFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblLaserFile.Name = "lblLaserFile";
		resources.ApplyResources(this.Label1, "Label1");
		this.Label1.BackColor = System.Drawing.Color.Navy;
		this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.Label1.ForeColor = System.Drawing.Color.White;
		this.Label1.Name = "Label1";
		resources.ApplyResources(this.PgbTotal, "PgbTotal");
		this.PgbTotal.Name = "PgbTotal";
		resources.ApplyResources(this.lblAvailableCodes, "lblAvailableCodes");
		this.lblAvailableCodes.BackColor = System.Drawing.Color.White;
		this.lblAvailableCodes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblAvailableCodes.Name = "lblAvailableCodes";
		resources.ApplyResources(this.Label9, "Label9");
		this.Label9.Name = "Label9";
		resources.ApplyResources(this.lblPromotion, "lblPromotion");
		this.lblPromotion.BackColor = System.Drawing.Color.White;
		this.lblPromotion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblPromotion.Name = "lblPromotion";
		resources.ApplyResources(this.Label2, "Label2");
		this.Label2.Name = "Label2";
		resources.ApplyResources(this.lblArtwork, "lblArtwork");
		this.lblArtwork.BackColor = System.Drawing.Color.Yellow;
		this.lblArtwork.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblArtwork.Name = "lblArtwork";
		resources.ApplyResources(this.Label3, "Label3");
		this.Label3.Name = "Label3";
		resources.ApplyResources(this.PictureBox5, "PictureBox5");
		this.PictureBox5.Image = IPFUser.My.Resources.Resources.green;
		this.PictureBox5.Name = "PictureBox5";
		this.PictureBox5.TabStop = false;
		resources.ApplyResources(this.Label14, "Label14");
		this.Label14.Name = "Label14";
		resources.ApplyResources(this.lblLastCodePosition, "lblLastCodePosition");
		this.lblLastCodePosition.BackColor = System.Drawing.Color.White;
		this.lblLastCodePosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblLastCodePosition.Name = "lblLastCodePosition";
		resources.ApplyResources(this.lblPendientes, "lblPendientes");
		this.lblPendientes.BackColor = System.Drawing.Color.LightCyan;
		this.lblPendientes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblPendientes.Name = "lblPendientes";
		resources.ApplyResources(this.Label15, "Label15");
		this.Label15.Name = "Label15";
		resources.ApplyResources(this.Label13, "Label13");
		this.Label13.Name = "Label13";
		resources.ApplyResources(this.lblLotePercent, "lblLotePercent");
		this.lblLotePercent.Name = "lblLotePercent";
		resources.ApplyResources(this.lblLastCode, "lblLastCode");
		this.lblLastCode.BackColor = System.Drawing.Color.White;
		this.lblLastCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblLastCode.Name = "lblLastCode";
		resources.ApplyResources(this.pnlInfo, "pnlInfo");
		this.pnlInfo.BackColor = System.Drawing.Color.Red;
		this.pnlInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.pnlInfo.Controls.Add(this.btnFinOk);
		this.pnlInfo.Controls.Add(this.lblInfo);
		this.pnlInfo.Controls.Add(this.ImgInfo);
		this.pnlInfo.Name = "pnlInfo";
		resources.ApplyResources(this.btnFinOk, "btnFinOk");
		this.btnFinOk.Name = "btnFinOk";
		this.btnFinOk.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.lblInfo, "lblInfo");
		this.lblInfo.BackColor = System.Drawing.Color.Transparent;
		this.lblInfo.ForeColor = System.Drawing.Color.White;
		this.lblInfo.Name = "lblInfo";
		resources.ApplyResources(this.ImgInfo, "ImgInfo");
		this.ImgInfo.Image = IPFUser.My.Resources.Resources.Warning;
		this.ImgInfo.Name = "ImgInfo";
		this.ImgInfo.TabStop = false;
		resources.ApplyResources(this.btnExit, "btnExit");
		this.btnExit.Image = IPFUser.My.Resources.Resources.door_in;
		this.btnExit.Name = "btnExit";
		this.btnExit.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.lblProducidos, "lblProducidos");
		this.lblProducidos.BackColor = System.Drawing.Color.White;
		this.lblProducidos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.lblProducidos.Name = "lblProducidos";
		resources.ApplyResources(this.Label11, "Label11");
		this.Label11.Name = "Label11";
		resources.ApplyResources(this.btnStop, "btnStop");
		this.btnStop.Image = IPFUser.My.Resources.Resources.Delete;
		this.btnStop.Name = "btnStop";
		this.btnStop.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.btnStart, "btnStart");
		this.btnStart.Image = IPFUser.My.Resources.Resources.Check;
		this.btnStart.Name = "btnStart";
		this.btnStart.UseVisualStyleBackColor = true;
		resources.ApplyResources(this.Label10, "Label10");
		this.Label10.BackColor = System.Drawing.Color.Navy;
		this.Label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.Label10.ForeColor = System.Drawing.Color.White;
		this.Label10.Name = "Label10";
		resources.ApplyResources(this.PgbLote, "PgbLote");
		this.PgbLote.Name = "PgbLote";
		resources.ApplyResources(this.Label4, "Label4");
		this.Label4.Name = "Label4";
		resources.ApplyResources(this.txtPedido, "txtPedido");
		this.txtPedido.Name = "txtPedido";
		resources.ApplyResources(this.Label5, "Label5");
		this.Label5.Name = "Label5";
		resources.ApplyResources(this.txtProducir, "txtProducir");
		this.txtProducir.Name = "txtProducir";
		resources.ApplyResources(this.StatusStrip1, "StatusStrip1");
		this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.lblState });
		this.StatusStrip1.Name = "StatusStrip1";
		resources.ApplyResources(this.lblState, "lblState");
		this.lblState.Name = "lblState";
		resources.ApplyResources(this.MenuStrip1, "MenuStrip1");
		this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.ConfiguraciónToolStripMenuItem });
		this.MenuStrip1.Name = "MenuStrip1";
		resources.ApplyResources(this.ConfiguraciónToolStripMenuItem, "ConfiguraciónToolStripMenuItem");
		this.ConfiguraciónToolStripMenuItem.Name = "ConfiguraciónToolStripMenuItem";
		this.SincroMarkState.Interval = 2000;
		this.TimerBlinkWarning.Interval = 500;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.SplitContainer1);
		base.Controls.Add(this.StatusStrip1);
		base.Controls.Add(this.MenuStrip1);
		base.MainMenuStrip = this.MenuStrip1;
		base.MaximizeBox = false;
		base.Name = "frmMain";
		this.SplitContainer1.Panel1.ResumeLayout(false);
		this.SplitContainer1.Panel1.PerformLayout();
		this.SplitContainer1.Panel2.ResumeLayout(false);
		this.SplitContainer1.Panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.SplitContainer1).EndInit();
		this.SplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PictureBox3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox2).EndInit();
		this.pnlPLC.ResumeLayout(false);
		this.Panel9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pbLedLife).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pbLedPLC).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox1).EndInit();
		this.pnlPercentWarning.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pbPercentWarning).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PictureBox5).EndInit();
		this.pnlInfo.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ImgInfo).EndInit();
		this.StatusStrip1.ResumeLayout(false);
		this.StatusStrip1.PerformLayout();
		this.MenuStrip1.ResumeLayout(false);
		this.MenuStrip1.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
