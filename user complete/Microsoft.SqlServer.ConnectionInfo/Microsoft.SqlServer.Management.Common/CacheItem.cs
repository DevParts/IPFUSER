namespace Microsoft.SqlServer.Management.Common;

internal abstract class CacheItem<K> where K : class
{
	private object result;

	private bool hasResult;

	private int executionCount;

	public abstract K Key { get; }

	public object Result
	{
		get
		{
			if (!hasResult)
			{
				return null;
			}
			return result;
		}
		set
		{
			hasResult = true;
			result = value;
		}
	}

	public int ExecutionCount
	{
		get
		{
			return executionCount;
		}
		set
		{
			executionCount = value;
		}
	}

	public CacheItem()
	{
	}

	public bool HasResult()
	{
		return hasResult;
	}

	public void ClearResult()
	{
		hasResult = false;
		result = null;
	}
}
