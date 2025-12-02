namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcBeforeObjectRenamedEventArgs : SfcEventArgs
{
	private Urn newUrn;

	private SfcKey newKey;

	public Urn NewUrn => newUrn;

	public SfcKey NewKey => newKey;

	public SfcBeforeObjectRenamedEventArgs(Urn urn, SfcInstance instance, Urn newUrn, SfcKey newKey)
		: base(urn, instance)
	{
		this.newUrn = newUrn;
		this.newKey = newKey;
	}
}
