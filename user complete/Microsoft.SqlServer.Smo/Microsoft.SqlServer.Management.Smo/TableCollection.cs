using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class TableCollection : SchemaCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public Table this[int index] => GetObjectByIndex(index) as Table;

	public Table this[string name]
	{
		get
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			return GetObjectByKey(new SchemaObjectKey(name, GetDefaultSchema())) as Table;
		}
	}

	public Table this[string name, string schema]
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
			return GetObjectByKey(new SchemaObjectKey(name, schema)) as Table;
		}
	}

	internal TableCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Table[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Table ItemById(int id)
	{
		return (Table)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Table);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Table(this, key, state);
	}

	public void Add(Table table)
	{
		if (table == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCollection, this, new ArgumentNullException("table"));
		}
		AddImpl(table);
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
