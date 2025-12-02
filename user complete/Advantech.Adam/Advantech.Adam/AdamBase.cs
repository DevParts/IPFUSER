using System.Globalization;
using System.Net.Sockets;
using Advantech.Common;

namespace Advantech.Adam;

public abstract class AdamBase
{
	private AdamCom m_com;

	private int m_iAddr;

	private AdamSocket m_socket;

	private ErrorCode m_error;

	private AdamType m_adamType;

	protected NumberFormatInfo m_numberFormatInfo;

	public int Address
	{
		get
		{
			return m_iAddr;
		}
		set
		{
			m_iAddr = value;
		}
	}

	public ErrorCode LastError
	{
		get
		{
			return m_error;
		}
		set
		{
			m_error = value;
		}
	}

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

	public AdamBase(AdamCom i_com)
	{
		m_numberFormatInfo = new CultureInfo("en-US", useUserOverride: false).NumberFormat;
		m_com = i_com;
		m_iAddr = 0;
		m_error = ErrorCode.No_Error;
		m_adamType = AdamType.Adam4000;
	}

	public AdamBase(AdamSocket i_socket)
	{
		m_numberFormatInfo = new CultureInfo("en-US", useUserOverride: false).NumberFormat;
		m_socket = i_socket;
		m_iAddr = 1;
		m_error = ErrorCode.No_Error;
		m_adamType = AdamType.Adam6000;
	}

	protected bool ASCIISendRecv(string i_szSend, out string o_szRecv)
	{
		o_szRecv = "";
		m_error = ErrorCode.No_Error;
		if (m_com != null)
		{
			bool result = m_com.AdamTransaction(i_szSend, out o_szRecv);
			m_error = m_com.LastError;
			return result;
		}
		if (m_socket != null)
		{
			bool result = m_socket.AdamTransaction(i_szSend, out o_szRecv);
			m_error = m_socket.LastError;
			return result;
		}
		m_error = ErrorCode.Adam_Null_Error;
		return false;
	}

	protected ProtocolType GetProtocolType()
	{
		return m_socket.GetProtocolType();
	}
}
