using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedDataTypeCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public UserDefinedDataType this[int index] => GetObjectByIndex(index) as UserDefinedDataType;

	public UserDefinedDataType this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as UserDefinedDataType;
		}
	}

	public UserDefinedDataType this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as UserDefinedDataType;
		}
	}

	internal UserDefinedDataTypeCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedDataType[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserDefinedDataType ItemById(int id)
	{
		return (UserDefinedDataType)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedDataType);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedDataType(this, key, state);
	}

	public void Add(UserDefinedDataType userDefinedDataType)
	{
		if (userDefinedDataType == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("userDefinedDataType"));
		}
		AddImpl(userDefinedDataType);
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
