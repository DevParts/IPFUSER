using System;
using Advantech.Common;

namespace Advantech.Adam;

public class Alarm : AdamBase
{
	public Alarm(AdamCom i_com)
		: base(i_com)
	{
	}

	public Alarm(AdamSocket i_socket)
		: base(i_socket)
	{
	}

	protected bool GetMode(int i_iSlot, int i_iChannel, int i_iHighLow, out Adam_AIAlarmMode o_mode)
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
		text = ((i_iHighLow != 1) ? (text + "AL\r") : (text + "AH\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, 1);
			if (text2 == "M")
			{
				o_mode = Adam_AIAlarmMode.Momentary;
			}
			else if (text2 == "L")
			{
				o_mode = Adam_AIAlarmMode.Latch;
			}
			else
			{
				o_mode = Adam_AIAlarmMode.Disable;
			}
			return true;
		}
		o_mode = Adam_AIAlarmMode.Unknown;
		return false;
	}

	protected bool GetMode(int i_iSlot, int i_iChannel, int i_iHighLow, out Adam5000_CounterAlarmMode o_mode)
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
		text = ((i_iHighLow != 1) ? (text + "AL\r") : (text + "AH\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2 == "L")
			{
				o_mode = Adam5000_CounterAlarmMode.Latch;
			}
			else
			{
				o_mode = Adam5000_CounterAlarmMode.Disable;
			}
			return true;
		}
		o_mode = Adam5000_CounterAlarmMode.Unknown;
		return false;
	}

