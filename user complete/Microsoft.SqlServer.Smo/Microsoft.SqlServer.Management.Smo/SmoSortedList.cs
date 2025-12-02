using System.Collections;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoSortedList : SmoInternalStorage
{
	internal sealed class SmoSortedListEnumerator : IEnumerator
	{
		private IEnumerator baseEnumerator;

		public object Current => ((DictionaryEntry)baseEnumerator.Current).Value;

		internal SmoSortedListEnumerator(IEnumerator enumerator)
		{
			baseEnumerator = enumerator;
		}

		public bool MoveNext()
		{
			return baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			baseEnumerator.Reset();
		}
	}

	private SortedList innerCollection;

	internal override SqlSmoObject this[ObjectKeyBase key]
	{
		get
		{
			return innerCollection[key] as SqlSmoObject;
		}
		set
		{
			innerCollection[key] = value;
		}
	}

	public override int Count => innerCollection.Count;

	internal override bool IsSynchronized => innerCollection.IsSynchronized;

	internal override object SyncRoot => innerCollection.SyncRoot;

	internal SmoSortedList(IComparer keyComparer)
		: base(keyComparer)
	{
		innerCollection = new SortedList(keyComparer);
	}

	internal override bool Contains(ObjectKeyBase key)
	{
		return innerCollection.Contains(key);
	}

	internal override int LookUp(ObjectKeyBase key)
	{
		if (!Contains(key))
		{
			return 0;
		}
		return 1;
	}

	internal override SqlSmoObject GetByIndex(int index)
	{
		return innerCollection.GetByIndex(index) as SqlSmoObject;
	}

	internal override void Add(ObjectKeyBase key, SqlSmoObject o)
	{
		innerCollection[key] = o;
		o.key.Writable = false;
	}

	internal override void Remove(ObjectKeyBase key)
	{
		innerCollection.Remove(key);
	}

	internal override void InsertAt(int position, SqlSmoObject o)
	{
		TraceHelper.Assert(condition: false);
	}

	internal override void RemoveAt(int position)
	{
		innerCollection.RemoveAt(position);
	}

	internal override void Clear()
	{
		innerCollection.Clear();
	}

	public override IEnumerator GetEnumerator()
	{
		return new SmoSortedListEnumerator(innerCollection.GetEnumerator());
	}
}
