namespace Microsoft.SqlServer.Management.Smo;

internal class ClusterMemberStateConverter : EnumToDisplayNameConverter
{
	public ClusterMemberStateConverter()
		: base(typeof(ClusterMemberState))
	{
	}
}
