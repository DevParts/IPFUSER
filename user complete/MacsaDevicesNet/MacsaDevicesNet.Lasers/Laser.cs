using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;

namespace MacsaDevicesNet.Lasers;

public class Laser : Inyector
{
	public enum PROTOCOL_TYPE
	{
		PT_DEFAULT,
		PT_VERSION4
	}

	public enum FILE_TYPES : byte
	{
		FT_MESSAGES = 1,
		FT_FONTS = 2,
		FT_FONTS_TRUETYPE = 4
	}

	public enum LASER_COMMANDS : short
	{
		NONE = 0,
		SET_COUNTER = 144,
		GET_COUNTER = 146,
		START_PRINT = 45,
		STOP_PRINT = 46,
		SELECT_MESSAGE = 87,
		STATUS = 112,
		USER_MESSAGE = 321,
		GET_MSG_LIST = 294,
		CLOSE_SOFT_CONNECTION = 240
	}

	public enum PRINT_START_MODES : ulong
	{
		PSM_DEFAULT = 0uL,
		PSM_EXTERNAL_SELECTION = 65535uL
	}

	public struct CTRL_PROT
	{
		public PROTOCOL_TYPE Type;

		public int State;

		public int Length;

		public int IndexDataIn;

		public byte[] BinData;

		public long TimeOut;

		public short Command;

		public byte[] Parameters;

		public byte LowCmd;

		public byte HighCmd;
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnCommandErrorEventHandler(object Sender, LASER_COMMANDS Command, string Description);

	public delegate void OnSetCounterEventHandler(object Sender);

	public delegate void OnGetCounterEventHandler(object Sender, int Index, long Value);

	public delegate void OnPrintingStartEventHandler(object Sender);

	public delegate void OnPrintingStopEventHandler(object Sender);

	public delegate void OnMessageSelectedEventHandler(object Sender);

	public delegate void OnUserMessageAppliedEventHandler(object Sender);

	public delegate void OnGetFileListEventHandler(object Sender, string[] Files);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const long WORD = 2L;

	private const long DWORD = 4L;

	private LASER_COMMANDS ActiveCommand;

	private CTRL_PROT CtrlProtocol;

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

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnCommandErrorEventHandler OnCommandError;

	[method: DebuggerNonUserCode]
	public event OnSetCounterEventHandler OnSetCounter;

	[method: DebuggerNonUserCode]
	public event OnGetCounterEventHandler OnGetCounter;

	[method: DebuggerNonUserCode]
	public event OnPrintingStartEventHandler OnPrintingStart;

	[method: DebuggerNonUserCode]
	public event OnPrintingStopEventHandler OnPrintingStop;

	[method: DebuggerNonUserCode]
	public event OnMessageSelectedEventHandler OnMessageSelected;

	[method: DebuggerNonUserCode]
	public event OnUserMessageAppliedEventHandler OnUserMessageApplied;