	protected bool SetMode(int i_iSlot, int i_iChannel, int i_iHighLow, Adam_AIAlarmMode i_mode)
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
		text = ((i_iHighLow != 1) ? (text + "AL") : (text + "AH"));
		string o_szRecv2;
		string o_szRecv;
		switch (i_mode)
		{
		case Adam_AIAlarmMode.Latch:
		{
			string i_szSend2 = text + "L\r";
			string i_szSend = text + "EE\r";
			if (ASCIISendRecv(i_szSend2, out o_szRecv2))
			{
				return ASCIISendRecv(i_szSend, out o_szRecv);
			}
			return false;
		}
		case Adam_AIAlarmMode.Momentary:
		{
			string i_szSend2 = text + "M\r";
			string i_szSend = text + "EE\r";
			if (ASCIISendRecv(i_szSend2, out o_szRecv2))
			{
				return ASCIISendRecv(i_szSend, out o_szRecv);
			}
			return false;
		}
		default:
		{
			string i_szSend = text + "ED\r";
			return ASCIISendRecv(i_szSend, out o_szRecv);
		}
		}
	}

	protected bool SetMode(int i_iSlot, int i_iChannel, int i_iHighLow, Adam5000_CounterAlarmMode i_mode)
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
		text = ((i_iHighLow != 1) ? (text + "AL") : (text + "AH"));
		string o_szRecv;
		string i_szSend;
		if (i_mode == Adam5000_CounterAlarmMode.Latch)
		{
			i_szSend = text + "EE\r";
			return ASCIISendRecv(i_szSend, out o_szRecv);
		}
		i_szSend = text + "ED\r";
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	protected bool GetLimit(int i_iSlot, int i_iChannel, int i_iHighLow, out float o_fLimit)
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
		text = ((i_iHighLow != 1) ? (text + "RLU\r") : (text + "RHU\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length > 7)
			{
				text2 = text2.Substring(0, 7);
			}
			try
			{
				o_fLimit = Convert.ToSingle(text2, m_numberFormatInfo);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fLimit = 0f;
		return false;
	}

	protected bool SetLimit(int i_iSlot, int i_iChannel, int i_iHighLow, float i_fLimit)
	{
		float num = ((!(i_fLimit >= 0f)) ? (i_fLimit * -1f) : i_fLimit);
		string text = ((num >= 0f && num < 10f) ? "+0.0000;-0.0000" : ((num >= 10f && num < 100f) ? "+00.000;-00.000" : ((!(num >= 100f) || !(num < 1000f)) ? "+0000.0;-0000.0" : "+000.00;-000.00")));
		string text2 = "$" + base.Address.ToString("X02");
		if (i_iSlot >= 0)
		{
			text2 = text2 + "S" + i_iSlot.ToString("X");
		}
		if (i_iChannel >= 0)
		{
			text2 = text2 + "C" + i_iChannel.ToString("X");
		}
		text2 = ((i_iHighLow != 1) ? (text2 + "ALU" + i_fLimit.ToString(text, m_numberFormatInfo) + "\r") : (text2 + "AHU" + i_fLimit.ToString(text, m_numberFormatInfo) + "\r"));
		string o_szRecv;
		return ASCIISendRecv(text2, out o_szRecv);
	}

	protected bool GetLimit(int i_iSlot, int i_iChannel, int i_iHighLow, out long o_lLimit)
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
		text = ((i_iHighLow != 1) ? (text + "RLU\r") : (text + "RHU\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string value = o_szRecv.Substring(3, o_szRecv.Length - 4);
			try
			{
				o_lLimit = Convert.ToInt64(value);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_lLimit = 0L;
		return false;
	}

	protected bool SetLimit(int i_iSlot, int i_iChannel, int i_iHighLow, long i_lLimit)
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
		text = ((i_iHighLow != 1) ? (text + "ALU" + i_lLimit.ToString("0000000000") + "\r") : (text + "AHU" + i_lLimit.ToString("0000000000") + "\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	protected bool GetMapping(int i_iSlot, int i_iChannel, int i_iHighLow, out int o_iSlot, out int o_iChannel)
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
		text = ((i_iHighLow != 1) ? (text + "RLC\r") : (text + "RHC\r"));
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 4)
			{
				string text3 = text2.Substring(1, 1);
				string text4 = text2.Substring(3, 1);
				if (text3 == "*" || text4 == "*")
				{
					o_iSlot = -1;
					o_iChannel = -1;
					return true;
				}
				try
				{
					o_iSlot = Convert.ToInt32(text3, 16);
					o_iChannel = Convert.ToInt32(text4, 16);
					return true;
				}
				catch
				{
					base.LastError = ErrorCode.Adam_Invalid_Data;
				}
			}
			else if (text2.Length == 2)
			{
				o_iSlot = -1;
				string text4 = text2.Substring(1, 1);
				if (text4 == "*")
				{
					o_iChannel = -1;
					return true;
				}
				try
				{
					o_iChannel = Convert.ToInt32(text4, 16);
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
		o_iSlot = -1;
		o_iChannel = -1;
		return false;
	}

	protected bool SetMapping(int i_iSlot, int i_iChannel, int i_iHighLow, int i_iSlotMap, int i_iChMap)
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
		text = ((i_iHighLow != 1) ? (text + "ALC") : (text + "AHC"));
		text = ((i_iSlot >= 0) ? ((i_iSlotMap < 0 || i_iChMap < 0) ? (text + "S*C*\r") : (text + "S" + i_iSlotMap.ToString("X") + "C" + i_iChMap.ToString("X") + "\r")) : ((i_iChMap < 0) ? (text + "C*\r") : (text + "C" + i_iChMap.ToString("X") + "\r")));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	protected bool SetLatchClear(int i_iSlot, int i_iChannel, int i_iHighLow)
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
		text = ((i_iHighLow != 1) ? (text + "CL\r") : (text + "CH\r"));
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public static int GetModeTotal(Adam4000Type adam4000Type)
	{
		int result = 0;
		if (adam4000Type == Adam4000Type.Adam4011 || adam4000Type == Adam4000Type.Adam4011D || adam4000Type == Adam4000Type.Adam4012 || adam4000Type == Adam4000Type.Adam4016 || adam4000Type == Adam4000Type.Adam4080D)
		{
			result = 3;
		}
		return result;
	}

	public static byte GetModeCode(Adam4000Type adam4000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		if (adam4000Type == Adam4000Type.Adam4011 || adam4000Type == Adam4000Type.Adam4011D || adam4000Type == Adam4000Type.Adam4012 || adam4000Type == Adam4000Type.Adam4016 || adam4000Type == Adam4000Type.Adam4080D)
		{
			switch (i_iIndex)
			{
			case 0:
				result = 68;
				break;
			case 1:
				result = 76;
				break;
			case 2:
				result = 77;
				break;
			}
		}
		return result;
	}

	public static int GetModeIndex(Adam4000Type adam4000Type, byte i_byMode)
	{
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		if (adam4000Type == Adam4000Type.Adam4011 || adam4000Type == Adam4000Type.Adam4011D || adam4000Type == Adam4000Type.Adam4012 || adam4000Type == Adam4000Type.Adam4016 || adam4000Type == Adam4000Type.Adam4080D)
		{
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				return 0;
			case Adam_AIAlarmMode.Latch:
				return 1;
			case Adam_AIAlarmMode.Momentary:
				return 2;
			}
		}
		return -1;
	}

	public static string GetModeName(Adam4000Type adam4000Type, byte i_byMode)
	{
		string result = "";
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		if (adam4000Type == Adam4000Type.Adam4011 || adam4000Type == Adam4000Type.Adam4011D || adam4000Type == Adam4000Type.Adam4012 || adam4000Type == Adam4000Type.Adam4016 || adam4000Type == Adam4000Type.Adam4080D)
		{
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				result = "Disable";
				break;
			case Adam_AIAlarmMode.Latch:
				result = "Latch";
				break;
			case Adam_AIAlarmMode.Momentary:
				result = "Momentary";
				break;
			}
		}
		return result;
	}

	public static int GetModeTotal(Adam5000Type adam5000Type)
	{
		int result = 0;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5017H:
		case Adam5000Type.Adam5018P:
		case Adam5000Type.Adam5017UH:
		case Adam5000Type.Adam5017P:
			result = 3;
			break;
		case Adam5000Type.Adam5080:
		case Adam5000Type.Adam5081:
			result = 2;
			break;
		}
		return result;
	}

	public static byte GetModeCode(Adam5000Type adam5000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5017H:
		case Adam5000Type.Adam5018P:
		case Adam5000Type.Adam5017UH:
		case Adam5000Type.Adam5017P:
			switch (i_iIndex)
			{
			case 0:
				result = 68;
				break;
			case 1:
				result = 76;
				break;
			case 2:
				result = 77;
				break;
			}
			break;
		case Adam5000Type.Adam5080:
		case Adam5000Type.Adam5081:
			switch (i_iIndex)
			{
			case 0:
				result = 68;
				break;
			case 1:
				result = 76;
				break;
			}
			break;
		}
		return result;
	}

	public static int GetModeIndex(Adam5000Type adam5000Type, byte i_byMode)
	{
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5017H:
		case Adam5000Type.Adam5018P:
		case Adam5000Type.Adam5017UH:
		case Adam5000Type.Adam5017P:
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				return 0;
			case Adam_AIAlarmMode.Latch:
				return 1;
			case Adam_AIAlarmMode.Momentary:
				return 2;
			}
			break;
		case Adam5000Type.Adam5080:
		case Adam5000Type.Adam5081:
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				return 0;
			case Adam_AIAlarmMode.Latch:
				return 1;
			}
			break;
		}
		return -1;
	}

	public static string GetModeName(Adam5000Type adam5000Type, byte i_byMode)
	{
		string result = "";
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		switch (adam5000Type)
		{
		case Adam5000Type.Adam5013:
		case Adam5000Type.Adam5017:
		case Adam5000Type.Adam5018:
		case Adam5000Type.Adam5017H:
		case Adam5000Type.Adam5018P:
		case Adam5000Type.Adam5017UH:
		case Adam5000Type.Adam5017P:
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				result = "Disable";
				break;
			case Adam_AIAlarmMode.Latch:
				result = "Latch";
				break;
			case Adam_AIAlarmMode.Momentary:
				result = "Momentary";
				break;
			}
			break;
		case Adam5000Type.Adam5080:
		case Adam5000Type.Adam5081:
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				result = "Disable";
				break;
			case Adam_AIAlarmMode.Latch:
				result = "Latch";
				break;
			}
			break;
		}
		return result;
	}

	public static int GetModeTotal(Adam6000Type adam6000Type)
	{
		int result = 0;
		if (adam6000Type == Adam6000Type.Adam6015 || adam6000Type == Adam6000Type.Adam6017 || adam6000Type == Adam6000Type.Adam6018)
		{
			result = 3;
		}
		return result;
	}

	public static byte GetModeCode(Adam6000Type adam6000Type, int i_iIndex)
	{
		byte result = byte.MaxValue;
		if (adam6000Type == Adam6000Type.Adam6015 || adam6000Type == Adam6000Type.Adam6017 || adam6000Type == Adam6000Type.Adam6018)
		{
			switch (i_iIndex)
			{
			case 0:
				result = 68;
				break;
			case 1:
				result = 76;
				break;
			case 2:
				result = 77;
				break;
			}
		}
		return result;
	}

	public static int GetModeIndex(Adam6000Type adam6000Type, byte i_byMode)
	{
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		if (adam6000Type == Adam6000Type.Adam6015 || adam6000Type == Adam6000Type.Adam6017 || adam6000Type == Adam6000Type.Adam6018)
		{
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				return 0;
			case Adam_AIAlarmMode.Latch:
				return 1;
			case Adam_AIAlarmMode.Momentary:
				return 2;
			}
		}
		return -1;
	}

	public static string GetModeName(Adam6000Type adam6000Type, byte i_byMode)
	{
		string result = "";
		Adam_AIAlarmMode adam_AIAlarmMode = (Adam_AIAlarmMode)i_byMode;
		if (adam6000Type == Adam6000Type.Adam6015 || adam6000Type == Adam6000Type.Adam6017 || adam6000Type == Adam6000Type.Adam6018)
		{
			switch (adam_AIAlarmMode)
			{
			case Adam_AIAlarmMode.Disable:
				result = "Disable";
				break;
			case Adam_AIAlarmMode.Latch:
				result = "Latch";
				break;
			case Adam_AIAlarmMode.Momentary:
				result = "Momentary";
				break;
			}
		}
		return result;
	}

	public bool GetModeAlarmDO(int i_iDOTotal, out Adam_AIAlarmMode o_mode, out bool[] o_bAlarmDO)
	{
		if (i_iDOTotal == 2 || i_iDOTotal == 4)
		{
			string i_szSend = "@" + base.Address.ToString("X02") + "DI\r";
			if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
			{
				string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
				if (text.Length == 5)
				{
					try
					{
						string text2 = text.Substring(0, 1);
						if (text2 == "1")
						{
							o_mode = Adam_AIAlarmMode.Momentary;
						}
						else if (text2 == "2")
						{
							o_mode = Adam_AIAlarmMode.Latch;
						}
						else
						{
							o_mode = Adam_AIAlarmMode.Disable;
						}
						text2 = text.Substring(1, 2);
						byte b = Convert.ToByte(text2, 16);
						o_bAlarmDO = new bool[i_iDOTotal];
						for (int i = 0; i < i_iDOTotal; i++)
						{
							o_bAlarmDO[i] = (b & (1 << i)) > 0;
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
		}
		o_mode = Adam_AIAlarmMode.Disable;
		o_bAlarmDO = null;
		return false;
	}

	public bool SetAlarmDO(bool[] i_bAlarmDO)
	{
		if (i_bAlarmDO.Length == 2)
		{
			string text = "@" + base.Address.ToString("X02") + "DO";
			byte b = 0;
			for (int i = 0; i < 2; i++)
			{
				if (i_bAlarmDO[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
			string o_szRecv;
			return ASCIISendRecv(text, out o_szRecv);
		}
		base.LastError = ErrorCode.Adam_Invalid_Length;
		return false;
	}

	public bool SetExtDO(bool[] i_bAlarmDO)
	{
		if (i_bAlarmDO.Length == 2)
		{
			string text = "@" + base.Address.ToString("X02") + "DO";
			byte b = 16;
			for (int i = 0; i < 2; i++)
			{
				if (i_bAlarmDO[i])
				{
					b = Convert.ToByte(b + (1 << i));
				}
			}
			text = text + b.ToString("X02") + "\r";
			string o_szRecv;
			return ASCIISendRecv(text, out o_szRecv);
		}
		base.LastError = ErrorCode.Adam_Invalid_Length;
		return false;
	}

	public bool SetMode(Adam_AIAlarmMode i_mode)
	{
		string text = "@" + base.Address.ToString("X02");
		string o_szRecv;
		return ASCIISendRecv(i_mode switch
		{
			Adam_AIAlarmMode.Latch => text + "EAL\r", 
			Adam_AIAlarmMode.Momentary => text + "EAM\r", 
			_ => text + "DA\r", 
		}, out o_szRecv);
	}

	public bool SetLatchClear()
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "CA\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetHighLimit(out float o_fLimit)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "RH\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length > 7)
			{
				text = text.Substring(0, 7);
			}
			try
			{
				o_fLimit = Convert.ToSingle(text, m_numberFormatInfo);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fLimit = 0f;
		return false;
	}

	public bool SetHighLimit(float i_fLimit)
	{
		float num = ((!(i_fLimit >= 0f)) ? (i_fLimit * -1f) : i_fLimit);
		string text = ((num >= 0f && num < 10f) ? "+0.0000;-0.0000" : ((num >= 10f && num < 100f) ? "+00.000;-00.000" : ((!(num >= 100f) || !(num < 1000f)) ? "+0000.0;-0000.0" : "+000.00;-000.00")));
		string i_szSend = "@" + base.Address.ToString("X02") + "HI" + i_fLimit.ToString(text, m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLowLimit(out float o_fLimit)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "RL\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length > 7)
			{
				text = text.Substring(0, 7);
			}
			try
			{
				o_fLimit = Convert.ToSingle(text, m_numberFormatInfo);
				return true;
			}
			catch
			{
				base.LastError = ErrorCode.Adam_Invalid_Data;
			}
		}
		o_fLimit = 0f;
		return false;
	}

	public bool SetLowLimit(float i_fLimit)
	{
		float num = ((!(i_fLimit >= 0f)) ? (i_fLimit * -1f) : i_fLimit);
		string text = ((num >= 0f && num < 10f) ? "+0.0000;-0.0000" : ((num >= 10f && num < 100f) ? "+00.000;-00.000" : ((!(num >= 100f) || !(num < 1000f)) ? "+0000.0;-0000.0" : "+000.00;-000.00")));
		string i_szSend = "@" + base.Address.ToString("X02") + "LO" + i_fLimit.ToString(text, m_numberFormatInfo) + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLimit(int i_iChannel, out float o_fHighLimit, out float o_fLowLimit)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "B" + i_iChannel.ToString("X") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 12)
			{
				string text2 = text.Substring(0, 1);
				string text3 = text.Substring(1, 1);
				string value = text.Substring(2, 4);
				string text4 = text.Substring(6, 1);
				string text5 = text.Substring(7, 1);
				string value2 = text.Substring(8, 4);
				try
				{
					o_fHighLimit = Convert.ToSingle(Convert.ToInt32(value, 16));
					switch (text3)
					{
					case "1":
						o_fHighLimit /= 10f;
						break;
					case "2":
						o_fHighLimit /= 100f;
						break;
					case "3":
						o_fHighLimit /= 1000f;
						break;
					case "4":
						o_fHighLimit /= 10000f;
						break;
					case "5":
						o_fHighLimit /= 100000f;
						break;
					}
					if (text2 == "-")
					{
						o_fHighLimit *= -1f;
					}
					o_fLowLimit = Convert.ToSingle(Convert.ToInt32(value2, 16));
					switch (text5)
					{
					case "1":
						o_fLowLimit /= 10f;
						break;
					case "2":
						o_fLowLimit /= 100f;
						break;
					case "3":
						o_fLowLimit /= 1000f;
						break;
					case "4":
						o_fLowLimit /= 10000f;
						break;
					case "5":
						o_fLowLimit /= 100000f;
						break;
					}
					if (text4 == "-")
					{
						o_fLowLimit *= -1f;
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
		o_fHighLimit = 0f;
		o_fLowLimit = 0f;
		return false;
	}

	public bool SetLimit(int i_iChannel, float i_fHighLimit, float i_fLowLimit)
	{
		string text = "@" + base.Address.ToString("X02") + "A" + i_iChannel.ToString("X");
		float num;
		if (i_fHighLimit >= 0f)
		{
			num = i_fHighLimit;
			text += "+";
		}
		else
		{
			num = i_fHighLimit * -1f;
			text += "-";
		}
		int num2;
		if (num < 1f)
		{
			num2 = Convert.ToInt32(num * 100000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "4";
			}
			else
			{
				text += "5";
			}
		}
		else if (num >= 1f && num < 10f)
		{
			num2 = Convert.ToInt32(num * 10000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "3";
			}
			else
			{
				text += "4";
			}
		}
		else if (num >= 10f && num < 100f)
		{
			num2 = Convert.ToInt32(num * 1000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "2";
			}
			else
			{
				text += "3";
			}
		}
		else if (num >= 100f && num < 1000f)
		{
			num2 = Convert.ToInt32(num * 100f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "1";
			}
			else
			{
				text += "2";
			}
		}
		else if (num >= 1000f && num < 10000f)
		{
			num2 = Convert.ToInt32(num * 10f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "0";
			}
			else
			{
				text += "1";
			}
		}
		else
		{
			num2 = Convert.ToInt32(num);
			if (num2 > 65535)
			{
				num2 = 65535;
			}
			text += "0";
		}
		text += num2.ToString("X04");
		float num3;
		if (i_fLowLimit >= 0f)
		{
			num3 = i_fLowLimit;
			text += "+";
		}
		else
		{
			num3 = i_fLowLimit * -1f;
			text += "-";
		}
		if (num3 < 1f)
		{
			num2 = Convert.ToInt32(num3 * 100000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "4";
			}
			else
			{
				text += "5";
			}
		}
		else if (num3 >= 1f && num3 < 10f)
		{
			num2 = Convert.ToInt32(num3 * 10000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "3";
			}
			else
			{
				text += "4";
			}
		}
		else if (num3 >= 10f && num3 < 100f)
		{
			num2 = Convert.ToInt32(num3 * 1000f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "2";
			}
			else
			{
				text += "3";
			}
		}
		else if (num3 >= 100f && num3 < 1000f)
		{
			num2 = Convert.ToInt32(num3 * 100f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "1";
			}
			else
			{
				text += "2";
			}
		}
		else if (num3 >= 1000f && num3 < 10000f)
		{
			num2 = Convert.ToInt32(num3 * 10f);
			if (num2 > 65535)
			{
				num2 /= 10;
				text += "0";
			}
			else
			{
				text += "1";
			}
		}
		else
		{
			num2 = Convert.ToInt32(num3);
			if (num2 > 65535)
			{
				num2 = 65535;
			}
			text += "0";
		}
		text = text + num2.ToString("X04") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetEnableAlarmDO(out bool[] o_bEnable, out bool[] o_bAlarmDO)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "DI\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 5)
			{
				try
				{
					string value = text.Substring(0, 1);
					byte b = Convert.ToByte(value, 16);
					o_bEnable = new bool[2];
					for (int i = 0; i < 2; i++)
					{
						o_bEnable[i] = (b & (1 << i)) > 0;
					}
					value = text.Substring(1, 2);
					b = Convert.ToByte(value, 16);
					o_bAlarmDO = new bool[2];
					for (int i = 0; i < 2; i++)
					{
						o_bAlarmDO[i] = (b & (1 << i)) > 0;
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
		o_bEnable = null;
		o_bAlarmDO = null;
		return false;
	}

	public bool SetEnable(int i_iChannel, bool i_bEnable)
	{
		string text = ((!i_bEnable) ? ("@" + base.Address.ToString("X02") + "DA") : ("@" + base.Address.ToString("X02") + "EA"));
		text = text + i_iChannel.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetModeAlarmDO(out Adam4000_CounterAlarmMode o_mode, out bool[] o_bAlarmDO)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "DI\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text.Length == 5)
			{
				try
				{
					string text2 = text.Substring(0, 1);
					if (text2 == "1")
					{
						o_mode = Adam4000_CounterAlarmMode.Momentary;
					}
					else if (text2 == "2")
					{
						o_mode = Adam4000_CounterAlarmMode.Latch;
					}
					else
					{
						o_mode = Adam4000_CounterAlarmMode.Disable;
					}
					text2 = text.Substring(1, 2);
					byte b = Convert.ToByte(text2, 16);
					o_bAlarmDO = new bool[2];
					for (int i = 0; i < 2; i++)
					{
						o_bAlarmDO[i] = (b & (1 << i)) > 0;
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
		o_mode = Adam4000_CounterAlarmMode.Disable;
		o_bAlarmDO = null;
		return false;
	}

	public bool SetMode(Adam4000_CounterAlarmMode i_mode)
	{
		string text = "@" + base.Address.ToString("X02");
		string o_szRecv;
		return ASCIISendRecv(i_mode switch
		{
			Adam4000_CounterAlarmMode.Latch => text + "EAL\r", 
			Adam4000_CounterAlarmMode.Momentary => text + "EAM\r", 
			_ => text + "DA\r", 
		}, out o_szRecv);
	}

	public bool GetHighLimit(out long o_lValue)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "RA\r";
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

	public bool SetHighLimit(long i_lValue)
	{
		if (i_lValue < 0)
		{
			i_lValue = 0L;
		}
		else if (i_lValue > uint.MaxValue)
		{
			i_lValue = 4294967295L;
		}
		string i_szSend = "@" + base.Address.ToString("X02") + "SA" + i_lValue.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLowLimit(out long o_lValue)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "RP\r";
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

	public bool SetLowLimit(long i_lValue)
	{
		if (i_lValue < 0)
		{
			i_lValue = 0L;
		}
		else if (i_lValue > uint.MaxValue)
		{
			i_lValue = 4294967295L;
		}
		string i_szSend = "@" + base.Address.ToString("X02") + "PA" + i_lValue.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLimit(int i_iChannel, out long o_lValue)
	{
		string i_szSend = ((i_iChannel != 0) ? ("@" + base.Address.ToString("X02") + "RA\r") : ("@" + base.Address.ToString("X02") + "RP\r"));
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

	public bool SetLimit(int i_iChannel, long i_lValue)
	{
		if (i_lValue < 0)
		{
			i_lValue = 0L;
		}
		else if (i_lValue > uint.MaxValue)
		{
			i_lValue = 4294967295L;
		}
		string text = ((i_iChannel != 0) ? ("@" + base.Address.ToString("X02") + "SA") : ("@" + base.Address.ToString("X02") + "PA"));
		text = text + i_lValue.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool GetHighMode(int i_iSlot, int i_iChannel, out Adam_AIAlarmMode o_mode)
	{
		return GetMode(i_iSlot, i_iChannel, 1, out o_mode);
	}

	public bool GetHighMode(int i_iSlot, int i_iChannel, out Adam5000_CounterAlarmMode o_mode)
	{
		return GetMode(i_iSlot, i_iChannel, 1, out o_mode);
	}

	public bool GetHighMode(int i_iChannel, out Adam_AIAlarmMode o_mode)
	{
		return GetMode(-1, i_iChannel, 1, out o_mode);
	}

	public bool SetHighMode(int i_iSlot, int i_iChannel, Adam_AIAlarmMode i_mode)
	{
		return SetMode(i_iSlot, i_iChannel, 1, i_mode);
	}

	public bool SetHighMode(int i_iSlot, int i_iChannel, Adam5000_CounterAlarmMode i_mode)
	{
		return SetMode(i_iSlot, i_iChannel, 1, i_mode);
	}

	public bool SetHighMode(int i_iChannel, Adam_AIAlarmMode i_mode)
	{
		return SetMode(-1, i_iChannel, 1, i_mode);
	}

	public bool GetLowMode(int i_iSlot, int i_iChannel, out Adam_AIAlarmMode o_mode)
	{
		return GetMode(i_iSlot, i_iChannel, 0, out o_mode);
	}

	public bool GetLowMode(int i_iSlot, int i_iChannel, out Adam5000_CounterAlarmMode o_mode)
	{
		return GetMode(i_iSlot, i_iChannel, 0, out o_mode);
	}

	public bool GetLowMode(int i_iChannel, out Adam_AIAlarmMode o_mode)
	{
		return GetMode(-1, i_iChannel, 0, out o_mode);
	}

	public bool SetLowMode(int i_iSlot, int i_iChannel, Adam_AIAlarmMode i_mode)
	{
		return SetMode(i_iSlot, i_iChannel, 0, i_mode);
	}

	public bool SetLowMode(int i_iSlot, int i_iChannel, Adam5000_CounterAlarmMode i_mode)
	{
		return SetMode(i_iSlot, i_iChannel, 0, i_mode);
	}

	public bool SetLowMode(int i_iChannel, Adam_AIAlarmMode i_mode)
	{
		return SetMode(-1, i_iChannel, 0, i_mode);
	}

	public bool GetHighLimit(int i_iSlot, int i_iChannel, out float o_fLimit)
	{
		return GetLimit(i_iSlot, i_iChannel, 1, out o_fLimit);
	}

	public bool GetHighLimit(int i_iSlot, int i_iChannel, out long o_lLimit)
	{
		return GetLimit(i_iSlot, i_iChannel, 1, out o_lLimit);
	}

	public bool GetHighLimit(int i_iChannel, out float o_fLimit)
	{
		return GetLimit(-1, i_iChannel, 1, out o_fLimit);
	}

	public bool SetHighLimit(int i_iSlot, int i_iChannel, float i_fLimit)
	{
		return SetLimit(i_iSlot, i_iChannel, 1, i_fLimit);
	}

	public bool SetHighLimit(int i_iSlot, int i_iChannel, long i_lLimit)
	{
		return SetLimit(i_iSlot, i_iChannel, 1, i_lLimit);
	}

	public bool SetHighLimit(int i_iChannel, float i_fLimit)
	{
		return SetLimit(-1, i_iChannel, 1, i_fLimit);
	}

	public bool GetLowLimit(int i_iSlot, int i_iChannel, out float o_fLimit)
	{
		return GetLimit(i_iSlot, i_iChannel, 0, out o_fLimit);
	}

	public bool GetLowLimit(int i_iSlot, int i_iChannel, out long o_lLimit)
	{
		return GetLimit(i_iSlot, i_iChannel, 0, out o_lLimit);
	}

	public bool GetLowLimit(int i_iChannel, out float o_fLimit)
	{
		return GetLimit(-1, i_iChannel, 0, out o_fLimit);
	}

	public bool SetLowLimit(int i_iSlot, int i_iChannel, float i_fLimit)
	{
		return SetLimit(i_iSlot, i_iChannel, 0, i_fLimit);
	}

	public bool SetLowLimit(int i_iSlot, int i_iChannel, long i_lLimit)
	{
		return SetLimit(i_iSlot, i_iChannel, 0, i_lLimit);
	}

	public bool SetLowLimit(int i_iChannel, float i_fLimit)
	{
		return SetLimit(-1, i_iChannel, 0, i_fLimit);
	}

	public bool GetStatus(int i_iSlot, int i_iChannel, out bool o_bHigh, out bool o_bLow)
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
		text += "S\r";
		if (ASCIISendRecv(text, out var o_szRecv) && o_szRecv.Length > 4)
		{
			string text2 = o_szRecv.Substring(3, o_szRecv.Length - 4);
			if (text2.Length == 2)
			{
				switch (text2)
				{
				case "11":
					o_bHigh = true;
					o_bLow = true;
					break;
				case "10":
					o_bHigh = true;
					o_bLow = false;
					break;
				case "01":
					o_bHigh = false;
					o_bLow = true;
					break;
				default:
					o_bHigh = false;
					o_bLow = false;
					break;
				}
				return true;
			}
			base.LastError = ErrorCode.Adam_Invalid_Length;
		}
		o_bHigh = false;
		o_bLow = false;
		return false;
	}

	public bool GetStatus(int i_iChannel, out bool o_bHigh, out bool o_bLow)
	{
		return GetStatus(-1, i_iChannel, out o_bHigh, out o_bLow);
	}

	public bool GetHighMapping(int i_iSlot, int i_iChannel, out int o_iSlot, out int o_iChannel)
	{
		return GetMapping(i_iSlot, i_iChannel, 1, out o_iSlot, out o_iChannel);
	}

	public bool GetHighMapping(int i_iChannel, out int o_iChannel)
	{
		int o_iSlot;
		return GetMapping(-1, i_iChannel, 1, out o_iSlot, out o_iChannel);
	}

	public bool SetHighMapping(int i_iSlot, int i_iChannel, int i_iSlotMap, int i_iChMap)
	{
		return SetMapping(i_iSlot, i_iChannel, 1, i_iSlotMap, i_iChMap);
	}

	public bool SetHighMapping(int i_iChannel, int i_iChMap)
	{
		return SetMapping(-1, i_iChannel, 1, -1, i_iChMap);
	}

	public bool GetLowMapping(int i_iSlot, int i_iChannel, out int o_iSlot, out int o_iChannel)
	{
		return GetMapping(i_iSlot, i_iChannel, 0, out o_iSlot, out o_iChannel);
	}

	public bool GetLowMapping(int i_iChannel, out int o_iChannel)
	{
		int o_iSlot;
		return GetMapping(-1, i_iChannel, 0, out o_iSlot, out o_iChannel);
	}

	public bool SetLowMapping(int i_iSlot, int i_iChannel, int i_iSlotMap, int i_iChMap)
	{
		return SetMapping(i_iSlot, i_iChannel, 0, i_iSlotMap, i_iChMap);
	}

	public bool SetLowMapping(int i_iChannel, int i_iChMap)
	{
		return SetMapping(-1, i_iChannel, 0, -1, i_iChMap);
	}

	public bool SetHighLatchClear(int i_iSlot, int i_iChannel)
	{
		return SetLatchClear(i_iSlot, i_iChannel, 1);
	}

	public bool SetHighLatchClear(int i_iChannel)
	{
		return SetLatchClear(-1, i_iChannel, 1);
	}

	public bool SetLowLatchClear(int i_iSlot, int i_iChannel)
	{
		return SetLatchClear(i_iSlot, i_iChannel, 0);
	}

	public bool SetLowLatchClear(int i_iChannel)
	{
		return SetLatchClear(-1, i_iChannel, 0);
	}

	public bool SetLatchClear(int i_iChannel)
	{
		string i_szSend = "@" + base.Address.ToString("X02") + "CAC" + i_iChannel.ToString("X") + "\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	private bool SetAlarmConfiguration(int i_iSlot, int i_iChannel, Adam5000_CounterAlarmMode i_mode, Adam_LocalAlarmType i_type, int i_iSlotMap, int i_iChannelMap, long i_lLimVal)
	{
		string text = "#" + base.Address.ToString("X02") + "QZ" + i_iSlot.ToString("X02") + i_iChannel.ToString("X02");
		switch (i_mode)
		{
		case Adam5000_CounterAlarmMode.Latch:
			text += "L";
			break;
		case Adam5000_CounterAlarmMode.Disable:
			text += "D";
			break;
		default:
			return false;
		}
		switch (i_type)
		{
		case Adam_LocalAlarmType.Low:
			text += "L";
			break;
		case Adam_LocalAlarmType.High:
			text += "H";
			break;
		default:
			return false;
		}
		string text2 = text;
		text = text2 + i_iSlotMap.ToString("X02") + i_iChannelMap.ToString("X02") + i_lLimVal.ToString("X08") + "\r";
		string o_szRecv;
		return ASCIISendRecv(text, out o_szRecv);
	}

	public bool SetLocalAlarmConfiguration(int i_iSlot, int i_iChannel, Adam5000_CounterAlarmMode i_mode, Adam_LocalAlarmType i_type, int i_iChannelMap, long i_lLimVal)
	{
		return SetAlarmConfiguration(i_iSlot, i_iChannel, i_mode, i_type, i_iSlot, i_iChannelMap, i_lLimVal);
	}

	private bool GetAlarmConfiguration(int i_iSlot, int i_iChannel, out Adam5000_CounterAlarmMode o_mode, out Adam_LocalAlarmType o_type, out int o_iSlotMap, out int o_iChannelMap, out long o_lLimVal)
	{
		o_mode = Adam5000_CounterAlarmMode.Unknown;
		o_type = Adam_LocalAlarmType.Unknown;
		o_iSlotMap = -1;
		o_iChannelMap = -1;
		o_lLimVal = -1L;
		string i_szSend = "$" + base.Address.ToString("X02") + "QZ" + i_iSlot.ToString("X02") + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 16)
		{
			if (o_szRecv.Substring(3, 1) == "D")
			{
				o_mode = Adam5000_CounterAlarmMode.Disable;
			}
			else if (o_szRecv.Substring(3, 1) == "L")
			{
				o_mode = Adam5000_CounterAlarmMode.Latch;
			}
			if (o_szRecv.Substring(4, 1) == "L")
			{
				o_type = Adam_LocalAlarmType.Low;
			}
			else if (o_szRecv.Substring(4, 1) == "H")
			{
				o_type = Adam_LocalAlarmType.High;
			}
			try
			{
				string value = o_szRecv.Substring(5, 2);
				o_iSlotMap = Convert.ToUInt16(value, 16);
				value = o_szRecv.Substring(7, 2);
				o_iChannelMap = Convert.ToUInt16(value, 16);
				value = o_szRecv.Substring(9, 8);
				o_lLimVal = Convert.ToUInt32(value, 16);
				return true;
			}
			catch
			{
				return false;
			}
		}
		return false;
	}

	public bool GetLocalAlarmConfiguration(int i_iSlot, int i_iChannel, out Adam5000_CounterAlarmMode o_mode, out Adam_LocalAlarmType o_type, out int o_iChannelMap, out long o_lLimVal)
	{
		o_mode = Adam5000_CounterAlarmMode.Unknown;
		o_type = Adam_LocalAlarmType.Unknown;
		int o_iSlotMap = -1;
		o_iChannelMap = -1;
		o_lLimVal = -1L;
		return GetAlarmConfiguration(i_iSlot, i_iChannel, out o_mode, out o_type, out o_iSlotMap, out o_iChannelMap, out o_lLimVal);
	}

	public bool SetLocalAlarmLatchClear(int i_iSlot, int i_iChannel)
	{
		string i_szSend = "#" + base.Address.ToString("X02") + "QD" + i_iSlot.ToString("X02") + i_iChannel.ToString("X02") + "0\r";
		string o_szRecv;
		return ASCIISendRecv(i_szSend, out o_szRecv);
	}

	public bool GetLocalAlarmStatus(int i_iSlot, int i_iChannel, out bool o_bStatus)
	{
		o_bStatus = false;
		string i_szSend = "$" + base.Address.ToString("X02") + "QD" + i_iSlot.ToString("X02") + i_iChannel.ToString("X02") + "\r";
		if (ASCIISendRecv(i_szSend, out var o_szRecv) && o_szRecv.Length > 3)
		{
			if (o_szRecv.Substring(3, 1) == "1")
			{
				o_bStatus = true;
			}
			else
			{
				o_bStatus = false;
			}
			return true;
		}
		return false;
	}
}
