using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcDependencyDiscoveryObjectSink
{
	SfcDependencyAction Action { get; }

	void Add(SfcDependencyDirection direction, SfcInstance targetObject, SfcTypeRelation relation, bool discovered);

	void Add(SfcDependencyDirection direction, IEnumerator targetObjects, SfcTypeRelation relation, bool discovered);

	void Add<T>(SfcDependencyDirection direction, IEnumerable<T> targetObjects, SfcTypeRelation relation, bool discovered) where T : SfcInstance;
}
