using System;
using Advantech.Common;

namespace Advantech.Adam;

public class AnalogInput : AdamBase
{
	public AnalogInput(AdamCom i_com)
		: base(i_com)
	{
	}

	public AnalogInput(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	protected static byte GetAdam4011RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 14, 
			8 => 15, 
			9 => 16, 
			10 => 17, 
			11 => 18, 
			12 => 19, 
			13 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4011RangeIndex(byte i_byRange)
	{
		return (Adam4011_InputRange)i_byRange switch
		{
			Adam4011_InputRange.mV_Neg15To15 => 0, 
			Adam4011_InputRange.mV_Neg50To50 => 1, 
			Adam4011_InputRange.mV_Neg100To100 => 2, 
			Adam4011_InputRange.mV_Neg500To500 => 3, 
			Adam4011_InputRange.V_Neg1To1 => 4, 
			Adam4011_InputRange.V_Neg2AndHalfTo2AndHalf => 5, 
			Adam4011_InputRange.mA_Neg20To20 => 6, 
			Adam4011_InputRange.Jtype_0To760C => 7, 
			Adam4011_InputRange.Ktype_0To1370C => 8, 
			Adam4011_InputRange.Ttype_Neg100To400C => 9, 
			Adam4011_InputRange.Etype_0To1000C => 10, 
			Adam4011_InputRange.Rtype_500To1750C => 11, 
			Adam4011_InputRange.Stype_500To1750C => 12, 
			Adam4011_InputRange.Btype_500To1800C => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam4011RangeName(byte i_byRange)
	{
		Adam4011_InputRange adam4011_InputRange = (Adam4011_InputRange)i_byRange;
		string result = "";
		switch (adam4011_InputRange)
		{
		case Adam4011_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam4011_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam4011_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4011_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4011_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4011_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam4011_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4011_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4011_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4011_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4011_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4011_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4011_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4011_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static byte GetAdam4012RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 8, 
			1 => 9, 
			2 => 10, 
			3 => 11, 
			4 => 12, 
			5 => 13, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4012RangeIndex(byte i_byRange)
	{
		return (Adam4012_InputRange)i_byRange switch
		{
			Adam4012_InputRange.V_Neg10To10 => 0, 
			Adam4012_InputRange.V_Neg5To5 => 1, 
			Adam4012_InputRange.V_Neg1To1 => 2, 
			Adam4012_InputRange.mV_Neg500To500 => 3, 
			Adam4012_InputRange.mV_Neg150To150 => 4, 
			Adam4012_InputRange.mA_Neg20To20 => 5, 
			_ => -1, 
		};
	}

	protected static string GetAdam4012RangeName(byte i_byRange)
	{
		Adam4012_InputRange adam4012_InputRange = (Adam4012_InputRange)i_byRange;
		string result = "";
		switch (adam4012_InputRange)
		{
		case Adam4012_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4012_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4012_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4012_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4012_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam4012_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		}
		return result;
	}

	protected static byte GetAdam4013RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 32, 
			1 => 33, 
			2 => 34, 
			3 => 35, 
			4 => 36, 
			5 => 37, 
			6 => 38, 
			7 => 39, 
			8 => 40, 
			9 => 41, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4013RangeIndex(byte i_byRange)
	{
		return (Adam4013_InputRange)i_byRange switch
		{
			Adam4013_InputRange.Pt385_Neg100To100 => 0, 
			Adam4013_InputRange.Pt385_0To100 => 1, 
			Adam4013_InputRange.Pt385_0To200 => 2, 
			Adam4013_InputRange.Pt385_0To600 => 3, 
			Adam4013_InputRange.Pt392_Neg100To100 => 4, 
			Adam4013_InputRange.Pt392_0To100 => 5, 
			Adam4013_InputRange.Pt392_0To200 => 6, 
			Adam4013_InputRange.Pt392_0To600 => 7, 
			Adam4013_InputRange.Ni518_Neg80To100 => 8, 
			Adam4013_InputRange.Ni518_0To100 => 9, 
			_ => -1, 
		};
	}

	protected static string GetAdam4013RangeName(byte i_byRange)
	{
		Adam4013_InputRange adam4013_InputRange = (Adam4013_InputRange)i_byRange;
		string result = "";
		switch (adam4013_InputRange)
		{
		case Adam4013_InputRange.Pt385_Neg100To100:
			result = "Pt(385) -100~100 'C";
			break;
		case Adam4013_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case Adam4013_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case Adam4013_InputRange.Pt385_0To600:
			result = "Pt(385) 0~600 'C";
			break;
		case Adam4013_InputRange.Pt392_Neg100To100:
			result = "Pt(392) -100~100 'C";
			break;
		case Adam4013_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case Adam4013_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case Adam4013_InputRange.Pt392_0To600:
			result = "Pt(392) 0~600 'C";
			break;
		case Adam4013_InputRange.Ni518_Neg80To100:
			result = "Ni(518) -80~100 'C";
			break;
		case Adam4013_InputRange.Ni518_0To100:
			result = "Ni(518) 0~100 'C";
			break;
		}
		return result;
	}

	protected static byte GetAdam4015RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 32, 
			1 => 33, 
			2 => 34, 
			3 => 35, 
			4 => 36, 
			5 => 37, 
			6 => 38, 
			7 => 39, 
			8 => 40, 
			9 => 41, 
			10 => 42, 
			11 => 43, 
			12 => 44, 
			13 => 45, 
			14 => 50, 
			15 => 51, 
			16 => 52, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4015RangeIndex(byte i_byRange)
	{
		return (Adam4015_InputRange)i_byRange switch
		{
			Adam4015_InputRange.Pt385_Neg50To150 => 0, 
			Adam4015_InputRange.Pt385_0To100 => 1, 
			Adam4015_InputRange.Pt385_0To200 => 2, 
			Adam4015_InputRange.Pt385_0To400 => 3, 
			Adam4015_InputRange.Pt385_Neg200To200 => 4, 
			Adam4015_InputRange.Pt392_Neg50To150 => 5, 
			Adam4015_InputRange.Pt392_0To100 => 6, 
			Adam4015_InputRange.Pt392_0To200 => 7, 
			Adam4015_InputRange.Pt392_0To400 => 8, 
			Adam4015_InputRange.Pt392_Neg200To200 => 9, 
			Adam4015_InputRange.Pt1000_Neg40To160 => 10, 
			Adam4015_InputRange.Balcon500_Neg30To120 => 11, 
			Adam4015_InputRange.Ni518_Neg80To100 => 12, 
			Adam4015_InputRange.Ni518_0To100 => 13, 
			Adam4015_InputRange.Ni508_0To100 => 14, 
			Adam4015_InputRange.Ni508_Neg50To200 => 15, 
			Adam4015_InputRange.BA1_Neg200To600 => 16, 
			_ => -1, 
		};
	}

	protected static string GetAdam4015RangeName(byte i_byRange)
	{
		Adam4015_InputRange adam4015_InputRange = (Adam4015_InputRange)i_byRange;
		string result = "";
		switch (adam4015_InputRange)
		{
		case Adam4015_InputRange.Pt385_Neg50To150:
			result = "Pt(385) -50~150 'C";
			break;
		case Adam4015_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case Adam4015_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case Adam4015_InputRange.Pt385_0To400:
			result = "Pt(385) 0~400 'C";
			break;
		case Adam4015_InputRange.Pt385_Neg200To200:
			result = "Pt(385) -200~200 'C";
			break;
		case Adam4015_InputRange.Pt392_Neg50To150:
			result = "Pt(392) -50~150 'C";
			break;
		case Adam4015_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case Adam4015_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case Adam4015_InputRange.Pt392_0To400:
			result = "Pt(392) 0~400 'C";
			break;
		case Adam4015_InputRange.Pt392_Neg200To200:
			result = "Pt(392) -200~200 'C";
			break;
		case Adam4015_InputRange.Pt1000_Neg40To160:
			result = "Pt(1000) -40~160 'C";
			break;
		case Adam4015_InputRange.Balcon500_Neg30To120:
			result = "Balcon(500) -30~120";
			break;
		case Adam4015_InputRange.Ni518_Neg80To100:
			result = "Ni(518) -80~100 'C";
			break;
		case Adam4015_InputRange.Ni518_0To100:
			result = "Ni(518) 0~100 'C";
			break;
		case Adam4015_InputRange.Ni508_0To100:
			result = "Ni(508) 0~100 'C";
			break;
		case Adam4015_InputRange.Ni508_Neg50To200:
			result = "Ni(508) -50~200 'C";
			break;
		case Adam4015_InputRange.BA1_Neg200To600:
			result = "BA1 -200~600 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4015ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4015RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4015RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4015_InputRange)i_byRange)
		{
		case Adam4015_InputRange.Pt385_Neg50To150:
			fHigh = 150f;
			fLow = -50f;
			break;
		case Adam4015_InputRange.Pt385_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt385_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt385_0To400:
			fHigh = 400f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt385_Neg200To200:
			fHigh = 200f;
			fLow = -200f;
			break;
		case Adam4015_InputRange.Pt392_Neg50To150:
			fHigh = 150f;
			fLow = -50f;
			break;
		case Adam4015_InputRange.Pt392_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt392_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt392_0To400:
			fHigh = 400f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Pt392_Neg200To200:
			fHigh = 200f;
			fLow = -200f;
			break;
		case Adam4015_InputRange.Pt1000_Neg40To160:
			fHigh = 160f;
			fLow = -40f;
			break;
		case Adam4015_InputRange.Balcon500_Neg30To120:
			fHigh = 120f;
			fLow = -30f;
			break;
		case Adam4015_InputRange.Ni518_Neg80To100:
			fHigh = 100f;
			fLow = -80f;
			break;
		case Adam4015_InputRange.Ni518_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Ni508_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4015_InputRange.Ni508_Neg50To200:
			fHigh = 200f;
			fLow = -50f;
			break;
		case Adam4015_InputRange.BA1_Neg200To600:
			fHigh = 600f;
			fLow = -200f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4015TRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 48, 
			1 => 49, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4015TRangeIndex(byte i_byRange)
	{
		return (Adam4015T_InputRange)i_byRange switch
		{
			Adam4015T_InputRange.Thermistor_3K_0To100 => 0, 
			Adam4015T_InputRange.Thermistor_10K_0To100 => 1, 
			_ => -1, 
		};
	}

	protected static string GetAdam4015TRangeName(byte i_byRange)
	{
		Adam4015T_InputRange adam4015T_InputRange = (Adam4015T_InputRange)i_byRange;
		string result = "";
		switch (adam4015T_InputRange)
		{
		case Adam4015T_InputRange.Thermistor_3K_0To100:
			result = "Thermistor 3K 0~100 'C";
			break;
		case Adam4015T_InputRange.Thermistor_10K_0To100:
			result = "Thermistor 10K 0~100 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4015TScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4015TRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4015TRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4015T_InputRange)i_byRange)
		{
		case Adam4015T_InputRange.Thermistor_3K_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4015T_InputRange.Thermistor_10K_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4016RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 6, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4016RangeIndex(byte i_byRange)
	{
		return (Adam4016_InputRange)i_byRange switch
		{
			Adam4016_InputRange.mV_Neg15To15 => 0, 
			Adam4016_InputRange.mV_Neg50To50 => 1, 
			Adam4016_InputRange.mV_Neg100To100 => 2, 
			Adam4016_InputRange.mV_Neg500To500 => 3, 
			Adam4016_InputRange.mA_Neg20To20 => 4, 
			_ => -1, 
		};
	}

	protected static string GetAdam4016RangeName(byte i_byRange)
	{
		Adam4016_InputRange adam4016_InputRange = (Adam4016_InputRange)i_byRange;
		string result = "";
		switch (adam4016_InputRange)
		{
		case Adam4016_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam4016_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam4016_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4016_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4016_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		}
		return result;
	}

	protected static byte GetAdam4017RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 8, 
			1 => 9, 
			2 => 10, 
			3 => 11, 
			4 => 12, 
			5 => 13, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4017RangeIndex(byte i_byRange)
	{
		return (Adam4017_InputRange)i_byRange switch
		{
			Adam4017_InputRange.V_Neg10To10 => 0, 
			Adam4017_InputRange.V_Neg5To5 => 1, 
			Adam4017_InputRange.V_Neg1To1 => 2, 
			Adam4017_InputRange.mV_Neg500To500 => 3, 
			Adam4017_InputRange.mV_Neg150To150 => 4, 
			Adam4017_InputRange.mA_Neg20To20 => 5, 
			_ => -1, 
		};
	}

	protected static string GetAdam4017RangeName(byte i_byRange)
	{
		Adam4017_InputRange adam4017_InputRange = (Adam4017_InputRange)i_byRange;
		string result = "";
		switch (adam4017_InputRange)
		{
		case Adam4017_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4017_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4017_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4017_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4017_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam4017_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		}
		return result;
	}

	protected static byte GetAdam4017PRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 9, 
			3 => 10, 
			4 => 11, 
			5 => 12, 
			6 => 13, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4017PRangeIndex(byte i_byRange)
	{
		return (Adam4017P_InputRange)i_byRange switch
		{
			Adam4017P_InputRange.mA_4To20 => 0, 
			Adam4017P_InputRange.V_Neg10To10 => 1, 
			Adam4017P_InputRange.V_Neg5To5 => 2, 
			Adam4017P_InputRange.V_Neg1To1 => 3, 
			Adam4017P_InputRange.mV_Neg500To500 => 4, 
			Adam4017P_InputRange.mV_Neg150To150 => 5, 
			Adam4017P_InputRange.mA_0To20 => 6, 
			_ => -1, 
		};
	}

	protected static string GetAdam4017PRangeName(byte i_byRange)
	{
		Adam4017P_InputRange adam4017P_InputRange = (Adam4017P_InputRange)i_byRange;
		string result = "";
		switch (adam4017P_InputRange)
		{
		case Adam4017P_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4017P_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4017P_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4017P_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4017P_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4017P_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam4017P_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam4017PScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4017PRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4017PRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4017P_InputRange)i_byRange)
		{
		case Adam4017P_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4017P_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam4017P_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam4017P_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam4017P_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam4017P_InputRange.mV_Neg150To150:
			fHigh = 150f;
			fLow = -150f;
			break;
		case Adam4017P_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4018RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 14, 
			8 => 15, 
			9 => 16, 
			10 => 17, 
			11 => 18, 
			12 => 19, 
			13 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4018RangeIndex(byte i_byRange)
	{
		return (Adam4018_InputRange)i_byRange switch
		{
			Adam4018_InputRange.mV_Neg15To15 => 0, 
			Adam4018_InputRange.mV_Neg50To50 => 1, 
			Adam4018_InputRange.mV_Neg100To100 => 2, 
			Adam4018_InputRange.mV_Neg500To500 => 3, 
			Adam4018_InputRange.V_Neg1To1 => 4, 
			Adam4018_InputRange.V_Neg2AndHalfTo2AndHalf => 5, 
			Adam4018_InputRange.mA_Neg20To20 => 6, 
			Adam4018_InputRange.Jtype_0To760C => 7, 
			Adam4018_InputRange.Ktype_0To1370C => 8, 
			Adam4018_InputRange.Ttype_Neg100To400C => 9, 
			Adam4018_InputRange.Etype_0To1000C => 10, 
			Adam4018_InputRange.Rtype_500To1750C => 11, 
			Adam4018_InputRange.Stype_500To1750C => 12, 
			Adam4018_InputRange.Btype_500To1800C => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam4018RangeName(byte i_byRange)
	{
		Adam4018_InputRange adam4018_InputRange = (Adam4018_InputRange)i_byRange;
		string result = "";
		switch (adam4018_InputRange)
		{
		case Adam4018_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam4018_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam4018_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4018_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4018_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4018_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam4018_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4018_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4018_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4018_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4018_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4018_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4018_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4018_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static byte GetAdam4018PRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 6, 
			1 => 7, 
			2 => 14, 
			3 => 15, 
			4 => 16, 
			5 => 17, 
			6 => 18, 
			7 => 19, 
			8 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4018PRangeIndex(byte i_byRange)
	{
		return (Adam4018P_InputRange)i_byRange switch
		{
			Adam4018P_InputRange.mA_0To20 => 0, 
			Adam4018P_InputRange.mA_4To20 => 1, 
			Adam4018P_InputRange.Jtype_0To760C => 2, 
			Adam4018P_InputRange.Ktype_0To1370C => 3, 
			Adam4018P_InputRange.Ttype_Neg100To400C => 4, 
			Adam4018P_InputRange.Etype_0To1000C => 5, 
			Adam4018P_InputRange.Rtype_500To1750C => 6, 
			Adam4018P_InputRange.Stype_500To1750C => 7, 
			Adam4018P_InputRange.Btype_500To1800C => 8, 
			_ => -1, 
		};
	}

	protected static string GetAdam4018PRangeName(byte i_byRange)
	{
		Adam4018P_InputRange adam4018P_InputRange = (Adam4018P_InputRange)i_byRange;
		string result = "";
		switch (adam4018P_InputRange)
		{
		case Adam4018P_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4018P_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4018P_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4018P_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4018P_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4018P_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4018P_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4018P_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4018P_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4018PScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4018PRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4018PRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4018P_InputRange)i_byRange)
		{
		case Adam4018P_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam4018P_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4018P_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam4018P_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam4018P_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam4018P_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam4018P_InputRange.Rtype_500To1750C:
		case Adam4018P_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam4018P_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4019RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 2, 
			1 => 3, 
			2 => 4, 
			3 => 5, 
			4 => 8, 
			5 => 9, 
			6 => 13, 
			7 => 14, 
			8 => 15, 
			9 => 16, 
			10 => 17, 
			11 => 18, 
			12 => 19, 
			13 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4019RangeIndex(byte i_byRange)
	{
		return (Adam4019_InputRange)i_byRange switch
		{
			Adam4019_InputRange.mV_Neg100To100 => 0, 
			Adam4019_InputRange.mV_Neg500To500 => 1, 
			Adam4019_InputRange.V_Neg1To1 => 2, 
			Adam4019_InputRange.V_Neg2AndHalfTo2AndHalf => 3, 
			Adam4019_InputRange.V_Neg10To10 => 4, 
			Adam4019_InputRange.V_Neg5To5 => 5, 
			Adam4019_InputRange.mA_Neg20To20 => 6, 
			Adam4019_InputRange.Jtype_0To760C => 7, 
			Adam4019_InputRange.Ktype_0To1370C => 8, 
			Adam4019_InputRange.Ttype_Neg100To400C => 9, 
			Adam4019_InputRange.Etype_0To1000C => 10, 
			Adam4019_InputRange.Rtype_500To1750C => 11, 
			Adam4019_InputRange.Stype_500To1750C => 12, 
			Adam4019_InputRange.Btype_500To1800C => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam4019RangeName(byte i_byRange)
	{
		Adam4019_InputRange adam4019_InputRange = (Adam4019_InputRange)i_byRange;
		string result = "";
		switch (adam4019_InputRange)
		{
		case Adam4019_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4019_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4019_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4019_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam4019_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4019_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4019_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4019_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4019_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4019_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4019_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4019_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4019_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4019_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static byte GetAdam4019PRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 2, 
			1 => 3, 
			2 => 4, 
			3 => 5, 
			4 => 7, 
			5 => 8, 
			6 => 9, 
			7 => 13, 
			8 => 14, 
			9 => 15, 
			10 => 16, 
			11 => 17, 
			12 => 18, 
			13 => 19, 
			14 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4019PRangeIndex(byte i_byRange)
	{
		return (Adam4019P_InputRange)i_byRange switch
		{
			Adam4019P_InputRange.mV_Neg100To100 => 0, 
			Adam4019P_InputRange.mV_Neg500To500 => 1, 
			Adam4019P_InputRange.V_Neg1To1 => 2, 
			Adam4019P_InputRange.V_Neg2AndHalfTo2AndHalf => 3, 
			Adam4019P_InputRange.mA_4To20 => 4, 
			Adam4019P_InputRange.V_Neg10To10 => 5, 
			Adam4019P_InputRange.V_Neg5To5 => 6, 
			Adam4019P_InputRange.mA_Neg20To20 => 7, 
			Adam4019P_InputRange.Jtype_0To760C => 8, 
			Adam4019P_InputRange.Ktype_0To1370C => 9, 
			Adam4019P_InputRange.Ttype_Neg100To400C => 10, 
			Adam4019P_InputRange.Etype_0To1000C => 11, 
			Adam4019P_InputRange.Rtype_500To1750C => 12, 
			Adam4019P_InputRange.Stype_500To1750C => 13, 
			Adam4019P_InputRange.Btype_500To1800C => 14, 
			_ => -1, 
		};
	}

	protected static string GetAdam4019PRangeName(byte i_byRange)
	{
		Adam4019P_InputRange adam4019P_InputRange = (Adam4019P_InputRange)i_byRange;
		string result = "";
		switch (adam4019P_InputRange)
		{
		case Adam4019P_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4019P_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4019P_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4019P_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam4019P_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4019P_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4019P_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4019P_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4019P_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4019P_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4019P_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4019P_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4019P_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4019P_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4019P_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4019PScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4019PRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4019PRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4019P_InputRange)i_byRange)
		{
		case Adam4019P_InputRange.mV_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam4019P_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam4019P_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam4019P_InputRange.V_Neg2AndHalfTo2AndHalf:
			fHigh = 2.5f;
			fLow = -2.5f;
			break;
		case Adam4019P_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4019P_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam4019P_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam4019P_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam4019P_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam4019P_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam4019P_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam4019P_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam4019P_InputRange.Rtype_500To1750C:
		case Adam4019P_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam4019P_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4022TRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 13, 
			3 => 32, 
			4 => 33, 
			5 => 34, 
			6 => 35, 
			7 => 36, 
			8 => 37, 
			9 => 38, 
			10 => 39, 
			11 => 42, 
			12 => 48, 
			13 => 49, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4022TRangeIndex(byte i_byRange)
	{
		return (Adam4022T_InputRange)i_byRange switch
		{
			Adam4022T_InputRange.mA_4To20 => 0, 
			Adam4022T_InputRange.V_0To10 => 1, 
			Adam4022T_InputRange.mA_0To20 => 2, 
			Adam4022T_InputRange.Pt385_Neg100To100 => 3, 
			Adam4022T_InputRange.Pt385_0To100 => 4, 
			Adam4022T_InputRange.Pt385_0To200 => 5, 
			Adam4022T_InputRange.Pt385_0To600 => 6, 
			Adam4022T_InputRange.Pt392_Neg100To100 => 7, 
			Adam4022T_InputRange.Pt392_0To100 => 8, 
			Adam4022T_InputRange.Pt392_0To200 => 9, 
			Adam4022T_InputRange.Pt392_0To600 => 10, 
			Adam4022T_InputRange.Pt1000_Neg40To160 => 11, 
			Adam4022T_InputRange.Thermistor_3K_0To100 => 12, 
			Adam4022T_InputRange.Thermistor_10K_0To100 => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam4022TRangeName(byte i_byRange)
	{
		Adam4022T_InputRange adam4022T_InputRange = (Adam4022T_InputRange)i_byRange;
		string result = "";
		switch (adam4022T_InputRange)
		{
		case Adam4022T_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4022T_InputRange.V_0To10:
			result = "0~10 V";
			break;
		case Adam4022T_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4022T_InputRange.Pt385_Neg100To100:
			result = "Pt(385) -100~100 'C";
			break;
		case Adam4022T_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case Adam4022T_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case Adam4022T_InputRange.Pt385_0To600:
			result = "Pt(385) 0~600 'C";
			break;
		case Adam4022T_InputRange.Pt392_Neg100To100:
			result = "Pt(392) -100~100 'C";
			break;
		case Adam4022T_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case Adam4022T_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case Adam4022T_InputRange.Pt392_0To600:
			result = "Pt(392) 0~600 'C";
			break;
		case Adam4022T_InputRange.Pt1000_Neg40To160:
			result = "Pt(1000) -40~160 'C";
			break;
		case Adam4022T_InputRange.Thermistor_3K_0To100:
			result = "Thermistor 3K 0~100 'C";
			break;
		case Adam4022T_InputRange.Thermistor_10K_0To100:
			result = "Thermistor 10K 0~100 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4022TScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4022TRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4022TRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4022T_InputRange)i_byRange)
		{
		case Adam4022T_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4022T_InputRange.V_0To10:
			fHigh = 10f;
			fLow = 0f;
			break;
		case Adam4022T_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam4022T_InputRange.Pt385_Neg100To100:
		case Adam4022T_InputRange.Pt392_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam4022T_InputRange.Pt385_0To100:
		case Adam4022T_InputRange.Pt392_0To100:
		case Adam4022T_InputRange.Thermistor_3K_0To100:
		case Adam4022T_InputRange.Thermistor_10K_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam4022T_InputRange.Pt385_0To200:
		case Adam4022T_InputRange.Pt392_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam4022T_InputRange.Pt385_0To600:
		case Adam4022T_InputRange.Pt392_0To600:
			fHigh = 600f;
			fLow = 0f;
			break;
		case Adam4022T_InputRange.Pt1000_Neg40To160:
			fHigh = 160f;
			fLow = -40f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4117RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 9, 
			3 => 10, 
			4 => 11, 
			5 => 12, 
			6 => 13, 
			7 => 21, 
			8 => 72, 
			9 => 73, 
			10 => 74, 
			11 => 75, 
			12 => 76, 
			13 => 77, 
			14 => 85, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4117RangeIndex(byte i_byRange)
	{
		return (Adam4117_InputRange)i_byRange switch
		{
			Adam4117_InputRange.mA_4To20 => 0, 
			Adam4117_InputRange.V_Neg10To10 => 1, 
			Adam4117_InputRange.V_Neg5To5 => 2, 
			Adam4117_InputRange.V_Neg1To1 => 3, 
			Adam4117_InputRange.mV_Neg500To500 => 4, 
			Adam4117_InputRange.mV_Neg150To150 => 5, 
			Adam4117_InputRange.mA_Neg20To20 => 6, 
			Adam4117_InputRange.V_Neg15To15 => 7, 
			Adam4117_InputRange.V_0To10 => 8, 
			Adam4117_InputRange.V_0To5 => 9, 
			Adam4117_InputRange.V_0To1 => 10, 
			Adam4117_InputRange.mV_0To500 => 11, 
			Adam4117_InputRange.mV_0To150 => 12, 
			Adam4117_InputRange.mA_0To20 => 13, 
			Adam4117_InputRange.V_0To15 => 14, 
			_ => -1, 
		};
	}

	protected static string GetAdam4117RangeName(byte i_byRange)
	{
		Adam4117_InputRange adam4117_InputRange = (Adam4117_InputRange)i_byRange;
		string result = "";
		switch (adam4117_InputRange)
		{
		case Adam4117_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4117_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam4117_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam4117_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4117_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4117_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam4117_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4117_InputRange.V_Neg15To15:
			result = "+/- 15 V";
			break;
		case Adam4117_InputRange.V_0To10:
			result = "0~10 V";
			break;
		case Adam4117_InputRange.V_0To5:
			result = "0~5 V";
			break;
		case Adam4117_InputRange.V_0To1:
			result = "0~1 V";
			break;
		case Adam4117_InputRange.mV_0To500:
			result = "0~500 mV";
			break;
		case Adam4117_InputRange.mV_0To150:
			result = "0~150 mV";
			break;
		case Adam4117_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam4117_InputRange.V_0To15:
			result = "0~15 V";
			break;
		}
		return result;
	}

	protected static float GetAdam4117ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4117RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4117RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4117_InputRange)i_byRange)
		{
		case Adam4117_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4117_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam4117_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam4117_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam4117_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam4117_InputRange.mV_Neg150To150:
			fHigh = 150f;
			fLow = -150f;
			break;
		case Adam4117_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam4117_InputRange.V_Neg15To15:
			fHigh = 15f;
			fLow = -15f;
			break;
		case Adam4117_InputRange.V_0To10:
			fHigh = 10f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.V_0To5:
			fHigh = 5f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.V_0To1:
			fHigh = 1f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.mV_0To500:
			fHigh = 500f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.mV_0To150:
			fHigh = 150f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam4117_InputRange.V_0To15:
			fHigh = 15f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam4118RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 7, 
			8 => 14, 
			9 => 15, 
			10 => 16, 
			11 => 17, 
			12 => 18, 
			13 => 19, 
			14 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam4118RangeIndex(byte i_byRange)
	{
		return (Adam4118_InputRange)i_byRange switch
		{
			Adam4118_InputRange.mV_Neg15To15 => 0, 
			Adam4118_InputRange.mV_Neg50To50 => 1, 
			Adam4118_InputRange.mV_Neg100To100 => 2, 
			Adam4118_InputRange.mV_Neg500To500 => 3, 
			Adam4118_InputRange.V_Neg1To1 => 4, 
			Adam4118_InputRange.V_Neg2AndHalfTo2AndHalf => 5, 
			Adam4118_InputRange.mA_Neg20To20 => 6, 
			Adam4118_InputRange.mA_4To20 => 7, 
			Adam4118_InputRange.Jtype_0To760C => 8, 
			Adam4118_InputRange.Ktype_0To1370C => 9, 
			Adam4118_InputRange.Ttype_Neg100To400C => 10, 
			Adam4118_InputRange.Etype_0To1000C => 11, 
			Adam4118_InputRange.Rtype_500To1750C => 12, 
			Adam4118_InputRange.Stype_500To1750C => 13, 
			Adam4118_InputRange.Btype_500To1800C => 14, 
			_ => -1, 
		};
	}

	protected static string GetAdam4118RangeName(byte i_byRange)
	{
		Adam4118_InputRange adam4118_InputRange = (Adam4118_InputRange)i_byRange;
		string result = "";
		switch (adam4118_InputRange)
		{
		case Adam4118_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam4118_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam4118_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam4118_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam4118_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam4118_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam4118_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam4118_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam4118_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam4118_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam4118_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam4118_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam4118_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam4118_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam4118_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam4118ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam4118RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam4118RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam4118_InputRange)i_byRange)
		{
		case Adam4118_InputRange.mV_Neg15To15:
			fHigh = 15f;
			fLow = -15f;
			break;
		case Adam4118_InputRange.mV_Neg50To50:
			fHigh = 50f;
			fLow = -50f;
			break;
		case Adam4118_InputRange.mV_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam4118_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam4118_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam4118_InputRange.V_Neg2AndHalfTo2AndHalf:
			fHigh = 2.5f;
			fLow = -2.5f;
			break;
		case Adam4118_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam4118_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam4118_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam4118_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam4118_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam4118_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam4118_InputRange.Rtype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam4118_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam4118_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5013RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 32, 
			1 => 33, 
			2 => 34, 
			3 => 35, 
			4 => 36, 
			5 => 37, 
			6 => 38, 
			7 => 39, 
			8 => 40, 
			9 => 41, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5013RangeIndex(byte i_byRange)
	{
		return (Adam5013_InputRange)i_byRange switch
		{
			Adam5013_InputRange.Pt385_Neg100To100 => 0, 
			Adam5013_InputRange.Pt385_0To100 => 1, 
			Adam5013_InputRange.Pt385_0To200 => 2, 
			Adam5013_InputRange.Pt385_0To600 => 3, 
			Adam5013_InputRange.Pt392_Neg100To100 => 4, 
			Adam5013_InputRange.Pt392_0To100 => 5, 
			Adam5013_InputRange.Pt392_0To200 => 6, 
			Adam5013_InputRange.Pt392_0To600 => 7, 
			Adam5013_InputRange.Ni518_Neg80To100 => 8, 
			Adam5013_InputRange.Ni518_0To100 => 9, 
			_ => -1, 
		};
	}

	protected static string GetAdam5013RangeName(byte i_byRange)
	{
		Adam5013_InputRange adam5013_InputRange = (Adam5013_InputRange)i_byRange;
		string result = "";
		switch (adam5013_InputRange)
		{
		case Adam5013_InputRange.Pt385_Neg100To100:
			result = "Pt(385) -100~100 'C";
			break;
		case Adam5013_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case Adam5013_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case Adam5013_InputRange.Pt385_0To600:
			result = "Pt(385) 0~600 'C";
			break;
		case Adam5013_InputRange.Pt392_Neg100To100:
			result = "Pt(392) -100~100 'C";
			break;
		case Adam5013_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case Adam5013_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case Adam5013_InputRange.Pt392_0To600:
			result = "Pt(392) 0~600 'C";
			break;
		case Adam5013_InputRange.Ni518_Neg80To100:
			result = "Ni(518) -80~100 'C";
			break;
		case Adam5013_InputRange.Ni518_0To100:
			result = "Ni(518) 0~100 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam5013ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5013RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam5013RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5013_InputRange)i_byRange)
		{
		case Adam5013_InputRange.Pt385_Neg100To100:
		case Adam5013_InputRange.Pt392_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam5013_InputRange.Pt385_0To100:
		case Adam5013_InputRange.Pt392_0To100:
		case Adam5013_InputRange.Ni518_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam5013_InputRange.Pt385_0To200:
		case Adam5013_InputRange.Pt392_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam5013_InputRange.Pt385_0To600:
		case Adam5013_InputRange.Pt392_0To600:
			fHigh = 600f;
			fLow = 0f;
			break;
		case Adam5013_InputRange.Ni518_Neg80To100:
			fHigh = 100f;
			fLow = -80f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5017RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 8, 
			1 => 9, 
			2 => 10, 
			3 => 11, 
			4 => 12, 
			5 => 13, 
			6 => 7, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5017RangeIndex(byte i_byRange)
	{
		return (Adam5017_InputRange)i_byRange switch
		{
			Adam5017_InputRange.V_Neg10To10 => 0, 
			Adam5017_InputRange.V_Neg5To5 => 1, 
			Adam5017_InputRange.V_Neg1To1 => 2, 
			Adam5017_InputRange.mV_Neg500To500 => 3, 
			Adam5017_InputRange.mV_Neg150To150 => 4, 
			Adam5017_InputRange.mA_Neg20To20 => 5, 
			Adam5017_InputRange.mA_4To20 => 6, 
			_ => -1, 
		};
	}

	protected static string GetAdam5017RangeName(byte i_byRange)
	{
		Adam5017_InputRange adam5017_InputRange = (Adam5017_InputRange)i_byRange;
		string result = "";
		switch (adam5017_InputRange)
		{
		case Adam5017_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam5017_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam5017_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam5017_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam5017_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam5017_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam5017_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam5017ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5017RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam5017RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5017_InputRange)i_byRange)
		{
		case Adam5017_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam5017_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam5017_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam5017_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam5017_InputRange.mV_Neg150To150:
			fHigh = 150f;
			fLow = -150f;
			break;
		case Adam5017_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam5017_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5017HRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 7, 
			8 => 8, 
			9 => 9, 
			10 => 10, 
			11 => 11, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5017HRangeIndex(byte i_byRange)
	{
		return (Adam5017H_InputRange)i_byRange switch
		{
			Adam5017H_InputRange.V_Neg10To10 => 0, 
			Adam5017H_InputRange.V_0To10 => 1, 
			Adam5017H_InputRange.V_Neg5To5 => 2, 
			Adam5017H_InputRange.V_0To5 => 3, 
			Adam5017H_InputRange.V_Neg2AndHalfTo2AndHalf => 4, 
			Adam5017H_InputRange.V_0To2AndHalf => 5, 
			Adam5017H_InputRange.V_Neg1To1 => 6, 
			Adam5017H_InputRange.V_0To1 => 7, 
			Adam5017H_InputRange.mV_Neg500To500 => 8, 
			Adam5017H_InputRange.mV_0To500 => 9, 
			Adam5017H_InputRange.mA_4To20 => 10, 
			Adam5017H_InputRange.mA_0To20 => 11, 
			_ => -1, 
		};
	}

	protected static string GetAdam5017HRangeName(byte i_byRange)
	{
		Adam5017H_InputRange adam5017H_InputRange = (Adam5017H_InputRange)i_byRange;
		string result = "";
		switch (adam5017H_InputRange)
		{
		case Adam5017H_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam5017H_InputRange.V_0To10:
			result = "0~10 V";
			break;
		case Adam5017H_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam5017H_InputRange.V_0To5:
			result = "0~5 V";
			break;
		case Adam5017H_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam5017H_InputRange.V_0To2AndHalf:
			result = "0~2.5 V";
			break;
		case Adam5017H_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam5017H_InputRange.V_0To1:
			result = "0~1 V";
			break;
		case Adam5017H_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam5017H_InputRange.mV_0To500:
			result = "0~500 mV";
			break;
		case Adam5017H_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam5017H_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam5017HScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5017HRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 4095f + fLow;
	}

	protected static void GetAdam5017HRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5017H_InputRange)i_byRange)
		{
		case Adam5017H_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam5017H_InputRange.V_0To10:
			fHigh = 10f;
			fLow = 0f;
			break;
		case Adam5017H_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam5017H_InputRange.V_0To5:
			fHigh = 5f;
			fLow = 0f;
			break;
		case Adam5017H_InputRange.V_Neg2AndHalfTo2AndHalf:
			fHigh = 2.5f;
			fLow = -2.5f;
			break;
		case Adam5017H_InputRange.V_0To2AndHalf:
			fHigh = 2.5f;
			fLow = 0f;
			break;
		case Adam5017H_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam5017H_InputRange.V_0To1:
			fHigh = 1f;
			fLow = 0f;
			break;
		case Adam5017H_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam5017H_InputRange.mV_0To500:
			fHigh = 500f;
			fLow = 0f;
			break;
		case Adam5017H_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam5017H_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5017UHRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 67, 
			3 => 70, 
			4 => 72, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5017UHRangeIndex(byte i_byRange)
	{
		return (Adam5017UH_InputRange)i_byRange switch
		{
			Adam5017UH_InputRange.mA_4To20 => 0, 
			Adam5017UH_InputRange.V_Neg10To10 => 1, 
			Adam5017UH_InputRange.mV_0To500 => 2, 
			Adam5017UH_InputRange.mA_0To20 => 3, 
			Adam5017UH_InputRange.V_0To10 => 4, 
			_ => -1, 
		};
	}

	protected static string GetAdam5017UHRangeName(byte i_byRange)
	{
		Adam5017UH_InputRange adam5017UH_InputRange = (Adam5017UH_InputRange)i_byRange;
		string result = "";
		switch (adam5017UH_InputRange)
		{
		case Adam5017UH_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam5017UH_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam5017UH_InputRange.mV_0To500:
			result = "0~500 mV";
			break;
		case Adam5017UH_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam5017UH_InputRange.V_0To10:
			result = "0~10 V";
			break;
		}
		return result;
	}

	protected static float GetAdam5017UHScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5017UHRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 4095f + fLow;
	}

	protected static void GetAdam5017UHRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5017UH_InputRange)i_byRange)
		{
		case Adam5017UH_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam5017UH_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam5017UH_InputRange.mV_0To500:
			fHigh = 500f;
			fLow = 0f;
			break;
		case Adam5017UH_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam5017UH_InputRange.V_0To10:
			fHigh = 10f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5017PRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 9, 
			3 => 10, 
			4 => 11, 
			5 => 12, 
			6 => 13, 
			7 => 21, 
			8 => 72, 
			9 => 73, 
			10 => 74, 
			11 => 75, 
			12 => 76, 
			13 => 77, 
			14 => 85, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5017PRangeIndex(byte i_byRange)
	{
		return (Adam5017P_InputRange)i_byRange switch
		{
			Adam5017P_InputRange.mA_4To20 => 0, 
			Adam5017P_InputRange.V_Neg10To10 => 1, 
			Adam5017P_InputRange.V_Neg5To5 => 2, 
			Adam5017P_InputRange.V_Neg1To1 => 3, 
			Adam5017P_InputRange.mV_Neg500To500 => 4, 
			Adam5017P_InputRange.mV_Neg150To150 => 5, 
			Adam5017P_InputRange.mA_Neg20To20 => 6, 
			Adam5017P_InputRange.V_Neg15To15 => 7, 
			Adam5017P_InputRange.V_0To10 => 8, 
			Adam5017P_InputRange.V_0To5 => 9, 
			Adam5017P_InputRange.V_0To1 => 10, 
			Adam5017P_InputRange.mV_0To500 => 11, 
			Adam5017P_InputRange.mV_0To150 => 12, 
			Adam5017P_InputRange.mA_0To20 => 13, 
			Adam5017P_InputRange.V_0To15 => 14, 
			_ => -1, 
		};
	}

	protected static string GetAdam5017PRangeName(byte i_byRange)
	{
		Adam5017P_InputRange adam5017P_InputRange = (Adam5017P_InputRange)i_byRange;
		string result = "";
		switch (adam5017P_InputRange)
		{
		case Adam5017P_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam5017P_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam5017P_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam5017P_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam5017P_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam5017P_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam5017P_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam5017P_InputRange.V_Neg15To15:
			result = "+/- 15 V";
			break;
		case Adam5017P_InputRange.V_0To10:
			result = "0~10 V";
			break;
		case Adam5017P_InputRange.V_0To5:
			result = "0~5 V";
			break;
		case Adam5017P_InputRange.V_0To1:
			result = "0~1 V";
			break;
		case Adam5017P_InputRange.mV_0To500:
			result = "0~500 mV";
			break;
		case Adam5017P_InputRange.mV_0To150:
			result = "0~150 mV";
			break;
		case Adam5017P_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam5017P_InputRange.V_0To15:
			result = "0~15 V";
			break;
		}
		return result;
	}

	protected static float GetAdam5017PScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5017PRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam5017PRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5017P_InputRange)i_byRange)
		{
		case Adam5017P_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam5017P_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam5017P_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam5017P_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam5017P_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam5017P_InputRange.mV_Neg150To150:
			fHigh = 150f;
			fLow = -150f;
			break;
		case Adam5017P_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam5017P_InputRange.V_Neg15To15:
			fHigh = 15f;
			fLow = -15f;
			break;
		case Adam5017P_InputRange.V_0To10:
			fHigh = 10f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.V_0To5:
			fHigh = 5f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.V_0To1:
			fHigh = 1f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.mV_0To500:
			fHigh = 500f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.mV_0To150:
			fHigh = 150f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam5017P_InputRange.V_0To15:
			fHigh = 15f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5018RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 14, 
			8 => 15, 
			9 => 16, 
			10 => 17, 
			11 => 18, 
			12 => 19, 
			13 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5018RangeIndex(byte i_byRange)
	{
		return (Adam5018_InputRange)i_byRange switch
		{
			Adam5018_InputRange.mV_Neg15To15 => 0, 
			Adam5018_InputRange.mV_Neg50To50 => 1, 
			Adam5018_InputRange.mV_Neg100To100 => 2, 
			Adam5018_InputRange.mV_Neg500To500 => 3, 
			Adam5018_InputRange.V_Neg1To1 => 4, 
			Adam5018_InputRange.V_Neg2AndHalfTo2AndHalf => 5, 
			Adam5018_InputRange.mA_Neg20To20 => 6, 
			Adam5018_InputRange.Jtype_0To760C => 7, 
			Adam5018_InputRange.Ktype_0To1370C => 8, 
			Adam5018_InputRange.Ttype_Neg100To400C => 9, 
			Adam5018_InputRange.Etype_0To1000C => 10, 
			Adam5018_InputRange.Rtype_500To1750C => 11, 
			Adam5018_InputRange.Stype_500To1750C => 12, 
			Adam5018_InputRange.Btype_500To1800C => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam5018RangeName(byte i_byRange)
	{
		Adam5018_InputRange adam5018_InputRange = (Adam5018_InputRange)i_byRange;
		string result = "";
		switch (adam5018_InputRange)
		{
		case Adam5018_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam5018_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam5018_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam5018_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam5018_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam5018_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam5018_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam5018_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam5018_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam5018_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam5018_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam5018_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam5018_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam5018_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam5018ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5018RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam5018RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5018_InputRange)i_byRange)
		{
		case Adam5018_InputRange.mV_Neg15To15:
			fHigh = 15f;
			fLow = -15f;
			break;
		case Adam5018_InputRange.mV_Neg50To50:
			fHigh = 50f;
			fLow = -50f;
			break;
		case Adam5018_InputRange.mV_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam5018_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam5018_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam5018_InputRange.V_Neg2AndHalfTo2AndHalf:
			fHigh = 2.5f;
			fLow = -2.5f;
			break;
		case Adam5018_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam5018_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam5018_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam5018_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam5018_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam5018_InputRange.Rtype_500To1750C:
		case Adam5018_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam5018_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam5018PRangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			4 => 4, 
			5 => 5, 
			6 => 6, 
			7 => 7, 
			8 => 14, 
			9 => 15, 
			10 => 16, 
			11 => 17, 
			12 => 18, 
			13 => 19, 
			14 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam5018PRangeIndex(byte i_byRange)
	{
		return (Adam5018P_InputRange)i_byRange switch
		{
			Adam5018P_InputRange.mV_Neg15To15 => 0, 
			Adam5018P_InputRange.mV_Neg50To50 => 1, 
			Adam5018P_InputRange.mV_Neg100To100 => 2, 
			Adam5018P_InputRange.mV_Neg500To500 => 3, 
			Adam5018P_InputRange.V_Neg1To1 => 4, 
			Adam5018P_InputRange.V_Neg2AndHalfTo2AndHalf => 5, 
			Adam5018P_InputRange.mA_Neg20To20 => 6, 
			Adam5018P_InputRange.mA_4To20 => 7, 
			Adam5018P_InputRange.Jtype_0To760C => 8, 
			Adam5018P_InputRange.Ktype_0To1370C => 9, 
			Adam5018P_InputRange.Ttype_Neg100To400C => 10, 
			Adam5018P_InputRange.Etype_0To1000C => 11, 
			Adam5018P_InputRange.Rtype_500To1750C => 12, 
			Adam5018P_InputRange.Stype_500To1750C => 13, 
			Adam5018P_InputRange.Btype_500To1800C => 14, 
			_ => -1, 
		};
	}

	protected static string GetAdam5018PRangeName(byte i_byRange)
	{
		Adam5018P_InputRange adam5018P_InputRange = (Adam5018P_InputRange)i_byRange;
		string result = "";
		switch (adam5018P_InputRange)
		{
		case Adam5018P_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case Adam5018P_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case Adam5018P_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case Adam5018P_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam5018P_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam5018P_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case Adam5018P_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case Adam5018P_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam5018P_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam5018P_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam5018P_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam5018P_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam5018P_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam5018P_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam5018P_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam5018PScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam5018PRangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam5018PRangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam5018P_InputRange)i_byRange)
		{
		case Adam5018P_InputRange.mV_Neg15To15:
			fHigh = 15f;
			fLow = -15f;
			break;
		case Adam5018P_InputRange.mV_Neg50To50:
			fHigh = 50f;
			fLow = -50f;
			break;
		case Adam5018P_InputRange.mV_Neg100To100:
			fHigh = 100f;
			fLow = -100f;
			break;
		case Adam5018P_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam5018P_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam5018P_InputRange.V_Neg2AndHalfTo2AndHalf:
			fHigh = 2.5f;
			fLow = -2.5f;
			break;
		case Adam5018P_InputRange.mA_Neg20To20:
			fHigh = 20f;
			fLow = -20f;
			break;
		case Adam5018P_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam5018P_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam5018P_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam5018P_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam5018P_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam5018P_InputRange.Rtype_500To1750C:
		case Adam5018P_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam5018P_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam6015RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 32, 
			1 => 33, 
			2 => 34, 
			3 => 35, 
			4 => 36, 
			5 => 37, 
			6 => 38, 
			7 => 39, 
			8 => 40, 
			9 => 41, 
			10 => 42, 
			11 => 43, 
			12 => 44, 
			13 => 45, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6015RangeIndex(byte i_byRange)
	{
		return (Adam6015_InputRange)i_byRange switch
		{
			Adam6015_InputRange.Pt385_Neg50To150 => 0, 
			Adam6015_InputRange.Pt385_0To100 => 1, 
			Adam6015_InputRange.Pt385_0To200 => 2, 
			Adam6015_InputRange.Pt385_0To400 => 3, 
			Adam6015_InputRange.Pt385_Neg200To200 => 4, 
			Adam6015_InputRange.Pt392_Neg50To150 => 5, 
			Adam6015_InputRange.Pt392_0To100 => 6, 
			Adam6015_InputRange.Pt392_0To200 => 7, 
			Adam6015_InputRange.Pt392_0To400 => 8, 
			Adam6015_InputRange.Pt392_Neg200To200 => 9, 
			Adam6015_InputRange.Pt1000_Neg40To160 => 10, 
			Adam6015_InputRange.Balcon500_Neg30To120 => 11, 
			Adam6015_InputRange.Ni518_Neg80To100 => 12, 
			Adam6015_InputRange.Ni518_0To100 => 13, 
			_ => -1, 
		};
	}

	protected static string GetAdam6015RangeName(byte i_byRange)
	{
		Adam6015_InputRange adam6015_InputRange = (Adam6015_InputRange)i_byRange;
		string result = "";
		switch (adam6015_InputRange)
		{
		case Adam6015_InputRange.Pt385_Neg50To150:
			result = "Pt(385) -50~150 'C";
			break;
		case Adam6015_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case Adam6015_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case Adam6015_InputRange.Pt385_0To400:
			result = "Pt(385) 0~400 'C";
			break;
		case Adam6015_InputRange.Pt385_Neg200To200:
			result = "Pt(385) -200~200 'C";
			break;
		case Adam6015_InputRange.Pt392_Neg50To150:
			result = "Pt(392) -50~150 'C";
			break;
		case Adam6015_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case Adam6015_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case Adam6015_InputRange.Pt392_0To400:
			result = "Pt(392) 0~400 'C";
			break;
		case Adam6015_InputRange.Pt392_Neg200To200:
			result = "Pt(392) -200~200 'C";
			break;
		case Adam6015_InputRange.Pt1000_Neg40To160:
			result = "Pt(1000) -40~160 'C";
			break;
		case Adam6015_InputRange.Balcon500_Neg30To120:
			result = "Balcon(500) -30~120";
			break;
		case Adam6015_InputRange.Ni518_Neg80To100:
			result = "Ni(518) -80~100 'C";
			break;
		case Adam6015_InputRange.Ni518_0To100:
			result = "Ni(518) 0~100 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam6015ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam6015RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam6015RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam6015_InputRange)i_byRange)
		{
		case Adam6015_InputRange.Pt385_Neg50To150:
			fHigh = 150f;
			fLow = -50f;
			break;
		case Adam6015_InputRange.Pt385_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt385_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt385_0To400:
			fHigh = 400f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt385_Neg200To200:
			fHigh = 200f;
			fLow = -200f;
			break;
		case Adam6015_InputRange.Pt392_Neg50To150:
			fHigh = 150f;
			fLow = -50f;
			break;
		case Adam6015_InputRange.Pt392_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt392_0To200:
			fHigh = 200f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt392_0To400:
			fHigh = 400f;
			fLow = 0f;
			break;
		case Adam6015_InputRange.Pt392_Neg200To200:
			fHigh = 200f;
			fLow = -200f;
			break;
		case Adam6015_InputRange.Pt1000_Neg40To160:
			fHigh = 160f;
			fLow = -40f;
			break;
		case Adam6015_InputRange.Balcon500_Neg30To120:
			fHigh = 120f;
			fLow = -30f;
			break;
		case Adam6015_InputRange.Ni518_Neg80To100:
			fHigh = 100f;
			fLow = -80f;
			break;
		case Adam6015_InputRange.Ni518_0To100:
			fHigh = 100f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam6017RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 8, 
			1 => 9, 
			2 => 10, 
			3 => 11, 
			4 => 12, 
			5 => 13, 
			6 => 7, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6017RangeIndex(byte i_byRange)
	{
		return (Adam6017_InputRange)i_byRange switch
		{
			Adam6017_InputRange.V_Neg10To10 => 0, 
			Adam6017_InputRange.V_Neg5To5 => 1, 
			Adam6017_InputRange.V_Neg1To1 => 2, 
			Adam6017_InputRange.mV_Neg500To500 => 3, 
			Adam6017_InputRange.mV_Neg150To150 => 4, 
			Adam6017_InputRange.mA_0To20 => 5, 
			Adam6017_InputRange.mA_4To20 => 6, 
			_ => -1, 
		};
	}

	protected static string GetAdam6017RangeName(byte i_byRange)
	{
		Adam6017_InputRange adam6017_InputRange = (Adam6017_InputRange)i_byRange;
		string result = "";
		switch (adam6017_InputRange)
		{
		case Adam6017_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam6017_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case Adam6017_InputRange.V_Neg1To1:
			result = "+/- 1 V";
			break;
		case Adam6017_InputRange.mV_Neg500To500:
			result = "+/- 500 mV";
			break;
		case Adam6017_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case Adam6017_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		case Adam6017_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam6017ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam6017RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam6017RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam6017_InputRange)i_byRange)
		{
		case Adam6017_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam6017_InputRange.V_Neg5To5:
			fHigh = 5f;
			fLow = -5f;
			break;
		case Adam6017_InputRange.V_Neg1To1:
			fHigh = 1f;
			fLow = -1f;
			break;
		case Adam6017_InputRange.mV_Neg500To500:
			fHigh = 500f;
			fLow = -500f;
			break;
		case Adam6017_InputRange.mV_Neg150To150:
			fHigh = 150f;
			fLow = -150f;
			break;
		case Adam6017_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		case Adam6017_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam6018RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 14, 
			1 => 15, 
			2 => 16, 
			3 => 17, 
			4 => 18, 
			5 => 19, 
			6 => 20, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6018RangeIndex(byte i_byRange)
	{
		return (Adam6018_InputRange)i_byRange switch
		{
			Adam6018_InputRange.Jtype_0To760C => 0, 
			Adam6018_InputRange.Ktype_0To1370C => 1, 
			Adam6018_InputRange.Ttype_Neg100To400C => 2, 
			Adam6018_InputRange.Etype_0To1000C => 3, 
			Adam6018_InputRange.Rtype_500To1750C => 4, 
			Adam6018_InputRange.Stype_500To1750C => 5, 
			Adam6018_InputRange.Btype_500To1800C => 6, 
			_ => -1, 
		};
	}

	protected static string GetAdam6018RangeName(byte i_byRange)
	{
		Adam6018_InputRange adam6018_InputRange = (Adam6018_InputRange)i_byRange;
		string result = "";
		switch (adam6018_InputRange)
		{
		case Adam6018_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case Adam6018_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case Adam6018_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case Adam6018_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case Adam6018_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case Adam6018_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case Adam6018_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		}
		return result;
	}

	protected static float GetAdam6018ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam6018RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam6018RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam6018_InputRange)i_byRange)
		{
		case Adam6018_InputRange.Jtype_0To760C:
			fHigh = 760f;
			fLow = 0f;
			break;
		case Adam6018_InputRange.Ktype_0To1370C:
			fHigh = 1370f;
			fLow = 0f;
			break;
		case Adam6018_InputRange.Ttype_Neg100To400C:
			fHigh = 400f;
			fLow = -100f;
			break;
		case Adam6018_InputRange.Etype_0To1000C:
			fHigh = 1000f;
			fLow = 0f;
			break;
		case Adam6018_InputRange.Rtype_500To1750C:
		case Adam6018_InputRange.Stype_500To1750C:
			fHigh = 1750f;
			fLow = 500f;
			break;
		case Adam6018_InputRange.Btype_500To1800C:
			fHigh = 1800f;
			fLow = 500f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam6022RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 13, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6022RangeIndex(byte i_byRange)
	{
		return (Adam6022_InputRange)i_byRange switch
		{
			Adam6022_InputRange.mA_4To20 => 0, 
			Adam6022_InputRange.V_Neg10To10 => 1, 
			Adam6022_InputRange.mA_0To20 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam6022RangeName(byte i_byRange)
	{
		Adam6022_InputRange adam6022_InputRange = (Adam6022_InputRange)i_byRange;
		string result = "";
		switch (adam6022_InputRange)
		{
		case Adam6022_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam6022_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam6022_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam6022ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam6022RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam6022RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam6022_InputRange)i_byRange)
		{
		case Adam6022_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam6022_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam6022_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	protected static byte GetAdam6024RangeCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 7, 
			1 => 8, 
			2 => 13, 
			_ => byte.MaxValue, 
		};
	}

	protected static int GetAdam6024RangeIndex(byte i_byRange)
	{
		return (Adam6024_InputRange)i_byRange switch
		{
			Adam6024_InputRange.mA_4To20 => 0, 
			Adam6024_InputRange.V_Neg10To10 => 1, 
			Adam6024_InputRange.mA_0To20 => 2, 
			_ => -1, 
		};
	}

	protected static string GetAdam6024RangeName(byte i_byRange)
	{
		Adam6024_InputRange adam6024_InputRange = (Adam6024_InputRange)i_byRange;
		string result = "";
		switch (adam6024_InputRange)
		{
		case Adam6024_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case Adam6024_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case Adam6024_InputRange.mA_0To20:
			result = "0~20 mA";
			break;
		}
		return result;
	}

	protected static float GetAdam6024ScaledValue(byte i_byRange, int i_iValue)
	{
		GetAdam6024RangeHighLow(i_byRange, out var fHigh, out var fLow);
		return (fHigh - fLow) * Convert.ToSingle(i_iValue) / 65535f + fLow;
	}

	protected static void GetAdam6024RangeHighLow(byte i_byRange, out float fHigh, out float fLow)
	{
		switch ((Adam6024_InputRange)i_byRange)
		{
		case Adam6024_InputRange.mA_4To20:
			fHigh = 20f;
			fLow = 4f;
			break;
		case Adam6024_InputRange.V_Neg10To10:
			fHigh = 10f;
			fLow = -10f;
			break;
		case Adam6024_InputRange.mA_0To20:
			fHigh = 20f;
			fLow = 0f;
			break;
		default:
			fHigh = 0f;
			fLow = 0f;
			break;
		}
	}

	public static float GetScaledValue(byte i_byRange, int i_iValue)
	{
		GetRangeHighLow(i_byRange, out var o_fHigh, out var o_fLow);
		return (o_fHigh - o_fLow) * Convert.ToSingle(i_iValue) / 65535f + o_fLow;
	}

	public static void GetRangeHighLow(byte i_byRange, out float o_fHigh, out float o_fLow)
	{
		switch ((AdamUnknown_InputRange)i_byRange)
		{
		case AdamUnknown_InputRange.mV_Neg15To15:
			o_fHigh = 15f;
			o_fLow = -15f;
			break;
		case AdamUnknown_InputRange.mV_Neg50To50:
			o_fHigh = 50f;
			o_fLow = -50f;
			break;
		case AdamUnknown_InputRange.mV_Neg100To100:
			o_fHigh = 100f;
			o_fLow = -100f;
			break;
		case AdamUnknown_InputRange.mV_Neg500To500:
		case AdamUnknown_InputRange.mV_Neg500To500_Old:
			o_fHigh = 500f;
			o_fLow = -500f;
			break;
		case AdamUnknown_InputRange.V_Neg1To1:
		case AdamUnknown_InputRange.V_Neg1To1_Old:
			o_fHigh = 1f;
			o_fLow = -1f;
			break;
		case AdamUnknown_InputRange.V_Neg2AndHalfTo2AndHalf:
			o_fHigh = 2.5f;
			o_fLow = -2.5f;
			break;
		case AdamUnknown_InputRange.mA_Neg20To20:
			o_fHigh = 20f;
			o_fLow = -20f;
			break;
		case AdamUnknown_InputRange.mA_4To20:
			o_fHigh = 20f;
			o_fLow = 4f;
			break;
		case AdamUnknown_InputRange.V_Neg10To10:
			o_fHigh = 10f;
			o_fLow = -10f;
			break;
		case AdamUnknown_InputRange.V_Neg5To5:
			o_fHigh = 5f;
			o_fLow = -5f;
			break;
		case AdamUnknown_InputRange.mV_Neg150To150:
			o_fHigh = 150f;
			o_fLow = -150f;
			break;
		case AdamUnknown_InputRange.mA_0To20:
		case AdamUnknown_InputRange.mA_0To20_Old:
		case AdamUnknown_InputRange.mA_0To20_Old1:
			o_fHigh = 20f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Jtype_0To760C:
			o_fHigh = 760f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Ktype_0To1370C:
			o_fHigh = 1370f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Ttype_Neg100To400C:
			o_fHigh = 400f;
			o_fLow = -100f;
			break;
		case AdamUnknown_InputRange.Etype_0To1000C:
			o_fHigh = 1000f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Rtype_500To1750C:
		case AdamUnknown_InputRange.Stype_500To1750C:
			o_fHigh = 1750f;
			o_fLow = 500f;
			break;
		case AdamUnknown_InputRange.Btype_500To1800C:
			o_fHigh = 1800f;
			o_fLow = 500f;
			break;
		case AdamUnknown_InputRange.V_Neg15To15:
			o_fHigh = 15f;
			o_fLow = -15f;
			break;
		case AdamUnknown_InputRange.Pt392_Neg50To150:
			o_fHigh = 150f;
			o_fLow = -50f;
			break;
		case AdamUnknown_InputRange.Pt385_Neg200To200:
			o_fHigh = 200f;
			o_fLow = -200f;
			break;
		case AdamUnknown_InputRange.Pt385_0To400:
			o_fHigh = 400f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Pt385_Neg50To150:
			o_fHigh = 150f;
			o_fLow = -50f;
			break;
		case AdamUnknown_InputRange.Pt385_Neg100To100:
		case AdamUnknown_InputRange.Pt392_Neg100To100:
			o_fHigh = 100f;
			o_fLow = -100f;
			break;
		case AdamUnknown_InputRange.Pt385_0To100:
		case AdamUnknown_InputRange.Pt392_0To100:
			o_fHigh = 100f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Pt385_0To200:
		case AdamUnknown_InputRange.Pt392_0To200:
			o_fHigh = 200f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Pt385_0To600:
		case AdamUnknown_InputRange.Pt392_0To600:
			o_fHigh = 600f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Pt392_0To400:
			o_fHigh = 400f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Pt392_Neg200To200:
			o_fHigh = 200f;
			o_fLow = -200f;
			break;
		case AdamUnknown_InputRange.Pt1000_Neg40To160:
			o_fHigh = 160f;
			o_fLow = -40f;
			break;
		case AdamUnknown_InputRange.Balcon500_Neg30To120:
			o_fHigh = 120f;
			o_fLow = -30f;
			break;
		case AdamUnknown_InputRange.Ni518_0To100:
			o_fHigh = 100f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Ni518_Neg80To100:
			o_fHigh = 100f;
			o_fLow = -80f;
			break;
		case AdamUnknown_InputRange.Thermistor_3K_0To100:
		case AdamUnknown_InputRange.Thermistor_10K_0To100:
			o_fHigh = 100f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Ni508_0To100:
			o_fHigh = 100f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.Ni508_Neg50To200:
			o_fHigh = 200f;
			o_fLow = -50f;
			break;
		case AdamUnknown_InputRange.V_0To10:
			o_fHigh = 10f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.V_0To5:
			o_fHigh = 5f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.V_0To1:
			o_fHigh = 1f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.mV_0To500_Old:
		case AdamUnknown_InputRange.mV_0To500:
			o_fHigh = 500f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.mV_0To150:
			o_fHigh = 150f;
			o_fLow = 0f;
			break;
		case AdamUnknown_InputRange.V_0To15:
			o_fHigh = 15f;
			o_fLow = 0f;
			break;
		default:
			o_fHigh = 0f;
			o_fLow = 0f;
			break;
		}
	}

	public static string GetRangeName(byte i_byRange)
	{
		AdamUnknown_InputRange adamUnknown_InputRange = (AdamUnknown_InputRange)i_byRange;
		string result = "";
		switch (adamUnknown_InputRange)
		{
		case AdamUnknown_InputRange.mV_Neg15To15:
			result = "+/- 15 mV";
			break;
		case AdamUnknown_InputRange.mV_Neg50To50:
			result = "+/- 50 mV";
			break;
		case AdamUnknown_InputRange.mV_Neg100To100:
			result = "+/- 100 mV";
			break;
		case AdamUnknown_InputRange.mV_Neg500To500:
		case AdamUnknown_InputRange.mV_Neg500To500_Old:
			result = "+/- 500 mV";
			break;
		case AdamUnknown_InputRange.V_Neg1To1:
		case AdamUnknown_InputRange.V_Neg1To1_Old:
			result = "+/- 1 V";
			break;
		case AdamUnknown_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "+/- 2.5 V";
			break;
		case AdamUnknown_InputRange.mA_Neg20To20:
			result = "+/- 20 mA";
			break;
		case AdamUnknown_InputRange.mA_4To20:
			result = "4~20 mA";
			break;
		case AdamUnknown_InputRange.V_Neg10To10:
			result = "+/- 10 V";
			break;
		case AdamUnknown_InputRange.V_Neg5To5:
			result = "+/- 5 V";
			break;
		case AdamUnknown_InputRange.mV_Neg150To150:
			result = "+/- 150 mV";
			break;
		case AdamUnknown_InputRange.mA_0To20:
		case AdamUnknown_InputRange.mA_0To20_Old:
		case AdamUnknown_InputRange.mA_0To20_Old1:
			result = "0~20 mA";
			break;
		case AdamUnknown_InputRange.Jtype_0To760C:
			result = "T/C TypeJ 0~760 'C";
			break;
		case AdamUnknown_InputRange.Ktype_0To1370C:
			result = "T/C TypeK 0~1370 'C";
			break;
		case AdamUnknown_InputRange.Ttype_Neg100To400C:
			result = "T/C TypeT -100~400 'C";
			break;
		case AdamUnknown_InputRange.Etype_0To1000C:
			result = "T/C TypeE 0~1000 'C";
			break;
		case AdamUnknown_InputRange.Rtype_500To1750C:
			result = "T/C TypeR 500~1750 'C";
			break;
		case AdamUnknown_InputRange.Stype_500To1750C:
			result = "T/C TypeS 500~1750 'C";
			break;
		case AdamUnknown_InputRange.Btype_500To1800C:
			result = "T/C TypeB 500~1800 'C";
			break;
		case AdamUnknown_InputRange.V_Neg15To15:
			result = "+/- 15 V";
			break;
		case AdamUnknown_InputRange.Pt392_Neg50To150:
			result = "Pt(392) -50~150 'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg200To200:
			result = "Pt(385) -200~200 'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To400:
			result = "Pt(385) 0~400 'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg50To150:
			result = "Pt(385) -50~150 'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg100To100:
			result = "Pt(385) -100~100 'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To100:
			result = "Pt(385) 0~100 'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To200:
			result = "Pt(385) 0~200 'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To600:
			result = "Pt(385) 0~600 'C";
			break;
		case AdamUnknown_InputRange.Pt392_Neg100To100:
			result = "Pt(392) -100~100 'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To100:
			result = "Pt(392) 0~100 'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To200:
			result = "Pt(392) 0~200 'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To600:
			result = "Pt(392) 0~600 'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To400:
			result = "Pt(392) 0~400 'C";
			break;
		case AdamUnknown_InputRange.Pt392_Neg200To200:
			result = "Pt(392) -200~200 'C";
			break;
		case AdamUnknown_InputRange.Pt1000_Neg40To160:
			result = "Pt(1000) -40~160 'C";
			break;
		case AdamUnknown_InputRange.Balcon500_Neg30To120:
			result = "Balcon(500) -30~120";
			break;
		case AdamUnknown_InputRange.Ni518_Neg80To100:
			result = "Ni(518) -80~100 'C";
			break;
		case AdamUnknown_InputRange.Ni518_0To100:
			result = "Ni(518) 0~100 'C";
			break;
		case AdamUnknown_InputRange.Thermistor_3K_0To100:
			result = "Thermistor 3K 0~100 'C";
			break;
		case AdamUnknown_InputRange.Thermistor_10K_0To100:
			result = "Thermistor 10K 0~100 'C";
			break;
		case AdamUnknown_InputRange.Ni508_0To100:
			result = "Ni(508) 0~100 'C";
			break;
		case AdamUnknown_InputRange.Ni508_Neg50To200:
			result = "Ni(508) -50~200 'C";
			break;
		case AdamUnknown_InputRange.V_0To10:
			result = "0~10 V";
			break;
		case AdamUnknown_InputRange.V_0To5:
			result = "0~5 V";
			break;
		case AdamUnknown_InputRange.V_0To1:
			result = "0~1 V";
			break;
		case AdamUnknown_InputRange.mV_0To500_Old:
		case AdamUnknown_InputRange.mV_0To500:
			result = "0~500 mV";
			break;
		case AdamUnknown_InputRange.mV_0To150:
			result = "0~150 mV";
			break;
		case AdamUnknown_InputRange.V_0To15:
			result = "0~15 V";
			break;
		}
		return result;
	}

	public static string GetDataFormatName(byte i_byDataFormat)
	{
		string result = "";
		switch ((AdamUnknown_DataFormat)i_byDataFormat)
		{
		case AdamUnknown_DataFormat.EngineerUnit:
			result = "Engineering Unit";
			break;
		case AdamUnknown_DataFormat.Percent:
			result = "%FSR";
			break;
		case AdamUnknown_DataFormat.TwosComplementHex:
			result = "Two's complement Hexdicimal";
			break;
		case AdamUnknown_DataFormat.Ohms:
			result = "Ohms";
			break;
		}
		return result;
	}

	public static string GetFloatFormat(byte i_byRange)
	{
		AdamUnknown_InputRange adamUnknown_InputRange = (AdamUnknown_InputRange)i_byRange;
		string result = "";
		switch (adamUnknown_InputRange)
		{
		case AdamUnknown_InputRange.mV_Neg15To15:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_Neg50To50:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_Neg100To100:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_Neg500To500:
		case AdamUnknown_InputRange.mV_Neg500To500_Old:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_Neg1To1:
		case AdamUnknown_InputRange.V_Neg1To1_Old:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mA_Neg20To20:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mA_4To20:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_Neg10To10:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_Neg5To5:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_Neg150To150:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mA_0To20:
		case AdamUnknown_InputRange.mA_0To20_Old:
		case AdamUnknown_InputRange.mA_0To20_Old1:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.Jtype_0To760C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ktype_0To1370C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ttype_Neg100To400C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Etype_0To1000C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Rtype_500To1750C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Stype_500To1750C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Btype_500To1800C:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.V_Neg15To15:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.Pt392_Neg50To150:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_Neg200To200:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_0To400:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_Neg50To150:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_Neg100To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_0To200:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt385_0To600:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_Neg100To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_0To200:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_0To600:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_0To400:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt392_Neg200To200:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Pt1000_Neg40To160:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Balcon500_Neg30To120:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ni518_Neg80To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ni518_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Thermistor_3K_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Thermistor_10K_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ni508_0To100:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.Ni508_Neg50To200:
			result = "0.0;-0.0";
			break;
		case AdamUnknown_InputRange.V_0To10:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_0To5:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_0To1:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_0To500_Old:
		case AdamUnknown_InputRange.mV_0To500:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.mV_0To150:
			result = "0.000;-0.000";
			break;
		case AdamUnknown_InputRange.V_0To15:
			result = "0.000;-0.000";
			break;
		}
		return result;
	}

	public static string GetUnitName(byte i_byRange)
	{
		AdamUnknown_InputRange adamUnknown_InputRange = (AdamUnknown_InputRange)i_byRange;
		string result = "";
		switch (adamUnknown_InputRange)
		{
		case AdamUnknown_InputRange.mV_Neg15To15:
			result = "mV";
			break;
		case AdamUnknown_InputRange.mV_Neg50To50:
			result = "mV";
			break;
		case AdamUnknown_InputRange.mV_Neg100To100:
			result = "mV";
			break;
		case AdamUnknown_InputRange.mV_Neg500To500:
		case AdamUnknown_InputRange.mV_Neg500To500_Old:
			result = "mV";
			break;
		case AdamUnknown_InputRange.V_Neg1To1:
		case AdamUnknown_InputRange.V_Neg1To1_Old:
			result = "V";
			break;
		case AdamUnknown_InputRange.V_Neg2AndHalfTo2AndHalf:
			result = "V";
			break;
		case AdamUnknown_InputRange.mA_Neg20To20:
			result = "mA";
			break;
		case AdamUnknown_InputRange.mA_4To20:
			result = "mA";
			break;
		case AdamUnknown_InputRange.V_Neg10To10:
			result = "V";
			break;
		case AdamUnknown_InputRange.V_Neg5To5:
			result = "V";
			break;
		case AdamUnknown_InputRange.mV_Neg150To150:
			result = "mV";
			break;
		case AdamUnknown_InputRange.mA_0To20:
		case AdamUnknown_InputRange.mA_0To20_Old:
		case AdamUnknown_InputRange.mA_0To20_Old1:
			result = "mA";
			break;
		case AdamUnknown_InputRange.Jtype_0To760C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ktype_0To1370C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ttype_Neg100To400C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Etype_0To1000C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Rtype_500To1750C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Stype_500To1750C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Btype_500To1800C:
			result = "'C";
			break;
		case AdamUnknown_InputRange.V_Neg15To15:
			result = "V";
			break;
		case AdamUnknown_InputRange.Pt392_Neg50To150:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg200To200:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To400:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg50To150:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_Neg100To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To200:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt385_0To600:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_Neg100To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To200:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To600:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_0To400:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt392_Neg200To200:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Pt1000_Neg40To160:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Balcon500_Neg30To120:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ni518_Neg80To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ni518_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Thermistor_3K_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Thermistor_10K_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ni508_0To100:
			result = "'C";
			break;
		case AdamUnknown_InputRange.Ni508_Neg50To200:
			result = "'C";
			break;
		case AdamUnknown_InputRange.V_0To10:
			result = "V";
			break;
		case AdamUnknown_InputRange.V_0To5:
			result = "V";
			break;
		case AdamUnknown_InputRange.V_0To1:
			result = "V";
			break;
		case AdamUnknown_InputRange.mV_0To500_Old:
		case AdamUnknown_InputRange.mV_0To500:
			result = "mV";
			break;
		case AdamUnknown_InputRange.mV_0To150:
			result = "mV";
			break;
		case AdamUnknown_InputRange.V_0To15:
			result = "V";
			break;
		}
		return result;
	}

	public static float GetScaledValue(Adam4000Type adam4000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4015:
			result = GetAdam4015ScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4015T:
			result = GetAdam4015ScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4017P:
			result = GetAdam4017PScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4018P:
			result = GetAdam4018PScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4019P:
			result = GetAdam4019PScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4117:
			result = GetAdam4117ScaledValue(i_byRange, i_iValue);
			break;
		case Adam4000Type.Adam4118:
			result = GetAdam4118ScaledValue(i_byRange, i_iValue);
			break;
		}
		return result;
	}

	public static void GetRangeHighLow(Adam4000Type adam4000Type, byte i_byRange, out float o_fHigh, out float o_fLow)
	{
		o_fHigh = 10f;
		o_fLow = 0f;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4015:
			GetAdam4015RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4015T:
			GetAdam4015RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4017P:
			GetAdam4017PRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4018P:
			GetAdam4018PRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4019P:
			GetAdam4019PRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4022T:
			GetAdam4022TRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4117:
			GetAdam4117RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam4000Type.Adam4118:
			GetAdam4118RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		}
	}

	public static float GetScaledValue(Adam5000Type adam5000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = GetAdam5013ScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5017:
			result = GetAdam5017ScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5017H:
			result = GetAdam5017HScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5017UH:
			result = GetAdam5017UHScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5017P:
			result = GetAdam5017PScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5018:
			result = GetAdam5018ScaledValue(i_byRange, i_iValue);
			break;
		case Adam5000Type.Adam5018P:
			result = GetAdam5018PScaledValue(i_byRange, i_iValue);
			break;
		}
		return result;
	}

	public static void GetRangeHighLow(Adam5000Type adam5000Type, byte i_byRange, out float o_fHigh, out float o_fLow)
	{
		o_fHigh = 10f;
		o_fLow = 0f;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			GetAdam5013RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5017:
			GetAdam5017RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5017H:
			GetAdam5017HRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5017UH:
			GetAdam5017UHRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5017P:
			GetAdam5017PRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5018:
			GetAdam5018RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam5000Type.Adam5018P:
			GetAdam5018PRangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		}
	}

	public static float GetScaledValue(Adam6000Type adam6000Type, byte i_byRange, int i_iValue)
	{
		float result = 0f;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = GetAdam6015ScaledValue(i_byRange, i_iValue);
			break;
		case Adam6000Type.Adam6017:
			result = GetAdam6017ScaledValue(i_byRange, i_iValue);
			break;
		case Adam6000Type.Adam6018:
			result = GetAdam6018ScaledValue(i_byRange, i_iValue);
			break;
		case Adam6000Type.Adam6022:
			result = GetAdam6022ScaledValue(i_byRange, i_iValue);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024ScaledValue(i_byRange, i_iValue);
			break;
		}
		return result;
	}

	public static void GetRangeHighLow(Adam6000Type adam6000Type, byte i_byRange, out float o_fHigh, out float o_fLow)
	{
		o_fHigh = 10f;
		o_fLow = 0f;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			GetAdam6015RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam6000Type.Adam6017:
			GetAdam6017RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam6000Type.Adam6018:
			GetAdam6018RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam6000Type.Adam6022:
			GetAdam6022RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		case Adam6000Type.Adam6024:
			GetAdam6024RangeHighLow(i_byRange, out o_fHigh, out o_fLow);
			break;
		}
	}

	public static byte GetRangeCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4011D:
			result = GetAdam4011RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4012:
			result = GetAdam4012RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4013:
			result = GetAdam4013RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4015:
			result = GetAdam4015RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4015T:
			result = GetAdam4015TRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4016:
			result = GetAdam4016RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4017:
			result = GetAdam4017RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4017P:
			result = GetAdam4017PRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4018M:
			result = GetAdam4018RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4018P:
			result = GetAdam4018PRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4019:
			result = GetAdam4019RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4019P:
			result = GetAdam4019PRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4117:
			result = GetAdam4117RangeCode(i_iIndex);
			break;
		case Adam4000Type.Adam4118:
			result = GetAdam4118RangeCode(i_iIndex);
			break;
		}
		return result;
	}

	public static byte GetRangeCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = GetAdam5013RangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5017:
			result = GetAdam5017RangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5017H:
			result = GetAdam5017HRangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5017UH:
			result = GetAdam5017UHRangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5017P:
			result = GetAdam5017PRangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5018:
			result = GetAdam5018RangeCode(i_iIndex);
			break;
		case Adam5000Type.Adam5018P:
			result = GetAdam5018PRangeCode(i_iIndex);
			break;
		}
		return result;
	}

	public static byte GetRangeCode(Adam6000Type adam6000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = GetAdam6015RangeCode(i_iIndex);
			break;
		case Adam6000Type.Adam6017:
			result = GetAdam6017RangeCode(i_iIndex);
			break;
		case Adam6000Type.Adam6018:
			result = GetAdam6018RangeCode(i_iIndex);
			break;
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeCode(i_iIndex);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeCode(i_iIndex);
			break;
		}
		return result;
	}

	public static string GetRangeName(Adam4000Type adam4000Type, byte i_byRange)
	{
		string result = "";
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4011D:
			result = GetAdam4011RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4012:
			result = GetAdam4012RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4013:
			result = GetAdam4013RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4015:
			result = GetAdam4015RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4015T:
			result = GetAdam4015TRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4016:
			result = GetAdam4016RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4017:
			result = GetAdam4017RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4017P:
			result = GetAdam4017PRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4018M:
			result = GetAdam4018RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4018P:
			result = GetAdam4018PRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4019:
			result = GetAdam4019RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4019P:
			result = GetAdam4019PRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeName(i_byRange);
			break;
		case Adam4000Type.Adam4117:
			result = GetAdam4117RangeName(i_byRange);
			break;
		case Adam4000Type.Adam4118:
			result = GetAdam4118RangeName(i_byRange);
			break;
		}
		return result;
	}

	public static string GetRangeName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = GetAdam5013RangeName(i_byRange);
			break;
		case Adam5000Type.Adam5017:
			result = GetAdam5017RangeName(i_byRange);
			break;
		case Adam5000Type.Adam5017H:
			result = GetAdam5017HRangeName(i_byRange);
			break;
		case Adam5000Type.Adam5017UH:
			result = GetAdam5017UHRangeName(i_byRange);
			break;
		case Adam5000Type.Adam5017P:
			result = GetAdam5017PRangeName(i_byRange);
			break;
		case Adam5000Type.Adam5018:
			result = GetAdam5018RangeName(i_byRange);
			break;
		case Adam5000Type.Adam5018P:
			result = GetAdam5018PRangeName(i_byRange);
			break;
		}
		return result;
	}

	public static string GetRangeName(Adam6000Type adam6000Type, byte i_byRange)
	{
		string result = "";
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = GetAdam6015RangeName(i_byRange);
			break;
		case Adam6000Type.Adam6017:
			result = GetAdam6017RangeName(i_byRange);
			break;
		case Adam6000Type.Adam6018:
			result = GetAdam6018RangeName(i_byRange);
			break;
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeName(i_byRange);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeName(i_byRange);
			break;
		}
		return result;
	}

	public static int GetRangeIndex(Adam4000Type adam4000Type, byte i_byRange)
	{
		int result = -1;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4011D:
			result = GetAdam4011RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4012:
			result = GetAdam4012RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4013:
			result = GetAdam4013RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4015:
			result = GetAdam4015RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4015T:
			result = GetAdam4015TRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4016:
			result = GetAdam4016RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4017:
			result = GetAdam4017RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4017P:
			result = GetAdam4017PRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4018M:
			result = GetAdam4018RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4018P:
			result = GetAdam4018PRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4019:
			result = GetAdam4019RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4019P:
			result = GetAdam4019PRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4022T:
			result = GetAdam4022TRangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4117:
			result = GetAdam4117RangeIndex(i_byRange);
			break;
		case Adam4000Type.Adam4118:
			result = GetAdam4118RangeIndex(i_byRange);
			break;
		}
		return result;
	}

	public static int GetRangeIndex(Adam5000Type adam5000Type, byte i_byRange)
	{
		int result = -1;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = GetAdam5013RangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5017:
			result = GetAdam5017RangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5017H:
			result = GetAdam5017HRangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5017UH:
			result = GetAdam5017UHRangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5017P:
			result = GetAdam5017PRangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5018:
			result = GetAdam5018RangeIndex(i_byRange);
			break;
		case Adam5000Type.Adam5018P:
			result = GetAdam5018PRangeIndex(i_byRange);
			break;
		}
		return result;
	}

	public static int GetRangeIndex(Adam6000Type adam6000Type, byte i_byRange)
	{
		int result = -1;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = GetAdam6015RangeIndex(i_byRange);
			break;
		case Adam6000Type.Adam6017:
			result = GetAdam6017RangeIndex(i_byRange);
			break;
		case Adam6000Type.Adam6018:
			result = GetAdam6018RangeIndex(i_byRange);
			break;
		case Adam6000Type.Adam6022:
			result = GetAdam6022RangeIndex(i_byRange);
			break;
		case Adam6000Type.Adam6024:
			result = GetAdam6024RangeIndex(i_byRange);
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4013:
		case Adam4000Type.Adam4016:
		case Adam4000Type.Adam4011D:
			result = 1;
			break;
		case Adam4000Type.Adam4022T:
			result = 4;
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			result = 6;
			break;
		case Adam4000Type.Adam4017:
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4019:
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
		case Adam4000Type.Adam4017P:
		case Adam4000Type.Adam4018P:
		case Adam4000Type.Adam4019P:
		case Adam4000Type.Adam4018M:
			result = 8;
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = 3;
			break;
		case Adam5000Type.Adam5017:
			result = 8;
			break;
		case Adam5000Type.Adam5017H:
			result = 8;
			break;
		case Adam5000Type.Adam5017UH:
			result = 8;
			break;
		case Adam5000Type.Adam5017P:
			result = 8;
			break;
		case Adam5000Type.Adam5018:
			result = 7;
			break;
		case Adam5000Type.Adam5018P:
			result = 7;
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = 7;
			break;
		case Adam6000Type.Adam6017:
			result = 8;
			break;
		case Adam6000Type.Adam6018:
			result = 8;
			break;
		case Adam6000Type.Adam6022:
			result = 6;
			break;
		case Adam6000Type.Adam6024:
			result = 6;
			break;
		}
		return result;
	}

	public static int GetRangeTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4019:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4022T:
		case Adam4000Type.Adam4018M:
			result = 14;
			break;
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4017:
			result = 6;
			break;
		case Adam4000Type.Adam4013:
			result = 10;
			break;
		case Adam4000Type.Adam4015:
			result = 17;
			break;
		case Adam4000Type.Adam4015T:
			result = 2;
			break;
		case Adam4000Type.Adam4016:
			result = 5;
			break;
		case Adam4000Type.Adam4017P:
			result = 7;
			break;
		case Adam4000Type.Adam4018P:
			result = 9;
			break;
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
		case Adam4000Type.Adam4019P:
			result = 15;
			break;
		}
		return result;
	}

	public static int GetRangeTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = 10;
			break;
		case Adam5000Type.Adam5017:
			result = 7;
			break;
		case Adam5000Type.Adam5017H:
			result = 12;
			break;
		case Adam5000Type.Adam5017UH:
			result = 5;
			break;
		case Adam5000Type.Adam5017P:
			result = 15;
			break;
		case Adam5000Type.Adam5018:
			result = 14;
			break;
		case Adam5000Type.Adam5018P:
			result = 15;
			break;
		}
		return result;
	}

	public static int GetRangeTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = 14;
			break;
		case Adam6000Type.Adam6017:
			result = 7;
			break;
		case Adam6000Type.Adam6018:
			result = 7;
			break;
		case Adam6000Type.Adam6022:
			result = 3;
			break;
		case Adam6000Type.Adam6024:
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
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4019:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018M:
			result = ((GetRangeIndex(adam4000Type, i_byRange) < 7) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		case Adam4000Type.Adam4019P:
			result = ((GetRangeIndex(adam4000Type, i_byRange) < 8) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		case Adam4000Type.Adam4018P:
			result = ((GetRangeIndex(adam4000Type, i_byRange) < 2) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		case Adam4000Type.Adam4022T:
			result = ((GetRangeIndex(adam4000Type, i_byRange) <= 2) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4017:
		case Adam4000Type.Adam4017P:
			result = "0.000;-0.000";
			break;
		case Adam4000Type.Adam4013:
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			result = "0.0;-0.0";
			break;
		case Adam4000Type.Adam4016:
			result = "0.000;-0.000";
			break;
		case Adam4000Type.Adam4117:
			result = "0.000;-0.000";
			break;
		case Adam4000Type.Adam4118:
			result = ((GetRangeIndex(adam4000Type, i_byRange) < 8) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		}
		return result;
	}

	public static string GetFloatFormat(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "0.0;-0.0";
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = "0.0;-0.0";
			break;
		case Adam5000Type.Adam5017:
			result = "0.000;-0.000";
			break;
		case Adam5000Type.Adam5017H:
			result = "0.000;-0.000";
			break;
		case Adam5000Type.Adam5017UH:
			result = "0.000;-0.000";
			break;
		case Adam5000Type.Adam5017P:
			result = "0.000;-0.000";
			break;
		case Adam5000Type.Adam5018:
			result = ((GetRangeIndex(adam5000Type, i_byRange) < 7) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		case Adam5000Type.Adam5018P:
			result = ((GetRangeIndex(adam5000Type, i_byRange) < 8) ? "0.000;-0.000" : "0.0;-0.0");
			break;
		}
		return result;
	}

	public static string GetFloatFormat(Adam6000Type adam6000Type, byte i_byRange)
	{
		string result = "0.0;-0.0";
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			result = "0.0;-0.0";
			break;
		case Adam6000Type.Adam6017:
			result = "0.000;-0.000";
			break;
		case Adam6000Type.Adam6018:
			result = "0.0;-0.0";
			break;
		case Adam6000Type.Adam6022:
			result = "0.000;-0.000";
			break;
		case Adam6000Type.Adam6024:
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
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018M:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "mV";
				break;
			}
			if (rangeIndex >= 4 && rangeIndex <= 5)
			{
				result = "V";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "'C";
				break;
			}
			break;
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4017:
			if (rangeIndex >= 0 && rangeIndex <= 2)
			{
				result = "V";
			}
			else if (rangeIndex >= 3 && rangeIndex <= 4)
			{
				result = "mV";
			}
			else if (rangeIndex == 5)
			{
				result = "mA";
			}
			break;
		case Adam4000Type.Adam4013:
			if (rangeIndex >= 0 && rangeIndex <= 9)
			{
				result = "'C";
			}
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			if (rangeIndex >= 0 && rangeIndex <= 15)
			{
				result = "'C";
			}
			break;
		case Adam4000Type.Adam4016:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "mV";
			}
			else if (rangeIndex == 4)
			{
				result = "mA";
			}
			break;
		case Adam4000Type.Adam4017P:
			if (rangeIndex >= 1 && rangeIndex <= 3)
			{
				result = "V";
			}
			else if (rangeIndex >= 4 && rangeIndex <= 5)
			{
				result = "mV";
			}
			else if (rangeIndex == 0 || rangeIndex == 6)
			{
				result = "mA";
			}
			break;
		case Adam4000Type.Adam4018P:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mA";
			}
			else if (rangeIndex >= 2 && rangeIndex <= 8)
			{
				result = "'C";
			}
			break;
		case Adam4000Type.Adam4019:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mV";
				break;
			}
			if (rangeIndex >= 2 && rangeIndex <= 5)
			{
				result = "V";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "'C";
				break;
			}
			break;
		case Adam4000Type.Adam4019P:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "mV";
				break;
			}
			if ((rangeIndex >= 2 && rangeIndex <= 3) || (rangeIndex >= 5 && rangeIndex <= 6))
			{
				result = "V";
				break;
			}
			switch (rangeIndex)
			{
			case 4:
			case 7:
				result = "mA";
				break;
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				result = "'C";
				break;
			}
			break;
		case Adam4000Type.Adam4022T:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "mA";
				break;
			case 1:
				result = "V";
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "'C";
				break;
			}
			break;
		case Adam4000Type.Adam4117:
			switch (rangeIndex)
			{
			case 0:
			case 6:
			case 13:
				result = "mA";
				break;
			case 1:
			case 2:
			case 3:
			case 7:
			case 8:
			case 9:
			case 10:
			case 14:
				result = "V";
				break;
			case 4:
			case 5:
			case 11:
			case 12:
				result = "mV";
				break;
			}
			break;
		case Adam4000Type.Adam4118:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "mV";
			}
			else if (rangeIndex >= 4 && rangeIndex <= 5)
			{
				result = "V";
			}
			else if (rangeIndex >= 6 && rangeIndex <= 7)
			{
				result = "mA";
			}
			else if (rangeIndex >= 8 && rangeIndex <= 14)
			{
				result = "'C";
			}
			break;
		}
		return result;
	}

	public static string GetUnitName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			if (rangeIndex >= 0 && rangeIndex <= 9)
			{
				result = "'C";
			}
			break;
		case Adam5000Type.Adam5017:
			if (rangeIndex >= 0 && rangeIndex <= 2)
			{
				result = "V";
			}
			else if (rangeIndex >= 3 && rangeIndex <= 4)
			{
				result = "mV";
			}
			else if (rangeIndex >= 5 && rangeIndex <= 6)
			{
				result = "mA";
			}
			break;
		case Adam5000Type.Adam5017H:
			if (rangeIndex >= 0 && rangeIndex <= 7)
			{
				result = "V";
			}
			else if (rangeIndex >= 8 && rangeIndex <= 9)
			{
				result = "mV";
			}
			else if (rangeIndex >= 10 && rangeIndex <= 11)
			{
				result = "mA";
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (rangeIndex)
			{
			case 0:
			case 3:
				result = "mA";
				break;
			case 1:
			case 4:
				result = "V";
				break;
			case 2:
				result = "mV";
				break;
			}
			break;
		case Adam5000Type.Adam5017P:
			switch (rangeIndex)
			{
			case 0:
			case 6:
			case 13:
				result = "mA";
				break;
			case 1:
			case 2:
			case 3:
			case 7:
			case 8:
			case 9:
			case 10:
			case 14:
				result = "V";
				break;
			case 4:
			case 5:
			case 11:
			case 12:
				result = "mV";
				break;
			}
			break;
		case Adam5000Type.Adam5018:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "mV";
				break;
			}
			if (rangeIndex >= 4 && rangeIndex <= 5)
			{
				result = "V";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "'C";
				break;
			}
			break;
		case Adam5000Type.Adam5018P:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "mV";
			}
			else if (rangeIndex >= 4 && rangeIndex <= 5)
			{
				result = "V";
			}
			else if (rangeIndex >= 6 && rangeIndex <= 7)
			{
				result = "mA";
			}
			else if (rangeIndex >= 8 && rangeIndex <= 14)
			{
				result = "'C";
			}
			break;
		}
		return result;
	}

	public static string GetUnitName(Adam6000Type adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam6000Type, i_byRange);
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			if (rangeIndex >= 0 && rangeIndex <= 13)
			{
				result = "'C";
			}
			break;
		case Adam6000Type.Adam6017:
			if (rangeIndex >= 0 && rangeIndex <= 2)
			{
				result = "V";
			}
			else if (rangeIndex >= 3 && rangeIndex <= 4)
			{
				result = "mV";
			}
			else if (rangeIndex >= 5 && rangeIndex <= 6)
			{
				result = "mA";
			}
			break;
		case Adam6000Type.Adam6018:
			if (rangeIndex >= 0 && rangeIndex <= 6)
			{
				result = "'C";
			}
			break;
		case Adam6000Type.Adam6022:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "mA";
				break;
			case 1:
				result = "V";
				break;
			}
			break;
		case Adam6000Type.Adam6024:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "mA";
				break;
			case 1:
				result = "V";
				break;
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
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4011D:
			switch (rangeIndex)
			{
			case 0:
			case 13:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "1.0 V";
				break;
			case 5:
				result = "2.5 V";
				break;
			case 6:
				result = "20.0 mA";
				break;
			case 7:
			case 8:
				result = "50.0 mV";
				break;
			case 9:
			case 11:
			case 12:
				result = "22.0 mV";
				break;
			case 10:
				result = "80.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4017:
			switch (rangeIndex)
			{
			case 0:
				result = "10.0 V";
				break;
			case 1:
				result = "5.0 V";
				break;
			case 2:
				result = "1.0 V";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "150.0 mV";
				break;
			case 5:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam4000Type.Adam4013:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 4:
			case 5:
				result = "140 Ohms";
				break;
			case 2:
			case 6:
			case 8:
			case 9:
				result = "200 Ohms";
				break;
			case 3:
			case 7:
				result = "440 Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4015:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 5:
			case 6:
			case 16:
				result = "140 Ohms";
				break;
			case 2:
			case 4:
			case 7:
			case 9:
				result = "180 Ohms";
				break;
			case 3:
			case 8:
				result = "250 Ohms";
				break;
			case 11:
				result = "750 Ohms";
				break;
			case 12:
			case 13:
			case 14:
			case 15:
				result = "1000 Ohms";
				break;
			case 10:
				result = "1600 Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4015T:
			switch (rangeIndex)
			{
			case 0:
				result = "10K Ohms";
				break;
			case 1:
				result = "30K Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4016:
			switch (rangeIndex)
			{
			case 0:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam4000Type.Adam4017P:
			switch (rangeIndex)
			{
			case 0:
				result = "20.0 mA";
				break;
			case 1:
				result = "10.0 V";
				break;
			case 2:
				result = "5.0 V";
				break;
			case 3:
				result = "1.0 V";
				break;
			case 4:
				result = "500.0 mV";
				break;
			case 5:
				result = "150.0 mV";
				break;
			case 6:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4018M:
			switch (rangeIndex)
			{
			case 0:
			case 13:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "1.0 V";
				break;
			case 5:
				result = "2.5 V";
				break;
			case 6:
				result = "20.0 mA";
				break;
			case 7:
			case 8:
				result = "50.0 mV";
				break;
			case 9:
			case 11:
				result = "22.0 mV";
				break;
			case 12:
				result = "19.0 mV";
				break;
			case 10:
				result = "80.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4018P:
			switch (rangeIndex)
			{
			case 0:
			case 1:
				result = "20.0 mA";
				break;
			case 2:
				result = "50.0 mV";
				break;
			case 4:
			case 6:
				result = "22.0 mV";
				break;
			case 3:
			case 5:
				result = "78.0 mV";
				break;
			case 7:
				result = "19.0 mV";
				break;
			case 8:
				result = "15.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4019:
			switch (rangeIndex)
			{
			case 0:
				result = "156.25 mV";
				break;
			case 1:
				result = "625.0 mV";
				break;
			case 2:
				result = "1.25 V";
				break;
			case 3:
				result = "2.5 V";
				break;
			case 4:
				result = "10.0 V";
				break;
			case 5:
				result = "5.0 V";
				break;
			case 6:
				result = "20.0 mA";
				break;
			case 7:
			case 8:
				result = "78.125 mV";
				break;
			case 9:
			case 11:
				result = "39.0625 mV";
				break;
			case 10:
				result = "78.125 mV";
				break;
			case 12:
			case 13:
				result = "19.53125 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4019P:
			switch (rangeIndex)
			{
			case 0:
				result = "100.0 mV";
				break;
			case 1:
				result = "500.0 mV";
				break;
			case 2:
				result = "1.0 V";
				break;
			case 3:
				result = "2.5 V";
				break;
			case 4:
			case 7:
				result = "20.0 mA";
				break;
			case 5:
				result = "10.0 V";
				break;
			case 6:
				result = "5.0 V";
				break;
			case 8:
				result = "50.0 mV";
				break;
			case 9:
			case 11:
				result = "78.0 mV";
				break;
			case 10:
			case 12:
				result = "22.0 mV";
				break;
			case 13:
				result = "19.0 mV";
				break;
			case 14:
				result = "15.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4022T:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "20.0 mA";
				break;
			case 1:
				result = "10.0 V";
				break;
			case 3:
			case 4:
			case 7:
			case 8:
				result = "140 Ohms";
				break;
			case 5:
			case 9:
				result = "180 Ohms";
				break;
			case 6:
			case 10:
				result = "400 Ohms";
				break;
			case 11:
				result = "1600 Ohms";
				break;
			case 12:
				result = "10 K Ohms";
				break;
			case 13:
				result = "30 K Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4117:
			switch (rangeIndex)
			{
			case 0:
			case 6:
			case 13:
				result = "20.0 mA";
				break;
			case 1:
			case 8:
				result = "10.0 V";
				break;
			case 2:
			case 9:
				result = "5.0 V";
				break;
			case 3:
			case 10:
				result = "1.0 V";
				break;
			case 4:
			case 11:
				result = "500.0 mV";
				break;
			case 5:
			case 12:
				result = "150.0 mV";
				break;
			case 7:
			case 14:
				result = "15.0 V";
				break;
			}
			break;
		case Adam4000Type.Adam4118:
			switch (rangeIndex)
			{
			case 0:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "1.0 V";
				break;
			case 5:
				result = "2.5 V";
				break;
			case 6:
			case 7:
				result = "20.0 mA";
				break;
			case 8:
				result = "50.0 mV";
				break;
			case 9:
			case 11:
				result = "78.0 mV";
				break;
			case 10:
			case 12:
				result = "22.0 mV";
				break;
			case 13:
				result = "19.0 mV";
				break;
			case 14:
				result = "15.0 mV";
				break;
			}
			break;
		}
		return result;
	}

	public static string GetSpanCalibrationName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 4:
			case 5:
				result = "140 Ohms";
				break;
			case 2:
			case 6:
			case 8:
			case 9:
				result = "200 Ohms";
				break;
			case 3:
			case 7:
				result = "440 Ohms";
				break;
			}
			break;
		case Adam5000Type.Adam5017:
			switch (rangeIndex)
			{
			case 0:
				result = "10.0 V";
				break;
			case 1:
				result = "5.0 V";
				break;
			case 2:
				result = "1.0 V";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "150.0 mV";
				break;
			case 5:
			case 6:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam5000Type.Adam5017H:
			switch (rangeIndex)
			{
			case 0:
			case 1:
				result = "10.0 V";
				break;
			case 2:
			case 3:
				result = "5.0 V";
				break;
			case 4:
			case 5:
				result = "2.5 V";
				break;
			case 6:
			case 7:
				result = "1.0 V";
				break;
			case 8:
			case 9:
				result = "500.0 mV";
				break;
			case 10:
			case 11:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (rangeIndex)
			{
			case 1:
			case 4:
				result = "10.0 V";
				break;
			case 2:
				result = "500.0 mV";
				break;
			case 0:
			case 3:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam5000Type.Adam5017P:
			switch (rangeIndex)
			{
			case 0:
			case 6:
			case 13:
				result = "20.0 mA";
				break;
			case 1:
			case 8:
				result = "10.0 V";
				break;
			case 2:
			case 9:
				result = "5.0 V";
				break;
			case 3:
			case 10:
				result = "1.0 V";
				break;
			case 4:
			case 11:
				result = "500.0 mV";
				break;
			case 5:
			case 12:
				result = "150.0 mV";
				break;
			case 7:
			case 14:
				result = "15.0 V";
				break;
			}
			break;
		case Adam5000Type.Adam5018:
			switch (rangeIndex)
			{
			case 0:
			case 13:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "1.0 V";
				break;
			case 5:
				result = "2.5 V";
				break;
			case 6:
				result = "20.0 mA";
				break;
			case 7:
			case 8:
				result = "50.0 mV";
				break;
			case 9:
			case 11:
			case 12:
				result = "22.0 mV";
				break;
			case 10:
				result = "80.0 mV";
				break;
			}
			break;
		case Adam5000Type.Adam5018P:
			switch (rangeIndex)
			{
			case 0:
			case 14:
				result = "15.0 mV";
				break;
			case 1:
				result = "50.0 mV";
				break;
			case 2:
				result = "100.0 mV";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "1.0 V";
				break;
			case 5:
				result = "2.5 V";
				break;
			case 6:
			case 7:
				result = "20.0 mA";
				break;
			case 8:
			case 9:
				result = "50.0 mV";
				break;
			case 10:
			case 12:
			case 13:
				result = "22.0 mV";
				break;
			case 11:
				result = "80.0 mV";
				break;
			}
			break;
		}
		return result;
	}

	public static string GetSpanCalibrationName(Adam6000Type adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam6000Type, i_byRange);
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 5:
			case 6:
				result = "140 Ohms";
				break;
			case 2:
			case 4:
			case 7:
			case 9:
				result = "180 Ohms";
				break;
			case 3:
			case 8:
				result = "250 Ohms";
				break;
			case 11:
				result = "750 Ohms";
				break;
			case 12:
			case 13:
				result = "1000 Ohms";
				break;
			case 10:
				result = "1600 Ohms";
				break;
			}
			break;
		case Adam6000Type.Adam6017:
			switch (rangeIndex)
			{
			case 0:
				result = "10.0 V";
				break;
			case 1:
				result = "5.0 V";
				break;
			case 2:
				result = "1.0 V";
				break;
			case 3:
				result = "500.0 mV";
				break;
			case 4:
				result = "150.0 mV";
				break;
			case 5:
			case 6:
				result = "20.0 mA";
				break;
			}
			break;
		case Adam6000Type.Adam6018:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 3:
				result = "50.0 mV";
				break;
			case 2:
			case 4:
				result = "22.0 mV";
				break;
			case 5:
				result = "18.0 mV";
				break;
			case 6:
				result = "14.0 mV";
				break;
			}
			break;
		case Adam6000Type.Adam6022:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "20.0 mA";
				break;
			case 1:
				result = "10.0 V";
				break;
			}
			break;
		case Adam6000Type.Adam6024:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "20.0 mA";
				break;
			case 1:
				result = "10.0 V";
				break;
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
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4018:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018M:
			if (rangeIndex >= 0 && rangeIndex <= 5)
			{
				result = "0.0 mV";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "0.0 mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "0.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4017:
			if (rangeIndex >= 0 && rangeIndex <= 4)
			{
				result = "0.0 mV";
			}
			else if (rangeIndex >= 5)
			{
				result = "0.0 mA";
			}
			break;
		case Adam4000Type.Adam4013:
			if (rangeIndex >= 0 && rangeIndex <= 9)
			{
				result = "60 Ohms";
			}
			break;
		case Adam4000Type.Adam4015:
			switch (rangeIndex)
			{
			case 4:
			case 9:
			case 16:
				result = "20 Ohms";
				break;
			case 0:
			case 5:
				result = "80 Ohms";
				break;
			case 1:
			case 2:
			case 3:
			case 6:
			case 7:
			case 8:
				result = "100 Ohms";
				break;
			case 11:
			case 12:
			case 14:
			case 15:
				result = "400 Ohms";
				break;
			case 13:
				result = "600 Ohms";
				break;
			case 10:
				result = "850 Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4015T:
			switch (rangeIndex)
			{
			case 0:
				result = "200 Ohms";
				break;
			case 1:
				result = "800 Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4016:
			if (rangeIndex >= 0 && rangeIndex <= 3)
			{
				result = "0.0 mV";
			}
			else if (rangeIndex == 4)
			{
				result = "0.0 mA";
			}
			break;
		case Adam4000Type.Adam4017P:
			switch (rangeIndex)
			{
			case 0:
			case 6:
				result = "0.0 mA";
				break;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				result = "0.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4018P:
			if (rangeIndex >= 0 && rangeIndex <= 1)
			{
				result = "0.0 mA";
			}
			else if (rangeIndex >= 2 && rangeIndex <= 8)
			{
				result = "0.0 mV";
			}
			break;
		case Adam4000Type.Adam4019:
			if (rangeIndex >= 0 && rangeIndex <= 5)
			{
				result = "0.0 mV";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "0.0 mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "0.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4019P:
			if ((rangeIndex >= 0 && rangeIndex <= 3) || (rangeIndex >= 5 && rangeIndex <= 6))
			{
				result = "0.0 mV";
				break;
			}
			switch (rangeIndex)
			{
			case 4:
			case 7:
				result = "0.0 mA";
				break;
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				result = "0.0 mV";
				break;
			}
			break;
		case Adam4000Type.Adam4022T:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "0.0 mA";
				break;
			case 1:
				result = "0.0 V";
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
				result = "60 Ohms";
				break;
			default:
				switch (rangeIndex)
				{
				case 11:
					result = "850 Ohms";
					break;
				case 12:
					result = "200 Ohms";
					break;
				case 13:
					result = "800 Ohms";
					break;
				}
				break;
			}
			break;
		case Adam4000Type.Adam4117:
			if (rangeIndex == 0 || rangeIndex == 6 || rangeIndex == 13)
			{
				result = "0.0 mA";
			}
			else if (rangeIndex <= 14)
			{
				result = "0.0 mV";
			}
			break;
		case Adam4000Type.Adam4118:
			if (rangeIndex == 6 || rangeIndex == 7)
			{
				result = "0.0 mA";
			}
			else if (rangeIndex <= 14)
			{
				result = "0.0 mV";
			}
			break;
		}
		return result;
	}

	public static string GetZeroCalibrationName(Adam5000Type adam5000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam5000Type, i_byRange);
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			if (rangeIndex >= 0 && rangeIndex <= 9)
			{
				result = "60 Ohms";
			}
			break;
		case Adam5000Type.Adam5017:
			if (rangeIndex >= 0 && rangeIndex <= 4)
			{
				result = "0.0 mV";
			}
			else if (rangeIndex >= 5 && rangeIndex <= 6)
			{
				result = "0.0 mA";
			}
			break;
		case Adam5000Type.Adam5017H:
			if (rangeIndex >= 0 && rangeIndex <= 9)
			{
				result = "0.0 mV";
			}
			else if (rangeIndex >= 10 && rangeIndex <= 11)
			{
				result = "0.0 mA";
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (rangeIndex)
			{
			case 0:
			case 3:
				result = "0.0 mA";
				break;
			case 2:
			case 4:
				result = "0.0 mV";
				break;
			case 1:
				result = "-10.0 V";
				break;
			}
			break;
		case Adam5000Type.Adam5017P:
			if (rangeIndex == 0 || rangeIndex == 6 || rangeIndex == 13)
			{
				result = "0.0 mA";
			}
			else if (rangeIndex <= 14)
			{
				result = "0.0 mV";
			}
			break;
		case Adam5000Type.Adam5018:
			if (rangeIndex >= 0 && rangeIndex <= 5)
			{
				result = "0.0 mV";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
				result = "0.0 mA";
				break;
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				result = "0.0 mV";
				break;
			}
			break;
		case Adam5000Type.Adam5018P:
			if (rangeIndex >= 0 && rangeIndex <= 5)
			{
				result = "0.0 mV";
				break;
			}
			switch (rangeIndex)
			{
			case 6:
			case 7:
				result = "0.0 mA";
				break;
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
				result = "0.0 mV";
				break;
			}
			break;
		}
		return result;
	}

	public static string GetZeroCalibrationName(Adam6000Type adam6000Type, byte i_byRange)
	{
		string result = "";
		int rangeIndex = GetRangeIndex(adam6000Type, i_byRange);
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6015:
			switch (rangeIndex)
			{
			case 4:
			case 9:
				result = "20 Ohms";
				break;
			case 0:
			case 5:
				result = "80 Ohms";
				break;
			case 1:
			case 2:
			case 3:
			case 6:
			case 7:
			case 8:
				result = "100 Ohms";
				break;
			case 11:
			case 12:
				result = "400 Ohms";
				break;
			case 13:
				result = "600 Ohms";
				break;
			case 10:
				result = "850 Ohms";
				break;
			}
			break;
		case Adam6000Type.Adam6017:
			switch (rangeIndex)
			{
			case 0:
				result = "0.0 V";
				break;
			case 1:
				result = "0.0 V";
				break;
			case 2:
				result = "0.0 V";
				break;
			case 3:
				result = "0.0 mV";
				break;
			case 4:
				result = "0.0 mV";
				break;
			case 5:
			case 6:
				result = "0.0 mA";
				break;
			}
			break;
		case Adam6000Type.Adam6018:
			switch (rangeIndex)
			{
			case 0:
			case 1:
			case 2:
			case 3:
				result = "0.0 mV";
				break;
			case 4:
				result = "4.0 mV";
				break;
			case 5:
				result = "4.0 mV";
				break;
			case 6:
				result = "1.0 mV";
				break;
			}
			break;
		case Adam6000Type.Adam6022:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "0.0 mA";
				break;
			case 1:
				result = "0.0 V";
				break;
			}
			break;
		case Adam6000Type.Adam6024:
			switch (rangeIndex)
			{
			case 0:
			case 2:
				result = "0.0 mA";
				break;
			case 1:
				result = "0.0 V";
				break;
			}
			break;
		}
		return result;
	}

	public static int GetIntegrationTotal(AdamType adamType)
	{
		return 2;
	}

	public static int GetIntegrationIndex(AdamType adamType, byte i_byCode)
	{
		switch (adamType)
		{
		case AdamType.Adam4000:
			switch (i_byCode)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			}
			break;
		case AdamType.Adam5000:
			switch (i_byCode)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			}
			break;
		case AdamType.Adam5000Tcp:
			switch (i_byCode)
			{
			case 0:
				return 0;
			case 128:
				return 1;
			}
			break;
		case AdamType.Adam6000:
			switch (i_byCode)
			{
			case 80:
				return 0;
			case 96:
				return 1;
			}
			break;
		}
		return -1;
	}

	public static byte GetIntegrationCode(AdamType adamType, int i_iIndex)
	{
		switch (adamType)
		{
		case AdamType.Adam4000:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			}
			break;
		case AdamType.Adam5000:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			}
			break;
		case AdamType.Adam5000Tcp:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 128;
			}
			break;
		case AdamType.Adam6000:
			switch (i_iIndex)
			{
			case 0:
				return 80;
			case 1:
				return 96;
			}
			break;
		}
		return byte.MaxValue;
	}

	public static string GetIntegrationName(AdamType adamType, byte i_byCode)
	{
		string result = "Unknown";
		switch (adamType)
		{
		case AdamType.Adam4000:
			switch (i_byCode)
			{
			case 0:
				result = "50ms[60Hz]";
				break;
			case 1:
				result = "60ms[50Hz]";
				break;
			}
			break;
		case AdamType.Adam5000:
			switch (i_byCode)
			{
			case 0:
				result = "50ms[60Hz]";
				break;
			case 1:
				result = "60ms[50Hz]";
				break;
			}
			break;
		case AdamType.Adam5000Tcp:
			switch (i_byCode)
			{
			case 0:
				result = "50ms[60Hz]";
				break;
			case 128:
				result = "60ms[50Hz]";
				break;
			}
			break;
		case AdamType.Adam6000:
			switch (i_byCode)
			{
			case 80:
				result = "50ms[60Hz]";
				break;
			case 96:
				result = "60ms[50Hz]";
				break;
			}
			break;
		}
		return result;
	}

	public static string Get4100IntegrationName(byte i_byCode)
	{
		string result = "Unknown";
		switch (i_byCode)
		{
		case 0:
			result = "50Hz/60Hz";
			break;
		case 1:
			result = "User defined";
			break;
		}
		return result;
	}

	public static int GetDataFormatTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4013:
		case Adam4000Type.Adam4016:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018P:
		case Adam4000Type.Adam4019P:
			result = 3;
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			result = 2;
			break;
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
			result = 3;
			break;
		}
		return result;
	}

	public static byte GetDataFormatCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4016:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018P:
		case Adam4000Type.Adam4019P:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 2;
			}
			break;
		case Adam4000Type.Adam4013:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 3;
			}
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 3;
			}
			break;
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 2;
			}
			break;
		}
		return byte.MaxValue;
	}

	public static int GetDataFormatIndex(Adam4000Type adam4000Type, byte i_byDataFormat)
	{
		Adam4000_DataFormat adam4000_DataFormat = (Adam4000_DataFormat)i_byDataFormat;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4016:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018P:
		case Adam4000Type.Adam4019P:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				return 0;
			case Adam4000_DataFormat.Percent:
				return 1;
			case Adam4000_DataFormat.TwosComplementHex:
				return 2;
			}
			break;
		case Adam4000Type.Adam4013:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				return 0;
			case Adam4000_DataFormat.Percent:
				return 1;
			case Adam4000_DataFormat.Ohms:
				return 2;
			}
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				return 0;
			case Adam4000_DataFormat.Ohms:
				return 1;
			}
			break;
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				return 0;
			case Adam4000_DataFormat.Percent:
				return 1;
			case Adam4000_DataFormat.TwosComplementHex:
				return 2;
			}
			break;
		}
		return -1;
	}

	public static string GetDataFormatName(Adam4000Type adam4000Type, byte i_byDataFormat)
	{
		string result = "";
		Adam4000_DataFormat adam4000_DataFormat = (Adam4000_DataFormat)i_byDataFormat;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4011:
		case Adam4000Type.Adam4012:
		case Adam4000Type.Adam4016:
		case Adam4000Type.Adam4011D:
		case Adam4000Type.Adam4018P:
		case Adam4000Type.Adam4019P:
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
			break;
		case Adam4000Type.Adam4013:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam4000_DataFormat.Percent:
				result = "%FSR";
				break;
			case Adam4000_DataFormat.Ohms:
				result = "Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4015:
		case Adam4000Type.Adam4015T:
			switch (adam4000_DataFormat)
			{
			case Adam4000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam4000_DataFormat.Ohms:
				result = "Ohms";
				break;
			}
			break;
		case Adam4000Type.Adam4117:
		case Adam4000Type.Adam4118:
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
			break;
		}
		return result;
	}

	public static int GetDataFormatTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			result = 2;
			break;
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5017P:
			result = 1;
			break;
		case Adam5000Type.Adam5017H:
			result = 2;
			break;
		case Adam5000Type.Adam5017UH:
			result = 2;
			break;
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5018P:
			result = 1;
			break;
		}
		return result;
	}

	public static byte GetDataFormatCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 3;
			}
			break;
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5017P:
			if (i_iIndex == 0)
			{
				return 0;
			}
			break;
		case Adam5000Type.Adam5017H:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 2;
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (i_iIndex)
			{
			case 0:
				return 0;
			case 1:
				return 2;
			}
			break;
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5018P:
			if (i_iIndex == 0)
			{
				return 0;
			}
			break;
		}
		return byte.MaxValue;
	}

	public static int GetDataFormatIndex(Adam5000Type adam5000Type, byte i_byDataFormat)
	{
		Adam5000_DataFormat adam5000_DataFormat = (Adam5000_DataFormat)i_byDataFormat;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				return 0;
			case Adam5000_DataFormat.Ohms:
				return 1;
			}
			break;
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5017P:
			if (adam5000_DataFormat == Adam5000_DataFormat.EngineerUnit)
			{
				return 0;
			}
			break;
		case Adam5000Type.Adam5017H:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				return 0;
			case Adam5000_DataFormat.TwosComplementHex:
				return 1;
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				return 0;
			case Adam5000_DataFormat.TwosComplementHex:
				return 1;
			}
			break;
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5018P:
			if (adam5000_DataFormat == Adam5000_DataFormat.EngineerUnit)
			{
				return 0;
			}
			break;
		}
		return -1;
	}

	public static string GetDataFormatName(Adam5000Type adam5000Type, byte i_byDataFormat)
	{
		string result = "";
		Adam5000_DataFormat adam5000_DataFormat = (Adam5000_DataFormat)i_byDataFormat;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam5000_DataFormat.Ohms:
				result = "Ohms";
				break;
			}
			break;
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5017P:
			if (adam5000_DataFormat == Adam5000_DataFormat.EngineerUnit)
			{
				result = "Engineering Unit";
			}
			break;
		case Adam5000Type.Adam5017H:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam5000_DataFormat.TwosComplementHex:
				result = "Two's complement Hexdicimal";
				break;
			}
			break;
		case Adam5000Type.Adam5017UH:
			switch (adam5000_DataFormat)
			{
			case Adam5000_DataFormat.EngineerUnit:
				result = "Engineering Unit";
				break;
			case Adam5000_DataFormat.TwosComplementHex:
				result = "Two's complement Hexdicimal";
				break;
			}
			break;
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5018P:
			if (adam5000_DataFormat == Adam5000_DataFormat.EngineerUnit)
			{
				result = "Engineering Unit";
			}
			break;
		}
		return result;
	}

	public bool GetChannelDiagnostic(out int[] o_iStatus)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "B\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 2)
			{
				try
				{
					byte b = Convert.ToByte(text, 16);
					o_iStatus = new int[8];
					for (int i = 0; i < 8; i++)
					{
						if ((b & (1 << i)) > 0)
						{
							o_iStatus[i] = 1;
						}
						else
						{
							o_iStatus[i] = 0;
						}
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 4)
			{
				try
				{
					o_iStatus = new int[4];
					for (int i = 0; i < 4; i++)
					{
						string value = text.Substring(i, 1);
						o_iStatus[i] = Convert.ToInt32(value, 16);
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
		o_iStatus = null;
		return false;
	}

	public bool GetBurnoutMap(out Adam4100_BurnoutMap o_burnout)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "AT\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text == "1")
			{
				o_burnout = Adam4100_BurnoutMap.Highest;
				return true;
			}
			if (text == "0")
			{
				o_burnout = Adam4100_BurnoutMap.Lowest;
				return true;
			}
		}
		o_burnout = Adam4100_BurnoutMap.Lowest;
		return false;
	}

	public bool SetBurnoutMap(Adam4100_BurnoutMap i_burnout)
	{
		string text = "#" + base.Address.ToString("X02") + "AT";
		text = ((i_burnout != Adam4100_BurnoutMap.Highest) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetBurnoutDetect(out bool o_burnout)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "D2\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			o_burnout = text == "1";
			return true;
		}
		o_burnout = false;
		return false;
	}

	public bool SetBurnoutDetect(bool i_burnout)
	{
		string text = "$" + base.Address.ToString("X02") + "D";
		text = ((!i_burnout) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetAutoFilter()
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "MM\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetAutoFilterProgress(out int o_iPercent)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "MP\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_iPercent = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iPercent = 0;
		return false;
	}

	public bool GetAutoFilterSampleRate(out int o_iRate)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "MC\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_iRate = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iRate = 0;
		return false;
	}

	public bool GetAutoFilterEnabled(int i_iChannelTotal, out bool[] o_bEnabled, out int o_iPercentIndex)
	{
		if (i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_bEnabled = null;
			o_iPercentIndex = 0;
			return false;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "MD\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				string value = text.Substring(0, 2);
				byte b = Convert.ToByte(value, 16);
				o_bEnabled = new bool[i_iChannelTotal];
				for (int i = 0; i < i_iChannelTotal; i++)
				{
					o_bEnabled[i] = (b & (1 << i)) > 0;
				}
				value = text.Substring(2, 1);
				o_iPercentIndex = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bEnabled = null;
		o_iPercentIndex = 0;
		return false;
	}

	public bool SetAutoFilterEnabled(bool[] i_bEnabled, int i_iPercentIndex)
	{
		byte b = 0;
		int num = ((i_bEnabled.Length <= 8) ? i_bEnabled.Length : 8);
		if (i_iPercentIndex > 9)
		{
			i_iPercentIndex = 9;
		}
		if (i_iPercentIndex < 1)
		{
			i_iPercentIndex = 1;
		}
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				b += Convert.ToByte(1 << i);
			}
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "MK" + b.ToString("X02") + i_iPercentIndex.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetValues(int i_iChannelTotal, out float[] o_fValues, out Adam4000_ChannelStatus[] o_status)
	{
		if (i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_fValues = null;
			o_status = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (i_iChannelTotal == 1)
			{
				try
				{
					o_fValues = new float[1];
					o_status = new Adam4000_ChannelStatus[1];
					if (text == "+9999")
					{
						o_fValues[0] = 0f;
						o_status[0] = Adam4000_ChannelStatus.Over;
					}
					else if (text == "-0000")
					{
						o_fValues[0] = 0f;
						o_status[0] = Adam4000_ChannelStatus.Under;
					}
					else
					{
						o_fValues[0] = Convert.ToSingle(text, m_numberFormatInfo);
						o_status[0] = Adam4000_ChannelStatus.Normal;
					}
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length >= 7 * i_iChannelTotal)
			{
				try
				{
					o_fValues = new float[i_iChannelTotal];
					o_status = new Adam4000_ChannelStatus[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						string text2 = text.Substring(i * 7, 7);
						switch (text2)
						{
						case "       ":
							o_fValues[i] = 0f;
							o_status[i] = Adam4000_ChannelStatus.Disable;
							break;
						case "+999999":
							o_fValues[i] = 0f;
							o_status[i] = Adam4000_ChannelStatus.Over;
							break;
						case "-999999":
						case "-000000":
							o_fValues[i] = 0f;
							o_status[i] = Adam4000_ChannelStatus.Under;
							break;
						case "+888888":
							o_fValues[i] = 0f;
							o_status[i] = Adam4000_ChannelStatus.Burn;
							break;
						default:
							o_fValues[i] = Convert.ToSingle(text2, m_numberFormatInfo);
							o_status[i] = Adam4000_ChannelStatus.Normal;
							break;
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
		o_fValues = null;
		o_status = null;
		return false;
	}

	public bool GetValues(int i_iChannelTotal, out int[] o_iValues)
	{
		if (i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_iValues = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length >= 4 * i_iChannelTotal)
			{
				try
				{
					o_iValues = new int[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						string text2 = text.Substring(i * 4, 4);
						if (text2 == "    ")
						{
							o_iValues[i] = 0;
						}
						else
						{
							o_iValues[i] = Convert.ToInt32(text2, 16);
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
		o_iValues = null;
		return false;
	}

	public bool GetValue(int i_iChannel, out int o_iValue)
	{
		o_iValue = 0;
		if (i_iChannel < 1 || i_iChannel > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 4)
			{
				try
				{
					if (text == "    ")
					{
						o_iValue = 0;
					}
					else
					{
						o_iValue = Convert.ToInt32(text, 16);
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

	public bool GetValue(int i_iChannel, out float o_fValue, out Adam4000_ChannelStatus o_status)
	{
		o_fValue = 0f;
		o_status = Adam4000_ChannelStatus.Disable;
		if (i_iChannel < 0 || i_iChannel >= 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			try
			{
				switch (text)
				{
				case "       ":
					o_fValue = 0f;
					o_status = Adam4000_ChannelStatus.Disable;
					break;
				case "+999999":
					o_fValue = 0f;
					o_status = Adam4000_ChannelStatus.Over;
					break;
				case "-999999":
				case "-000000":
					o_fValue = 0f;
					o_status = Adam4000_ChannelStatus.Under;
					break;
				case "+888888":
					o_fValue = 0f;
					o_status = Adam4000_ChannelStatus.Burn;
					break;
				default:
					o_fValue = Convert.ToSingle(text, m_numberFormatInfo);
					o_status = Adam4000_ChannelStatus.Normal;
					break;
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		return false;
	}

	public bool GetValues(int i_iSlot, int i_iChannelTotal, out float[] o_fValues)
	{
		if (i_iSlot < 0 || i_iSlot >= 8 || i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_fValues = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length >= 7 * i_iChannelTotal)
			{
				try
				{
					o_fValues = new float[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						string text2 = text.Substring(i * 7, 7);
						if (text2 == "       ")
						{
							o_fValues[i] = 0f;
						}
						else
						{
							o_fValues[i] = Convert.ToSingle(text2, m_numberFormatInfo);
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
		o_fValues = null;
		return false;
	}

	public bool GetValues(int i_iSlot, int i_iChannelTotal, out int[] o_iValues)
	{
		if (i_iSlot < 0 || i_iSlot >= 8 || i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_iValues = null;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length >= 4 * i_iChannelTotal)
			{
				try
				{
					o_iValues = new int[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						string text2 = text.Substring(i * 4, 4);
						if (text2 == "    ")
						{
							o_iValues[i] = 0;
						}
						else
						{
							o_iValues[i] = Convert.ToInt32(text2, 16);
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
		o_iValues = null;
		return false;
	}

	public bool GetValue(int i_iSlot, int i_iChannel, out float o_fValue)
	{
		o_fValue = 0f;
		if (i_iSlot < 0 || i_iSlot >= 8 || i_iChannel < 0 || i_iChannel >= 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			return false;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "C" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 7)
			{
				try
				{
					if (text == "       ")
					{
						o_fValue = 0f;
					}
					else
					{
						o_fValue = Convert.ToSingle(text, m_numberFormatInfo);
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

	public bool GetChannelEnabled(int i_iChannelTotal, out bool[] o_bEnabled)
	{
		return GetChannelEnabled(-1, i_iChannelTotal, out o_bEnabled);
	}

	public bool SetChannelEnabled(bool[] i_bEnabled)
	{
		return SetChannelEnabled(-1, i_bEnabled);
	}

	public bool GetChannelEnabled(int i_iSlot, int i_iChannelTotal, out bool[] o_bEnabled)
	{
		if (i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_bEnabled = null;
			return false;
		}
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "6\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 2)
			{
				try
				{
					byte b = Convert.ToByte(text2, 16);
					o_bEnabled = new bool[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						o_bEnabled[i] = (b & (1 << i)) > 0;
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
		o_bEnabled = null;
		return false;
	}

	public bool SetChannelEnabled(int i_iSlot, bool[] i_bEnabled)
	{
		byte b = 0;
		int num = ((i_bEnabled.Length <= 8) ? i_bEnabled.Length : 8);
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				b += Convert.ToByte(1 << i);
			}
		}
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "5" + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetAverageChannelEnabled(int i_iChannelTotal, out bool[] o_bEnabled)
	{
		if (i_iChannelTotal < 1 || i_iChannelTotal > 8)
		{
			base.LastError = ErrorCode.Adam_Invalid_Param;
			o_bEnabled = null;
			return false;
		}
		string text = "$" + base.Address.ToString("X02");
		text += "E\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 2)
			{
				try
				{
					byte b = Convert.ToByte(text2, 16);
					o_bEnabled = new bool[i_iChannelTotal];
					for (int i = 0; i < i_iChannelTotal; i++)
					{
						o_bEnabled[i] = (b & (1 << i)) > 0;
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
		o_bEnabled = null;
		return false;
	}

	public bool SetAverageChannelEnabled(bool[] i_bEnabled)
	{
		byte b = 0;
		int num = ((i_bEnabled.Length <= 8) ? i_bEnabled.Length : 8);
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				b += Convert.ToByte(1 << i);
			}
		}
		string text = "$" + base.Address.ToString("X02");
		text = text + "E" + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetRangeIntegration(int i_iSlot, out byte o_byRange, out byte o_byIntegration)
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
					o_byRange = Convert.ToByte(value, 16);
					o_byIntegration = Convert.ToByte(value2, 16);
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
		o_byIntegration = byte.MaxValue;
		return false;
	}

	public bool SetRangeIntegration(int i_iSlot, byte i_byRange, byte i_byIntegration)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "A" + i_byRange.ToString("X02") + i_byIntegration.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetRangeIntegrationDataFormat(int i_iSlot, out byte o_byRange, out byte o_byIntegration, out byte o_byDataFormat)
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
					o_byRange = Convert.ToByte(value, 16);
					byte b = Convert.ToByte(value2, 16);
					if (Convert.ToByte(b & 0x80) == 128)
					{
						o_byIntegration = 1;
					}
					else
					{
						o_byIntegration = 0;
					}
					o_byDataFormat = Convert.ToByte(b & 3);
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
		o_byIntegration = byte.MaxValue;
		o_byDataFormat = byte.MaxValue;
		return false;
	}

	public bool SetRangeIntegrationDataFormat(int i_iSlot, byte i_byRange, byte i_byIntegration, byte i_byDataFormat)
	{
		byte b = (byte)((i_byIntegration != 0) ? 128 : 0);
		b = Convert.ToByte(b + i_byDataFormat);
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "A" + i_byRange.ToString("X02") + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetDataFormat(int i_iSlot, out byte o_byDataFormat)
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
				string value = text2.Substring(2, 2);
				try
				{
					o_byDataFormat = Convert.ToByte(value, 16);
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
		o_byDataFormat = byte.MaxValue;
		return false;
	}

	public bool SetDataFormat(int i_iSlot, byte i_byDataFormat)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "AFF" + i_byDataFormat.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetIntegration(out byte o_byIntegration)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "B\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 2)
			{
				try
				{
					o_byIntegration = Convert.ToByte(text, 16);
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
		o_byIntegration = byte.MaxValue;
		return false;
	}

	public bool SetIntegration(byte i_byIntegration)
	{
		string text = "$" + base.Address.ToString("X02");
		text = text + "A" + i_byIntegration.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetInputRange(int i_iSlot, int i_iChannel, out byte o_byRange)
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
					o_byRange = Convert.ToByte(value, 16);
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

	public bool SetInputRange(int i_iSlot, int i_iChannel, byte i_byRange)
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
		text = text + "A" + i_byRange.ToString("X02") + "FF\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetInputRange(int i_iChannel, out byte o_byRange)
	{
		string o_szRecv;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			string text = "$" + base.Address.ToString("X02");
			text = text + "B" + i_iChannel.ToString("X02") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 4)
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
		}
		else
		{
			string text = "$" + base.Address.ToString("X02");
			text = text + "8C" + i_iChannel.ToString("X") + "\r";
			if (ASCIISendRecv(text, out o_szRecv) && o_szRecv.Length > 4)
			{
				string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text2.Length == 5)
				{
					string value = text2.Substring(3, 2);
					try
					{
						o_byRange = Convert.ToByte(value, 16);
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
		o_byRange = byte.MaxValue;
		return false;
	}

	public bool SetInputRange(int i_iChannel, byte i_byRange)
	{
		string o_szRecv;
		string text;
		if (base.AdamSeriesType == AdamType.Adam6000)
		{
			text = "$" + base.Address.ToString("X02");
			text = text + "A" + i_iChannel.ToString("X02") + i_byRange.ToString("X02") + "\r";
			return ASCIISendRecv(text, out o_szRecv);
		}
		text = "$" + base.Address.ToString("X02");
		text = text + "7C" + i_iChannel.ToString("X") + "R" + i_byRange.ToString("X02") + "\r";
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetCalibrationValues(int i_iSlot, out string o_szZero, out string o_szSpan)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "AV" + i_iSlot.ToString("X02") + "FF\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 18)
			{
				o_szZero = text.Substring(1, 8);
				o_szSpan = text.Substring(10, 8);
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_szZero = "";
		o_szSpan = "";
		return false;
	}

	public bool SetSpanCalibration(int i_iSlot)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "0\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetSpanCalibration()
	{
		return SetSpanCalibration(-1);
	}

	public bool SetChannelSpanCalibration(int i_iChannel)
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

	public bool SetZeroCalibration(int i_iSlot)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "1\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetZeroCalibration()
	{
		return SetZeroCalibration(-1);
	}

	public bool SetChannelZeroCalibration(int i_iChannel)
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

	public bool SetChannelNegCalibration(int i_iChannel)
	{
		string text = "$" + base.Address.ToString("X02") + "N";
		if (i_iChannel >= 0)
		{
			text = text + "C" + i_iChannel.ToString("X");
		}
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetCJCValue(int i_iSlot, out float o_fCJCVal)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "3\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string value = o_szRecv.Substring(1, o_szRecv.Length - 2);
			try
			{
				o_fCJCVal = Convert.ToSingle(value, m_numberFormatInfo);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fCJCVal = 0f;
		return false;
	}

	public bool GetCJCValue(out float o_fCJCVal)
	{
		return GetCJCValue(-1, out o_fCJCVal);
	}

	public bool SetCJCOffset(int i_iSlot, float i_fCJCOffset)
	{
		bool flag;
		float num;
		if ((double)i_fCJCOffset > 99.9)
		{
			flag = true;
			num = 99.9f;
		}
		else if ((double)i_fCJCOffset < -99.9)
		{
			flag = false;
			num = 99.9f;
		}
		else if (i_fCJCOffset < 0f)
		{
			flag = false;
			num = i_fCJCOffset * -1f;
		}
		else
		{
			flag = true;
			num = i_fCJCOffset;
		}
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			int num2 = Convert.ToInt32(num / 0.009f);
			text = text + "S" + i_iSlot.ToString("X");
			text = ((!flag) ? (text + "9-") : (text + "9+"));
			text = text + num2.ToString("X04") + "\r";
		}
		else if (i_iSlot == -1)
		{
			int num2 = Convert.ToInt32(num / 0.009f);
			text = ((!flag) ? (text + "9-") : (text + "9+"));
			text = text + num2.ToString("X04") + "\r";
		}
		else
		{
			int num2 = Convert.ToInt32(num * 80f);
			text = ((!flag) ? (text + "9-") : (text + "9+"));
			text = text + num2.ToString("X04") + "\r";
		}
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetCJCOffset(float i_fCJCOffset)
	{
		return SetCJCOffset(-1, i_fCJCOffset);
	}

	public bool SetCJCValue(float i_fCJCOffset)
	{
		return SetCJCOffset(-2, i_fCJCOffset);
	}

	public bool GetLeadWire(int i_iType, out float o_fVal)
	{
		string text = "$" + base.Address.ToString("X02");
		text = ((i_iType != 0) ? (text + "R1\r") : (text + "R0\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 5)
		{
			string text2 = o_szRecv.Substring(3, 1);
			bool flag = text2 == "+";
			string value = o_szRecv.Substring(4, o_szRecv.Length - 5);
			try
			{
				int value2 = Convert.ToInt32(value, 16);
				o_fVal = Convert.ToSingle((double)Convert.ToSingle(value2) / 100.0);
				if (!flag)
				{
					o_fVal *= -1f;
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fVal = 0f;
		return false;
	}

	public bool SetLeadWire(int i_iType, float i_fVal)
	{
		int num = Convert.ToInt32(i_fVal * 100f);
		bool flag;
		if (num > 65535)
		{
			flag = true;
			num = 65535;
		}
		else if (num < -65535)
		{
			flag = false;
			num = 65535;
		}
		else if (num < 0)
		{
			flag = false;
			num *= -1;
		}
		else
		{
			flag = true;
		}
		string text = "$" + base.Address.ToString("X02");
		text = ((i_iType != 0) ? (text + "R1") : (text + "R0"));
		text = ((!flag) ? (text + "-") : (text + "+"));
		text = text + num.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetMemConfig(out bool[] o_bEnabled, out bool o_bRecord, out byte o_byRecordMode, out byte o_MemMode, out int o_iInterval)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "D\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 9)
			{
				try
				{
					string value = text.Substring(0, 2);
					byte b = Convert.ToByte(value, 16);
					o_bEnabled = new bool[8];
					for (int i = 0; i < 8; i++)
					{
						o_bEnabled[i] = (b & (1 << i)) > 0;
					}
					value = text.Substring(2, 1);
					o_bRecord = value == "1";
					value = text.Substring(3, 1);
					if (value == "2")
					{
						o_byRecordMode = 2;
					}
					else if (value == "1")
					{
						o_byRecordMode = 1;
					}
					else
					{
						o_byRecordMode = 0;
					}
					value = text.Substring(4, 1);
					if (value == "1")
					{
						o_MemMode = 1;
					}
					else
					{
						o_MemMode = 0;
					}
					value = text.Substring(5, 4);
					o_iInterval = Convert.ToInt32(value, 16);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
		}
		o_bEnabled = null;
		o_bRecord = false;
		o_byRecordMode = 0;
		o_MemMode = 0;
		o_iInterval = 2;
		return false;
	}

	public bool SetMemConfig(bool[] i_bEnabled, bool i_bRecord, byte i_byRecordMode, byte i_MemMode, int i_iInterval)
	{
		byte b = 0;
		int num = ((i_bEnabled.Length <= 8) ? i_bEnabled.Length : 8);
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				b += Convert.ToByte(1 << i);
			}
		}
		string text = "@" + base.Address.ToString("X02") + "C" + b.ToString("X02");
		text = ((!i_bRecord) ? (text + "0") : (text + "1"));
		text += i_byRecordMode;
		text += i_MemMode;
		if (i_iInterval > 65535)
		{
			i_iInterval = 65535;
		}
		if (i_iInterval < 2)
		{
			i_iInterval = 2;
		}
		text = text + i_iInterval.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetMemOperation(out bool o_bRecording)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "T\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text == "1")
			{
				o_bRecording = true;
			}
			else
			{
				o_bRecording = false;
			}
			return true;
		}
		o_bRecording = false;
		return false;
	}

	public bool SetMemOperation(bool i_bRecording)
	{
		string text = "@" + base.Address.ToString("X02") + "S";
		text = ((!i_bRecording) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetMemStandardRecordCount(out int o_iCount)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "N\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 4)
			{
				try
				{
					o_iCount = Convert.ToInt32(text, 16);
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
		o_iCount = 0;
		return false;
	}

	public bool GetMemEventRecordCount(out int o_iCount)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "L\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 4)
			{
				try
				{
					o_iCount = Convert.ToInt32(text, 16);
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
		o_iCount = 0;
		return false;
	}

	public bool GetMemRecordData(int i_iIndex, out int o_iChannel, out float o_fData, out long o_lElapse)
	{
		if (i_iIndex > 9999)
		{
			i_iIndex = 9999;
		}
		if (i_iIndex < 0)
		{
			i_iIndex = 0;
		}
		string i_szSend = "@" + base.Address.ToString("X02") + "R" + i_iIndex.ToString("0000") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 6)
			{
				try
				{
					string value = text.Substring(0, 1);
					o_iChannel = Convert.ToInt32(value);
					value = text.Substring(1, 1);
					byte b = Convert.ToByte(value, 16);
					byte b2 = Convert.ToByte(b >> 1);
					value = text.Substring(2, 4);
					o_fData = Convert.ToSingle(Convert.ToInt32(value, 16));
					switch (b2)
					{
					case 1:
						o_fData /= 10f;
						break;
					case 2:
						o_fData /= 100f;
						break;
					case 3:
						o_fData /= 1000f;
						break;
					case 4:
						o_fData /= 10000f;
						break;
					case 5:
						o_fData /= 100000f;
						break;
					case 6:
						o_fData /= 1000000f;
						break;
					case 7:
						o_fData /= 10000000f;
						break;
					}
					if ((b & 1) == 1)
					{
						o_fData *= -1f;
					}
					o_lElapse = 0L;
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text.Length == 14)
			{
				try
				{
					string value = text.Substring(0, 1);
					o_iChannel = Convert.ToInt32(value);
					value = text.Substring(1, 1);
					byte b = Convert.ToByte(value, 16);
					byte b2 = Convert.ToByte(b >> 1);
					value = text.Substring(2, 4);
					o_fData = Convert.ToSingle(Convert.ToInt32(value, 16));
					switch (b2)
					{
					case 1:
						o_fData /= 10f;
						break;
					case 2:
						o_fData /= 100f;
						break;
					case 3:
						o_fData /= 1000f;
						break;
					case 4:
						o_fData /= 10000f;
						break;
					case 5:
						o_fData /= 100000f;
						break;
					case 6:
						o_fData /= 1000000f;
						break;
					case 7:
						o_fData /= 10000000f;
						break;
					}
					if ((b & 1) == 1)
					{
						o_fData *= -1f;
					}
					value = text.Substring(6, 8);
					o_lElapse = Convert.ToInt64(value, 16);
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
		o_iChannel = 0;
		o_fData = 0f;
		o_lElapse = 0L;
		return false;
	}

	public bool GetUnknownModuleName(int i_iSlot, out string o_szName)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "N\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			o_szName = o_szRecv.Substring(1, o_szRecv.Length - 2);
			return true;
		}
		o_szName = "";
		return false;
	}

	public bool GetUnknownModuleDescription(int i_iSlot, out string o_szDesc)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "N0\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			o_szDesc = o_szRecv.Substring(1, o_szRecv.Length - 2);
			return true;
		}
		o_szDesc = "";
		return false;
	}

	public bool GetUnknownRangeSupport(int i_iSlot, out byte[] o_byRanges)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "N1\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length > 4 && text.Length % 2 == 0)
			{
				try
				{
					string value = text.Substring(0, 2);
					Convert.ToInt32(value, 16);
					value = text.Substring(2, 2);
					int num = Convert.ToInt32(value, 16);
					if (num * 2 == text.Length - 4)
					{
						o_byRanges = new byte[num];
						for (int i = 0; i < num; i++)
						{
							value = text.Substring(4 + i * 2, 2);
							o_byRanges[i] = Convert.ToByte(value, 16);
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
			else
			{
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
		}
		o_byRanges = null;
		return false;
	}

	public bool GetUnknownModuleInfo(int i_iSlot, out int o_iChTotal, out byte[] o_byRanges, out byte o_byMask, out byte o_byConfig, out string o_szModuleID)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot.ToString("X") + "N2\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length > 2 && text.Length % 2 == 0)
			{
				try
				{
					string value = text.Substring(0, 2);
					o_iChTotal = Convert.ToInt32(value, 16);
					if (o_iChTotal * 2 == text.Length - 8)
					{
						o_byRanges = new byte[o_iChTotal];
						for (int i = 0; i < o_iChTotal; i++)
						{
							value = text.Substring(2 + i * 2, 2);
							o_byRanges[o_iChTotal - i - 1] = Convert.ToByte(value, 16);
						}
						value = text.Substring(2 + o_iChTotal * 2, 2);
						o_byMask = Convert.ToByte(value, 16);
						value = text.Substring(4 + o_iChTotal * 2, 2);
						o_byConfig = Convert.ToByte(value, 16);
						o_szModuleID = text.Substring(6 + o_iChTotal * 2, 2);
						return true;
					}
					base.LastError = ErrorCode.Adam_Invalid_Length;
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
		o_iChTotal = 0;
		o_byRanges = null;
		o_byMask = 0;
		o_byConfig = 0;
		o_szModuleID = "";
		return false;
	}

	public bool GetBiasValue(int i_iChannel, out float o_fBiasValue)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "AYFF" + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 5)
		{
			string text = o_szRecv.Substring(3, 1);
			bool flag = text == "+";
			string value = o_szRecv.Substring(4, o_szRecv.Length - 5);
			try
			{
				o_fBiasValue = Convert.ToSingle(Convert.ToInt16(value, 16)) / 10f;
				if (!flag)
				{
					o_fBiasValue *= -1f;
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fBiasValue = 0f;
		return false;
	}

	public bool SetBiasValue(int i_iChannel, float i_fBiasValue)
	{
		string text = "#" + base.Address.ToString("X02") + "AYFF" + i_iChannel.ToString("X02");
		i_fBiasValue *= 10f;
		if (i_fBiasValue >= 0f)
		{
			text += "+";
		}
		else
		{
			text += "-";
			i_fBiasValue *= -1f;
		}
		text = text + Convert.ToInt16(i_fBiasValue).ToString("X02") + "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv == ">" + base.Address.ToString("X02") + "\r")
		{
			return true;
		}
		return false;
	}
}
