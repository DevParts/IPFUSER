using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;

namespace MacsaDevicesNet.InkJets;

public class Sauven : Inyector
{
	public enum SAUVEN_COMMANDS : byte
	{
		NONE = 0,
		POLL = 80,
		PRINT = 69,
		SELECTION = 78,
		CREATEMSG = 77,
		EXTRAMSG = 88,
		A_POLL = 112,
		A_SELECTION = 110,
		A_CREATEMSG = 109,
		A_EXTRAMSG = 120
	}

	public enum PRINTER_MODES : byte
	{
		PRINT_OFF,
		PRINT_ON
	}

	public struct CTRL_PROT
	{
		public int State;

		public byte Length;

		public byte IndexDataIn;

		public byte[] BinData;

		public byte Crc;

		public byte UId;

		public byte Command;

		public byte[] Data;
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnMessageSelectedEventHandler(object sender);

	public delegate void OnCommandAcceptedEventHandler(object sender, SAUVEN_COMMANDS command);

	public delegate void OnCommandRefusedEventHandler(object sender, SAUVEN_COMMANDS command);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private SAUVEN_COMMANDS ActiveCommand;

	private CTRL_PROT CtrlProtocol;

	private byte _UId;

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

	public byte UId
	{
		get
		{
			return _UId;
		}
		set
		{
			_UId = value;
		}
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnMessageSelectedEventHandler OnMessageSelected;

	[method: DebuggerNonUserCode]
	public event OnCommandAcceptedEventHandler OnCommandAccepted;

	[method: DebuggerNonUserCode]
	public event OnCommandRefusedEventHandler OnCommandRefused;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	public Sauven()
	{
		__ENCAddToList(this);
		_UId = 1;
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
		FlushComm();
		_IsOnLine = false;
		byte[] bData = Encode(SAUVEN_COMMANDS.POLL, new byte[0]);
		if (!SendData(bData, lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			return false;
		}
		bool result = default(bool);
		return result;
	}

	public void StartPrint()
	{
		byte[] bData = Encode(SAUVEN_COMMANDS.PRINT, new byte[1] { 1 });
		SendData(bData, 0L);
	}

	public void StopPrint()
	{
		byte[] bData = Encode(SAUVEN_COMMANDS.PRINT, new byte[1] { 0 });
		SendData(bData, 0L);
	}

	public void SelectMessage(string PrintMsg, string ViewMsg)
	{
		if (PrintMsg.Length == 1)
		{
			PrintMsg = PrintMsg.PadLeft(2, '0');
		}
		if (ViewMsg.Length == 1)
		{
			ViewMsg = ViewMsg.PadLeft(2, '0');
		}
		byte[] bytes = Encoding.Default.GetBytes(ViewMsg + "," + PrintMsg);
		byte[] bData = Encode(SAUVEN_COMMANDS.SELECTION, bytes);
		SendData(bData, 0L);
	}

	public void SendMessage(SauvenMsg Msj)
	{
		Common.Wait(800L);
		byte[] bData = Encode(SAUVEN_COMMANDS.CREATEMSG, Msj.GetBinMessage());
		SendData(bData, 0L);
		Common.Wait(800L);
		bData = Encode(SAUVEN_COMMANDS.EXTRAMSG, Msj.GetExtraBinMessage());
		SendData(bData, 0L);
	}

	public byte[] Encode(SAUVEN_COMMANDS Command, byte[] Data)
	{
		ActiveCommand = Command;
		byte[] array = new byte[checked(4 + Data.Length + 1 + 1 - 1 + 1)];
		array[0] = 2;
		array[1] = UId;
		array[2] = (byte)Command;
		checked
		{
			array[3] = (byte)Data.Length;
			if (Data.Length > 0)
			{
				Array.Copy(Data, 0, array, 4, Data.Length);
			}
			array[array.Length - 2] = 3;
			byte b = 0;
			int num = array.Length - 2;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				b = unchecked((byte)(b ^ array[num2]));
				num2++;
			}
			array[array.Length - 1] = b;
			return array;
		}
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		AnswerRecv = true;
		byte[] array = new byte[checked(SerialComm.BytesToRead - 1 + 1)];
		SerialComm.Read(array, 0, SerialComm.BytesToRead);
		RecProtocol(array);
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		AnswerRecv = true;
		byte[] bytes = Encoding.Default.GetBytes(sData);
		RecProtocol(bytes);
	}

	private void RecProtocol(byte[] bData)
	{
		int num = checked(bData.Length - 1);
		int num2 = 0;
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
			{
				byte b = bData[num2];
				switch (b)
				{
				case 2:
					CtrlProtocol.State = 1;
					CtrlProtocol.Crc = 2;
					break;
				default:
					if (0 == 0)
					{
						if (b == 6)
						{
							OnCommandAccepted?.Invoke(this, ActiveCommand);
						}
						break;
					}
					goto case 21;
				case 21:
				case 24:
					OnCommandRefused?.Invoke(this, ActiveCommand);
					break;
				}
				break;
			}
			case 1:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.UId = bData[num2];
				CtrlProtocol.State = 2;
				break;
			case 2:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.Command = bData[num2];
				CtrlProtocol.State = 3;
				break;
			case 3:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.Length = bData[num2];
				if (CtrlProtocol.Length == 0)
				{
					CtrlProtocol.State = 5;
					break;
				}
				CtrlProtocol.State = 4;
				CtrlProtocol.IndexDataIn = 0;
				CtrlProtocol.BinData = new byte[checked(CtrlProtocol.Length - 1 + 1)];
				break;
			case 4:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.BinData[CtrlProtocol.IndexDataIn] = bData[num2];
				checked
				{
					CtrlProtocol.IndexDataIn = (byte)unchecked((uint)(CtrlProtocol.IndexDataIn + 1));
					if (CtrlProtocol.IndexDataIn == CtrlProtocol.Length)
					{
						CtrlProtocol.State = 5;
					}
					break;
				}
			case 5:
				if (bData[num2] == 3)
				{
					CtrlProtocol.Crc ^= 3;
					CtrlProtocol.State = 6;
				}
				else
				{
					CtrlProtocol.State = 0;
				}
				break;
			case 6:
				if (((CtrlProtocol.Crc ^ bData[num2]) == 0) & (_UId == CtrlProtocol.UId))
				{
					ProcessNewMessage();
				}
				CtrlProtocol.State = 0;
				break;
			}
			num2 = checked(num2 + 1);
		}
	}

	private void ProcessNewMessage()
	{
		switch (CtrlProtocol.Command)
		{
		case 112:
			_IsOnLine = true;
			Online?.Invoke(this);
			break;
		case 110:
			OnMessageSelected?.Invoke(this);
			break;
		case 111:
			break;
		}
	}
}
