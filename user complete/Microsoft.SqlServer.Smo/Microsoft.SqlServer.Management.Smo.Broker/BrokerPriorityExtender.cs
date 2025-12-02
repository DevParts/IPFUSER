using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class BrokerPriorityExtender : SmoObjectExtender<BrokerPriority>, ISfcValidate
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

	public BrokerPriorityExtender()
	{
	}

	public BrokerPriorityExtender(BrokerPriority brokerPriority)
		: base(brokerPriority)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
