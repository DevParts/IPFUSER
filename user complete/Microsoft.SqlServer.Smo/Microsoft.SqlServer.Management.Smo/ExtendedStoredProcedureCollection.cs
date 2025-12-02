using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExtendedStoredProcedureCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public ExtendedStoredProcedure this[int index] => GetObjectByIndex(index) as ExtendedStoredProcedure;

	public ExtendedStoredProcedure this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as ExtendedStoredProcedure;
		}
	}

	public ExtendedStoredProcedure this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as ExtendedStoredProcedure;
		}
	}

	internal ExtendedStoredProcedureCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExtendedStoredProcedure[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public ExtendedStoredProcedure ItemById(int id)
	{
		return (ExtendedStoredProcedure)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExtendedStoredProcedure);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExtendedStoredProcedure(this, key, state);
	}

	public void Add(ExtendedStoredProcedure extendedStoredProcedure)
	{
		if (extendedStoredProcedure == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("extendedStoredProcedure"));
		}
		AddImpl(extendedStoredProcedure);
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
