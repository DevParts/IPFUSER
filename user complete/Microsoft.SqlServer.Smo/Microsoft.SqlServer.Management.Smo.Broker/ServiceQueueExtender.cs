using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class ServiceQueueExtender : SmoObjectExtender<ServiceQueue>, ISfcValidate
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
	public Version ServerVersion => base.Parent.GetServerObject().Version;

	public ServiceQueueExtender()
	{
	}

	public ServiceQueueExtender(ServiceQueue serviceQueue)
		: base(serviceQueue)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
