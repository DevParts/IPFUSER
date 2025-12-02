namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcBeforeObjectMovedEventArgs : SfcEventArgs
{
	private Urn newUrn;

	private SfcInstance newParent;

	public Urn NewUrn => newUrn;

	public SfcInstance NewParent => newParent;

	public SfcBeforeObjectMovedEventArgs(Urn urn, SfcInstance instance, Urn newUrn, SfcInstance newParent)
		: base(urn, instance)
	{
		this.newUrn = newUrn;
		this.newParent = newParent;
	}
}
