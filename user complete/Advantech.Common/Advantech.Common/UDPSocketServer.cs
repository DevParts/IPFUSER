using System.Net;
using System.Net.Sockets;

namespace Advantech.Common;

public class UDPSocketServer
{
	private Socket m_socket;

	private int m_iSendTimeout = 2000;

	private int m_iRecvTimeout = 2000;

	public int SendTimeout
	{
		get
		{
			return m_iSendTimeout;
		}
		set
		{
			m_iSendTimeout = value;
		}
	}

	public int ReceiveTimeout
	{
		get
		{
			return m_iRecvTimeout;
		}
		set
		{
			m_iRecvTimeout = value;
		}
	}

	public void Create(int i_iPort)
	{
		IPEndPoint localEP = new IPEndPoint(IPAddress.Any, i_iPort);
		m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
		m_socket.Bind(localEP);
	}

	public void Terminate()
	{
		if (m_socket != null)
		{
			m_socket.Close();
			m_socket = null;
		}
	}

	public Socket ServerSocket()
	{
		return m_socket;
	}
}
