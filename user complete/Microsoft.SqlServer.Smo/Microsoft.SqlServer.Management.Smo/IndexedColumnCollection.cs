using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class IndexedColumnCollection : ParameterCollectionBase
{
	public Index Parent => base.ParentInstance as Index;

	public IndexedColumn this[int index] => GetObjectByIndex(index) as IndexedColumn;

	public IndexedColumn this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as IndexedColumn;

	internal IndexedColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(IndexedColumn[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(IndexedColumn);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new IndexedColumn(this, key, state);
	}

	public void Add(IndexedColumn indexedColumn)
	{
		AddImpl(indexedColumn);
	}

	public void Add(IndexedColumn indexedColumn, string insertAtColumnName)
	{
		AddImpl(indexedColumn, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(IndexedColumn indexedColumn, int insertAtPosition)
	{
		AddImpl(indexedColumn, insertAtPosition);
	}

	public void Remove(IndexedColumn indexedColumn)
	{
		if (indexedColumn == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("indexedColumn"));
		}
		RemoveObj(indexedColumn, indexedColumn.key);
	}

	public IndexedColumn ItemById(int id)
	{
		return (IndexedColumn)GetItemById(id);
	}
}
