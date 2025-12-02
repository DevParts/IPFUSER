using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class ServiceContractExtender : SmoObjectExtender<ServiceContract>, ISfcValidate
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
	public MessageTypeMappingCollection MessageTypeMappings => base.Parent.MessageTypeMappings;

	public ServiceContractExtender()
	{
	}

	public ServiceContractExtender(ServiceContract serviceContract)
		: base(serviceContract)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
