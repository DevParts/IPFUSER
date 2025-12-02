using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Advantech.Common;

namespace Advantech.Adam;

public class AdamSocket
{
	internal const int ADAM_MAX_MSGLEN = 1200;

	internal const int ADAMTCP_BUFSIZE = 1200;

	internal const int ADAMTCP_PORT = 502;

	internal const int ADAMCONFIG_PORT = 5048;

	internal const int ADAM5KUDP_PORT = 6168;

	internal const int ADAM6KUDP_PORT = 1025;

	internal const int ADAM5K_TOTALREG = 40;

	internal const int ADAM6K_TOTALREG = 126;

	private AdamType m_adamType;

	private int m_sendTimeout = 2000;

	private int m_recvTimeout = 2000;

	private int m_connectTimeout = 2000;

	private int m_port;

	private IPAddress m_ipAddr;

	private Socket m_socket;

	private Configuration m_Config;

	private DigitalInput m_DI;

	private DigitalOutput m_DO;

	private AnalogInput m_AI;

	private AnalogOutput m_AO;

	private Modbus m_Modbus;

	private Alarm m_Alarm;

	private Counter m_Counter;

	private PID m_Pid;

	private static bool m_b5000Found;

	private static bool m_bWirelessFound;

	private static byte m_byMsgID;

	private ErrorCode m_error;

	public ErrorCode LastError => m_error;

	public AdamType AdamSeriesType
	{
		get
		{
			return m_adamType;
		}
		set
		{
			m_adamType = value;
		}
	}

	public bool Connected
	{
		get
		{
			if (m_socket == null)
			{
				return false;
			}
			return m_socket.Connected;
		}
	}

	public static bool Adam5000Found => m_b5000Found;

	public static bool WirelessFound => m_bWirelessFound;

	public AdamSocket()
	{
		m_adamType = AdamType.Adam6000;
		if (m_Modbus == null)
		{
			m_Modbus = new Modbus(this);
		}
		m_Modbus.AdamSeriesType = AdamType.Adam6000;
	}

	public ProtocolType GetProtocolType()
	{
		if (m_socket != null)
		{
			return m_socket.ProtocolType;
		}
		return ProtocolType.IP;
	}

	public string GetIP()
	{
		return m_ipAddr.ToString();
	}

	public Configuration Configuration()
	{
		if (m_Config == null)
		{
			m_Config = new Configuration(this);
		}
		m_Config.AdamSeriesType = m_adamType;
		return m_Config;
	}

	public Configuration Configuration(int i_iAddr)
	{
		if (m_Config == null)
		{
			m_Config = new Configuration(this);
		}
		m_Config.Address = i_iAddr;
		m_Config.AdamSeriesType = m_adamType;
		return m_Config;
	}

	public DigitalInput DigitalInput()
	{
		if (m_DI == null)
		{
			m_DI = new DigitalInput(this);
		}
		m_DI.AdamSeriesType = m_adamType;
		return m_DI;
	}

	public DigitalInput DigitalInput(int i_iAddr)
	{
		if (m_DI == null)
		{
			m_DI = new DigitalInput(this);
		}
		m_DI.Address = i_iAddr;
		m_DI.AdamSeriesType = m_adamType;
		return m_DI;
	}

	public DigitalOutput DigitalOutput()
	{
		if (m_DO == null)
		{
			m_DO = new DigitalOutput(this);
		}
		m_DO.AdamSeriesType = m_adamType;
		return m_DO;
	}

	public DigitalOutput DigitalOutput(int i_iAddr)
	{
		if (m_DO == null)
		{
			m_DO = new DigitalOutput(this);
		}
		m_DO.Address = i_iAddr;
		m_DO.AdamSeriesType = m_adamType;
		return m_DO;
	}

	public AnalogInput AnalogInput()
	{
		if (m_AI == null)
		{
			m_AI = new AnalogInput(this);
		}
		m_AI.AdamSeriesType = m_adamType;
		return m_AI;
	}

	public AnalogInput AnalogInput(int i_iAddr)
	{
		if (m_AI == null)
		{
			m_AI = new AnalogInput(this);
		}
		m_AI.Address = i_iAddr;
		m_AI.AdamSeriesType = m_adamType;
		return m_AI;
	}

	public AnalogOutput AnalogOutput()
	{
		if (m_AO == null)
		{
			m_AO = new AnalogOutput(this);
		}
		m_AO.AdamSeriesType = m_adamType;
		return m_AO;
	}

