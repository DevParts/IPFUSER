using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Microsoft.SqlServer.Management.Smo;

internal class ObjectKeyBase
{
	private bool writable = true;

	public virtual string UrnFilter => string.Empty;

	public virtual bool IsNull => false;

	internal bool Writable
	{
		get
		{
			return writable;
		}
		set
		{
			writable = value;
		}
	}

	internal virtual void Validate(Type objectType)
	{
	}

	public virtual StringCollection GetFieldNames()
	{
		return new StringCollection();
	}

	public virtual ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new ObjectComparerBase(stringComparer);
	}

	public virtual ObjectKeyBase Clone()
	{
		return new ObjectKeyBase();
	}

	public virtual string GetExceptionName()
	{
		return ToString();
	}

	internal static StringCollection GetFieldNames(Type t)
	{
		if (t.IsSubclassOf(typeof(ScriptSchemaObjectBase)))
		{
			return SchemaObjectKey.schemaFields;
		}
		if (t.IsSubclassOf(typeof(MessageObjectBase)))
		{
			return MessageObjectKey.fields;
		}
		if (t.IsSubclassOf(typeof(SoapMethodObject)))
		{
			return SoapMethodKey.soapMethodFields;
		}
		if (t == typeof(NumberedStoredProcedure))
		{
			return NumberedObjectKey.fields;
		}
		if (t == typeof(PhysicalPartition))
		{
			return PartitionNumberedObjectKey.fields;
		}
		if (t == typeof(ScheduleBase))
		{
			return SimpleObjectKey.fields;
		}
		if (t == typeof(Job))
		{
			return JobObjectKey.jobKeyFields;
		}
		if (t == typeof(DatabaseReplicaState))
		{
			return DatabaseReplicaStateObjectKey.fields;
		}
		if (t == typeof(AvailabilityGroupListenerIPAddress))
		{
			return AvailabilityGroupListenerIPAddressObjectKey.fields;
		}
		if (t == typeof(SecurityPredicate))
		{
			return SecurityPredicateObjectKey.fields;
		}
		if (t == typeof(ColumnEncryptionKeyValue))
		{
			return ColumnEncryptionKeyValueObjectKey.fields;
		}
		return SimpleObjectKey.fields;
	}

	internal static ObjectKeyBase CreateKeyOffset(Type t, IDataReader reader, int columnOffset)
	{
		if (t.IsSubclassOf(typeof(ScriptSchemaObjectBase)))
		{
			return new SchemaObjectKey(reader.GetString(columnOffset + 1), reader.GetString(columnOffset));
		}
		if (t.IsSubclassOf(typeof(MessageObjectBase)))
		{
			return new MessageObjectKey(reader.GetInt32(columnOffset), reader.GetString(columnOffset + 1));
		}
		if (t == typeof(NumberedStoredProcedure))
		{
			return new NumberedObjectKey(reader.GetInt16(columnOffset));
		}
		if (t == typeof(PhysicalPartition))
		{
			return new PartitionNumberedObjectKey((short)reader.GetInt32(columnOffset));
		}
		if (SqlSmoObject.IsOrderedByID(t))
		{
			return new SimpleObjectKey(reader.GetString(columnOffset + 1));
		}
		if (t.IsSubclassOf(typeof(SoapMethodObject)))
		{
			return new SoapMethodKey(reader.GetString(columnOffset + 1), reader.GetString(columnOffset));
		}
		if (t.IsSubclassOf(typeof(ScheduleBase)))
		{
			return new ScheduleObjectKey(reader.GetString(columnOffset), reader.GetInt32(columnOffset + 1));
		}
		if (t == typeof(Job))
		{
			return new JobObjectKey(reader.GetString(columnOffset), reader.GetInt32(columnOffset + 1));
		}
		if (t == typeof(DatabaseReplicaState))
		{
			return new DatabaseReplicaStateObjectKey(reader.GetString(columnOffset), reader.GetString(columnOffset + 1));
		}
		if (t == typeof(AvailabilityGroupListenerIPAddress))
		{
			return new AvailabilityGroupListenerIPAddressObjectKey(reader.GetString(columnOffset), reader.GetString(columnOffset + 1), reader.GetString(columnOffset + 2));
		}
		if (t == typeof(SecurityPredicate))
		{
			return new SecurityPredicateObjectKey(reader.GetInt32(columnOffset));
		}
		if (t == typeof(ColumnEncryptionKeyValue))
		{
			return new ColumnEncryptionKeyValueObjectKey(reader.GetInt32(columnOffset));
		}
		return new SimpleObjectKey(reader.GetString(columnOffset));
	}
}
