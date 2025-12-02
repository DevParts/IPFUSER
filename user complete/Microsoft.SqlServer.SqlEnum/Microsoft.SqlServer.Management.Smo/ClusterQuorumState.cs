using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ClusterQuorumStateConverter))]
public enum ClusterQuorumState
{
	[LocDisplayName("cqsUnknownQuorumState")]
	UnknownQuorumState,
	[LocDisplayName("cqsNormalQuorum")]
	NormalQuorum,
	[LocDisplayName("cqsForcedQuorum")]
	ForcedQuorum,
	[LocDisplayName("cqsNotApplicable")]
	NotApplicable
}
