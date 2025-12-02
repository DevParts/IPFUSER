namespace Advantech.Adam;

public class AdamWirelessAP
{
	private string m_szSSID;

	private string m_szMAC;

	private byte m_byChannel;

	private byte m_byMode;

	private bool m_bWepUsed;

	private byte m_byStrength;

	public string SSID
	{
		get
		{
			return m_szSSID;
		}
		set
		{
			m_szSSID = value;
		}
	}

	public string MAC
	{
		get
		{
			return m_szMAC;
		}
		set
		{
			m_szMAC = value;
		}
	}

	public byte Channel
	{
		get
		{
			return m_byChannel;
		}
		set
		{
			m_byChannel = value;
		}
	}

	public byte Mode
	{
		get
		{
			return m_byMode;
		}
		set
		{
			m_byMode = value;
		}
	}

	public bool WepUsed
	{
		get
		{
			return m_bWepUsed;
		}
		set
		{
			m_bWepUsed = value;
		}
	}

	public byte Strength
	{
		get
		{
			return m_byStrength;
		}
		set
		{
			m_byStrength = value;
		}
	}
}
