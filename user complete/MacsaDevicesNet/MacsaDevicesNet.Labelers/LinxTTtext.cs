using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class LinxTTtext : Etiquetadora
{
	public delegate void OnErrorEventHandler(object sender, string ErrCode, string ErrDesc, Common.ERROR_TYPE ErrType);

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private string RecepBuff;

	private long _BatchCounter;

	private long _TotalCounter;

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

	public long BatchCounter => _BatchCounter;

	public long TotalCounter => _TotalCounter;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveLabelEventHandler OnReadyToReceiveLabel;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	public LinxTTtext()
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

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return "";
	}

	protected override Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 0:
			result.Desc = Common.Rm.GetString("Zodiac3");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 1:
			result.Desc = Common.Rm.GetString("Zodiac1");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 2:
			result.Desc = Common.Rm.GetString("Zodiac2");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("Zodiac4");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}

	public void SetPrintingMode(bool Mode)
	{
		if (Mode)
		{
			SendData("SST|3|\r", 0L);
		}
		else
		{
			SendData("SST|4|\r", 0L);
		}
	}

	public void StartPrint()
	{
		SendData("SST|3|\r", 0L);
	}

	public void StopPrint()
	{
		SendData("SST|4|\r", 0L);
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		FlushComm();
		_IsOnLine = false;
		if (!SendData("GST\r", 2000L, WAIT_TYPE.Thread))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
		bool result = default(bool);
		return result;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		RecepBuff += sData;
		int num = RecepBuff.IndexOf('\r');
		if (num >= 0)
		{
			AnswerRecv = true;
			ProcessAnswer(RecepBuff.Substring(0, num));
			RecepBuff = "";
		}
	}

	public override bool SendLabel(string sPathLabel)
	{
		string text = "SLA|" + sPathLabel + "|";
		LABELFIELD lABELFIELD2 = default(LABELFIELD);
		foreach (object labelField in LabelFields)
		{
			LABELFIELD lABELFIELD = ((labelField != null) ? ((LABELFIELD)labelField) : lABELFIELD2);
			text = text + lABELFIELD.sKey + "=" + lABELFIELD.sValue + "|";
		}
		text += "\r";
		SendData(text, 0L);
		bool result = default(bool);
		return result;
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		RecepBuff += SerialComm.ReadExisting();
		int num = RecepBuff.IndexOf('\r');
		if (num >= 0)
		{
			AnswerRecv = true;
			ProcessAnswer(RecepBuff.Substring(0, num));
			RecepBuff = "";
		}
	}

	private void ProcessAnswer(string Data)
	{
		string[] array = Data.Split('|');
		if (Operators.CompareString(array[0], "STS", TextCompare: false) == 0)
		{
			if (Operators.CompareString(array[2], "0", TextCompare: false) != 0)
			{
				Common.DATA_ERROR dataError = GetDataError(Conversions.ToInteger(array[2]));
				OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
				return;
			}
			_BatchCounter = Conversions.ToInteger(array[4]);
			_TotalCounter = Conversions.ToInteger(array[5]);
			_IsOnLine = true;
			OnLine?.Invoke(this);
			if (IsLabelRequested)
			{
				OnReadyToReceiveLabel?.Invoke(this);
				IsLabelRequested = false;
			}
		}
		else if (Operators.CompareString(array[0], "ERR", TextCompare: false) == 0)
		{
			Common.DATA_ERROR dataError2 = GetDataError(0);
			OnError?.Invoke(this, dataError2.Id, dataError2.Desc, dataError2.Type);
		}
	}
}
