namespace Advantech.Adam;

public class Adam5000Config
{
	private byte m_byBaud;

	private byte m_byStatus;

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

	public Adam_Protocol Protocol
	{
		get
		{
			if ((byte)(m_byStatus & 4) == 0)
			{
				return Adam_Protocol.Advantech;
			}
			return Adam_Protocol.Modbus;
		}
		set
		{
			m_byStatus &= 251;
			if (value == Adam_Protocol.Modbus)
			{
				m_byStatus |= 4;
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
}
