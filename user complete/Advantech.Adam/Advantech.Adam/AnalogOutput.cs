using System;
using Advantech.Common;

namespace Advantech.Adam;

public class AnalogOutput : AdamBase
{
	public AnalogOutput(AdamCom i_com)
		: base(i_com)
	{
	}

	public AnalogOutput(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	protected static byte GetSlewRateCode(int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (i_iIndex)
		{
		case 0:
			result = 0;
			break;
		case 1:
			result = 1;
			break;
		case 2:
			result = 2;
			break;
		case 3:
			result = 3;
			break;
		case 4:
			result = 4;
			break;
		case 5:
			result = 5;
			break;
		case 6:
			result = 6;
			break;
		case 7:
			result = 7;
			break;
		case 8:
			result = 8;
			break;
		case 9:
			result = 9;
			break;
		case 10:
			result = 10;
			break;
		case 11:
			result = 11;
			break;
		}
		return result;
	}

	protected static string GetSlewRateName(byte i_bySlew)
	{
		Adam_SlewRate adam_SlewRate = (Adam_SlewRate)i_bySlew;
		string result = "";
		switch (adam_SlewRate)
		{
		case Adam_SlewRate.Immediate:
			result = "Immediate change";
			break;
		case Adam_SlewRate.V_p0625_mA_p125:
			result = "0.0625 V/sec;0.0125 mA/sec";
			break;
		case Adam_SlewRate.V_p125_mA_p250:
			result = "0.0125 V/sec;0.0250 mA/sec";
			break;
		case Adam_SlewRate.V_p250_mA_p5:
			result = "0.0250 V/sec;0.500 mA/sec";
			break;
		case Adam_SlewRate.V_p5_mA_1:
			result = "0.500 V/sec;1 mA/sec";
			break;
		case Adam_SlewRate.V_1_mA_2:
			result = "1 V/sec;2 mA/sec";
			break;
		case Adam_SlewRate.V_2_mA_4:
			result = "2 V/sec;4 mA/sec";
			break;
		case Adam_SlewRate.V_4_mA_8:
			result = "4 V/sec;8 mA/sec";
			break;
		case Adam_SlewRate.V_8_mA_16:
			result = "8 V/sec;16 mA/sec";
			break;
		case Adam_SlewRate.V_16_mA_32:
			result = "16 V/sec;32 mA/sec";
			break;
		case Adam_SlewRate.V_32_mA_64:
			result = "32 V/sec;64 mA/sec";
			break;
		case Adam_SlewRate.V_64_mA_128:
			result = "64 V/sec;128 mA/sec";
			break;
		}
		return result;
	}

	protected static int GetSlewRateTotal()
	{
		return 12;
	}

	protected static int GetSlewRateIndex(byte i_bySlew)
	{
		int result = -1;
		switch ((Adam_SlewRate)i_bySlew)
		{
		case Adam_SlewRate.Immediate:
			result = 0;
			break;
		case Adam_SlewRate.V_p0625_mA_p125:
			result = 1;
			break;
		case Adam_SlewRate.V_p125_mA_p250:
			result = 2;
			break;
		case Adam_SlewRate.V_p250_mA_p5:
			result = 3;
			break;
		case Adam_SlewRate.V_p5_mA_1:
			result = 4;
			break;
		case Adam_SlewRate.V_1_mA_2:
			result = 5;
			break;
		case Adam_SlewRate.V_2_mA_4:
			result = 6;
			break;
		case Adam_SlewRate.V_4_mA_8:
			result = 7;
			break;
		case Adam_SlewRate.V_8_mA_16:
			result = 8;
			break;
		case Adam_SlewRate.V_16_mA_32:
			result = 9;
			break;
		case Adam_SlewRate.V_32_mA_64:
			result = 10;
			break;
		case Adam_SlewRate.V_64_mA_128:
			result = 11;
			break;
		}
		return result;
	}

	protected static byte GetAdam4021RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 48, 
			1 => 49, 
			2 => 50, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4021RangeIndex(byte i_byRange)
	{
		return (Adam4021_OutputRange)i_byRange switch
		{
			Adam4021_OutputRange.mA_0To20 => 0, 
			Adam4021_OutputRange.mA_4To20 => 1, 
			Adam4021_OutputRange.V_0To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam4021RangeName(byte i_byRange)
	{
		Adam4021_OutputRange adam4021_OutputRange = (Adam4021_OutputRange)i_byRange;
		string result = "";
		switch (adam4021_OutputRange)
		{
		case Adam4021_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4021_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4021_OutputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static byte GetAdam4022TRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4022TRangeIndex(byte i_byRange)
	{
		return (Adam4022T_OutputRange)i_byRange switch
		{
			Adam4022T_OutputRange.mA_0To20 => 0, 
			Adam4022T_OutputRange.mA_4To20 => 1, 
			Adam4022T_OutputRange.V_0To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam4022TRangeName(byte i_byRange)
	{
		Adam4022T_OutputRange adam4022T_OutputRange = (Adam4022T_OutputRange)i_byRange;
		string result = "";
		switch (adam4022T_OutputRange)
		{
		case Adam4022T_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4022T_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4022T_OutputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam4022TScaledValue(byte i_byRange, int i_iValue)
	{
		float num;
		float num2;
		switch ((Adam4022T_OutputRange)i_byRange)
		{
		case Adam4022T_OutputRange.mA_0To20:
			num = 20f;
			num2 = 0f;
			break;
		case Adam4022T_OutputRange.mA_4To20:
			num = 20f;
			num2 = 4f;
			break;
		case Adam4022T_OutputRange.V_0To10:
			num = 10f;
			num2 = 0f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		return (num - num2) * Convert.ToSingle(i_iValue) / 4095f + num2;
	}

	protected static byte GetAdam4024RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 48, 
			1 => 49, 
			2 => 50, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4024RangeIndex(byte i_byRange)
	{
		return (Adam4024_OutputRange)i_byRange switch
		{
			Adam4024_OutputRange.mA_0To20 => 0, 
			Adam4024_OutputRange.mA_4To20 => 1, 
			Adam4024_OutputRange.V_Neg10To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam4024RangeName(byte i_byRange)
	{
		Adam4024_OutputRange adam4024_OutputRange = (Adam4024_OutputRange)i_byRange;
		string result = "";
		switch (adam4024_OutputRange)
		{
		case Adam4024_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4024_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4024_OutputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam4024ScaledValue(byte i_byRange, int i_iValue)
	{
		float num;
		float num2;
		switch ((Adam4024_OutputRange)i_byRange)
		{
		case Adam4024_OutputRange.mA_0To20:
			num = 20f;
			num2 = 0f;
			break;
		case Adam4024_OutputRange.mA_4To20:
			num = 20f;
			num2 = 4f;
			break;
		case Adam4024_OutputRange.V_Neg10To10:
			num = 10f;
			num2 = -10f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		return (num - num2) * Convert.ToSingle(i_iValue) / 4095f + num2;
	}

	public static float GetScaledValue(Adam4000Type adam4000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4024:
			result = GetAdam4024ScaledValue(i_byRange, i_iValue);
			break;
		}
		return result;
	}

	public static byte GetSlewRateCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		if (adam4000Type == Adam4000Type.Adam4021 || adam4000Type == Adam4000Type.Adam4024)
		{
			return GetSlewRateCode(i_iIndex);
		}
		return result;
	}

	public static string GetSlewRateName(Adam4000Type adam4000Type, byte i_bySlew)
	{
		string result = "";
		if (adam4000Type == Adam4000Type.Adam4021 || adam4000Type == Adam4000Type.Adam4024)
		{
			return GetSlewRateName(i_bySlew);
		}
		return result;
	}

	public static int GetSlewRateTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		if (adam4000Type == Adam4000Type.Adam4021 || adam4000Type == Adam4000Type.Adam4024)
		{
			return GetSlewRateTotal();
		}
		return result;
	}

	public static int GetSlewRateIndex(Adam4000Type adam4000Type, byte i_bySlew)
	{
		int result = -1;
		if (adam4000Type == Adam4000Type.Adam4021 || adam4000Type == Adam4000Type.Adam4024)
		{
			return GetSlewRateIndex(i_bySlew);
		}
		return result;
	}

	public static byte GetRangeCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = GetAdam4021RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4024:
			result = GetAdam4024RangeCode(i_iIndex);
			break;
		}
		return result;
	}

	public static string GetRangeName(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "";
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = GetAdam4021RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4024:
			result = GetAdam4024RangeName(i_byRange);
			break;
		}
		return result;
	}

	public static int GetRangeIndex(Adam4000Type adam4000Type, byte i_byRange)
	{
		int result = -1;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = GetAdam4021RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4024:
			result = GetAdam4024RangeIndex(i_byRange);
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = 1;
			break;
		case Adam4000Type.Adam4022T:
			result = 2;
			break;
		case Adam4000Type.Adam4024:
			result = 4;
			break;
		}
		return result;
	}

	public static int GetRangeTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = 3;
			break;
		case Adam4000Type.Adam4022T:
			result = 3;
			break;
		case Adam4000Type.Adam4024:
			result = 3;
			break;
		}
		return result;
	}

	public static string GetFloatFormat(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "0.0;-0.0";
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			result = "0.000;-0.000";
			break;
		case Adam4000Type.Adam4022T:
			result = "0.000;-0.000";
			break;
		case Adam4000Type.Adam4024:
			result = "0.000;-0.000";
			break;
		}
		return result;
	}

	public static string GetUnitName(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam4000Type, i_byRange);
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
			break;
		case Adam4000Type.Adam4022T:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
			break;
		case Adam4000Type.Adam4024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
			break;
		}
		return result;
	}

	public static string GetSpanCalibrationName(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam4000Type, i_byRange);
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			break;
		case Adam4000Type.Adam4022T:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "10 V";
			}
			break;
		case Adam4000Type.Adam4024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			break;
		}
		return result;
	}

