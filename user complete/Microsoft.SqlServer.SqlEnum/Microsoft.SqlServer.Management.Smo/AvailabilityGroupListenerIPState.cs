using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityGroupListenerIPStateConverter))]
public enum AvailabilityGroupListenerIPState
{
	[LocDisplayName("aglipOffline")]
	Offline,
	[LocDisplayName("aglipOnline")]
	Online,
	[LocDisplayName("aglipOnlinePending")]
	OnlinePending,
	[LocDisplayName("agliFailure")]
	Failure,
	[LocDisplayName("agliUnknown")]
	Unknown
}
