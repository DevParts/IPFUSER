using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class PostProcessWithRowCaching : PostProcess
{
	protected DataRowCollection rowResults;

	private Dictionary<string, bool> dbIsAccessible = new Dictionary<string, bool>(StringComparer.Ordinal);

	protected bool rowsRetrieved;

	protected abstract string SqlQuery { get; }

	protected void GetCachedRowResultsForDatabase(DataProvider dp, string databaseName)
	{
		StringCollection stringCollection = new StringCollection();
		DataTable dataTable = null;
		if (!dbIsAccessible.ContainsKey(databaseName))
		{
			dbIsAccessible[databaseName] = true;
		}
		if (!dbIsAccessible[databaseName] || rowsRetrieved)
		{
			return;
		}
		try
		{
			dbIsAccessible[databaseName] = ExecuteSql.GetIsDatabaseAccessibleNoThrow(base.ConnectionInfo, databaseName);
			if (dbIsAccessible[databaseName])
			{
				stringCollection.Add(SqlQuery);
				dataTable = ExecuteSql.ExecuteWithResults(stringCollection, base.ConnectionInfo, databaseName, poolConnection: false);
			}
		}
		catch (Exception ex)
		{
			if (ex is ConnectionFailureException || ex is ExecutionFailureException)
			{
				rowsRetrieved = false;
				rowResults = null;
				dbIsAccessible[databaseName] = false;
				return;
			}
			throw;
		}
		if (dataTable != null)
		{
			rowResults = dataTable.Rows;
			rowsRetrieved = true;
		}
	}

	public override void CleanRowData()
	{
		rowResults = null;
		rowsRetrieved = false;
		dbIsAccessible.Clear();
	}
}
