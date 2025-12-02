using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class BatchBlock
{
	private string typeName;

	private int startIndex;

	private PrefetchObjectsFunc prefetchFunc;

	public string TypeName => typeName;

	public int StartIndex
	{
		get
		{
			return startIndex;
		}
		set
		{
			startIndex = value;
		}
	}

	public abstract string FilterConditionText { get; }

	internal BatchBlock(string typeName, PrefetchObjectsFunc prefetchFunc)
	{
		this.typeName = typeName;
		this.prefetchFunc = prefetchFunc;
	}

	public abstract bool TryAdd(Prefetch prefetch, Urn urn);

	public void PrefetchObjects()
	{
		if (prefetchFunc != null)
		{
			prefetchFunc(this);
		}
	}
}
