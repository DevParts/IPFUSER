using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(DatabaseReplicaSuspendReasonConverter))]
public enum DatabaseReplicaSuspendReason
{
	[LocDisplayName("drsrSuspendFromUser")]
	SuspendFromUser,
	[LocDisplayName("drsrSuspendFromPartner")]
	SuspendFromPartner,
	[LocDisplayName("drsrSuspendFromRedo")]
	SuspendFromRedo,
	[LocDisplayName("drsrSuspendFromApply")]
	SuspendFromApply,
	[LocDisplayName("drsrSuspendFromCapture")]
	SuspendFromCapture,
	[LocDisplayName("drsrSuspendFromRestart")]
	SuspendFromRestart,
	[LocDisplayName("drsrSuspendFromUndo")]
	SuspendFromUndo,
	[LocDisplayName("drsrNotApplicable")]
	NotApplicable
}
