using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ObjectRenamedEventArgs : SmoEventArgs
{
	private string oldUrn;

	private string newName;

	private string oldName;

	private object innerObject;

	public object SmoObject => innerObject;

	public string OldName => oldName;

	public string NewName => newName;

	public string OldUrn => oldUrn;

	public ObjectRenamedEventArgs(Urn urn, object innerObject, string oldName, string newName)
		: base(urn)
	{
		this.innerObject = innerObject;
		this.oldName = oldName;
		this.newName = newName;
	}

	public ObjectRenamedEventArgs(Urn newUrn, object innerObject, string oldName, string newName, string oldUrn)
		: base(newUrn)
	{
		this.innerObject = innerObject;
		this.oldName = oldName;
		this.newName = newName;
		this.oldUrn = oldUrn;
	}
}
