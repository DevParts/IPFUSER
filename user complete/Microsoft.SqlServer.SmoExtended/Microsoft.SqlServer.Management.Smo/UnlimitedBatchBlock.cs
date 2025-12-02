using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class UnlimitedBatchBlock : BatchBlock
{
	public override string FilterConditionText => string.Empty;

	internal UnlimitedBatchBlock(string typeName, PrefetchObjectsFunc prefetchFunc)
		: base(typeName, prefetchFunc)
	{
	}

	public override bool TryAdd(Prefetch prefetch, Urn urn)
	{
		return true;
	}
}
