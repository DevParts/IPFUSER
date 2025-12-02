using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class Mectec : Etiquetadora
{
	private enum MECTEC_ERRORCODES
	{
		Noerror = 0,
		UnKnown = -1,
		CmdNak = -2,
		IllegalXVal = 1,
		IllegalYVal = 2,
		IllegalBarDim = 3,
		IllegalType = 4,
		UnknownLabelId = 12,
		FewDynStrings = 16,
		ManyDynStrings = 23,
		PaperOut = 805,
		RibbonOut = 806,
		NotConnected = 816,
		EmergencyStop = 818,
		NoAir = 822,
		PrintheadUp = 801,
		BarcodeNotReadable = 812,
		LabelInProgress = 990,
		CylinderOut = 992,
		PrintingInProgress = 998
	}

	public enum CMD41_CLEARBUFFER
	{
		NoClear,
		ClearQueueNotCurrent,
		ClearAll,
		NoClearQueueClearImage,
		ClearQueueAndImage,
		ClearAllAndImage
	}

	public enum COUNTERS
	{
		BatchCounter,
		TotalCounter
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnAckEventHandler(object sender, string sCmdId);

	public delegate void OnCountersReadEventHandler(object sender, long BatchCounter, long TotalCounter);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string MECTEC_ACK = "A";

	private const string MECTEC_NACK = "E";

	private const string MECTEC_ERROR = "D";

	private const byte FRAME_START = 2;

	private const byte FRAME_END = 13;

	private const string CMD_SENDIMAGE = "3B";

	private const string CMD_1B = "1B";

	private const string CMD_SENDTEXT = "0D";

	private const string CMD_STATUS = "00";

	private const string CMD_RESET = "02";

	private const string CMD_STOP_LABEL_PRINTING = "0F";

	private const string CMD_RESET_TOTAL_COUNTER = "35B";

	private const string CMD_RESET_BATCH_COUNTER = "35A";

	private const string CMD_READ_COUNTERS = "36";

	private MECTEC_ERRORCODES ErrorCode;

	private int Id485;

	private Thread thSendLabel;

	private string sBuffInToSend;

	private byte[] bBuffInToSend;

	private bool SendingDummy;

	private string DummyData;

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

	public bool DummyMode
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
	public event OnAckEventHandler OnAck;

	[method: DebuggerNonUserCode]
	public event OnCountersReadEventHandler OnCountersRead;

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

	public Mectec()
	{
		__ENCAddToList(this);
		thSendLabel = new Thread(ThreadSendLabel);
		SendingDummy = false;
		DummyData = null;
		bool dummyMode = false;
		DummyMode = dummyMode;
		Id485 = 0;
		ErrorCode = MECTEC_ERRORCODES.Noerror;
	}

	private string BuildFrame(string sData)
	{
		return "\u0002" + Strings.Format(Id485) + sData + "??\r";
	}

	private bool CheckStatus(string sSts)
	{
		bool result = true;
		switch (sSts)
		{
		case "001":
		case "002":
		case "003":
		case "004":
		case "012":
		case "016":
		case "023":
		case "801":
		case "805":
		case "806":
		case "812":
		case "816":
		case "818":
		case "822":
		case "990":
		case "992":
		case "998":
			if (true)
			{
				ErrorCode = (MECTEC_ERRORCODES)Conversions.ToInteger(sSts);
				break;
			}
			goto default;
		default:
			switch (sSts)
			{
			case "991":
			case "999":
			case "994":
			case "803":
			case "804":
				if (true)
				{
					ErrorCode = MECTEC_ERRORCODES.Noerror;
					result = false;
					break;
				}
				goto default;
			default:
				ErrorCode = MECTEC_ERRORCODES.UnKnown;
				break;
			}
			break;
		}
		return result;
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		if (!SendingDummy)
		{
			sBuffIn += SerialComm.ReadExisting();
			ProcessInputData(sBuffIn);
		}
		else
		{
			DummyData += SerialComm.ReadExisting();
		}
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		ProcessInputData(sBuffIn);
	}

	private void ProcessInputData(string sBuff)
	{
		byte[] array = new byte[5];
		byte[] array2 = new byte[9];
		try
		{
			if (Operators.CompareString(Strings.Right(sBuffIn, 1), "\r", TextCompare: false) != 0)
			{
				return;
			}
			AnswerRecv = true;
			if (Strings.Len(sBuff) > 6)
			{
				string text = Strings.Mid(sBuff, 3, 1);
				string text2 = Strings.Mid(sBuff, 4, 2);
				switch (text)
				{
				case "A":
					OnAck?.Invoke(this, text2);
					switch (text2)
					{
					case "00":
					{
						string sSts = Strings.Mid(sBuff, 6, 3);
						CheckStatus(sSts);
						if ((ErrorCode != MECTEC_ERRORCODES.Noerror) & (ErrorCode != MECTEC_ERRORCODES.PrintingInProgress))
						{
							Common.DATA_ERROR dataError = GetDataError((int)ErrorCode);
							_IsOnLine = false;
							OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
							break;
						}
						_IsOnLine = true;
						Online?.Invoke(this);
						if (IsLabelRequested)
						{
							OnReadyToReceiveLabel?.Invoke(this);
							IsLabelRequested = false;
						}
						break;
					}
					case "3B":
						OnInformation?.Invoke(this, "CMD_SENDIMAGE");
						break;
					case "1B":
						OnInformation?.Invoke(this, "CMD_1B");
						break;
					case "36":
						checked
						{
							array[0] = (byte)(Strings.Asc(Strings.Mid(sBuff, 6, 1)) - 48);
							array[1] = (byte)((Strings.Asc(Strings.Mid(sBuff, 7, 1)) - 48) * 2);
							array[2] = (byte)(Strings.Asc(Strings.Mid(sBuff, 8, 1)) - 48);
							array[3] = (byte)((Strings.Asc(Strings.Mid(sBuff, 9, 1)) - 48) * 2);
							byte[] array3 = array;
							byte[] array4 = array3;
							int num = 0;
							array4[num] = (byte)unchecked((uint)(array3[num] + array[1]));
							array3 = array;
							byte[] array5 = array3;
							num = 2;
							array5[num] = (byte)unchecked((uint)(array3[num] + array[3]));
							array[1] = array[2];
							long batchCounter = array[0] * 256 + array[1];
							array2[0] = (byte)(Strings.Asc(Strings.Mid(sBuff, 11, 1)) - 48);
							array2[1] = (byte)((Strings.Asc(Strings.Mid(sBuff, 12, 1)) - 48) * 2);
							array2[2] = (byte)(Strings.Asc(Strings.Mid(sBuff, 13, 1)) - 48);
							array2[3] = (byte)((Strings.Asc(Strings.Mid(sBuff, 14, 1)) - 48) * 2);
							array2[4] = (byte)(Strings.Asc(Strings.Mid(sBuff, 15, 1)) - 48);
							array2[5] = (byte)((Strings.Asc(Strings.Mid(sBuff, 16, 1)) - 48) * 2);
							array2[6] = (byte)(Strings.Asc(Strings.Mid(sBuff, 17, 1)) - 48);
							array2[7] = (byte)((Strings.Asc(Strings.Mid(sBuff, 18, 1)) - 48) * 2);
							array3 = array2;
							byte[] array6 = array3;
							num = 0;
							array6[num] = (byte)unchecked((uint)(array3[num] + array2[1]));
							array3 = array2;
							byte[] array7 = array3;
							num = 2;
							array7[num] = (byte)unchecked((uint)(array3[num] + array2[3]));
							array3 = array2;
							byte[] array8 = array3;
							num = 4;
							array8[num] = (byte)unchecked((uint)(array3[num] + array2[5]));
							array3 = array2;
							byte[] array9 = array3;
							num = 6;
							array9[num] = (byte)unchecked((uint)(array3[num] + array2[7]));
							array[0] = array2[0];
							array[1] = array2[2];
							array[2] = array2[4];
							array[3] = array2[6];
							long totalCounter = BitConverter.ToInt32(array, 0);
							OnCountersRead?.Invoke(this, batchCounter, totalCounter);
							break;
						}
					}
					break;
				case "D":
				{
					string sSts = Strings.Mid(sBuff, 4, 3);
					CheckStatus(sSts);
					if (ErrorCode != MECTEC_ERRORCODES.Noerror)
					{
						Common.DATA_ERROR dataError = GetDataError((int)ErrorCode);
						OnError?.Invoke(this, sSts, dataError.Desc, dataError.Type);
					}
					break;
				}
				case "E":
				{
					Common.DATA_ERROR dataError = GetDataError(-2);
					OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
					break;
				}
				}
			}
			sBuffIn = "";
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Mectec1") + ": '" + sBuff + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private void ThreadSendLabel()
	{
		string stringMatch = "\u0002" + Conversions.ToString(Id485);
		string @string = "??\r";
		try
		{
			OnInformation?.Invoke(this, Common.Rm.GetString("Mectec25"));
			bool flag = true;
			int num = 1;
			while (((num > 0) & (num < Strings.Len(sBuffInToSend))) && flag)
			{
				string text = "";
				flag = false;
				int num2 = Strings.InStr(num, sBuffInToSend, @string, CompareMethod.Text);
				checked
				{
					num2 = ((num2 >= num) ? (num2 + 3) : (Strings.Len(sBuffInToSend) + 1));
					string text2 = Strings.Mid(sBuffInToSend, num, num2 - num);
					string left = Strings.Mid(text2, 3, 2);
					if (Operators.CompareString(left, "3B", TextCompare: false) == 0)
					{
						flag = SendData(text2, 5000L, WAIT_TYPE.Activa);
						if (flag)
						{
							int num3 = num2;
							num2 = Strings.InStr(num2 - 1, sBuffInToSend, @string, CompareMethod.Text);
							num2 = Strings.InStrRev(sBuffInToSend, stringMatch, num2, CompareMethod.Text);
							text = Strings.Mid(sBuffInToSend, num3, num2 - num3);
							byte[] bytes = Encoding.Default.GetBytes(text);
							flag = SendData(Common.ConvertStringToByteArray(text), 5000L, WAIT_TYPE.Activa);
						}
					}
					else if (Operators.CompareString(left, "0D", TextCompare: false) == 0)
					{
						text = text2;
						flag = SendData(Common.ConvertStringToByteArray(text), 5000L, WAIT_TYPE.Activa);
					}
					else
					{
						text = text2;
						flag = SendData(text, 5000L, WAIT_TYPE.Activa);
					}
					if (!flag)
					{
						OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
					}
					num = num2;
				}
			}
			OnInformation?.Invoke(this, Common.Rm.GetString("Mectec2"));
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Mectec3"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private void ThreadSendLabelCmd41()
	{
		if (!SendData(Common.ConvertStringToByteArray(sBuffInToSend), 2000L, WAIT_TYPE.Activa))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
		}
	}

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return sBuff;
	}

	public override bool GetStatus(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		bool result = true;
		FlushComm();
		_IsOnLine = false;
		sBuffIn = "";
		if (!SendData(BuildFrame("00"), lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public bool ReadCounters()
	{
		return SendData(BuildFrame("36"), 0L);
	}

	public bool DoSoftReset()
	{
		return SendData(BuildFrame("02"), 0L);
	}

	public bool StopCurrentLabelPrinting()
	{
		return SendData(BuildFrame("0F"), 0L);
	}

	public bool ResetCounters(COUNTERS cCounterType)
	{
		if (cCounterType == COUNTERS.BatchCounter)
		{
			return SendData(BuildFrame("35A"), 0L);
		}
		return SendData(BuildFrame("35B"), 0L);
	}

	public override bool SendLabel(string sPathLabel)
	{
		bool result = default(bool);
		if (DummyMode)
		{
			SendDummyLabel(sPathLabel);
		}
		else
		{
			string text = "";
			result = false;
			sPathLabel = Strings.Trim(sPathLabel);
			if (Operators.CompareString(sPathLabel, "", TextCompare: false) != 0)
			{
				try
				{
					string fullPath = sPathLabel;
					string ErrInfo = "";
					sBuffInToSend = GetFileContents(fullPath, ref ErrInfo);
					if (!thSendLabel.IsAlive)
					{
						Thread.Sleep(50);
						thSendLabel = new Thread(ThreadSendLabel);
						thSendLabel.Start();
					}
					else
					{
						text = Common.Rm.GetString("Mectec4");
						OnError?.Invoke(this, "", text, Common.ERROR_TYPE.Error);
						Common.MACSALog(text, TraceEventType.Error);
					}
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Common.MACSALog(Common.Rm.GetString("Mectec3") + " '" + sPathLabel + "'", TraceEventType.Error, ex2.Message);
					ProjectData.ClearProjectError();
				}
			}
		}
		return result;
	}

	public void SendDummyLabel(string sPathLabel)
	{
		bool flag = false;
		string text = "";
		sPathLabel = Strings.Trim(sPathLabel);
		if (Operators.CompareString(sPathLabel, "", TextCompare: false) == 0)
		{
			return;
		}
		FileStream stream = new FileStream(sPathLabel, FileMode.Open);
		StreamReader streamReader = new StreamReader(stream, Encoding.Default);
		sBuffInToSend = streamReader.ReadToEnd();
		streamReader.Close();
		string text2 = "\u0002" + Conversions.ToString(Id485);
		string text3 = "??\r";
		try
		{
			bool flag2 = true;
			int num = 1;
			while (((num > 0) & (num < Strings.Len(sBuffInToSend))) && flag2)
			{
				string text4 = "";
				flag2 = false;
				int num2 = Strings.InStr(num, sBuffInToSend, "\r", CompareMethod.Text);
				checked
				{
					num2 = ((num2 >= num) ? (num2 + 1) : (Strings.Len(sBuffInToSend) + 1));
					string text5 = Strings.Mid(sBuffInToSend, num, num2 - num);
					int num3 = 1;
					while ((num3 > 0) & (num3 < Strings.Len(text5)))
					{
						string text6 = "";
						int num4 = Strings.InStr(num3, text5, "\n", CompareMethod.Text);
						if (num4 < num3)
						{
							num4 = Strings.Len(text5) + 1;
							flag = false;
						}
						else
						{
							num4++;
							flag = true;
						}
						text6 = Strings.Mid(text5, num3, num4 - num3);
						flag2 = ((!flag) ? SendData(Encoding.Default.GetBytes(text6), 200L, WAIT_TYPE.Activa) : SendData(Encoding.Default.GetBytes(text6), 0L));
						if (!flag2)
						{
							OnError?.Invoke(this, "", "Error al enviar los datos. Revise el log.", Common.ERROR_TYPE.Error);
						}
						num3 = num4;
					}
					num = num2;
				}
			}
			OnInformation?.Invoke(this, Common.Rm.GetString("Mectec2"));
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Mectec3"), TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	public bool SendLabelCmd41(int iLabelId, long lNumLabels = 0L, CMD41_CLEARBUFFER cClearBuffer = CMD41_CLEARBUFFER.ClearQueueNotCurrent)
	{
		bool result = false;
		string text = "\u0002041C" + Conversions.ToString((int)cClearBuffer) + "E" + Conversions.ToString(iLabelId);
		if (lNumLabels > 0)
		{
			text = text + "Q" + Conversions.ToString(lNumLabels);
		}
		text += "\u0017";
		if (LabelFields.Count > 0)
		{
			text += "D";
			LABELFIELD lABELFIELD = default(LABELFIELD);
			foreach (object labelField in LabelFields)
			{
				text = text + ((labelField != null) ? ((LABELFIELD)labelField) : lABELFIELD).sValue + "\n";
			}
			text = Strings.Left(text, checked(Strings.Len(text) - 1));
		}
		text += "??\r";
		try
		{
			if (!thSendLabel.IsAlive)
			{
				Thread.Sleep(50);
				sBuffInToSend = text;
				thSendLabel = new Thread(ThreadSendLabelCmd41);
				thSendLabel.Start();
				result = true;
			}
			else
			{
				OnError?.Invoke(this, "", Common.Rm.GetString("Mectec4"), Common.ERROR_TYPE.Error);
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Mectec5") + ": '" + Conversions.ToString(iLabelId) + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public bool Init(string sPort, int iBauds, int iId485)
	{
		if (iId485 >= 0 || iId485 <= 31)
		{
			Id485 = iId485;
		}
		return Init(sPort, iBauds);
	}

	public bool Init(int iPort, string sIp, int iId485)
	{
		if (iId485 >= 0 || iId485 <= 31)
		{
			Id485 = iId485;
		}
		return Init(iPort, sIp);
	}

	public int GetCurrentError()
	{
		return (int)ErrorCode;
	}

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = sErrorId.ToString();
		switch (sErrorId)
		{
		case 0L:
			result.Desc = Common.Rm.GetString("Mectec6");
			result.Type = Common.ERROR_TYPE.NoError;
			break;
		case 1L:
			result.Desc = Common.Rm.GetString("Mectec7");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 2L:
			result.Desc = Common.Rm.GetString("Mectec8");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 3L:
			result.Desc = Common.Rm.GetString("Mectec9");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 4L:
			result.Desc = Common.Rm.GetString("Mectec10");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 12L:
			result.Desc = Common.Rm.GetString("Mectec11");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 16L:
			result.Desc = Common.Rm.GetString("Mectec12");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 23L:
			result.Desc = Common.Rm.GetString("Mectec13");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 805L:
			result.Desc = Common.Rm.GetString("Mectec14");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 806L:
			result.Desc = Common.Rm.GetString("Mectec15");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 818L:
			result.Desc = Common.Rm.GetString("Mectec16");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 816L:
			result.Desc = Common.Rm.GetString("Mectec17");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 801L:
			result.Desc = Common.Rm.GetString("Mectec18");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 812L:
			result.Desc = Common.Rm.GetString("Mectec19");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 990L:
			result.Desc = Common.Rm.GetString("Mectec20");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 992L:
			result.Desc = Common.Rm.GetString("Mectec21");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 998L:
			result.Desc = Common.Rm.GetString("Mectec22");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case -2L:
			result.Desc = Common.Rm.GetString("Mectec23");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("Mectec24");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}
}
