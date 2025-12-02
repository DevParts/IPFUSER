using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class StoredProcedureCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public StoredProcedure this[int index] => GetObjectByIndex(index) as StoredProcedure;

	public StoredProcedure this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as StoredProcedure;
		}
	}

	public StoredProcedure this[string name, string schema]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			if (schema == null)
			{
				throw new ArgumentNullException("schema cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as StoredProcedure;
		}
	}

	internal StoredProcedureCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(StoredProcedure[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public StoredProcedure ItemById(int id)
	{
		return (StoredProcedure)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(StoredProcedure);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new StoredProcedure(this, key, state);
	}

	public void Add(StoredProcedure storedProcedure)
	{
		if (storedProcedure == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("storedProcedure"));
		}
		AddImpl(storedProcedure);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("schema cannot be null");
		}
		return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema()));
	}
}
