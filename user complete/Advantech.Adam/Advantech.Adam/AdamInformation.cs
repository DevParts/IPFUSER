using System;
using System.Text;

namespace Advantech.Adam;

public class AdamInformation
{
	private byte[] m_byDeviceID;

	private byte[] m_byIP;

	private byte[] m_byMac;

	private byte[] m_byNewMac;

	private byte[] m_bySubnet;

	private byte[] m_byGateway;

	private string m_szPassword;

	private string m_szDeviceName;

	private string m_szDescription;

	private bool m_bHostIdle;

	private int m_iHostIdleTime;

	private int m_byDatabits;

	private int m_byParity;

	private int m_byStopbits;

	private int m_byFlowControl;

	private int m_iBaudrate;

	private int m_iFrameTimeout;

	private byte m_byHardwareType;

	private byte[] m_byHeaderReserved;

	private long m_lWirelessRegionFlag;

	private long m_lWirelessRegion;

	private long m_lWirelessModeFlag;

	private byte m_byWirelessMode;

	private byte m_byWirelessChannel;

	private int m_iBasicRate;

	private byte m_byInitSSIDLength;

	private string m_szInitSSID;

	private byte m_bySSIDLength;

	private string m_szSSID;

	private byte m_byWEP;

	private byte m_byWEPKeyIndex;

	private byte[] m_byWEPKey64;

	private byte[] m_byWEPKey64_2;

	private byte[] m_byWEPKey64_3;

	private byte[] m_byWEPKey64_4;

	private byte[] m_byWEPKey128;

	public byte[] DeviceID
	{
		get
		{
			return m_byDeviceID;
		}
		set
		{
			if (value.Length == 2)
			{
				Array.Copy(value, 0, m_byDeviceID, 0, 2);
			}
		}
	}

	public byte[] IP
	{
		get
		{
			return m_byIP;
		}
		set
		{
			if (value.Length == 4)
			{
				Array.Copy(value, 0, m_byIP, 0, 4);
			}
		}
	}

	public byte[] Mac
	{
		get
		{
			return m_byMac;
		}
		set
		{
			if (value.Length == 6)
			{
				Array.Copy(value, 0, m_byMac, 0, 6);
			}
		}
	}

	public byte[] NewMac
	{
		get
		{
			return m_byNewMac;
		}
		set
		{
			if (value.Length == 6)
			{
				Array.Copy(value, 0, m_byNewMac, 0, 6);
			}
		}
	}

	public byte[] Subnet
	{
		get
		{
			return m_bySubnet;
		}
		set
		{
			if (value.Length == 4)
			{
				Array.Copy(value, 0, m_bySubnet, 0, 4);
			}
		}
	}

	public byte[] Gateway
	{
		get
		{
			return m_byGateway;
		}
		set
		{
			if (value.Length == 4)
			{
				Array.Copy(value, 0, m_byGateway, 0, 4);
			}
		}
	}

	public string Password
	{
		get
		{
			return m_szPassword;
		}
		set
		{
			m_szPassword = value;
		}
	}

	public string DeviceName
	{
		get
		{
			return m_szDeviceName;
		}
		set
		{
			m_szDeviceName = value;
		}
	}

	public string Description
	{
		get
		{
			return m_szDescription;
		}
		set
		{
			m_szDescription = value;
		}
	}

	public bool HostIdle
	{
		get
		{
			return m_bHostIdle;
		}
		set
		{
			m_bHostIdle = value;
		}
	}

	public int HostIdleTime
	{
		get
		{
			return m_iHostIdleTime;
		}
		set
		{
			m_iHostIdleTime = value;
		}
	}

	public int Databits
	{
		get
		{
			return m_byDatabits;
		}
		set
		{
			m_byDatabits = value;
		}
	}

	public int Parity
	{
		get
		{
			return m_byParity;
		}
		set
		{
			m_byParity = value;
		}
	}

	public int Stopbits
	{
		get
		{
			return m_byStopbits;
		}
		set
		{
			m_byStopbits = value;
		}
	}

