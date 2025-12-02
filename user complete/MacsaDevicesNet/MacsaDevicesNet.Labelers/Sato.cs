using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Labelers;

public class Sato : Etiquetadora
{
	public enum PROTOCOL_TYPE
	{
		M8400,
		STANDARD
	}

	public enum SATO_ERRORCODES
	{
		Noerror,
		PaperOut,
		PauseOn,
		BufferFull,
		CorruptRam,
		HeadUp,
		RibbonOut,
		LabelWaiting,
		LabelsInBatch,
		HeaderError,
		OffLine,
		UnKnown,
		NotConnected,
		MediaError,
		SensorError
	}

	public delegate void OnErrorEventHandler(object sender, string sErrCode, string sErrDesc, Common.ERROR_TYPE tErrType);

	public delegate void OnlineEventHandler(object sender);

	public delegate void OnReadyToReceiveLabelEventHandler(object sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string CMD_STATUS = "\u0005";

	private const string CMD_RESET = "\u0018";

	private SATO_ERRORCODES ErrorCode;

	private PROTOCOL_TYPE _ProtocolType;

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

	public PROTOCOL_TYPE ProtocolType
	{
		get
		{
			return _ProtocolType;
		}
		set
		{
			_ProtocolType = value;
		}
	}

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
					Common.MACSALog(Common.Rm.GetString("Sato20") + ": '" + sPathLabel + "'", TraceEventType.Error, ex2.Message);
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
		if (!SendData("\u0005", lTimeToWait, tWaitType))
		{
			OnError?.Invoke(this, "", Common.Rm.GetString("String1"), Common.ERROR_TYPE.Error);
			result = false;
		}
		return result;
	}

	public bool DoSoftReset()
	{
		return SendData("\u0018", 0L);
	}

	protected override Common.DATA_ERROR GetDataError(int sErrorId)
	{
		Common.DATA_ERROR result = default(Common.DATA_ERROR);
		result.Id = sErrorId.ToString();
		long num = sErrorId;
		if (num > 14 || num < 0)
		{
			goto IL_025d;
		}
		switch ((int)num)
		{
		case 0:
			break;
		case 1:
			goto IL_0092;
		case 2:
			goto IL_00b6;
		case 3:
			goto IL_00da;
		case 4:
			goto IL_00fe;
		case 5:
			goto IL_0122;
		case 6:
			goto IL_0146;
		case 7:
			goto IL_016a;
		case 8:
			goto IL_018e;
		case 9:
			goto IL_01b2;
		case 10:
			goto IL_01d6;
		case 13:
			goto IL_01fa;
		case 14:
			goto IL_021b;
		case 12:
			goto IL_023c;
		default:
			goto IL_025d;
		}
		result.Desc = Common.Rm.GetString("Sato1");
		result.Type = Common.ERROR_TYPE.NoError;
		goto IL_027c;
		IL_021b:
		result.Desc = Common.Rm.GetString("Sato13");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_025d:
		result.Desc = Common.Rm.GetString("Sato15");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_027c:
		return result;
		IL_0092:
		result.Desc = Common.Rm.GetString("Sato2");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_00b6:
		result.Desc = Common.Rm.GetString("Sato3");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_00da:
		result.Desc = Common.Rm.GetString("Sato4");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_00fe:
		result.Desc = Common.Rm.GetString("Sato5");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_0122:
		result.Desc = Common.Rm.GetString("Sato6");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_0146:
		result.Desc = Common.Rm.GetString("Sato7");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_016a:
		result.Desc = Common.Rm.GetString("Sato8");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_018e:
		result.Desc = Common.Rm.GetString("Sato9");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_01b2:
		result.Desc = Common.Rm.GetString("Sato10");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_01d6:
		result.Desc = Common.Rm.GetString("Sato11");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_023c:
		result.Desc = Common.Rm.GetString("Sato14");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
		IL_01fa:
		result.Desc = Common.Rm.GetString("Sato12");
		result.Type = Common.ERROR_TYPE.Error;
		goto IL_027c;
	}

