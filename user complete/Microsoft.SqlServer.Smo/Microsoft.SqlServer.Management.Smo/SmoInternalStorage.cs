using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class SmoInternalStorage : IEnumerable
{
	protected IComparer keyComparer;

	internal abstract SqlSmoObject this[ObjectKeyBase key] { get; set; }

	public abstract int Count { get; }

	internal abstract bool IsSynchronized { get; }

	internal abstract object SyncRoot { get; }

	internal SmoInternalStorage(IComparer keyComparer)
	{
		this.keyComparer = keyComparer;
	}

	internal abstract bool Contains(ObjectKeyBase key);

	internal abstract int LookUp(ObjectKeyBase key);

	internal abstract SqlSmoObject GetByIndex(int index);

	internal abstract void Add(ObjectKeyBase key, SqlSmoObject o);

	internal abstract void Remove(ObjectKeyBase key);

	internal abstract void InsertAt(int position, SqlSmoObject o);

	internal abstract void RemoveAt(int position);

	public abstract IEnumerator GetEnumerator();

	internal abstract void Clear();
}