	public int FlowControl
	{
		get
		{
			return m_byFlowControl;
		}
		set
		{
			m_byFlowControl = value;
		}
	}

	public int Baudrate
	{
		get
		{
			return m_iBaudrate;
		}
		set
		{
			m_iBaudrate = value;
		}
	}

	public int FrameTimeout
	{
		get
		{
			return m_iFrameTimeout;
		}
		set
		{
			m_iFrameTimeout = value;
		}
	}

	public byte HardwareType
	{
		get
		{
			return m_byHardwareType;
		}
		set
		{
			m_byHardwareType = value;
		}
	}

	public byte[] HeaderReserved
	{
		get
		{
			return m_byHeaderReserved;
		}
		set
		{
			if (value.Length == 2)
			{
				Array.Copy(value, 0, m_byHeaderReserved, 0, 2);
			}
		}
	}

	public long WirelessRegionFlag
	{
		get
		{
			return m_lWirelessRegionFlag;
		}
		set
		{
			m_lWirelessRegionFlag = value;
		}
	}

	public long WirelessRegion
	{
		get
		{
			return m_lWirelessRegion;
		}
		set
		{
			m_lWirelessRegion = value;
		}
	}

	public long WirelessModeFlag
	{
		get
		{
			return m_lWirelessModeFlag;
		}
		set
		{
			m_lWirelessModeFlag = value;
		}
	}

	public byte WirelessMode
	{
		get
		{
			return m_byWirelessMode;
		}
		set
		{
			m_byWirelessMode = value;
		}
	}

	public byte WirelessChannel
	{
		get
		{
			return m_byWirelessChannel;
		}
		set
		{
			m_byWirelessChannel = value;
		}
	}

	public int BasicRate
	{
		get
		{
			return m_iBasicRate;
		}
		set
		{
			m_iBasicRate = value;
		}
	}

	public byte InitSSIDLength
	{
		get
		{
			return m_byInitSSIDLength;
		}
		set
		{
			m_byInitSSIDLength = value;
		}
	}

	public string InitSSID
	{
		get
		{
			return m_szInitSSID;
		}
		set
		{
			m_szInitSSID = value;
		}
	}

	public byte SSIDLength
	{
		get
		{
			return m_bySSIDLength;
		}
		set
		{
			m_bySSIDLength = value;
		}
	}

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

	public byte WEP
	{
		get
		{
			return m_byWEP;
		}
		set
		{
			m_byWEP = value;
		}
	}

	public byte WEPKeyIndex
	{
		get
		{
			return m_byWEPKeyIndex;
		}
		set
		{
			m_byWEPKeyIndex = value;
		}
	}

	public byte[] WEPKey64
	{
		get
		{
			return m_byWEPKey64;
		}
		set
		{
			if (value.Length == 8)
			{
				Array.Copy(value, 0, m_byWEPKey64, 0, 8);
			}
			else if (value.Length < 8)
			{
				byte[] array = new byte[8];
				Array.Copy(value, 0, array, 0, value.Length);
				Array.Copy(array, 0, m_byWEPKey64, 0, 8);
			}
		}
	}

	public byte[] WEPKey64_2
	{
		get
		{
			return m_byWEPKey64_2;
		}
		set
		{
			if (value.Length == 8)
			{
				Array.Copy(value, 0, m_byWEPKey64_2, 0, 8);
			}
			else if (value.Length < 8)
			{
				byte[] array = new byte[8];
				Array.Copy(value, 0, array, 0, value.Length);
				Array.Copy(array, 0, m_byWEPKey64_2, 0, 8);
			}
		}
	}

	public byte[] WEPKey64_3
	{
		get
		{
			return m_byWEPKey64_3;
		}
		set
		{
			if (value.Length == 8)
			{
				Array.Copy(value, 0, m_byWEPKey64_3, 0, 8);
			}
			else if (value.Length < 8)
			{
				byte[] array = new byte[8];
				Array.Copy(value, 0, array, 0, value.Length);
				Array.Copy(array, 0, m_byWEPKey64_3, 0, 8);
			}
		}
	}

