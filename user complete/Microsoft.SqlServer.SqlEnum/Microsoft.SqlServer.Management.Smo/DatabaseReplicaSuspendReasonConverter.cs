namespace Microsoft.SqlServer.Management.Smo;

internal class DatabaseReplicaSuspendReasonConverter : EnumToDisplayNameConverter
{
	public DatabaseReplicaSuspendReasonConverter()
		: base(typeof(DatabaseReplicaSuspendReason))
	{
	}
}
