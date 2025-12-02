namespace Microsoft.SqlServer.Management.Smo;

public sealed class EventProperty
{
	private string name;

	private object value;

	public string Name => name;

	public object Value => value;

	internal EventProperty(string name, object value)
	{
		this.name = name;
		this.value = value;
	}
}