	public byte[] WEPKey64_4
	{
		get
		{
			return m_byWEPKey64_4;
		}
		set
		{
			if (value.Length == 8)
			{
				Array.Copy(value, 0, m_byWEPKey64_4, 0, 8);
			}
			else if (value.Length < 8)
			{
				byte[] array = new byte[8];
				Array.Copy(value, 0, array, 0, value.Length);
				Array.Copy(array, 0, m_byWEPKey64_4, 0, 8);
			}
		}
	}

	public byte[] WEPKey128
	{
		get
		{
			return m_byWEPKey128;
		}
		set
		{
			if (value.Length == 16)
			{
				Array.Copy(value, 0, m_byWEPKey128, 0, 16);
			}
		}
	}

	public AdamInformation()
	{
		m_byDeviceID = new byte[2];
		m_byIP = new byte[4];
		m_byMac = new byte[6];
		m_byNewMac = new byte[6];
		m_bySubnet = new byte[4];
		m_byGateway = new byte[4];
		m_byHeaderReserved = new byte[2];
		m_byWEPKey64 = new byte[8];
		m_byWEPKey64_2 = new byte[8];
		m_byWEPKey64_3 = new byte[8];
		m_byWEPKey64_4 = new byte[8];
		m_byWEPKey128 = new byte[16];
	}