	[method: DebuggerNonUserCode]
	public event OnGetFileListEventHandler OnGetFileList;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	public Laser()
	{
		__ENCAddToList(this);
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

	public new bool Init(int iPort, string sIp)
	{
		return base.Init(iPort, sIp);
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		_IsOnLine = false;
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		byte[] array = new byte[5] { 2, 2, 0, 0, 0 };
		byte[] bytes = BitConverter.GetBytes((short)112);
		Array.Copy(bytes, 0L, array, 2L, 2L);
		array[4] = 3;
		ActiveCommand = LASER_COMMANDS.STATUS;
		if (!SendData(array, lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			return false;
		}
		bool result = default(bool);
		return result;
	}

	public void CloseSoftConnection()
	{
		SendCommand(LASER_COMMANDS.CLOSE_SOFT_CONNECTION, new byte[0]);
	}

	public void SetCounter(int CounterIndex, int HighValue, int LowValue)
	{
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		byte[] array = new byte[12];
		byte[] bytes = BitConverter.GetBytes(CounterIndex);
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		bytes = BitConverter.GetBytes(LowValue);
		Array.Copy(bytes, 0, array, 4, bytes.Length);
		bytes = BitConverter.GetBytes(HighValue);
		Array.Copy(bytes, 0, array, 8, bytes.Length);
		SendCommand(LASER_COMMANDS.SET_COUNTER, array);
	}

	public void GetCounter(int CounterIndex)
	{
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		byte[] array = new byte[4];
		byte[] bytes = BitConverter.GetBytes(CounterIndex);
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		SendCommand(LASER_COMMANDS.GET_COUNTER, array);
	}

	public void StartPrint(PRINT_START_MODES Mode, int Copies, int Batch, string FileName)
	{
		if (FileName.IndexOf(".") >= 0)
		{
			FileName = FileName.Substring(0, FileName.IndexOf("."));
		}
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		byte[] array = new byte[20];
		byte[] bytes = BitConverter.GetBytes((ulong)Mode);
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		bytes = BitConverter.GetBytes(Copies);
		Array.Copy(bytes, 0, array, 4, bytes.Length);
		bytes = BitConverter.GetBytes(Batch);
		Array.Copy(bytes, 0, array, 8, bytes.Length);
		FileName = FileName.PadRight(8, '\0');
		bytes = Encoding.Default.GetBytes(FileName);
		Array.Copy(bytes, 0, array, 12, bytes.Length);
		SendCommand(LASER_COMMANDS.START_PRINT, array);
	}

	public void StopPrint()
	{
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		SendCommand(LASER_COMMANDS.STOP_PRINT, new byte[0]);
	}

	public void SelectMessage(string FileName)
	{
		if (FileName.IndexOf(".") >= 0)
		{
			FileName = FileName.Substring(0, FileName.IndexOf("."));
		}
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_DEFAULT;
		FileName = FileName.PadRight(8, '\0');
		SendCommand(LASER_COMMANDS.SELECT_MESSAGE, Encoding.Default.GetBytes(FileName));
	}

	public void SendUserMessage(byte UserMessageIndex, string Message)
	{
		byte[] array = new byte[checked(2 + Message.Length - 1 + 1)];
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_VERSION4;
		array[0] = 0;
		array[1] = UserMessageIndex;
		byte[] bytes = Encoding.Default.GetBytes(Message);
		Array.Copy(bytes, 0, array, 2, Message.Length);
		SendCommandV4(LASER_COMMANDS.USER_MESSAGE, array);
	}

	public void GetMessageList(FILE_TYPES FileType, byte FrameIndex)
	{
		byte[] array = new byte[2];
		CtrlProtocol.Type = PROTOCOL_TYPE.PT_VERSION4;
		array[0] = (byte)FileType;
		array[1] = FrameIndex;
		SendCommandV4(LASER_COMMANDS.GET_MSG_LIST, array);
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		byte[] bytes = Encoding.Default.GetBytes(sData);
		checked
		{
			int num2;
			if (CtrlProtocol.Type == PROTOCOL_TYPE.PT_DEFAULT)
			{
				int num = sData.Length - 1;
				num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					switch (CtrlProtocol.State)
					{
					case 0:
						if (bytes[num2] == 2)
						{
							CtrlProtocol.State = 1;
						}
						break;
					case 1:
						CtrlProtocol.Length = bytes[num2];
						if (CtrlProtocol.Length == 0)
						{
							CtrlProtocol.State = 3;
							break;
						}
						CtrlProtocol.State = 2;
						CtrlProtocol.IndexDataIn = 0;
						CtrlProtocol.BinData = new byte[CtrlProtocol.Length - 1 + 1];
						break;
					case 2:
						CtrlProtocol.BinData[CtrlProtocol.IndexDataIn] = bytes[num2];
						CtrlProtocol.IndexDataIn++;
						if (CtrlProtocol.IndexDataIn == CtrlProtocol.Length)
						{
							CtrlProtocol.State = 3;
						}
						break;
					case 3:
						if (bytes[num2] == 3)
						{
							AnswerRecv = true;
							CtrlProtocol.Command = BitConverter.ToInt16(CtrlProtocol.BinData, 0);
							CtrlProtocol.Parameters = new byte[CtrlProtocol.BinData.Length - 2 + 1];
							if (CtrlProtocol.BinData.Length > 2)
							{
								Array.Copy(CtrlProtocol.BinData, 2, CtrlProtocol.Parameters, 0, CtrlProtocol.Length - 2);
							}
							ProcessNewMessage();
						}
						CtrlProtocol.State = 0;
						break;
					}
					num2++;
				}
				return;
			}
			int num5 = sData.Length - 1;
			num2 = 0;
			while (true)
			{
				int num6 = num2;
				int num4 = num5;
				if (num6 > num4)
				{
					break;
				}
				switch (CtrlProtocol.State)
				{
				case 0:
					if (bytes[num2] == 2)
					{
						CtrlProtocol.State = 1;
					}
					break;
				case 1:
					if (bytes[num2] == 4)
					{
						CtrlProtocol.State = 2;
					}
					break;
				case 2:
					CtrlProtocol.LowCmd = bytes[num2];
					CtrlProtocol.State = 3;
					break;
				case 3:
					CtrlProtocol.HighCmd = bytes[num2];
					CtrlProtocol.State = 4;
					break;
				case 4:
					CtrlProtocol.Length = bytes[num2];
					CtrlProtocol.State = 5;
					break;
				case 5:
					CtrlProtocol.Length += bytes[num2] * 256;
					if (CtrlProtocol.Length == 0)
					{
						CtrlProtocol.State = 7;
						break;
					}
					CtrlProtocol.State = 6;
					CtrlProtocol.IndexDataIn = 0;
					CtrlProtocol.BinData = new byte[CtrlProtocol.Length - 1 + 1];
					break;
				case 6:
					CtrlProtocol.BinData[CtrlProtocol.IndexDataIn] = bytes[num2];
					CtrlProtocol.IndexDataIn++;
					if (CtrlProtocol.IndexDataIn == CtrlProtocol.Length)
					{
						CtrlProtocol.State = 7;
					}
					break;
				case 7:
					if (bytes[num2] == 3)
					{
						AnswerRecv = true;
						CtrlProtocol.Command = (short)(CtrlProtocol.HighCmd * 256 + CtrlProtocol.LowCmd);
						CtrlProtocol.Parameters = new byte[CtrlProtocol.BinData.Length + 1];
						Array.Copy(CtrlProtocol.BinData, 0, CtrlProtocol.Parameters, 0, CtrlProtocol.Length);
						ProcessNewMessage();
					}
					CtrlProtocol.State = 0;
					break;
				}
				num2++;
			}
		}
	}

	private void ProcessNewMessage()
	{
		switch ((LASER_COMMANDS)CtrlProtocol.Command)
		{
		case LASER_COMMANDS.GET_COUNTER:
			if ((CtrlProtocol.Parameters[0] == byte.MaxValue) & (CtrlProtocol.Parameters[1] == byte.MaxValue))
			{
				OnCommandError?.Invoke(this, LASER_COMMANDS.GET_COUNTER, Common.Rm.GetString("Laser1"));
			}
			else
			{
				OnGetCounter?.Invoke(this, BitConverter.ToInt32(CtrlProtocol.Parameters, 0), BitConverter.ToInt64(CtrlProtocol.Parameters, 4));
			}
			break;
		case LASER_COMMANDS.SET_COUNTER:
			if ((CtrlProtocol.Parameters[0] == byte.MaxValue) & (CtrlProtocol.Parameters[1] == byte.MaxValue))
			{
				OnCommandError?.Invoke(this, LASER_COMMANDS.SET_COUNTER, Common.Rm.GetString("Laser1"));
			}
			else
			{
				OnSetCounter?.Invoke(this);
			}
			break;
		case LASER_COMMANDS.GET_MSG_LIST:
		{
			string text = new string(Encoding.Default.GetChars(CtrlProtocol.Parameters));
			string[] files = text.Split('\n');
			OnGetFileList?.Invoke(this, files);
			break;
		}
		case LASER_COMMANDS.SELECT_MESSAGE:
			OnMessageSelected?.Invoke(this);
			break;
		case LASER_COMMANDS.START_PRINT:
			switch (BitConverter.ToInt32(CtrlProtocol.Parameters, 0))
			{
			case 65521:
				OnPrintingStart?.Invoke(this);
				break;
			case 3084:
				OnCommandError?.Invoke(this, LASER_COMMANDS.START_PRINT, Common.Rm.GetString("Laser2"));
				break;
			case 2120:
				OnCommandError?.Invoke(this, LASER_COMMANDS.START_PRINT, Common.Rm.GetString("Laser3"));
				break;
			}
			break;
		case LASER_COMMANDS.STOP_PRINT:
			OnPrintingStop?.Invoke(this);
			break;
		case LASER_COMMANDS.USER_MESSAGE:
			OnUserMessageApplied?.Invoke(this);
			break;
		case LASER_COMMANDS.STATUS:
			switch (BitConverter.ToInt16(CtrlProtocol.Parameters, 24))
			{
			case 0:
				_IsOnLine = true;
				Online?.Invoke(this);
				break;
			case 3086:
				OnError?.Invoke(this, "C0E", Common.Rm.GetString("Laser4"), Common.ERROR_TYPE.Error);
				break;
			case 65535:
				OnError?.Invoke(this, "FFFF", Common.Rm.GetString("Laser5"), Common.ERROR_TYPE.Error);
				break;
			case 2120:
			{
				int errorId = BitConverter.ToInt16(CtrlProtocol.Parameters, 26);
				Common.DATA_ERROR dataError = GetDataError(errorId);
				OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
				break;
			}
			}
			break;
		}
	}

	protected override Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 2:
			result.Desc = Common.Rm.GetString("Laser6");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 10:
			result.Desc = Common.Rm.GetString("Laser7");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 12:
			result.Desc = Common.Rm.GetString("Laser8");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 13:
			result.Desc = Common.Rm.GetString("Laser9");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 16:
			result.Desc = Common.Rm.GetString("Laser10");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 21:
			result.Desc = Common.Rm.GetString("Laser11");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 22:
			result.Desc = Common.Rm.GetString("Laser12");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 37:
			result.Desc = Common.Rm.GetString("Laser13");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 38:
			result.Desc = Common.Rm.GetString("Laser14");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 48:
			result.Desc = Common.Rm.GetString("Laser15");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 49:
			result.Desc = Common.Rm.GetString("Laser16");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 50:
			result.Desc = Common.Rm.GetString("Laser17");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 51:
			result.Desc = Common.Rm.GetString("Laser18");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 52:
			result.Desc = Common.Rm.GetString("Laser19");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 53:
			result.Desc = Common.Rm.GetString("Laser20");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 54:
			result.Desc = Common.Rm.GetString("Laser21");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 55:
			result.Desc = Common.Rm.GetString("Laser22");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 56:
			result.Desc = Common.Rm.GetString("Laser23");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 57:
			result.Desc = Common.Rm.GetString("Laser24");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 64:
			result.Desc = Common.Rm.GetString("Laser25");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 65:
			result.Desc = Common.Rm.GetString("Laser26");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 66:
			result.Desc = Common.Rm.GetString("Laser27");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 67:
			result.Desc = Common.Rm.GetString("Laser28");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("Laser30");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}

