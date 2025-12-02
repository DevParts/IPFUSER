using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class BrokerServiceExtender : SmoObjectExtender<BrokerService>, ISfcValidate
{
	[ExtendedProperty]
	public string Name
	{
		get
		{
			return base.Parent.Name;
		}
		set
		{
			base.Parent.Name = value;
		}
	}

	[ExtendedProperty]
	public ServiceContractMappingCollection ServiceContractMappings => base.Parent.ServiceContractMappings;

	public BrokerServiceExtender()
	{
	}

	public BrokerServiceExtender(BrokerService brokerService)
		: base(brokerService)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
