namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcMovable
{
	void Move(SfcInstance newParent);

	ISfcScript ScriptMove(SfcInstance newParent);
}
