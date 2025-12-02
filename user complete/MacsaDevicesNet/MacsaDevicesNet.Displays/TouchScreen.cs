using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Displays;

public class TouchScreen : MacsaDevice
{
	public delegate void OnDataReadyEventHandler(object sender, byte cmdId, string Data);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	public const byte NOCMD = byte.MaxValue;

	private byte FrameBegin;

	private byte FrameEnd;

	[SpecialName]
	private StaticLocalInitFlag _0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init;

	[SpecialName]
	private string _0024STATIC_0024ProcessInputData_00242001_0024Info;

	[SpecialName]
	private byte _0024STATIC_0024ProcessInputData_00242001_0024bCmdId;

	[SpecialName]
	private int _0024STATIC_0024ProcessInputData_00242001_0024RecState;

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
	public event OnInformationEventHandler OnInformation;

	[DebuggerNonUserCode]
	public TouchScreen()
	{
		__ENCAddToList(this);
		_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init = new StaticLocalInitFlag();
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

	public bool Init(string sPort, int iBauds, byte bFrameEnd, byte bFrameBegin)
	{
		CommonInit(bFrameEnd, bFrameBegin);
		return Init(sPort, iBauds);
	}

	public bool Init(int Port, string Ip, byte bFrameEnd, byte bFrameBegin)
	{
		CommonInit(bFrameEnd, bFrameBegin);
		return Init(Port, Ip);
	}

	public bool SendCmd(int iCmdId, byte[] data)
	{
		FlushComm();
		bool flag = false;
		int num = ((iCmdId == 255) ? 1 : 2);
		checked
		{
			byte[] array = new byte[data.Length + num + 1];
			array[0] = FrameBegin;
			if (iCmdId != 255)
			{
				array[1] = (byte)iCmdId;
			}
			int num2 = data.Length - 1;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				array[num3 + num] = data[num3];
				num3++;
			}
			array[data.Length + num] = FrameEnd;
			return SendData(array, 0L);
		}
	}

	public bool SendCmd(int iCmdId, string sData)
	{
		checked
		{
			byte[] array = new byte[Strings.Len(sData) - 1 + 1];
			int num = Strings.Len(sData) - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				array[num2] = (byte)Strings.Asc(Strings.Mid(sData, num2 + 1, 1));
				num2++;
			}
			return SendCmd(iCmdId, array);
		}
	}

	private void CommonInit(byte bFrameEnd, byte bFrameBegin)
	{
		FrameBegin = bFrameBegin;
		FrameEnd = bFrameEnd;
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
		bool lockTaken = false;
		try
		{
			Monitor.Enter(_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init, ref lockTaken);
			if (_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init.State == 0)
			{
				_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init.State = 2;
				_0024STATIC_0024ProcessInputData_00242001_0024Info = "";
			}
			else if (_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init.State == 2)
			{
				throw new IncompleteInitialization();
			}
		}
		finally
		{
			_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init.State = 1;
			if (lockTaken)
			{
				Monitor.Exit(_0024STATIC_0024ProcessInputData_00242001_0024Info_0024Init);
			}
		}
		byte[] bytes = Encoding.Default.GetBytes(sBuffIn);
		sBuffIn = "";
		checked
		{
			try
			{
				int num = bytes.Length - 1;
				int num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					switch (_0024STATIC_0024ProcessInputData_00242001_0024RecState)
					{
					case 0:
						if (bytes[num2] == FrameBegin)
						{
							_0024STATIC_0024ProcessInputData_00242001_0024RecState = 1;
						}
						break;
					case 1:
						_0024STATIC_0024ProcessInputData_00242001_0024bCmdId = bytes[num2];
						_0024STATIC_0024ProcessInputData_00242001_0024Info = "";
						_0024STATIC_0024ProcessInputData_00242001_0024RecState = 2;
						break;
					case 2:
						if (bytes[num2] != FrameEnd)
						{
							_0024STATIC_0024ProcessInputData_00242001_0024Info += Conversions.ToString(Strings.Chr(bytes[num2]));
							break;
						}
						_0024STATIC_0024ProcessInputData_00242001_0024RecState = 0;
						OnDataReady?.Invoke(this, _0024STATIC_0024ProcessInputData_00242001_0024bCmdId, _0024STATIC_0024ProcessInputData_00242001_0024Info);
						break;
					}
					num2++;
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("TouchScreen1"), TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}
}
