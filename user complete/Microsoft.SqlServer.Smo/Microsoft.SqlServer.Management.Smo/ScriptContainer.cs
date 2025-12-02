namespace Microsoft.SqlServer.Management.Smo;

internal abstract class ScriptContainer
{
	public ScriptFragment CreateScript { get; protected set; }

	public ScriptFragment DropScript { get; protected set; }

	public string DatabaseContext { get; protected set; }
}
