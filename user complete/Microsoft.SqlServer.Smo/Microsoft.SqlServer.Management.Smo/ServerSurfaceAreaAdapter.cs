using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerSurfaceAreaAdapter : ServerAdapterBase, IDmfAdapter, ISurfaceAreaFacet, IDmfFacet
{
	private EndpointState desiredBrokerEndpointState;

	private bool brokerEndpointStateAltered;

	private bool disableSoapEndpoints;

	public bool ServiceBrokerEndpointActive
	{
		get
		{
			Endpoint brokerEndpoint = GetBrokerEndpoint();
			if (brokerEndpoint != null)
			{
				return brokerEndpoint.EndpointState == EndpointState.Started;
			}
			return false;
		}
		set
		{
			if (value)
			{
				desiredBrokerEndpointState = EndpointState.Started;
			}
			else
			{
				desiredBrokerEndpointState = EndpointState.Stopped;
			}
			brokerEndpointStateAltered = true;
		}
	}

	public bool SoapEndpointsEnabled
	{
		get
		{
			foreach (Endpoint endpoint in base.Server.Endpoints)
			{
				if (endpoint.EndpointType == EndpointType.Soap && endpoint.EndpointState != EndpointState.Disabled)
				{
					return true;
				}
			}
			return false;
		}
		set
		{
			if (value)
			{
				throw new SmoException(ExceptionTemplatesImpl.CannotEnableSoapEndpoints);
			}
			disableSoapEndpoints = true;
		}
	}

	public ServerSurfaceAreaAdapter(Server obj)
		: base(obj)
	{
	}

	private Endpoint GetBrokerEndpoint()
	{
		Endpoint result = null;
		foreach (Endpoint endpoint in base.Server.Endpoints)
		{
			if (endpoint.EndpointType == EndpointType.ServiceBroker)
			{
				result = endpoint;
				break;
			}
		}
		return result;
	}

	protected void RefreshEndpoints()
	{
		base.Server.Endpoints.Refresh();
		foreach (Endpoint endpoint in base.Server.Endpoints)
		{
			endpoint.Refresh();
		}
		brokerEndpointStateAltered = false;
		disableSoapEndpoints = false;
	}

	protected void AlterEndpoints()
	{
		if (brokerEndpointStateAltered)
		{
			brokerEndpointStateAltered = false;
			Endpoint brokerEndpoint = GetBrokerEndpoint();
			if (brokerEndpoint != null)
			{
				if (desiredBrokerEndpointState == EndpointState.Stopped)
				{
					brokerEndpoint.Stop();
				}
				else
				{
					if (desiredBrokerEndpointState != EndpointState.Started)
					{
						throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration("EndpointState"));
					}
					brokerEndpoint.Start();
				}
			}
			else if (desiredBrokerEndpointState != EndpointState.Stopped)
			{
				throw new SmoException(ExceptionTemplatesImpl.MissingBrokerEndpoint);
			}
		}
		if (!disableSoapEndpoints)
		{
			return;
		}
		foreach (Endpoint endpoint in base.Server.Endpoints)
		{
			if (endpoint.EndpointType == EndpointType.Soap && endpoint.EndpointState != EndpointState.Disabled)
			{
				endpoint.Disable();
			}
		}
		disableSoapEndpoints = false;
	}

	public override void Refresh()
	{
		base.Refresh();
		RefreshEndpoints();
	}

	public override void Alter()
	{
		base.Alter();
		AlterEndpoints();
	}
}
