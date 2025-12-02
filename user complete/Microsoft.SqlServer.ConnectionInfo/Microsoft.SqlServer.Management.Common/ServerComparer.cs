using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Common;

public sealed class ServerComparer : IComparer<string>
{
	private const int SHILOH = 8;

	private CompareOptions compareOps;

	private CultureInfo cultureInfo;

	private DatabaseNameEqualityComparer databaseNameEqualityComparer;

	internal CultureInfo CultureInfo => cultureInfo;

	internal CompareOptions CompareOptions => compareOps;

	public IEqualityComparer<string> DatabaseNameEqualityComparer => databaseNameEqualityComparer ?? (databaseNameEqualityComparer = new DatabaseNameEqualityComparer(this));

	public ServerComparer(ServerConnection conn)
		: this(conn, null)
	{
	}

	public ServerComparer(ServerConnection conn, string databaseName)
	{
		if (conn != null && !conn.IsForceDisconnected)
		{
			int num;
			int num2;
			if (conn.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && databaseName != null && databaseName.Equals("master", StringComparison.OrdinalIgnoreCase))
			{
				num = 1033;
				num2 = 196609;
			}
			else
			{
				string cmdText = "SELECT COLLATIONPROPERTY((select collation_name from sys.databases where name = ISNULL(@dbname, db_name())), 'LCID'), COLLATIONPROPERTY((select collation_name from sys.databases where name = ISNULL(@dbname, db_name())), 'ComparisonStyle')";
				if (conn.ServerVersion.Major <= 8)
				{
					cmdText = "SELECT COLLATIONPROPERTY(\r\n                (select\r\n                CAST(DATABASEPROPERTYEX(name, 'Collation') as nvarchar(255))\r\n                from master.dbo.sysdatabases\r\n                where name = ISNULL(@dbname, db_name())\r\n                ),\r\n                'LCID'\r\n                ),\r\n                COLLATIONPROPERTY(\r\n                (select\r\n                CAST(DATABASEPROPERTYEX(name, 'Collation') as nvarchar(255))\r\n                from master.dbo.sysdatabases\r\n                where name = ISNULL(@dbname, db_name())\r\n                ),\r\n                'ComparisonStyle'\r\n                )";
				}
				SqlCommand sqlCommand = new SqlCommand(cmdText)
				{
					Parameters = 
					{
						new SqlParameter("@dbname", SqlDbType.NVarChar)
					}
				};
				if (!string.IsNullOrEmpty(databaseName))
				{
					sqlCommand.Parameters["@dbname"].Value = databaseName;
				}
				else
				{
					sqlCommand.Parameters["@dbname"].Value = DBNull.Value;
				}
				SqlDataReader sqlDataReader = null;
				SqlExecutionModes sqlExecutionModes = conn.SqlExecutionModes;
				ServerConnection serverConnection = null;
				try
				{
					conn.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
					try
					{
						sqlDataReader = conn.ExecuteReader(sqlCommand);
					}
					catch (InvalidOperationException)
					{
						serverConnection = conn.Copy();
						sqlDataReader = serverConnection.ExecuteReader(sqlCommand);
					}
					sqlDataReader.Read();
					num = sqlDataReader.GetInt32(0);
					if (num == 66577)
					{
						num = 1041;
					}
					num2 = sqlDataReader.GetInt32(1);
				}
				finally
				{
					sqlDataReader?.Dispose();
					serverConnection?.Disconnect();
					conn.PoolDisconnect();
					conn.SqlExecutionModes = sqlExecutionModes;
				}
			}
			cultureInfo = NetCoreHelpers.GetNewCultureInfo(num);
			if (num2 == 0)
			{
				compareOps = CompareOptions.Ordinal;
				return;
			}
			if ((num2 & 1) != 0)
			{
				compareOps |= CompareOptions.IgnoreCase;
			}
			if ((num2 & 2) != 0)
			{
				compareOps |= CompareOptions.IgnoreNonSpace;
			}
			if ((num2 & 0x10000) != 0)
			{
				compareOps |= CompareOptions.IgnoreKanaType;
			}
			if ((num2 & 0x20000) != 0)
			{
				compareOps |= CompareOptions.IgnoreWidth;
			}
		}
		else
		{
			cultureInfo = CultureInfo.InvariantCulture;
		}
	}

	int IComparer<string>.Compare(string x, string y)
	{
		return cultureInfo.CompareInfo.Compare(x, y, compareOps);
	}
}
