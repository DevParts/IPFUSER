using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IReadOnlyDictionary<K, T> : IReadOnlyCollection<T>, IReadOnlyCollection, IEnumerable<T>, IEnumerable
{
	T this[K key] { get; }

	IEnumerable<K> Keys { get; }

	IEnumerable<T> Values { get; }

	bool ContainsKey(K key);

	bool TryGetValue(K key, out T value);
}
