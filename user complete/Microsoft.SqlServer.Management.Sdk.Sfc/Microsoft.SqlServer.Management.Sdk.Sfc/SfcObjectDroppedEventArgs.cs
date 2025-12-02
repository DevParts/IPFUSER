namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcObjectDroppedEventArgs : SfcEventArgs
{
	public SfcObjectDroppedEventArgs(Urn urn, SfcInstance instance)
		: base(urn, instance)
	{
	}
}
