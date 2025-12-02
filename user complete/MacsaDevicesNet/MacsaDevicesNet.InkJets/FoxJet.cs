using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class FoxJet : Inyector
{
	public enum FJCOMMANDS
	{
		IPC_SELECT_LABEL = 38,
		IPC_SET_SELECTED_COUNT = 301,
		IPC_GET_SELECTED_COUNT = 302,
		IPC_SET_VARDATA_ID = 305,
		IPC_GET_STATUS = 6,
		IPC_SET_STATUS_MODE = 309,
		IPC_SET_PRINTER_MODE = 299
	}

	public struct FJ_PACKET
	{
		public FJCOMMANDS Cmd;

		public int BatchID;

		public int Len;

		public string strCmd;
	}

	public struct MSG_FIELD
	{
		public int Id;

		public string Valor;

		public int Len;
	}

	public delegate void OnGetCounterEventHandler(object Sender, long Value);

	public delegate void OnErrorEventHandler(object Sender, string HeaderValue, string GeneralValue);

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	public delegate void OnReadyToReceiveDataEventHandler(object sender);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private FJ_PACKET WFjPacket;

	private FJ_PACKET RFjPacket;

	private int ReceptionState;

	private string RecepBuff;

	private List<MSG_FIELD> MsgFields;

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
	public event OnGetCounterEventHandler OnGetCounter;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveDataEventHandler OnReadyToReceiveData;

	public FoxJet()
	{
		__ENCAddToList(this);
		ReceptionState = 0;
		MsgFields = new List<MSG_FIELD>();
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

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		RecepBuff += SerialComm.ReadExisting();
		while (RecepBuff.Length >= 12)
		{
			DecodeFJHeader();
			if (RFjPacket.strCmd.Length == RFjPacket.Len)
			{
				ProcessState();
			}
		}
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		AnswerRecv = true;
		RecepBuff += sData;
		while (RecepBuff.Length >= 12)
		{
			DecodeFJHeader();
			if (RFjPacket.strCmd.Length == RFjPacket.Len)
			{
				ProcessState();
			}
		}
	}

	public bool RequestToSendData()
	{
		IsDataSendRequested = true;
		return GetStatus(WAIT_TYPE.Thread, 1000L);
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		_IsOnLine = false;
		WFjPacket.Cmd = FJCOMMANDS.IPC_GET_STATUS;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = ";";
		WFjPacket.Len = 1;
		byte[] bData = EncodePacket();
		if (SendData(bData, 2000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
		bool result = default(bool);
		return result;
	}

	public void SelectLabel(string LabelName)
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_SELECT_LABEL;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = "LABELID=\"" + LabelName + "\";";
		WFjPacket.Len = WFjPacket.strCmd.Length;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void SetCounter(int Value)
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_SET_SELECTED_COUNT;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = "CountSel=" + Value + ";";
		WFjPacket.Len = WFjPacket.strCmd.Length;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void GetCounter()
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_GET_SELECTED_COUNT;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = ";";
		WFjPacket.Len = 1;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void SetVariable(int VariableId, string Value)
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_SET_VARDATA_ID;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = "ID=" + VariableId + ";DATA=\"" + Value + "\";";
		WFjPacket.Len = WFjPacket.strCmd.Length;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void SetStatusMode(bool Mode)
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_SET_STATUS_MODE;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = "STATUS_MODE=" + Conversions.ToString(Interaction.IIf(Mode, "ON", "OFF")) + ";";
		WFjPacket.Len = WFjPacket.strCmd.Length;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void SetPrinterMode(bool Mode)
	{
		WFjPacket.Cmd = FJCOMMANDS.IPC_SET_PRINTER_MODE;
		WFjPacket.BatchID = 1;
		WFjPacket.strCmd = "PRINT_MODE=" + Conversions.ToString(Interaction.IIf(Mode, "RESUME", "STOP")) + ";";
		WFjPacket.Len = WFjPacket.strCmd.Length;
		byte[] bData = EncodePacket();
		if (!SendData(bData, 0L))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"));
		}
	}

	public void SetAllVariables()
	{
		checked
		{
			int num = MsgFields.Count - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 <= num4)
				{
					MSG_FIELD mSG_FIELD = MsgFields[num2];
					if (mSG_FIELD.Len > 0)
					{
						SetVariable(mSG_FIELD.Id, mSG_FIELD.Valor.PadRight(mSG_FIELD.Len - mSG_FIELD.Valor.Length, ' '));
					}
					else
					{
						SetVariable(mSG_FIELD.Id, mSG_FIELD.Valor);
					}
					num2++;
					continue;
				}
				break;
			}
		}
	}

	public void ClearFields()
	{
		MsgFields.Clear();
	}

	public void AddField(int IdField, string Value, int FieldLength)
	{
		MSG_FIELD item = default(MSG_FIELD);
		item.Id = IdField;
		item.Valor = Value;
		item.Len = FieldLength;
		MsgFields.Add(item);
	}

	public byte[] EncodePacket()
	{
		byte[] array = new byte[checked(12 + WFjPacket.strCmd.Length - 1 + 1)];
		byte[] bytes = BitConverter.GetBytes((int)WFjPacket.Cmd);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		bytes = BitConverter.GetBytes(WFjPacket.BatchID);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, array, 4, bytes.Length);
		bytes = BitConverter.GetBytes(WFjPacket.Len);
		Array.Reverse(bytes);
		Array.Copy(bytes, 0, array, 8, bytes.Length);
		bytes = Encoding.Default.GetBytes(WFjPacket.strCmd);
		Array.Copy(bytes, 0, array, 12, bytes.Length);
		return array;
	}

	public void DecodeFJHeader()
	{
		byte[] bytes = Encoding.Default.GetBytes(RecepBuff.Substring(0, 4));
		Array.Reverse(bytes);
		RFjPacket.Cmd = (FJCOMMANDS)BitConverter.ToInt32(bytes, 0);
		bytes = Encoding.Default.GetBytes(RecepBuff.Substring(4, 4));
		Array.Reverse(bytes);
		RFjPacket.BatchID = BitConverter.ToInt32(bytes, 0);
		bytes = Encoding.Default.GetBytes(RecepBuff.Substring(8, 4));
		Array.Reverse(bytes);
		RFjPacket.Len = BitConverter.ToInt32(bytes, 0);
		RFjPacket.strCmd = RecepBuff.Substring(12, RFjPacket.Len);
		RecepBuff = checked(RecepBuff.Substring(12 + RFjPacket.Len, RecepBuff.Length - (12 + RFjPacket.Len)));
	}

	public string GetTokenValue(string Token)
	{
		string result = "";
		int num = RFjPacket.strCmd.IndexOf(Token);
		checked
		{
			if (num >= 0)
			{
				num += Token.Length + 1;
				int num2 = RFjPacket.strCmd.IndexOf(';', num);
				if (num2 >= 0)
				{
					result = RFjPacket.strCmd.Substring(num, num2 - num);
				}
			}
			return result;
		}
	}

	public void ProcessState()
	{
		string headerValue = "";
		string generalValue = "";
		bool flag = false;
		string tokenValue = GetTokenValue("CountSel");
		if (Operators.CompareString(tokenValue, "", TextCompare: false) != 0)
		{
			OnGetCounter?.Invoke(this, Conversions.ToLong(tokenValue));
		}
		tokenValue = GetTokenValue("HeadStatus");
		bool flag2 = default(bool);
		if (Operators.CompareString(tokenValue, "", TextCompare: false) != 0)
		{
			flag = true;
			if (Operators.CompareString(tokenValue, "Ready", TextCompare: false) != 0)
			{
				headerValue = tokenValue;
				flag2 = true;
			}
		}
		tokenValue = GetTokenValue("InkLevel");
		if (Operators.CompareString(tokenValue, "", TextCompare: false) != 0)
		{
			flag = true;
			if (Operators.CompareString(tokenValue, "LOW", TextCompare: false) == 0)
			{
				generalValue = Common.Rm.GetString("Foxjet1");
				flag2 = true;
			}
		}
		if (!flag)
		{
			return;
		}
		if (flag2)
		{
			OnError?.Invoke(this, headerValue, generalValue);
			return;
		}
		_IsOnLine = true;
		OnLine?.Invoke(this);
		if (IsDataSendRequested)
		{
			OnReadyToReceiveData?.Invoke(this);
			IsDataSendRequested = false;
		}
	}
}
