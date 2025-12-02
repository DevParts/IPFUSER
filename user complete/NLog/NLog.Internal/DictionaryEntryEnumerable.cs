using System;
using System.Collections;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Ensures that IDictionary.GetEnumerator returns DictionaryEntry values
/// </summary>
internal struct DictionaryEntryEnumerable : IEnumerable<DictionaryEntry>, IEnumerable
{
	internal struct DictionaryEntryEnumerator : IEnumerator<DictionaryEntry>, IDisposable, IEnumerator
	{
		private readonly IDictionaryEnumerator _entryEnumerator;

		public DictionaryEntry Current => _entryEnumerator.Entry;

		object IEnumerator.Current => Current;

		public DictionaryEntryEnumerator(IDictionary? dictionary)
		{
			_entryEnumerator = ((dictionary != null && dictionary.Count > 0) ? dictionary.GetEnumerator() : EmptyDictionaryEnumerator.Default);
		}

		public void Dispose()
		{
			if (_entryEnumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}

		public bool MoveNext()
		{
			return _entryEnumerator.MoveNext();
		}

		public void Reset()
		{
			_entryEnumerator.Reset();
		}
	}

	private sealed class EmptyDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		public static readonly IDictionaryEnumerator Default = new EmptyDictionaryEnumerator();

		public DictionaryEntry Entry => default(DictionaryEntry);

		public object? Key => null;

		public object? Value => null;

		object? IEnumerator.Current => Entry;

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}
	}

	private readonly IDictionary? _dictionary;

	public DictionaryEntryEnumerable(IDictionary? dictionary)
	{
		_dictionary = dictionary;
	}

	public DictionaryEntryEnumerator GetEnumerator()
	{
		return new DictionaryEntryEnumerator(_dictionary);
	}

	IEnumerator<DictionaryEntry> IEnumerable<DictionaryEntry>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
