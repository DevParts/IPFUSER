using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DefaultCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Default this[int index] => GetObjectByIndex(index) as Default;

	public Default this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as Default;
		}
	}

	public Default this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as Default;
		}
	}

	internal DefaultCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Default[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Default ItemById(int id)
	{
		return (Default)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Default);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Default(this, key, state);
	}

	public void Add(Default def)
	{
		if (def == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("def"));
		}
		AddImpl(def);
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
