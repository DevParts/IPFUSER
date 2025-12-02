using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class FoxJetPro : Inyector
{
	public enum FP_ERRORCODES
	{
		NoError
	}

	public enum FP_COMMANDS
	{
		GET_COUNTER,
		GET_STATE,
		GET_TASKS,
		GET_CURRENT_TASK,
		GET_HEADS,
		GET_LINE_ID,
		GET_LINES,
		GET_USER_ELEMENTS,
		IDLE_TASK,
		LOAD_TASK,
		RESUME_TASK,
		SET_COUNTER,
		START_TASK,
		STOP_TASK,
		SET_USER_ELEMENT
	}

	public struct FIELD_ITEM
	{
		public string Key;

		public string Value;
	}

	public delegate void OnGetCounterEventHandler(object Sender, int Value);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnInformationEventHandler(object sender, string Message);

	public delegate void OnReadyToReceiveDataEventHandler(object sender);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	public const byte NOCMD = byte.MaxValue;

	private string[] StrCommands;

	private byte mFrameBegin;

	private byte mFrameEnd;

	private FP_COMMANDS ActiveCommand;

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
	public event OnGetCounterEventHandler OnGetCounter;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveDataEventHandler OnReadyToReceiveData;

	public FoxJetPro()
	{
		__ENCAddToList(this);
		StrCommands = new string[15]
		{
			"{Get count}", "{Get current state}", "{Get tasks}", "{Get current task}", "{Get heads}", " {Get line ID}", "{Get lines}", "{Get user elements}", " {Idle task}", "{Load task,,***}",
			"{Resume task}", "{Set count,,***}", "{Start task}", " {Stop task}", "{Set user elements,"
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

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		return result;
	}

	public bool Init(string Ip)
	{
		mFrameBegin = 123;
		mFrameEnd = 125;
		CommonInit(mFrameEnd, mFrameBegin);
		return Init(2202, Ip);
	}

	public bool SendCmd(FP_COMMANDS CmdId, string Parameters = "")
	{
		ActiveCommand = CmdId;
		string text = StrCommands[(int)CmdId];
		if (Operators.CompareString(Parameters, "", TextCompare: false) != 0)
		{
			text = text.Replace("***", Parameters);
		}
		return SendData(text, 0L);
	}

	public bool RequestToSendData()
	{
		IsDataSendRequested = true;
		return GetStatus(WAIT_TYPE.Thread, 1000L);
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		bool result = true;
		_IsOnLine = false;
		if ((GetConnectionType() == CONNECTION_TYPE.Serial && SerialComm.IsOpen) ? true : false)
		{
			SerialComm.DiscardInBuffer();
		}
		ActiveCommand = FP_COMMANDS.GET_STATE;
		if (!SendData(StrCommands[1], lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public void GetCounter()
	{
		Common.MACSALog("Get Counter", TraceEventType.Information);
		if (!SendCmd(FP_COMMANDS.GET_COUNTER))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void SetCounter(int Value)
	{
		if (!SendCmd(FP_COMMANDS.SET_COUNTER, Value.ToString()))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void LoadTask(string TaskName)
	{
		if (!SendCmd(FP_COMMANDS.LOAD_TASK, TaskName))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void StartTask()
	{
		if (!SendCmd(FP_COMMANDS.START_TASK))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void StopTask()
	{
		if (!SendCmd(FP_COMMANDS.STOP_TASK))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void PauseTask()
	{
		if (!SendCmd(FP_COMMANDS.IDLE_TASK))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void ResumeTask()
	{
		if (!SendCmd(FP_COMMANDS.RESUME_TASK))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	public void SendRemoteData()
	{
		string text = "";
		ActiveCommand = FP_COMMANDS.SET_USER_ELEMENT;
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
				text = text + "," + Fields[num2].Key + "," + Fields[num2].Value;
				num2++;
			}
			if (Operators.CompareString(text, "", TextCompare: false) != 0)
			{
				text += "}";
				string sTxt = StrCommands[14] + text;
				SendData(sTxt, 0L);
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

	private void CommonInit(byte bFrameEnd, byte bFrameBegin)
	{
		mFrameBegin = bFrameBegin;
		mFrameEnd = bFrameEnd;
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
			int num = Strings.InStr(sBuffIn, Conversions.ToString(Strings.Chr(mFrameBegin)));
			int num2 = Strings.InStr(sBuffIn, Conversions.ToString(Strings.Chr(mFrameEnd)));
			if (!(num > 0 && num2 > num))
			{
				return;
			}
			sBuffIn = sBuffIn.Replace("{", "");
			sBuffIn = sBuffIn.Replace("}", "");
			AnswerRecv = true;
			switch ((int)ActiveCommand)
			{
			case 0:
			{
				string text = sBuffIn.Split(',')[2];
				if (Versioned.IsNumeric(text))
				{
					OnGetCounter?.Invoke(this, Conversions.ToInteger(text));
					break;
				}
				string text2 = "";
				checked
				{
					int num3 = text.Length - 1;
					int num4 = 0;
					while (true)
					{
						int num5 = num4;
						int num6 = num3;
						if (num5 > num6)
						{
							break;
						}
						if (Versioned.IsNumeric(text[num4]))
						{
							text2 += Conversions.ToString(text[num4]);
						}
						num4++;
					}
					if (Versioned.IsNumeric(text2))
					{
						OnGetCounter?.Invoke(this, Conversions.ToInteger(text2));
					}
					break;
				}
			}
			case 1:
				_IsOnLine = true;
				Online?.Invoke(this);
				if (IsDataSendRequested)
				{
					OnReadyToReceiveData?.Invoke(this);
					IsDataSendRequested = false;
				}
				break;
			case 14:
				SendCmd(FP_COMMANDS.START_TASK);
				break;
			}
			sBuffIn = "";
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("FoxjetPro1"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		ProcessInputData();
	}
}
