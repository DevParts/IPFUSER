using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class UrnCollection : IList<Urn>, ICollection<Urn>, IEnumerable<Urn>, IEnumerable
{
	private List<Urn> innerColl;

	public Urn this[int index]
	{
		get
		{
			return innerColl[index];
		}
		set
		{
			innerColl[index] = value;
		}
	}

	public int Count => innerColl.Count;

	bool ICollection<Urn>.IsReadOnly => ((ICollection<Urn>)innerColl).IsReadOnly;

	public UrnCollection()
	{
		innerColl = new List<Urn>();
	}

	public int IndexOf(Urn urn)
	{
		return innerColl.IndexOf(urn);
	}

	public void Insert(int index, Urn urn)
	{
		innerColl.Insert(index, urn);
	}

	public void RemoveAt(int index)
	{
		innerColl.RemoveAt(index);
	}

	public void Add(Urn urn)
	{
		innerColl.Add(urn);
	}

	public void AddRange(IEnumerable<Urn> urnCollection)
	{
		innerColl.AddRange(urnCollection);
	}

	public void Clear()
	{
		innerColl.Clear();
	}

	public bool Contains(Urn urn)
	{
		return innerColl.Contains(urn);
	}

	public void CopyTo(Urn[] array, int arrayIndex)
	{
		innerColl.CopyTo(array, arrayIndex);
	}

	public bool Remove(Urn urn)
	{
		return innerColl.Remove(urn);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)innerColl).GetEnumerator();
	}

	public IEnumerator<Urn> GetEnumerator()
	{
		return innerColl.GetEnumerator();
	}
}
