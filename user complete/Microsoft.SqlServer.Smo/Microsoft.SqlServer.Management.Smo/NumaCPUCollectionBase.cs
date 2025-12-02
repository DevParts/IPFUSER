using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class NumaCPUCollectionBase<T> : ICollection, IEnumerable
{
	private AffinityInfoBase parent;

	internal SortedList cpuNumaCol = new SortedList(new NumaCPUComparer());

	int ICollection.Count
	{
		get
		{
			if (parent.AffinityInfoTable == null)
			{
				parent.PopulateDataTable();
			}
			return cpuNumaCol.Count;
		}
	}

	bool ICollection.IsSynchronized
	{
		get
		{
			if (parent.AffinityInfoTable == null)
			{
				parent.PopulateDataTable();
			}
			return parent.AffinityInfoTable.Rows.IsSynchronized;
		}
	}

	object ICollection.SyncRoot
	{
		get
		{
			if (parent.AffinityInfoTable == null)
			{
				parent.PopulateDataTable();
			}
			return parent.AffinityInfoTable.Rows.SyncRoot;
		}
	}

	internal T this[int index]
	{
		get
		{
			if (parent.AffinityInfoTable == null)
			{
				parent.PopulateDataTable();
			}
			return (T)cpuNumaCol[index];
		}
	}

	internal NumaCPUCollectionBase(AffinityInfoBase parent)
	{
		this.parent = parent;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		if (index < 0 || index >= cpuNumaCol.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				T val = (T)enumerator.Current;
				array.SetValue(val, index++);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void CopyTo(T[] array, int index)
	{
		if (index < 0 || index >= cpuNumaCol.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				T val = (T)enumerator.Current;
				array.SetValue(val, index++);
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	public IEnumerator GetEnumerator()
	{
		if (parent.AffinityInfoTable == null)
		{
			parent.PopulateDataTable();
		}
		return new NumaCPUEnumerator(cpuNumaCol);
	}
}
