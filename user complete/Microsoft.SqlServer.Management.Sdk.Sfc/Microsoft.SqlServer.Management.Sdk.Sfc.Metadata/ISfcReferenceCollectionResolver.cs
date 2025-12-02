using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

public interface ISfcReferenceCollectionResolver
{
	IEnumerable ResolveCollection(object instance, object[] args);
}
public interface ISfcReferenceCollectionResolver<T, S> : ISfcReferenceCollectionResolver
{
	IEnumerable<T> ResolveCollection(S instance, object[] args);
}
