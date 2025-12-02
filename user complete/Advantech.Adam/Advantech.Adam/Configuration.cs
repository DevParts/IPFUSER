using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Advantech.Common;

namespace Advantech.Adam;

public class Configuration : AdamBase
{
	public Configuration(AdamCom i_com)
		: base(i_com)
	{
	}

	public Configuration(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	public static int GetBaudrateTotal()
	{
		return 8;
	}

	public static byte GetBaudrateCode(int i_iIndex)
	{
		return i_iIndex switch
		{
			0 => 3, 
			1 => 4, 
			2 => 5, 
			3 => 6, 
			4 => 7, 
			5 => 8, 
			6 => 9, 
			_ => 10, 
		};
	}

	public static int GetBaudrateIndex(byte i_byBaudrate)
	{
		return (Adam_Baudrate)i_byBaudrate switch
		{
			Adam_Baudrate.Baud_1200 => 0, 
			Adam_Baudrate.Baud_2400 => 1, 
			Adam_Baudrate.Baud_4800 => 2, 
			Adam_Baudrate.Baud_9600 => 3, 
			Adam_Baudrate.Baud_19200 => 4, 
			Adam_Baudrate.Baud_38400 => 5, 
			Adam_Baudrate.Baud_57600 => 6, 
			Adam_Baudrate.Baud_115200 => 7, 
			_ => -1, 
		};
	}

	public static string GetBaudrateName(byte i_byBaudrate)
	{
		string result = "";
		switch ((Adam_Baudrate)i_byBaudrate)
		{
		case Adam_Baudrate.Baud_1200:
			result = "1200 bps";
			break;
		case Adam_Baudrate.Baud_2400:
			result = "2400 bps";
			break;
		case Adam_Baudrate.Baud_4800:
			result = "4800 bps";
			break;
		case Adam_Baudrate.Baud_9600:
			result = "9600 bps";
			break;
		case Adam_Baudrate.Baud_19200:
			result = "19200 bps";
			break;
		case Adam_Baudrate.Baud_38400:
			result = "38400 bps";
			break;
		case Adam_Baudrate.Baud_57600:
			result = "57600 bps";
			break;
		case Adam_Baudrate.Baud_115200:
			result = "115200 bps";
			break;
		}
		return result;
	}

	public bool GetModuleName(out string o_szName)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "M\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			o_szName = o_szRecv.Substring(3, o_szRecv.Length - 4);
			return true;
		}
		o_szName = "";
		return false;
	}

	public bool GetFirmwareVer(out string o_szVer)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "F\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			o_szVer = o_szRecv.Substring(3, o_szRecv.Length - 4);
			return true;
		}
		o_szVer = "";
		return false;
	}

	public bool GetSlotInfo(out string[] o_szSlotInfo)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "T\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			int length = text.Length;
			if (length == 8 || length == 16)
			{
				int num = length / 2;
				o_szSlotInfo = new string[num];
				for (int i = 0; i < num; i++)
				{
					string text2 = text.Substring(i * 2, 2);
					switch (text2)
					{
					case "FF":
						o_szSlotInfo[i] = "";
						break;
					case "AI":
						o_szSlotInfo[i] = "5000AI";
						break;
					case "AO":
						o_szSlotInfo[i] = "5000AO";
						break;
					case "DI":
						o_szSlotInfo[i] = "5000DI";
						break;
					case "DO":
						o_szSlotInfo[i] = "5000DO";
						break;
					case "7H":
						o_szSlotInfo[i] = "5017H";
						break;
					case "7U":
						o_szSlotInfo[i] = "5017UH";
						break;
					case "7P":
						o_szSlotInfo[i] = "5017P";
						break;
					case "8P":
						o_szSlotInfo[i] = "5018P";
						break;
					default:
						o_szSlotInfo[i] = "50" + text2;
						break;
					}
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_szSlotInfo = null;
		return false;
	}

	public bool GetSlotInfo(out Adam5000Type[] o_aSlotInfo)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "T\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				int length = text.Length;
				if (length == 8 || length == 16)
				{
					int num = length / 2;
					o_aSlotInfo = new Adam5000Type[num];
					for (int i = 0; i < num; i++)
					{
						string text2 = text.Substring(i * 2, 2);
						switch (text2)
						{
						case "FF":
							o_aSlotInfo[i] = Adam5000Type.Non;
							break;
						case "AI":
							o_aSlotInfo[i] = Adam5000Type.Adam5000AI;
							break;
						case "AO":
							o_aSlotInfo[i] = Adam5000Type.Adam5000AO;
							break;
						case "DI":
							o_aSlotInfo[i] = Adam5000Type.Adam5000DI;
							break;
						case "DO":
							o_aSlotInfo[i] = Adam5000Type.Adam5000DO;
							break;
						case "7H":
							o_aSlotInfo[i] = Adam5000Type.Adam5017H;
							break;
						case "7U":
							o_aSlotInfo[i] = Adam5000Type.Adam5017UH;
							break;
						case "7P":
							o_aSlotInfo[i] = Adam5000Type.Adam5017P;
							break;
						case "8P":
							o_aSlotInfo[i] = Adam5000Type.Adam5018P;
							break;
						default:
							o_aSlotInfo[i] = (Adam5000Type)(5000 + Convert.ToInt32(text2, 10));
							break;
						}
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
		o_aSlotInfo = null;
		return false;
	}

	public bool GetWDTTimeout(out float o_fWDTTimeout)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "Y\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
				o_fWDTTimeout = Convert.ToSingle((double)Convert.ToSingle(value) / 10.0);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fWDTTimeout = 0f;
		return false;
	}

	public bool SetWDTTimeout(float i_fWDTTimeout)
	{
		int num = Convert.ToInt32(i_fWDTTimeout * 10f);
		if (num > 9999)
		{
			num = 9999;
		}
		else if (num < 0)
		{
			num = 0;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "X" + num.ToString("0000") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetThermocoupleMode(out int o_iMode)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "OD\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
				o_iMode = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iMode = 0;
		return false;
	}

	public bool SetThermocoupleMode(int i_iMode)
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "OD" + i_iMode.ToString("00") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetCommunicationSafety(out float o_fTimeout)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "X1\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			try
			{
				string value = o_szRecv.Substring(1, o_szRecv.Length - 2);
				int value2 = Convert.ToInt32(value, 16);
				o_fTimeout = Convert.ToSingle((double)Convert.ToSingle(value2) / 10.0);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fTimeout = 0f;
		return false;
	}

	public bool SetCommunicationSafety(float o_fTimeout)
	{
		int num = Convert.ToInt32(o_fTimeout * 10f);
		if (num > 65535)
		{
			num = 65535;
		}
		else if (num < 0)
		{
			num = 0;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "X0" + num.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetWDTTimeout(out int o_iWDTTimeout)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "XR\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
				o_iWDTTimeout = Convert.ToInt32(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iWDTTimeout = 0;
		return false;
	}

	public bool SetWDTTimeout(int i_iWDTTimeout)
	{
		if (i_iWDTTimeout > 9999)
		{
			i_iWDTTimeout = 9999;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "X" + i_iWDTTimeout.ToString("0000") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetWDTMask(out bool[] o_bEnabled)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "XER\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 2)
				{
					o_bEnabled = new bool[8];
					byte b = Convert.ToByte(text, 16);
					for (int i = 0; i < 8; i++)
					{
						o_bEnabled[i] = (b & (1 << i)) > 0;
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
		o_bEnabled = null;
		return false;
	}

	public bool SetWDTMask(bool[] i_bEnabled)
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
		string i_szSend = "$" + base.Address.ToString("X02") + "XEW" + b.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetDataStreamingIP(int i_iIndex, out string o_szIP)
	{
		byte[] array = new byte[4];
		if (i_iIndex < 0 || i_iIndex > 7)
		{
			i_iIndex = 0;
		}
		string i_szSend = "%" + base.Address.ToString("X02") + "GETUDP" + i_iIndex + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			try
			{
				string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
				if (text.Length == 8)
				{
					for (int i = 0; i < 4; i++)
					{
						string value = text.Substring(i * 2, 2);
						array[i] = Convert.ToByte(value, 16);
					}
					o_szIP = $"{array[0]}.{array[1]}.{array[2]}.{array[3]}";
					return true;
				}
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_szIP = "";
		return false;
	}

	public bool SetDataStreamingIP(int i_iIndex, string i_szIP)
	{
		if (i_iIndex < 0 || i_iIndex > 7)
		{
			i_iIndex = 0;
		}
		byte[] addressBytes = IPAddress.Parse(i_szIP).GetAddressBytes();
		string i_szSend = "%" + base.Address.ToString("X02") + "SETUDP" + i_iIndex + addressBytes[0].ToString("X02") + addressBytes[1].ToString("X02") + addressBytes[2].ToString("X02") + addressBytes[3].ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetDataStreamingMask(int i_iIndex, out bool o_bEnabled)
	{
		if (i_iIndex < 0 || i_iIndex > 7)
		{
			i_iIndex = 0;
		}
		string i_szSend = "%" + base.Address.ToString("X02") + "GETUDP" + i_iIndex + "STU\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			try
			{
				string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
				if (text.Length == 2)
				{
					byte b = Convert.ToByte(text, 16);
					o_bEnabled = b > 0;
					return true;
				}
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_bEnabled = false;
		return false;
	}

	public bool SetDataStreamingMask(int i_iIndex, bool i_bEnabled)
	{
		if (i_iIndex < 0 || i_iIndex > 7)
		{
			i_iIndex = 0;
		}
		string text = "%" + base.Address.ToString("X02") + "SETUDP" + i_iIndex;
		text = ((!i_bEnabled) ? (text + "P\r") : (text + "R\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetDataStreamingInterval(out int o_iInterval)
	{
		string i_szSend = "%" + base.Address.ToString("X02") + "GETUDPST\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 2)
		{
			try
			{
				string text = o_szRecv.Substring(1, o_szRecv.Length - 2);
				if (text.Length == 8)
				{
					o_iInterval = Convert.ToInt32(text, 16);
					return true;
				}
				base.LastError = ErrorCode.Adam_Invalid_Length;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iInterval = 0;
		return false;
	}

	public bool SetDataStreamingInterval(int i_iInterval)
	{
		if (i_iInterval < 5)
		{
			i_iInterval = 5;
		}
		if (i_iInterval > 36000000)
		{
			i_iInterval = 36000000;
		}
		string i_szSend = "%" + base.Address.ToString("X02") + "SETUDPST" + i_iInterval.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetEventTriggerMask(Adam_EventType i_Type, out bool[] o_bEnabled)
	{
		int num = Convert.ToInt32(i_Type);
		string i_szSend = "%" + base.Address.ToString("X02") + "ETSM" + num + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 32)
				{
					o_bEnabled = new bool[128];
					for (int i = 0; i < 8; i++)
					{
						string value = text.Substring(i * 4, 4);
						ushort num2 = Convert.ToUInt16(value, 16);
						for (int j = 0; j < 16; j++)
						{
							o_bEnabled[i * 16 + j] = (num2 & (1 << j)) > 0;
						}
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
		o_bEnabled = null;
		return false;
	}

	public bool SetEventTriggerMask(Adam_EventType i_Type, bool[] i_bEnabled)
	{
		int num = Convert.ToInt32(i_Type);
		string text = "%" + base.Address.ToString("X02") + "ETSM" + num;
		int num2 = ((i_bEnabled.Length <= 128) ? i_bEnabled.Length : 128);
		int num3 = 0;
		ushort num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			num3 = i % 16;
			if (i_bEnabled[i])
			{
				num4 += Convert.ToUInt16(1 << num3);
			}
			if (num3 == 15)
			{
				text += num4.ToString("X04");
				num4 = 0;
			}
		}
		if (num3 != 15)
		{
			text += num4.ToString("X04");
		}
		text += "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetEventTriggerMask(Adam_EventType i_Type, out string o_szMask)
	{
		int num = Convert.ToInt32(i_Type);
		string i_szSend = "%" + base.Address.ToString("X02") + "ETSM" + num + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				o_szMask = o_szRecv.Substring(3, o_szRecv.Length - 4);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_szMask = "";
		return false;
	}

	public bool SetEventTriggerMask(Adam_EventType i_Type, string i_szMask)
	{
		int num = Convert.ToInt32(i_Type);
		string i_szSend = "%" + base.Address.ToString("X02") + "ETSM" + num + i_szMask + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool IsValidPassword(string i_szPassword)
	{
		if (GetProtocolType() != ProtocolType.Tcp)
		{
			return false;
		}
		string s;
		if (i_szPassword.Length < 8)
		{
			string text = new string(' ', 8 - i_szPassword.Length);
			s = i_szPassword + text;
		}
		else
		{
			s = i_szPassword.Substring(0, 8);
		}
		byte[] bytes = Encoding.ASCII.GetBytes(s);
		int num = bytes.Length;
		for (int i = 0; i < num; i++)
		{
			bytes[i] = Convert.ToByte(bytes[i] ^ 0x3F);
		}
		string text2 = Encoding.ASCII.GetString(bytes, 0, num);
		string i_szSend = "$" + base.Address.ToString("X02") + "PW0" + text2 + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool ChangePassword(string i_szOldPassword, string i_szNewPassword)
	{
		if (GetProtocolType() != ProtocolType.Tcp)
		{
			return false;
		}
		string s;
		if (i_szOldPassword.Length < 8)
		{
			string text = new string(' ', 8 - i_szOldPassword.Length);
			s = i_szOldPassword + text;
		}
		else
		{
			s = i_szOldPassword.Substring(0, 8);
		}
		byte[] bytes = Encoding.ASCII.GetBytes(s);
		int num = bytes.Length;
		for (int i = 0; i < num; i++)
		{
			bytes[i] = Convert.ToByte(bytes[i] ^ 0x3F);
		}
		string text2 = Encoding.ASCII.GetString(bytes, 0, num);
		string text3 = "$" + base.Address.ToString("X02") + "PW1" + text2;
		if (i_szNewPassword.Length < 8)
		{
			string text = new string(' ', 8 - i_szNewPassword.Length);
			s = i_szNewPassword + text;
		}
		else
		{
			s = i_szNewPassword.Substring(0, 8);
		}
		bytes = Encoding.ASCII.GetBytes(s);
		num = bytes.Length;
		for (int i = 0; i < num; i++)
		{
			bytes[i] = Convert.ToByte(bytes[i] ^ 0x3F);
		}
		text2 = Encoding.ASCII.GetString(bytes, 0, num);
		text3 = text3 + text2 + "\r";
		string o_szRecv;
		return ASCIISendRecv(text3, out o_szRecv);
	}

	public bool ResetPassword()
	{
		if (GetProtocolType() != ProtocolType.Tcp)
		{
			return false;
		}
		string i_szSend = "$" + base.Address.ToString("X02") + "RST\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetWirelessAPTotal(out int o_iTotal)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "NC\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			try
			{
				string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
				o_iTotal = Convert.ToInt32(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_iTotal = 0;
		return false;
	}

	public bool GetWirelessAPInfo(int i_iIndex, out AdamWirelessAP o_objAP)
	{
		byte[] array = new byte[32];
		o_objAP = new AdamWirelessAP();
		string i_szSend = "$" + base.Address.ToString("X02") + "NN" + i_iIndex.ToString("X02") + "FF\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length >= 87)
		{
			try
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				int i;
				string value;
				for (i = 0; i < 32; i++)
				{
					value = text.Substring(i * 2, 2);
					array[i] = Convert.ToByte(value, 16);
					if (array[i] == 0)
					{
						break;
					}
				}
				byte[] array2 = new byte[i];
				Array.Copy(array, 0, array2, 0, i);
				o_objAP.SSID = Encoding.ASCII.GetString(array2, 0, i);
				o_objAP.MAC = text.Substring(64, 2) + ":" + text.Substring(66, 2) + ":" + text.Substring(68, 2) + ":" + text.Substring(70, 2) + ":" + text.Substring(72, 2) + ":" + text.Substring(74, 2);
				value = text.Substring(76, 2);
				o_objAP.Channel = Convert.ToByte(value, 16);
				value = text.Substring(78, 2);
				o_objAP.Mode = Convert.ToByte(value, 16);
				if (text.Substring(80, 1) == "1")
				{
					o_objAP.WepUsed = true;
				}
				else
				{
					o_objAP.WepUsed = false;
				}
				value = text.Substring(81, 2);
				o_objAP.Strength = Convert.ToByte(value, 16);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		return false;
	}

	public bool GetModuleConfig(out Adam4000Config o_objCon)
	{
		o_objCon = new Adam4000Config();
		string text = "$" + base.Address.ToString("X02");
		text += "2\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(1, o_szRecv.Length - 2);
			if (text2.Length == 8)
			{
				try
				{
					string value = text2.Substring(0, 2);
					o_objCon.Address = Convert.ToByte(value, 16);
					value = text2.Substring(2, 2);
					o_objCon.TypeCode = Convert.ToByte(value, 16);
					value = text2.Substring(4, 2);
					o_objCon.Baudrate = Convert.ToByte(value, 16);
					value = text2.Substring(6, 2);
					o_objCon.Status = Convert.ToByte(value, 16);
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

	public bool SetModuleConfig(Adam4000Config o_objCon)
	{
		string text = "%" + base.Address.ToString("X02") + o_objCon.Address.ToString("X02");
		string text2 = text;
		text = text2 + o_objCon.TypeCode.ToString("X02") + o_objCon.Baudrate.ToString("X02") + o_objCon.Status.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetModuleConfig(out Adam5000Config o_objCon)
	{
		o_objCon = new Adam5000Config();
		string i_szSend = "$" + base.Address.ToString("X02") + "2\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 4)
			{
				try
				{
					string value = text.Substring(0, 2);
					o_objCon.Baudrate = Convert.ToByte(value, 16);
					value = text.Substring(2, 2);
					o_objCon.Status = Convert.ToByte(value, 16);
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

	public bool SetModuleConfig(Adam5000Config o_objCon)
	{
		string text = "%" + base.Address.ToString("X02") + "00";
		text = text + o_objCon.Baudrate.ToString("X02") + o_objCon.Status.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetModuleLocated(bool i_bTurnOn)
	{
		string text = "#" + base.Address.ToString("X02") + "FQ";
		text = ((!i_bTurnOn) ? (text + "0\r") : (text + "1\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetModuleCOMConfig(out AdamCOMConfig o_objCon)
	{
		o_objCon = new AdamCOMConfig();
		string i_szSend = "$" + base.Address.ToString("X02") + "OU\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 3)
			{
				try
				{
					string value = text.Substring(1, 2);
					o_objCon.Status = Convert.ToByte(value, 16);
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

	public bool SetModuleCOMConfig(AdamCOMConfig o_objCon)
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "OU0" + o_objCon.Status.ToString("X02") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetModbusAddressMode(out bool o_bFixed)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "PD\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 1)
			{
				if (text == "0")
				{
					o_bFixed = true;
				}
				else
				{
					o_bFixed = false;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bFixed = false;
		return false;
	}

	public bool GetModbusAddressConfig(int i_iSlot, out int o_iStartAddr, out int o_iLengthIndex)
	{
		string i_szSend = "$" + base.Address.ToString("X02") + "PN" + i_iSlot.ToString("X02") + "FF\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 5)
			{
				try
				{
					string value = text.Substring(0, 4);
					o_iStartAddr = Convert.ToInt32(value, 16);
					value = text.Substring(4, 1);
					o_iLengthIndex = Convert.ToInt32(value, 16);
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
		o_iStartAddr = 0;
		o_iLengthIndex = 0;
		return false;
	}

	public bool SetModbusAddressConfig(int i_iSlot, int i_iStartAddr, int i_iLengthIndex)
	{
		if (i_iLengthIndex < 0)
		{
			i_iLengthIndex = 0;
		}
		if (i_iLengthIndex > 7)
		{
			i_iLengthIndex = 7;
		}
		string i_szSend = "#" + base.Address.ToString("X02") + "PN" + i_iSlot.ToString("X02") + "FF" + i_iStartAddr.ToString("X04") + i_iLengthIndex + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool FinishModbusAddressConfig()
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "PD\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetP2P_Config(int i_iChannel, out bool o_bState, out int o_iPeriodTime, out byte[] o_byIP, out int o_iModbusAddr, out byte o_byModuleID, out bool[] o_bMask)
	{
		o_bState = false;
		o_iPeriodTime = -1;
		o_byIP = null;
		o_byModuleID = 0;
		o_iModbusAddr = -1;
		o_bMask = null;
		string i_szSend = "$" + base.Address.ToString("X02") + "RbFF" + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length >= 30)
		{
			try
			{
				string text = o_szRecv.Substring(3, 1);
				if (text == "1")
				{
					o_bState = true;
				}
				else
				{
					o_bState = false;
				}
				text = o_szRecv.Substring(4, 3);
				o_iPeriodTime = Convert.ToInt32(text, 16);
				if (o_iPeriodTime < 0 && o_iPeriodTime > 4095)
				{
					return false;
				}
				text = o_szRecv.Substring(7, 8);
				o_byIP = new byte[4];
				for (int i = 0; i < 4; i++)
				{
					o_byIP[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
				}
				text = o_szRecv.Substring(15, 4);
				o_iModbusAddr = Convert.ToInt32(text, 16);
				text = o_szRecv.Substring(19, 2);
				o_byModuleID = Convert.ToByte(text, 16);
				o_bMask = new bool[32];
				text = o_szRecv.Substring(21, 8);
				int num = Convert.ToInt32(text, 16);
				for (int i = 0; i < 32; i++)
				{
					o_bMask[i] = (num & (1 << i)) > 0;
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
				return false;
			}
		}
		return false;
	}

	public bool SetP2P_Config(int i_iChannel, bool i_bState, int i_iPeriodTime, byte[] i_byIP, int i_iModbusAddr, byte i_byModuleID, bool[] i_bMask)
	{
		int num = 0;
		string text = "#" + base.Address.ToString("X02") + "RbFF" + i_iChannel.ToString("X02");
		text = ((!i_bState) ? (text + "0") : (text + "1"));
		text += i_iPeriodTime.ToString("X03");
		for (int i = 0; i < 4; i++)
		{
			text += i_byIP[i].ToString("X02");
		}
		text += i_iModbusAddr.ToString("X04");
		text += i_byModuleID.ToString("X02");
		for (int i = 0; i < i_bMask.Length; i++)
		{
			if (i_bMask[i])
			{
				num += 1 << i;
			}
		}
		text += num.ToString("X08");
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv))
		{
			try
			{
				if (o_szRecv == ">" + base.Address.ToString("X02") + "\r")
				{
					return true;
				}
				return false;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
				return false;
			}
		}
		return false;
	}

	public bool GetAccessControl_Config(int i_iIndex, out bool o_bState, out int o_iMode, out byte[] o_byIP_MAC)
	{
		o_bState = false;
		o_iMode = -1;
		o_byIP_MAC = null;
		string i_szSend = "$" + base.Address.ToString("X02") + "SaFF" + i_iIndex.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length >= 13)
		{
			try
			{
				string text = o_szRecv.Substring(3, 1);
				if (text == "1")
				{
					o_bState = true;
				}
				else
				{
					o_bState = false;
				}
				text = o_szRecv.Substring(4, 1);
				if (text == "0")
				{
					o_iMode = 0;
				}
				else
				{
					o_iMode = 1;
				}
				text = o_szRecv.Substring(5, 12);
				o_byIP_MAC = new byte[6];
				for (int i = 0; i < 6; i++)
				{
					o_byIP_MAC[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
				return false;
			}
		}
		return false;
	}

	public bool SetAccessControl_Config(int i_iIndex, bool i_bState, int i_iMode, byte[] i_byIP_MAC)
	{
		string text = "#" + base.Address.ToString("X02") + "SaFF" + i_iIndex.ToString("X02");
		text = ((!i_bState) ? (text + "0") : (text + "1"));
		text += i_iMode.ToString("X01");
		if (i_byIP_MAC != null)
		{
			for (int i = 0; i < 6; i++)
			{
				text = ((i < i_byIP_MAC.Length) ? (text + i_byIP_MAC[i].ToString("X02")) : (text + "00"));
			}
		}
		else
		{
			text += "000000000000";
		}
		text += "\r";
		if (ASCIISendRecv(text, out var o_szRecv))
		{
			try
			{
				if (o_szRecv == ">" + base.Address.ToString("X02") + "\r")
				{
					return true;
				}
				return false;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
				return false;
			}
		}
		return false;
	}

	public bool VerifyAccessControl(out bool o_bValid)
	{
		o_bValid = false;
		string i_szSend = "$" + base.Address.ToString("X02") + "SEFFFF\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length >= 4)
		{
			try
			{
				string text = o_szRecv.Substring(3, 1);
				if (text == "1")
				{
					o_bValid = true;
				}
				else
				{
					o_bValid = false;
				}
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
				return false;
			}
		}
		return false;
	}
}
