using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseMapping
{
	private string loginName;

	private string dbName;

	private string userName;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string LoginName => loginName;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DBName => dbName;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string UserName => userName;

	public DatabaseMapping(string loginName, string dbName, string userName)
	{
		this.loginName = loginName;
		this.dbName = dbName;
		this.userName = userName;
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "Login={0};Database={1};User={2}", new object[3]
		{
			SqlSmoObject.MakeSqlBraket(LoginName),
			SqlSmoObject.MakeSqlBraket(DBName),
			SqlSmoObject.MakeSqlBraket(UserName)
		});
	}
}