	public void SetDeviceID(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byDeviceID, 0, 2);
	}

	public void SetIP(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byIP, 0, 4);
	}

	public void SetMac(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex + 1, m_byMac, 0, 3);
		Array.Copy(i_byBuffer, i_iIndex + 5, m_byMac, 3, 3);
	}

	public void SetSubnet(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_bySubnet, 0, 4);
	}

	public void SetGateway(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byGateway, 0, 4);
	}

	public void SetPassword(byte[] i_byBuffer, int i_iIndex)
	{
		byte[] array = new byte[10];
		byte b = 63;
		for (int i = 0; i < 8; i++)
		{
			array[i] = Convert.ToByte(i_byBuffer[i_iIndex + i] ^ b);
		}
		try
		{
			m_szPassword = Encoding.ASCII.GetString(array, 0, 8);
		}
		catch
		{
			m_szPassword = "00000000";
		}
	}

	public void SetDeviceName(byte[] i_byBuffer, int i_iIndex, int i_iLen)
	{
		try
		{
			int i;
			for (i = 0; i < i_iLen && i_byBuffer[i_iIndex + i] != 0; i++)
			{
			}
			if (i > 0)
			{
				byte[] array = new byte[i];
				Array.Copy(i_byBuffer, i_iIndex, array, 0, i);
				m_szDeviceName = Encoding.ASCII.GetString(array, 0, i);
			}
			else
			{
				m_szDeviceName = "";
			}
		}
		catch
		{
			m_szDeviceName = "";
		}
	}

	public void SetDescription(byte[] i_byBuffer, int i_iIndex, int i_iLen)
	{
		try
		{
			int i;
			for (i = 0; i < i_iLen && i_byBuffer[i_iIndex + i] != 0; i++)
			{
			}
			if (i > 0)
			{
				byte[] array = new byte[i];
				Array.Copy(i_byBuffer, i_iIndex, array, 0, i);
				m_szDescription = Encoding.ASCII.GetString(array, 0, i);
			}
			else
			{
				m_szDescription = "";
			}
		}
		catch
		{
			m_szDescription = "";
		}
	}

	public void SetHostIdle(byte[] i_byBuffer, int i_iIndex)
	{
		if ((i_byBuffer[i_iIndex] & 0xA0) > 0)
		{
			m_bHostIdle = true;
		}
		else
		{
			m_bHostIdle = false;
		}
	}

	public void SetHostIdleTime(byte[] i_byBuffer, int i_iIndex)
	{
		m_iHostIdleTime = (i_byBuffer[i_iIndex] & 0xF) * 256 + i_byBuffer[i_iIndex + 1];
	}

	public void SetDatabits(byte[] i_byBuffer, int i_iIndex)
	{
		m_byDatabits = Convert.ToInt32(i_byBuffer[i_iIndex + 1]);
	}

	public void SetParity(byte[] i_byBuffer, int i_iIndex)
	{
		m_byParity = Convert.ToInt32(i_byBuffer[i_iIndex + 1]);
	}

	public void SetStopbits(byte[] i_byBuffer, int i_iIndex)
	{
		m_byStopbits = Convert.ToInt32(i_byBuffer[i_iIndex + 1]);
	}

	public void SetFlowControl(byte[] i_byBuffer, int i_iIndex)
	{
		m_byFlowControl = Convert.ToInt32(i_byBuffer[i_iIndex + 1]);
	}

	public void SetBaudrate(byte[] i_byBuffer, int i_iIndex)
	{
		m_iBaudrate = BitConverter.ToInt32(new byte[4]
		{
			i_byBuffer[i_iIndex + 3],
			i_byBuffer[i_iIndex + 2],
			i_byBuffer[i_iIndex + 1],
			i_byBuffer[i_iIndex]
		}, 0);
	}

	public void SetFrameTimeout(byte[] i_byBuffer, int i_iIndex)
	{
		m_iFrameTimeout = BitConverter.ToInt32(new byte[4]
		{
			i_byBuffer[i_iIndex + 3],
			i_byBuffer[i_iIndex + 2],
			i_byBuffer[i_iIndex + 1],
			i_byBuffer[i_iIndex]
		}, 0);
	}

	public void SetHardwareType(byte[] i_byBuffer, int i_iIndex)
	{
		m_byHardwareType = i_byBuffer[i_iIndex];
	}

	public void SetHeaderReserved(byte[] i_byBuffer, int i_iIndex)
	{
		m_byHeaderReserved[0] = i_byBuffer[i_iIndex];
		m_byHeaderReserved[1] = i_byBuffer[i_iIndex + 1];
	}

	public void SetWirelessRegionFlag(byte[] i_byBuffer, int i_iIndex)
	{
		m_lWirelessRegionFlag = BitConverter.ToInt32(new byte[4]
		{
			i_byBuffer[i_iIndex + 3],
			i_byBuffer[i_iIndex + 2],
			i_byBuffer[i_iIndex + 1],
			i_byBuffer[i_iIndex]
		}, 0);
	}

	public void SetWirelessRegion(byte[] i_byBuffer, int i_iIndex)
	{
		m_lWirelessRegion = BitConverter.ToInt32(new byte[4]
		{
			i_byBuffer[i_iIndex + 3],
			i_byBuffer[i_iIndex + 2],
			i_byBuffer[i_iIndex + 1],
			i_byBuffer[i_iIndex]
		}, 0);
	}

	public void SetWirelessModeFlag(byte[] i_byBuffer, int i_iIndex)
	{
		m_lWirelessModeFlag = BitConverter.ToInt32(new byte[4]
		{
			i_byBuffer[i_iIndex + 3],
			i_byBuffer[i_iIndex + 2],
			i_byBuffer[i_iIndex + 1],
			i_byBuffer[i_iIndex]
		}, 0);
	}

	public void SetWirelessMode(byte[] i_byBuffer, int i_iIndex)
	{
		m_byWirelessMode = i_byBuffer[i_iIndex];
	}

	public void SetWirelessChannel(byte[] i_byBuffer, int i_iIndex)
	{
		m_byWirelessChannel = i_byBuffer[i_iIndex];
	}

	public void SetBasicRate(byte[] i_byBuffer, int i_iIndex)
	{
		m_iBasicRate = i_byBuffer[i_iIndex] * 256 + i_byBuffer[i_iIndex + 1];
	}

	public void SetInitSSIDLength(byte[] i_byBuffer, int i_iIndex)
	{
		m_byInitSSIDLength = i_byBuffer[i_iIndex];
	}

	public void SetInitSSID(byte[] i_byBuffer, int i_iIndex, int i_iLen)
	{
		try
		{
			int i;
			for (i = 0; i < i_iLen && i_byBuffer[i_iIndex + i] != 0; i++)
			{
			}
			if (i > 0)
			{
				byte[] array = new byte[i];
				Array.Copy(i_byBuffer, i_iIndex, array, 0, i);
				m_szInitSSID = Encoding.ASCII.GetString(array, 0, i);
			}
			else
			{
				m_szInitSSID = "";
			}
		}
		catch
		{
			m_szInitSSID = "";
		}
	}

	public void SetSSIDLength(byte[] i_byBuffer, int i_iIndex)
	{
		m_bySSIDLength = i_byBuffer[i_iIndex];
	}

	public void SetSSID(byte[] i_byBuffer, int i_iIndex, int i_iLen)
	{
		try
		{
			int i;
			for (i = 0; i < i_iLen && i_byBuffer[i_iIndex + i] != 0; i++)
			{
			}
			if (i > 0)
			{
				byte[] array = new byte[i];
				Array.Copy(i_byBuffer, i_iIndex, array, 0, i);
				m_szSSID = Encoding.ASCII.GetString(array, 0, i);
			}
			else
			{
				m_szSSID = "";
			}
		}
		catch
		{
			m_szSSID = "";
		}
	}

	public void SetWEP(byte[] i_byBuffer, int i_iIndex)
	{
		m_byWEP = i_byBuffer[i_iIndex];
	}

	public void SetWEPKeyIndex(byte[] i_byBuffer, int i_iIndex)
	{
		m_byWEPKeyIndex = i_byBuffer[i_iIndex];
	}

	public void SetWEPKey64(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byWEPKey64, 0, 8);
	}

	public void SetWEPKey64_2(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byWEPKey64_2, 0, 8);
	}

	public void SetWEPKey64_3(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byWEPKey64_3, 0, 8);
	}

	public void SetWEPKey64_4(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byWEPKey64_4, 0, 8);
	}

	public void SetWEPKey128(byte[] i_byBuffer, int i_iIndex)
	{
		Array.Copy(i_byBuffer, i_iIndex, m_byWEPKey128, 0, 8);
	}

	public void CopyTo(ref AdamInformation adamObj)
	{
		adamObj.DeviceID = m_byDeviceID;
		adamObj.IP = m_byIP;
		adamObj.Mac = m_byMac;
		adamObj.NewMac = m_byNewMac;
		adamObj.Subnet = m_bySubnet;
		adamObj.Gateway = m_byGateway;
		adamObj.Password = m_szPassword;
		adamObj.DeviceName = m_szDeviceName;
		adamObj.Description = m_szDescription;
		adamObj.HostIdle = m_bHostIdle;
		adamObj.HostIdleTime = m_iHostIdleTime;
		adamObj.Databits = m_byDatabits;
		adamObj.Parity = m_byParity;
		adamObj.Stopbits = m_byStopbits;
		adamObj.FlowControl = m_byFlowControl;
		adamObj.Baudrate = m_iBaudrate;
		adamObj.FrameTimeout = m_iFrameTimeout;
		adamObj.HardwareType = m_byHardwareType;
		adamObj.HeaderReserved = m_byHeaderReserved;
		adamObj.WirelessRegionFlag = m_lWirelessRegionFlag;
		adamObj.WirelessRegion = m_lWirelessRegion;
		adamObj.WirelessModeFlag = m_lWirelessModeFlag;
		adamObj.WirelessMode = m_byWirelessMode;
		adamObj.WirelessChannel = m_byWirelessChannel;
		adamObj.BasicRate = m_iBasicRate;
		adamObj.InitSSIDLength = m_byInitSSIDLength;
		adamObj.InitSSID = m_szInitSSID;
		adamObj.SSIDLength = m_bySSIDLength;
		adamObj.SSID = m_szSSID;
		adamObj.WEP = m_byWEP;
		adamObj.WEPKeyIndex = m_byWEPKeyIndex;
		adamObj.WEPKey64 = m_byWEPKey64;
		adamObj.WEPKey64_2 = m_byWEPKey64_2;
		adamObj.WEPKey64_3 = m_byWEPKey64_3;
		adamObj.WEPKey64_4 = m_byWEPKey64_4;
		adamObj.WEPKey128 = m_byWEPKey128;
	}
}
