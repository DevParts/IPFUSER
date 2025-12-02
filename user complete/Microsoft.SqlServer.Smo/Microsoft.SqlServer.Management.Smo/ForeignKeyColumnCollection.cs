using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ForeignKeyColumnCollection : ParameterCollectionBase
{
	public ForeignKey Parent => base.ParentInstance as ForeignKey;

	public ForeignKeyColumn this[int index] => GetObjectByIndex(index) as ForeignKeyColumn;

	public ForeignKeyColumn this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as ForeignKeyColumn;

	internal ForeignKeyColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ForeignKeyColumn[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ForeignKeyColumn);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ForeignKeyColumn(this, key, state);
	}

	public void Add(ForeignKeyColumn foreignKeyColumn)
	{
		AddImpl(foreignKeyColumn);
	}

	public void Add(ForeignKeyColumn foreignKeyColumn, string insertAtColumnName)
	{
		AddImpl(foreignKeyColumn, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(ForeignKeyColumn foreignKeyColumn, int insertAtPosition)
	{
		AddImpl(foreignKeyColumn, insertAtPosition);
	}

	public void Remove(ForeignKeyColumn foreignKeyColumn)
	{
		if (foreignKeyColumn == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("foreignKeyColumn"));
		}
		RemoveObj(foreignKeyColumn, foreignKeyColumn.key);
	}

	public ForeignKeyColumn ItemById(int id)
	{
		return (ForeignKeyColumn)GetItemById(id);
	}
}
