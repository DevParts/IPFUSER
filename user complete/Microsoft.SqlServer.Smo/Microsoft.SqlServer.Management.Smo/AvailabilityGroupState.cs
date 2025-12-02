using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class AvailabilityGroupState : IAvailabilityGroupState, IDmfFacet, IDmfAdapter, IRefreshable
{
	private AvailabilityGroup ag;

	private bool isInitialized;

	private bool isOnline;

	private bool? isAutoFailover;

	private int numberOfSynchronizedSecondaryReplicas;

	private int numberOfNotSynchronizingReplicas;

	private int numberOfNotSynchronizedReplicas;

	private int numberOfReplicasWithUnhealthyRole;

	private int numberOfDisconnectedReplicas;

	public bool IsOnline
	{
		get
		{
			CheckInitialized();
			return isOnline;
		}
	}

	public bool IsAutoFailover
	{
		get
		{
			if (!isAutoFailover.HasValue)
			{
				isAutoFailover = false;
				string primaryReplicaServerName = ag.PrimaryReplicaServerName;
				AvailabilityReplica availabilityReplica = ((!string.IsNullOrEmpty(primaryReplicaServerName)) ? ag.AvailabilityReplicas[primaryReplicaServerName] : null);
				if (availabilityReplica == null)
				{
					throw new PropertyCannotBeRetrievedException(ExceptionTemplatesImpl.PropertyCannotBeRetrievedFromSecondary("IsAutoFailover"));
				}
				isAutoFailover = availabilityReplica.FailoverMode == AvailabilityReplicaFailoverMode.Automatic && availabilityReplica.AvailabilityMode == AvailabilityReplicaAvailabilityMode.SynchronousCommit;
			}
			return isAutoFailover.Value;
		}
	}

	public int NumberOfSynchronizedSecondaryReplicas
	{
		get
		{
			CheckInitialized();
			return numberOfSynchronizedSecondaryReplicas;
		}
	}

	public int NumberOfNotSynchronizingReplicas
	{
		get
		{
			CheckInitialized();
			return numberOfNotSynchronizingReplicas;
		}
	}

	public int NumberOfNotSynchronizedReplicas
	{
		get
		{
			CheckInitialized();
			return numberOfNotSynchronizedReplicas;
		}
	}

	public int NumberOfReplicasWithUnhealthyRole
	{
		get
		{
			CheckInitialized();
			return numberOfReplicasWithUnhealthyRole;
		}
	}

	public int NumberOfDisconnectedReplicas
	{
		get
		{
			CheckInitialized();
			return numberOfDisconnectedReplicas;
		}
	}

	public AvailabilityGroupState(AvailabilityGroup ag)
	{
		this.ag = ag;
		isInitialized = false;
	}

	public void Refresh()
	{
		ag.Refresh();
		isInitialized = false;
		isAutoFailover = null;
	}

	private void Initialize()
	{
		isOnline = !string.IsNullOrEmpty(ag.PrimaryReplicaServerName);
		numberOfSynchronizedSecondaryReplicas = (from AvailabilityReplica ar in ag.AvailabilityReplicas
			where ar.Role == AvailabilityReplicaRole.Secondary && ar.FailoverMode == AvailabilityReplicaFailoverMode.Automatic && ar.AvailabilityMode == AvailabilityReplicaAvailabilityMode.SynchronousCommit && ar.RollupSynchronizationState == AvailabilityReplicaRollupSynchronizationState.Synchronized
			select ar).Count();
		numberOfNotSynchronizingReplicas = (from AvailabilityReplica ar in ag.AvailabilityReplicas
			where ar.RollupSynchronizationState == AvailabilityReplicaRollupSynchronizationState.NotSynchronizing
			select ar).Count();
		numberOfNotSynchronizedReplicas = (from AvailabilityReplica ar in ag.AvailabilityReplicas
			where ar.AvailabilityMode == AvailabilityReplicaAvailabilityMode.SynchronousCommit && ar.RollupSynchronizationState != AvailabilityReplicaRollupSynchronizationState.Synchronized
			select ar).Count();
		numberOfReplicasWithUnhealthyRole = (from AvailabilityReplica ar in ag.AvailabilityReplicas
			where ar.Role != AvailabilityReplicaRole.Secondary && ar.Role != AvailabilityReplicaRole.Primary
			select ar).Count();
		numberOfDisconnectedReplicas = (from AvailabilityReplica ar in ag.AvailabilityReplicas
			where ar.ConnectionState != AvailabilityReplicaConnectionState.Connected
			select ar).Count();
		isInitialized = true;
	}

	private void CheckInitialized()
	{
		if (!isInitialized)
		{
			Initialize();
		}
	}
}
