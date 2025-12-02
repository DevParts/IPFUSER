using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Timers;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Specifics;

public class Scanpal2 : MacsaDevice
{
	public delegate void OnDataReadyEventHandler(object sender, string sData);

	public delegate void DataReceptionFinishedEventHandler(object sender);

	public delegate void OnAckEventHandler(object sender);

	public delegate void OnNackEventHandler(object sender);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private long NumReads;

	[AccessedThroughProperty("oTimer")]
	private Timer _oTimer;

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
			SerialPinChangedEventHandler value2 = mComm_PinChanged;
			SerialDataReceivedEventHandler value3 = mComm_DataReceived;
			if (base.SerialComm != null)
			{
				base.SerialComm.PinChanged -= value2;
				base.SerialComm.DataReceived -= value3;
			}
			base.SerialComm = value;
			if (base.SerialComm != null)
			{
				base.SerialComm.PinChanged += value2;
				base.SerialComm.DataReceived += value3;
			}
		}
	}

	private virtual Timer oTimer
	{
		[DebuggerNonUserCode]
		get
		{
			return _oTimer;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[DebuggerNonUserCode]
		set
		{
			ElapsedEventHandler value2 = oTimer_Elapsed;
			if (_oTimer != null)
			{
				_oTimer.Elapsed -= value2;
			}
			_oTimer = value;
			if (_oTimer != null)
			{
				_oTimer.Elapsed += value2;
			}
		}
	}

	[method: DebuggerNonUserCode]
	public event OnDataReadyEventHandler OnDataReady;

	[method: DebuggerNonUserCode]
	public event DataReceptionFinishedEventHandler DataReceptionFinished;

	[method: DebuggerNonUserCode]
	public event OnAckEventHandler OnAck;

	[method: DebuggerNonUserCode]
	public event OnNackEventHandler OnNack;

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

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		int bytesToRead = SerialComm.BytesToRead;
		byte[] array = new byte[checked(bytesToRead - 1 + 1)];
		SerialComm.Read(array, 0, bytesToRead);
		ProcessInputData(array);
	}

	private void mComm_PinChanged(object sender, SerialPinChangedEventArgs e)
	{
		if (e.EventType == SerialPinChange.CtsChanged)
		{
			RequestData();
		}
	}

	private void ProcessInputData(byte[] bIn)
	{
		int num = Information.UBound(bIn);
		int num2 = 0;
		checked
		{
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 <= num4)
				{
					sBuffIn += Conversions.ToString(Strings.Chr(bIn[num2]));
					if (bIn[num2] == 15)
					{
						SerialComm.RtsEnable = true;
						sBuffIn = "";
						RequestData();
						break;
					}
					if (Strings.InStr(sBuffIn, "ACK\r") > 0)
					{
						sBuffIn = "";
						OnAck?.Invoke(this);
						SendAck();
					}
					else if (Strings.InStr(sBuffIn, "NAK\r") > 0)
					{
						sBuffIn = "";
						OnNack?.Invoke(this);
					}
					else if (Operators.CompareString(sBuffIn, "\r", TextCompare: false) == 0)
					{
						sBuffIn = "";
					}
					else if (Operators.CompareString(Conversions.ToString(Strings.Chr(bIn[num2])), "\r", TextCompare: false) == 0)
					{
						string sData = Strings.Mid(sBuffIn, 2, Strings.Len(sBuffIn) - 4);
						OnDataReady?.Invoke(this, sData);
						IncrementNumReads(1);
						sBuffIn = "";
						Common.Wait(50L);
						SendAck();
					}
					else if (Strings.InStr(sBuffIn, "OVER") > 0)
					{
						DataReceptionFinished?.Invoke(this);
						sBuffIn = "";
					}
					num2++;
					continue;
				}
				break;
			}
		}
	}

	private bool RequestData()
	{
		byte[] array = new byte[5];
		bool result = false;
		array[0] = 82;
		array[1] = 69;
		array[2] = 65;
		array[3] = 68;
		array[4] = 13;
		int num = 0;
		int num2;
		int num3;
		do
		{
			SerialComm.Write(Conversions.ToString(Strings.Chr(array[num])));
			num = checked(num + 1);
			num2 = num;
			num3 = 4;
		}
		while (num2 <= num3);
		return result;
	}

	private bool SendAck()
	{
		byte[] array = new byte[4];
		bool result = false;
		array[0] = 65;
		array[1] = 67;
		array[2] = 75;
		array[3] = 13;
		if (SendData(array, 0L))
		{
			result = true;
		}
		return result;
	}

	private void oTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		RequestInitTx();
	}

	public Scanpal2()
	{
		__ENCAddToList(this);
		NumReads = 0L;
		oTimer = new Timer();
		oTimer.Enabled = false;
	}

	protected override void Finalize()
	{
		oTimer.Enabled = false;
		base.Finalize();
	}

	protected override void TcpComm_DataReceived(string sData)
	{
	}

	public bool Init(string sPort, int iBauds, bool bIsIR, double dTimerVal)
	{
		if (bIsIR && dTimerVal >= 0.0)
		{
			iBauds = 38400;
			oTimer.Interval = dTimerVal;
			oTimer.Enabled = true;
			Common.MACSALog("Scanpal2: Se ha forzado la velocidad a 38400 para trabajar con la Cradle-IR.", TraceEventType.Error);
		}
		return Init(sPort, iBauds);
	}

	public void SetTimerRequestData(bool bVal, double dInterval = 0.0)
	{
		if (dInterval == 0.0)
		{
			dInterval = 1000.0;
		}
		oTimer.Enabled = false;
		oTimer.Interval = dInterval;
		oTimer.Enabled = bVal;
	}

	public bool RequestInitTx()
	{
		byte[] array = new byte[1];
		bool result = false;
		SerialComm.RtsEnable = false;
		SerialComm.DtrEnable = false;
		SerialComm.RtsEnable = true;
		SerialComm.DtrEnable = true;
		SerialComm.RtsEnable = false;
		array[0] = 15;
		if (SendData(array, 0L))
		{
			result = true;
		}
		return result;
	}

	public long GetNumReads()
	{
		return NumReads;
	}

	public long IncrementNumReads(int iStep)
	{
		checked
		{
			NumReads += iStep;
			return NumReads;
		}
	}

	public void ResetNumReads()
	{
		NumReads = 0L;
	}

	public void SetNumReadsValue(long lVal)
	{
		NumReads = lVal;
	}
}
