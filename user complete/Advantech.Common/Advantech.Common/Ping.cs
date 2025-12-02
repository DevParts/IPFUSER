using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Advantech.Common;

public class Ping
{
	private const string KernelDll = "kernel32.dll";

	private const int LMEM_ZEROINIT = 64;

	private ICMP_ECHO_REPLY m_reply;

	private IntPtr m_handle;

	private int m_iTimeout;

	private int m_iLastError;

	[DllImport("kernel32.dll")]
	private static extern IntPtr LocalAlloc(int flags, int size);

	[DllImport("kernel32.dll")]
	private static extern IntPtr LocalFree(IntPtr pMem);

	[DllImport("iphlpapi")]
	private static extern IntPtr IcmpCreateFile();

	[DllImport("iphlpapi")]
	private static extern bool IcmpCloseHandle(IntPtr h);

	[DllImport("iphlpapi")]
	private static extern uint IcmpSendEcho(IntPtr IcmpHandle, uint DestinationAddress, byte[] RequestData, short RequestSize, IntPtr RequestOptions, byte[] ReplyBuffer, int ReplySize, int Timeout);

	[DllImport("kernel32.dll")]
	private static extern int GetLastError();

	public Ping()
	{
		m_handle = IntPtr.Zero;
		m_reply = null;
		m_iTimeout = 1000;
		m_iLastError = 0;
	}

	public void Initialize(int i_iTimeout)
	{
		m_iTimeout = i_iTimeout;
		m_reply = new ICMP_ECHO_REPLY(255);
		m_reply.DataSize = 255;
		IntPtr data = LocalAlloc(64, m_reply.DataSize);
		m_reply.Data = data;
		m_handle = IcmpCreateFile();
	}

	public void PingIP(string i_szIPAddr, out string o_szMessage)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(new string('\0', 32));
		IPAddress iPAddress = IPAddress.Parse(i_szIPAddr);
		byte[] addressBytes = iPAddress.GetAddressBytes();
		uint destinationAddress = BitConverter.ToUInt32(addressBytes, 0);
		if (IcmpSendEcho(m_handle, destinationAddress, bytes, (short)bytes.Length, IntPtr.Zero, m_reply._Data, m_reply._Data.Length, m_iTimeout) == 0)
		{
			m_iLastError = GetLastError();
		}
		else
		{
			m_iLastError = 0;
		}
		if (m_iLastError == 11010)
		{
			o_szMessage = "Request timed out.";
			return;
		}
		o_szMessage = $"Reply from {i_szIPAddr}: bytes={m_reply.DataSize}, time={m_reply.RoundTripTime}ms, TTL={m_reply.Ttl}";
	}

	public void Terminate()
	{
		if (m_handle != IntPtr.Zero)
		{
			IcmpCloseHandle(m_handle);
		}
		if (m_reply != null)
		{
			LocalFree(m_reply.Data);
			m_reply = null;
		}
	}

	public int LastError()
	{
		return m_iLastError;
	}
}
