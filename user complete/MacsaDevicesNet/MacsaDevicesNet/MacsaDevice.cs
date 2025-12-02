using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

public abstract class MacsaDevice
{
	public delegate void OnAnswerWaitingEventHandler(object sender);

	public delegate void OnThreadTimeoutEventHandler(object sender);

	public delegate void OnSocketClosedByRemoteHostEventHandler(object sender);

	public enum CONNECTION_TYPE
	{
		Unknown,
		Serial,
		TcpIp
	}

	public enum WAIT_TYPE
	{
		NoWait,
		Activa,
		Thread
	}

	public const byte NUL = 0;

	public const byte SOH = 1;

	public const byte STX = 2;

	public const byte ETX = 3;

	public const byte EOT = 4;

	public const byte ENQ = 5;

	public const byte ACK = 6;

	public const byte BEL = 7;

	public const byte BS = 8;

	public const byte HT = 9;

	public const byte LF = 10;

	public const byte CR = 13;

	public const byte SI = 15;

	public const byte DLE = 16;

	public const byte DC1 = 17;

	public const byte DC2 = 18;

	public const byte DC3 = 19;

	public const byte DC4 = 20;

	public const byte NAK = 21;

	public const byte SYN = 22;

	public const byte ETB = 23;

	public const byte CAN = 24;

	public const byte ESC = 27;

	public const byte GS = 29;

	public const byte RS = 30;

	public const byte SPC = 32;

	protected string sBuffIn;

	protected byte[] bBuffIn;

	protected bool AnswerRecv;

	protected CONNECTION_TYPE ConnectionType;

	private bool LocalHostIsClosing;

	[AccessedThroughProperty("SerialComm")]
	private SerialPort _SerialComm;

	[AccessedThroughProperty("TcpComm")]
	private TcpClient _TcpComm;

	private byte[] SocketData;

	private string SocketIp;

	private int SocketPort;

	private long IniWait;

	private long WaitTime;

	private object ThreadLockObject;

	protected Thread thWaitResponse;

	private object _Tag;

	protected virtual SerialPort SerialComm
	{
		[DebuggerNonUserCode]
		get
		{
			return _SerialComm;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_SerialComm = value;
		}
	}

	protected virtual TcpClient TcpComm
	{
		[DebuggerNonUserCode]
		get
		{
			return _TcpComm;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			_TcpComm = value;
		}
	}

	public object Tag
	{
		get
		{
			return _Tag;
		}
		set
		{
			_Tag = RuntimeHelpers.GetObjectValue(value);
		}
	}

	[method: DebuggerNonUserCode]
	public event OnAnswerWaitingEventHandler OnAnswerWaiting;

	[method: DebuggerNonUserCode]
	public event OnThreadTimeoutEventHandler OnThreadTimeout;

	[method: DebuggerNonUserCode]
	public event OnSocketClosedByRemoteHostEventHandler OnSocketClosedByRemoteHost;

	public MacsaDevice()
	{
		bBuffIn = new byte[1];
		SerialComm = new SerialPort();
		TcpComm = new TcpClient();
		SocketData = new byte[1025];
		ThreadLockObject = RuntimeHelpers.GetObjectValue(new object());
		thWaitResponse = new Thread(ThreadWaitResp);
		ConnectionType = CONNECTION_TYPE.Unknown;
		AnswerRecv = true;
		sBuffIn = "";
	}

