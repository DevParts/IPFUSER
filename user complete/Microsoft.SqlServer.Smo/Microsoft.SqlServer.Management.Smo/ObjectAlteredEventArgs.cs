using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ObjectAlteredEventArgs : SmoEventArgs
{
	private object innerObject;

	public object SmoObject => innerObject;

	public ObjectAlteredEventArgs(Urn urn, object innerObject)
		: base(urn)
	{
		this.innerObject = innerObject;
	}
}
