using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class StatisticCollection : SimpleObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public Statistic this[int index] => GetObjectByIndex(index) as Statistic;

	public Statistic this[string name] => GetObjectByName(name) as Statistic;

	internal StatisticCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Statistic[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Statistic ItemById(int id)
	{
		return (Statistic)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Statistic);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Statistic(this, key, state);
	}

	public void Remove(Statistic statistic)
	{
		if (statistic == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("statistic"));
		}
		RemoveObj(statistic, new SimpleObjectKey(statistic.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(Statistic statistic)
	{
		AddImpl(statistic);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
