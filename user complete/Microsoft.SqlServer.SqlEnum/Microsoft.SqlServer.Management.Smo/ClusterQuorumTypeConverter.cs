namespace Microsoft.SqlServer.Management.Smo;

internal class ClusterQuorumTypeConverter : EnumToDisplayNameConverter
{
	public ClusterQuorumTypeConverter()
		: base(typeof(ClusterQuorumType))
	{
	}
}
