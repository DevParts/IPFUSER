using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IReadOnlyCollection : IEnumerable
{
	int Count { get; }
}
public interface IReadOnlyCollection<T> : IReadOnlyCollection, IEnumerable<T>, IEnumerable
{
	bool Contains(T item);

	void CopyTo(T[] array, int arrayIndex);
}
