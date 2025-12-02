using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.InkJets;

public class FoxJetDuo
{
	public enum PRINTER_MODES
	{
		PM_STOP,
		PM_START,
		PM_PAUSE
	}

	public struct MSG_FIELD
	{
		public int Id;

		public string Valor;

		public int Len;
	}

	public delegate void OnErrorEventHandler(object sender, string ErrCode, string ErrDesc, Common.ERROR_TYPE ErrType);

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnThreadTimeOutEventHandler(object Sender);

	public delegate void OnInformationEventHandler(object sender, string Message);

	public delegate void OnReadyToReceiveDataEventHandler(object sender);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const string FJD_SELECT_MESSAGE = "/serial.cgi?idx=0&nme=<MSGNAME>.prd&net=1";

	private const string FJD_SET_VARDATA = "/serial.cgi?idx=0&nme=<MSGNAME>.prd&fst=<IDVAR>&lst=<IDVAR>&d<IDVAR>=<VALUE>&net=1";

	private const string FJD_GET_STATUS = "/status.html";

	private const string FJD_START_PRINT = "/print.cgi?idx=0&status=Start";

	private const string FJD_PAUSE_PRINT = "/print.cgi?idx=0&status=Pause";

	private const string FJD_CANCEL_PRINT = "/print.cgi?idx=0&status=Cancel";

	private const string FJD_RESET_COUNTER = "/print.cgi?idx=0&status=ResetCount";

	private const string FJD_SET_DATA_HEADER = "/serial.cgi?idx=0&nme=<MSGNAME>.prd&fst=1&lst=<QTY>&";

	private const string FJD_SET_DATA_BODY = "d<IDVAR>=<VALUE>&";

	private const string FJD_SET_DATA_END = "net=1";

	private int ReceptionState;

	private string RecepBuff;

	private List<MSG_FIELD> MsgFields;

	private string URI;

	private string _JetState;

	private string _Msj;

	public string JetState => _JetState;

	public bool IsDataSendRequested
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	[method: DebuggerNonUserCode]
	public event OnErrorEventHandler OnError;

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnThreadTimeOutEventHandler OnThreadTimeOut;

	[method: DebuggerNonUserCode]
	public event OnInformationEventHandler OnInformation;

	[method: DebuggerNonUserCode]
	public event OnReadyToReceiveDataEventHandler OnReadyToReceiveData;

