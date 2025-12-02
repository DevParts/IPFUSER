namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcObjectAlteredEventArgs : SfcEventArgs
{
	public SfcObjectAlteredEventArgs(Urn urn, SfcInstance instance)
		: base(urn, instance)
	{
	}
}