	public bool WriteRFIDDataIntoLabel(string sFileFrom, string sFileTo, int iPosX, int iPosY, int iRot, string sCodeRFIDtoWrite)
	{
		bool flag = false;
		Common.MACSALog(Common.Rm.GetString("Sato19"), TraceEventType.Information);
		string text = default(string);
		if (iPosX > 0 && iPosY > 0)
		{
			text = "\u001b%" + Conversions.ToString(iRot);
			text = text + "\u001bV" + Conversions.ToString(iPosY) + "\u001bH" + Conversions.ToString(iPosX) + "\u001bTM1";
		}
		string text2 = "\u001bIP0" + sCodeRFIDtoWrite;
		string text3;
		try
		{
			StreamReader streamReader = new StreamReader(sFileFrom);
			text3 = streamReader.ReadToEnd();
			streamReader.Close();
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("Sato17") + "Ruta = " + sFileFrom, TraceEventType.Critical, ex2.Message);
			ProjectData.ClearProjectError();
			bool result = default(bool);
			return result;
		}
		int num = Strings.InStrRev(text3, "\u001bA");
		if (num > 0)
		{
			num = checked(num + 1);
			text3 = text3.Insert(num, text2 + text);
		}
		StreamWriter streamWriter = new StreamWriter(sFileTo, append: false);
		streamWriter.Write(text3);
		streamWriter.Close();
		flag = true;
		Common.MACSALog(Common.Rm.GetString("Sato18"), TraceEventType.Information);
		return flag;
	}

	public Sato()
	{
		__ENCAddToList(this);
		ErrorCode = SATO_ERRORCODES.Noerror;
	}

	private void mComm_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		int num = ((_ProtocolType != PROTOCOL_TYPE.M8400) ? 11 : 7);
		sBuffIn += SerialComm.ReadExisting();
		if (Strings.Len(sBuffIn) >= num)
		{
			AnswerRecv = true;
			ProcessInputData(sBuffIn);
			sBuffIn = "";
		}
	}

	private void ProcessInputData(string sBuffIn)
	{
		try
		{
			if (CheckStatus(sBuffIn))
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
			Common.MACSALog(Common.Rm.GetString("Sato16") + ": '" + sBuffIn + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	private bool CheckStatus(string sSts)
	{
		bool result = true;
		if (_ProtocolType == PROTOCOL_TYPE.M8400)
		{
			switch (sSts.Substring(3, 1))
			{
			case "E":
				ErrorCode = SATO_ERRORCODES.HeadUp;
				break;
			case "@":
				ErrorCode = SATO_ERRORCODES.RibbonOut;
				break;
			case "A":
				ErrorCode = SATO_ERRORCODES.PaperOut;
				break;
			case "G":
				ErrorCode = SATO_ERRORCODES.HeaderError;
				break;
			case "1":
				ErrorCode = SATO_ERRORCODES.OffLine;
				break;
			default:
				ErrorCode = SATO_ERRORCODES.Noerror;
				result = false;
				break;
			}
		}
		else
		{
			string text = sSts.Substring(3, 1);
			if ((Operators.CompareString(text, "0", TextCompare: false) >= 0 && Operators.CompareString(text, "3", TextCompare: false) <= 0) ? true : false)
			{
				ErrorCode = SATO_ERRORCODES.OffLine;
			}
			else
			{
				switch (text)
				{
				case "b":
					ErrorCode = SATO_ERRORCODES.HeadUp;
					break;
				case "c":
					ErrorCode = SATO_ERRORCODES.PaperOut;
					break;
				case "d":
					ErrorCode = SATO_ERRORCODES.RibbonOut;
					break;
				case "g":
					ErrorCode = SATO_ERRORCODES.HeaderError;
					break;
				case "e":
					ErrorCode = SATO_ERRORCODES.MediaError;
					break;
				case "f":
					ErrorCode = SATO_ERRORCODES.SensorError;
					break;
				default:
					if ((Operators.CompareString(text, "i", TextCompare: false) >= 0 && Operators.CompareString(text, "k", TextCompare: false) <= 0) ? true : false)
					{
						ErrorCode = SATO_ERRORCODES.UnKnown;
						break;
					}
					if (Operators.CompareString(sSts.Substring(4, 6), "000000", TextCompare: false) != 0)
					{
						ErrorCode = SATO_ERRORCODES.BufferFull;
					}
					else
					{
						ErrorCode = SATO_ERRORCODES.HeaderError;
					}
					result = false;
					break;
				}
			}
		}
		return result;
	}

	protected override string DeleteLabelPrinterCommands(string sBuffIn)
	{
		sBuffIn = DeleteField(sBuffIn, "\u001bCS", "\u001b");
		sBuffIn = DeleteField(sBuffIn, "\u001bA3", "\u001b");
		sBuffIn = DeleteField(sBuffIn, "\u001bPM", "\u001b");
		sBuffIn = DeleteField(sBuffIn, "\u001bPH", "\u001b");
		sBuffIn = DeleteField(sBuffIn, "\u001bPO", "\u001b");
		return sBuffIn;
	}

	protected override void TcpComm_DataReceived(string sData)
	{
		sBuffIn += sData;
		switch ((int)_ProtocolType)
		{
		case 0:
		{
			int num3 = 7;
			if (Strings.Len(sBuffIn) >= num3)
			{
				AnswerRecv = true;
				ProcessInputData(sBuffIn);
				sBuffIn = "";
			}
			break;
		}
		case 1:
		{
			int num = sBuffIn.IndexOf('\u0002');
			if (num >= 0)
			{
				int num2 = sBuffIn.IndexOf('\u0003', num);
				if (num2 > 0)
				{
					AnswerRecv = true;
					ProcessInputData(sBuffIn.Substring(num, checked(num2 - num)));
					sBuffIn = "";
				}
			}
			break;
		}
		}
	}
}
