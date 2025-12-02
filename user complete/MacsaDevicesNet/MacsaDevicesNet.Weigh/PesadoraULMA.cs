using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Weigh;

public class PesadoraULMA : MacsaDevice
{
	public enum COMMANDS
	{
		STATUS = 0,
		START_CONVEYOR = 1,
		STOP_CONVEYOR = 2,
		BATCH_DATA = 3,
		CLEAR_BATCH_DATA = 3,
		COUNTER = 4,
		RESET_COUNTER = 5
	}

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnErrorEventHandler(object Sender, string Datos);

	public delegate void NewWeightDataEventHandler(object sender, int Totales, int Debajo, int Pasan);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string STATUS = "0";

	private const string START_CONVEYOR = "12";

	private const string STOP_CONVEYOR = "13";

	private const string BATCH_DATA = "342";

	private const string CLEAR_BATCH_DATA = "242";

	private const string TOTAL_COUNTER = "364";

	private const string RESET_COUNTER = "212";

	private byte _ProductId;

	private COMMANDS _ActiveCommand;

	private int _TotalCounter;

	private int _PassCounter;

	private int _UnderCounter;

	public byte ProductId
	{
		get
		{
			return _ProductId;
		}
		set
		{
			if (value < 50)
			{
				_ProductId = value;
			}
		}
	}

	public int TotalCounter => _TotalCounter;

	public int PassCounter => _PassCounter;

	public int UnderCounter => _UnderCounter;

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event NewWeightDataEventHandler NewWeightData;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	public PesadoraULMA()
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

	public void GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		_ActiveCommand = COMMANDS.STATUS;
		SendData(Encode("0"), lTimeToWait, tWaitType);
	}

	public void SetConveyor(bool Mode)
	{
		if (Mode)
		{
			_ActiveCommand = COMMANDS.START_CONVEYOR;
			SendData(Encode("12"), 0L);
		}
		else
		{
			_ActiveCommand = COMMANDS.STOP_CONVEYOR;
			SendData(Encode("13"), 0L);
		}
	}

	public void GetBatchData(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		GetCounter();
	}

	public void ResetCounters()
	{
		_ActiveCommand = COMMANDS.RESET_COUNTER;
		SendData(Encode("212"), 0L);
	}

	public void GetCounter()
	{
		_ActiveCommand = COMMANDS.COUNTER;
		SendData(Encode("364"), 0L);
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		if (sBuffIn.IndexOf('\u0003') <= 0)
		{
			return;
		}
		AnswerRecv = true;
		string text = sBuffIn.Substring(1, checked(sBuffIn.Length - 2));
		sBuffIn = "";
		switch ((int)_ActiveCommand)
		{
		case 0:
			_ProductId = checked((byte)Conversions.ToInteger(text.Substring(6, 2)));
			OnLine?.Invoke(this);
			break;
		case 3:
		{
			string[] array = text.Split('\r');
			if (array.Length > 3)
			{
				_TotalCounter = Conversions.ToInteger(array[9]);
				_UnderCounter = Conversions.ToInteger(array[2]);
				_PassCounter = Conversions.ToInteger(array[6]);
				NewWeightData?.Invoke(this, Conversions.ToInteger(array[9]), Conversions.ToInteger(array[2]), Conversions.ToInteger(array[6]));
			}
			break;
		}
		case 4:
			_TotalCounter = Conversions.ToInteger(text.Substring(3));
			NewWeightData?.Invoke(this, _TotalCounter, 0, 0);
			break;
		case 1:
		case 2:
			break;
		}
	}

	public string Encode(string Data)
	{
		return "\u0002" + Data + "\u0003";
	}
}
