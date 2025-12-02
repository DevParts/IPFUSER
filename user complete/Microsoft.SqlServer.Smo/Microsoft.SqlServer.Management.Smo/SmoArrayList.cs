using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoArrayList : SmoInternalStorage
{
	internal ArrayList innerCollection;

	private SmoCollectionBase parent;

	internal override SqlSmoObject this[ObjectKeyBase key]
	{
		get
		{
			int num = LookUp(key);
			if (num != -1)
			{
				return innerCollection[num] as SqlSmoObject;
			}
			return null;
		}
		set
		{
			int num = LookUp(key);
			if (num != -1)
			{
				innerCollection[num] = value;
			}
			else
			{
				innerCollection.Add(value);
			}
		}
	}

	public override int Count => innerCollection.Count;

	internal override bool IsSynchronized => innerCollection.IsSynchronized;

	internal override object SyncRoot => innerCollection.SyncRoot;

	internal SmoArrayList(IComparer keyComparer, SmoCollectionBase parent)
		: base(keyComparer)
	{
		innerCollection = new ArrayList();
		this.parent = parent;
	}

	internal override bool Contains(ObjectKeyBase key)
	{
		return LookUp(key) != -1;
	}

	internal override int LookUp(ObjectKeyBase key)
	{
		for (int i = 0; i < innerCollection.Count; i++)
		{
			if (keyComparer.Compare(key, ((SqlSmoObject)innerCollection[i]).key) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	internal override SqlSmoObject GetByIndex(int index)
	{
		return innerCollection[index] as SqlSmoObject;
	}

	internal override void Add(ObjectKeyBase key, SqlSmoObject o)
	{
		innerCollection.Add(o);
		o.key.Writable = false;
	}

	internal override void Remove(ObjectKeyBase key)
	{
		int num = LookUp(key);
		if (num != -1)
		{
			((SqlSmoObject)innerCollection[num]).key.Writable = true;
			innerCollection.RemoveAt(num);
			return;
		}
		throw new InternalSmoErrorException(ExceptionTemplatesImpl.CouldNotFindKey(key.ToString()));
	}

	internal override void Clear()
	{
		innerCollection.Clear();
	}

	internal override void InsertAt(int position, SqlSmoObject o)
	{
		innerCollection.Insert(position, o);
	}

	internal override void RemoveAt(int position)
	{
		innerCollection.RemoveAt(position);
	}

	public override IEnumerator GetEnumerator()
	{
		return innerCollection.GetEnumerator();
	}
}