	private void SendCommand(LASER_COMMANDS Command, byte[] Parameters)
	{
		ActiveCommand = Command;
		checked
		{
			byte[] array = new byte[4 + Parameters.Length + 1 - 1 + 1];
			array[0] = 2;
			array[1] = (byte)(2L + unchecked((long)Parameters.Length));
			byte[] bytes = BitConverter.GetBytes(unchecked((short)Command));
			Array.Copy(bytes, 0, array, 2, bytes.Length);
			if (Parameters.Length > 0)
			{
				Array.Copy(Parameters, 0, array, 4, Parameters.Length);
			}
			array[array.Length - 1] = 3;
			SendData(array, 0L);
		}
	}

	private void SendCommandV4(LASER_COMMANDS Command, byte[] Parameters)
	{
		ActiveCommand = Command;
		byte[] array = new byte[checked(6 + Parameters.Length + 1 - 1 + 1)];
		array[0] = 2;
		array[1] = 4;
		byte[] bytes = BitConverter.GetBytes((short)Command);
		Array.Copy(bytes, 0, array, 2, bytes.Length);
		checked
		{
			bytes = BitConverter.GetBytes((short)Parameters.Length);
			Array.Copy(bytes, 0, array, 4, bytes.Length);
			Array.Copy(Parameters, 0, array, 6, Parameters.Length);
			array[array.Length - 1] = 3;
			SendData(array, 0L);
		}
	}
}
