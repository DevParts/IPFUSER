using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseReplicaStateCollection : DatabaseReplicaStateCollectionBase
{
	public AvailabilityGroup Parent => base.ParentInstance as AvailabilityGroup;

	public DatabaseReplicaState this[int index] => GetObjectByIndex(index) as DatabaseReplicaState;

	public DatabaseReplicaState this[string replicaName, string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			if (replicaName == null)
			{
				throw new ArgumentNullException("replica name cannot be null");
			}
			return GetObjectByKey(new DatabaseReplicaStateObjectKey(replicaName, name)) as DatabaseReplicaState;
		}
	}

	internal DatabaseReplicaStateCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(DatabaseReplicaState[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public DatabaseReplicaState ItemById(int id)
	{
		return (DatabaseReplicaState)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(DatabaseReplicaState);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new DatabaseReplicaState(this, key, state);
	}

	public void Remove(string replicaName, string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name cannot be null");
		}
		if (replicaName == null)
		{
			throw new ArgumentNullException("replica name cannot be null");
		}
		Remove(new DatabaseReplicaStateObjectKey(replicaName, name));
	}

	public void Remove(DatabaseReplicaState DatabaseReplicaState)
	{
		if (DatabaseReplicaState == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("DatabaseReplicaState"));
		}
		RemoveObj(DatabaseReplicaState, new DatabaseReplicaStateObjectKey(DatabaseReplicaState.AvailabilityReplicaServerName, DatabaseReplicaState.AvailabilityDatabaseName));
	}

	public void Add(DatabaseReplicaState DatabaseReplicaState)
	{
		if (DatabaseReplicaState == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("DatabaseReplicaState"));
		}
		AddImpl(DatabaseReplicaState);
	}
}
