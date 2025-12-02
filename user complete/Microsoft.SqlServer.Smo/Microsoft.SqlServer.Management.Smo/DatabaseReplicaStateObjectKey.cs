using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class DatabaseReplicaStateObjectKey : ObjectKeyBase
{
	internal static StringCollection fields;

	public string ReplicaName { get; set; }

	public string DatabaseName { get; set; }

	public override string UrnFilter => string.Format(SmoApplication.DefaultCulture, "@AvailabilityReplicaServerName='{0}' and @AvailabilityDatabaseName='{1}'", new object[2]
	{
		SqlSmoObject.SqlString(ReplicaName),
		SqlSmoObject.SqlString(DatabaseName)
	});

	public override bool IsNull
	{
		get
		{
			if (DatabaseName != null)
			{
				return null == ReplicaName;
			}
			return true;
		}
	}

	public DatabaseReplicaStateObjectKey(string replicaName, string databaseName)
	{
		ReplicaName = replicaName;
		DatabaseName = databaseName;
	}

	static DatabaseReplicaStateObjectKey()
	{
		fields = new StringCollection();
		fields.Add("AvailabilityReplicaServerName");
		fields.Add("AvailabilityDatabaseName");
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	internal override void Validate(Type objectType)
	{
		if (string.IsNullOrEmpty(ReplicaName) || string.IsNullOrEmpty(DatabaseName))
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(objectType.ToString())).SetHelpContext("UnsupportedObjectNameExceptionText");
		}
	}

	public override string GetExceptionName()
	{
		return string.Format(SmoApplication.DefaultCulture, "Database {1} in Availability Replica {0}", new object[2] { ReplicaName, DatabaseName });
	}

	public override ObjectKeyBase Clone()
	{
		return new DatabaseReplicaStateObjectKey(ReplicaName, DatabaseName);
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new DatabaseReplicaStateObjectComparer(stringComparer);
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}/{1}", new object[2] { ReplicaName, DatabaseName });
	}
}