	public static string GetZeroCalibrationName(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam4000Type, i_byRange);
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4021:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			break;
		case Adam4000Type.Adam4022T:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "0 V";
			}
			break;
		case Adam4000Type.Adam4024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			break;
		}
		return result;
	}

	public static int GetDataFormatTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		if (adam4000Type == Adam4000Type.Adam4021)
		{
			result = 3;
		}
		return result;
	}

	public static byte GetDataFormatCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		if (adam4000Type == Adam4000Type.Adam4021)
		{
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 2;
			}
		}
		return byte.MaxValue;
	}

	public static int GetDataFormatIndex(Adam4000Type adam4000Type, byte i_byDataFormat)
	{
		Adam4000_DataFormat adam4000_DataFormat = (Adam4000_DataFormat)i_byDataFormat;
		if (adam4000Type == Adam4000Type.Adam4021)
		{
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				return 0;
			case Adam4000_DataFormat.Percent:
				return 1;
			case Adam4000_DataFormat.TwosComplementHex:
				return 2;
			}
		}
		return -1;
	}

	public static string GetDataFormatName(Adam4000Type adam4000Type, byte i_byDataFormat)
	{
		string result = "";
		Adam4000_DataFormat adam4000_DataFormat = (Adam4000_DataFormat)i_byDataFormat;
		if (adam4000Type == Adam4000Type.Adam4021)
		{
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam4000_DataFormat.Percent:
				result = "%FSR";
				break;
			case Adam4000_DataFormat.TwosComplementHex:
				result = "Two's complement Hexdicimal";
				break;
			}
		}
		return result;
	}

	protected static byte GetAdam5024RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 48, 
			1 => 49, 
			2 => 50, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5024RangeIndex(byte i_byRange)
	{
		return (Adam5024_OutputRange)i_byRange switch
		{
			Adam5024_OutputRange.mA_0To20 => 0, 
			Adam5024_OutputRange.mA_4To20 => 1, 
			Adam5024_OutputRange.V_0To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam5024RangeName(byte i_byRange)
	{
		Adam5024_OutputRange adam5024_OutputRange = (Adam5024_OutputRange)i_byRange;
		string result = "";
		switch (adam5024_OutputRange)
		{
		case Adam5024_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam5024_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam5024_OutputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam5024ScaledValue(byte i_byRange, int i_iValue)
	{
		float num;
		float num2;
		switch ((Adam5024_OutputRange)i_byRange)
		{
		case Adam5024_OutputRange.mA_0To20:
			num = 20f;
			num2 = 0f;
			break;
		case Adam5024_OutputRange.mA_4To20:
			num = 20f;
			num2 = 4f;
			break;
		case Adam5024_OutputRange.V_0To10:
			num = 10f;
			num2 = 0f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		return (num - num2) * Convert.ToSingle(i_iValue) / 4095f + num2;
	}

	public static float GetScaledValue(Adam5000Type adam5000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = GetAdam5024ScaledValue(i_byRange, i_iValue);
		}
		return result;
	}

	public static byte GetSlewRateCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			return GetSlewRateCode(i_iIndex);
		}
		return result;
	}

	public static string GetSlewRateName(Adam5000Type adam5000Type, byte i_bySlew)
	{
		string result = "";
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			return GetSlewRateName(i_bySlew);
		}
		return result;
	}

	public static int GetSlewRateTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			return GetSlewRateTotal();
		}
		return result;
	}

	public static int GetSlewRateIndex(Adam5000Type adam5000Type, byte i_bySlew)
	{
		int result = -1;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			return GetSlewRateIndex(i_bySlew);
		}
		return result;
	}

	public static byte GetRangeCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = GetAdam5024RangeCode(i_iIndex);
		}
		return result;
	}

	public static string GetRangeName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = GetAdam5024RangeName(i_byRange);
		}
		return result;
	}

	public static int GetRangeIndex(Adam5000Type adam5000Type, byte i_byRange)
	{
		int result = -1;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = GetAdam5024RangeIndex(i_byRange);
		}
		return result;
	}

	public static int GetChannelTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = 4;
		}
		return result;
	}

	public static int GetRangeTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = 3;
		}
		return result;
	}

	public static string GetFloatFormat(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "0.0;-0.0";
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			result = "0.000;-0.000";
		}
		return result;
	}

	public static string GetUnitName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
		}
		return result;
	}

	public static string GetSpanCalibrationName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "10 V";
			}
		}
		return result;
	}

	public static string GetZeroCalibrationName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		if (adam5000Type == Adam5000Type.Adam5024)
		{
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "0 V";
			}
		}
		return result;
	}

	protected static byte GetAdam6022RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6022RangeIndex(byte i_byRange)
	{
		return (Adam6022_OutputRange)i_byRange switch
		{
			Adam6022_OutputRange.mA_0To20 => 0, 
			Adam6022_OutputRange.mA_4To20 => 1, 
			Adam6022_OutputRange.V_0To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam6022RangeName(byte i_byRange)
	{
		Adam6022_OutputRange adam6022_OutputRange = (Adam6022_OutputRange)i_byRange;
		string result = "";
		switch (adam6022_OutputRange)
		{
		case Adam6022_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam6022_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam6022_OutputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam6022ScaledValue(byte i_byRange, int i_iValue)
	{
		float num;
		float num2;
		switch ((Adam6022_OutputRange)i_byRange)
		{
		case Adam6022_OutputRange.mA_0To20:
			num = 20f;
			num2 = 0f;
			break;
		case Adam6022_OutputRange.mA_4To20:
			num = 20f;
			num2 = 4f;
			break;
		case Adam6022_OutputRange.V_0To10:
			num = 10f;
			num2 = 0f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		return (num - num2) * Convert.ToSingle(i_iValue) / 4095f + num2;
	}

	protected static byte GetAdam6024RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6024RangeIndex(byte i_byRange)
	{
		return (Adam6024_OutputRange)i_byRange switch
		{
			Adam6024_OutputRange.mA_0To20 => 0, 
			Adam6024_OutputRange.mA_4To20 => 1, 
			Adam6024_OutputRange.V_0To10 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam6024RangeName(byte i_byRange)
	{
		Adam6024_OutputRange adam6024_OutputRange = (Adam6024_OutputRange)i_byRange;
		string result = "";
		switch (adam6024_OutputRange)
		{
		case Adam6024_OutputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam6024_OutputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam6024_OutputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam6024ScaledValue(byte i_byRange, int i_iValue)
	{
		float num;
		float num2;
		switch ((Adam6024_OutputRange)i_byRange)
		{
		case Adam6024_OutputRange.mA_0To20:
			num = 20f;
			num2 = 0f;
			break;
		case Adam6024_OutputRange.mA_4To20:
			num = 20f;
			num2 = 4f;
			break;
		case Adam6024_OutputRange.V_0To10:
			num = 10f;
			num2 = 0f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		return (num - num2) * Convert.ToSingle(i_iValue) / 4095f + num2;
	}

	public static float GetScaledValue(Adam6000Type Adam6000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = GetAdam6022ScaledValue(i_byRange, i_iValue);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024ScaledValue(i_byRange, i_iValue);
			break;
		}
		return result;
	}

	public static byte GetRangeCode(Adam6000Type Adam6000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeCode(i_iIndex);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeCode(i_iIndex);
			break;
		}
		return result;
	}

	public static string GetRangeName(Adam6000Type Adam6000Type, byte i_byRange)
	{
		string result = "";
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeName(i_byRange);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeName(i_byRange);
			break;
		}
		return result;
	}

	public static int GetRangeIndex(Adam6000Type Adam6000Type, byte i_byRange)
	{
		int result = -1;
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeIndex(i_byRange);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeIndex(i_byRange);
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam6000Type Adam6000Type)
	{
		int result = 0;
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = 2;
			break;
		case Adam6000Type.Adam6024:
			result = 2;
			break;
		}
		return result;
	}

	public static int GetRangeTotal(Adam6000Type Adam6000Type)
	{
		int result = 0;
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = 3;
			break;
		case Adam6000Type.Adam6024:
			result = 3;
			break;
		}
		return result;
	}

	public static string GetFloatFormat(Adam6000Type Adam6000Type, byte i_byRange)
	{
		string result = "0.0;-0.0";
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = "0.000;-0.000";
			break;
		case Adam6000Type.Adam6024:
			result = "0.000;-0.000";
			break;
		}
		return result;
	}

	public static string GetUnitName(Adam6000Type Adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(Adam6000Type, i_byRange);
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
			break;
		case Adam6000Type.Adam6024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex == 2)
			{
				result = "V";
			}
			break;
		}
		return result;
	}

	public static string GetSpanCalibrationName(Adam6000Type Adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(Adam6000Type, i_byRange);
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "10 V";
			}
			break;
		case Adam6000Type.Adam6024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "20 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "10 V";
			}
			break;
		}
		return result;
	}

	public static string GetZeroCalibrationName(Adam6000Type Adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(Adam6000Type, i_byRange);
		switch (Adam6000Type)
		{
		case Adam6000Type.Adam6022:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "0 V";
			}
			break;
		case Adam6000Type.Adam6024:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "4 mA";
			}
			else if (rangeIndex == 2)
			{
				result = "0 V";
			}
			break;
		}
		return result;
	}

	public bool GetExcitationValue(out float o_fValue)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "6\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 7)
			{
				try
				{
					o_fValue = Convert.ToSingle(text, m_numberFormatInfo);
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
		o_fValue = 0f;
		return false;
	}

	public bool SetExcitationValue(float i_fValue)
	{
		string text = "$" + base.Address.ToString("X02") + "7";
		text = text + i_fValue.ToString("+00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetExcitationToStartup()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetExcitationCalibrationCounts(int i_iCount)
	{
		byte b = (byte)((i_iCount > 127) ? 127 : ((i_iCount >= 0 && i_iCount <= 127) ? Convert.ToByte(i_iCount) : ((i_iCount < -128 || i_iCount > -1) ? 128 : Convert.ToByte(i_iCount + 256))));
		string i_szSend = "$" + base.Address.ToString("X02") + "E" + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetExcitationSpanCalibration()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "B\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetExcitationZeroCalibration()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "A\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetCurrentValue(byte i_byFormat, out float o_fValue)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "6\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				if (i_byFormat == 0 || i_byFormat == 1)
				{
					o_fValue = Convert.ToSingle(value, m_numberFormatInfo);
				}
				else
				{
					int value2 = Convert.ToInt32(value, 16);
					o_fValue = Convert.ToSingle(value2);
				}
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

	public bool SetCurrentValue(byte i_byFormat, float i_fValue)
	{
		string i_szSend;
		switch (i_byFormat)
		{
		case 0:
			i_szSend = "#" + base.Address.ToString("X02") + i_fValue.ToString("00.000", m_numberFormatInfo) + "\r";
			break;
		case 1:
			i_szSend = "#" + base.Address.ToString("X02") + i_fValue.ToString("+000.00", m_numberFormatInfo) + "\r";
			break;
		default:
		{
			int num = Convert.ToInt32(i_fValue);
			i_szSend = "#" + base.Address.ToString("X02") + num.ToString("X03") + "\r";
			break;
		}
		}
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool Set20mACalibration()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "1\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool Set4mACalibration()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "0\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetCurrentToStartup()
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "4\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool SetCalibrationCounts(int i_iCount)
	{
		int num = ((i_iCount > 95) ? 95 : ((i_iCount >= 0 && i_iCount <= 95) ? i_iCount : ((i_iCount < -95 || i_iCount > -1) ? 161 : (i_iCount + 256))));
		string i_szSend = "$" + base.Address.ToString("X02") + "3" + num.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetValue(int i_iChannel, out float o_fValue)
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "O" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 7)
			{
				try
				{
					o_fValue = Convert.ToSingle(text, m_numberFormatInfo);
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
		o_fValue = 0f;
		return false;
	}

	public bool SetValue(int i_iChannel, float i_fValue)
	{
		string text = "#" + base.Address.ToString("X02");
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + i_fValue.ToString("00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetRange(int i_iChannel, out byte o_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "9";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 7)
		{
			string text2 = o_szRecv.Substring(6, o_szRecv.Length - 7);
			if (text2.Length == 2)
			{
				try
				{
					o_byRange = Convert.ToByte(text2, 16);
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
		o_byRange = byte.MaxValue;
		return false;
	}

	public bool SetRange(int i_iChannel, byte i_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "9";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "R" + i_byRange.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetCurrentValue(int i_iChannel, out float o_fValue)
	{
		string text = "$" + base.Address.ToString("X02") + "6";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 7)
			{
				try
				{
					o_fValue = Convert.ToSingle(text2, m_numberFormatInfo);
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
		o_fValue = 0f;
		return false;
	}

	public bool SetCurrentValue(int i_iChannel, float i_fValue)
	{
		string text = "#" + base.Address.ToString("X02");
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + i_fValue.ToString("+00.000;-00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStartupValue(int i_iChannel, out float o_fValue)
	{
		string text = "$" + base.Address.ToString("X02") + "D";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_fValue = Convert.ToSingle(value, m_numberFormatInfo);
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

	public bool SetStartupValue(int i_iChannel, float i_fValue)
	{
		string text = "#" + base.Address.ToString("X02") + "S";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + i_fValue.ToString("+00.000;-00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetEmergencyValue(int i_iChannel, out float o_fValue)
	{
		string text = "$" + base.Address.ToString("X02") + "E";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_fValue = Convert.ToSingle(value, m_numberFormatInfo);
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

	public bool SetEmergencyValue(int i_iChannel, float i_fValue)
	{
		string text = "#" + base.Address.ToString("X02") + "E";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + i_fValue.ToString("+00.000;-00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetOutputRange(int i_iChannel, out byte o_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "8";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 6)
		{
			string text2 = o_szRecv.Substring(5, o_szRecv.Length - 6);
			if (text2.Length == 2)
			{
				try
				{
					o_byRange = Convert.ToByte(text2, 16);
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
		o_byRange = byte.MaxValue;
		return false;
	}

	public bool SetOutputRange(int i_iChannel, byte i_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "7";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "R" + i_byRange.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool Set20mACalibration(int i_iChannel)
	{
		string text = "$" + base.Address.ToString("X02") + "1";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool Set4mACalibration(int i_iChannel)
	{
		string text = "$" + base.Address.ToString("X02") + "0";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetCalibrationCounts(int i_iChannel, int i_iCount)
	{
		int num = ((i_iCount > 127) ? 127 : ((i_iCount >= 0 && i_iCount <= 127) ? i_iCount : ((i_iCount < -127 || i_iCount > -1) ? 255 : (128 - i_iCount))));
		string i_szSend = "$" + base.Address.ToString("X02") + "3C" + i_iChannel.ToString("X") + num.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetEmergencyFlag(int i_iChannel, out bool o_bFlag)
	{
		string text = "$" + base.Address.ToString("X02") + "B";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 6)
		{
			string text2 = o_szRecv.Substring(5, o_szRecv.Length - 6);
			if (text2 == "1")
			{
				o_bFlag = true;
				return true;
			}
			if (text2 == "0")
			{
				o_bFlag = false;
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Data;
		}
		o_bFlag = false;
		return false;
	}

	public bool GetEmergencyDI(int i_iChannelTotal, out bool[] o_bDI)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "I\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				if (i_iChannelTotal <= 8)
				{
					byte b = Convert.ToByte(value, 16);
					o_bDI = new bool[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						o_bDI[i] = (b & (1 << i)) > 0;
					}
					return true;
				}
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bDI = null;
		return false;
	}

	public bool SetEmergencyFlag(int i_iChannel, bool i_bFlag)
	{
		string text = "$" + base.Address.ToString("X02") + "A";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = ((!i_bFlag) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetCurrentValue(int i_iSlot, int i_iChannel, out float o_fValue)
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
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 6)
			{
				try
				{
					o_fValue = Convert.ToSingle(text2, m_numberFormatInfo);
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
		o_fValue = 0f;
		return false;
	}

	public bool GetConfiguration(int i_iSlot, int i_iChannel, out byte o_byRange, out byte o_bySlewrate)
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
				string value2 = text2.Substring(2, 2);
				try
				{
					o_byRange = Convert.ToByte(value, 16);
					byte b = Convert.ToByte(value2, 16);
					o_bySlewrate = Convert.ToByte((b & 0x3C) >> 2);
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
		o_byRange = byte.MaxValue;
		o_bySlewrate = byte.MaxValue;
		return false;
	}

	public bool SetConfiguration(int i_iSlot, int i_iChannel, byte i_byRange, byte i_bySlew)
	{
		byte b = Convert.ToByte(i_bySlew << 2);
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "A" + i_byRange.ToString("X02") + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetConfiguration(int i_iSlot, int i_iChannel, out byte o_byRange)
	{
		byte o_bySlewrate;
		return GetConfiguration(i_iSlot, i_iChannel, out o_byRange, out o_bySlewrate);
	}

	public bool SetConfiguration(int i_iSlot, int i_iChannel, byte i_byRange)
	{
		return SetConfiguration(i_iSlot, i_iChannel, i_byRange, 0);
	}

	public bool SetSpanCalibration(int i_iSlot, int i_iChannel)
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
		text += "1\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetZeroCalibration(int i_iSlot, int i_iChannel)
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
		text += "0\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStartupValue(int i_iSlot, int i_iChannel, out float o_fValue)
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
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_fValue = Convert.ToSingle(value, m_numberFormatInfo);
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

	public bool SetCurrentToStartup(int i_iSlot, int i_iChannel)
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
		text += "4\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetCurrentValue(int i_iSlot, int i_iChannel, float i_fValue)
	{
		string text = "#" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + i_fValue.ToString("00.000", m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetCalibrationCounts(int i_iSlot, int i_iChannel, int i_iCount)
	{
		int num = ((i_iCount > 95) ? 95 : ((i_iCount >= 0 && i_iCount <= 95) ? i_iCount : ((i_iCount < -95 || i_iCount > -1) ? 161 : (i_iCount + 256))));
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text = text + "3" + num.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetConfiguration(int i_iChannel, out byte o_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "C";
		text = text + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 2)
			{
				try
				{
					o_byRange = Convert.ToByte(text2, 16);
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
		o_byRange = byte.MaxValue;
		return false;
	}

	public bool SetConfiguration(int i_iChannel, byte i_byRange)
	{
		string text = "$" + base.Address.ToString("X02") + "C";
		text += i_iChannel.ToString("X02");
		text = text + i_byRange.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetSpanCalibrationRaw(int i_iChannel, out int o_iRawData)
	{
		string o_szRecv;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			string text = "$" + base.Address.ToString("X02") + "2";
			text = text + i_iChannel.ToString("X02") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 4)
			{
				string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text2.Length == 3)
				{
					try
					{
						o_iRawData = Convert.ToInt32(text2, 16);
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
		}
		else
		{
			string text = "$" + base.Address.ToString("X02") + "2C";
			text = text + i_iChannel.ToString("X") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 6)
			{
				string text2 = o_szRecv.Substring(5, o_szRecv.Length - 6);
				if (text2.Length == 3)
				{
					try
					{
						o_iRawData = Convert.ToInt32(text2, 16);
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
		}
		o_iRawData = 0;
		return false;
	}

	public bool SetSpanCalibrationRaw(int i_iChannel, int i_iRawData)
	{
		string o_szRecv;
		string text;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			text = "$" + base.Address.ToString("X02") + "2";
			text += i_iChannel.ToString("X02");
			text = text + i_iRawData.ToString("X03") + "\r";
			return ASCIISendRecv(text, out o_szRecv);
		}
		text = "$" + base.Address.ToString("X02") + "2C";
		text += i_iChannel.ToString("X");
		text = text + i_iRawData.ToString("X03") + "\r";
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetZeroCalibrationRaw(int i_iChannel, out int o_iRawData)
	{
		string o_szRecv;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			string text = "$" + base.Address.ToString("X02") + "3";
			text = text + i_iChannel.ToString("X02") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 4)
			{
				string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text2.Length == 3)
				{
					try
					{
						o_iRawData = Convert.ToInt32(text2, 16);
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
		}
		else
		{
			string text = "$" + base.Address.ToString("X02") + "3C";
			text = text + i_iChannel.ToString("X") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 6)
			{
				string text2 = o_szRecv.Substring(5, o_szRecv.Length - 6);
				if (text2.Length == 3)
				{
					try
					{
						o_iRawData = Convert.ToInt32(text2, 16);
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
		}
		o_iRawData = 0;
		return false;
	}

	public bool SetZeroCalibrationRaw(int i_iChannel, int i_iRawData)
	{
		string o_szRecv;
		string text;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			text = "$" + base.Address.ToString("X02") + "3";
			text += i_iChannel.ToString("X02");
			text = text + i_iRawData.ToString("X03") + "\r";
			return ASCIISendRecv(text, out o_szRecv);
		}
		text = "$" + base.Address.ToString("X02") + "3C";
		text += i_iChannel.ToString("X");
		text = text + i_iRawData.ToString("X03") + "\r";
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetCalibrationRawOutput(int i_iChannel, int i_iRawData)
	{
		string o_szRecv;
		string text;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			text = "$" + base.Address.ToString("X02") + "R";
			text += i_iChannel.ToString("X02");
			text = text + i_iRawData.ToString("X03") + "\r";
			return ASCIISendRecv(text, out o_szRecv);
		}
		text = "#" + base.Address.ToString("X02") + "RC";
		text += i_iChannel.ToString("X");
		text = text + i_iRawData.ToString("X03") + "\r";
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetStartupValue(int i_iChannel, out int o_iValue)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "D" + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_iValue = Convert.ToInt32(value, 16);
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

	public bool SetStartupValue(int i_iChannel, int i_iValue)
	{
		if (i_iValue > 4095)
		{
			i_iValue = 4095;
		}
		if (i_iValue < 0)
		{
			i_iValue = 0;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "D" + i_iChannel.ToString("X02") + i_iValue.ToString("X03") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}
}
