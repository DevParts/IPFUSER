using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Weigh;

public class Pesadora : MacsaDevice
{
	public delegate void OnPesoReadyEventHandler(object sender, string sSts, double dPeso);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	protected bool m_bPesoRecv;

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
	public event OnPesoReadyEventHandler OnPesoReady;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

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

	public Pesadora()
	{
		__ENCAddToList(this);
		m_bPesoRecv = false;
	}

	public void GetPeso()
	{
		m_bPesoRecv = false;
		SendData(BuildFrame(5), 1000L, WAIT_TYPE.Thread);
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		sBuffIn += SerialComm.ReadExisting();
		ProcessInputData();
	}

	private void ProcessInputData()
	{
		try
		{
			if ((Strings.Asc(Strings.Left(sBuffIn, 1)) == 2) & (Strings.Asc(Strings.Right(sBuffIn, 1)) == 3))
			{
				string sSts = Strings.Mid(sBuffIn, 2, 1);
				string expression = Strings.Mid(sBuffIn, 3, checked(Strings.Len(sBuffIn) - 3));
				expression = Strings.Replace(expression, ".", ",");
				double dPeso = Conversions.ToDouble(expression);
				m_bPesoRecv = true;
				OnPesoReady?.Invoke(this, sSts, dPeso);
				sBuffIn = "";
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog("Pesadora: Error al procesar los datos recibidos 232.", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private byte[] BuildFrame(byte bCmd)
	{
		return new byte[3] { 2, bCmd, 3 };
	}

	protected override void Finalize()
	{
		base.Finalize();
	}

	protected override void TcpComm_DataReceived(string sData)
	{
	}
}
