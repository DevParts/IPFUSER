using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

public class Payload
{
	private Endpoint m_endpoint;

	private SoapPayload m_soapPayload;

	private ServiceBrokerPayload m_serviceBrokerPayload;

	private DatabaseMirroringPayload m_databaseMirroringPayload;

	public SoapPayload Soap
	{
		get
		{
			if (m_soapPayload == null)
			{
				m_soapPayload = new SoapPayload(m_endpoint, new ObjectKeyBase(), GetStateForType(EndpointType.Soap));
			}
			return m_soapPayload;
		}
	}

	public ServiceBrokerPayload ServiceBroker
	{
		get
		{
			if (m_serviceBrokerPayload == null)
			{
				m_serviceBrokerPayload = new ServiceBrokerPayload(m_endpoint, new ObjectKeyBase(), GetStateForType(EndpointType.ServiceBroker));
			}
			return m_serviceBrokerPayload;
		}
	}

	public DatabaseMirroringPayload DatabaseMirroring
	{
		get
		{
			if (m_databaseMirroringPayload == null)
			{
				m_databaseMirroringPayload = new DatabaseMirroringPayload(m_endpoint, new ObjectKeyBase(), GetStateForType(EndpointType.DatabaseMirroring));
			}
			return m_databaseMirroringPayload;
		}
	}

	internal EndpointPayload EndpointPayload => (EndpointType)m_endpoint.GetPropValue("EndpointType") switch
	{
		EndpointType.Soap => Soap, 
		EndpointType.ServiceBroker => ServiceBroker, 
		EndpointType.DatabaseMirroring => DatabaseMirroring, 
		EndpointType.TSql => null, 
		_ => throw new WrongPropertyValueException(m_endpoint.Properties.Get("EndpointType")), 
	};

	private string PayloadDdlName => (EndpointType)m_endpoint.GetPropValue("EndpointType") switch
	{
		EndpointType.Soap => "SOAP", 
		EndpointType.ServiceBroker => "SERVICE_BROKER", 
		EndpointType.DatabaseMirroring => "DATA_MIRRORING", 
		EndpointType.TSql => "TSQL", 
		_ => throw new WrongPropertyValueException(m_endpoint.Properties.Get("EndpointType")), 
	};

	internal Payload(Endpoint endpoint)
	{
		m_endpoint = endpoint;
	}

	private SqlSmoState GetStateForType(EndpointType et)
	{
		object propValueOptional = m_endpoint.GetPropValueOptional("EndpointType");
		Property property = m_endpoint.Properties.Get("EndpointType");
		propValueOptional = (property.Dirty ? m_endpoint.oldEndpointTypeValue : property.Value);
		if (propValueOptional == null)
		{
			return SqlSmoState.Creating;
		}
		if ((EndpointType)propValueOptional == et)
		{
			return m_endpoint.State;
		}
		return SqlSmoState.Creating;
	}

	internal void MarkDropped()
	{
		if (Soap != null)
		{
			Soap.MarkDroppedInternal();
		}
		if (ServiceBroker != null)
		{
			ServiceBroker.MarkDroppedInternal();
		}
		if (DatabaseMirroring != null)
		{
			DatabaseMirroring.MarkDroppedInternal();
		}
	}

	internal void Script(StringBuilder sb, ScriptingPreferences sp)
	{
		sb.Append(PayloadDdlName);
		EndpointPayload endpointPayload = EndpointPayload;
		sb.Append(" (");
		endpointPayload?.Script(sb, sp);
		sb.Append(")");
	}
}
