namespace Microsoft.SqlServer.Management.Smo;

internal class ClusterQuorumStateConverter : EnumToDisplayNameConverter
{
	public ClusterQuorumStateConverter()
		: base(typeof(ClusterQuorumState))
	{
	}
}
