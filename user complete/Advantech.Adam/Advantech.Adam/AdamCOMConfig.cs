using Advantech.Common;

namespace Advantech.Adam;

public class AdamCOMConfig
{
	private byte m_byStatus;

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

	public Parity Parity
	{
		get
		{
			return (byte)(m_byStatus & 0xE0) switch
			{
				32 => Parity.Even, 
				64 => Parity.Odd, 
				_ => Parity.None, 
			};
		}
		set
		{
			m_byStatus &= 31;
			switch (value)
			{
			case Parity.Even:
				m_byStatus |= 32;
				break;
			case Parity.Odd:
				m_byStatus |= 64;
				break;
			}
		}
	}

	public Databits Databits
	{
		get
		{
			return (byte)(m_byStatus & 0x1C) switch
			{
				4 => Databits.Six, 
				8 => Databits.Seven, 
				12 => Databits.Eight, 
				_ => Databits.Five, 
			};
		}
		set
		{
			m_byStatus &= 227;
			switch (value)
			{
			case Databits.Six:
				m_byStatus |= 4;
				break;
			case Databits.Seven:
				m_byStatus |= 8;
				break;
			case Databits.Eight:
				m_byStatus |= 12;
				break;
			}
		}
	}

	public Stopbits Stopbits
	{
		get
		{
			byte b = (byte)(m_byStatus & 3);
			if (b == 2)
			{
				return Stopbits.Two;
			}
			return Stopbits.One;
		}
		set
		{
			m_byStatus &= 252;
			if (value == Stopbits.Two)
			{
				m_byStatus |= 2;
			}
		}
	}
}
