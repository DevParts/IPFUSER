using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Advantech.Common;

public class SocketStream
{
	private Socket m_socket;

	private int m_iSendTimeout = 2000;

	private int m_iRecvTimeout = 2000;

	public SocketStream(ref Socket i_socket, int i_iSendTimeout, int i_iRecvTimeout)
	{
		m_socket = i_socket;
		m_iSendTimeout = i_iSendTimeout;
		m_iRecvTimeout = i_iRecvTimeout;
	}

	public bool SendUDP(ref EndPoint i_remoteEP, byte[] i_byData, int i_iLen)
	{
		try
		{
			if (m_socket != null && m_socket.Poll(m_iSendTimeout * 1000, SelectMode.SelectWrite))
			{
				int num = m_socket.SendTo(i_byData, i_iLen, SocketFlags.None, i_remoteEP);
				if (num == i_iLen)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	public bool RecvUDP(out EndPoint o_remoteEP, byte[] i_byData, out int o_iLen)
	{
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
		o_remoteEP = iPEndPoint;
		try
		{
			if (m_socket != null && m_socket.Poll(m_iRecvTimeout * 1000, SelectMode.SelectRead))
			{
				Thread.Sleep(10);
				o_iLen = m_socket.ReceiveFrom(i_byData, SocketFlags.None, ref o_remoteEP);
				return true;
			}
		}
		catch
		{
		}
		o_iLen = 0;
		return false;
	}

	public bool DataArrive(int i_iWaitMilliSecond)
	{
		int num = 0;
		do
		{
			if (m_socket.Poll(1000, SelectMode.SelectRead))
			{
				return true;
			}
			Thread.Sleep(1);
			num++;
		}
		while (num * 2 < i_iWaitMilliSecond);
		return false;
	}
}
