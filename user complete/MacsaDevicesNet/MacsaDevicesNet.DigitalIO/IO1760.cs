using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using AdvDIOLib;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.DigitalIO;

public class IO1760 : EntradaSalida
{
	public struct DATA_DIO
	{
		public int Sts;

		public long LastValTime;
	}

	public delegate void OnDIChangedEventHandler(object Sender, int InputsValue);

	public delegate void OnDOChangedEventHandler(object Sender, int InputsValue);

	public delegate void OnPulseEndEventHandler(object Sender, int Pin);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private AdvDIO AxAdvDIO;

	public const int NUM_IOS = 8;

	private int LastInputs;

	private int LastOutputs;

	private DATA_DIO NewStsVal;

	private bool[] LastState;

	private Thread threadDIOs;

	public long PulsoInLen
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	public int ThreadSleepTime
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	public int AdvantechDeviceNumber
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	[method: DebuggerNonUserCode]
	public event OnDIChangedEventHandler OnDIChanged;

	[method: DebuggerNonUserCode]
	public event OnDOChangedEventHandler OnDOChanged;

	[method: DebuggerNonUserCode]
	public event OnPulseEndEventHandler OnPulseEnd;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	public IO1760()
	{
		__ENCAddToList(this);
		Guid clsid = new Guid("8141E3A6-DC17-4EF9-9B84-1339EFCBAD39");
		AxAdvDIO = (AdvDIO)Activator.CreateInstance(Type.GetTypeFromCLSID(clsid));
		LastState = new bool[8];
		long pulsoInLen = 300L;
		PulsoInLen = pulsoInLen;
		int threadSleepTime = 10;
		ThreadSleepTime = threadSleepTime;
		threadSleepTime = 0;
		AdvantechDeviceNumber = threadSleepTime;
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

	public void StartInputScan()
	{
		AxAdvDIO.deviceNumber = AdvantechDeviceNumber;
		threadDIOs = new Thread(TaskIO);
		threadDIOs.IsBackground = true;
		NewStsVal.Sts = 0;
		NewStsVal.LastValTime = MyProject.Computer.Clock.TickCount;
		int num = Information.UBound(LastState);
		int num2 = 0;
		while (true)
		{
			int num3 = num2;
			int num4 = num;
			if (num3 > num4)
			{
				break;
			}
			LastState[num2] = false;
			num2 = checked(num2 + 1);
		}
		threadDIOs.Start();
	}

	public void EndInputScan()
	{
		if (threadDIOs.IsAlive)
		{
			threadDIOs.Abort();
		}
	}

	public override bool WriteDO(int Output, bool State)
	{
		checked
		{
			if (State)
			{
				AxAdvDIO.WriteDoChannel(1, Output - 1);
			}
			else
			{
				AxAdvDIO.WriteDoChannel(0, Output - 1);
			}
			bool result = default(bool);
			return result;
		}
	}

	public override void DoPulseOutput(int Output, long Time)
	{
		checked
		{
			try
			{
				AxAdvDIO.WriteDoChannel(1, Output - 1);
				Common.Wait((int)Time);
				AxAdvDIO.WriteDoChannel(0, Output - 1);
				OnPulseEnd?.Invoke(this, Output);
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("PCI17601") + "->" + ex2.Message, TraceEventType.Error);
				ProjectData.ClearProjectError();
			}
		}
	}

	private void TaskIO()
	{
		object pBuffer = RuntimeHelpers.GetObjectValue(new object());
		byte[] array = new byte[1];
		while (true)
		{
			try
			{
				AxAdvDIO.ReadDiPorts(out pBuffer, 0, 8);
				array = (byte[])pBuffer;
				int num = array[0];
				if (NewStsVal.Sts != num)
				{
					NewStsVal.Sts = num;
					NewStsVal.LastValTime = MyProject.Computer.Clock.TickCount;
				}
				if ((LastInputs != num) & (checked(MyProject.Computer.Clock.TickCount - NewStsVal.LastValTime) > PulsoInLen))
				{
					LastInputs = num;
					OnDIChanged?.Invoke(this, num);
				}
				AxAdvDIO.ReadDoPorts(out pBuffer, 0, 8);
				array = (byte[])pBuffer;
				int num2 = array[0];
				if (LastOutputs != num2)
				{
					LastOutputs = num2;
					OnDOChanged?.Invoke(this, num2);
				}
			}
			catch (ThreadAbortException ex)
			{
				ProjectData.SetProjectError(ex);
				ThreadAbortException ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("PCI17602") + "->" + ex2.Message, TraceEventType.Information);
				ProjectData.ClearProjectError();
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				Common.MACSALog(Common.Rm.GetString("PCI17602") + "->" + ex4.Message, TraceEventType.Error);
				ProjectData.ClearProjectError();
				break;
			}
			Thread.Sleep(ThreadSleepTime);
		}
	}
}
