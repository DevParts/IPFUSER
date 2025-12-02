using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using Advantech.Adam;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.DigitalIO;

public class Adam6000 : EntradaSalida, IDisposable
{
	public enum ADAM_MODELS
	{
		ADAM6060,
		ADAM6050
	}

	public class SALIDA
	{
		public int Pin;

		public int Estado;

		public int LifeTime;

		public int ConsumedTime;

		[DebuggerNonUserCode]
		public SALIDA()
		{
		}
	}

	public delegate void OnDIChangedEventHandler(object Sender, int DINum, bool DIState, ref bool[] DIs, double DI);

	public delegate void OnDIChangedTotalEventHandler(object Sender, ref bool[] DI, double DI);

	public delegate void OnDOChangedEventHandler(object Sender, int DONum, bool DOState, ref bool[] DOs, double DO);

	public delegate void OnDOChangedTotalEventHandler(object Sender, ref bool[] DOs, double DO);

	public delegate void OnPulseEndEventHandler(object Sender, int Output);

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private int INPUTS;

	private int OUTPUTS;

	private bool[] ReadDIData;

	private AdamSocket adamModbus;

	private Adam6000Type Adam6000Type;

	private int NumDI;

	private int NumDO;

	private long Count;

	private bool Abort;

	private Thread thTask;

	private int Timer;

	private long LastValTime;

	private int _FilterTime;

	private bool[] DIDataOld;

	private bool[] DODataOld;

	private byte InputPossibleValue;

	private byte InputValue;

	private bool CountersEnabled;

	private int CountersPollTime;

	private int[] Counters;

	private Queue<SALIDA> QueueOuts;

	private bool disposedValue;

	public int Counter
	{
		get
		{
			if ((Pin >= 0) & (Pin <= checked(INPUTS - 1)))
			{
				return Counters[Pin];
			}
			return 0;
		}
	}

	public bool DI
	{
		get
		{
			if ((Pin >= 0) & (Pin <= checked(INPUTS - 1)))
			{
				return ReadDIData[Pin];
			}
			return false;
		}
	}

	public int MaxInputs => INPUTS;

	public int MaxOutputs => OUTPUTS;

	public bool AdamStarted
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	[method: DebuggerNonUserCode]
	public event OnDIChangedEventHandler OnDIChanged;

	[method: DebuggerNonUserCode]
	public event OnDIChangedTotalEventHandler OnDIChangedTotal;

	[method: DebuggerNonUserCode]
	public event OnDOChangedEventHandler OnDOChanged;

	[method: DebuggerNonUserCode]
	public event OnDOChangedTotalEventHandler OnDOChangedTotal;

	[method: DebuggerNonUserCode]
	public event OnPulseEndEventHandler OnPulseEnd;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

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

	public Adam6000()
	{
		__ENCAddToList(this);
		INPUTS = 6;
		OUTPUTS = 6;
		thTask = new Thread(ThreadTaskIO);
		CountersEnabled = false;
		CountersPollTime = 1000;
		QueueOuts = new Queue<SALIDA>();
		adamModbus = new AdamSocket();
		_FilterTime = 200;
	}

	public Adam6000(int FilterTime)
	{
		__ENCAddToList(this);
		INPUTS = 6;
		OUTPUTS = 6;
		thTask = new Thread(ThreadTaskIO);
		CountersEnabled = false;
		CountersPollTime = 1000;
		QueueOuts = new Queue<SALIDA>();
		adamModbus = new AdamSocket();
		_FilterTime = FilterTime;
	}

	public Adam6000(bool EnableCounters, int PollingTime)
	{
		__ENCAddToList(this);
		INPUTS = 6;
		OUTPUTS = 6;
		thTask = new Thread(ThreadTaskIO);
		CountersEnabled = false;
		CountersPollTime = 1000;
		QueueOuts = new Queue<SALIDA>();
		adamModbus = new AdamSocket();
		CountersEnabled = EnableCounters;
		CountersPollTime = PollingTime;
	}

	public bool Connect(ADAM_MODELS Model, string sIp, int iPort, int iTimer, int iTimeOut)
	{
		switch ((int)Model)
		{
		case 0:
			INPUTS = 6;
			OUTPUTS = 6;
			break;
		case 1:
			INPUTS = 12;
			OUTPUTS = 6;
			break;
		}
		return Connect(sIp, iPort, INPUTS, OUTPUTS, iTimer, iTimeOut);
	}

