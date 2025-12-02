namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcAfterObjectRenamedEventArgs : SfcEventArgs
{
	private Urn oldUrn;

	private SfcKey oldKey;

	public Urn OldUrn => oldUrn;

	public SfcKey OldKey => oldKey;

	public SfcAfterObjectRenamedEventArgs(Urn urn, SfcInstance instance, Urn oldUrn, SfcKey oldKey)
		: base(urn, instance)
	{
		this.oldUrn = oldUrn;
		this.oldKey = oldKey;
	}
}
