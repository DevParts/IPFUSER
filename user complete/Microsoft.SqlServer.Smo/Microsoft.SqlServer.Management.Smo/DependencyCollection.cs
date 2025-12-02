using System.Collections;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyCollection : IList<DependencyCollectionNode>, ICollection<DependencyCollectionNode>, IEnumerable<DependencyCollectionNode>, IEnumerable
{
	private List<DependencyCollectionNode> innerColl;

	public DependencyCollectionNode this[int index]
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

	bool ICollection<DependencyCollectionNode>.IsReadOnly => ((ICollection<DependencyCollectionNode>)innerColl).IsReadOnly;

	public bool ContainsUrn(Urn urn, Server srv)
	{
		for (int i = 0; i < Count; i++)
		{
			if (srv.CompareUrn(urn, this[i].Urn) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public DependencyCollection()
	{
		innerColl = new List<DependencyCollectionNode>();
	}

	public int IndexOf(DependencyCollectionNode dependencyCollectionNode)
	{
		return innerColl.IndexOf(dependencyCollectionNode);
	}

	public void Insert(int index, DependencyCollectionNode dependencyCollectionNode)
	{
		innerColl.Insert(index, dependencyCollectionNode);
	}

	public void RemoveAt(int index)
	{
		innerColl.RemoveAt(index);
	}

	public void Add(DependencyCollectionNode dependencyCollectionNode)
	{
		innerColl.Add(dependencyCollectionNode);
	}

	public void AddRange(IEnumerable<DependencyCollectionNode> dependencyCollectionNodeCollection)
	{
		innerColl.AddRange(dependencyCollectionNodeCollection);
	}

	public void Clear()
	{
		innerColl.Clear();
	}

	public bool Contains(DependencyCollectionNode dependencyCollectionNode)
	{
		return innerColl.Contains(dependencyCollectionNode);
	}

	public void CopyTo(DependencyCollectionNode[] array, int arrayIndex)
	{
		innerColl.CopyTo(array, arrayIndex);
	}

	public bool Remove(DependencyCollectionNode dependencyCollectionNode)
	{
		return innerColl.Remove(dependencyCollectionNode);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)innerColl).GetEnumerator();
	}

	public IEnumerator<DependencyCollectionNode> GetEnumerator()
	{
		return innerColl.GetEnumerator();
	}
}
