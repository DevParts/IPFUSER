using System.ComponentModel;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayDescriptionKey("AvailabilityGroupStateDesc")]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
[DisplayNameKey("AvailabilityGroupStateName")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
public interface IAvailabilityGroupState : IDmfFacet, IRefreshable
{
	[DisplayDescriptionKey("AvailabilityGroupState_IsOnlineDesc")]
	[DisplayNameKey("AvailabilityGroupState_IsOnlineName")]
	bool IsOnline { get; }

	[DisplayDescriptionKey("AvailabilityGroupState_IsAutoFailoverDesc")]
	[DisplayNameKey("AvailabilityGroupState_IsAutoFailoverName")]
	bool IsAutoFailover { get; }

	[DisplayDescriptionKey("AvailabilityGroupState_NumberOfSynchronizedSecondaryReplicasDesc")]
	[DisplayNameKey("AvailabilityGroupState_NumberOfSynchronizedSecondaryReplicasName")]
	int NumberOfSynchronizedSecondaryReplicas { get; }

	[DisplayDescriptionKey("AvailabilityGroupState_NumberOfNotSynchronizingReplicasDesc")]
	[DisplayNameKey("AvailabilityGroupState_NumberOfNotSynchronizingReplicasName")]
	int NumberOfNotSynchronizingReplicas { get; }

	[DisplayNameKey("AvailabilityGroupState_NumberOfNotSynchronizedReplicasName")]
	[DisplayDescriptionKey("AvailabilityGroupState_NumberOfNotSynchronizedReplicasDesc")]
	int NumberOfNotSynchronizedReplicas { get; }

	[DisplayNameKey("AvailabilityGroupState_NumberOfReplicasWithUnhealthyRoleName")]
	[DisplayDescriptionKey("AvailabilityGroupState_NumberOfReplicasWithUnhealthyRoleDesc")]
	int NumberOfReplicasWithUnhealthyRole { get; }

	[DisplayNameKey("AvailabilityGroupState_NumberOfDisconnectedReplicasName")]
	[DisplayDescriptionKey("AvailabilityGroupState_NumberOfDisconnectedReplicasDesc")]
	int NumberOfDisconnectedReplicas { get; }
}
