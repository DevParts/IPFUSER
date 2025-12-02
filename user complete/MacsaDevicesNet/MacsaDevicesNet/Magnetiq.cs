using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

public abstract class Magnetiq : Etiquetadora
{
	public enum ML_CMDS
	{
		TRANSPARENT_MODE_INIT = 144,
		TRANSPARENT_MODE_END = 145,
		IDENTIFY_DEVICE = 0,
		STATUS = 1,
		READ_PARAMETERS = 2,
		WRITE_PARAMETERS = 130,
		TESTIO = 84,
		OUTPUT_ACTIVATION_OK = 79,
		OUTPUT_ACTIVATION_ERROR = 69,
		MODE_RUN = 138,
		MODE_OFFLINE = 1,
		MODE_ONLINE = 0,
		MODE_ONLINE25 = 37,
		SCANNER_GETLASTREAD = 13,
		SIMULATE_INPUT = 141,
		MODE_PROACTIVE_IE = 148,
		MODE_PROACTIVE_OE = 149,
		WRITE_COUNTERS = 139,
		READ_FAMILY = 12,
		WRITE_FAMILY = 140,
		READ_FAMILY_MULTI = 14,
		WRITE_FAMILY_MULTI = 142,
		READ_FAMILY_HS = 15,
		WRITE_FAMILY_HS = 147
	}

	public enum DEVICE_TYPE : byte
	{
		MLP = 128,
		MLA = 65,
		MLC = 67
	}

	public enum ML_DO_STATE
	{
		DISABLED = 115,
		ENABLED = 83
	}

	public enum ML_IO_TYPE
	{
		INPUT = 1,
		OUTPUT
	}

	public enum COUNTER_TYPES
	{
		PARTIAL,
		ACUMULATED,
		TOTAL,
		BOTH
	}

	public enum ML_IOS_INDEXS
	{
		IE1 = 1,
		IE2,
		IE3,
		IE4
	}

	public enum ML_IO_STATE
	{
		DISABLED,
		ENABLED,
		ERR,
		NOEXIST
	}

	public struct PORT_ETHERNET
	{
		public uint Ip;

		public uint NetMask;

		public uint GateWay;

		public byte Enable;

		public byte RFU;

		public ushort Port;
	}

	public struct ML_IOS
	{
		public byte DIQuantity;

		public byte DOQuantity;

		public byte AIQuantity;

		public byte AOQuantity;

		public byte IdBoard;

		public byte ActionsQuantity;

		public ML_IO_STATE[] DI;

		public ML_IO_STATE[] DO;

		public byte[] AI;

		public byte[] AO;
	}

	public delegate void OnConnectedEventHandler(object sender);

