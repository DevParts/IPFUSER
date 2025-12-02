namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public enum SfcObjectState
{
	None,
	Pending,
	Existing,
	Dropped,
	ToBeDropped,
	Recreate
}
