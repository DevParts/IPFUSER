namespace Microsoft.SqlServer.Management.Smo;

public class RemoteTableMigrationStatistics
{
	public double SizeInKB { get; private set; }

	public long RowCount { get; private set; }

	internal RemoteTableMigrationStatistics()
	{
		SizeInKB = 0.0;
		RowCount = 0L;
	}

	internal RemoteTableMigrationStatistics(double sizeInKB, long rowCount)
	{
		SizeInKB = sizeInKB;
		RowCount = rowCount;
	}
}
