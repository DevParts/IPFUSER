using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class Datamax : Etiquetadora
{
	public enum DATAMAX_ERRORCODES
	{
		Noerror,
		ProcessingLabel,
		PaperOut,
		RibbonOut,
		LabelsInBatch,
		Busy,
		PauseOn,
		LabelPresented,
		NotConnected
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnGetCounterEventHandler(object sender, int Counter);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string CMD_STATUS = "\u0001A";

	private const string CMD_CANCEL = "\u0001C";

	private const string CMD_RESUME = "\u0001B";

	private const string CMD_WAKEUP = "\u0002G";

	private const string CMD_COUNTER = "\u0001e";

	private const string CMD_NONE = null;

	private DATAMAX_ERRORCODES ErrorCode;

	private string LastCommand;

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

	public bool IsRs422
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveLabelEventHandler OnReadyToReceiveLabel;

	[method: DebuggerNonUserCode]
	public event OnGetCounterEventHandler OnGetCounter;

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

	public override bool SendLabel(string sPathLabel)
	{
		bool result = false;
		sPathLabel = Strings.Trim(sPathLabel);
		checked
		{
			if (Operators.CompareString(sPathLabel, "", TextCompare: false) != 0)
			{
				FileStream fileStream = default(FileStream);
				BinaryReader binaryReader = default(BinaryReader);
				try
				{
					fileStream = new FileStream(sPathLabel, FileMode.Open);
					binaryReader = new BinaryReader(fileStream);
					byte[] array = new byte[(int)fileStream.Length + 1];
					binaryReader.Read(array, 0, (int)fileStream.Length);
					result = SendData(array, 0L);
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Common.MACSALog("DataMax: Error al enviar etiqueta: '" + sPathLabel + "'", TraceEventType.Error, ex2.Message);
					ProjectData.ClearProjectError();
				}
				finally
				{
					binaryReader.Close();
					fileStream.Close();
				}
			}
			return result;
		}
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		bool result = true;
		_IsOnLine = false;
		sBuffIn = "";
		if (!SendData("\u0001A", lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		else
		{
			LastCommand = "\u0001A";
		}
		return result;
	}

	public bool DoSoftReset()
	{
		sBuffIn = "";
		SendData("\u0001C", 0L);
		Common.Wait(1000L);
		return SendData("\u0001B", 0L);
	}

	public bool WakeUpLastLabel()
	{
		if (SendData("\u0002G", 0L))
		{
			return true;
		}
		return false;
	}

	public bool GetCounter(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		sBuffIn = "";
		if (!SendData("\u0001e", lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			return false;
		}
		LastCommand = "\u0001e";
		bool result = default(bool);
		return result;
	}

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = sErrorId.ToString();
		long num = sErrorId;
		if (num > 8 || num < 0)
		{
			goto IL_016c;
		}
		switch ((int)num)
		{
		case 0:
			break;
		case 1:
			goto IL_0079;
		case 2:
			goto IL_009d;
		case 3:
			goto IL_00c1;
		case 4:
			goto IL_00e5;
		case 5:
			goto IL_0109;
		case 6:
			goto IL_012a;
		case 8:
			goto IL_014b;
		default:
			goto IL_016c;
		}
		result.Desc = Common.Rm.GetString("Datamax1");
		result.Type = Common.ERROR_TYPE.NoError;
		goto IL_018b;
		IL_014b:
		result.Desc = Common.Rm.GetString("Datamax8");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_016c:
		result.Desc = Common.Rm.GetString("Datamax9");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_018b:
		return result;
		IL_0079:
		result.Desc = Common.Rm.GetString("Datamax2");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_009d:
		result.Desc = Common.Rm.GetString("Datamax3");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_00c1:
		result.Desc = Common.Rm.GetString("Datamax4");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_00e5:
		result.Desc = Common.Rm.GetString("Datamax5");
		result.Type = Common.ERROR_TYPE.Warning;
		goto IL_018b;
		IL_0109:
		result.Desc = Common.Rm.GetString("Datamax6");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
		IL_012a:
		result.Desc = Common.Rm.GetString("Datamax7");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_018b;
	}

	public Datamax()
	{
		__ENCAddToList(this);
		ErrorCode = DATAMAX_ERRORCODES.Noerror;
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		sBuffIn += SerialComm.ReadExisting();
		if (!IsRs422)
		{
			int num = sBuffIn.IndexOf('\r');
			if (num >= 0)
			{
				AnswerRecv = true;
				ProcessInputData(sBuffIn.Substring(0, num));
				sBuffIn = "";
			}
		}
		else if ((sBuffIn.Length == 8) & ((Operators.CompareString(Conversions.ToString(sBuffIn[0]), "Y", TextCompare: false) == 0) | (Operators.CompareString(Conversions.ToString(sBuffIn[0]), "N", TextCompare: false) == 0)))
		{
			AnswerRecv = true;
			ProcessInputData(sBuffIn.Substring(0, 8));
			sBuffIn = "";
		}
	}

	private void ProcessInputData(string sBuff)
	{
		try
		{
			string lastCommand = LastCommand;
			if (Operators.CompareString(lastCommand, "\u0001A", TextCompare: false) == 0)
			{
				if (CheckStatus(sBuff))
				{
					Common.DATA_ERROR dataError = GetDataError((int)ErrorCode);
					_IsOnLine = false;
					OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
				}
				else
				{
					_IsOnLine = true;
					Online?.Invoke(this);
					if (IsLabelRequested)
					{
						OnReadyToReceiveLabel?.Invoke(this);
						IsLabelRequested = false;
					}
				}
			}
			else if (Operators.CompareString(lastCommand, "\u0001e", TextCompare: false) == 0)
			{
				OnGetCounter?.Invoke(this, int.Parse(sBuff.Substring(0, 4)));
			}
			LastCommand = null;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Datamax10") + ": '" + sBuff + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private bool CheckStatus(string sSts)
	{
		if (Operators.CompareString(sSts.Substring(0, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.ProcessingLabel;
			return true;
		}
		if (Operators.CompareString(sSts.Substring(1, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.PaperOut;
			return true;
		}
		if (Operators.CompareString(sSts.Substring(2, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.RibbonOut;
			return true;
		}
		if (Operators.CompareString(sSts.Substring(3, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.LabelsInBatch;
			return true;
		}
		if (Operators.CompareString(sSts.Substring(4, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.Busy;
			return true;
		}
		if (Operators.CompareString(sSts.Substring(5, 1), "Y", TextCompare: false) == 0)
		{
			ErrorCode = DATAMAX_ERRORCODES.PauseOn;
			return true;
		}
		return false;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		int num = sBuffIn.IndexOf('\r');
		if (num >= 0)
		{
			AnswerRecv = true;
			ProcessInputData(sBuffIn.Substring(0, num));
			sBuffIn = "";
		}
	}

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return sBuff;
	}
}
