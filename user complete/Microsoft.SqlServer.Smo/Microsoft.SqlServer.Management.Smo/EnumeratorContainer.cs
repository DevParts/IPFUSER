using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class EnumeratorContainer : IEnumerator<string>, IDisposable, IEnumerator
{
	private enum EnumeratorState
	{
		NotStarted,
		InProgress,
		Finished
	}

	private List<IEnumerable> listOfObjects = new List<IEnumerable>();

	private IEnumerator currentEnumerator;

	private int indexOfObject = -1;

	private EnumeratorState state;

	public string Current
	{
		get
		{
			if (state == EnumeratorState.NotStarted)
			{
				throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
			}
			if (state == EnumeratorState.Finished)
			{
				throw new InvalidOperationException("Enumeration already finished.");
			}
			return (string)currentEnumerator.Current;
		}
	}

	object IEnumerator.Current => Current;

	internal EnumeratorContainer()
	{
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
		if (stringCollection == null)
		{
			throw new ArgumentNullException("stringCollection");
		}
		listOfObjects.Add(stringCollection);
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		if (state == EnumeratorState.NotStarted)
		{
			state = EnumeratorState.InProgress;
		}
		while (currentEnumerator == null || !currentEnumerator.MoveNext())
		{
			indexOfObject++;
			if (indexOfObject == listOfObjects.Count)
			{
				state = EnumeratorState.Finished;
				return false;
			}
			currentEnumerator = listOfObjects[indexOfObject].GetEnumerator();
		}
		return true;
	}

	public void Reset()
	{
		indexOfObject = -1;
		currentEnumerator = null;
		state = EnumeratorState.NotStarted;
	}
}
