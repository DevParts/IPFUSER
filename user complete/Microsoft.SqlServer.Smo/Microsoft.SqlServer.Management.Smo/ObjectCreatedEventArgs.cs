using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ObjectCreatedEventArgs : SmoEventArgs
{
	private object innerObject;

	public object SmoObject => innerObject;

	public ObjectCreatedEventArgs(Urn urn, object innerObject)
		: base(urn)
	{
		this.innerObject = innerObject;
	}
}
