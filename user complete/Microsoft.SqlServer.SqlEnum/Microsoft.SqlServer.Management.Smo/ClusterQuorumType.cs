using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ClusterQuorumTypeConverter))]
public enum ClusterQuorumType
{
	[LocDisplayName("cqtNodeMajority")]
	NodeMajority,
	[LocDisplayName("cqtNodeAndDiskMajority")]
	NodeAndDiskMajority,
	[LocDisplayName("cqtNodeAndFileshareMajority")]
	NodeAndFileshareMajority,
	[LocDisplayName("cqtDiskOnly")]
	DiskOnly,
	[LocDisplayName("cqtNotApplicable")]
	NotApplicable
}
