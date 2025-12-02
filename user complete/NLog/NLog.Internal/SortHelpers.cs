using System;
using System.Collections;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Provides helpers to sort log events and associated continuations.
/// </summary>
internal static class SortHelpers
{
	/// <summary>
	/// Key selector delegate.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="value">Value to extract key information from.</param>
	/// <returns>Key selected from log event.</returns>
	internal delegate TKey KeySelector<in TValue, out TKey>(TValue value);

	/// <summary>
	/// Single-Bucket optimized readonly dictionary. Uses normal internally Dictionary if multiple buckets are needed.
	///
	/// Avoids allocating a new dictionary, when all items are using the same bucket
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public struct ReadOnlySingleBucketGroupBy<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey : notnull
	{
		/// <summary>
		/// Non-Allocating struct-enumerator
		/// </summary>
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			private bool _singleBucketFirstRead;

			private KeyValuePair<TKey, TValue> _singleBucket;

			private readonly IEnumerator<KeyValuePair<TKey, TValue>>? _multiBuckets;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (_multiBuckets != null)
					{
						return _multiBuckets.Current;
					}
					return new KeyValuePair<TKey, TValue>(_singleBucket.Key, _singleBucket.Value);
				}
			}

			object IEnumerator.Current => Current;

			internal Enumerator(Dictionary<TKey, TValue> multiBucket)
			{
				_singleBucketFirstRead = false;
				_singleBucket = default(KeyValuePair<TKey, TValue>);
				_multiBuckets = multiBucket.GetEnumerator();
			}

			internal Enumerator(KeyValuePair<TKey, TValue> singleBucket)
			{
				_singleBucketFirstRead = false;
				_singleBucket = singleBucket;
				_multiBuckets = null;
			}

			public void Dispose()
			{
				if (_multiBuckets != null)
				{
					_multiBuckets.Dispose();
				}
			}

			public bool MoveNext()
			{
				if (_multiBuckets != null)
				{
					return _multiBuckets.MoveNext();
				}
				if (_singleBucketFirstRead)
				{
					return false;
				}
				return _singleBucketFirstRead = true;
			}

			public void Reset()
			{
				if (_multiBuckets != null)
				{
					_multiBuckets.Reset();
				}
				else
				{
					_singleBucketFirstRead = false;
				}
			}
		}

		private KeyValuePair<TKey, TValue>? _singleBucket;

		private readonly Dictionary<TKey, TValue>? _multiBucket;

		private readonly IEqualityComparer<TKey> _comparer;

		public IEqualityComparer<TKey> Comparer => _comparer;

		/// <inheritDoc />
		public int Count => _multiBucket?.Count ?? (_singleBucket.HasValue ? 1 : 0);

		public ReadOnlySingleBucketGroupBy(KeyValuePair<TKey, TValue> singleBucket)
			: this(singleBucket, EqualityComparer<TKey>.Default)
		{
		}

		public ReadOnlySingleBucketGroupBy(Dictionary<TKey, TValue> multiBucket)
			: this(multiBucket, EqualityComparer<TKey>.Default)
		{
		}

		public ReadOnlySingleBucketGroupBy(KeyValuePair<TKey, TValue>? singleBucket, IEqualityComparer<TKey> comparer)
		{
			_comparer = comparer;
			_multiBucket = null;
			_singleBucket = singleBucket;
		}

		public ReadOnlySingleBucketGroupBy(Dictionary<TKey, TValue> multiBucket, IEqualityComparer<TKey> comparer)
		{
			_comparer = comparer;
			_multiBucket = multiBucket;
			_singleBucket = default(KeyValuePair<TKey, TValue>);
		}

		public Enumerator GetEnumerator()
		{
			if (_multiBucket != null)
			{
				return new Enumerator(_multiBucket);
			}
			if (_singleBucket.HasValue)
			{
				return new Enumerator(_singleBucket.Value);
			}
			return new Enumerator(new Dictionary<TKey, TValue>());
		}

		/// <inheritDoc />
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritDoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	/// <summary>
	/// Performs bucket sort (group by) on an array of items and returns a dictionary for easy traversal of the result set.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="inputs">The inputs.</param>
	/// <param name="keySelector">The key selector function.</param>
	/// <returns>
	/// Dictionary where keys are unique input keys, and values are lists of <see cref="T:NLog.Common.AsyncLogEventInfo" />.
	/// </returns>
	public static Dictionary<TKey, List<TValue>> BucketSort<TValue, TKey>(this IEnumerable<TValue> inputs, KeySelector<TValue, TKey> keySelector) where TKey : notnull
	{
		Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>();
		foreach (TValue input in inputs)
		{
			TKey key = keySelector(input);
			if (!dictionary.TryGetValue(key, out var value))
			{
				value = new List<TValue>();
				dictionary.Add(key, value);
			}
			value.Add(input);
		}
		return dictionary;
	}

	/// <summary>
	/// Performs bucket sort (group by) on an array of items and returns a dictionary for easy traversal of the result set.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="inputs">The inputs.</param>
	/// <param name="keySelector">The key selector function.</param>
	/// <returns>
	/// Dictionary where keys are unique input keys, and values are lists of <see cref="T:NLog.Common.AsyncLogEventInfo" />.
	/// </returns>
	public static ReadOnlySingleBucketGroupBy<TKey, IList<TValue>> BucketSort<TValue, TKey>(this IList<TValue> inputs, KeySelector<TValue, TKey> keySelector) where TKey : notnull
	{
		return inputs.BucketSort(keySelector, EqualityComparer<TKey>.Default);
	}

	/// <summary>
	/// Performs bucket sort (group by) on an array of items and returns a dictionary for easy traversal of the result set.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <param name="inputs">The inputs.</param>
	/// <param name="keySelector">The key selector function.</param>
	/// <param name="keyComparer">The key comparer function.</param>
	/// <returns>
	/// Dictionary where keys are unique input keys, and values are lists of <see cref="T:NLog.Common.AsyncLogEventInfo" />.
	/// </returns>
	public static ReadOnlySingleBucketGroupBy<TKey, IList<TValue>> BucketSort<TValue, TKey>(this IList<TValue> inputs, KeySelector<TValue, TKey> keySelector, IEqualityComparer<TKey> keyComparer) where TKey : notnull
	{
		if (inputs.Count == 0)
		{
			return new ReadOnlySingleBucketGroupBy<TKey, IList<TValue>>((KeyValuePair<TKey, IList<TValue>>?)null, keyComparer);
		}
		Dictionary<TKey, IList<TValue>> dictionary = null;
		TKey val = keySelector(inputs[0]);
		for (int i = 1; i < inputs.Count; i++)
		{
			TKey val2 = keySelector(inputs[i]);
			if (dictionary == null)
			{
				if (!keyComparer.Equals(val, val2))
				{
					dictionary = CreateBucketDictionaryWithValue(inputs, keyComparer, i, val, val2);
				}
				continue;
			}
			if (!dictionary.TryGetValue(val2, out var value))
			{
				value = new List<TValue>();
				dictionary.Add(val2, value);
			}
			value.Add(inputs[i]);
		}
		if (dictionary == null)
		{
			return new ReadOnlySingleBucketGroupBy<TKey, IList<TValue>>(new KeyValuePair<TKey, IList<TValue>>(val, inputs), keyComparer);
		}
		return new ReadOnlySingleBucketGroupBy<TKey, IList<TValue>>(dictionary, keyComparer);
	}

	private static Dictionary<TKey, IList<TValue>> CreateBucketDictionaryWithValue<TValue, TKey>(IList<TValue> inputs, IEqualityComparer<TKey> keyComparer, int currentIndex, TKey firstBucketKey, TKey nextBucketKey)
	{
		Dictionary<TKey, IList<TValue>> dictionary = new Dictionary<TKey, IList<TValue>>(keyComparer);
		List<TValue> list = new List<TValue>(currentIndex);
		for (int i = 0; i < currentIndex; i++)
		{
			list.Add(inputs[i]);
		}
		dictionary[firstBucketKey] = list;
		List<TValue> value = new List<TValue> { inputs[currentIndex] };
		dictionary[nextBucketKey] = value;
		return dictionary;
	}
}
