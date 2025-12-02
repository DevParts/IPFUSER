using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedTypeCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public UserDefinedType this[int index] => GetObjectByIndex(index) as UserDefinedType;

	public UserDefinedType this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as UserDefinedType;
		}
	}

	public UserDefinedType this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as UserDefinedType;
		}
	}

	internal UserDefinedTypeCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedType[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserDefinedType ItemById(int id)
	{
		return (UserDefinedType)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedType);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedType(this, key, state);
	}

	public void Add(UserDefinedType userDefinedType)
	{
		if (userDefinedType == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("userDefinedType"));
		}
		AddImpl(userDefinedType);
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
