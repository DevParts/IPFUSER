using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class DependencyChainCollection : ArrayList
{
	public new Dependency this[int index] => (Dependency)base[index];

	public DependencyChainCollection()
	{
	}

	public DependencyChainCollection(DependencyChainCollection deps)
	{
		for (int i = 0; i < deps.Count; i++)
		{
			Add(deps[i].Copy());
		}
	}

	public void CopyTo(Dependency[] array, int index)
	{
		int num = 0;
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				array.SetValue((Dependency)((DictionaryEntry)enumerator.Current).Value, num++);
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

	public int Add(Dependency value)
	{
		return Add((object)value);
	}

	public void Insert(int index, Dependency value)
	{
		Insert(index, (object)value);
	}

	public int IndexOf(Dependency value)
	{
		return IndexOf((object)value);
	}

	public bool Contains(Dependency value)
	{
		return Contains((object)value);
	}

	public void Remove(Dependency value)
	{
		Remove((object)value);
	}
}
