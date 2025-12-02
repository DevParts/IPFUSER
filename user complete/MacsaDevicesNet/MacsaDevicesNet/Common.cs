#define TRACE
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MacsaDevicesNet.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic.Logging;

namespace MacsaDevicesNet;

[StandardModule]
public sealed class Common
{
	public enum ERROR_TYPE
	{
		NoError,
		Error,
		Warning
	}

	public enum WAIT_TYPE
	{
		NoWait,
		Activa,
		Thread
	}

	public struct DATA_ERROR
	{
		public string Id;

		public string Desc;

		public ERROR_TYPE Type;
	}

	private static FileLogTraceListener FileLogListener;

	private static string LoggerPath = "";

	private static string _Language = "ES";

	public static ResourceManager Rm;

	public static string Language
	{
		get
		{
			return _Language;
		}
		set
		{
			_Language = value;
			SetMessages();
		}
	}

	public static string GetModuleName()
	{
		Assembly callingAssembly = Assembly.GetCallingAssembly();
		string[] array = Strings.Split(callingAssembly.FullName, ",");
		return array[0];
	}

	public static void Wait(long lTime)
	{
		long num = MyProject.Computer.Clock.TickCount;
		while (checked(MyProject.Computer.Clock.TickCount - num) < lTime)
		{
			Application.DoEvents();
		}
	}

	public static string GetLibraryVersion()
	{
		return Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}

	public static void MACSALog(string sText, TraceEventType tEventType, string sExMsg = "")
	{
		try
		{
			if (FileLogListener == null)
			{
				FileLogListener = new FileLogTraceListener();
				FileLogListener.CustomLocation = Application.StartupPath + "\\Logs";
				FileLogListener.Location = LogFileLocation.Custom;
				FileLogListener.LogFileCreationSchedule = LogFileCreationScheduleOption.Daily;
				FileLogListener.AutoFlush = true;
				FileLogListener.TraceOutputOptions = TraceOptions.DateTime;
				LoggerPath = FileLogListener.FullLogFileName;
				Trace.Listeners.Add(FileLogListener);
			}
			Trace.WriteLine(Strings.Format(DateAndTime.Now, "HH:mm:ss - ") + tEventType.ToString() + ": " + sText);
			if (Information.Err().Number != 0)
			{
				Trace.WriteLine("\t" + Rm.GetString("String20") + " " + Conversions.ToString(Information.Err().Number) + ":" + Information.Err().Description);
			}
			if (Operators.CompareString(sExMsg, "", TextCompare: false) != 0)
			{
				Trace.WriteLine("\t" + Rm.GetString("String21") + ": '" + sExMsg + "'");
			}
			FileLogListener.Dispose();
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
	}

	public static string GetLogListenerFileName()
	{
		return LoggerPath;
	}

	public static string ConvertStringUsingCodeTable(int TableNumber, string SrcString)
	{
		string text = "";
		byte[] bytes = Encoding.Default.GetBytes(SrcString);
		byte[] array = Encoding.Convert(Encoding.Default, Encoding.GetEncoding(TableNumber), bytes);
		checked
		{
			int num = array.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text += Conversions.ToString(Strings.Chr(array[num2]));
				num2++;
			}
			return text;
		}
	}

	public static byte[] ConvertStringToByteArray(string sData)
	{
		checked
		{
			byte[] array = new byte[Strings.Len(sData) - 1 + 1];
			int num = Strings.Len(sData);
			int num2 = 1;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				array[num2 - 1] = (byte)Strings.Asc(Strings.Mid(sData, num2, 1));
				num2++;
			}
			return array;
		}
	}

	public static void ConvertLabelEncoding(string File1, Encoding Encoding1, string File2, Encoding Encoding2)
	{
		FileStream stream = new FileStream(File1, FileMode.Open);
		StreamReader streamReader = new StreamReader(stream, Encoding1);
		FileStream stream2 = new FileStream(File2, FileMode.Create);
		StreamWriter streamWriter = new StreamWriter(stream2, Encoding2);
		string text = streamReader.ReadToEnd();
		text = text.Replace("UTF-16", "UTF-8");
		streamWriter.Write(text);
		streamWriter.Close();
		streamReader.Close();
	}

	public static string CalculaControlSSCC(string sCodi)
	{
		int num = 3;
		int num2 = Strings.Len(sCodi);
		checked
		{
			int num5 = default(int);
			while (true)
			{
				int num3 = num2;
				int num4 = 1;
				if (num3 < num4)
				{
					break;
				}
				num5 += Conversions.ToInteger(sCodi.Substring(num2 - 1, 1)) * num;
				num = ((num == 3) ? 1 : 3);
				num2 += -1;
			}
			int num6 = unchecked(num5 % 10);
			num6 = ((num6 != 0) ? (10 - num6) : 0);
			if (num6 == 10)
			{
				num6 = 0;
			}
			return sCodi + Strings.Format(num6);
		}
	}

	public static bool PrevInstance(string ProcessName)
	{
		Process[] processesByName = Process.GetProcessesByName(ProcessName);
		if (processesByName.Length <= 1)
		{
			return false;
		}
		return true;
	}

	public static void SetMessages()
	{
		string language = _Language;
		if (Operators.CompareString(language, "ES", TextCompare: false) == 0)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
			Rm = new ResourceManager("MacsaDevicesNet.Messages", Assembly.GetExecutingAssembly());
		}
		else if (Operators.CompareString(language, "EN", TextCompare: false) == 0)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			Rm = new ResourceManager("MacsaDevicesNet.Messages", Assembly.GetExecutingAssembly());
		}
		else
		{
			Rm = new ResourceManager("MacsaDevicesNet.Messages", Assembly.GetExecutingAssembly());
		}
	}
}
