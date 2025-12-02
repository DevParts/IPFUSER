using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ClusterMemberStateConverter))]
public enum ClusterMemberState
{
	[LocDisplayName("cmsOffline")]
	Offline,
	[LocDisplayName("cmsOnline")]
	Online,
	[LocDisplayName("cmsPartiallyOnline")]
	PartiallyOnline,
	[LocDisplayName("cmsUnknown")]
	Unknown
}
