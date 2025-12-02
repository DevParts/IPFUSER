using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.PLCS;

public class HostLink : MacsaDevice
{
	public enum COMMANDS : byte
	{
		ReadAreaDM,
		WriteAreaIR,
		WriteAreaHR,
		Asynchron,
		WriteAreaDM
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnAnswerEventHandler(object sender, string Command, string Data);

	public delegate void OnFramingErrorEventHandler(object sender);

	public delegate void OnCommandErrorEventHandler(object sender, string Command);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private byte _ID;

	private int ProtocolState;

	private string ProtocolData;

	private string ActiveCommand;

	private string[] StrCommands;

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

	public byte Id
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
		}
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnAnswerEventHandler OnAnswer;

	[method: DebuggerNonUserCode]
	public event OnFramingErrorEventHandler OnFramingError;

	[method: DebuggerNonUserCode]
	public event OnCommandErrorEventHandler OnCommandError;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	public HostLink()
	{
		__ENCAddToList(this);
		StrCommands = new string[5] { "RD", "WR", "WH", "EX", "WD" };
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

	public void ReadDM(int InitChannel, int Qty)
	{
		if (!SendData(Encode(StrCommands[0] + InitChannel.ToString("D4") + Qty.ToString("D4")), 1000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void WriteIR(int Channel, int Value)
	{
		if (!SendData(Encode(StrCommands[1] + Channel.ToString("D4") + Conversion.Hex(Value).PadLeft(4, '0')), 1000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void WriteHR(int Channel, int Value)
	{
		if (!SendData(Encode(StrCommands[2] + Channel.ToString("D4") + Conversion.Hex(Value).PadLeft(4, '0')), 1000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void WriteDM(int Channel, int Value)
	{
		if (!SendData(Encode(StrCommands[4] + Channel.ToString("D4") + Conversion.Hex(Value).PadLeft(4, '0')), 1000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		sBuffIn += SerialComm.ReadExisting();
		ProcessInputData();
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		ProcessInputData();
	}

	public void ProcessInputData()
	{
		checked
		{
			int num = sBuffIn.Length - 1;
			int num2 = 0;
			byte b = default(byte);
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				switch (ProtocolState)
				{
				case 0:
					if (Operators.CompareString(sBuffIn.Substring(num2, 1), "@", TextCompare: false) == 0)
					{
						ProtocolData = "@";
						ProtocolState = 1;
					}
					break;
				case 1:
					if (Operators.CompareString(sBuffIn.Substring(num2, 1), "*", TextCompare: false) == 0)
					{
						ProtocolState = 2;
					}
					else
					{
						ProtocolData += sBuffIn.Substring(num2, 1);
					}
					break;
				case 2:
					if (Operators.CompareString(sBuffIn.Substring(num2, 1), "\r", TextCompare: false) == 0)
					{
						AnswerRecv = true;
						int num5 = ProtocolData.Length - 3;
						int num6 = 0;
						while (true)
						{
							int num7 = num6;
							num4 = num5;
							if (num7 > num4)
							{
								break;
							}
							unchecked
							{
								b = (byte)(b ^ checked((byte)Strings.Asc(ProtocolData.Substring(num6, 1))));
							}
							num6++;
						}
						if (Operators.CompareString(Conversion.Hex(b).PadLeft(2, '0'), ProtocolData.Substring(ProtocolData.Length - 2, 2), TextCompare: false) == 0)
						{
							string left = ProtocolData.Substring(3, 2);
							if (Operators.CompareString(left, "FE", TextCompare: false) == 0)
							{
								OnFramingError?.Invoke(this);
							}
							else if (Operators.CompareString(left, "ER", TextCompare: false) == 0)
							{
								OnCommandError?.Invoke(this, ActiveCommand);
							}
							else
							{
								OnAnswer?.Invoke(this, ProtocolData.Substring(3, 2), ProtocolData.Substring(5, ProtocolData.Length - 7));
							}
						}
					}
					ProtocolState = 0;
					break;
				}
				num2++;
			}
			sBuffIn = "";
		}
	}

	public string Encode(string Data)
	{
		string text = "@" + Conversion.Hex(_ID).PadLeft(2, '0') + Data;
		return text + FCS(text) + "*\r";
	}

	public string FCS(string Data)
	{
		int num = checked(Data.Length - 1);
		int num2 = 0;
		byte b = default(byte);
		while (true)
		{
			int num3 = num2;
			int num4 = num;
			if (num3 > num4)
			{
				break;
			}
			b = (byte)(b ^ checked((byte)Strings.Asc(Data.Substring(num2, 1))));
			num2 = checked(num2 + 1);
		}
		return Conversion.Hex(b).PadLeft(2, '0');
	}
}
