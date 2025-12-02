using System;
using Advantech.Common;

namespace Advantech.Adam;

public class Counter : AdamBase
{
	private static byte MODE_MASK = 15;

	private static byte RECORD_LAST_COUNT = 32;

	private static byte ENABLE_DIGITAL_FILTER = 64;

	private static byte ENABLE_INVERT_MODE = 128;

	public Counter(AdamCom i_com)
		: base(i_com)
	{
	}

	public Counter(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	public static string GetModeName(Adam4000Type adam4000Type, byte i_byMode)
	{
		string result = "";
		Adam4080_CounterMode adam4080_CounterMode = (Adam4080_CounterMode)i_byMode;
		if (adam4000Type == Adam4000Type.Adam4080 || adam4000Type == Adam4000Type.Adam4080D)
		{
			switch (adam4080_CounterMode)
			{
			case Adam4080_CounterMode.Counter:
				result = "Counter";
				break;
			case Adam4080_CounterMode.Frequency:
				result = "Frequency";
				break;
			}
		}
		return result;
	}

	public static int GetChannelTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		if (adam4000Type == Adam4000Type.Adam4080 || adam4000Type == Adam4000Type.Adam4080D)
		{
			result = 2;
		}
		return result;
	}

	public static int GetModeTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		if (adam4000Type == Adam4000Type.Adam4080 || adam4000Type == Adam4000Type.Adam4080D)
		{
			result = 2;
		}
		return result;
	}

	public static string GetFormat(Adam4000Type adam4000Type, byte i_byMode)
	{
		string result = "";
		if (adam4000Type == Adam4000Type.Adam4080 || adam4000Type == Adam4000Type.Adam4080D)
		{
			Adam4080_CounterMode adam4080_CounterMode = (Adam4080_CounterMode)i_byMode;
			if (adam4080_CounterMode == Adam4080_CounterMode.Frequency)
			{
				result = "0.00";
			}
		}
		return result;
	}

	public static string GetUnitName(Adam4000Type adam4000Type, byte i_byMode)
	{
		string result = "";
		if (adam4000Type == Adam4000Type.Adam4080 || adam4000Type == Adam4000Type.Adam4080D)
		{
			Adam4080_CounterMode adam4080_CounterMode = (Adam4080_CounterMode)i_byMode;
			result = ((adam4080_CounterMode != Adam4080_CounterMode.Frequency) ? "counts" : "Hz");
		}
		return result;
	}

	protected static string GetAdam5080ModeName(byte i_byMode)
	{
		Adam5080_CounterMode adam5080_CounterMode = (Adam5080_CounterMode)i_byMode;
		string result = "";
		switch (adam5080_CounterMode)
		{
		case Adam5080_CounterMode.Bi_Direction:
			result = "Bi-Directory";
			break;
		case Adam5080_CounterMode.Up_Down:
			result = "Up/Down";
			break;
		case Adam5080_CounterMode.Frequency:
			result = "Frequency";
			break;
		}
		return result;
	}

	protected static string GetAdam5081ModeName(byte i_byMode)
	{
		Adam5081_CounterMode adam5081_CounterMode = (Adam5081_CounterMode)i_byMode;
		string result = "";
		switch (adam5081_CounterMode)
		{
		case Adam5081_CounterMode.Bi_Direction:
			result = "Bi-Directory";
			break;
		case Adam5081_CounterMode.Up_Down:
			result = "Up/Down";
			break;
		case Adam5081_CounterMode.Up:
			result = "Up";
			break;
		case Adam5081_CounterMode.Frequency:
			result = "Frequency";
			break;
		case Adam5081_CounterMode.AB1X:
			result = "A/B-1X";
			break;
		case Adam5081_CounterMode.AB2X:
			result = "A/B-2X";
			break;
		case Adam5081_CounterMode.AB4X:
			result = "A/B-4X";
			break;
		}
		return result;
	}

	protected static double GetAdam5080ScaledValue(byte i_byMode, int i_iHigh, int i_iLow)
	{
		double result = 0.0;
		switch ((Adam5080_CounterMode)i_byMode)
		{
		case Adam5080_CounterMode.Bi_Direction:
		case Adam5080_CounterMode.Up_Down:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num);
			break;
		}
		case Adam5080_CounterMode.Frequency:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num) / 100.0;
			break;
		}
		}
		return result;
	}

	protected static double GetAdam5081ScaledValue(byte i_byMode, int i_iHigh, int i_iLow)
	{
		double result = 0.0;
		switch ((Adam5081_CounterMode)i_byMode)
		{
		case Adam5081_CounterMode.Bi_Direction:
		case Adam5081_CounterMode.Up_Down:
		case Adam5081_CounterMode.Up:
		case Adam5081_CounterMode.AB1X:
		case Adam5081_CounterMode.AB2X:
		case Adam5081_CounterMode.AB4X:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num);
			break;
		}
		case Adam5081_CounterMode.Frequency:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num);
			break;
		}
		}
		return result;
	}

	public static double GetScaledValue(Adam5000Type adam5000Type, byte i_byMode, int i_iHigh, int i_iLow)
	{
		double result = 0.0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
			result = GetAdam5080ScaledValue(i_byMode, i_iHigh, i_iLow);
			break;
		case Adam5000Type.Adam5081:
			result = GetAdam5081ScaledValue(i_byMode, i_iHigh, i_iLow);
			break;
		}
		return result;
	}

	public static string GetModeName(Adam5000Type adam5000Type, byte i_byMode)
	{
		string result = "";
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
			result = GetAdam5080ModeName(i_byMode);
			break;
		case Adam5000Type.Adam5081:
			result = GetAdam5081ModeName(i_byMode);
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
			result = 4;
			break;
		case Adam5000Type.Adam5081:
			result = 8;
			break;
		}
		return result;
	}

	public static int GetModeTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
			result = 3;
			break;
		case Adam5000Type.Adam5081:
			result = 7;
			break;
		}
		return result;
	}

	public static string GetFormat(Adam5000Type adam5000Type, byte i_byMode)
	{
		string result = "";
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
		{
			Adam5080_CounterMode adam5080_CounterMode = (Adam5080_CounterMode)i_byMode;
			if (adam5080_CounterMode == Adam5080_CounterMode.Frequency)
			{
				result = "0.00";
			}
			break;
		}
		case Adam5000Type.Adam5081:
		{
			Adam5081_CounterMode adam5081_CounterMode = (Adam5081_CounterMode)i_byMode;
			if (adam5081_CounterMode == Adam5081_CounterMode.Frequency)
			{
				result = "0";
			}
			break;
		}
		}
		return result;
	}

	public static string GetUnitName(Adam5000Type adam5000Type, byte i_byMode)
	{
		string result = "";
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5080:
		{
			Adam5080_CounterMode adam5080_CounterMode = (Adam5080_CounterMode)i_byMode;
			result = ((adam5080_CounterMode != Adam5080_CounterMode.Frequency) ? "counts" : "Hz");
			break;
		}
		case Adam5000Type.Adam5081:
		{
			Adam5081_CounterMode adam5081_CounterMode = (Adam5081_CounterMode)i_byMode;
			result = ((adam5081_CounterMode != Adam5081_CounterMode.Frequency) ? "counts" : "Hz");
			break;
		}
		}
		return result;
	}

	public static int GetDataFormatTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		if (adam5000Type == Adam5000Type.Adam5080)
		{
			result = 2;
		}
		return result;
	}

	public static byte GetDataFormatCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		if (adam5000Type == Adam5000Type.Adam5080)
		{
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 2;
			}
		}
		return byte.MaxValue;
	}

	public static int GetDataFormatIndex(Adam5000Type adam5000Type, byte i_byDataFormat)
	{
		Adam5000_DataFormat adam5000_DataFormat = (Adam5000_DataFormat)i_byDataFormat;
		if (adam5000Type == Adam5000Type.Adam5080)
		{
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				return 0;
			case Adam5000_DataFormat.TwosComplementHex:
				return 1;
			}
		}
		return -1;
	}

	public static string GetDataFormatName(Adam5000Type adam5000Type, byte i_byDataFormat)
	{
		string result = "";
		Adam5000_DataFormat adam5000_DataFormat = (Adam5000_DataFormat)i_byDataFormat;
		if (adam5000Type == Adam5000Type.Adam5080)
		{
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam5000_DataFormat.TwosComplementHex:
				result = "Hexdicimal";
				break;
			}
		}
		return result;
	}

	protected static string GetAdam6051ModeName(byte i_byMode)
	{
		Adam6051_CounterMode adam6051_CounterMode = (Adam6051_CounterMode)i_byMode;
		string result = "";
		switch (adam6051_CounterMode)
		{
		case Adam6051_CounterMode.Counter:
			result = "Counter";
			break;
		case Adam6051_CounterMode.Frequency:
			result = "Frequency";
			break;
		}
		return result;
	}

	protected static double GetAdam6051ScaledValue(byte i_byMode, int i_iHigh, int i_iLow)
	{
		double result = 0.0;
		switch ((Adam6051_CounterMode)i_byMode)
		{
		case Adam6051_CounterMode.Counter:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num);
			break;
		}
		case Adam6051_CounterMode.Frequency:
		{
			long num = i_iHigh;
			num = num * 65536 + i_iLow;
			result = Convert.ToDouble(num) / 10.0;
			break;
		}
		}
		return result;
	}

	public static string GetModeName(Adam6000Type adam6000Type, byte i_byMode)
	{
		string result = "";
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			result = GetAdam6051ModeName(i_byMode);
		}
		return result;
	}

	public static int GetChannelTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			result = 2;
		}
		return result;
	}

	public static int GetChannelStart(Adam6000Type adam6000Type)
	{
		int result = 0;
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			result = 12;
		}
		return result;
	}

	public static double GetScaledValue(Adam6000Type adam6000Type, byte i_byMode, int i_iHigh, int i_iLow)
	{
		double result = 0.0;
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			result = GetAdam6051ScaledValue(i_byMode, i_iHigh, i_iLow);
		}
		return result;
	}

	public static int GetModeTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			result = 2;
		}
		return result;
	}

	public static string GetFormat(Adam6000Type adam6000Type, byte i_byMode)
	{
		string result = "";
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			Adam6051_CounterMode adam6051_CounterMode = (Adam6051_CounterMode)i_byMode;
			if (adam6051_CounterMode == Adam6051_CounterMode.Frequency)
			{
				result = "0.00";
			}
		}
		return result;
	}

	public static string GetUnitName(Adam6000Type adam6000Type, byte i_byMode)
	{
		string result = "";
		if (adam6000Type == Adam6000Type.Adam6051 || adam6000Type == Adam6000Type.Adam6051W)
		{
			Adam6051_CounterMode adam6051_CounterMode = (Adam6051_CounterMode)i_byMode;
			result = ((adam6051_CounterMode != Adam6051_CounterMode.Frequency) ? "counts" : "Hz");
		}
		return result;
	}

	public static void ParseIOConfig(byte i_byConfig, out byte o_byMode, out bool o_bRecordLastCount, out bool o_bDigitalFilter, out bool o_bInvert)
	{
		if ((i_byConfig & MODE_MASK) == 1)
		{
			o_byMode = 1;
		}
		else if ((i_byConfig & MODE_MASK) == 4)
		{
			o_byMode = 4;
		}
		else
		{
			o_byMode = 16;
		}
		if ((i_byConfig & RECORD_LAST_COUNT) > 0)
		{
			o_bRecordLastCount = true;
		}
		else
		{
			o_bRecordLastCount = false;
		}
		if ((i_byConfig & ENABLE_DIGITAL_FILTER) > 0)
		{
			o_bDigitalFilter = true;
		}
		else
		{
			o_bDigitalFilter = false;
		}
		if ((i_byConfig & ENABLE_INVERT_MODE) > 0)
		{
			o_bInvert = true;
		}
		else
		{
			o_bInvert = false;
		}
	}

	public static void FormIOConfig(byte i_byMode, bool i_bRecordLastCount, bool i_bDigitalFilter, bool i_bInvert, out byte o_byConfig)
	{
		if (i_byMode != 16)
		{
			o_byConfig = i_byMode;
		}
		else
		{
			o_byConfig = 0;
		}
		if (i_bRecordLastCount)
		{
			o_byConfig |= RECORD_LAST_COUNT;
		}
		if (i_bDigitalFilter)
		{
			o_byConfig |= ENABLE_DIGITAL_FILTER;
		}
		if (i_bInvert)
		{
			o_byConfig |= ENABLE_INVERT_MODE;
		}
	}

	public bool GetValue(out long o_lValue)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "RE\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_lValue = Convert.ToInt64(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lValue = 0L;
		return false;
	}

	public bool SetClear()
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "CE\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetInputSignalMode(out byte o_byMode)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "B\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_byMode = Convert.ToByte(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_byMode = 0;
		return false;
	}

	public bool SetInputSignalMode(byte i_byMode)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "B" + i_byMode.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLEDSource(out byte o_bySource)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "8\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_bySource = Convert.ToByte(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bySource = 0;
		return false;
	}

	public bool SetLEDSource(byte i_bySource)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "8" + i_bySource.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetLEDText(string i_szText)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "9" + i_szText + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetValue(int i_iChannel, out long o_lValue)
	{
		string i_szSend = "#" + base.Address.ToString("X02") + i_iChannel + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string value = o_szRecv.Substring(1, o_szRecv.Length - 2);
			try
			{
				o_lValue = Convert.ToInt64(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lValue = 0L;
		return false;
	}

	public bool GetMaxValue(int i_iChannel, out long o_lValue)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "3" + i_iChannel + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_lValue = Convert.ToInt64(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lValue = 0L;
		return false;
	}

	public bool SetMaxValue(int i_iChannel, long i_lValue)
	{
		if (i_lValue < 0)
		{
			i_lValue = 0L;
		}
		else if (i_lValue > uint.MaxValue)
		{
			i_lValue = 4294967295L;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "3" + i_iChannel.ToString("X") + i_lValue.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetGateMode(out byte o_byMode)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "A\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_byMode = Convert.ToByte(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_byMode = 0;
		return false;
	}

	public bool SetGateMode(byte i_byMode)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "A" + i_byMode.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetStatus(int i_iChannel, out bool o_bCounting)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "5" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 1)
			{
				if (text == "1")
				{
					o_bCounting = true;
				}
				else
				{
					o_bCounting = false;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bCounting = false;
		return false;
	}

	public bool SetStatus(int i_iChannel, bool i_bCounting)
	{
		string text = "$" + base.Address.ToString("X02") + "5" + i_iChannel.ToString("X");
		text = ((!i_bCounting) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetClear(int i_iChannel)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "6" + i_iChannel.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetOverflowFlag(int i_iChannel, out bool o_bOver)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "7" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 1)
			{
				if (text == "1")
				{
					o_bOver = true;
				}
				else
				{
					o_bOver = false;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bOver = false;
		return false;
	}

	public bool GetDigitalFilter(out bool o_bFilter)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "4\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 1)
			{
				if (text == "1")
				{
					o_bFilter = true;
				}
				else
				{
					o_bFilter = false;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bFilter = false;
		return false;
	}

	public bool SetDigitalFilter(bool i_bFilter)
	{
		string text = "$" + base.Address.ToString("X02");
		text = ((!i_bFilter) ? (text + "40\r") : (text + "41\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetMinInputSignalWidth(byte i_byLevel, out int o_iValue)
	{
		string text = "$" + base.Address.ToString("X02");
		text = ((i_byLevel != 0) ? (text + "0H\r") : (text + "0L\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_iValue = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iValue = 0;
		return false;
	}

	public bool SetMinInputSignalWidth(byte i_byLevel, int i_iValue)
	{
		if (i_iValue < 2)
		{
			i_iValue = 2;
		}
		else if (i_iValue > 65535)
		{
			i_iValue = 65535;
		}
		string text = "$" + base.Address.ToString("X02");
		text = ((i_byLevel != 0) ? (text + "0H" + i_iValue.ToString("00000") + "\r") : (text + "0L" + i_iValue.ToString("00000") + "\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetTriggerLevel(byte i_byLevel, out float o_fValue)
	{
		string text = "$" + base.Address.ToString("X02");
		text = ((i_byLevel != 0) ? (text + "1H\r") : (text + "1L\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_fValue = Convert.ToSingle(value) / 10f;
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fValue = 0f;
		return false;
	}

	public bool SetTriggerLevel(byte i_byLevel, float i_fValue)
	{
		if ((double)i_fValue < 0.1)
		{
			i_fValue = 0.1f;
		}
		else if ((double)i_fValue > 5.0)
		{
			i_fValue = 5f;
		}
		int num = Convert.ToInt32((double)i_fValue * 10.0);
		string text = "$" + base.Address.ToString("X02");
		text = ((i_byLevel != 0) ? (text + "1H" + num.ToString("00") + "\r") : (text + "1L" + num.ToString("00") + "\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStartupValue(int i_iChannel, out long o_lStartup)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "G" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_lStartup = Convert.ToInt64(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lStartup = 0L;
		return false;
	}

	public bool SetStartupValue(int i_iChannel, long i_lStartup)
	{
		if (i_lStartup < 0)
		{
			i_lStartup = 0L;
		}
		else if (i_lStartup > uint.MaxValue)
		{
			i_lStartup = 4294967295L;
		}
		string i_szSend = "@" + base.Address.ToString("X02") + "P" + i_iChannel.ToString("X") + i_lStartup.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetValues(int i_iSlot, byte i_byMode, out double[] o_dValues)
	{
		Adam5080_CounterMode adam5080_CounterMode = (Adam5080_CounterMode)i_byMode;
		if (i_iSlot < 0 || i_iSlot >= 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_dValues = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 40)
			{
				try
				{
					o_dValues = new double[4];
					for (int i = 0; i < 4; i++)
					{
						string text2 = text.Substring(i * 10, 10);
						if (text2 == "          ")
						{
							o_dValues[i] = 0.0;
						}
						else
						{
							o_dValues[i] = Convert.ToDouble(text2);
						}
						if (adam5080_CounterMode == Adam5080_CounterMode.Frequency)
						{
							o_dValues[i] /= 100.0;
						}
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 32)
			{
				try
				{
					o_dValues = new double[4];
					for (int i = 0; i < 4; i++)
					{
						string text2 = text.Substring(i * 8, 8);
						ulong value = ((!(text2 == "        ")) ? Convert.ToUInt32(text2, 16) : 0);
						o_dValues[i] = Convert.ToDouble(value);
						if (adam5080_CounterMode == Adam5080_CounterMode.Frequency)
						{
							o_dValues[i] /= 100.0;
						}
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_dValues = null;
		return false;
	}

	public bool GetValue(int i_iSlot, int i_iChannel, byte i_byMode, out double o_dValue)
	{
		Adam5080_CounterMode adam5080_CounterMode = (Adam5080_CounterMode)i_byMode;
		o_dValue = 0.0;
		if (i_iSlot < 0 || i_iSlot >= 8 || i_iChannel < 0 || i_iChannel >= 4)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "C" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 10)
			{
				try
				{
					if (text == "          ")
					{
						o_dValue = 0.0;
					}
					else
					{
						o_dValue = Convert.ToDouble(text);
					}
					if (adam5080_CounterMode == Adam5080_CounterMode.Frequency)
					{
						o_dValue /= 100.0;
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 8)
			{
				try
				{
					ulong value = ((!(text == "        ")) ? Convert.ToUInt32(text, 16) : 0);
					o_dValue = Convert.ToDouble(value);
					if (adam5080_CounterMode == Adam5080_CounterMode.Frequency)
					{
						o_dValue /= 100.0;
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		return false;
	}

	public bool GetValues(int i_iSlot, byte i_byMode, out long[] o_lValues)
	{
		if (i_iSlot < 0 || i_iSlot >= 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_lValues = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 80)
			{
				try
				{
					o_lValues = new long[8];
					for (int i = 0; i < 8; i++)
					{
						string text2 = text.Substring(i * 10, 10);
						if (text2 == "          ")
						{
							o_lValues[i] = 0L;
						}
						else
						{
							o_lValues[i] = Convert.ToInt64(text2);
						}
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 64)
			{
				try
				{
					o_lValues = new long[8];
					for (int i = 0; i < 8; i++)
					{
						string text2 = text.Substring(i * 8, 8);
						ulong value = ((!(text2 == "        ")) ? Convert.ToUInt32(text2, 16) : 0);
						o_lValues[i] = Convert.ToInt64(value);
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_lValues = null;
		return false;
	}

	public bool GetValue(int i_iSlot, int i_iChannel, byte i_byMode, out long o_lValue)
	{
		o_lValue = 0L;
		if (i_iSlot < 0 || i_iSlot >= 8 || i_iChannel < 0 || i_iChannel >= 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "C" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 10)
			{
				try
				{
					if (text == "          ")
					{
						o_lValue = 0L;
					}
					else
					{
						o_lValue = Convert.ToInt64(text);
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 8)
			{
				try
				{
					ulong value = ((!(text == "        ")) ? Convert.ToUInt32(text, 16) : 0);
					o_lValue = Convert.ToInt64(value);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		return false;
	}

	public bool GetMode(int i_iSlot, out byte o_byMode)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "B\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				string value = text2.Substring(0, 2);
				try
				{
					o_byMode = Convert.ToByte(value, 16);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_byMode = 0;
		return false;
	}

	public bool GetMode(int i_iSlot, int i_iChannel, out byte o_byMode)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "B\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				string value = text2.Substring(0, 2);
				try
				{
					o_byMode = Convert.ToByte(value, 16);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_byMode = 0;
		return false;
	}

	public bool SetMode(int i_iSlot, byte i_byMode)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "A" + i_byMode.ToString("X02") + "00\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetMode(int i_iSlot, int i_iChannel, byte i_byMode)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "A" + i_byMode.ToString("X02") + "FF\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetModeFormat(int i_iSlot, out byte o_byMode, out byte o_byFormat)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "B\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				string value = text2.Substring(0, 2);
				string value2 = text2.Substring(2, 2);
				try
				{
					o_byMode = Convert.ToByte(value, 16);
					o_byFormat = Convert.ToByte(value2, 16);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_byMode = 0;
		o_byFormat = 0;
		return false;
	}

	public bool SetModeFormat(int i_iSlot, byte i_byMode, byte i_byFormat)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "A" + i_byMode.ToString("X02") + i_byFormat.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetDigitalFilter(int i_iSlot, out int o_iFilter)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "0\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_iFilter = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iFilter = 0;
		return false;
	}

	public bool SetDigitalFilter(Adam5000Type adam5000Type, int i_iSlot, int i_iFilter)
	{
		if (adam5000Type == Adam5000Type.Adam5081)
		{
			if (i_iFilter < 1)
			{
				i_iFilter = 1;
			}
			else if (i_iFilter > 65000)
			{
				i_iFilter = 65000;
			}
		}
		else if (i_iFilter < 8)
		{
			i_iFilter = 8;
		}
		else if (i_iFilter > 65000)
		{
			i_iFilter = 65000;
		}
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "0" + i_iFilter.ToString("00000") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStartupValue(int i_iSlot, int i_iChannel, out long o_lStartup)
	{
		string text = "@" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "G\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_lStartup = Convert.ToInt64(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lStartup = 0L;
		return false;
	}

	public bool SetStartupValue(int i_iSlot, int i_iChannel, long i_lStartup)
	{
		if (i_lStartup < 0)
		{
			i_lStartup = 0L;
		}
		else if (i_lStartup > uint.MaxValue)
		{
			i_lStartup = 4294967295L;
		}
		string text = "@" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "P" + i_lStartup.ToString("0000000000") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetOverflowFlag(Adam5000Type adam5000Type, int i_iSlot, out bool[] o_bOver)
	{
		int channelTotal = GetChannelTotal(adam5000Type);
		int num = channelTotal * 2;
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "7\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == num)
			{
				o_bOver = new bool[channelTotal];
				for (int i = 0; i < channelTotal; i++)
				{
					string text3 = text2.Substring(i * 2, 2);
					if (text3 == "01")
					{
						o_bOver[i] = true;
					}
					else
					{
						o_bOver[i] = false;
					}
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bOver = null;
		return false;
	}

	public bool SetToStartup(int i_iSlot, int i_iChannel)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "6\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStatus(int i_iSlot, int i_iChannel, out bool o_bCounting)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "5\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 1)
			{
				if (text2 == "1")
				{
					o_bCounting = true;
				}
				else
				{
					o_bCounting = false;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bCounting = false;
		return false;
	}

	public bool SetStatus(int i_iSlot, int i_iChannel, bool i_bCounting)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = ((!i_bCounting) ? (text + "50\r") : (text + "51\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetIOConfig(out byte[] o_byConfig)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "C\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			int num = text.Length / 2;
			try
			{
				o_byConfig = new byte[num];
				for (int i = 0; i < num; i++)
				{
					string value = text.Substring(i * 2, 2);
					o_byConfig[i] = Convert.ToByte(value, 16);
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_byConfig = null;
		return false;
	}

	public bool SetIOConfig(byte[] i_byConfig)
	{
		string text = "$" + base.Address.ToString("X02") + "C";
		int num = i_byConfig.Length;
		for (int i = 0; i < num; i++)
		{
			text += i_byConfig[i].ToString("X02");
		}
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetIOConfig(int i_iChannel, out byte o_byConfig)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "CIC" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
				o_byConfig = Convert.ToByte(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_byConfig = 0;
		return false;
	}

	public bool SetIOConfig(int i_iChannel, byte i_byConfig)
	{
		string text = "$" + base.Address.ToString("X02") + "CIC" + i_iChannel.ToString("X");
		text = text + i_byConfig.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetDigitalFilterMiniSignalWidth(out long[] o_lHigh, out long[] o_lLow)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "0\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			int num = text.Length / 16;
			try
			{
				o_lHigh = new long[num];
				o_lLow = new long[num];
				for (int i = 0; i < num; i++)
				{
					string value = text.Substring(i * 16, 8);
					o_lLow[i] = Convert.ToInt64(value, 16);
					value = text.Substring(i * 16 + 8, 8);
					o_lHigh[i] = Convert.ToInt64(value, 16);
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lHigh = null;
		o_lLow = null;
		return false;
	}

	public bool SetDigitalFilterMiniSignalWidth(long[] i_lHigh, long[] i_lLow)
	{
		if (i_lHigh.Length == i_lLow.Length)
		{
			string text = "$" + base.Address.ToString("X02") + "0";
			int num = i_lHigh.Length;
			for (int i = 0; i < num; i++)
			{
				text += i_lLow[i].ToString("X08");
				text += i_lHigh[i].ToString("X08");
			}
			text += "\r";
			string o_szRecv;
			return ASCIISendRecv(text, out o_szRecv);
		}
		return false;
	}

	public bool GetDigitalFilterMiniSignalWidth(int i_iChannel, out long o_lHigh, out long o_lLow)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "0C" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				string value = text.Substring(0, 8);
				o_lLow = Convert.ToUInt32(value, 16);
				value = text.Substring(8, 8);
				o_lHigh = Convert.ToUInt32(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lHigh = 0L;
		o_lLow = 0L;
		return false;
	}

	public bool SetDigitalFilterMiniSignalWidth(int i_iChannel, long i_lHigh, long i_lLow)
	{
		string text = "$" + base.Address.ToString("X02") + "0C" + i_iChannel.ToString("X");
		text = text + i_lLow.ToString("X08") + i_lHigh.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}
}
