using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class IJet : Inyector
{
	public enum JO_ERRORCODES
	{
		NoError
	}

	public enum IJ_COMMANDS
	{
		HANDSHAKE,
		CLEAR_PRINT_MEMORY,
		CLEAR_BUFFER_MEMORY,
		STOP_PRINT,
		START_PRINT,
		SELECT_HEAD_1,
		SELECT_HEAD_2,
		GET_VERSION,
		GET_STATUS,
		START_JOB,
		TRIGGER
	}

	public struct FIELD_ITEM
	{
		public string Key;

		public string Value;
	}

	public delegate void OnJobStartedEventHandler(object Sender);

	public delegate void OnJobFailureEventHandler(object Sender, string Cause);

	public delegate void OnGetVersionEventHandler(object sender, string Version);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	public const char IJ_ESC = '\u001b';

	private string[] StrCommands;

	private IJ_COMMANDS ActiveCommand;

	private List<FIELD_ITEM> Fields;

	private int NumFields;

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
	public event OnJobStartedEventHandler OnJobStarted;

	[method: DebuggerNonUserCode]
	public event OnJobFailureEventHandler OnJobFailure;

	[method: DebuggerNonUserCode]
	public event OnGetVersionEventHandler OnGetVersion;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	public IJet()
	{
		__ENCAddToList(this);
		StrCommands = new string[11]
		{
			"\u001b*", "\u001bC", "\u0002CLR\u0002", "\u001bC0\r", "\u001bC1\r", "\u001bK1\r", "\u001bK2\r", "\u001bSV\r", "\u001bS1\r", "\u0002TZ<Data>",
			"\u001bF\r"
		};
		Fields = new List<FIELD_ITEM>();
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

	protected override Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		return result;
	}

	public bool SendCmd(IJ_COMMANDS CmdId)
	{
		ClearInputBuffer();
		ActiveCommand = CmdId;
		string sTxt = StrCommands[(int)CmdId];
		return SendData(sTxt, 0L);
	}

	public void ClearInputBuffer()
	{
		if ((GetConnectionType() == CONNECTION_TYPE.Serial && SerialComm.IsOpen) ? true : false)
		{
			SerialComm.DiscardInBuffer();
		}
		if ((GetConnectionType() == CONNECTION_TYPE.TcpIp && TcpComm.Connected) ? true : false)
		{
			int available = TcpComm.Available;
			if (available > 0)
			{
				byte[] buffer = new byte[checked(available + 1)];
				TcpComm.GetStream().Read(buffer, 0, available);
			}
		}
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		bool result = true;
		ClearInputBuffer();
		ActiveCommand = IJ_COMMANDS.GET_STATUS;
		if (!SendData(StrCommands[8], lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public void ClearPrintMemory()
	{
		if (!SendCmd(IJ_COMMANDS.CLEAR_PRINT_MEMORY))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void StopPrint()
	{
		if (!SendCmd(IJ_COMMANDS.STOP_PRINT))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void StartPrint()
	{
		if (!SendCmd(IJ_COMMANDS.START_PRINT))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void GetVersion()
	{
		if (!SendCmd(IJ_COMMANDS.GET_VERSION))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void Trigger()
	{
		if (!SendCmd(IJ_COMMANDS.TRIGGER))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void SetHeader(int Header)
	{
		IJ_COMMANDS cmdId = IJ_COMMANDS.SELECT_HEAD_1;
		if (Header == 2)
		{
			cmdId = IJ_COMMANDS.SELECT_HEAD_2;
		}
		if (!SendCmd(cmdId))
		{
			OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
		}
	}

	public void StartJob(string FileName)
	{
		string text = StrCommands[9];
		string text2 = FileName + ".00I;11\r";
		ActiveCommand = IJ_COMMANDS.START_JOB;
		checked
		{
			int num = Fields.Count - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text2 = text2 + Fields[num2].Value + "\r";
				num2++;
			}
			text2 += "\u0003";
			text = text.Replace("<Data>", text2);
			if (!SendData(text, 0L))
			{
				OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
			}
		}
	}

	public void ClearFields()
	{
		Fields.Clear();
	}

	public void AddField(string Key, string Value)
	{
		FIELD_ITEM item = default(FIELD_ITEM);
		item.Key = Key;
		item.Value = Value;
		Fields.Add(item);
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
			int num = sBuffIn.IndexOf('\r');
			AnswerRecv = true;
			if (num > 0)
			{
				string text = sBuffIn.Substring(0, num);
				sBuffIn = "";
				switch ((int)ActiveCommand)
				{
				case 8:
				{
					string[] array = text.Split(':');
					if (Conversions.ToInteger(array[0]) == 2)
					{
						OnError?.Invoke(this, "", "Falta Tinta", Common.ERROR_TYPE.Error);
					}
					if (Conversions.ToInteger(array[0]) == 1)
					{
						OnError?.Invoke(this, "", "Tinta Baja", Common.ERROR_TYPE.Warning);
					}
					if (Conversions.ToInteger(array[3]) == 0)
					{
						OnError?.Invoke(this, "", "Clock sin Pulsos", Common.ERROR_TYPE.Error);
					}
					if (Conversions.ToInteger(array[4]) == 1)
					{
						OnError?.Invoke(this, "", "Bateria Vacía", Common.ERROR_TYPE.Error);
					}
					Online?.Invoke(this);
					break;
				}
				case 7:
					OnGetVersion?.Invoke(this, text);
					break;
				}
			}
			num = sBuffIn.IndexOf('\u0003');
			if (num <= 0)
			{
				return;
			}
			string text2 = sBuffIn.Substring(0, num);
			sBuffIn = "";
			IJ_COMMANDS activeCommand = ActiveCommand;
			if (activeCommand != IJ_COMMANDS.START_JOB)
			{
				return;
			}
			if (text2[0] == '\u0006')
			{
				OnJobStarted?.Invoke(this);
				return;
			}
			switch (Strings.Asc(text2[3]))
			{
			case 1:
				OnJobFailure?.Invoke(this, "Photocell triggered without data");
				break;
			case 2:
				OnJobFailure?.Invoke(this, "Unknown command");
				break;
			case 3:
				OnJobFailure?.Invoke(this, "No existe fichero en dispositivo");
				break;
			case 4:
				OnJobFailure?.Invoke(this, "No se detecta cabezal");
				break;
			case 6:
				OnJobFailure?.Invoke(this, "El Sistema no está listo");
				break;
			case 5:
				break;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog("iJet: Error al processar los datos recibidos.", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		ProcessInputData();
	}
}
