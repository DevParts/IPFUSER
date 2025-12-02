namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

public interface ISfcReferenceResolver
{
	object Resolve(object instance, object[] args);
}
public interface ISfcReferenceResolver<T, S> : ISfcReferenceResolver
{
	T Resolve(S instance, object[] args);
}
