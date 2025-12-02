using System.Collections;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal class DatabaseReplicaStateObjectComparer : ObjectComparerBase
{
	public DatabaseReplicaStateObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		DatabaseReplicaStateObjectKey databaseReplicaStateObjectKey = obj1 as DatabaseReplicaStateObjectKey;
		DatabaseReplicaStateObjectKey databaseReplicaStateObjectKey2 = obj2 as DatabaseReplicaStateObjectKey;
		TraceHelper.Assert(databaseReplicaStateObjectKey != null || null != databaseReplicaStateObjectKey2, "Can't compare null objects for DatabaseReplicaState");
		int num = stringComparer.Compare(databaseReplicaStateObjectKey.ReplicaName, databaseReplicaStateObjectKey2.ReplicaName);
		if (num == 0)
		{
			return stringComparer.Compare(databaseReplicaStateObjectKey.DatabaseName, databaseReplicaStateObjectKey2.DatabaseName);
		}
		return num;
	}
}
