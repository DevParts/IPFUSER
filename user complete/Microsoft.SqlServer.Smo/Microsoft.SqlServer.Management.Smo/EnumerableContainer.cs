using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class EnumerableContainer : IEnumerable<string>, IEnumerable
{
	private List<IEnumerable> listOfObjects = new List<IEnumerable>();

	internal void Insert(int index, StringCollection stringCollection)
	{
		if (stringCollection == null)
		{
			throw new ArgumentNullException("stringCollection");
		}
		listOfObjects.Insert(index, stringCollection);
	}

	internal void Clear()
	{
		listOfObjects.Clear();
	}

	internal void Add(IEnumerable<string> stringEnumerable)
	{
		if (stringEnumerable == null)
		{
			throw new ArgumentNullException("stringEnumerable");
		}
		listOfObjects.Add(stringEnumerable);
	}

	internal void Add(StringCollection stringCollection)
	{
		Insert(listOfObjects.Count, stringCollection);
	}

	internal static StringCollection IEnumerableToStringCollection(IEnumerable<string> enumerable)
	{
		StringCollection stringCollection = new StringCollection();
		foreach (string item in enumerable)
		{
			stringCollection.Add(item);
		}
		return stringCollection;
	}

	public IEnumerator<string> GetEnumerator()
	{
		EnumeratorContainer enumeratorContainer = new EnumeratorContainer();
		foreach (IEnumerable listOfObject in listOfObjects)
		{
			if (listOfObject is StringCollection stringCollection)
			{
				enumeratorContainer.Add(stringCollection);
			}
			else
			{
				enumeratorContainer.Add((IEnumerable<string>)listOfObject);
			}
		}
		return enumeratorContainer;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
