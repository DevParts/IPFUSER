using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseEventArgs : SmoEventArgs
{
	private object innerObject;

	private string databaseName;

	private DatabaseEventType databaseEventType;

	public object SmoObject => innerObject;

	public DatabaseEventType DatabaseEventType => databaseEventType;

	public string Name => databaseName;

	public DatabaseEventArgs(Urn urn, object obj, string name, DatabaseEventType databaseEventType)
		: base(urn)
	{
		innerObject = obj;
		databaseName = name;
		this.databaseEventType = databaseEventType;
	}
}
