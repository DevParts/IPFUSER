namespace Microsoft.SqlServer.Management.Smo;

internal class IdBasedObjectScriptContainer : ObjectScriptContainer
{
	public int ID { get; set; }

	public IdBasedObjectScriptContainer(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
		: base(obj, sp, retryEvent)
	{
	}

	protected override void Initialize(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		ID = (int)obj.Properties.GetValueWithNullReplacement("ID");
		base.Initialize(obj, sp, retryEvent);
	}
}