	protected virtual void Finalize()
	{
		LocalHostIsClosing = true;
		thWaitResponse.Abort();
		try
		{
			if (ConnectionType == CONNECTION_TYPE.Serial)
			{
				SerialComm.Close();
			}
			else if (ConnectionType == CONNECTION_TYPE.TcpIp)
			{
				TcpComm.Close();
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String2"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		base.Finalize();
	}

	public CONNECTION_TYPE GetConnectionType()
	{
		return ConnectionType;
	}

	public int GetSocketPort()
	{
		return SocketPort;
	}

	public string GetSocketIp()
	{
		return SocketIp;
	}

	public TcpClient GetSocket()
	{
		return TcpComm;
	}

	public SerialPort GetSerialPort()
	{
		return SerialComm;
	}

	public string GetName()
	{
		return ToString();
	}

	public bool CloseConnection()
	{
		LocalHostIsClosing = true;
		bool result = true;
		try
		{
			if (ConnectionType == CONNECTION_TYPE.Serial)
			{
				SerialComm.Close();
			}
			else if (ConnectionType == CONNECTION_TYPE.TcpIp)
			{
				TcpComm.Close();
			}
			else
			{
				result = false;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String3"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	protected abstract void TcpComm_DataReceived(string sData);

	public bool Init(string sPort, int iBauds, Parity iParity = Parity.None, int iDataBits = 8, StopBits iStopBits = StopBits.One, Handshake iHandshake = Handshake.None, bool bDtrEnabled = false)
	{
		bool result = false;
		ConnectionType = CONNECTION_TYPE.Serial;
		try
		{
			SerialPort serialComm = SerialComm;
			serialComm.PortName = sPort;
			if (serialComm.IsOpen)
			{
				serialComm.Close();
			}
			serialComm.BaudRate = iBauds;
			serialComm.Parity = iParity;
			serialComm.DataBits = iDataBits;
			serialComm.StopBits = iStopBits;
			serialComm.Handshake = iHandshake;
			serialComm.DtrEnable = bDtrEnabled;
			serialComm.Open();
			serialComm.DiscardInBuffer();
			serialComm = null;
			result = true;
			LocalHostIsClosing = false;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String4") + " '" + sPort + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public bool Init(int iPort, string sIp)
	{
		bool result = false;
		ConnectionType = CONNECTION_TYPE.TcpIp;
		SocketIp = sIp;
		SocketPort = iPort;
		try
		{
			if (!MyProject.Computer.Network.Ping(sIp, 300))
			{
				Common.MACSALog(Common.Rm.GetString("String5") + " '" + sIp + "', " + Common.Rm.GetString("String6") + " '" + Conversions.ToString(iPort) + "'", TraceEventType.Error, Common.Rm.GetString("String7"));
				goto IL_0260;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String5") + " '" + sIp + "', " + Common.Rm.GetString("String6") + " '" + Conversions.ToString(iPort) + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
			goto IL_0260;
		}
		try
		{
			if (TcpComm.Connected)
			{
				TcpComm.Close();
			}
			TcpComm = new TcpClient();
			TcpClient tcpComm = TcpComm;
			if (tcpComm.Connected)
			{
				tcpComm.Close();
			}
			tcpComm.Connect(sIp, iPort);
			TcpComm.GetStream().BeginRead(SocketData, 0, 1024, DoRead, null);
			tcpComm = null;
			result = true;
			LocalHostIsClosing = false;
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			Common.MACSALog(Common.Rm.GetString("String5") + " '" + sIp + "', " + Common.Rm.GetString("String6") + " '" + Conversions.ToString(iPort) + "'", TraceEventType.Error, ex4.Message);
			ProjectData.ClearProjectError();
		}
		goto IL_0260;
		IL_0260:
		return result;
	}

	protected bool SendData(string sTxt, long lWaitTime = 0L, WAIT_TYPE tWaitType = WAIT_TYPE.NoWait)
	{
		return SendData(Encoding.Default.GetBytes(sTxt), lWaitTime, tWaitType);
	}

	protected bool SendData(byte[] bData, long lWaitTime = 0L, WAIT_TYPE tWaitTYpe = WAIT_TYPE.NoWait)
	{
		bool result = false;
		try
		{
			if (lWaitTime > 0)
			{
				AnswerRecv = false;
			}
			else
			{
				AnswerRecv = true;
			}
			if (GetConnectionType() == CONNECTION_TYPE.Serial)
			{
				if (!SerialComm.IsOpen)
				{
					SerialComm.Open();
				}
				SerialComm.Write(bData, 0, checked(Information.UBound(bData) + 1));
				result = true;
				goto IL_0159;
			}
			if (GetConnectionType() != CONNECTION_TYPE.TcpIp)
			{
				goto IL_0159;
			}
			if (TcpComm.Client != null && TcpComm.Connected)
			{
				goto IL_0140;
			}
			if (MyProject.Computer.Network.Ping(SocketIp))
			{
				TcpComm = new TcpClient();
				TcpComm.Connect(SocketIp, SocketPort);
				if (TcpComm.Connected)
				{
					TcpComm.GetStream().BeginRead(SocketData, 0, 1024, DoRead, null);
				}
				goto IL_0140;
			}
			Common.MACSALog(Common.Rm.GetString("String8"), TraceEventType.Error);
			result = false;
			goto end_IL_0004;
			IL_0159:
			result = WaitGetAnswer(lWaitTime, tWaitTYpe);
			goto end_IL_0004;
			IL_0140:
			NetworkStream stream = TcpComm.GetStream();
			stream.Write(bData, 0, bData.Length);
			goto IL_0159;
			end_IL_0004:;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String9"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	protected void FlushComm()
	{
		sBuffIn = "";
		if ((ConnectionType == CONNECTION_TYPE.Serial && SerialComm.IsOpen) ? true : false)
		{
			SerialComm.ReadExisting();
		}
		if ((ConnectionType == CONNECTION_TYPE.TcpIp && TcpComm.Connected) ? true : false)
		{
			int available = TcpComm.Available;
			if (available > 0)
			{
				byte[] buffer = new byte[checked(available + 1)];
				TcpComm.GetStream().Read(buffer, 0, available);
			}
		}
	}

	private void DoRead(IAsyncResult ar)
	{
		string text = "";
		checked
		{
			try
			{
				Common.SetMessages();
				int num = TcpComm.GetStream().EndRead(ar);
				if (num == 0)
				{
					TcpComm.Close();
					if (!LocalHostIsClosing)
					{
						Common.MACSALog(Common.Rm.GetString("String10") + " " + GetSocketIp() + " " + Common.Rm.GetString("String11"), TraceEventType.Error);
						OnSocketClosedByRemoteHost?.Invoke(this);
					}
					return;
				}
				int num2 = num - 1;
				int num3 = 0;
				while (true)
				{
					int num4 = num3;
					int num5 = num2;
					if (num4 > num5)
					{
						break;
					}
					text += Conversions.ToString(Strings.Chr(SocketData[num3]));
					num3++;
				}
				TcpComm_DataReceived(text);
				TcpComm.GetStream().BeginRead(SocketData, 0, 1024, DoRead, null);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				if (!LocalHostIsClosing)
				{
					Common.MACSALog(Common.Rm.GetString("String12") + " " + GetSocketIp(), TraceEventType.Error, ex2.Message);
				}
				ProjectData.ClearProjectError();
			}
		}
	}

	private bool WaitGetAnswer(long lWaitTime = 0L, WAIT_TYPE tWaitType = WAIT_TYPE.NoWait)
	{
		bool result = true;
		try
		{
			if (lWaitTime > 0 && tWaitType != WAIT_TYPE.NoWait)
			{
				if (!thWaitResponse.IsAlive)
				{
					WaitTime = lWaitTime;
					switch (tWaitType)
					{
					case WAIT_TYPE.Thread:
						thWaitResponse = new Thread(ThreadWaitResp);
						thWaitResponse.Start();
						break;
					case WAIT_TYPE.Activa:
						ThreadWaitResp();
						if (AnswerRecv)
						{
							result = true;
						}
						break;
					}
				}
				else
				{
					OnAnswerWaiting?.Invoke(this);
					result = false;
				}
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String13"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	private void ThreadWaitResp()
	{
		object threadLockObject = ThreadLockObject;
		ObjectFlowControl.CheckForSyncLockOnValueType(threadLockObject);
		bool lockTaken = false;
		try
		{
			Monitor.Enter(threadLockObject, ref lockTaken);
			IniWait = MyProject.Computer.Clock.TickCount;
			while ((checked(MyProject.Computer.Clock.TickCount - IniWait) < WaitTime) & !AnswerRecv)
			{
				Application.DoEvents();
			}
			if (!AnswerRecv)
			{
				OnThreadTimeout?.Invoke(this);
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
