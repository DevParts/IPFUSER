using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class Sauven6000plus : Inyector
{
	public enum COMMAND_TYPES : byte
	{
		MESSAGE = 0,
		PRINTER = 4
	}

	public enum SUB_COMMANDS : byte
	{
		UPDATE_FIELDS = 11,
		REGENERATE_FIELDS = 12,
		PRINT = 12
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

	public struct MSG_FIELD
	{
		public string Id;

		public string Valor;

		public int Len;
	}

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const byte PC_ADDRESS = 0;

	private COMMAND_TYPES ActiveCommand;

	private CTRL_PROT CtrlProtocol;

	private byte _UId;

	private List<MSG_FIELD> MsgFields;

	private bool HasComm;

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
	public event OnInformationEventHandler OnInformation;

	public Sauven6000plus()
	{
		__ENCAddToList(this);
		_UId = 1;
		MsgFields = new List<MSG_FIELD>();
		HasComm = false;
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
		bool result = default(bool);
		return result;
	}

	public void SelectMessage(string MsgName, bool Printing)
	{
		HasComm = false;
		checked
		{
			byte[] array = new byte[33]
			{
				12,
				1,
				Conversions.ToByte(Interaction.IIf(Printing, 1, 0)),
				0,
				0,
				1,
				(byte)DateTime.Now.Day,
				(byte)DateTime.Now.Month,
				(byte)unchecked(DateTime.Now.Year % 2000),
				(byte)DateTime.Now.Hour,
				(byte)DateTime.Now.Minute,
				(byte)DateTime.Now.Second,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			Array.Copy(Encoding.Default.GetBytes(MsgName.PadRight(21, '\0')), 0, array, 12, 21);
			SendData(Encode(COMMAND_TYPES.PRINTER, array), 0L);
		}
	}

	public void ClearFields()
	{
		MsgFields.Clear();
	}

	public bool SetAllFields()
	{
		checked
		{
			int num = MsgFields.Count - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				MSG_FIELD mSG_FIELD = MsgFields[num2];
				if (mSG_FIELD.Len > 0)
				{
					SetField(mSG_FIELD.Id, mSG_FIELD.Valor.PadRight(mSG_FIELD.Len - mSG_FIELD.Valor.Length, ' '));
				}
				else
				{
					SetField(mSG_FIELD.Id, mSG_FIELD.Valor);
				}
				num2++;
			}
			SendData(Encode(COMMAND_TYPES.MESSAGE, new byte[1] { 12 }), 0L);
			return HasComm;
		}
	}

	public void AddField(string IdField, string Value, int FieldLength)
	{
		MSG_FIELD item = default(MSG_FIELD);
		item.Id = IdField;
		item.Valor = Value;
		item.Len = FieldLength;
		MsgFields.Add(item);
	}

	private void SetField(string VariableId, string Value)
	{
		VariableId = VariableId.PadRight(11, '\0');
		checked
		{
			byte[] array = new byte[13 + Value.Length - 1 + 1];
			byte[] bytes = Encoding.Default.GetBytes(VariableId);
			array[0] = 11;
			Array.Copy(bytes, 0, array, 1, bytes.Length);
			bytes = Encoding.Default.GetBytes(Value);
			array[12] = (byte)Value.Length;
			Array.Copy(bytes, 0, array, 13, Value.Length);
			SendData(Encode(COMMAND_TYPES.MESSAGE, array), 0L);
		}
	}

	public byte[] Encode(COMMAND_TYPES Command, byte[] Data)
	{
		ActiveCommand = Command;
		byte[] array = new byte[checked(5 + Data.Length + 1 + 1 - 1 + 1)];
		array[0] = 2;
		array[1] = 0;
		array[2] = UId;
		array[3] = (byte)Command;
		checked
		{
			array[4] = (byte)Data.Length;
			if (Data.Length > 0)
			{
				Array.Copy(Data, 0, array, 5, Data.Length);
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
		HasComm = true;
		RecProtocol(array);
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		AnswerRecv = true;
		byte[] bytes = Encoding.Default.GetBytes(sData);
		HasComm = true;
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
				if (b == 2)
				{
					CtrlProtocol.State = 1;
					CtrlProtocol.Crc = 2;
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
				CtrlProtocol.State = 4;
				break;
			case 3:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.Command = bData[num2];
				CtrlProtocol.State = 4;
				break;
			case 4:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.Length = bData[num2];
				if (CtrlProtocol.Length == 0)
				{
					CtrlProtocol.State = 6;
					break;
				}
				CtrlProtocol.State = 5;
				CtrlProtocol.IndexDataIn = 0;
				CtrlProtocol.BinData = new byte[checked(CtrlProtocol.Length - 1 + 1)];
				break;
			case 5:
				CtrlProtocol.Crc ^= bData[num2];
				CtrlProtocol.BinData[CtrlProtocol.IndexDataIn] = bData[num2];
				checked
				{
					CtrlProtocol.IndexDataIn = (byte)unchecked((uint)(CtrlProtocol.IndexDataIn + 1));
					if (CtrlProtocol.IndexDataIn == CtrlProtocol.Length)
					{
						CtrlProtocol.State = 6;
					}
					break;
				}
			case 6:
				if (bData[num2] == 3)
				{
					CtrlProtocol.Crc ^= 3;
					CtrlProtocol.State = 7;
				}
				else
				{
					CtrlProtocol.State = 0;
				}
				break;
			case 7:
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
		COMMAND_TYPES command = (COMMAND_TYPES)CtrlProtocol.Command;
	}
}
