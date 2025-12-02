using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoSet<T>
{
	private Dictionary<T, object> namesList;

	public SmoSet()
	{
		namesList = new Dictionary<T, object>();
	}

	public SmoSet(SmoSet<T> set)
	{
		namesList = new Dictionary<T, object>(set.namesList);
	}

	public bool Add(T name)
	{
		bool result = true;
		try
		{
			namesList.Add(name, null);
		}
		catch (ArgumentException)
		{
			result = false;
		}
		return result;
	}

	public bool Remove(T name)
	{
		bool flag = true;
		try
		{
			return namesList.Remove(name);
		}
		catch (ArgumentNullException)
		{
			return false;
		}
	}

	public bool Contains(T name)
	{
		try
		{
			return namesList.ContainsKey(name);
		}
		catch (ArgumentNullException)
		{
			return false;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		return namesList.Keys.GetEnumerator();
	}
}
