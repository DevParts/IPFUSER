using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.ScanReaders;

public class Lector : MacsaDevice
{
	public delegate void OnDataReadyEventHandler(object sender, string sData);

	public delegate void OnBadCodeEventHandler(object sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private long GoodReads;

	private long BadReads;

	private string FrameBegin;

	private string FrameEnd;

	private int LenBegin;

	private int LenEnd;

	private string BadReadCode;

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
	public event OnDataReadyEventHandler OnDataReady;

	[method: DebuggerNonUserCode]
	public event OnBadCodeEventHandler OnBadCode;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	public Lector()
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

	public bool Init(string Port, int Bauds, string FrameEnd, string FrameBegin = "", string sBadRead = "")
	{
		CommonInit(FrameEnd, FrameBegin, sBadRead);
		return Init(Port, Bauds);
	}

	public bool Init(int Port, string Ip, string FrameEnd, string FrameBegin = "", string BadRead = "")
	{
		CommonInit(FrameEnd, FrameBegin, BadRead);
		return Init(Port, Ip);
	}

	public long GetGoodReads()
	{
		return GoodReads;
	}

	public long GetBadReads()
	{
		return BadReads;
	}

	public long IncrementGoodReads(int Step)
	{
		checked
		{
			GoodReads += Step;
			return GoodReads;
		}
	}

	public long IncrementBadReads(int Step)
	{
		checked
		{
			BadReads += Step;
			return BadReads;
		}
	}

	public void ResetCounters()
	{
		BadReads = 0L;
		GoodReads = 0L;
	}

	public virtual void SendCmd(string Data, WAIT_TYPE WaitType = WAIT_TYPE.Thread, long TimeToWait = 1000L)
	{
		try
		{
			if (!SendData(Data, TimeToWait, WaitType))
			{
				Common.MACSALog(Common.Rm.GetString("Scanner1") + ": '" + Data + "'", TraceEventType.Information);
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Scanner1") + ": '" + Data + "'", TraceEventType.Information);
			ProjectData.ClearProjectError();
		}
	}

	private bool CommonInit(string sFrameEnd, string sFrameBegin = "", string sBadRead = "")
	{
		bool flag = false;
		GoodReads = 0L;
		BadReads = 0L;
		FrameBegin = "";
		if (Operators.CompareString(sFrameBegin, "", TextCompare: false) != 0)
		{
			FrameBegin = sFrameBegin;
		}
		LenBegin = Strings.Len(sFrameBegin);
		FrameEnd = sFrameEnd;
		LenEnd = Strings.Len(sFrameEnd);
		BadReadCode = sBadRead;
		return true;
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

	private void ProcessInputData()
	{
		checked
		{
			try
			{
				if (Operators.CompareString(Strings.Right(sBuffIn, LenEnd), FrameEnd, TextCompare: false) == 0)
				{
					string text = Strings.Left(sBuffIn, Strings.Len(sBuffIn) - LenEnd);
					if ((LenBegin > 0) & (Operators.CompareString(Strings.Left(text, LenBegin), FrameBegin, TextCompare: false) == 0))
					{
						text = Strings.Right(text, Strings.Len(text) - LenBegin);
					}
					if ((Operators.CompareString(BadReadCode, "", TextCompare: false) != 0) & (Operators.CompareString(text, BadReadCode, TextCompare: false) == 0))
					{
						OnBadCode?.Invoke(this);
					}
					else
					{
						OnDataReady?.Invoke(this, text);
					}
					sBuffIn = "";
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("Scanner2"), TraceEventType.Information);
				ProjectData.ClearProjectError();
			}
		}
	}
}
