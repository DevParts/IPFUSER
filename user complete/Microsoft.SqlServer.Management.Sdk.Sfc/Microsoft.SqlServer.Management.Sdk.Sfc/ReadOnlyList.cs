using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public struct ReadOnlyList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IReadOnlyCollection, IEnumerable<T>, IEnumerable
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct Enumerable
	{
		public static IEnumerator<T> Empty
		{
			get
			{
				yield break;
			}
		}
	}

	private readonly IList<T> list;

	public int Count
	{
		get
		{
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}
	}

	public T this[int index]
	{
		get
		{
			if (list == null)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return list[index];
		}
	}

	public ReadOnlyList(IList<T> list)
	{
		this.list = list;
	}

	public bool Contains(T item)
	{
		if (list != null)
		{
			return list.Contains(item);
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (list != null)
		{
			list.CopyTo(array, arrayIndex);
		}
	}

	public int IndexOf(T item)
	{
		if (list == null)
		{
			return -1;
		}
		return list.IndexOf(item);
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (list == null)
		{
			return Enumerable.Empty;
		}
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static implicit operator ReadOnlyList<T>(T[] array)
	{
		return new ReadOnlyList<T>(array);
	}

	public static implicit operator ReadOnlyList<T>(List<T> list)
	{
		return new ReadOnlyList<T>(list);
	}
}
