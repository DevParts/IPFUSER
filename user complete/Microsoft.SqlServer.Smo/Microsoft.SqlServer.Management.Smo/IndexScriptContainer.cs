namespace Microsoft.SqlServer.Management.Smo;

internal class IndexScriptContainer : ObjectScriptContainer
{
	private bool isMemoryOptimized;

	public IndexType IndexType { get; private set; }

	public bool IsMemoryOptimizedIndex => isMemoryOptimized;

	public IndexScriptContainer(Index index, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
		: base(index, sp, retryEvent)
	{
	}

	protected override void Initialize(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		Index index = (Index)obj;
		IndexType = index.InferredIndexType;
		if (index.IsSupportedProperty("IsMemoryOptimized", sp) && index.GetPropValueOptional("IsMemoryOptimized", defaultValue: false))
		{
			isMemoryOptimized = true;
		}
		base.Initialize(obj, sp, retryEvent);
	}
}