	public FoxJetDuo()
	{
		__ENCAddToList(this);
		ReceptionState = 0;
		MsgFields = new List<MSG_FIELD>();
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

	public bool Init(string sIp)
	{
		URI = "http://" + sIp;
		try
		{
			if (MyProject.Computer.Network.Ping(sIp))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
		return false;
	}

	public bool RequestToSendData()
	{
		IsDataSendRequested = true;
		return GetStatus();
	}

	public bool GetStatus()
	{
		string uRL = URI + "/status.html";
		string uRLData = GetURLData(uRL);
		checked
		{
			if (Operators.CompareString(uRLData, "", TextCompare: false) != 0)
			{
				int num = uRLData.IndexOf("<!--ifc");
				int num2 = uRLData.IndexOf("-->", num);
				string text = uRLData.Substring(num + 7, num2 - num - 7);
				string[] array = text.Split('\t');
				int startIndex = uRLData.IndexOf("-->");
				startIndex = uRLData.IndexOf("<td>", startIndex);
				startIndex += 4;
				startIndex = uRLData.IndexOf("<td>", startIndex);
				startIndex += 4;
				int num3 = uRLData.IndexOf("</td>", startIndex);
				_JetState = uRLData.Substring(startIndex, num3 - startIndex);
				if (((Conversions.ToInteger(array[7]) & 0x4000) == 16384) | ((Conversions.ToInteger(array[7]) & 0x8000) == 32768))
				{
					if ((Conversions.ToInteger(array[7]) & 8) == 8)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo2"), Common.ERROR_TYPE.Error);
					}
					else if ((Conversions.ToInteger(array[7]) & 0x400) == 1024)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo4"), Common.ERROR_TYPE.Error);
					}
					else if ((Conversions.ToInteger(array[7]) & 0x800) == 2048)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo5"), Common.ERROR_TYPE.Error);
					}
					else if ((Conversions.ToInteger(array[7]) & 0x1000) == 4096)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo6"), Common.ERROR_TYPE.Error);
					}
					else if ((Conversions.ToInteger(array[7]) & 0x2000) == 8192)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo7"), Common.ERROR_TYPE.Error);
					}
					else if ((Conversions.ToInteger(array[7]) & 0x10) == 16)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo3"), Common.ERROR_TYPE.Warning);
					}
					else if ((Conversions.ToInteger(array[7]) & 1) == 1)
					{
						OnError?.Invoke(this, array[7], Common.Rm.GetString("FoxjetDuo1"), Common.ERROR_TYPE.Warning);
					}
					else
					{
						OnLine?.Invoke(this);
						if (IsDataSendRequested)
						{
							OnReadyToReceiveData?.Invoke(this);
							IsDataSendRequested = false;
						}
					}
				}
				else
				{
					OnLine?.Invoke(this);
					if (IsDataSendRequested)
					{
						OnReadyToReceiveData?.Invoke(this);
						IsDataSendRequested = false;
					}
				}
				return true;
			}
			OnThreadTimeOut?.Invoke(this);
			return false;
		}
	}

	public void SelectLabel(string LabelName)
	{
		_Msj = LabelName;
	}

	public void ResetCounter()
	{
		string uRL = URI + "/print.cgi?idx=0&status=ResetCount";
		GetURLData(uRL);
	}

	public void SetVariable(int VariableId, string Value)
	{
		string text = URI + "/serial.cgi?idx=0&nme=<MSGNAME>.prd&fst=<IDVAR>&lst=<IDVAR>&d<IDVAR>=<VALUE>&net=1";
		text = text.Replace("<MSGNAME>", _Msj);
		text = text.Replace("<IDVAR>", VariableId.ToString());
		text = text.Replace("<VALUE>", Value);
		GetURLData(text);
	}

	public void SetPrinterMode(PRINTER_MODES Mode)
	{
		string uRL = default(string);
		switch ((int)Mode)
		{
		case 0:
			uRL = URI + "/print.cgi?idx=0&status=Cancel";
			break;
		case 2:
			uRL = URI + "/print.cgi?idx=0&status=Pause";
			break;
		case 1:
			uRL = URI + "/print.cgi?idx=0&status=Start";
			break;
		}
		GetURLData(uRL);
	}

	public void SetAllVariables()
	{
		string text = URI + "/serial.cgi?idx=0&nme=<MSGNAME>.prd&fst=1&lst=<QTY>&";
		text = text.Replace("<MSGNAME>", _Msj);
		text = text.Replace("<QTY>", MsgFields.Count.ToString());
		checked
		{
			int num = MsgFields.Count - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				MSG_FIELD mSG_FIELD = MsgFields[num2];
				text += "d<IDVAR>=<VALUE>&";
				text = text.Replace("<IDVAR>", mSG_FIELD.Id.ToString());
				text = text.Replace("<VALUE>", mSG_FIELD.Valor.PadLeft(mSG_FIELD.Len - mSG_FIELD.Valor.Length, ' '));
				num2++;
			}
			text += "net=1";
			GetURLData(text);
		}
	}

	public void ClearFields()
	{
		MsgFields.Clear();
	}

	public void AddField(int IdField, string Value, int FieldLength)
	{
		MSG_FIELD item = default(MSG_FIELD);
		item.Id = IdField;
		item.Valor = EncodeStringToURL(Value);
		if (item.Valor.Length > FieldLength)
		{
			FieldLength = item.Valor.Length;
		}
		item.Len = FieldLength;
		MsgFields.Add(item);
	}

	private string EncodeStringToURL(string Text)
	{
		string text = "";
		checked
		{
			int num = Text.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text = ((!((Strings.Asc(Text[num2]) < 128) & (Strings.Asc(Text[num2]) > 32))) ? ((Operators.CompareString(Conversions.ToString(Text[num2]), "Ñ", TextCompare: false) != 0) ? ((Operators.CompareString(Conversions.ToString(Text[num2]), "ñ", TextCompare: false) != 0) ? (text + "%" + Conversion.Hex(Strings.Asc(Text[num2])).PadLeft(2, '0').ToUpper()) : (text + "%F1")) : (text + "%D1")) : (text + Conversions.ToString(Text[num2])));
				num2++;
			}
			return text;
		}
	}

	private string GetURLData(string URL)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
		httpWebRequest.MaximumAutomaticRedirections = 4;
		httpWebRequest.MaximumResponseHeadersLength = 4;
		httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
		string result;
		try
		{
			HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			Console.WriteLine("Content length is {0}", httpWebResponse.ContentLength);
			Console.WriteLine("Content type is {0}", httpWebResponse.ContentType);
			Stream responseStream = httpWebResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
			Console.WriteLine("Response stream received.");
			string text = streamReader.ReadToEnd();
			Console.WriteLine(text);
			httpWebResponse.Close();
			streamReader.Close();
			result = text;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			OnThreadTimeOut?.Invoke(this);
			result = "";
			ProjectData.ClearProjectError();
		}
		return result;
	}
}
