using System;

namespace Microsoft.SqlServer.Management.Smo;

public class RemoteDataArchiveMigrationStatusReport
{
	public string DatabaseName { get; private set; }

	public string TableName { get; private set; }

	public long MigratedRows { get; private set; }

	public DateTime StatusReportStartTimeInUtc { get; private set; }

	public DateTime StatusReportEndTimeInUtc { get; private set; }

	public int ErrorNumber { get; private set; }

	public int ErrorSeverity { get; private set; }

	public int ErrorState { get; private set; }

	public string Details { get; private set; }

	internal RemoteDataArchiveMigrationStatusReport(string databaseName, string tableName, long migratedRows, DateTime statusReportStartTimeInUtc, DateTime statusReportEndTimeInUtc)
	{
		DatabaseName = databaseName;
		TableName = tableName;
		MigratedRows = migratedRows;
		StatusReportStartTimeInUtc = statusReportStartTimeInUtc;
		StatusReportEndTimeInUtc = statusReportEndTimeInUtc;
		ErrorNumber = 0;
		ErrorSeverity = 0;
		ErrorState = 0;
		Details = string.Empty;
	}

	internal RemoteDataArchiveMigrationStatusReport(string databaseName, string tableName, long migratedRows, DateTime statusReportStartTimeInUtc, DateTime statusReportEndTimeInUtc, int? errorNumber, int? errorSeverity, int? errorState, string details)
	{
		DatabaseName = databaseName;
		TableName = tableName;
		MigratedRows = migratedRows;
		StatusReportStartTimeInUtc = statusReportStartTimeInUtc;
		StatusReportEndTimeInUtc = statusReportEndTimeInUtc;
		ErrorNumber = (errorNumber.HasValue ? errorNumber.Value : 0);
		ErrorSeverity = (errorSeverity.HasValue ? errorSeverity.Value : 0);
		ErrorState = (errorState.HasValue ? errorState.Value : 0);
		Details = ((details == null) ? string.Empty : details);
	}
}
