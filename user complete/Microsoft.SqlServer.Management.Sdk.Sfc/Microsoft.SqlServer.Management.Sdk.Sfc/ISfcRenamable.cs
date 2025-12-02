namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcRenamable
{
	void Rename(SfcKey newKey);

	ISfcScript ScriptRename(SfcKey newKey);
}
