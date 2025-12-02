using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public class Protocol
{
	private Endpoint m_endpoint;

	private HttpProtocol m_httpProtocol;

	private TcpProtocol m_tcpProtocol;

	public HttpProtocol Http
	{
		get
		{
			if (m_httpProtocol == null)
			{
				m_httpProtocol = new HttpProtocol(m_endpoint, null, GetStateForType(ProtocolType.Http));
			}
			return m_httpProtocol;
		}
	}

	public TcpProtocol Tcp
	{
		get
		{
			if (m_tcpProtocol == null)
			{
				m_tcpProtocol = new TcpProtocol(m_endpoint, null, GetStateForType(ProtocolType.Tcp));
			}
			return m_tcpProtocol;
		}
	}

	internal EndpointProtocol EndpointProtocol => (ProtocolType)m_endpoint.GetPropValue("ProtocolType") switch
	{
		ProtocolType.Http => Http, 
		ProtocolType.Tcp => Tcp, 
		ProtocolType.NamedPipes => null, 
		ProtocolType.SharedMemory => null, 
		ProtocolType.Via => null, 
		_ => throw new WrongPropertyValueException(m_endpoint.Properties.Get("ProtocolType")), 
	};

	private string ProtocolDdlName => (ProtocolType)m_endpoint.GetPropValue("ProtocolType") switch
	{
		ProtocolType.Http => "HTTP", 
		ProtocolType.Tcp => "TCP", 
		ProtocolType.NamedPipes => "NAMEDPIPES", 
		ProtocolType.SharedMemory => "SHAREDMEMORY", 
		ProtocolType.Via => "Via", 
		_ => throw new WrongPropertyValueException(m_endpoint.Properties.Get("ProtocolType")), 
	};

	internal Protocol(Endpoint endpoint)
	{
		m_endpoint = endpoint;
	}

	private SqlSmoState GetStateForType(ProtocolType et)
	{
		object propValueOptional = m_endpoint.GetPropValueOptional("ProtocolType");
		Property property = m_endpoint.Properties.Get("ProtocolType");
		propValueOptional = (property.Dirty ? m_endpoint.oldEndpointTypeValue : property.Value);
		if (propValueOptional == null)
		{
			return SqlSmoState.Creating;
		}
		if ((ProtocolType)propValueOptional == et)
		{
			return m_endpoint.State;
		}
		return SqlSmoState.Creating;
	}

	internal void MarkDropped()
	{
		if (Http != null)
		{
			Http.MarkDroppedInternal();
		}
		if (Tcp != null)
		{
			Http.MarkDroppedInternal();
		}
	}

	internal void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		EndpointProtocol endpointProtocol = EndpointProtocol;
		if (endpointProtocol != null)
		{
			sb.Append(ProtocolDdlName);
			sb.Append(" (");
			endpointProtocol.Script(sb, sp);
			sb.Append(")");
			return;
		}
		throw new InvalidSmoOperationException(ExceptionTemplatesImpl.IncorrectEndpointProtocol);
	}
}
