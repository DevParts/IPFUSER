using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class Linx : Inyector
{
	public struct MSGFIELD
	{
		public byte[] Val;

		public int Length;
	}

	public struct LINXSTATE
	{
		public byte PrinterStatus;

		public byte CommandStatus;

		public byte JetState;

		public byte PrintState;

		public long ErrorMask;
	}

	public enum LINX_TYPE
	{
		lStd = 1,
		l4900
	}

	public enum LINX_PRINT_MODE
	{
		pmSingle = 1,
		pmContinuous
	}

	public enum LINX_RESP_TYPE
	{
		respUnknown,
		respAck,
		respNack
	}

	public enum LINX_KEYBOARD_STATE
	{
		stsKbDispEnabled,
		stsKbDisDispEnb,
		stsKbDispDisabled
	}

	public enum LINX_REMOTE_BUFFER_DIVISOR
	{
		div1 = 1,
		div2 = 2,
		div4 = 4,
		div8 = 8,
		div16 = 0x10,
		div32 = 0x20,
		div64 = 0x40,
		div128 = 0x80
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnBufferFullEventHandler(object sender);

	public delegate void OnlineExtendedEventHandler(object sender, LINXSTATE State);

	public delegate void OnReadyToReceiveDataEventHandler(object sender, LINXSTATE State);

	public delegate void OnGetCounterEventHandler(object Sender, long Contador);

	public delegate void OnGetDirectoryEventHandler(object Sender, string[] Messages);

	public delegate void OnGetMessageEventHandler(object Sender, byte[] MessageData);

	public delegate void OnSetMessageEventHandler(object Sender);

	public delegate void OnMessageDeletedEventHandler(object sender);

	public delegate void OnGetCurrentMessageEventHandler(object Sender, string MessageName);

	public delegate void OnSystemInfoEventHandler(object Sender, int Model, string SoftVersion);

	public delegate void OnInformationEventHandler(object sender, string Message);

	public delegate void OnDateTimeSetEventHandler(object sender);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private int ProtocolState;

	private const string FRAME_START = "\u001b\u0002";

	private const string FRAME_END = "\u001b\u0003";

	private const string FRAME_ACK = "\u001b\u0006";

	private const string FRAME_NAK = "\u001b\u0015";

	private const string CMD_STATUS = "\u0014";

	private const string CMD_STARTJET = "\u000f";

	private const string CMD_STOPJET = "\u0010";

	private const string CMD_STARTPRINT = "\u0011";

	private const string CMD_STOPPRINT = "\u0012";

	private const string CMD_SETPRINTMODE = " ";

	private const string CMD_SENDREMOTEFIELDS = "\u001d";

	private const string CMD_SET_KEYBOARD_DISPLAY_STATE = "-";

	private const string CMD_SET_PHOTOCELL = "%";

	private const string CMD_SET_INTERPRINT_DELAY = "b";

	private const string CMD_SET_INTERPRINT_DELAY_NOTCONTINUOUS = "\u0005";

	private const string CMD_SET_COUNTER = "\a";

	private const string CMD_GET_COUNTER = "\b";

	private const string CMD_SET_REVERSE_MESSAGE = "\t";

	private const string CMD_SET_INVERT_MESSAGE = "\v";

	private const string CMD_GET_DIRECTORY = "a";

	private const string CMD_DOWNLOAD_MESSAGE = "\u0019";

	private const string CMD_UPLOAD_MESSAGE = "\u001a";

	private const string CMD_ERASE_MESSAGE = "\u001b";

	private const string CMD_GET_CURRENT_MESSAGE = "\u001f";

	private const string CMD_GET_SYSTEM_INFO = "3";

	private const string CMD_SET_PRINTWIDTH = "\u0003";

	private const string CMD_SET_DATETIME = "\r";

	private LINX_RESP_TYPE Resp;

	private LINXSTATE State;

	private Collection MsgFields;

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
	public event OnBufferFullEventHandler OnBufferFull;

	[method: DebuggerNonUserCode]
	public event OnlineExtendedEventHandler OnlineExtended;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveDataEventHandler OnReadyToReceiveData;

	[method: DebuggerNonUserCode]
	public event OnGetCounterEventHandler OnGetCounter;

	[method: DebuggerNonUserCode]
	public event OnGetDirectoryEventHandler OnGetDirectory;

	[method: DebuggerNonUserCode]
	public event OnGetMessageEventHandler OnGetMessage;

	[method: DebuggerNonUserCode]
	public event OnSetMessageEventHandler OnSetMessage;

	[method: DebuggerNonUserCode]
	public event OnMessageDeletedEventHandler OnMessageDeleted;

	[method: DebuggerNonUserCode]
	public event OnGetCurrentMessageEventHandler OnGetCurrentMessage;

	[method: DebuggerNonUserCode]
	public event OnSystemInfoEventHandler OnSystemInfo;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[method: DebuggerNonUserCode]
	public event OnDateTimeSetEventHandler OnDateTimeSet;

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

	private string BuildFrame(string sData)
	{
		return "\u001b\u0002" + sData + "\u001b\u0003";
	}

	private byte[] BuildFrame(byte[] sData)
	{
		checked
		{
			byte[] array = new byte[2 + sData.Length + 2 + 1];
			array[0] = 27;
			array[1] = 2;
			Array.Copy(sData, 0, array, 2, sData.Length);
			array[2 + sData.Length] = 27;
			array[2 + sData.Length + 1] = 3;
			return array;
		}
	}

	private void RecProtocol(byte[] Datos)
	{
		int num = Datos.Length;
		checked
		{
			switch (ProtocolState)
			{
			case 0:
			{
				int num7 = Datos.Length - 2;
				int num8 = 0;
				while (true)
				{
					int num9 = num8;
					int num6 = num7;
					if (num9 > num6)
					{
						break;
					}
					if (((Datos[num8] == 27) & (Datos[num8 + 1] == 6)) | ((Datos[num8] == 27) & (Datos[num8 + 1] == 21)))
					{
						bBuffIn = new byte[num - num8 - 1 + 1];
						Array.Copy(Datos, num8, bBuffIn, 0, num - num8);
						ProtocolState = 1;
						int num10 = bBuffIn.Length - 2;
						int num11 = 0;
						while (true)
						{
							int num12 = num11;
							num6 = num10;
							if (num12 > num6)
							{
								break;
							}
							if ((bBuffIn[num11] == 27) & (bBuffIn[num11 + 1] == 3))
							{
								AnswerRecv = true;
								ProcessInputData(bBuffIn);
								ProtocolState = 0;
								break;
							}
							num11++;
						}
					}
					num8++;
				}
				break;
			}
			case 1:
			{
				int num2 = bBuffIn.GetUpperBound(0) + 1;
				bBuffIn = (byte[])Utils.CopyArray(bBuffIn, new byte[num2 + num - 1 + 1]);
				Array.Copy(Datos, 0, bBuffIn, num2, num);
				int num3 = bBuffIn.Length - 2;
				int num4 = 0;
				while (true)
				{
					int num5 = num4;
					int num6 = num3;
					if (num5 <= num6)
					{
						if ((bBuffIn[num4] == 27) & (bBuffIn[num4 + 1] == 3))
						{
							AnswerRecv = true;
							ProcessInputData(bBuffIn);
							ProtocolState = 0;
							break;
						}
						num4++;
						continue;
					}
					break;
				}
				break;
			}
			}
		}
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		int bytesToRead = SerialComm.BytesToRead;
		byte[] array = new byte[checked(bytesToRead - 1 + 1)];
		SerialComm.Read(array, 0, bytesToRead);
		RecProtocol(array);
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		int length = sData.Length;
		byte[] bytes = Encoding.Default.GetBytes(sData);
		RecProtocol(bytes);
	}

	private void ProcessInputData(byte[] bIn)
	{
		byte[] array = new byte[4];
		checked
		{
			byte[] array2 = new byte[bIn.Length - 1 + 1];
			int num = 0;
			int num2 = bIn.Length - 2;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				array2[num] = bIn[num3];
				if ((bIn[num3] == 27) & (bIn[num3 + 1] == 27))
				{
					num3++;
				}
				num++;
				num3++;
			}
			array2[num] = bIn[bIn.Length - 1];
			array2 = (byte[])Utils.CopyArray(array2, new byte[num + 1]);
			bIn = array2;
			try
			{
				byte b = bIn[1];
				if (!unchecked(b == 6 || b == 21))
				{
					return;
				}
				State.PrinterStatus = bIn[2];
				State.CommandStatus = bIn[3];
				switch (bIn[4])
				{
				case 20:
				{
					if (State.PrinterStatus != 0)
					{
						int errorId = 31 + State.PrinterStatus;
						Common.DATA_ERROR dataError = GetDataError(errorId);
						OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
						break;
					}
					if (State.CommandStatus != 0)
					{
						if ((State.CommandStatus == 66) | (State.CommandStatus == 67))
						{
							OnBufferFull?.Invoke(this);
							break;
						}
						Common.DATA_ERROR commandError = GetCommandError(State.CommandStatus);
						OnError?.Invoke(this, commandError.Id, commandError.Desc, commandError.Type);
						break;
					}
					num = 0;
					int num14;
					int num5;
					do
					{
						State.ErrorMask = bIn[7 + num];
						if (State.ErrorMask != 0)
						{
							num3 = 0;
							int num13;
							do
							{
								if ((State.ErrorMask & (long)Math.Round(Math.Pow(2.0, num3))) != 0)
								{
									int errorId = num3 + num * 8;
									Common.DATA_ERROR dataError2 = GetDataError(errorId);
									OnError?.Invoke(this, dataError2.Id, dataError2.Desc, dataError2.Type);
									return;
								}
								num3++;
								num13 = num3;
								num5 = 7;
							}
							while (num13 <= num5);
						}
						num++;
						num14 = num;
						num5 = 3;
					}
					while (num14 <= num5);
					State.JetState = bIn[5];
					State.PrintState = bIn[6];
					State.ErrorMask = 0L;
					_IsOnLine = true;
					Online?.Invoke(this);
					OnlineExtended?.Invoke(this, State);
					if (IsDataSendRequested)
					{
						OnReadyToReceiveData?.Invoke(this, State);
						IsDataSendRequested = false;
					}
					break;
				}
				case 32:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(51).Id, GetDataError(51).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 29:
					if (b == 21)
					{
						Resp = LINX_RESP_TYPE.respNack;
					}
					else
					{
						Resp = LINX_RESP_TYPE.respAck;
					}
					break;
				case 45:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(52).Id, GetDataError(52).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 37:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(53).Id, GetDataError(53).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 5:
				case 98:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(54).Id, GetDataError(54).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 7:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(55).Id, GetDataError(55).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 9:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(58).Id, GetDataError(58).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 11:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(63).Id, GetDataError(63).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 8:
				{
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(56).Id, GetDataError(56).Desc, Common.ERROR_TYPE.Error);
						break;
					}
					Array.Copy(bIn, 5, array, 0, 4);
					long contador = BitConverter.ToUInt32(array, 0);
					OnGetCounter?.Invoke(this, contador);
					break;
				}
				case 97:
				{
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(57).Id, GetDataError(57).Desc, Common.ERROR_TYPE.Error);
						break;
					}
					int num6 = (int)Math.Round((double)(bIn.Length - 10) / 41.0);
					string[] array3 = new string[num6 - 1 + 1];
					int num7 = 17;
					int num8 = num6 - 1;
					num3 = 0;
					while (true)
					{
						int num9 = num3;
						int num5 = num8;
						if (num9 > num5)
						{
							break;
						}
						num = 0;
						while (bIn[num7 + num] != 0)
						{
							string[] array4 = array3;
							int num10 = num3;
							array4[num10] += Conversions.ToString(Strings.Chr(bIn[num7 + num]));
							num++;
							int num11 = num;
							num5 = 15;
							if (num11 > num5)
							{
								break;
							}
						}
						num7 += 41;
						num3++;
					}
					OnGetDirectory?.Invoke(this, array3);
					break;
				}
				case 31:
				{
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(59).Id, GetDataError(59).Desc, Common.ERROR_TYPE.Error);
						break;
					}
					string text = "";
					num3 = 0;
					int num12;
					int num5;
					do
					{
						text += Conversions.ToString(Strings.Chr(bIn[5 + num3]));
						num3++;
						num12 = num3;
						num5 = 15;
					}
					while (num12 <= num5);
					OnGetCurrentMessage?.Invoke(this, text);
					break;
				}
				case 51:
				{
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(60).Id, GetDataError(60).Desc, Common.ERROR_TYPE.Error);
						break;
					}
					string text2 = "";
					num3 = 0;
					int num15;
					int num5;
					do
					{
						text2 += Conversions.ToString(Strings.Chr(bIn[27 + num3]));
						num3++;
						num15 = num3;
						num5 = 15;
					}
					while (num15 <= num5);
					OnSystemInfo?.Invoke(this, bIn[5], text2);
					break;
				}
				case 26:
				{
					byte[] array5 = new byte[bIn.Length - 8 - 1 + 1];
					Array.Copy(bIn, 6, array5, 0, bIn.Length - 8);
					OnGetMessage?.Invoke(this, array5);
					break;
				}
				case 25:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(57).Id, GetDataError(57).Desc, Common.ERROR_TYPE.Error);
					}
					else
					{
						OnSetMessage?.Invoke(this);
					}
					break;
				case 27:
					OnMessageDeleted?.Invoke(this);
					break;
				case 3:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(61).Id, GetDataError(61).Desc, Common.ERROR_TYPE.Error);
					}
					break;
				case 13:
					if (b == 21)
					{
						OnError?.Invoke(this, GetDataError(62).Id, GetDataError(62).Desc, Common.ERROR_TYPE.Error);
					}
					else
					{
						OnDateTimeSet?.Invoke(this);
					}
					break;
				case 4:
				case 6:
				case 10:
				case 12:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 22:
				case 23:
				case 24:
				case 28:
				case 30:
				case 33:
				case 34:
				case 35:
				case 36:
				case 38:
				case 39:
				case 40:
				case 41:
				case 42:
				case 43:
				case 44:
				case 46:
				case 47:
				case 48:
				case 49:
				case 50:
				case 52:
				case 53:
				case 54:
				case 55:
				case 56:
				case 57:
				case 58:
				case 59:
				case 60:
				case 61:
				case 62:
				case 63:
				case 64:
				case 65:
				case 66:
				case 67:
				case 68:
				case 69:
				case 70:
				case 71:
				case 72:
				case 73:
				case 74:
				case 75:
				case 76:
				case 77:
				case 78:
				case 79:
				case 80:
				case 81:
				case 82:
				case 83:
				case 84:
				case 85:
				case 86:
				case 87:
				case 88:
				case 89:
				case 90:
				case 91:
				case 92:
				case 93:
				case 94:
				case 95:
				case 96:
					break;
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog("Linx: Error al procesar los datos recibidos.", TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	public bool Init(string sPort, int iBauds, LINX_TYPE linxType)
	{
		if (linxType == LINX_TYPE.l4900)
		{
			return Init(sPort, iBauds, Parity.None, 8, StopBits.One, Handshake.RequestToSend, bDtrEnabled: true);
		}
		return Init(sPort, iBauds);
	}

	public new bool Init(int iPort, string sIp)
	{
		return base.Init(iPort, sIp);
	}

	protected override Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 0:
			result.Desc = Common.Rm.GetString("Linx1");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 1:
			result.Desc = Common.Rm.GetString("Linx2");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 2:
			result.Desc = Common.Rm.GetString("Linx3");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 3:
			result.Desc = Common.Rm.GetString("Linx4");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 4:
			result.Desc = Common.Rm.GetString("Linx5");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 5:
			result.Desc = Common.Rm.GetString("Linx6");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 6:
			result.Desc = Common.Rm.GetString("Linx7");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 7:
			result.Desc = Common.Rm.GetString("Linx8");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 8:
			result.Desc = Common.Rm.GetString("Linx9");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 9:
			result.Desc = Common.Rm.GetString("Linx10");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 10:
			result.Desc = Common.Rm.GetString("Linx11");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 11:
			result.Desc = Common.Rm.GetString("Linx12");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 12:
			result.Desc = Common.Rm.GetString("Linx13");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 13:
			result.Desc = Common.Rm.GetString("Linx14");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 14:
			result.Desc = Common.Rm.GetString("Linx15");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 15:
			result.Desc = Common.Rm.GetString("Linx16");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 16:
			result.Desc = Common.Rm.GetString("Linx17");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 17:
			result.Desc = Common.Rm.GetString("Linx18");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 18:
			result.Desc = Common.Rm.GetString("Linx19");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 19:
			result.Desc = Common.Rm.GetString("Linx20");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 20:
			result.Desc = Common.Rm.GetString("Linx21");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 21:
			result.Desc = Common.Rm.GetString("Linx22");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 22:
			result.Desc = Common.Rm.GetString("Linx23");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 23:
			result.Desc = Common.Rm.GetString("Linx24");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 24:
			result.Desc = Common.Rm.GetString("Linx25");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 25:
			result.Desc = Common.Rm.GetString("Linx26");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 26:
			result.Desc = Common.Rm.GetString("Linx27");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 27:
			result.Desc = Common.Rm.GetString("Linx28");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 28:
			result.Desc = Common.Rm.GetString("Linx29");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 29:
			result.Desc = Common.Rm.GetString("Linx30");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 30:
			result.Desc = Common.Rm.GetString("Linx31");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 31:
			result.Desc = Common.Rm.GetString("Linx32");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 32:
			result.Desc = Common.Rm.GetString("Linx33");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 33:
			result.Desc = Common.Rm.GetString("Linx34");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 34:
			result.Desc = Common.Rm.GetString("Linx35");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 35:
			result.Desc = Common.Rm.GetString("Linx36");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 36:
			result.Desc = Common.Rm.GetString("Linx37");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 37:
			result.Desc = Common.Rm.GetString("Linx38");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 38:
			result.Desc = Common.Rm.GetString("Linx39");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 39:
			result.Desc = Common.Rm.GetString("Linx40");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 40:
			result.Desc = Common.Rm.GetString("Linx41");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 41:
			result.Desc = Common.Rm.GetString("Linx42");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 42:
			result.Desc = Common.Rm.GetString("Linx43");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 43:
			result.Desc = Common.Rm.GetString("Linx44");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 44:
			result.Desc = Common.Rm.GetString("Linx45");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 45:
			result.Desc = Common.Rm.GetString("Linx46");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 46:
			result.Desc = Common.Rm.GetString("Linx47");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 47:
			result.Desc = Common.Rm.GetString("Linx48");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 48:
			result.Desc = Common.Rm.GetString("Linx49");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 50:
			result.Desc = Common.Rm.GetString("Linx50");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 51:
			result.Desc = Common.Rm.GetString("Linx51");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 52:
			result.Desc = Common.Rm.GetString("Linx52");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 53:
			result.Desc = Common.Rm.GetString("Linx53");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 54:
			result.Desc = Common.Rm.GetString("Linx54");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 55:
			result.Desc = Common.Rm.GetString("Linx55");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 56:
			result.Desc = Common.Rm.GetString("Linx56");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 57:
			result.Desc = Common.Rm.GetString("Linx57");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 58:
			result.Desc = Common.Rm.GetString("Linx58");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 59:
			result.Desc = Common.Rm.GetString("Linx59");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 60:
			result.Desc = Common.Rm.GetString("Linx60");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 61:
			result.Desc = Common.Rm.GetString("Linx61");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 62:
			result.Desc = Common.Rm.GetString("Linx84");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 63:
			result.Desc = Common.Rm.GetString("Linx85");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("Linx83") + " " + ErrorId;
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}

	public Common.DATA_ERROR GetCommandError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 1:
			result.Desc = Common.Rm.GetString("Linx62");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 2:
			result.Desc = Common.Rm.GetString("Linx63");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 3:
			result.Desc = Common.Rm.GetString("Linx64");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 4:
			result.Desc = Common.Rm.GetString("Linx65");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 8:
			result.Desc = Common.Rm.GetString("Linx66");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 17:
			result.Desc = Common.Rm.GetString("Linx67");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 18:
			result.Desc = Common.Rm.GetString("Linx68");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 19:
			result.Desc = Common.Rm.GetString("Linx69");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 23:
			result.Desc = Common.Rm.GetString("Linx70");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 28:
			result.Desc = Common.Rm.GetString("Linx71");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 36:
			result.Desc = Common.Rm.GetString("Linx72");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 37:
			result.Desc = Common.Rm.GetString("Linx73");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 57:
			result.Desc = Common.Rm.GetString("Linx74");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 60:
			result.Desc = Common.Rm.GetString("Linx75");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 62:
			result.Desc = Common.Rm.GetString("Linx76");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 63:
			result.Desc = Common.Rm.GetString("Linx77");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 64:
			result.Desc = Common.Rm.GetString("Linx78");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 65:
			result.Desc = Common.Rm.GetString("Linx79");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 66:
			result.Desc = Common.Rm.GetString("Linx80");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 67:
			result.Desc = Common.Rm.GetString("Linx81");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 81:
			result.Desc = Common.Rm.GetString("Linx82");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		default:
			result.Desc = Common.Rm.GetString("Linx84") + " " + ErrorId;
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		}
		return result;
	}

	public bool RequestToSendData()
	{
		IsDataSendRequested = true;
		return GetStatus(WAIT_TYPE.Thread, 1000L);
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		bool result = true;
		_IsOnLine = false;
		FlushComm();
		if (!SendData(BuildFrame("\u0014"), lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public bool StartFastPrintMode(LINX_REMOTE_BUFFER_DIVISOR oBuffDiv, bool bResetBuffer)
	{
		bool flag = false;
		bool result = default(bool);
		if (SetPrintState(bMode: false))
		{
			Common.Wait(500L);
			if (SetPrintMode(LINX_PRINT_MODE.pmSingle, oBuffDiv, bResetBuffer))
			{
				Common.Wait(500L);
				result = SetPrintState(bMode: true);
				Common.Wait(500L);
			}
		}
		return result;
	}

	public bool SetInjectorMode(bool bMode)
	{
		if (bMode)
		{
			SendData(BuildFrame("\u000f"), 0L);
		}
		else
		{
			SetPrintState(bMode: false);
			Common.Wait(400L);
			SendData(BuildFrame("\u0010"), 0L);
		}
		bool result = default(bool);
		return result;
	}

	public bool SetPrintState(bool bMode)
	{
		if (bMode)
		{
			return SendData(BuildFrame("\u0011"), 0L);
		}
		return SendData(BuildFrame("\u0012"), 0L);
	}

	public bool SetPrintMode(LINX_PRINT_MODE pmMode, LINX_REMOTE_BUFFER_DIVISOR oBuffDiv, bool bResetBuffer)
	{
		string text = " " + Conversions.ToString(Strings.Chr((int)pmMode));
		text += "\0";
		text += "\0";
		text = (bResetBuffer ? (text + "\u0001") : (text + "\0"));
		text += Conversions.ToString(Strings.Chr((int)oBuffDiv));
		text += "\0";
		text += "\0";
		text += "\0";
		text += "\0";
		return SendData(BuildFrame(text), 0L);
	}

	public bool SetKeyboardDisplayState(LINX_KEYBOARD_STATE stsKbDisp)
	{
		string sData = "-" + Conversions.ToString(Strings.Chr((int)stsKbDisp));
		return SendData(BuildFrame(sData), 0L);
	}

	public bool EraseMessage(string sMsgName)
	{
		string sTxt;
		if (Operators.CompareString(sMsgName, "", TextCompare: false) == 0)
		{
			sTxt = "\u001b\u0002\u001b\u001b\0" + sMsgName + "\0\u001b\u0003";
		}
		else
		{
			sTxt = "\u001b\u0002\u001b\u001b\u0001" + sMsgName;
			int num = Strings.Len(sMsgName);
			while (true)
			{
				int num2 = num;
				int num3 = 16;
				if (num2 > num3)
				{
					break;
				}
				sTxt += "\0";
				num = checked(num + 1);
			}
			sTxt += "\u001b\u0003";
		}
		return SendData(sTxt, 0L);
	}

	public bool SendRemoteFields()
	{
		bool flag = false;
		int num = 0;
		checked
		{
			MSGFIELD mSGFIELD = default(MSGFIELD);
			foreach (object msgField in MsgFields)
			{
				num += ((msgField != null) ? ((MSGFIELD)msgField) : mSGFIELD).Length;
			}
			byte[] array = new byte[7 + num - 1 + 1];
			array[0] = 27;
			array[1] = 2;
			array[2] = 29;
			byte[] bytes = BitConverter.GetBytes((short)num);
			array[3] = bytes[0];
			array[4] = bytes[1];
			int num2 = 5;
			foreach (object msgField2 in MsgFields)
			{
				MSGFIELD mSGFIELD2 = ((msgField2 != null) ? ((MSGFIELD)msgField2) : mSGFIELD);
				int num3 = mSGFIELD2.Length - 1;
				int num4 = 0;
				while (true)
				{
					int num5 = num4;
					int num6 = num3;
					if (num5 > num6)
					{
						break;
					}
					array[num2 + num4] = mSGFIELD2.Val[num4];
					num4++;
				}
				num2 += mSGFIELD2.Length;
			}
			array[num2] = 27;
			array[num2 + 1] = 3;
			Resp = LINX_RESP_TYPE.respUnknown;
			return SendData(array, 0L);
		}
	}

	public LINX_RESP_TYPE GetLastAnswerState()
	{
		return Resp;
	}

	public bool SetMsgInExec(string sMsgName)
	{
		byte[] array = new byte[23];
		SetPrintState(bMode: false);
		Common.Wait(400L);
		array[0] = 27;
		array[1] = 2;
		array[2] = 30;
		int num = Strings.Len(sMsgName);
		int num2 = 1;
		checked
		{
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				array[2 + num2] = (byte)Strings.Asc(Strings.Mid(sMsgName, num2, 1));
				num2++;
			}
			int num5 = 2 + Strings.Len(sMsgName);
			num2 = Strings.Len(sMsgName) + 1;
			while (true)
			{
				int num6 = num2;
				int num4 = 16;
				if (num6 > num4)
				{
					break;
				}
				num5++;
				array[num5] = 0;
				num2++;
			}
			array[num5 + 1] = 0;
			array[num5 + 2] = 0;
			array[num5 + 3] = 27;
			array[num5 + 4] = 3;
			return SendData(array, 0L);
		}
	}

	public bool SetPhotocellMode(bool bContinuous)
	{
		byte[] array = new byte[2] { 37, 0 };
		if (bContinuous)
		{
			array[1] = 0;
			return SendData(BuildFrame(array), 0L);
		}
		array[1] = 1;
		return SendData(BuildFrame(array), 0L);
	}

	public bool SetInterprintDelay(bool Continuous, uint value)
	{
		byte[] array = new byte[3];
		if (Continuous)
		{
			array[0] = 98;
		}
		else
		{
			array[0] = 5;
		}
		checked
		{
			array[1] = (byte)(unchecked((long)value) & 0xFFL);
			array[2] = (byte)Math.Round((double)(unchecked((long)value) & 0xFF00L) / 256.0);
			return SendData(BuildFrame(array), 0L);
		}
	}

	public bool SetCounter(long value)
	{
		byte[] array = new byte[5];
		byte[] bytes = BitConverter.GetBytes(value);
		array[0] = 7;
		array[1] = bytes[0];
		array[2] = bytes[1];
		array[3] = bytes[2];
		array[4] = bytes[3];
		return SendData(BuildFrame(array), 0L);
	}

	public bool GetCounter()
	{
		return SendData(BuildFrame("\b"), 0L);
	}

	public bool GetDirectory()
	{
		return SendData(BuildFrame("aM"), 0L);
	}

	public bool GetMessage(string MessageName)
	{
		if (MessageName.Length > 15)
		{
			return false;
		}
		return SendData(BuildFrame("\u001a\u0001" + MessageName.PadRight(16, '\0')), 0L);
	}

	public bool SetReverse(bool On)
	{
		byte[] array = new byte[9] { 9, 0, 0, 0, 0, 0, 0, 0, 0 };
		if (On)
		{
			array[1] = 0;
			array[2] = 0;
			array[3] = 1;
			array[4] = 0;
			array[5] = 0;
			array[6] = 0;
			array[7] = 0;
			array[8] = 0;
		}
		else
		{
			array[1] = 1;
			array[2] = 0;
			array[3] = 0;
			array[4] = 0;
			array[5] = 0;
			array[6] = 0;
			array[7] = 0;
			array[8] = 0;
		}
		return SendData(BuildFrame(array), 0L);
	}

	public bool SetInvert(bool On)
	{
		byte[] array = new byte[9] { 11, 0, 0, 0, 0, 0, 0, 0, 0 };
		if (On)
		{
			array[1] = 0;
			array[2] = 0;
			array[3] = 1;
			array[4] = 0;
			array[5] = 0;
			array[6] = 0;
			array[7] = 0;
			array[8] = 0;
		}
		else
		{
			array[1] = 1;
			array[2] = 0;
			array[3] = 0;
			array[4] = 0;
			array[5] = 0;
			array[6] = 0;
			array[7] = 0;
			array[8] = 0;
		}
		return SendData(BuildFrame(array), 0L);
	}

	public bool SetMessage(byte[] MessageData)
	{
		int num = BitConverter.ToUInt16(MessageData, 0);
		checked
		{
			if (num != MessageData.Length)
			{
				ushort value = (ushort)MessageData.Length;
				Array.Copy(BitConverter.GetBytes(value), 0, MessageData, 0, 2);
			}
			int num2 = MessageData.Length - 1;
			int num3 = 0;
			int num6 = default(int);
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				if (MessageData[num3] == 27)
				{
					num6++;
				}
				num3++;
			}
			byte[] array = new byte[2 + num6 + MessageData.Length - 1 + 1];
			array[0] = 25;
			array[1] = 1;
			int num7 = 2;
			int num8 = MessageData.Length - 1;
			num3 = 0;
			while (true)
			{
				int num9 = num3;
				int num5 = num8;
				if (num9 > num5)
				{
					break;
				}
				array[num7] = MessageData[num3];
				if (array[num7] == 27)
				{
					num7++;
					array[num7] = 27;
				}
				num7++;
				num3++;
			}
			return SendData(BuildFrame(array), 0L);
		}
	}

	public bool DeleteMessage(string MessageName)
	{
		if (MessageName.Length > 15)
		{
			return false;
		}
		return SendData(BuildFrame("\u001b\u001b\u0001" + MessageName.PadRight(16, '\0')), 0L);
	}

	public bool GetCurrentMessage()
	{
		return SendData(BuildFrame("\u001f"), 0L);
	}

	public bool GetSystemInfo()
	{
		return SendData(BuildFrame("3"), 0L);
	}

	public bool SetPrintWidth(ushort value)
	{
		byte[] array = new byte[3];
		byte[] bytes = BitConverter.GetBytes(value);
		array[0] = 3;
		array[1] = bytes[0];
		array[2] = bytes[1];
		return SendData(BuildFrame(array), 0L);
	}

	public bool SetDataAndTime(bool CalculateWeekDay)
	{
		checked
		{
			byte[] array = new byte[7]
			{
				13,
				(byte)DateTime.Now.Minute,
				(byte)DateTime.Now.Hour,
				0,
				0,
				0,
				0
			};
			if (CalculateWeekDay)
			{
				array[3] = (byte)(DateTime.Now.DayOfWeek + 1);
			}
			else
			{
				array[3] = 0;
			}
			array[4] = (byte)DateTime.Now.Day;
			array[5] = (byte)DateTime.Now.Month;
			array[6] = (byte)unchecked(DateTime.Now.Year % 100);
			return SendData(BuildFrame(array), 0L);
		}
	}

	public void ClearFields()
	{
		MsgFields.Clear();
	}

	public void AddField(string sVal, int iLong)
	{
		checked
		{
			MSGFIELD mSGFIELD = default(MSGFIELD);
			mSGFIELD.Val = new byte[iLong - 1 + 1];
			int num = iLong - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				if (num2 < Strings.Len(sVal))
				{
					mSGFIELD.Val[num2] = (byte)Strings.Asc(Strings.Mid(sVal, num2 + 1, 1));
				}
				else
				{
					mSGFIELD.Val[num2] = 32;
				}
				num2++;
			}
			mSGFIELD.Length = iLong;
			MsgFields.Add(mSGFIELD);
		}
	}

	public Linx()
	{
		__ENCAddToList(this);
		MsgFields = new Collection();
	}
}