	public AnalogOutput AnalogOutput(int i_iAddr)
	{
		if (m_AO == null)
		{
			m_AO = new AnalogOutput(this);
		}
		m_AO.Address = i_iAddr;
		m_AO.AdamSeriesType = m_adamType;
		return m_AO;
	}

	public Modbus Modbus()
	{
		if (m_Modbus == null)
		{
			m_Modbus = new Modbus(this);
		}
		m_Modbus.AdamSeriesType = m_adamType;
		return m_Modbus;
	}

	public Modbus Modbus(int i_iAddr)
	{
		if (m_Modbus == null)
		{
			m_Modbus = new Modbus(this);
		}
		m_Modbus.Address = i_iAddr;
		m_Modbus.AdamSeriesType = m_adamType;
		return m_Modbus;
	}

	public Alarm Alarm()
	{
		if (m_Alarm == null)
		{
			m_Alarm = new Alarm(this);
		}
		m_Alarm.AdamSeriesType = m_adamType;
		return m_Alarm;
	}

	public Alarm Alarm(int i_iAddr)
	{
		if (m_Alarm == null)
		{
			m_Alarm = new Alarm(this);
		}
		m_Alarm.Address = i_iAddr;
		m_Alarm.AdamSeriesType = m_adamType;
		return m_Alarm;
	}

	public Counter Counter()
	{
		if (m_Counter == null)
		{
			m_Counter = new Counter(this);
		}
		m_Counter.AdamSeriesType = m_adamType;
		return m_Counter;
	}

	public Counter Counter(int i_iAddr)
	{
		if (m_Counter == null)
		{
			m_Counter = new Counter(this);
		}
		m_Counter.Address = i_iAddr;
		m_Counter.AdamSeriesType = m_adamType;
		return m_Counter;
	}

	public PID Pid()
	{
		if (m_Pid == null)
		{
			m_Pid = new PID(this);
		}
		m_Pid.AdamSeriesType = m_adamType;
		return m_Pid;
	}

	public PID Pid(int i_iAddr)
	{
		if (m_Pid == null)
		{
			m_Pid = new PID(this);
		}
		m_Pid.Address = i_iAddr;
		m_Pid.AdamSeriesType = m_adamType;
		return m_Pid;
	}

	public void SetTimeout(int i_iConnectTimeout, int i_iSendTimeout, int i_iRecvTimeout)
	{
		m_connectTimeout = i_iConnectTimeout;
		m_sendTimeout = i_iSendTimeout;
		m_recvTimeout = i_iRecvTimeout;
	}

