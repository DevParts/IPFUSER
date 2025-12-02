using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ObjectDroppedEventArgs : SmoEventArgs
{
	public ObjectDroppedEventArgs(Urn urn)
		: base(urn)
	{
	}
}
