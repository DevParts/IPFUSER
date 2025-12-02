using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedAggregateCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public UserDefinedAggregate this[int index] => GetObjectByIndex(index) as UserDefinedAggregate;

	public UserDefinedAggregate this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as UserDefinedAggregate;
		}
	}

	public UserDefinedAggregate this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as UserDefinedAggregate;
		}
	}

	internal UserDefinedAggregateCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedAggregate[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserDefinedAggregate ItemById(int id)
	{
		return (UserDefinedAggregate)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedAggregate);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedAggregate(this, key, state);
	}

	public void Add(UserDefinedAggregate userDefinedAggregate)
	{
		if (userDefinedAggregate == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("userDefinedAggregate"));
		}
		AddImpl(userDefinedAggregate);
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
