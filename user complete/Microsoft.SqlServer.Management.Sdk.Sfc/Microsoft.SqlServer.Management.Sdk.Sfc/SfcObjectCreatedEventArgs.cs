namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcObjectCreatedEventArgs : SfcEventArgs
{
	public SfcObjectCreatedEventArgs(Urn urn, SfcInstance instance)
		: base(urn, instance)
	{
	}
}
