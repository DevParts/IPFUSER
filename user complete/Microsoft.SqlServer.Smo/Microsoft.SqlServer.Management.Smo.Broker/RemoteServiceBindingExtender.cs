using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Broker;

[CLSCompliant(false)]
public class RemoteServiceBindingExtender : SmoObjectExtender<RemoteServiceBinding>, ISfcValidate
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

	public RemoteServiceBindingExtender()
	{
	}

	public RemoteServiceBindingExtender(RemoteServiceBinding remoteServiceBinding)
		: base(remoteServiceBinding)
	{
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		return base.Parent.Validate(methodName, arguments);
	}
}
