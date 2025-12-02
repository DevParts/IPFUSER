using System;

namespace Advantech.Adam;

public class Adam4000Config
{
	private byte m_byAddr;

	private byte m_byType;

	private byte m_byBaud;

	private byte m_byStatus;

	public byte Address
	{
		get
		{
			return m_byAddr;
		}
		set
		{
			m_byAddr = value;
		}
	}

	public byte TypeCode
	{
		get
		{
			return m_byType;
		}
		set
		{
			m_byType = value;
		}
	}

	public byte Baudrate
	{
		get
		{
			return m_byBaud;
		}
		set
		{
			m_byBaud = value;
		}
	}

	public byte Status
	{
		get
		{
			return m_byStatus;
		}
		set
		{
			m_byStatus = value;
		}
	}

	public Adam4000_DataFormat Format
	{
		get
		{
			return (byte)(m_byStatus & 3) switch
			{
				1 => Adam4000_DataFormat.Percent, 
				2 => Adam4000_DataFormat.TwosComplementHex, 
				3 => Adam4000_DataFormat.Ohms, 
				_ => Adam4000_DataFormat.EngineerUnit, 
			};
		}
		set
		{
			m_byStatus &= 252;
			switch (value)
			{
			case Adam4000_DataFormat.Percent:
				m_byStatus |= 1;
				break;
			case Adam4000_DataFormat.TwosComplementHex:
				m_byStatus |= 2;
				break;
			case Adam4000_DataFormat.Ohms:
				m_byStatus |= 3;
				break;
			}
		}
	}

	public Adam4000_FreqGateTime FreqGateTime
	{
		get
		{
			if ((byte)(m_byStatus & 4) == 0)
			{
				return Adam4000_FreqGateTime.OneTenthSecond;
			}
			return Adam4000_FreqGateTime.OneSecond;
		}
		set
		{
			m_byStatus &= 251;
			if (value == Adam4000_FreqGateTime.OneSecond)
			{
				m_byStatus |= 4;
			}
		}
	}

	public Adam_Protocol Protocol
	{
		get
		{
			return (byte)(m_byStatus & 0x14) switch
			{
				0 => Adam_Protocol.Advantech, 
				4 => Adam_Protocol.Modbus, 
				_ => Adam_Protocol.BACnet, 
			};
		}
		set
		{
			m_byStatus &= 235;
			switch (value)
			{
			case Adam_Protocol.Modbus:
				m_byStatus |= 4;
				break;
			case Adam_Protocol.BACnet:
				m_byStatus |= 16;
				break;
			}
		}
	}

	public Adam_Protocol Protocol4024
	{
		get
		{
			if ((byte)(m_byStatus & 0x80) == 0)
			{
				return Adam_Protocol.Advantech;
			}
			return Adam_Protocol.Modbus;
		}
		set
		{
			m_byStatus &= 127;
			if (value == Adam_Protocol.Modbus)
			{
				m_byStatus |= 128;
			}
		}
	}

	public Adam4100_HighSpeed HighSpeed4100
	{
		get
		{
			if ((byte)(m_byStatus & 0x20) == 0)
			{
				return Adam4100_HighSpeed.Disable;
			}
			return Adam4100_HighSpeed.Enable;
		}
		set
		{
			m_byStatus &= 223;
			if (value == Adam4100_HighSpeed.Enable)
			{
				m_byStatus |= 32;
			}
		}
	}

	public Adam_Checksum Checksum
	{
		get
		{
			if ((byte)(m_byStatus & 0x40) == 0)
			{
				return Adam_Checksum.Disable;
			}
			return Adam_Checksum.Enable;
		}
		set
		{
			m_byStatus &= 191;
			if (value == Adam_Checksum.Enable)
			{
				m_byStatus |= 64;
			}
		}
	}

	public Adam4000_Integration Integration
	{
		get
		{
			if ((byte)(m_byStatus & 0x80) == 0)
			{
				return Adam4000_Integration.Hz60_50ms;
			}
			return Adam4000_Integration.Hz50_60ms;
		}
		set
		{
			m_byStatus &= 127;
			if (value == Adam4000_Integration.Hz50_60ms)
			{
				m_byStatus |= 128;
			}
		}
	}

	public Adam4100_Integration Integration4100
	{
		get
		{
			if ((byte)(m_byStatus & 0x80) == 0)
			{
				return Adam4100_Integration.Hz50_60;
			}
			return Adam4100_Integration.UserDefine;
		}
		set
		{
			m_byStatus &= 127;
			if (value == Adam4100_Integration.UserDefine)
			{
				m_byStatus |= 128;
			}
		}
	}

	public Adam_Temperature Temperature
	{
		get
		{
			if ((byte)(m_byStatus & 8) == 0)
			{
				return Adam_Temperature.Centigrade;
			}
			return Adam_Temperature.Fahrenheit;
		}
		set
		{
			m_byStatus &= 247;
			if (value == Adam_Temperature.Fahrenheit)
			{
				m_byStatus |= 8;
			}
		}
	}

	public byte Slewrate
	{
		get
		{
			return (byte)((m_byStatus & 0x3C) >> 2);
		}
		set
		{
			m_byStatus &= 195;
			m_byStatus = Convert.ToByte(m_byStatus + (byte)(value << 2));
		}
	}
}
