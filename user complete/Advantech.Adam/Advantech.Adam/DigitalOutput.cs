using System;
using Advantech.Common;

namespace Advantech.Adam;

public class DigitalOutput : AdamBase
{
	private static byte MODE_MASK = 15;

	public DigitalOutput(AdamCom i_com)
		: base(i_com)
	{
	}

	public DigitalOutput(AdamSocket i_socket)
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
			result = 8;
			break;
		case Adam4000Type.Adam4055:
			result = 8;
			break;
		case Adam4000Type.Adam4056S:
		case Adam4000Type.Adam4056SO:
			result = 12;
			break;
		case Adam4000Type.Adam4060:
			result = 4;
			break;
		case Adam4000Type.Adam4068:
			result = 8;
			break;
		case Adam4000Type.Adam4069:
			result = 8;
			break;
		case Adam4000Type.Adam4150:
		case Adam4000Type.Adam4168:
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
		case Adam5000Type.Adam5055:
			result = 8;
			break;
		case Adam5000Type.Adam5056:
			result = 16;
			break;
		case Adam5000Type.Adam5060:
			result = 6;
			break;
		case Adam5000Type.Adam5068:
			result = 8;
			break;
		case Adam5000Type.Adam5069:
			result = 8;
			break;
		case Adam5000Type.Adam5081:
			result = 4;
			break;
		}
		return result;
	}

	public static int GetChannelTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		switch (adam6000Type)
		{
		case Adam6000Type.Adam6017:
			result = 2;
			break;
		case Adam6000Type.Adam6018:
			result = 8;
			break;
		case Adam6000Type.Adam6022:
			result = 2;
			break;
		case Adam6000Type.Adam6024:
			result = 2;
			break;
		case Adam6000Type.Adam6050:
		case Adam6000Type.Adam6050W:
			result = 6;
			break;
		case Adam6000Type.Adam6051:
		case Adam6000Type.Adam6051W:
			result = 2;
			break;
		case Adam6000Type.Adam6052:
			result = 8;
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

	public static int GetChannelStart(Adam4000Type adam4000Type)
	{
		int result = 0;
		switch (adam4000Type)
		{
		case Adam4000Type.Adam4150:
			result = 7;
			break;
		case Adam4000Type.Adam4168:
			result = 0;
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
			result = 12;
			break;
		case Adam6000Type.Adam6051:
		case Adam6000Type.Adam6051W:
			result = 14;
			break;
		case Adam6000Type.Adam6052:
			result = 8;
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

	public static void ParseIOConfig(byte i_byConfig, out byte o_byMode)
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
		else
		{
			o_byMode = 16;
		}
	}

	public static void FormIOConfig(byte i_byMode, out byte o_byConfig)
	{
		if (i_byMode != 16)
		{
			o_byConfig = i_byMode;
		}
		else
		{
			o_byConfig = 0;
		}
	}

	public bool GetValues(out bool[] o_bDI, out bool[] o_bDO)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "7\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 42);
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

	public bool SetValue(int i_iChannel, bool i_bValue)
	{
		string text = "#" + base.Address.ToString("X02");
		text = text + "1" + i_iChannel.ToString("X");
		text = ((!i_bValue) ? (text + "00\r") : (text + "01\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length >= 2)
		{
			return true;
		}
		return false;
	}

	public bool SetSValue(int i_iChannel, bool i_bValue)
	{
		string text = "#" + base.Address.ToString("X02");
		text = text + "1" + i_iChannel.ToString("X");
		text = ((!i_bValue) ? (text + "0000\r") : (text + "0001\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length >= 2)
		{
			return true;
		}
		return false;
	}

	public bool SetValues(bool[] i_bValues)
	{
		string text = "#" + base.Address.ToString("X02");
		text += "00";
		if (i_bValues.Length <= 8)
		{
			byte b = 0;
			for (int i = 0; i < i_bValues.Length; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
		}
		else if (i_bValues.Length <= 16)
		{
			byte b = 0;
			for (int i = 8; i < i_bValues.Length; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i - 8));
				}
			}
			text += b.ToString("X02");
			b = 0;
			for (int i = 0; i < 8; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
		}
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length >= 2)
		{
			return true;
		}
		return false;
	}

	public bool GetWDTFlag(out bool o_bEnabled)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "X2\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			o_bEnabled = text == "01";
			return true;
		}
		o_bEnabled = false;
		return false;
	}

	public bool GetWDTInfo(out float o_fTimeout, out bool[] o_bEnabled)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "X1\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text.Length == 6)
			{
				try
				{
					o_bEnabled = new bool[16];
					string value = text.Substring(0, 4);
					int value2 = Convert.ToInt32(value, 16);
					o_fTimeout = Convert.ToSingle(value2);
					o_fTimeout /= 10f;
					value = text.Substring(4, 2);
					byte b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
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
			else if (text.Length == 8)
			{
				try
				{
					o_bEnabled = new bool[16];
					string value = text.Substring(0, 4);
					int value2 = Convert.ToInt32(value, 16);
					o_fTimeout = Convert.ToSingle(value2);
					o_fTimeout /= 10f;
					value = text.Substring(6, 2);
					byte b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bEnabled[i] = (b & (1 << i)) > 0;
					}
					value = text.Substring(4, 2);
					b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bEnabled[i + 8] = (b & (1 << i)) > 0;
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
		o_fTimeout = 0f;
		o_bEnabled = null;
		return false;
	}

	public bool SetWDTInfo(float i_fTimeout, bool[] i_bEnabled)
	{
		int num = Convert.ToInt32(i_fTimeout * 10f);
		if (num > 65535)
		{
			num = 65535;
		}
		if (num < 0)
		{
			num = 0;
		}
		int num2 = ((i_bEnabled.Length <= 16) ? i_bEnabled.Length : 16);
		ushort num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			if (i_bEnabled[i])
			{
				num3 += Convert.ToUInt16(1 << i);
			}
		}
		string text = "$" + base.Address.ToString("X02") + "X0" + num.ToString("X04");
		text = ((i_bEnabled.Length > 8) ? (text + num3.ToString("X04") + "\r") : (text + num3.ToString("X02") + "\r"));
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
						string value = text.Substring(0, 2);
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

	public bool SetValue(int i_iSlot, int i_iChannel, bool i_bValue)
	{
		string text = "#" + base.Address.ToString("X02") + "S" + i_iSlot;
		text = text + "1" + i_iChannel.ToString("X");
		text = ((!i_bValue) ? (text + "00\r") : (text + "01\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length >= 2)
		{
			return true;
		}
		return false;
	}

	public bool SetValues(int i_iSlot, bool[] i_bValues)
	{
		string text = "#" + base.Address.ToString("X02") + "S" + i_iSlot;
		text += "00";
		if (i_bValues.Length <= 8)
		{
			byte b = 0;
			for (int i = 0; i < i_bValues.Length; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
		}
		else if (i_bValues.Length <= 16)
		{
			byte b = 0;
			for (int i = 8; i < i_bValues.Length; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i - 8));
				}
			}
			text += b.ToString("X02");
			b = 0;
			for (int i = 0; i < 8; i++)
			{
				if (i_bValues[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
		}
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length >= 2)
		{
			return true;
		}
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

	public bool GetAlarmMappingMask(int i_iSlot, int i_iChannelTotal, out bool[] o_bEnabled)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "S" + i_iSlot + "M\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 2)
				{
					if (i_iChannelTotal <= 8)
					{
						o_bEnabled = new bool[i_iChannelTotal];
						string value = text.Substring(0, 2);
						byte b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iChannelTotal; i++)
						{
							o_bEnabled[i] = (b & (1 << i)) > 0;
						}
						return true;
					}
					base.LastError = ErrorCode.Adam_Invalid_Param;
				}
				else if (text.Length == 4)
				{
					if (i_iChannelTotal <= 8)
					{
						o_bEnabled = new bool[i_iChannelTotal];
						string value = text.Substring(2, 2);
						byte b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iChannelTotal; i++)
						{
							o_bEnabled[i] = (b & (1 << i)) > 0;
						}
						return true;
					}
					if (i_iChannelTotal <= 16)
					{
						o_bEnabled = new bool[i_iChannelTotal];
						string value = text.Substring(2, 2);
						byte b = Convert.ToByte(value, 16);
						for (int i = 0; i < 8; i++)
						{
							o_bEnabled[i] = (b & (1 << i)) > 0;
						}
						value = text.Substring(0, 2);
						b = Convert.ToByte(value, 16);
						for (int i = 0; i < i_iChannelTotal - 8; i++)
						{
							o_bEnabled[i + 8] = (b & (1 << i)) > 0;
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
		o_bEnabled = null;
		return false;
	}

	public bool GetWDTMask(int i_iSlot, out bool[] o_bEnabled)
	{
		string text = "$" + base.Address.ToString("X02") + "X";
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				try
				{
					o_bEnabled = new bool[16];
					for (int i = 0; i < 2; i++)
					{
						string value = text2.Substring(i * 2, 2);
						byte b = Convert.ToByte(value, 16);
						for (int j = 0; j < 8; j++)
						{
							o_bEnabled[(1 - i) * 8 + j] = (b & (1 << j)) > 0;
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
		o_bEnabled = null;
		return false;
	}

	public bool GetWDTMask(out bool o_bEnabled, out bool[] o_bMask)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "X\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 4)
			{
				string text2 = text.Substring(0, 2);
				string value = text.Substring(2, 2);
				if (text2 == "01")
				{
					o_bEnabled = true;
				}
				else
				{
					o_bEnabled = false;
				}
				try
				{
					o_bMask = new bool[8];
					byte b = Convert.ToByte(value, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bMask[i] = (b & (1 << i)) > 0;
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
		o_bEnabled = false;
		o_bMask = null;
		return false;
	}

	public bool SetWDTMask(int i_iSlot, bool[] i_bEnabled)
	{
		int num = ((i_bEnabled.Length <= 16) ? i_bEnabled.Length : 16);
		ushort num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (i_bEnabled[i])
			{
				num2 += Convert.ToUInt16(1 << i);
			}
		}
		string text = "$" + base.Address.ToString("X02") + "X";
		if (i_iSlot >= 0)
		{
			text = text + "S" + i_iSlot.ToString("X");
		}
		text = text + "D" + num2.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetWDTMask(bool i_bEnabled, bool[] i_bMask)
	{
		int num = ((i_bMask.Length <= 8) ? i_bMask.Length : 8);
		byte b = 0;
		for (int i = 0; i < num; i++)
		{
			if (i_bMask[i])
			{
				b += Convert.ToByte(1 << i);
			}
		}
		string text = "$" + base.Address.ToString("X02") + "X";
		text = ((!i_bEnabled) ? (text + "00" + b.ToString("X02") + "\r") : (text + "01" + b.ToString("X02") + "\r"));
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
		string i_szSend = "$" + base.Address.ToString("X02") + "COC" + i_iChannel.ToString("X") + "\r";
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
		string text = "$" + base.Address.ToString("X02") + "COC" + i_iChannel.ToString("X");
		text = text + i_byConfig.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetPulseOutputWidthAndDelayTime(int i_iChannel, out long o_lPulseHighWidth, out long o_lPulseLowWidth, out long o_lHighToLowDelay, out long o_lLowToHighDelay)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "9" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				string value = text.Substring(0, 8);
				o_lPulseLowWidth = Convert.ToUInt32(value, 16);
				value = text.Substring(8, 8);
				o_lPulseHighWidth = Convert.ToUInt32(value, 16);
				value = text.Substring(16, 8);
				o_lLowToHighDelay = Convert.ToUInt32(value, 16);
				value = text.Substring(24, 8);
				o_lHighToLowDelay = Convert.ToUInt32(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lPulseHighWidth = 0L;
		o_lPulseLowWidth = 0L;
		o_lLowToHighDelay = 0L;
		o_lHighToLowDelay = 0L;
		return false;
	}

	public bool SetPulseOutputWidthAndDelayTime(int i_iChannel, long i_lPulseHighWidth, long i_lPulseLowWidth, long i_lHighToLowDelay, long i_lLowToHighDelay)
	{
		string text = "$" + base.Address.ToString("X02") + "9" + i_iChannel.ToString("X");
		text = text + i_lPulseLowWidth.ToString("X08") + i_lPulseHighWidth.ToString("X08");
		text = text + i_lLowToHighDelay.ToString("X08") + i_lHighToLowDelay.ToString("X08");
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetPulseOutputCount(int i_iChannel, out bool o_bContinue, out long o_lPulseCount)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "ERFF" + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				string text2 = text.Substring(0, 1);
				o_bContinue = text2 == "1";
				text2 = text.Substring(1, 8);
				o_lPulseCount = Convert.ToInt64(text2, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bContinue = false;
		o_lPulseCount = 0L;
		return false;
	}

	public bool SetPulseOutputCount(int i_iChannel, bool i_bContinue, long i_lPulseCount)
	{
		string text = "#" + base.Address.ToString("X02") + "ERFF" + i_iChannel.ToString("X02");
		text = ((!i_bContinue) ? (text + i_lPulseCount.ToString("X08")) : (text + "00000000"));
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetPulseOutputWidthAndDelayTime(out long[] o_lPulseHighWidth, out long[] o_lPulseLowWidth, out long[] o_lHighToLowDelay, out long[] o_lLowToHighDelay)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "8\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			int num = text.Length / 32;
			try
			{
				o_lPulseHighWidth = new long[num];
				o_lPulseLowWidth = new long[num];
				o_lLowToHighDelay = new long[num];
				o_lHighToLowDelay = new long[num];
				for (int i = 0; i < num; i++)
				{
					string value = text.Substring(i * 16, 8);
					o_lPulseLowWidth[i] = Convert.ToUInt32(value, 16);
					value = text.Substring(i * 16 + 8, 8);
					o_lPulseHighWidth[i] = Convert.ToUInt32(value, 16);
					value = text.Substring(num * 16 + i * 16, 8);
					o_lLowToHighDelay[i] = Convert.ToUInt32(value, 16);
					value = text.Substring(num * 16 + i * 16 + 8, 8);
					o_lHighToLowDelay[i] = Convert.ToUInt32(value, 16);
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lPulseHighWidth = null;
		o_lPulseLowWidth = null;
		o_lLowToHighDelay = null;
		o_lHighToLowDelay = null;
		return false;
	}

	public bool SetPulseOutputWidthAndDelayTime(long[] i_lPulseHighWidth, long[] i_lPulseLowWidth, long[] i_lHighToLowDelay, long[] i_lLowToHighDelay)
	{
		if (i_lPulseHighWidth.Length == i_lPulseLowWidth.Length && i_lPulseLowWidth.Length == i_lHighToLowDelay.Length && i_lHighToLowDelay.Length == i_lLowToHighDelay.Length)
		{
			string text = "$" + base.Address.ToString("X02") + "8";
			int num = i_lPulseHighWidth.Length;
			for (int i = 0; i < num; i++)
			{
				text += i_lPulseLowWidth[i].ToString("X08");
				text += i_lPulseHighWidth[i].ToString("X08");
			}
			for (int i = 0; i < num; i++)
			{
				text += i_lLowToHighDelay[i].ToString("X08");
				text += i_lHighToLowDelay[i].ToString("X08");
			}
			text += "\r";
			string o_szRecv;
			return ASCIISendRecv(text, out o_szRecv);
		}
		base.LastError = ErrorCode.Adam_Invalid_Param;
		return false;
	}
}
