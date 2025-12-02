using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class OrderColumnCollection : ParameterCollectionBase
{
	public UserDefinedFunction Parent => base.ParentInstance as UserDefinedFunction;

	public OrderColumn this[int index] => GetObjectByIndex(index) as OrderColumn;

	public OrderColumn this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as OrderColumn;

	internal OrderColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(OrderColumn[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(OrderColumn);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new OrderColumn(this, key, state);
	}

	public void Add(OrderColumn orderColumn)
	{
		AddImpl(orderColumn);
	}

	public void Add(OrderColumn orderColumn, string insertAtColumnName)
	{
		AddImpl(orderColumn, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(OrderColumn orderColumn, int insertAtPosition)
	{
		AddImpl(orderColumn, insertAtPosition);
	}

	public void Remove(OrderColumn orderColumn)
	{
		if (orderColumn == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("orderColumn"));
		}
		RemoveObj(orderColumn, orderColumn.key);
	}

	public OrderColumn ItemById(int id)
	{
		return (OrderColumn)GetItemById(id);
	}
}
