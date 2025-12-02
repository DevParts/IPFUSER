using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedTableTypeCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public UserDefinedTableType this[int index] => GetObjectByIndex(index) as UserDefinedTableType;

	public UserDefinedTableType this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as UserDefinedTableType;
		}
	}

	public UserDefinedTableType this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as UserDefinedTableType;
		}
	}

	internal UserDefinedTableTypeCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedTableType[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserDefinedTableType ItemById(int id)
	{
		return (UserDefinedTableType)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedTableType);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedTableType(this, key, state);
	}

	public void Add(UserDefinedTableType userDefinedTableType)
	{
		if (userDefinedTableType == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("userDefinedTableType"));
		}
		AddImpl(userDefinedTableType);
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
