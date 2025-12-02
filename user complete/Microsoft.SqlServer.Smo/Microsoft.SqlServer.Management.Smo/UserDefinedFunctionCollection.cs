using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedFunctionCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public UserDefinedFunction this[int index] => GetObjectByIndex(index) as UserDefinedFunction;

	public UserDefinedFunction this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as UserDefinedFunction;
		}
	}

	public UserDefinedFunction this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as UserDefinedFunction;
		}
	}

	internal UserDefinedFunctionCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(UserDefinedFunction[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public UserDefinedFunction ItemById(int id)
	{
		return (UserDefinedFunction)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedFunction);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedFunction(this, key, state);
	}

	public void Add(UserDefinedFunction userDefinedFunction)
	{
		if (userDefinedFunction == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("userDefinedFunction"));
		}
		AddImpl(userDefinedFunction);
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