	public delegate void OnDisconnectedEventHandler(object sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnInitTransparentModeEventHandler(object sender);

	public delegate void OnEndTransparentModeEventHandler(object sender);

	public delegate void OnAnswerReceivedEventHandler(object sender, ML_CMDS eCmd);

	public delegate void OnBadChecksumEventHandler(object sender);

	public delegate void OnOutputActivationOkEventHandler(object sender);

	public delegate void OnOutputActivationErrorEventHandler(object sender);

	public delegate void OnScannerDataReceivedEventHandler(object sender, string sData);

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnFamilyReadEventHandler(object sender, byte[] Data);

	public delegate void OnFamilyMultiReadEventHandler(object sender, byte[] Data);

	public delegate void OnFamilyHSReadEventHandler(object sender, byte[] Data);

	protected const byte ML_OFFLINE = 1;

	protected const byte ML_ONLINE = 0;

	private const byte ML_CMD_ACCEPTED = 1;

	public ML_IOS IO;

	protected ML_IOS LastIO;

	protected override SerialPort SerialComm
	{
		[DebuggerNonUserCode]
		get
		{
			return base.SerialComm;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			SerialDataReceivedEventHandler value2 = mComm_DataReceived;
			if (base.SerialComm != null)
			{
				base.SerialComm.DataReceived -= value2;
			}
			base.SerialComm = value;
			if (base.SerialComm != null)
			{
				base.SerialComm.DataReceived += value2;
			}
		}
	}

	public DEVICE_TYPE DeviceType
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	[method: DebuggerNonUserCode]
	public event OnConnectedEventHandler OnConnected;

	[method: DebuggerNonUserCode]
	public event OnDisconnectedEventHandler OnDisconnected;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveLabelEventHandler OnReadyToReceiveLabel;

	[method: DebuggerNonUserCode]
	public event OnInitTransparentModeEventHandler OnInitTransparentMode;

	[method: DebuggerNonUserCode]
	public event OnEndTransparentModeEventHandler OnEndTransparentMode;

	[method: DebuggerNonUserCode]
	public event OnAnswerReceivedEventHandler OnAnswerReceived;

	[method: DebuggerNonUserCode]
	public event OnBadChecksumEventHandler OnBadChecksum;

	[method: DebuggerNonUserCode]
	public event OnOutputActivationOkEventHandler OnOutputActivationOk;

	[method: DebuggerNonUserCode]
	public event OnOutputActivationErrorEventHandler OnOutputActivationError;

	[method: DebuggerNonUserCode]
	public event OnScannerDataReceivedEventHandler OnScannerDataReceived;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnFamilyReadEventHandler OnFamilyRead;

	[method: DebuggerNonUserCode]
	public event OnFamilyMultiReadEventHandler OnFamilyMultiRead;

	[method: DebuggerNonUserCode]
	public event OnFamilyHSReadEventHandler OnFamilyHSRead;

	[DebuggerNonUserCode]
	protected Magnetiq()
	{
	}

	public abstract void MapParameters(byte[] bBuff);

	public abstract void MapStatus(byte[] bBuff);

	public abstract void DetectIOChanges();

	public void ClearBufferDataReceived()
	{
		bBuffIn = new byte[1];
		sBuffIn = "";
	}

	public override bool SendLabel(string sPathLabel)
	{
		bool result = false;
		sPathLabel = Strings.Trim(sPathLabel);
		checked
		{
			if (Operators.CompareString(sPathLabel, "", TextCompare: false) != 0)
			{
				FileStream fileStream = default(FileStream);
				BinaryReader binaryReader = default(BinaryReader);
				try
				{
					fileStream = new FileStream(sPathLabel, FileMode.Open);
					binaryReader = new BinaryReader(fileStream);
					byte[] array = new byte[(int)fileStream.Length + 1];
					binaryReader.Read(array, 0, (int)fileStream.Length);
					result = SendData(array, 0L);
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Common.MACSALog(Common.Rm.GetString("MLP51") + "'" + sPathLabel + "'", TraceEventType.Error, ex2.Message);
					ProjectData.ClearProjectError();
				}
				finally
				{
					binaryReader.Close();
					fileStream.Close();
				}
			}
			return result;
		}
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray("\u0001");
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetIdentification(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray("\0");
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetParameters(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray("\u0002");
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetTestIOs(ML_CMDS iMode, WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray("T" + Conversions.ToString(Strings.Chr((int)iMode)));
		return SendCommand(aBytesToSend, 250, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetScannerLastRead(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray("\r");
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SimulateInput(ML_IOS_INDEXS iInputIndex)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr(141)) + Conversions.ToString(Strings.Chr((int)iInputIndex)));
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetCommMode(ML_CMDS iMode)
	{
		if (iMode != ML_CMDS.TRANSPARENT_MODE_INIT && iMode != ML_CMDS.TRANSPARENT_MODE_END)
		{
			return false;
		}
		byte[] aBytesToSend = default(byte[]);
		switch (iMode)
		{
		case ML_CMDS.TRANSPARENT_MODE_INIT:
			aBytesToSend = Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr((int)iMode)) + "\u0005");
			break;
		case ML_CMDS.TRANSPARENT_MODE_END:
			aBytesToSend = Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr((int)iMode)));
			break;
		}
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetProactiveMode(ML_CMDS iMode, bool bEnable)
	{
		if (iMode != ML_CMDS.MODE_PROACTIVE_IE && iMode != ML_CMDS.MODE_PROACTIVE_OE)
		{
			return false;
		}
		byte[] aBytesToSend = ((!bEnable) ? Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr((int)iMode)) + "\0") : Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr((int)iMode)) + "\u0001"));
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetParoMarcha(ML_CMDS iMode)
	{
		if (iMode != ML_CMDS.STATUS && iMode != ML_CMDS.IDENTIFY_DEVICE)
		{
			return false;
		}
		byte[] aBytesToSend = Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr(138)) + Conversions.ToString(Strings.Chr((int)iMode)));
		return SendCommand(aBytesToSend, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool ChangeDOState(int iDO, ML_DO_STATE eVal)
	{
		byte[] aBytesToSend = Common.ConvertStringToByteArray(Conversions.ToString(Strings.Chr((int)eVal)) + Conversions.ToString(Strings.Chr(iDO)));
		return SendCommand(aBytesToSend, 250, WAIT_TYPE.NoWait, 1000L);
	}

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		return result;
	}

	public bool IsConnected()
	{
		return GetSocket().Connected;
	}

	private void InitIOsStruct()
	{
		checked
		{
			IO.DI = new ML_IO_STATE[IO.DIQuantity + 1];
			IO.DO = new ML_IO_STATE[IO.DOQuantity + 1];
			IO.AI = new byte[IO.AIQuantity + 1];
			IO.AO = new byte[IO.AOQuantity + 1];
		}
	}

	private void MapIOReceived(byte[] bBuff)
	{
		checked
		{
			try
			{
				IO.DIQuantity = bBuff[1];
				IO.DOQuantity = bBuff[2];
				IO.AIQuantity = bBuff[3];
				IO.AOQuantity = bBuff[4];
				IO.IdBoard = bBuff[5];
				IO.ActionsQuantity = bBuff[6];
				InitIOsStruct();
				int num = (int)Math.Round((double)unchecked((int)IO.DIQuantity) / 4.0);
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
					int num6 = num3 * 4;
					unchecked
					{
						IO.DI[num6] = (ML_IO_STATE)(bBuff[checked(8 + num3)] & 3);
						IO.DI[checked(num6 + 1)] = (ML_IO_STATE)(byte)((bBuff[checked(8 + num3)] & 0xC) >>> 2);
						IO.DI[checked(num6 + 2)] = (ML_IO_STATE)(byte)((bBuff[checked(8 + num3)] & 0x30) >>> 4);
						IO.DI[checked(num6 + 3)] = (ML_IO_STATE)(byte)((bBuff[checked(8 + num3)] & 0xC0) >>> 6);
					}
					num3++;
				}
				int num7 = 8 + num3;
				num = (int)Math.Round((double)unchecked((int)IO.DOQuantity) / 4.0);
				int num8 = num - 1;
				num3 = 0;
				while (true)
				{
					int num9 = num3;
					int num5 = num8;
					if (num9 <= num5)
					{
						int num6 = num3 * 4;
						unchecked
						{
							IO.DO[num6] = (ML_IO_STATE)(bBuff[checked(num7 + num3)] & 3);
							IO.DO[checked(num6 + 1)] = (ML_IO_STATE)(byte)((bBuff[checked(num7 + num3)] & 0xC) >>> 2);
							IO.DO[checked(num6 + 2)] = (ML_IO_STATE)(byte)((bBuff[checked(num7 + num3)] & 0x30) >>> 4);
							IO.DO[checked(num6 + 3)] = (ML_IO_STATE)(byte)((bBuff[checked(num7 + num3)] & 0xC0) >>> 6);
						}
						num3++;
						continue;
					}
					break;
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("MLP52"), TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	private int CalculateChecksum(byte[] bBuff)
	{
		int num = 512;
		checked
		{
			int num2 = bBuff.Length - 3;
			int num3 = 2;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				int num6 = 65535;
				num6 = ((num6 & bBuff[num3]) << 8) | bBuff[num3 + 1];
				num ^= num6;
				num3 += 2;
			}
			return num;
		}
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		checked
		{
			try
			{
				int num = bBuffIn.Length - 1;
				int bytesToRead = SerialComm.BytesToRead;
				if (num != 0)
				{
					num++;
				}
				bBuffIn = (byte[])Utils.CopyArray(bBuffIn, new byte[num + bytesToRead - 1 + 1]);
				int num2 = num;
				int num3 = bBuffIn.Length - 1;
				int num4 = num2;
				while (true)
				{
					int num5 = num4;
					int num6 = num3;
					if (num5 > num6)
					{
						break;
					}
					bBuffIn[num4] = byte.MaxValue;
					num4++;
				}
				SerialComm.Read(bBuffIn, num, bytesToRead);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("MLP53"), TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
			try
			{
				int num7 = bBuffIn.Length - 1;
				if (num7 <= 2)
				{
					return;
				}
				byte b = bBuffIn[1];
				byte b2 = bBuffIn[2];
				int num8 = b2 + 4;
				if (num7 >= num8)
				{
					int num9 = bBuffIn[num8];
					if (unchecked(bBuffIn[0] == 2 && num9 == 4))
					{
						byte[] array = new byte[b2 - 1 + 1];
						Array.ConstrainedCopy(bBuffIn, 3, array, 0, b2);
						AnswerRecv = true;
						ProcessInputData(array);
						ClearBufferDataReceived();
					}
				}
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				Common.MACSALog(Common.Rm.GetString("MLP54"), TraceEventType.Error, ex4.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	private void ProcessInputData(byte[] byBuffData)
	{
		try
		{
			bool flag = false;
			if (byBuffData.Length == 1)
			{
				if (byBuffData[0] == 79)
				{
					OnOutputActivationOk?.Invoke(this);
				}
				else
				{
					OnOutputActivationError?.Invoke(this);
				}
				return;
			}
			byte b = byBuffData[0];
			byte b2 = byBuffData[1];
			checked
			{
				if (b == 2)
				{
					try
					{
						int num = 0xFFFF & byBuffData[byBuffData.Length - 2];
						num = (num << 8) | byBuffData[byBuffData.Length - 1];
						int num2 = CalculateChecksum(byBuffData);
						if (num != num2)
						{
							OnBadChecksum?.Invoke(this);
						}
					}
					catch (Exception ex)
					{
						ProjectData.SetProjectError(ex);
						Exception ex2 = ex;
						Common.MACSALog(Common.Rm.GetString("MLP55"), TraceEventType.Error, ex2.Message);
						ProjectData.ClearProjectError();
					}
				}
			}
			switch ((ML_CMDS)b)
			{
			case ML_CMDS.IDENTIFY_DEVICE:
				DeviceType = (DEVICE_TYPE)byBuffData[2];
				OnAnswerReceived?.Invoke(this, ML_CMDS.IDENTIFY_DEVICE);
				break;
			case ML_CMDS.SCANNER_GETLASTREAD:
				OnAnswerReceived?.Invoke(this, ML_CMDS.SCANNER_GETLASTREAD);
				checked
				{
					if (b2 == 1)
					{
						byte[] array = new byte[byBuffData.Length - 4 + 1];
						Array.ConstrainedCopy(byBuffData, 2, array, 0, byBuffData.Length - 3);
						string sData = Encoding.Default.GetString(array);
						OnScannerDataReceived?.Invoke(this, sData);
					}
					else
					{
						OnScannerDataReceived?.Invoke(this, "NOK");
					}
					break;
				}
			case ML_CMDS.SIMULATE_INPUT:
				OnAnswerReceived?.Invoke(this, ML_CMDS.SIMULATE_INPUT);
				if (b2 != 1)
				{
				}
				break;
			case ML_CMDS.READ_PARAMETERS:
				if (b2 == 1)
				{
					MapParameters(byBuffData);
					OnAnswerReceived?.Invoke(this, ML_CMDS.READ_PARAMETERS);
				}
				break;
			case ML_CMDS.STATUS:
				if (b2 == 1)
				{
					MapStatus(byBuffData);
					OnAnswerReceived?.Invoke(this, ML_CMDS.STATUS);
				}
				break;
			case ML_CMDS.TESTIO:
				MapIOReceived(byBuffData);
				OnAnswerReceived?.Invoke(this, ML_CMDS.TESTIO);
				if (LastIO.DIQuantity != 0)
				{
					DetectIOChanges();
				}
				LastIO = IO;
				break;
			case ML_CMDS.MODE_RUN:
				if (b2 == 1)
				{
					OnAnswerReceived?.Invoke(this, ML_CMDS.MODE_RUN);
				}
				break;
			case ML_CMDS.TRANSPARENT_MODE_INIT:
				if (b2 == 1)
				{
					OnInitTransparentMode?.Invoke(this);
				}
				break;
			case ML_CMDS.TRANSPARENT_MODE_END:
				if (b2 == 1)
				{
					OnEndTransparentMode?.Invoke(this);
				}
				break;
			case ML_CMDS.MODE_PROACTIVE_IE:
				OnAnswerReceived?.Invoke(this, ML_CMDS.MODE_PROACTIVE_IE);
				if (b2 != 1)
				{
				}
				break;
			case ML_CMDS.MODE_PROACTIVE_OE:
				OnAnswerReceived?.Invoke(this, ML_CMDS.MODE_PROACTIVE_OE);
				if (b2 != 1)
				{
				}
				break;
			case ML_CMDS.WRITE_COUNTERS:
				OnAnswerReceived?.Invoke(this, ML_CMDS.WRITE_COUNTERS);
				if (b2 != 1)
				{
				}
				break;
			case ML_CMDS.WRITE_PARAMETERS:
				if (b2 == 1)
				{
					OnAnswerReceived?.Invoke(this, ML_CMDS.WRITE_PARAMETERS);
				}
				break;
			case ML_CMDS.READ_FAMILY:
				if (b2 == 1)
				{
					OnFamilyRead?.Invoke(this, byBuffData);
					OnAnswerReceived?.Invoke(this, ML_CMDS.READ_FAMILY);
				}
				break;
			case ML_CMDS.WRITE_FAMILY:
				if (b2 == 1)
				{
					OnAnswerReceived?.Invoke(this, ML_CMDS.WRITE_FAMILY);
				}
				break;
			case ML_CMDS.READ_FAMILY_MULTI:
				if (b2 == 1)
				{
					OnFamilyMultiRead?.Invoke(this, byBuffData);
					OnAnswerReceived?.Invoke(this, ML_CMDS.READ_FAMILY_MULTI);
				}
				break;
			case ML_CMDS.WRITE_FAMILY_MULTI:
				if (b2 == 1)
				{
					OnAnswerReceived?.Invoke(this, ML_CMDS.WRITE_FAMILY_MULTI);
				}
				break;
			case ML_CMDS.READ_FAMILY_HS:
				if (b2 == 1)
				{
					OnFamilyHSRead?.Invoke(this, byBuffData);
					OnAnswerReceived?.Invoke(this, ML_CMDS.READ_FAMILY_HS);
				}
				break;
			case ML_CMDS.WRITE_FAMILY_HS:
				if (b2 == 1)
				{
					OnAnswerReceived?.Invoke(this, ML_CMDS.WRITE_FAMILY_HS);
				}
				break;
			}
			OnConnected?.Invoke(this);
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			Common.MACSALog(Common.Rm.GetString("MLP50"), TraceEventType.Error, ex4.Message);
			ProjectData.ClearProjectError();
		}
	}

	private string BuildFrame(byte[] bByteData, byte byAddress = 1)
	{
		int num = checked(bByteData.Length - 1);
		int num2 = 0;
		byte b = default(byte);
		string text = default(string);
		while (true)
		{
			int num3 = num2;
			int num4 = num;
			if (num3 > num4)
			{
				break;
			}
			b ^= bByteData[num2];
			text += Conversions.ToString(Strings.Chr(bByteData[num2]));
			num2 = checked(num2 + 1);
		}
		return "\u0002" + Conversions.ToString(Strings.Chr(byAddress)) + Conversions.ToString(Strings.Chr(bByteData.Length)) + text + Conversions.ToString(Strings.Chr(b)) + "\u0004";
	}

	protected void EventError(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType)
	{
		OnError?.Invoke(RuntimeHelpers.GetObjectValue(sender), sErrCode, sErrDesc, tErrType);
	}

	protected bool SendCommand(byte[] aBytesToSend, byte byAddress = 1, WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		ClearBufferDataReceived();
		bool result = default(bool);
		try
		{
			if (!SendData(Common.ConvertStringToByteArray(BuildFrame(aBytesToSend, byAddress)), lTimeToWait, tWaitType))
			{
				OnError?.Invoke(this, "ML_NO_COMM", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
				OnDisconnected?.Invoke(this);
				result = false;
				return result;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("MLP56"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		checked
		{
			try
			{
				int num = bBuffIn.Length - 1;
				int length = sData.Length;
				if (num != 0)
				{
					num++;
				}
				bBuffIn = (byte[])Utils.CopyArray(bBuffIn, new byte[num + length - 1 + 1]);
				int num2 = num;
				int num3 = bBuffIn.Length - 1;
				int num4 = num2;
				while (true)
				{
					int num5 = num4;
					int num6 = num3;
					if (num5 > num6)
					{
						break;
					}
					bBuffIn[num4] = byte.MaxValue;
					num4++;
				}
				Array.Copy(Encoding.Default.GetBytes(sData), 0, bBuffIn, num, length);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("MLP57"), TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
			try
			{
				int num7 = bBuffIn.Length - 1;
				if (num7 <= 2)
				{
					return;
				}
				byte b = bBuffIn[1];
				byte b2 = bBuffIn[2];
				int num8 = b2 + 4;
				if (num7 >= num8)
				{
					int num9 = bBuffIn[num8];
					if (unchecked(bBuffIn[0] == 2 && num9 == 4))
					{
						byte[] array = new byte[b2 - 1 + 1];
						Array.ConstrainedCopy(bBuffIn, 3, array, 0, b2);
						AnswerRecv = true;
						ProcessInputData(array);
						ClearBufferDataReceived();
					}
				}
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				Common.MACSALog(Common.Rm.GetString("MLP54"), TraceEventType.Error, ex4.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return sBuff;
	}
}
