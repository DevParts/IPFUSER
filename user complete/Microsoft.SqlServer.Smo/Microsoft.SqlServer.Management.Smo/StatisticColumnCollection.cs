using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class StatisticColumnCollection : ParameterCollectionBase
{
	public Statistic Parent => base.ParentInstance as Statistic;

	public StatisticColumn this[int index] => GetObjectByIndex(index) as StatisticColumn;

	public StatisticColumn this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as StatisticColumn;

	internal StatisticColumnCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(StatisticColumn[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(StatisticColumn);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new StatisticColumn(this, key, state);
	}

	public void Add(StatisticColumn statisticColumn)
	{
		AddImpl(statisticColumn);
	}

	public void Add(StatisticColumn statisticColumn, string insertAtColumnName)
	{
		AddImpl(statisticColumn, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(StatisticColumn statisticColumn, int insertAtPosition)
	{
		AddImpl(statisticColumn, insertAtPosition);
	}

	public void Remove(StatisticColumn statisticColumn)
	{
		if (statisticColumn == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("statisticColumn"));
		}
		RemoveObj(statisticColumn, statisticColumn.key);
	}

	public StatisticColumn ItemById(int id)
	{
		return (StatisticColumn)GetItemById(id);
	}
}
