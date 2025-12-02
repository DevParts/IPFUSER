namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcSimpleMap<TKey, TValue>
{
	TValue this[TKey key] { get; }
}
