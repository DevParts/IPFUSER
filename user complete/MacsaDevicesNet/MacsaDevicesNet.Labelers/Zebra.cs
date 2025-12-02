using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class Zebra : Etiquetadora
{
	public enum PEELOFF_TYPE
	{
		No,
		Yes
	}

	public enum ZEBRA_ERRORCODES
	{
		Noerror = 0,
		UnKnown = -1,
		PaperOut = 1,
		PauseOn = 2,
		BufferFull = 3,
		CorruptRam = 4,
		HeadUp = 5,
		RibbonOut = 6,
		LabelWaiting = 7,
		LabelsInBatch = 8,
		HeaderError = 9,
		Offline = 10,
		NotConnected = 11
	}

	public delegate void OnErrorEventHandler(object sender, string ErrCode, string ErrDesc, Common.ERROR_TYPE ErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string CMD_STATUS = "~HS";

	private const string CMD_RESET = "~JA";

	private ZEBRA_ERRORCODES ErrorCode;

	private PEELOFF_TYPE PeelOff;

	private long _FormatsInBufferCount;

	private long _LabelsInBufferCount;

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

	public long FormatsInBufferCount => _FormatsInBufferCount;

	public long LabelsInBufferCount => _LabelsInBufferCount;

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnlineEventHandler Online;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveLabelEventHandler OnReadyToReceiveLabel;

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
					Common.MACSALog(Common.Rm.GetString("Zebra1") + " '" + sPathLabel + "'", TraceEventType.Error, ex2.Message);
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
		FlushComm();
		_IsOnLine = false;
		bool result = true;
		sBuffIn = "";
		if (!SendData("~HS", lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public bool DoSoftReset()
	{
		sBuffIn = "";
		return SendData("~JA", 0L);
	}

	protected override Common.DATA_ERROR GetDataError(int ErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = ErrorId.ToString();
		switch (ErrorId)
		{
		case 0:
			result.Desc = Common.Rm.GetString("Zebra2");
			result.Type = Common.ERROR_TYPE.NoError;
			break;
		case 1:
			result.Desc = Common.Rm.GetString("Zebra3");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 2:
			result.Desc = Common.Rm.GetString("Zebra4");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 3:
			result.Desc = Common.Rm.GetString("Zebra5");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 4:
			result.Desc = Common.Rm.GetString("Zebra6");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 5:
			result.Desc = Common.Rm.GetString("Zebra7");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 6:
			result.Desc = Common.Rm.GetString("Zebra8");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 7:
			result.Desc = Common.Rm.GetString("Zebra9");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 8:
			result.Desc = Common.Rm.GetString("Zebra10");
			result.Type = Common.ERROR_TYPE.Warning;
			break;
		case 9:
			result.Desc = Common.Rm.GetString("Zebra11");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 10:
			result.Desc = Common.Rm.GetString("Zebra12");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		case 11:
			result.Desc = Common.Rm.GetString("Zebra13");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		default:
			result.Desc = Common.Rm.GetString("Zebra14");
			result.Type = Common.ERROR_TYPE.Error;
			break;
		}
		return result;
	}

	public bool CreateLabel(string PathModel, string PathNew, long NumLabels = 1L, CODEPAGE_ENCODE Codepage = CODEPAGE_ENCODE.NoEncode, bool DeleteControlCmds = false)
	{
		bool result = false;
		string text = MyProject.Computer.FileSystem.ReadAllText(PathModel);
		string text2;
		try
		{
			StreamReader streamReader = ((Codepage == CODEPAGE_ENCODE.NoEncode) ? new StreamReader(PathModel) : new StreamReader(PathModel, Encoding.GetEncoding((int)Codepage)));
			text2 = streamReader.ReadToEnd();
			streamReader.Close();
			LABELFIELD lABELFIELD2 = default(LABELFIELD);
			foreach (object labelField in LabelFields)
			{
				LABELFIELD lABELFIELD = ((labelField != null) ? ((LABELFIELD)labelField) : lABELFIELD2);
				text2 = ReplaceField(text2, lABELFIELD.sKey, lABELFIELD.sValue);
			}
			if (NumLabels > 0)
			{
				text2 = ReplaceField(text2, "^PQ1", "^PQ" + Conversions.ToString(NumLabels));
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Zebra16") + " '" + PathModel + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
			goto IL_01a5;
		}
		StreamWriter streamWriter = default(StreamWriter);
		try
		{
			streamWriter = ((Codepage == CODEPAGE_ENCODE.NoEncode) ? new StreamWriter(PathNew, append: false) : new StreamWriter(PathNew, append: false, Encoding.GetEncoding((int)Codepage)));
			streamWriter.Write(text2);
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			Common.MACSALog(Common.Rm.GetString("Zebra15"), TraceEventType.Error, ex4.Message);
			ProjectData.ClearProjectError();
			goto IL_01a5;
		}
		finally
		{
			streamWriter.Close();
		}
		result = true;
		goto IL_01a5;
		IL_01a5:
		return result;
	}

	public bool CreateAndSendLabel(string PathModel, string PathNew, long NumLabels = 1L, CODEPAGE_ENCODE Codepage = CODEPAGE_ENCODE.OemLatin1, bool DeleteControlCmds = false)
	{
		bool result = false;
		if (CreateLabel(PathModel, PathNew, Codepage, DeleteControlCmds))
		{
			result = SendLabel(PathNew);
		}
		return result;
	}

	public Zebra()
	{
		__ENCAddToList(this);
		ErrorCode = ZEBRA_ERRORCODES.Noerror;
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		sBuffIn += SerialComm.ReadExisting();
		if (Strings.Len(sBuffIn) >= 82)
		{
			AnswerRecv = true;
			ProcessInputData(sBuffIn);
			sBuffIn = "";
		}
	}

	private void ProcessInputData(string sBuff)
	{
		try
		{
			if (CheckStatus(sBuff))
			{
				Common.DATA_ERROR dataError = GetDataError((int)ErrorCode);
				_IsOnLine = false;
				OnError?.Invoke(this, dataError.Id, dataError.Desc, dataError.Type);
				return;
			}
			_IsOnLine = true;
			Online?.Invoke(this);
			if (IsLabelRequested)
			{
				OnReadyToReceiveLabel?.Invoke(this);
				IsLabelRequested = false;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Zebra17") + " '" + sBuff + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private bool CheckStatus(string sSts)
	{
		bool result = true;
		_FormatsInBufferCount = Conversions.ToLong(Strings.Mid(sSts, 15, 3));
		_LabelsInBufferCount = Conversions.ToLong(Strings.Mid(sSts, 56, 8));
		if (Operators.CompareString(Strings.Mid(sSts, 6, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.PaperOut;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 19, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.BufferFull;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 29, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.CorruptRam;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 44, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.HeadUp;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 46, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.RibbonOut;
		}
		else if ((PeelOff == PEELOFF_TYPE.Yes) & (Operators.CompareString(Strings.Mid(sSts, 54, 1), "1", TextCompare: false) == 0))
		{
			ErrorCode = ZEBRA_ERRORCODES.LabelWaiting;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 56, 8), "00000000", TextCompare: false) != 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.LabelsInBatch;
		}
		else if (Operators.CompareString(Strings.Mid(sSts, 8, 1), "1", TextCompare: false) == 0)
		{
			ErrorCode = ZEBRA_ERRORCODES.PauseOn;
		}
		else
		{
			ErrorCode = ZEBRA_ERRORCODES.Noerror;
			result = false;
		}
		return result;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		if (Strings.Len(sBuffIn) >= 82)
		{
			AnswerRecv = true;
			ProcessInputData(sBuffIn);
			sBuffIn = "";
		}
	}

	protected override string DeleteLabelPrinterCommands(string sBuff)
	{
		return sBuff;
	}
}
