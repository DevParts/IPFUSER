using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class ServiceRouteExtender : SmoObjectExtender<ServiceRoute>, ISfcValidate
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

	public ServiceRouteExtender()
	{
	}

	public ServiceRouteExtender(ServiceRoute route)
		: base(route)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
