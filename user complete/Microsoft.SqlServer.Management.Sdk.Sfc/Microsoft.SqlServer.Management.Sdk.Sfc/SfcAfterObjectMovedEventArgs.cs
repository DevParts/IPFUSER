namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcAfterObjectMovedEventArgs : SfcEventArgs
{
	private Urn oldUrn;

	private SfcInstance oldParent;

	public Urn OldUrn => oldUrn;

	public SfcInstance OldParent => oldParent;

	public SfcAfterObjectMovedEventArgs(Urn urn, SfcInstance instance, Urn oldUrn, SfcInstance oldParent)
		: base(urn, instance)
	{
		this.oldUrn = oldUrn;
		this.oldParent = oldParent;
	}
}
