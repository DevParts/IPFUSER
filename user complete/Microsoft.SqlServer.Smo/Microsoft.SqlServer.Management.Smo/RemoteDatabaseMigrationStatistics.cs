namespace Microsoft.SqlServer.Management.Smo;

public class RemoteDatabaseMigrationStatistics
{
	public double RemoteDatabaseSizeInMB { get; private set; }

	internal RemoteDatabaseMigrationStatistics()
	{
		RemoteDatabaseSizeInMB = 0.0;
	}

	internal RemoteDatabaseMigrationStatistics(double remoteDatabaseSizeInMB)
	{
		RemoteDatabaseSizeInMB = remoteDatabaseSizeInMB;
	}
}