	private void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			if (m_socket == null || socket.Handle != m_socket.Handle)
			{
				socket.Close();
			}
		}
		catch
		{
		}
	}

	public bool Connect(AdamType i_type, string i_szIPAddr, ProtocolType protocolType)
	{
		int num = 0;
		m_error = ErrorCode.No_Error;
		try
		{
			m_adamType = i_type;
			m_ipAddr = IPAddress.Parse(i_szIPAddr);
			if (protocolType == ProtocolType.Udp)
			{
				m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				if (m_adamType == AdamType.Adam5000Tcp)
				{
					m_port = 6168;
				}
				else
				{
					m_port = 1025;
				}
				return true;
			}
			m_port = 502;
			IPEndPoint iPEndPoint = new IPEndPoint(m_ipAddr, m_port);
			m_socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			m_socket.BeginConnect(iPEndPoint, ConnectCallback, m_socket);
			while (num * 50 < m_connectTimeout)
			{
				Thread.Sleep(50);
				num++;
				if (m_socket.Connected)
				{
					return true;
				}
			}
			m_error = ErrorCode.Socket_Connect_Fail;
			m_socket = null;
			return false;
		}
		catch
		{
			m_error = ErrorCode.Socket_Invalid_IP;
			m_socket = null;
			return false;
		}
	}

	public bool Connect(string i_szIPAddr, ProtocolType protocolType, int i_iPort)
	{
		int num = 0;
		m_error = ErrorCode.No_Error;
		try
		{
			m_ipAddr = IPAddress.Parse(i_szIPAddr);
			m_port = i_iPort;
			if (protocolType == ProtocolType.Udp)
			{
				m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				return true;
			}
			IPEndPoint iPEndPoint = new IPEndPoint(m_ipAddr, m_port);
			m_socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			m_socket.BeginConnect(iPEndPoint, ConnectCallback, m_socket);
			while (num * 50 < m_connectTimeout)
			{
				Thread.Sleep(50);
				num++;
				if (m_socket.Connected)
				{
					return true;
				}
			}
			m_error = ErrorCode.Socket_Connect_Fail;
			m_socket = null;
			return false;
		}
		catch
		{
			m_error = ErrorCode.Socket_Invalid_IP;
			m_socket = null;
			return false;
		}
	}

	public void Disconnect()
	{
		if (m_socket != null)
		{
			m_socket.Shutdown(SocketShutdown.Both);
			m_socket.Close();
			m_socket = null;
		}
	}

	public bool Send(byte[] i_byData, int i_iLen)
	{
		m_error = ErrorCode.No_Error;
		try
		{
			if (m_socket != null)
			{
				if (m_socket.ProtocolType == ProtocolType.Tcp)
				{
					if (m_socket.Poll(m_sendTimeout * 1000, SelectMode.SelectWrite))
					{
						int num = m_socket.Send(i_byData, i_iLen, SocketFlags.None);
						if (num == i_iLen)
						{
							return true;
						}
					}
					m_error = ErrorCode.Socket_Send_Fail;
					return false;
				}
				IPEndPoint remoteEP = new IPEndPoint(m_ipAddr, m_port);
				if (m_socket.Poll(m_sendTimeout * 1000, SelectMode.SelectWrite))
				{
					int num = m_socket.SendTo(i_byData, i_iLen, SocketFlags.None, remoteEP);
					if (num == i_iLen)
					{
						return true;
					}
				}
				m_error = ErrorCode.Socket_Send_Fail;
				return false;
			}
			m_error = ErrorCode.Socket_Null;
			return false;
		}
		catch
		{
			m_error = ErrorCode.Socket_Unknown;
			return false;
		}
	}

	public bool Receive(byte[] i_byData, out int o_iLen)
	{
		m_error = ErrorCode.No_Error;
		o_iLen = 0;
		if (i_byData == null)
		{
			m_error = ErrorCode.Adam_Null_Error;
			return false;
		}
		try
		{
			if (m_socket != null)
			{
				if (m_socket.ProtocolType == ProtocolType.Tcp)
				{
					if (m_socket.Poll(m_recvTimeout * 1000, SelectMode.SelectRead) && m_socket.Connected)
					{
						Thread.Sleep(10);
						int size = ((i_byData.Length >= m_socket.Available) ? m_socket.Available : i_byData.Length);
						o_iLen = m_socket.Receive(i_byData, size, SocketFlags.None);
						return true;
					}
					m_error = ErrorCode.Socket_Recv_Fail;
					return false;
				}
				IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
				EndPoint remoteEP = iPEndPoint;
				if (m_socket.Poll(m_recvTimeout * 1000, SelectMode.SelectRead))
				{
					Thread.Sleep(10);
					o_iLen = m_socket.ReceiveFrom(i_byData, SocketFlags.None, ref remoteEP);
					return true;
				}
				m_error = ErrorCode.Socket_Recv_Fail;
				return false;
			}
			m_error = ErrorCode.Socket_Null;
			return false;
		}
		catch
		{
			m_error = ErrorCode.Socket_Unknown;
			return false;
		}
	}

	public bool AdamTransaction(string i_szSend, out string o_szRecv)
	{
		if (m_socket.ProtocolType == ProtocolType.Tcp)
		{
			return AdamTCPTransaction(i_szSend, out o_szRecv);
		}
		return AdamUDPTransaction(i_szSend, out o_szRecv);
	}

	protected bool AdamUDPTransaction(string i_szSend, out string o_szRecv)
	{
		byte[] array = new byte[1200];
		m_error = ErrorCode.No_Error;
		o_szRecv = "";
		byte[] bytes = Encoding.ASCII.GetBytes(i_szSend);
		int i_iLen = bytes.Length;
		if (Send(bytes, i_iLen))
		{
			if (Receive(array, out var o_iLen) && o_iLen > 0)
			{
				o_szRecv = Encoding.ASCII.GetString(array, 0, o_iLen);
				if (array[o_iLen - 1] != 13)
				{
					o_szRecv += "\r";
				}
				if (o_szRecv.Substring(0, 1) == "!" || o_szRecv.Substring(0, 1) == ">")
				{
					return true;
				}
				m_error = ErrorCode.Adam_Invalid_Head;
			}
			Thread.Sleep(100);
		}
		return false;
	}

	protected bool AdamTCPTransaction(string i_szSend, out string o_szRecv)
	{
		m_error = ErrorCode.No_Error;
		o_szRecv = "";
		int i_iTotalPoint = ((m_adamType != AdamType.Adam5000Tcp) ? 126 : 40);
		int num = i_szSend.Length;
		if (num % 2 == 1)
		{
			num++;
		}
		byte[] array = new byte[num];
		byte[] bytes = Encoding.ASCII.GetBytes(i_szSend);
		Array.Copy(bytes, 0, array, 0, bytes.Length);
		int i_iTotalReg = num / 2;
		if (m_Modbus.PresetMultiRegs(10000, i_iTotalReg, num, array))
		{
			int num2 = 0;
			while (true)
			{
				if (m_Modbus.ReadHoldingRegs(10000, i_iTotalPoint, out byte[] o_byteData))
				{
					int num3 = 0;
					while (num3 < o_byteData.Length && o_byteData[num3++] != 13)
					{
					}
					o_szRecv = Encoding.ASCII.GetString(o_byteData, 0, num3);
					if (o_szRecv.Substring(0, 1) == "!" || o_szRecv.Substring(0, 1) == ">")
					{
						return true;
					}
					m_error = ErrorCode.Adam_Invalid_Head;
					return false;
				}
				m_error = m_Modbus.LastError;
				if (++num2 >= 3)
				{
					break;
				}
				Thread.Sleep(100);
			}
			return false;
		}
		m_error = m_Modbus.LastError;
		return false;
	}

	public static bool GetLocalNetwork(out string[] o_szLocalIP)
	{
		try
		{
			string hostName = Dns.GetHostName();
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			IPAddress[] addressList = hostEntry.AddressList;
			o_szLocalIP = new string[addressList.Length];
			for (int i = 0; i < addressList.Length; i++)
			{
				o_szLocalIP[i] = addressList[i].ToString();
			}
			return true;
		}
		catch
		{
			o_szLocalIP = null;
			return false;
		}
	}

	protected static bool GetAdamInformation(AdamInfoType i_infoType, int i_iTimeout, ref ArrayList adamList)
	{
		byte[] array = new byte[1200];
		byte[] array2 = new byte[60];
		array2[0] = 77;
		array2[1] = 65;
		array2[2] = 68;
		array2[3] = 65;
		array2[4] = 0;
		array2[5] = 0;
		array2[6] = 0;
		array2[7] = 131;
		array2[8] = 1;
		array2[9] = 0;
		array2[10] = 80;
		array2[11] = 0;
		int num;
		switch (i_infoType)
		{
		case AdamInfoType.AdamDeviceInfo:
			array2[53] = 0;
			num = 280;
			break;
		case AdamInfoType.AdamNetConfig:
			array2[53] = 16;
			num = 92;
			break;
		case AdamInfoType.AdamPortConfig:
			array2[53] = 32;
			num = 104;
			break;
		case AdamInfoType.AdamDeviceConfig:
			array2[53] = 48;
			num = 96;
			break;
		case AdamInfoType.AdamWirelessConfig:
			array2[53] = 64;
			num = 1012;
			break;
		default:
			array2[53] = 160;
			num = 280;
			break;
		}
		if (i_infoType == AdamInfoType.AdamNetConfig)
		{
			m_b5000Found = false;
			m_bWirelessFound = false;
		}
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		IPAddress address = IPAddress.Parse("255.255.255.255");
		IPEndPoint remoteEP = new IPEndPoint(address, 5048);
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint remoteEP2 = iPEndPoint;
		IComparer comparer = new AdamMacComparer();
		bool flag = false;
		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 262144);
		try
		{
			int num2 = 0;
			while (num2 * 50 < i_iTimeout)
			{
				if (socket.Poll(50000, SelectMode.SelectWrite))
				{
					int num3 = socket.SendTo(array2, SocketFlags.None, remoteEP);
					if (num3 == 60)
					{
						flag = true;
						break;
					}
				}
				else
				{
					num2++;
				}
			}
			if (flag)
			{
				flag = false;
				num2 = 0;
				while (num2 * 50 < i_iTimeout)
				{
					if (socket.Poll(50000, SelectMode.SelectRead) && socket.Available >= num)
					{
						int num4 = socket.ReceiveFrom(array, SocketFlags.None, ref remoteEP2);
						if (i_infoType == AdamInfoType.AdamNetConfig && array[52] == 128 && array[53] == 16 && num4 >= 92)
						{
							AdamInformation adamInformation = new AdamInformation();
							adamInformation.SetDeviceID(array, 10);
							adamInformation.SetMac(array, 12);
							adamInformation.SetPassword(array, 20);
							adamInformation.SetHostIdle(array, 74);
							adamInformation.SetHostIdleTime(array, 74);
							adamInformation.SetIP(array, 80);
							adamInformation.SetSubnet(array, 84);
							adamInformation.SetGateway(array, 88);
							int i;
							for (i = 0; i < adamList.Count; i++)
							{
								AdamInformation adamInformation2 = (AdamInformation)adamList[i];
								if (adamInformation.Mac[0] == adamInformation2.Mac[0] && adamInformation.Mac[1] == adamInformation2.Mac[1] && adamInformation.Mac[2] == adamInformation2.Mac[2] && adamInformation.Mac[3] == adamInformation2.Mac[3] && adamInformation.Mac[4] == adamInformation2.Mac[4] && adamInformation.Mac[5] == adamInformation2.Mac[5])
								{
									break;
								}
							}
							if (i == adamList.Count)
							{
								adamList.Add(adamInformation);
								if (adamInformation.DeviceID[0] == 80)
								{
									m_b5000Found = true;
								}
							}
							else
							{
								adamInformation = null;
							}
						}
						else if (i_infoType == AdamInfoType.AdamDeviceInfo && array[52] == 128 && array[53] == 0 && num4 >= 88)
						{
							AdamInformation adamInformation = new AdamInformation();
							adamInformation.SetMac(array, 12);
							int i = adamList.BinarySearch(0, adamList.Count, adamInformation, comparer);
							if (i >= 0)
							{
								AdamInformation adamInformation2 = (AdamInformation)adamList[i];
								adamInformation2.SetHardwareType(array, 61);
								if (adamInformation2.HardwareType == 2)
								{
									m_bWirelessFound = true;
								}
								int num5 = Convert.ToInt32(array[75]);
								int i_iLen = Convert.ToInt32(array[83]);
								adamInformation2.SetDeviceName(array, 88, num5);
								adamInformation2.SetDescription(array, 88 + num5, i_iLen);
							}
							adamInformation = null;
						}
						else if (i_infoType == AdamInfoType.AdamPortConfig && array[52] == 128 && array[53] == 32 && num4 >= 104)
						{
							AdamInformation adamInformation = new AdamInformation();
							adamInformation.SetMac(array, 12);
							int i = adamList.BinarySearch(0, adamList.Count, adamInformation, comparer);
							if (i >= 0)
							{
								AdamInformation adamInformation2 = (AdamInformation)adamList[i];
								adamInformation2.SetDatabits(array, 72);
								adamInformation2.SetParity(array, 74);
								adamInformation2.SetStopbits(array, 76);
								adamInformation2.SetFlowControl(array, 78);
								adamInformation2.SetBaudrate(array, 92);
							}
							adamInformation = null;
						}
						else if (i_infoType == AdamInfoType.AdamDeviceConfig && array[52] == 128 && array[53] == 48 && num4 >= 96)
						{
							AdamInformation adamInformation = new AdamInformation();
							adamInformation.SetMac(array, 12);
							int i = adamList.BinarySearch(0, adamList.Count, adamInformation, comparer);
							if (i >= 0)
							{
								AdamInformation adamInformation2 = (AdamInformation)adamList[i];
								adamInformation2.SetFrameTimeout(array, 60);
							}
							adamInformation = null;
						}
						else if (i_infoType == AdamInfoType.AdamWirelessConfig && array[52] == 128 && array[53] == 64 && num4 >= 420)
						{
							AdamInformation adamInformation = new AdamInformation();
							adamInformation.SetMac(array, 12);
							int i = adamList.BinarySearch(0, adamList.Count, adamInformation, comparer);
							if (i >= 0)
							{
								AdamInformation adamInformation2 = (AdamInformation)adamList[i];
								adamInformation2.SetHeaderReserved(array, 62);
								adamInformation2.SetInitSSIDLength(array, 115);
								int i_iLen2 = Convert.ToInt32(array[115]);
								adamInformation2.SetInitSSID(array, 116, i_iLen2);
								adamInformation2.SetWirelessRegionFlag(array, 148);
								adamInformation2.SetWirelessRegion(array, 152);
								adamInformation2.SetWirelessModeFlag(array, 156);
								adamInformation2.SetWirelessMode(array, 163);
								adamInformation2.SetWirelessChannel(array, 171);
								adamInformation2.SetBasicRate(array, 174);
								adamInformation2.SetSSIDLength(array, 259);
								i_iLen2 = Convert.ToInt32(array[259]);
								adamInformation2.SetSSID(array, 260, i_iLen2);
								adamInformation2.SetWEP(array, 367);
								adamInformation2.SetWEPKeyIndex(array, 368);
								adamInformation2.SetWEPKey64(array, 372);
								adamInformation2.SetWEPKey64_2(array, 380);
								adamInformation2.SetWEPKey64_3(array, 388);
								adamInformation2.SetWEPKey64_4(array, 396);
							}
							adamInformation = null;
						}
						else if (i_infoType == AdamInfoType.AdvantechDevice && array[52] == 128 && array[53] == 160)
						{
							AdamInformation adamInformation = new AdamInformation();
							IPEndPoint iPEndPoint2 = (IPEndPoint)remoteEP2;
							byte[] addressBytes = iPEndPoint2.Address.GetAddressBytes();
							adamInformation.SetIP(addressBytes, 0);
							adamInformation.SetHardwareType(array, 61);
							int num5 = Convert.ToInt32(array[75]);
							int i_iLen = Convert.ToInt32(array[83]);
							adamInformation.SetDeviceName(array, 88, num5);
							adamInformation.SetDescription(array, 88 + num5, i_iLen);
							adamList.Add(adamInformation);
						}
						flag = true;
					}
					else
					{
						num2++;
					}
				}
				if (flag && i_infoType == AdamInfoType.AdamNetConfig)
				{
					adamList.Sort(comparer);
				}
			}
		}
		catch
		{
		}
		comparer = null;
		iPEndPoint = null;
		remoteEP = null;
		socket = null;
		array2 = null;
		return flag;
	}

	public static bool GetAdamDeviceList(int i_iTimeout, out ArrayList o_lstAdam)
	{
		o_lstAdam = new ArrayList();
		bool adamInformation = GetAdamInformation(AdamInfoType.AdamNetConfig, i_iTimeout, ref o_lstAdam);
		if (adamInformation)
		{
			Thread.Sleep(200);
			adamInformation = GetAdamInformation(AdamInfoType.AdamDeviceInfo, i_iTimeout, ref o_lstAdam);
		}
		if (adamInformation)
		{
			Thread.Sleep(200);
			adamInformation = GetAdamInformation(AdamInfoType.AdamPortConfig, i_iTimeout, ref o_lstAdam);
		}
		if (adamInformation && m_b5000Found)
		{
			Thread.Sleep(200);
			GetAdamInformation(AdamInfoType.AdamDeviceConfig, i_iTimeout, ref o_lstAdam);
		}
		if (adamInformation && m_bWirelessFound)
		{
			Thread.Sleep(200);
			GetAdamInformation(AdamInfoType.AdamWirelessConfig, i_iTimeout, ref o_lstAdam);
		}
		return adamInformation;
	}

	public static bool GetAdvantechDeviceList(int i_iTimeout, out ArrayList o_lstAdam)
	{
		o_lstAdam = new ArrayList();
		return GetAdamInformation(AdamInfoType.AdvantechDevice, i_iTimeout, ref o_lstAdam);
	}

	public static bool SetAdamInformation(AdamInfoType i_infoType, int i_iTimeout, AdamInformation i_adamObject)
	{
		byte[] array = new byte[1200];
		if (m_byMsgID == byte.MaxValue)
		{
			m_byMsgID = 0;
		}
		else
		{
			m_byMsgID++;
		}
		int num = i_infoType switch
		{
			AdamInfoType.AdamDeviceInfo => 272, 
			AdamInfoType.AdamNetConfig => 84, 
			AdamInfoType.AdamPortConfig => 88, 
			AdamInfoType.AdamDeviceConfig => 100, 
			AdamInfoType.AdamWirelessConfig => 1012, 
			_ => 60, 
		};
		byte[] array2 = new byte[num];
		array2[0] = 77;
		array2[1] = 65;
		array2[2] = 68;
		array2[3] = 65;
		array2[4] = 0;
		array2[5] = 0;
		array2[6] = 0;
		array2[7] = m_byMsgID;
		array2[8] = 1;
		array2[9] = 0;
		array2[10] = 80;
		array2[11] = 0;
		byte[] mac = i_adamObject.Mac;
		array2[12] = 0;
		array2[13] = mac[0];
		array2[14] = mac[1];
		array2[15] = mac[2];
		array2[16] = 0;
		array2[17] = mac[3];
		array2[18] = mac[4];
		array2[19] = mac[5];
		switch (i_infoType)
		{
		case AdamInfoType.AdamDeviceInfo:
		{
			array2[52] = 64;
			array2[53] = 0;
			array2[54] = 0;
			array2[55] = 216;
			array2[59] = 64;
			array2[63] = 0;
			array2[67] = 128;
			array2[71] = 64;
			mac = Encoding.ASCII.GetBytes(i_adamObject.DeviceName);
			int num2 = mac.Length;
			if (num2 > 64)
			{
				num2 = 64;
			}
			Array.Copy(mac, 0, array2, 80, num2);
			mac = Encoding.ASCII.GetBytes(i_adamObject.Description);
			num2 = mac.Length;
			if (num2 > 128)
			{
				num2 = 128;
			}
			Array.Copy(mac, 0, array2, 144, num2);
			break;
		}
		case AdamInfoType.AdamNetConfig:
		{
			array2[52] = 64;
			array2[53] = 16;
			array2[54] = 0;
			array2[55] = 28;
			array2[56] = 0;
			array2[57] = 0;
			array2[58] = 0;
			array2[59] = 0;
			mac = i_adamObject.NewMac;
			if (mac[0] == 0 && mac[1] == 0 && mac[2] == 0 && mac[3] == 0 && mac[4] == 0 && mac[5] == 0)
			{
				mac = i_adamObject.Mac;
			}
			array2[60] = mac[0];
			array2[61] = mac[1];
			array2[62] = mac[2];
			array2[63] = mac[3];
			array2[64] = mac[4];
			array2[65] = mac[5];
			if (i_adamObject.HostIdle)
			{
				array2[66] = 160;
			}
			int num3 = i_adamObject.HostIdleTime;
			if (num3 > 4095)
			{
				num3 = 4095;
			}
			else if (num3 < 0)
			{
				num3 = 0;
			}
			array2[66] += Convert.ToByte(num3 / 256);
			array2[67] = Convert.ToByte(num3 % 256);
			mac = i_adamObject.IP;
			array2[72] = mac[0];
			array2[73] = mac[1];
			array2[74] = mac[2];
			array2[75] = mac[3];
			mac = i_adamObject.Subnet;
			array2[76] = mac[0];
			array2[77] = mac[1];
			array2[78] = mac[2];
			array2[79] = mac[3];
			mac = i_adamObject.Gateway;
			array2[80] = mac[0];
			array2[81] = mac[1];
			array2[82] = mac[2];
			array2[83] = mac[3];
			break;
		}
		case AdamInfoType.AdamPortConfig:
			array2[52] = 64;
			array2[53] = 32;
			array2[54] = 0;
			array2[55] = 32;
			array2[60] = 1;
			array2[61] = 229;
			array2[63] = 1;
			if (i_adamObject.Databits == 4)
			{
				array2[65] = 4;
			}
			else if (i_adamObject.Databits == 5)
			{
				array2[65] = 5;
			}
			else if (i_adamObject.Databits == 6)
			{
				array2[65] = 6;
			}
			else if (i_adamObject.Databits == 7)
			{
				array2[65] = 7;
			}
			else
			{
				array2[65] = 8;
			}
			if (i_adamObject.Parity == 1)
			{
				array2[67] = 1;
			}
			else if (i_adamObject.Parity == 2)
			{
				array2[67] = 2;
			}
			else if (i_adamObject.Parity == 3)
			{
				array2[67] = 3;
			}
			else if (i_adamObject.Parity == 4)
			{
				array2[67] = 4;
			}
			else
			{
				array2[67] = 0;
			}
			if (i_adamObject.Stopbits == 1)
			{
				array2[69] = 1;
			}
			else if (i_adamObject.Stopbits == 2)
			{
				array2[69] = 2;
			}
			else
			{
				array2[69] = 0;
			}
			if (i_adamObject.FlowControl == 1)
			{
				array2[71] = 1;
			}
			else if (i_adamObject.FlowControl == 2)
			{
				array2[71] = 2;
			}
			else if (i_adamObject.FlowControl == 3)
			{
				array2[71] = 3;
			}
			else
			{
				array2[71] = 0;
			}
			mac = BitConverter.GetBytes(i_adamObject.Baudrate);
			array2[72] = mac[3];
			array2[73] = mac[2];
			array2[74] = mac[1];
			array2[75] = mac[0];
			break;
		case AdamInfoType.AdamDeviceConfig:
			array2[52] = 64;
			array2[53] = 48;
			array2[54] = 0;
			array2[55] = 44;
			array2[56] = 1;
			array2[57] = 246;
			array2[58] = 3;
			array2[59] = 232;
			mac = BitConverter.GetBytes(i_adamObject.FrameTimeout);
			array2[60] = mac[3];
			array2[61] = mac[2];
			array2[62] = mac[1];
			array2[63] = mac[0];
			break;
		case AdamInfoType.AdamWirelessConfig:
		{
			array2[52] = 64;
			array2[53] = 64;
			array2[54] = 3;
			array2[55] = 187;
			mac = i_adamObject.HeaderReserved;
			array2[62] = mac[0];
			array2[63] = mac[1];
			array2[163] = i_adamObject.WirelessMode;
			mac = BitConverter.GetBytes(i_adamObject.BasicRate);
			array2[174] = mac[1];
			array2[175] = mac[0];
			array2[259] = i_adamObject.SSIDLength;
			mac = Encoding.ASCII.GetBytes(i_adamObject.SSID);
			int num2 = mac.Length;
			if (num2 > 32)
			{
				num2 = 32;
			}
			Array.Copy(mac, 0, array2, 260, num2);
			array2[367] = i_adamObject.WEP;
			array2[368] = i_adamObject.WEPKeyIndex;
			mac = i_adamObject.WEPKey64;
			Array.Copy(mac, 0, array2, 372, 8);
			mac = i_adamObject.WEPKey64_2;
			Array.Copy(mac, 0, array2, 380, 8);
			mac = i_adamObject.WEPKey64_3;
			Array.Copy(mac, 0, array2, 388, 8);
			mac = i_adamObject.WEPKey64_4;
			Array.Copy(mac, 0, array2, 396, 8);
			mac = i_adamObject.WEPKey128;
			Array.Copy(mac, 0, array2, 404, 16);
			break;
		}
		default:
			array2[4] = Convert.ToByte(array2[0] ^ array2[12] ^ array2[16]);
			array2[5] = Convert.ToByte(array2[1] ^ array2[13] ^ array2[17]);
			array2[6] = Convert.ToByte(array2[2] ^ array2[14] ^ array2[18]);
			array2[7] = Convert.ToByte(array2[3] ^ array2[15] ^ array2[19]);
			array2[52] = 64;
			array2[53] = 240;
			array2[54] = 0;
			array2[55] = 0;
			break;
		}
		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		IPAddress address = IPAddress.Parse("255.255.255.255");
		IPEndPoint remoteEP = new IPEndPoint(address, 5048);
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint remoteEP2 = iPEndPoint;
		bool flag = false;
		socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
		try
		{
			if (socket.Poll(i_iTimeout * 1000, SelectMode.SelectWrite))
			{
				int num2 = socket.SendTo(array2, SocketFlags.None, remoteEP);
				if (num2 == num)
				{
					flag = true;
				}
			}
			if (flag)
			{
				flag = false;
				if (socket.Poll(i_iTimeout * 1000, SelectMode.SelectRead))
				{
					Thread.Sleep(10);
					int num4 = socket.ReceiveFrom(array, SocketFlags.None, ref remoteEP2);
					if (num4 == 60 && array[13] == array2[13] && array[14] == array2[14] && array[15] == array2[15] && array[17] == array2[17] && array[18] == array2[18] && array[19] == array2[19])
					{
						flag = ((array[56] == 0 && array[57] == 0 && array[58] == 0 && array[59] == 0) ? true : false);
					}
					Thread.Sleep(20);
				}
			}
		}
		catch
		{
		}
		iPEndPoint = null;
		remoteEP = null;
		socket = null;
		array2 = null;
		return flag;
	}
}
