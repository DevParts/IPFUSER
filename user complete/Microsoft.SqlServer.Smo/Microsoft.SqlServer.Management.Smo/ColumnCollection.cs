using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ColumnCollection : ParameterCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public Column this[int index] => GetObjectByIndex(index) as Column;

	public Column this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as Column;

	internal ColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Column[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Column);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Column(this, key, state);
	}

	public void Add(Column column)
	{
		AddImpl(column);
	}

	public void Add(Column column, string insertAtColumnName)
	{
		AddImpl(column, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(Column column, int insertAtPosition)
	{
		AddImpl(column, insertAtPosition);
	}

	public void Remove(Column column)
	{
		if (column == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("column"));
		}
		RemoveObj(column, column.key);
	}

	public Column ItemById(int id)
	{
		return (Column)GetItemById(id);
	}
}