	public bool Connect(string sIp, int iPort, int iNumDI, int iNumDO, int iTimer, int iTimeOut)
	{
		checked
		{
			if (!AdamStarted)
			{
				NumDI = iNumDI;
				NumDO = iNumDO;
				Timer = iTimer;
				DIDataOld = new bool[NumDI - 1 + 1];
				DODataOld = new bool[NumDO - 1 + 1];
				ReadDIData = new bool[NumDI - 1 + 1];
				Counters = new int[NumDI - 1 + 1];
				int num = Information.UBound(DIDataOld);
				int num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					DIDataOld[num2] = true;
					num2++;
				}
				int num5 = Information.UBound(DODataOld);
				num2 = 0;
				while (true)
				{
					int num6 = num2;
					int num4 = num5;
					if (num6 > num4)
					{
						break;
					}
					DODataOld[num2] = true;
					num2++;
				}
				adamModbus.SetTimeout(iTimeOut, iTimeOut, iTimeOut);
				if (adamModbus.Connect(sIp, ProtocolType.Tcp, iPort))
				{
					Count = 0L;
					Abort = false;
					if (CountersEnabled)
					{
						thTask = new Thread(ThreadTaskCounters);
						thTask.IsBackground = false;
						thTask.Start();
						Common.MACSALog(Common.Rm.GetString("Adam1"), TraceEventType.Information);
					}
					else
					{
						thTask = new Thread(ThreadTaskIO);
						thTask.IsBackground = false;
						thTask.Start();
						Common.MACSALog(Common.Rm.GetString("Adam2"), TraceEventType.Information);
					}
					AdamStarted = true;
				}
			}
			return AdamStarted;
		}
	}

	public void Disconnect()
	{
		if (AdamStarted)
		{
			AdamStarted = false;
			Abort = true;
			while (thTask.IsAlive)
			{
			}
			adamModbus.Disconnect();
		}
	}

	public bool ClearCounter(int Pin)
	{
		if (!AdamStarted)
		{
			return false;
		}
		checked
		{
			if ((Pin >= 0) & (Pin <= INPUTS - 1))
			{
				SALIDA sALIDA = new SALIDA();
				sALIDA.Pin = 34 + Pin * 4;
				sALIDA.Estado = 0;
				sALIDA.LifeTime = 0;
				sALIDA.ConsumedTime = 0;
				QueueOuts.Enqueue(sALIDA);
				return true;
			}
			return false;
		}
	}

	private void ThreadTaskCounters()
	{
		checked
		{
			while (!Abort)
			{
				try
				{
					adamModbus.Modbus().ReadHoldingRegs(1, INPUTS * 2, out byte[] o_byteData);
					int num = 0;
					int num2 = INPUTS * 4 - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						int num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						int num6 = (o_byteData[0 + num3] << 8) + (o_byteData[1 + num3] + (o_byteData[2 + num3] << 24) + (o_byteData[3 + num3] << 16));
						Counters[num] = num6;
						num++;
						num3 += 4;
					}
					if (QueueOuts.Count > 0)
					{
						SALIDA sALIDA = new SALIDA();
						sALIDA = QueueOuts.Dequeue();
						adamModbus.Modbus().ForceSingleCoil(sALIDA.Pin, 1);
					}
					Thread.Sleep(CountersPollTime);
				}
				catch (ThreadAbortException ex)
				{
					ProjectData.SetProjectError(ex);
					ThreadAbortException ex2 = ex;
					ProjectData.ClearProjectError();
				}
				catch (Exception ex3)
				{
					ProjectData.SetProjectError(ex3);
					Exception ex4 = ex3;
					Common.MACSALog(Common.Rm.GetString("Adam3"), TraceEventType.Critical, ex4.Message);
					OnError?.Invoke(this, "-1", Common.Rm.GetString("Adam3"), Common.ERROR_TYPE.Error);
					ProjectData.ClearProjectError();
					break;
				}
			}
		}
	}

	private void ThreadTaskIO()
	{
		int i_iStartIndex = 1;
		int i_iStartIndex2 = 17;
		checked
		{
			int tickCount = default(int);
			bool flag2 = default(bool);
			while (!Abort)
			{
				try
				{
					int num3;
					if (adamModbus.Modbus().ReadCoilStatus(i_iStartIndex, NumDI, out bool[] o_bCoil))
					{
						bool flag = false;
						bool[] destinationArray = new bool[NumDI - 1 + 1];
						Array.Copy(o_bCoil, 0, destinationArray, 0, NumDI);
						Array.Copy(o_bCoil, 0, ReadDIData, 0, NumDI);
						int num = 0;
						int num2 = Information.UBound(o_bCoil);
						num3 = 0;
						while (true)
						{
							int num4 = num3;
							int num5 = num2;
							if (num4 > num5)
							{
								break;
							}
							if (o_bCoil[num3])
							{
								num += (int)Math.Round(Math.Pow(2.0, num3));
							}
							num3++;
						}
						if (InputPossibleValue != num)
						{
							tickCount = MyProject.Computer.Clock.TickCount;
							InputPossibleValue = (byte)num;
						}
						if ((InputValue != InputPossibleValue && MyProject.Computer.Clock.TickCount - tickCount > _FilterTime) ? true : false)
						{
							InputValue = InputPossibleValue;
							OnDIChangedTotal?.Invoke(this, ref o_bCoil, unchecked((int)InputValue));
						}
					}
					if (adamModbus.Modbus().ReadCoilStatus(i_iStartIndex2, NumDO, out bool[] o_bCoil2))
					{
						int num6 = 0;
						int num7 = Information.UBound(o_bCoil2);
						num3 = 0;
						while (true)
						{
							int num8 = num3;
							int num5 = num7;
							if (num8 > num5)
							{
								break;
							}
							if (o_bCoil2[num3])
							{
								num6 += (int)Math.Round(Math.Pow(2.0, num3));
							}
							num3++;
						}
						int num9 = Information.UBound(o_bCoil2);
						num3 = 0;
						while (true)
						{
							int num10 = num3;
							int num5 = num9;
							if (num10 > num5)
							{
								break;
							}
							if (o_bCoil2[num3] != DODataOld[num3])
							{
								OnDOChanged?.Invoke(this, num3, o_bCoil2[num3], ref o_bCoil2, num6);
								DODataOld[num3] = o_bCoil2[num3];
								flag2 = true;
							}
							num3++;
						}
						if (flag2)
						{
							Array.Copy(DODataOld, 0, o_bCoil2, 0, NumDO);
							OnDOChangedTotal?.Invoke(this, ref o_bCoil2, num6);
						}
					}
					int count = QueueOuts.Count;
					int num11 = count - 1;
					num3 = 0;
					while (true)
					{
						int num12 = num3;
						int num5 = num11;
						if (num12 > num5)
						{
							break;
						}
						SALIDA sALIDA = QueueOuts.Peek();
						if (sALIDA.LifeTime == 0)
						{
							adamModbus.Modbus().ForceSingleCoil(sALIDA.Pin, sALIDA.Estado);
							QueueOuts.Dequeue();
						}
						else
						{
							SALIDA sALIDA2 = sALIDA;
							sALIDA2.ConsumedTime += Timer;
							if (sALIDA.ConsumedTime < sALIDA.LifeTime)
							{
								break;
							}
							adamModbus.Modbus().ForceSingleCoil(sALIDA.Pin, sALIDA.Estado);
							QueueOuts.Dequeue();
							OnPulseEnd?.Invoke(this, sALIDA.Pin - 17);
						}
						num3++;
					}
				}
				catch (ThreadAbortException ex)
				{
					ProjectData.SetProjectError(ex);
					ThreadAbortException ex2 = ex;
					ProjectData.ClearProjectError();
				}
				catch (Exception ex3)
				{
					ProjectData.SetProjectError(ex3);
					Exception ex4 = ex3;
					Common.MACSALog(Common.Rm.GetString("Adam4"), TraceEventType.Critical, ex4.Message);
					OnError?.Invoke(this, "-1", Common.Rm.GetString("Adam4"), Common.ERROR_TYPE.Error);
					ProjectData.ClearProjectError();
					break;
				}
				Thread.Sleep(Timer);
			}
		}
	}

	public override bool WriteDO(int iDO, bool bState)
	{
		int pin = checked(17 + iDO);
		if (AdamStarted)
		{
			int estado = 0;
			if (bState)
			{
				estado = 1;
			}
			SALIDA sALIDA = new SALIDA();
			sALIDA.Pin = pin;
			sALIDA.Estado = estado;
			sALIDA.LifeTime = 0;
			sALIDA.ConsumedTime = 0;
			QueueOuts.Enqueue(sALIDA);
			return true;
		}
		return false;
	}

	public override void DoPulseOutput(int iPin, long iLen)
	{
		checked
		{
			int pin = 17 + iPin;
			if (AdamStarted)
			{
				SALIDA sALIDA = new SALIDA();
				sALIDA.Pin = pin;
				sALIDA.Estado = 1;
				sALIDA.LifeTime = 0;
				sALIDA.ConsumedTime = 0;
				QueueOuts.Enqueue(sALIDA);
				sALIDA = new SALIDA();
				sALIDA.Pin = pin;
				sALIDA.Estado = 0;
				sALIDA.LifeTime = (int)iLen;
				sALIDA.ConsumedTime = 0;
				QueueOuts.Enqueue(sALIDA);
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue && disposing && AdamStarted)
		{
			Abort = true;
			thTask.Abort();
			while (thTask.IsAlive)
			{
			}
			adamModbus.Disconnect();
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	void IDisposable.Dispose()
	{
		//ILSpy generated this explicit interface implementation from .override directive in Dispose
		this.Dispose();
	}
}
