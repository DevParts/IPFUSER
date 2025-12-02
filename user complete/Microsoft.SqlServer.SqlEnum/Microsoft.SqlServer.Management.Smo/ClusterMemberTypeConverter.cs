namespace Microsoft.SqlServer.Management.Smo;

internal class ClusterMemberTypeConverter : EnumToDisplayNameConverter
{
	public ClusterMemberTypeConverter()
		: base(typeof(ClusterMemberType))
	{
	}
}
