using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class LinxTTbin : Etiquetadora
{
	public struct FIELD_ITEM
	{
		public string Key;

		public string Value;
	}

	public enum GLOBAL_COMMANDS : ushort
	{
		GET_STATE = 1,
		GET_COUNTERS,
		SELECT_JOB,
		UPDATE_JOB,
		DO_COMMAND,
		SEND_FILE,
		SET_DATETIME
	}

	public enum HLP_INFOTAGS : ushort
	{
		STATUS = 1,
		COUNTERS
	}

	public enum HLP_COMMANDS : ushort
	{
		INFO_REQUEST = 20481,
		FILE_TRANSMIT = 20484,
		SELECT_JOB = 20487,
		JOB_DATA_UPDATE = 20488,
		DO_COMMAND = 20490,
		SET_DATETIME = 20494,
		INFO_RESPONSE = 20482
	}

	public enum ACTIONS : ushort
	{
		DO_PRINT = 1,
		ON_LINE = 3,
		OFF_LINE = 4,
		START_UP = 5,
		SHUTDOWN = 6
	}

	public enum FILE_TYPES : ushort
	{
		CIFF = 1,
		BMP = 3
	}

	public enum LLP_FLAGS : byte
	{
		SQS = 1,
		FIN = 2,
		ACK = 4,
		NAK = 8,
		CS = 0x10,
		ADR = 0x20,
		ASY = 0x40,
		RES = 0x80
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnJobSelectedEventHandler(object sender, string JobName);

	public delegate void OnJobUpdatedEventHandler(object sender);

	public delegate void OnPrinterModeChangedEventHandler(object sender, ACTIONS PrinterMode);

	public delegate void OnGotCountersEventHandler(object sender, uint JobCounter, uint TotalCounter);

	public delegate void OnFileSentEventHandler(object sender, string FileName);

	public delegate void OnLabelSentEventHandler(object sender, string LabelName);

	public delegate void OnPrinterSetDateTimeEventHandler(object Sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private List<FIELD_ITEM> Fields;

	private string _LabelsPath;

	private GLOBAL_COMMANDS ActiveCommand;

	private byte[] DataIn;

	private byte[] DataOut;

	private const byte LLP_START = 165;

	private const byte LLP_END = 228;

	private const int LLP_MAX_PACKET_SIZE = 2000;

	private ushort LLPTx_TransactionId;

	private int LLPRx_ProtocolState;

	private byte LLPRx_PacketFlags;

	private int LLPRx_PacketSize;

	private ushort LLPRx_TransactionId;

	private ushort LLPRx_SequenceId;

	private int LLPRx_Address;

	private int LLPRx_ChkHeader;

	private byte[] LLPRx_Payload;

	private byte LLPRx_ChkPayload;

	private int LLPRx_PayloadLen;

	private bool LLPRx_HasPayload;

	private int LLPTx_TotalPackets;

	private int LLPTx_CurrentPacket;

	private bool LLPRx_PacketReceived;

	private bool LLPRx_TimeOut;

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

	public string LabelsPath
	{
		get
		{
			return _LabelsPath;
		}
		set
		{
			_LabelsPath = value;
		}
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveLabelEventHandler OnReadyToReceiveLabel;

	[method: DebuggerNonUserCode]
	public event OnJobSelectedEventHandler OnJobSelected;

	[method: DebuggerNonUserCode]
	public event OnJobUpdatedEventHandler OnJobUpdated;

	[method: DebuggerNonUserCode]
	public event OnPrinterModeChangedEventHandler OnPrinterModeChanged;

	[method: DebuggerNonUserCode]
	public event OnGotCountersEventHandler OnGotCounters;

	[method: DebuggerNonUserCode]
	public event OnFileSentEventHandler OnFileSent;

	[method: DebuggerNonUserCode]
	public event OnLabelSentEventHandler OnLabelSent;

	[method: DebuggerNonUserCode]
	public event OnPrinterSetDateTimeEventHandler OnPrinterSetDateTime;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	public LinxTTbin()
	{
		__ENCAddToList(this);
		Fields = new List<FIELD_ITEM>();
		LLPRx_HasPayload = false;
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

	public byte LLP_ChkHeader(byte[] Packet)
	{
		int num = 0;
		int num2 = default(int);
		checked
		{
			int num3;
			int num4;
			do
			{
				num2 += Packet[num];
				num++;
				num3 = num;
				num4 = 8;
			}
			while (num3 <= num4);
		}
		num2 %= 256;
		return (byte)(~checked((byte)num2));
	}

	public bool LLP_SendData()
	{
		int num = 0;
		DataIn = new byte[0];
		if (LLPTx_TransactionId == 32)
		{
			LLPTx_TransactionId = 0;
		}
		checked
		{
			LLPTx_TransactionId = (ushort)unchecked((uint)(LLPTx_TransactionId + 1));
			LLPTx_TotalPackets = unchecked(DataOut.Length / 2000) + 1;
			LLPTx_CurrentPacket = 1;
			LLPRx_HasPayload = false;
			while (unchecked(LLPTx_CurrentPacket <= LLPTx_TotalPackets && num < 5))
			{
				LLPRx_PacketReceived = false;
				LLP_SendPacket(LLPTx_CurrentPacket);
				long num2 = MyProject.Computer.Clock.TickCount;
				while (MyProject.Computer.Clock.TickCount - num2 < 2000 && !LLPRx_PacketReceived)
				{
				}
				if (LLPRx_PacketReceived)
				{
					if ((LLPRx_PacketFlags & 4) == 4)
					{
						LLPTx_CurrentPacket++;
						num = 0;
					}
					else if ((LLPRx_PacketFlags & 4) == 8)
					{
						num++;
					}
				}
				else
				{
					num++;
				}
			}
			if (num == 5)
			{
				return false;
			}
			while (true)
			{
				if (!(LLPRx_PacketReceived & LLPRx_HasPayload))
				{
					LLPRx_PacketReceived = false;
					long num2 = MyProject.Computer.Clock.TickCount;
					while (MyProject.Computer.Clock.TickCount - num2 < 4000 && !LLPRx_PacketReceived)
					{
					}
				}
				if (LLPRx_PacketReceived)
				{
					int destinationIndex = DataIn.Length;
					DataIn = (byte[])Utils.CopyArray(DataIn, new byte[DataIn.Length + LLPRx_PayloadLen - 1 + 1]);
					Array.Copy(LLPRx_Payload, 0, DataIn, destinationIndex, LLPRx_PayloadLen);
					LLP_SendAck();
					if ((LLPRx_PacketFlags & 2) == 2)
					{
						return true;
					}
					continue;
				}
				break;
			}
			return false;
		}
	}

	public void LLP_SendAck()
	{
		byte[] array = new byte[10];
		array[0] = 165;
		array[1] = LLPRx_PacketFlags;
		array[1] = (byte)(array[1] | 4);
		byte[] bytes = BitConverter.GetBytes(checked((ushort)array.Length));
		Array.Copy(bytes, 0, array, 2, 2);
		bytes = BitConverter.GetBytes(LLPRx_TransactionId);
		Array.Copy(bytes, 0, array, 4, 2);
		bytes = BitConverter.GetBytes(LLPRx_SequenceId);
		Array.Copy(bytes, 0, array, 6, 2);
		array[8] = 228;
		array[9] = LLP_ChkHeader(array);
		SendData(array, 0L);
	}

	public void LLP_SendPacket(int IdPacket)
	{
		int num;
		byte[] array;
		checked
		{
			num = DataOut.Length - (IdPacket - 1) * 2000;
			if (num > 2000)
			{
				num = 2000;
			}
			array = new byte[9 + num + 1];
			array[0] = 165;
		}
		if (IdPacket == 1)
		{
			array[1] = (byte)(array[1] | 1);
		}
		if (IdPacket == LLPTx_TotalPackets)
		{
			array[1] = (byte)(array[1] | 2);
		}
		checked
		{
			byte[] bytes = BitConverter.GetBytes((ushort)array.Length);
			Array.Copy(bytes, 0, array, 2, 2);
			bytes = BitConverter.GetBytes(LLPTx_TransactionId);
			Array.Copy(bytes, 0, array, 4, 2);
			bytes = BitConverter.GetBytes((short)(IdPacket - 1));
			Array.Copy(bytes, 0, array, 6, 2);
			array[8] = 228;
			array[9] = LLP_ChkHeader(array);
			Array.Copy(DataOut, (IdPacket - 1) * 2000, array, 10, num);
			SendData(array, 0L);
		}
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		while (true)
		{
			int bytesToRead = SerialComm.BytesToRead;
			if (bytesToRead > 0)
			{
				byte[] array = new byte[checked(bytesToRead - 1 + 1)];
				SerialComm.Read(array, 0, bytesToRead);
				ProcessLowLevelRx(array);
				continue;
			}
			break;
		}
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		byte[] bytes = Encoding.Default.GetBytes(sData);
		ProcessLowLevelRx(bytes);
	}

	public void ProcessLowLevelRx(byte[] Datos)
	{
		checked
		{
			int num = Datos.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				byte b = Datos[num2];
				if (ConnectionType == CONNECTION_TYPE.Serial)
				{
					Thread.Sleep(1);
				}
				switch (LLPRx_ProtocolState)
				{
				case 0:
					if (b == 165)
					{
						LLPRx_ChkHeader = b;
						LLPRx_ProtocolState = 1;
					}
					break;
				case 1:
					LLPRx_PacketFlags = b;
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 2;
					break;
				case 2:
					LLPRx_PacketSize = b;
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 3;
					break;
				case 3:
					LLPRx_PacketSize += unchecked((ushort)(b << 8));
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 4;
					break;
				case 4:
					LLPRx_TransactionId = b;
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 5;
					break;
				case 5:
					LLPRx_TransactionId = (ushort)unchecked((uint)(LLPRx_TransactionId + (ushort)(b << 8)));
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 6;
					break;
				case 6:
					LLPRx_SequenceId = b;
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 7;
					break;
				case 7:
					LLPRx_SequenceId = (ushort)unchecked((uint)(LLPRx_SequenceId + (ushort)(b << 8)));
					LLPRx_ChkHeader += b;
					LLPRx_ProtocolState = 8;
					break;
				case 8:
					if (b == 228)
					{
						LLPRx_ChkHeader += b;
						LLPRx_ProtocolState = 9;
					}
					else
					{
						LLPRx_ProtocolState = 0;
					}
					break;
				case 9:
					unchecked
					{
						LLPRx_ChkHeader %= 256;
						if (b == (byte)(~checked((byte)LLPRx_ChkHeader)))
						{
							checked
							{
								LLPRx_PacketSize -= 10;
								if (LLPRx_PacketSize > 0)
								{
									LLPRx_PayloadLen = 0;
									LLPRx_Payload = new byte[LLPRx_PacketSize + 1];
									LLPRx_ProtocolState = 10;
								}
								else
								{
									AnswerRecv = true;
									LLPRx_HasPayload = false;
									LLPRx_PacketReceived = true;
									LLPRx_ProtocolState = 0;
								}
							}
						}
						else
						{
							LLPRx_ProtocolState = 0;
						}
						break;
					}
				case 10:
					LLPRx_Payload[LLPRx_PayloadLen] = b;
					LLPRx_PayloadLen++;
					if (LLPRx_PayloadLen == LLPRx_PacketSize)
					{
						AnswerRecv = true;
						LLPRx_HasPayload = true;
						LLPRx_PacketReceived = true;
						LLPRx_ProtocolState = 0;
					}
					break;
				}
				num2++;
			}
		}
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		DataOut = new byte[10];
		FlushComm();
		_IsOnLine = false;
		ActiveCommand = GLOBAL_COMMANDS.GET_STATE;
		byte[] bytes = BitConverter.GetBytes((ushort)20481);
		Array.Copy(bytes, 0, DataOut, 0, 2);
		bytes = BitConverter.GetBytes(9);
		Array.Copy(bytes, 0, DataOut, 2, 2);
		bytes = BitConverter.GetBytes((ushort)1);
		Array.Copy(bytes, 0, DataOut, 6, 2);
		bytes = BitConverter.GetBytes((ushort)1);
		Array.Copy(bytes, 0, DataOut, 8, 2);
		if (LLP_SendData())
		{
			DecodeDataIn();
		}
		else
		{
			OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
		}
		bool result = default(bool);
		return result;
	}

	public bool GetCounters()
	{
		DataOut = new byte[10];
		ActiveCommand = GLOBAL_COMMANDS.GET_COUNTERS;
		byte[] bytes = BitConverter.GetBytes((ushort)20481);
		Array.Copy(bytes, 0, DataOut, 0, 2);
		bytes = BitConverter.GetBytes(9);
		Array.Copy(bytes, 0, DataOut, 2, 2);
		bytes = BitConverter.GetBytes((ushort)1);
		Array.Copy(bytes, 0, DataOut, 6, 2);
		bytes = BitConverter.GetBytes((ushort)2);
		Array.Copy(bytes, 0, DataOut, 8, 2);
		if (LLP_SendData())
		{
			DecodeDataIn();
		}
		else
		{
			OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
		}
		bool result = default(bool);
		return result;
	}

	public bool PrintMode(ACTIONS Command)
	{
		DataOut = new byte[8];
		ActiveCommand = GLOBAL_COMMANDS.DO_COMMAND;
		byte[] bytes = BitConverter.GetBytes((ushort)20490);
		Array.Copy(bytes, 0, DataOut, 0, 2);
		bytes = BitConverter.GetBytes(8);
		Array.Copy(bytes, 0, DataOut, 2, 2);
		bytes = BitConverter.GetBytes((ushort)Command);
		Array.Copy(bytes, 0, DataOut, 6, 2);
		if (LLP_SendData())
		{
			DecodeDataIn();
			OnPrinterModeChanged?.Invoke(this, Command);
		}
		else
		{
			OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
		}
		bool result = default(bool);
		return result;
	}

	public bool SetDateTime()
	{
		DataOut = new byte[18];
		ActiveCommand = GLOBAL_COMMANDS.SET_DATETIME;
		byte[] bytes = BitConverter.GetBytes((ushort)20494);
		Array.Copy(bytes, 0, DataOut, 0, 2);
		bytes = BitConverter.GetBytes(18);
		Array.Copy(bytes, 0, DataOut, 2, 2);
		checked
		{
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Year);
			Array.Copy(bytes, 0, DataOut, 6, 2);
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Month);
			Array.Copy(bytes, 0, DataOut, 8, 2);
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Day);
			Array.Copy(bytes, 0, DataOut, 10, 2);
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Hour);
			Array.Copy(bytes, 0, DataOut, 12, 2);
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Minute);
			Array.Copy(bytes, 0, DataOut, 14, 2);
			bytes = BitConverter.GetBytes((ushort)DateTime.Now.Second);
			Array.Copy(bytes, 0, DataOut, 16, 2);
			if (LLP_SendData())
			{
				DecodeDataIn();
				OnPrinterSetDateTime?.Invoke(this);
				return true;
			}
			OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
			return false;
		}
	}

	public bool SelectJob(string JobName)
	{
		DataOut = new byte[1025];
		string text = JobName + "\0";
		if (text.Length % 2 == 1)
		{
			text += "\0";
		}
		ActiveCommand = GLOBAL_COMMANDS.SELECT_JOB;
		byte[] bytes = BitConverter.GetBytes((ushort)20487);
		Array.Copy(bytes, 0, DataOut, 0, 2);
		bytes = BitConverter.GetBytes(0);
		Array.Copy(bytes, 0, DataOut, 2, 2);
		checked
		{
			bytes = BitConverter.GetBytes((ushort)text.Length);
			Array.Copy(bytes, 0, DataOut, 6, 2);
			bytes = Encoding.Default.GetBytes(text);
			Array.Copy(bytes, 0, DataOut, 8, bytes.Length);
			int num = 8 + bytes.Length;
			bytes = BitConverter.GetBytes(0u);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 4;
			bytes = BitConverter.GetBytes(0u);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 4;
			bytes = BitConverter.GetBytes((ushort)0);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 2;
			DataOut = (byte[])Utils.CopyArray(DataOut, new byte[num - 1 + 1]);
			bytes = BitConverter.GetBytes((ushort)DataOut.Length);
			Array.Copy(bytes, 0, DataOut, 2, bytes.Length);
			if (LLP_SendData())
			{
				DecodeDataIn();
				OnJobSelected?.Invoke(this, JobName);
			}
			else
			{
				OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
			}
			bool result = default(bool);
			return result;
		}
	}

	public bool SendFile(string FileName, FILE_TYPES Type = FILE_TYPES.CIFF)
	{
		string text = Path.GetFileName(FileName) + "\0";
		if (text.Length % 2 == 1)
		{
			text += "\0";
		}
		checked
		{
			if (Type == FILE_TYPES.CIFF)
			{
				FileStream input = new FileStream(FileName, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(input, Encoding.Default);
				int num = (int)new FileInfo(FileName).Length;
				byte[] array = binaryReader.ReadBytes(num);
				binaryReader.Close();
				int i = default(int);
				for (; array[i] != 60; i++)
				{
				}
				File.Delete(FileName);
				FileStream output = new FileStream(FileName, FileMode.Create);
				BinaryWriter binaryWriter = new BinaryWriter(output, Encoding.Default);
				binaryWriter.Write(array, i, num - i);
				binaryWriter.Close();
			}
			FileStream stream = new FileStream(FileName, FileMode.Open);
			StreamReader streamReader = new StreamReader(stream, Encoding.Default);
			string text2 = streamReader.ReadToEnd();
			streamReader.Close();
			int length = text2.Length;
			if (unchecked(text2.Length % 2) == 1)
			{
				text2 += "\0";
			}
			byte[] bytes = Encoding.Default.GetBytes(text2);
			DataOut = new byte[100 + bytes.Length + 1];
			ActiveCommand = GLOBAL_COMMANDS.SEND_FILE;
			byte[] bytes2 = BitConverter.GetBytes((ushort)20484);
			Array.Copy(bytes2, 0, DataOut, 0, 2);
			bytes2 = BitConverter.GetBytes(0);
			Array.Copy(bytes2, 0, DataOut, 2, 4);
			bytes2 = BitConverter.GetBytes(unchecked((int)Type));
			Array.Copy(bytes2, 0, DataOut, 6, 2);
			bytes2 = BitConverter.GetBytes(length);
			Array.Copy(bytes2, 0, DataOut, 8, 4);
			bytes2 = BitConverter.GetBytes((ushort)text.Length);
			Array.Copy(bytes2, 0, DataOut, 12, 2);
			bytes2 = Encoding.Default.GetBytes(text);
			Array.Copy(bytes2, 0, DataOut, 14, bytes2.Length);
			int num2 = 14 + bytes2.Length;
			Array.Copy(bytes, 0, DataOut, num2, bytes.Length);
			num2 += bytes.Length;
			DataOut = (byte[])Utils.CopyArray(DataOut, new byte[num2 - 1 + 1]);
			bytes2 = BitConverter.GetBytes((ushort)DataOut.Length);
			Array.Copy(bytes2, 0, DataOut, 2, bytes2.Length);
			if (LLP_SendData())
			{
				DecodeDataIn();
				OnFileSent?.Invoke(this, FileName);
			}
			else
			{
				OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
			}
			bool result = default(bool);
			return result;
		}
	}

	public bool SendRemoteFields()
	{
		string text = BuildRemoteData();
		if (Operators.CompareString(text, "", TextCompare: false) == 0)
		{
			return true;
		}
		text += "\0";
		if (text.Length % 2 == 1)
		{
			text += "\0";
		}
		checked
		{
			DataOut = new byte[text.Length + 1024 + 1];
			ActiveCommand = GLOBAL_COMMANDS.UPDATE_JOB;
			byte[] bytes = BitConverter.GetBytes((ushort)20488);
			Array.Copy(bytes, 0, DataOut, 0, 2);
			bytes = BitConverter.GetBytes(0);
			Array.Copy(bytes, 0, DataOut, 2, 2);
			int num = 6;
			bytes = BitConverter.GetBytes(0u);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 4;
			bytes = BitConverter.GetBytes(0u);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 4;
			bytes = BitConverter.GetBytes((ushort)text.Length);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += 2;
			bytes = Encoding.Default.GetBytes(text);
			Array.Copy(bytes, 0, DataOut, num, bytes.Length);
			num += text.Length;
			DataOut = (byte[])Utils.CopyArray(DataOut, new byte[num - 1 + 1]);
			bytes = BitConverter.GetBytes((ushort)DataOut.Length);
			Array.Copy(bytes, 0, DataOut, 2, bytes.Length);
			if (LLP_SendData())
			{
				DecodeDataIn();
				OnJobUpdated?.Invoke(this);
			}
			else
			{
				OnError?.Invoke(this, "", "Fallo de Comunicación", Common.ERROR_TYPE.Error);
			}
			bool result = default(bool);
			return result;
		}
	}

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		return default(Common.DATA_ERROR);
	}

	public override bool SendLabel(string sPathLabel)
	{
		List<string> list = new List<string>();
		FileStream stream = new FileStream(sPathLabel, FileMode.Open);
		checked
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				string text = streamReader.ReadToEnd();
				int num;
				for (num = text.IndexOf("\\Graphics\\"); num >= 0; num = text.IndexOf("\\Graphics\\", num))
				{
					num += 10;
					string text2 = "";
					for (; Operators.CompareString(Conversions.ToString(text[num]), "]", TextCompare: false) != 0; num++)
					{
						text2 += Conversions.ToString(text[num]);
					}
					if (!list.Contains(text2))
					{
						list.Add(text2);
					}
				}
			}
			int num2 = list.Count - 1;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				if (!File.Exists(_LabelsPath + "\\graphics\\" + list[num3]))
				{
					return false;
				}
				num3++;
			}
			int num6 = list.Count - 1;
			int num7 = 0;
			while (true)
			{
				int num8 = num7;
				int num5 = num6;
				if (num8 > num5)
				{
					break;
				}
				SendFile(_LabelsPath + "\\graphics\\" + list[num7], FILE_TYPES.BMP);
				num7++;
			}
			SendFile(sPathLabel);
			OnLabelSent?.Invoke(this, Path.GetFileName(sPathLabel));
			bool result = default(bool);
			return result;
		}
	}

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return sBuff;
	}

	public void DecodeDataIn()
	{
		switch ((ushort)ActiveCommand)
		{
		case 1:
			checked
			{
				if ((DataIn[12] & 4) == 4)
				{
					ushort num = BitConverter.ToUInt16(DataIn, 22);
					string text = "";
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
						if (DataIn[24 + num3] > 0)
						{
							text += Conversions.ToString(Strings.Chr(DataIn[24 + num3]));
						}
						num3++;
					}
					OnError?.Invoke(this, "", text, Common.ERROR_TYPE.Error);
				}
				else if ((DataIn[12] & 8) == 8)
				{
					ushort num6 = BitConverter.ToUInt16(DataIn, 22);
					string text2 = "";
					int num7 = num6 - 1;
					int num8 = 0;
					while (true)
					{
						int num9 = num8;
						int num5 = num7;
						if (num9 > num5)
						{
							break;
						}
						if (DataIn[24 + num8] > 0)
						{
							text2 += Conversions.ToString(Strings.Chr(DataIn[24 + num8]));
						}
						num8++;
					}
					OnError?.Invoke(this, "", text2, Common.ERROR_TYPE.Warning);
				}
				else if ((DataIn[12] & 2) == 0)
				{
					OnError?.Invoke(this, "", "Off-Line", Common.ERROR_TYPE.Error);
				}
				else
				{
					_IsOnLine = true;
					OnLine?.Invoke(this);
					if (IsLabelRequested)
					{
						OnReadyToReceiveLabel?.Invoke(this);
						IsLabelRequested = false;
					}
				}
				break;
			}
		case 2:
		{
			uint jobCounter = BitConverter.ToUInt32(DataIn, 12);
			uint totalCounter = BitConverter.ToUInt32(DataIn, 16);
			OnGotCounters?.Invoke(this, jobCounter, totalCounter);
			break;
		}
		}
	}

	public void ClearRemoteFields()
	{
		Fields.Clear();
	}

	public void AddRemoteField(string Key, string Value)
	{
		FIELD_ITEM item = default(FIELD_ITEM);
		item.Key = Key;
		item.Value = Value;
		Fields.Add(item);
	}

	private string BuildRemoteData()
	{
		string result = "";
		checked
		{
			if (Fields.Count > 0)
			{
				result = "<ImageData>";
				int num = Fields.Count - 1;
				int num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					result = result + "<Field Name=\"" + Fields[num2].Key + "\">" + Fields[num2].Value + "</Field>";
					num2++;
				}
				result += "</ImageData>";
			}
			return result;
		}
	}
}
