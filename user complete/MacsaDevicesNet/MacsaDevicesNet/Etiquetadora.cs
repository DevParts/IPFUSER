using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet;

public abstract class Etiquetadora : MacsaDevice
{
	public struct LABELFIELD
	{
		public string sKey;

		public string sValue;
	}

	public enum CODEPAGE_ENCODE
	{
		NoEncode = 0,
		OemUSA = 437,
		OemLatin1 = 850,
		AnsiCentral = 1250,
		eAnsiLatin1 = 1252,
		UTF8 = 65001,
		UTF16 = 1200,
		UTF32 = 12000
	}

	protected Collection LabelFields;

	protected bool _IsOnLine;

	public bool IsOnLine => _IsOnLine;

	public bool IsLabelRequested
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	protected abstract Common.DATA_ERROR GetDataError(int ErrorId);

	public Etiquetadora()
	{
		LabelFields = new Collection();
		_IsOnLine = false;
		IsLabelRequested = false;
		ClearLabelFields();
	}

	public bool GetLastOnline()
	{
		return IsOnLine;
	}

	public abstract bool SendLabel(string sPathLabel);

	public abstract bool GetStatus(WAIT_TYPE WaitType = WAIT_TYPE.Thread, long TimeToWait = 1000L);

	public void ClearLabelFields()
	{
		LabelFields.Clear();
	}

	public void AddLabelField(string sKey, string sVal)
	{
		LABELFIELD lABELFIELD = default(LABELFIELD);
		lABELFIELD.sKey = sKey;
		lABELFIELD.sValue = sVal;
		LabelFields.Add(lABELFIELD);
	}

	public bool CreateLabel(string PathModel, string PathNew, CODEPAGE_ENCODE Codepage = CODEPAGE_ENCODE.NoEncode, bool DeleteControlCmds = false)
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
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog(Common.Rm.GetString("String14") + " '" + PathModel + "'", TraceEventType.Error, ex2.Message);
			ProjectData.ClearProjectError();
			goto IL_0178;
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
			Common.MACSALog(Common.Rm.GetString("String15"), TraceEventType.Error, ex4.Message);
			ProjectData.ClearProjectError();
			goto IL_0178;
		}
		finally
		{
			streamWriter.Close();
		}
		result = true;
		goto IL_0178;
		IL_0178:
		return result;
	}

	public bool CreateAndSendLabel(string sPathModel, string sPathNew, CODEPAGE_ENCODE eCodepage = CODEPAGE_ENCODE.OemLatin1, bool bDeleteControlCmds = false)
	{
		bool result = false;
		if (CreateLabel(sPathModel, sPathNew, eCodepage, bDeleteControlCmds))
		{
			result = SendLabel(sPathNew);
		}
		return result;
	}

	public void RequestToSendLabel()
	{
		IsLabelRequested = true;
		GetStatus(WAIT_TYPE.Thread, 1000L);
	}

	public void SendString(string Data)
	{
		SendData(Data, 0L);
	}

	public Collection GetFields()
	{
		return LabelFields;
	}

	protected abstract string DeleteLabelPrinterCommands(string sBuff);

	protected string ReplaceField(string sBuffer, string sKey, string sVal)
	{
		string result = sBuffer;
		if (Operators.CompareString(sKey, "", TextCompare: false) != 0)
		{
			try
			{
				long num = Strings.InStr(sBuffer, sKey);
				if (num > 0)
				{
					string text = Strings.Replace(sBuffer, sKey, sVal);
					sBuffer = text;
				}
				result = sBuffer;
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog(Common.Rm.GetString("String16") + " '" + sKey + "' " + Common.Rm.GetString("String17") + " '" + sVal + "'.", TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
		return result;
	}

	protected string DeleteField(string sBuffer, string sFromWhere, string sToWhere)
	{
		string result = sBuffer;
		checked
		{
			if (!((Operators.CompareString(sFromWhere, "", TextCompare: false) == 0) | (Operators.CompareString(sToWhere, "", TextCompare: false) == 0)))
			{
				try
				{
					int num = Strings.InStr(sBuffer, sFromWhere);
					if (num > 0)
					{
						int num2 = Strings.InStr(num + 1, sBuffer, sToWhere);
						sBuffer = sBuffer.Remove(num, num2 - num);
					}
					result = sBuffer;
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					Common.MACSALog(Common.Rm.GetString("String18") + " '" + sFromWhere + "' " + Common.Rm.GetString("String19") + " '" + sToWhere + "'.", TraceEventType.Error, ex2.Message);
					ProjectData.ClearProjectError();
				}
			}
			return result;
		}
	}

	protected override void Finalize()
	{
		base.Finalize();
	}

	protected string GetFileContents(string FullPath, [Optional][DefaultParameterValue("")] ref string ErrInfo)
	{
		FileInfo fileInfo = new FileInfo(FullPath);
		string text = "";
		FileStream fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(fileStream);
		checked
		{
			byte[] array = new byte[(int)fileInfo.Length + 1];
			binaryReader.Read(array, 0, (int)fileInfo.Length);
			binaryReader.Close();
			fileStream.Close();
			return new string(Encoding.Default.GetChars(array));
		}
	}
}
