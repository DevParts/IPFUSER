using System;
using Advantech.Common;

namespace Advantech.Adam;

public class DigitalInput : AdamBase
{
	private static byte MODE_MASK = 15;

	private static byte RECORD_LAST_COUNT = 32;

	private static byte ENABLE_DIGITAL_FILTER = 64;

	private static byte ENABLE_INVERT_MODE = 128;

	public DigitalInput(AdamCom i_com)
		: base(i_com)
	{
	}

	public DigitalInput(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	public static int GetChannelTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4022T:
			result = 2;
			break;
		case Adam4000Type.Adam4050:
			result = 7;
			break;
		case Adam4000Type.Adam4051:
			result = 16;
			break;
		case Adam4000Type.Adam4052:
			result = 8;
			break;
		case Adam4000Type.Adam4053:
			result = 16;
			break;
		case Adam4000Type.Adam4055:
			result = 8;
			break;
		case Adam4000Type.Adam4150:
			result = 7;
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5050:
			result = 16;
			break;
		case Adam5000Type.Adam5051:
			result = 16;
			break;
		case Adam5000Type.Adam5052:
			result = 8;
			break;
		case Adam5000Type.Adam5055:
			result = 8;
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6022:
			result = 2;
			break;
		case Adam6000Type.Adam6024:
			result = 2;
			break;
		case Adam6000Type.Adam6050:
		case Adam6000Type.Adam6050W:
			result = 12;
			break;
		case Adam6000Type.Adam6051:
		case Adam6000Type.Adam6051W:
			result = 12;
			break;
		case Adam6000Type.Adam6052:
			result = 8;
			break;
		case Adam6000Type.Adam6055:
			result = 18;
			break;
		case Adam6000Type.Adam6060:
		case Adam6000Type.Adam6060W:
			result = 6;
			break;
		case Adam6000Type.Adam6066:
			result = 6;
			break;
		}
		return result;
	}

	public static int GetChannelStart(Adam6000Type adam6000Type)
	{
		int result = 0;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6050:
		case Adam6000Type.Adam6050W:
			result = 0;
			break;
		case Adam6000Type.Adam6051:
		case Adam6000Type.Adam6051W:
			result = 0;
			break;
		case Adam6000Type.Adam6052:
			result = 0;
			break;
		case Adam6000Type.Adam6055:
			result = 0;
			break;
		case Adam6000Type.Adam6060:
		case Adam6000Type.Adam6060W:
			result = 0;
			break;
		case Adam6000Type.Adam6066:
			result = 0;
			break;
		}
		return result;
	}

	public static void ParseIOConfig(byte i_byConfig, out byte o_byMode, out bool o_bRecordLastCount, out bool o_bDigitalFilter, out bool o_bInvert)
	{
		if ((i_byConfig & MODE_MASK) == 0)
		{
			o_byMode = 0;
		}
		else if ((i_byConfig & MODE_MASK) == 1)
		{
			o_byMode = 1;
		}
		else if ((i_byConfig & MODE_MASK) == 2)
		{
			o_byMode = 2;
		}
		else if ((i_byConfig & MODE_MASK) == 3)
		{
			o_byMode = 3;
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

	public static double GetCounterScaledValue(int i_iHigh, int i_iLow)
	{
		double num = 0.0;
		long num2 = i_iHigh;
		num2 = num2 * 65536 + i_iLow;
		return Convert.ToDouble(num2);
	}

	public static double GetFrequencyScaledValue(int i_iHigh, int i_iLow)
	{
		double num = 0.0;
		long num2 = i_iHigh;
		num2 = num2 * 65536 + i_iLow;
		return Convert.ToDouble(num2) / 10.0;
	}

	public bool GetValue(out bool o_bDI)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "DI\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 5)
			{
				try
				{
					string value = text.Substring(3, 2);
					byte b = Convert.ToByte(value, 16);
					o_bDI = b == 1;
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
		o_bDI = false;
		return false;
	}

	public bool GetValues(out bool[] o_bDI, out bool[] o_bDO)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "7\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 4)
				{
					o_bDI = new bool[2];
					o_bDO = new bool[2];
					string value = text.Substring(2, 2);
					byte b = Convert.ToByte(value, 16);
					for (int i = 0; i < 2; i++)
					{
						o_bDI[i] = (b & (1 << i)) > 0;
					}
					value = text.Substring(0, 2);
					b = Convert.ToByte(value, 16);
					for (int i = 0; i < 2; i++)
					{
						o_bDO[i] = (b & (1 << i)) > 0;
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
		o_bDO = null;
		return false;
	}

	public bool GetValues(out bool[] o_bDI)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "I\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 1)
				{
					o_bDI = new bool[4];
					byte b = Convert.ToByte(text, 16);
					for (int i = 0; i < 4; i++)
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

	public bool GetValues(int i_iDITotal, int i_iDOTotal, out bool[] o_bDI, out bool[] o_bDO)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "6\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			try
			{
				string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
				if (text.Length == 6)
				{
					string value;
					byte b;
					if (i_iDITotal > 0)
					{
						if (i_iDOTotal > 0)
						{
							o_bDI = new bool[i_iDITotal];
							o_bDO = new bool[i_iDOTotal];
							value = text.Substring(2, 2);
							b = Convert.ToByte(value, 16);
							for (int i = 0; i < i_iDITotal; i++)
							{
								o_bDI[i] = (b & (1 << i)) > 0;
							}
							value = text.Substring(0, 2);
							b = Convert.ToByte(value, 16);
							for (int i = 0; i < i_iDOTotal; i++)
							{
								o_bDO[i] = (b & (1 << i)) > 0;
							}
							return true;
						}
						o_bDI = new bool[i_iDITotal];
						o_bDO = null;
						if (i_iDITotal > 8)
						{
							value = text.Substring(2, 2);
							b = Convert.ToByte(value, 16);
							for (int i = 0; i < 8; i++)
							{
								o_bDI[i] = (b & (1 << i)) > 0;
							}
							value = text.Substring(0, 2);
							b = Convert.ToByte(value, 16);
							for (int i = 0; i < i_iDITotal - 8; i++)
							{
								o_bDI[i + 8] = (b & (1 << i)) > 0;
							}
							return true;
						}
						value = text.Substring(0, 2);
						b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iDITotal; i++)
						{
							o_bDI[i] = (b & (1 << i)) > 0;
						}
						return true;
					}
					o_bDI = null;
					o_bDO = new bool[i_iDOTotal];
					if (i_iDOTotal > 8)
					{
						value = text.Substring(2, 2);
						b = Convert.ToByte(value, 16);
						for (int i = 0; i < 8; i++)
						{
							o_bDO[i] = (b & (1 << i)) > 0;
						}
						value = text.Substring(0, 2);
						b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iDOTotal - 8; i++)
						{
							o_bDO[i + 8] = (b & (1 << i)) > 0;
						}
						return true;
					}
					value = text.Substring(0, 2);
					b = Convert.ToByte(value, 16);
					for (int i = 0; i < i_iDOTotal; i++)
					{
						o_bDO[i] = (b & (1 << i)) > 0;
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
		o_bDO = null;
		return false;
	}

	public bool GetInvertMask(out bool[] o_bEnabled)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "V\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 4)
			{
				try
				{
					o_bEnabled = new bool[16];
					int num = Convert.ToInt32(text, 16);
					for (int i = 0; i < 16; i++)
					{
						o_bEnabled[i] = (num & (1 << i)) > 0;
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

	public bool SetInvertMask(bool[] i_bEnabled)
	{
		int num = ((i_bEnabled.Length <= 16) ? i_bEnabled.Length : 16);
		string text = "$" + base.Address.ToString("X02") + "V";
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				num2 += 1 << i;
			}
		}
		text = text + num2.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetValues(int i_iSlot, int i_iChannelTotal, out bool[] o_bValues)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot + "6\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 6)
				{
					if (i_iChannelTotal <= 8)
					{
						o_bValues = new bool[i_iChannelTotal];
						string value = text.Substring(2, 2);
						byte b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iChannelTotal; i++)
						{
							o_bValues[i] = (b & (1 << i)) > 0;
						}
						return true;
					}
					if (i_iChannelTotal <= 16)
					{
						o_bValues = new bool[i_iChannelTotal];
						string value = text.Substring(2, 2);
						byte b = Convert.ToByte(value, 16);
						for (int i = 0; i < 8; i++)
						{
							o_bValues[i] = (b & (1 << i)) > 0;
						}
						value = text.Substring(0, 2);
						b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iChannelTotal - 8; i++)
						{
							o_bValues[i + 8] = (b & (1 << i)) > 0;
						}
						return true;
					}
					base.LastError = ErrorCode.Adam_Invalid_Param;
				}
				else
				{
					base.LastError = ErrorCode.Adam_Invalid_Length;
				}
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bValues = null;
		return false;
	}

	public bool GetUniversalStatus(int i_iSlot, out bool[] o_bIsDI)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "7\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				try
				{
					o_bIsDI = new bool[16];
					string value = text2.Substring(2, 2);
					byte b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bIsDI[i] = (b & (1 << i)) > 0;
					}
					value = text2.Substring(0, 2);
					b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bIsDI[i + 8] = (b & (1 << i)) > 0;
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
		o_bIsDI = null;
		return false;
	}

	public bool GetInvertMask(int i_iSlot, out bool[] o_bEnabled)
	{
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "C\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 32)
			{
				try
				{
					o_bEnabled = new bool[16];
					for (int i = 0; i < 16; i++)
					{
						string text3 = text2.Substring(i * 2, 2);
						o_bEnabled[i] = text3 == "80";
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

	public bool SetInvertMask(int i_iSlot, bool[] i_bEnabled)
	{
		int num = ((i_bEnabled.Length <= 16) ? i_bEnabled.Length : 16);
		string text = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "C";
		for (int i = 0; i < num; i++)
		{
			text = ((!i_bEnabled[i]) ? (text + "00") : (text + "80"));
		}
		text += "\r";
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
					o_lLow[i] = Convert.ToUInt32(value, 16);
					value = text.Substring(i * 16 + 8, 8);
					o_lHigh[i] = Convert.ToUInt32(value, 16);
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
		base.LastError = ErrorCode.Adam_Invalid_Param;
		return false;
	}
}
