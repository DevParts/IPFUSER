using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface IReadOnlySet : IReadOnlyCollection, IEnumerable
{
	bool IsSubsetOf(IEnumerable other);

	bool IsSupersetOf(IEnumerable other);

	bool IsProperSubsetOf(IEnumerable other);

	bool IsProperSupersetOf(IEnumerable other);

	bool Overlaps(IEnumerable other);

	bool SetEquals(IEnumerable other);
}
public interface IReadOnlySet<T> : IReadOnlySet, IReadOnlyCollection<T>, IReadOnlyCollection, IEnumerable<T>, IEnumerable
{
	bool IsSubsetOf(IEnumerable<T> other);

	bool IsSupersetOf(IEnumerable<T> other);

	bool IsProperSubsetOf(IEnumerable<T> other);

	bool IsProperSupersetOf(IEnumerable<T> other);

	bool Overlaps(IEnumerable<T> other);

	bool SetEquals(IEnumerable<T> other);
}
