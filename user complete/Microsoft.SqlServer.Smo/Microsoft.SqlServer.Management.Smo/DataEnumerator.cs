using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal class DataEnumerator : IEnumerator<string>, IEnumerator, IDisposable
{
	private enum EnumeratorState
	{
		NotStarted,
		PersistedON,
		IdentityON,
		Data,
		IdentityOFF,
		PersistedOFF,
		Finished
	}

	private SqlDataReader reader;

	private SqlConnection conn;

	private Database database;

	private Dictionary<string, SqlDataType> columnDataType;

	private Dictionary<string, int> columnNumericPrecision;

	private Dictionary<string, int> columnNumericScale;

	private Dictionary<string, string> columnCollation;

	private string tableName;

	private string schemaQualifiedTableName;

	private ScriptingPreferences options;

	private string insertPrefix;

	private string selectCommand;

	private string columnNames;

	private bool hasIdentity;

	private bool hasPersisted;

	private bool hasWritableColumns;

	private string currentScriptString;

	private EnumeratorState state;

	public object Current
	{
		get
		{
			if (state == EnumeratorState.NotStarted || state == EnumeratorState.Finished)
			{
				throw new InvalidOperationException();
			}
			return currentScriptString;
		}
	}

	string IEnumerator<string>.Current
	{
		get
		{
			if (state == EnumeratorState.NotStarted || state == EnumeratorState.Finished)
			{
				throw new InvalidOperationException();
			}
			return currentScriptString;
		}
	}

	private SqlConnection Connection
	{
		get
		{
			if (conn == null)
			{
				SqlConnection sqlConnectionObject = database.ExecutionManager.ConnectionContext.GetDatabaseConnection(database.Name, poolConnection: false).SqlConnectionObject;
				if (sqlConnectionObject.State == ConnectionState.Closed)
				{
					sqlConnectionObject.Open();
				}
				conn = sqlConnectionObject;
			}
			return conn;
		}
	}

	internal DataEnumerator(Table table, ScriptingPreferences options)
	{
		database = table.Parent;
		columnNumericScale = new Dictionary<string, int>(database.StringComparer);
		columnNumericPrecision = new Dictionary<string, int>(database.StringComparer);
		columnCollation = new Dictionary<string, string>(database.StringComparer);
		columnDataType = new Dictionary<string, SqlDataType>(database.StringComparer);
		this.options = options;
		tableName = table.FormatFullNameForScripting(options);
		ScriptingPreferences scriptingPreferences = (ScriptingPreferences)options.Clone();
		scriptingPreferences.IncludeScripts.SchemaQualify = true;
		schemaQualifiedTableName = table.FormatFullNameForScripting(scriptingPreferences);
		hasPersisted = false;
		GetColumnNamesAndSelectSQL(out var columnNameSQL, out var selectSQL, options, table);
		hasWritableColumns = columnNameSQL.Length > 0;
		if (hasWritableColumns)
		{
			columnNames = columnNameSQL.ToString();
			insertPrefix = string.Format(CultureInfo.InvariantCulture, "INSERT {0} ({1}) VALUES (", new object[2]
			{
				tableName,
				columnNameSQL.ToString()
			});
			if (table.IsSupportedProperty("IsMemoryOptimized") && table.IsMemoryOptimized)
			{
				selectCommand = string.Format(CultureInfo.InvariantCulture, "SELECT {0} FROM {1} WITH (SNAPSHOT)", new object[2] { selectSQL, schemaQualifiedTableName });
			}
			else
			{
				selectCommand = string.Format(CultureInfo.InvariantCulture, "SELECT {0} FROM {1}", new object[2] { selectSQL, schemaQualifiedTableName });
			}
		}
	}

	public bool MoveNext()
	{
		bool? flag = null;
		UserOptions userOptions = null;
		switch (state)
		{
		case EnumeratorState.NotStarted:
		{
			bool flag2 = false;
			if (hasWritableColumns)
			{
				SqlCommand sqlCommand = new SqlCommand(selectCommand, Connection);
				reader = sqlCommand.ExecuteReader();
				flag2 = reader.Read();
			}
			if (!flag2)
			{
				currentScriptString = null;
				state = EnumeratorState.Finished;
				flag = false;
			}
			else if (hasPersisted)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET ANSI_PADDING ON");
				state = EnumeratorState.PersistedON;
				flag = true;
			}
			else if (hasIdentity)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT {0} ON {1}", new object[2]
				{
					tableName,
					Environment.NewLine
				});
				state = EnumeratorState.IdentityON;
				flag = true;
			}
			else
			{
				currentScriptString = GetNextInsertStatement();
				state = EnumeratorState.Data;
				flag = true;
			}
			break;
		}
		case EnumeratorState.PersistedON:
			if (hasIdentity)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT {0} ON {1}", new object[2]
				{
					tableName,
					Environment.NewLine
				});
				state = EnumeratorState.IdentityON;
				flag = true;
			}
			else
			{
				currentScriptString = GetNextInsertStatement();
				state = EnumeratorState.Data;
				flag = true;
			}
			break;
		case EnumeratorState.IdentityON:
			currentScriptString = GetNextInsertStatement();
			state = EnumeratorState.Data;
			flag = true;
			break;
		case EnumeratorState.Data:
			if (reader.Read())
			{
				currentScriptString = GetNextInsertStatement();
				state = EnumeratorState.Data;
				flag = true;
				break;
			}
			if (hasIdentity)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT {0} OFF", new object[1] { tableName });
				state = EnumeratorState.IdentityOFF;
				flag = true;
				break;
			}
			if (database.GetServerObject().IsSupportedObject<UserOptions>())
			{
				userOptions = database.GetServerObject().UserOptions;
			}
			if (hasPersisted && userOptions != null && userOptions.IsSupportedProperty("AnsiPadding") && !userOptions.AnsiPadding)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET ANSI_PADDING OFF");
				state = EnumeratorState.PersistedOFF;
				flag = true;
			}
			else
			{
				currentScriptString = null;
				state = EnumeratorState.Finished;
				flag = false;
			}
			break;
		case EnumeratorState.IdentityOFF:
			if (database.GetServerObject().IsSupportedObject<UserOptions>())
			{
				userOptions = database.GetServerObject().UserOptions;
			}
			if (hasPersisted && userOptions != null && userOptions.IsSupportedProperty("AnsiPadding") && !userOptions.AnsiPadding)
			{
				currentScriptString = string.Format(CultureInfo.InvariantCulture, "SET ANSI_PADDING OFF");
				state = EnumeratorState.PersistedOFF;
				flag = true;
			}
			else
			{
				currentScriptString = null;
				state = EnumeratorState.Finished;
				flag = false;
			}
			break;
		case EnumeratorState.PersistedOFF:
			currentScriptString = null;
			state = EnumeratorState.Finished;
			flag = false;
			break;
		case EnumeratorState.Finished:
			flag = false;
			break;
		default:
			TraceHelper.Assert(condition: false, "Bug in dev code");
			throw new Exception("Unknown state");
		}
		if (state == EnumeratorState.Finished)
		{
			CleanUp();
		}
		if (!flag.HasValue)
		{
			TraceHelper.Assert(condition: false, "MoveNext not initialized. Bug in code");
			throw new Exception("MoveNext not initialized. Bug in code");
		}
		return flag.Value;
	}

	public void Reset()
	{
		CleanUp();
		reader = null;
		hasPersisted = false;
		hasIdentity = false;
		state = EnumeratorState.NotStarted;
	}

	public void Dispose()
	{
		CleanUp();
	}

	private void GetColumnNamesAndSelectSQL(out StringBuilder columnNameSQL, out StringBuilder selectSQL, ScriptingPreferences options, Table table)
	{
		columnNameSQL = new StringBuilder();
		selectSQL = new StringBuilder();
		bool flag = true;
		foreach (Column column in table.Columns)
		{
			column.VersionValidate(options);
			StoreDataTypeInformation(column);
			if (column.UnderlyingSqlDataType == SqlDataType.Timestamp || column.Computed)
			{
				if (this.options.IncludeScripts.AnsiPadding && column.ServerVersion.Major > 8 && column.IsPersisted)
				{
					hasPersisted = true;
				}
				continue;
			}
			if (options.Table.Identities && column.Identity)
			{
				hasIdentity = true;
			}
			if (!flag)
			{
				columnNameSQL.Append(", ");
				selectSQL.Append(", ");
			}
			flag = false;
			columnNameSQL.Append(string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { column.Name }));
			selectSQL.Append(FormatValueByTypeForSelect(column));
		}
	}

	private void StoreDataTypeInformation(Column col)
	{
		columnDataType.Add(col.Name, col.UnderlyingSqlDataType);
		switch (col.UnderlyingSqlDataType)
		{
		case SqlDataType.Decimal:
		case SqlDataType.Numeric:
			columnNumericPrecision.Add(col.Name, col.DataType.NumericPrecision);
			columnNumericScale.Add(col.Name, col.DataType.NumericScale);
			break;
		case SqlDataType.Char:
		case SqlDataType.Text:
		case SqlDataType.VarChar:
		case SqlDataType.VarCharMax:
			columnCollation.Add(col.Name, col.Collation);
			break;
		}
	}

	private string GetNextInsertStatement()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string empty = string.Empty;
		bool flag = true;
		DataTable schemaTable = reader.GetSchemaTable();
		int num = 0;
		foreach (DataRow row in schemaTable.Rows)
		{
			empty = string.Empty;
			string text = row[0].ToString();
			if (!string.IsNullOrEmpty(text))
			{
				if (reader.IsDBNull(num))
				{
					empty = "NULL";
				}
				else
				{
					SqlDataType sqlDataType = columnDataType[text];
					if (sqlDataType == SqlDataType.Timestamp)
					{
						num++;
						continue;
					}
					empty = FormatValueByType(text, num);
				}
				if (!string.IsNullOrEmpty(empty))
				{
					if (flag)
					{
						stringBuilder.Append(empty);
						flag = false;
					}
					else
					{
						stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, ", {0}", new object[1] { empty }));
					}
				}
			}
			num++;
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}{1})", new object[2]
		{
			insertPrefix,
			stringBuilder.ToString()
		});
	}

	private string FormatValueByTypeForSelect(Column col)
	{
		string empty = string.Empty;
		switch (col.UnderlyingSqlDataType)
		{
		case SqlDataType.DateTime:
		case SqlDataType.SmallDateTime:
		case SqlDataType.Date:
		case SqlDataType.Time:
		case SqlDataType.DateTimeOffset:
		case SqlDataType.DateTime2:
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { col.Name });
		case SqlDataType.Variant:
			return string.Format(CultureInfo.InvariantCulture, "[{0}], SQL_VARIANT_PROPERTY([{0}], N'basetype'), SQL_VARIANT_PROPERTY([{0}], N'Collation')", new object[1] { col.Name });
		default:
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { col.Name });
		}
	}

	private string FormatSqlVariantValue(int columnIndex)
	{
		string empty = string.Empty;
		object providerSpecificValue = reader.GetProviderSpecificValue(columnIndex);
		string text = reader[columnIndex + 1].ToString().ToLowerInvariant();
		string text2 = string.Empty;
		if (!reader.IsDBNull(columnIndex + 2))
		{
			text2 = reader[columnIndex + 2].ToString();
		}
		switch (text)
		{
		case "bit":
		{
			string text4 = "0";
			if ((SqlBoolean)providerSpecificValue)
			{
				text4 = "1";
			}
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1})", new object[2] { text4, text });
		}
		case "decimal":
		case "numeric":
		{
			SqlDecimal sqlDecimal = (SqlDecimal)providerSpecificValue;
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1}({2},{3}))", providerSpecificValue.ToString(), text, sqlDecimal.Precision, sqlDecimal.Scale);
		}
		case "bigint":
		case "int":
		case "money":
		case "smalldatetime":
		case "smallint":
		case "smallmoney":
		case "tinyint":
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1})", new object[2]
			{
				providerSpecificValue.ToString(),
				text
			});
		case "float":
			return string.Format("CAST({0} AS {1})", ((SqlDouble)providerSpecificValue).Value.ToString("R", GetUsCultureInfo()), text);
		case "real":
			return string.Format("CAST({0} AS {1})", ((SqlSingle)providerSpecificValue).Value.ToString("R", GetUsCultureInfo()), text);
		case "nchar":
		case "nvarchar":
			if (options.IncludeScripts.Collation && !string.IsNullOrEmpty(text2))
			{
				return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1}({2})) COLLATE {3}", SqlSmoObject.MakeSqlString(providerSpecificValue.ToString()), text, providerSpecificValue.ToString().Length, text2);
			}
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1}({2}))", new object[3]
			{
				SqlSmoObject.MakeSqlString(providerSpecificValue.ToString()),
				text,
				providerSpecificValue.ToString().Length
			});
		case "varchar":
		case "char":
		{
			string text3;
			if (!options.IncludeScripts.Collation || string.IsNullOrEmpty(text2))
			{
				string.IsNullOrEmpty(text2);
				text3 = string.Format(CultureInfo.InvariantCulture, "Convert(text, {0})", new object[1] { SqlSmoObject.MakeSqlString(providerSpecificValue.ToString()) });
			}
			else
			{
				text3 = string.Format(CultureInfo.InvariantCulture, "Convert(text, {0} collate {1})", new object[2]
				{
					SqlSmoObject.MakeSqlString(providerSpecificValue.ToString()),
					text2
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1}({2}))", new object[3]
			{
				text3,
				text,
				providerSpecificValue.ToString().Length
			});
		}
		case "uniqueidentifier":
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1})", new object[2]
			{
				SqlSmoObject.MakeSqlString(providerSpecificValue.ToString()),
				text
			});
		case "binary":
		case "varbinary":
			return string.Format(CultureInfo.InvariantCulture, "CAST({0} AS {1})", new object[2]
			{
				ByteArrayToHexString((byte[])providerSpecificValue),
				text
			});
		default:
			throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "sql_variant type {0} is not supported", new object[1] { text }));
		}
	}

	private string FormatValueByType(string columnName, int columnIndex)
	{
		string empty = string.Empty;
		SqlDataType sqlDataType = columnDataType[columnName];
		switch (sqlDataType)
		{
		case SqlDataType.UserDefinedType:
		case SqlDataType.Geometry:
		case SqlDataType.Geography:
			empty = ByteArrayToHexString(reader.GetSqlBinary(columnIndex).Value);
			break;
		case SqlDataType.BigInt:
		case SqlDataType.Int:
		case SqlDataType.SmallInt:
		case SqlDataType.TinyInt:
			empty = reader.GetProviderSpecificValue(columnIndex).ToString();
			break;
		case SqlDataType.Money:
		case SqlDataType.SmallMoney:
			empty = ((SqlMoney)reader.GetProviderSpecificValue(columnIndex)).Value.ToString(GetUsCultureInfo());
			break;
		case SqlDataType.Float:
			empty = ((SqlDouble)reader.GetProviderSpecificValue(columnIndex)).Value.ToString("R", GetUsCultureInfo());
			break;
		case SqlDataType.Real:
			empty = ((SqlSingle)reader.GetProviderSpecificValue(columnIndex)).Value.ToString("R", GetUsCultureInfo());
			break;
		case SqlDataType.Bit:
			empty = "0";
			if ((SqlBoolean)reader.GetProviderSpecificValue(columnIndex))
			{
				empty = "1";
			}
			break;
		case SqlDataType.Decimal:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS Decimal({1}, {2}))", new object[3]
			{
				string.Format(GetUsCultureInfo(), "{0}", new object[1] { reader.GetProviderSpecificValue(columnIndex).ToString() }),
				columnNumericPrecision[columnName],
				columnNumericScale[columnName]
			});
			break;
		case SqlDataType.Numeric:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS Numeric({1}, {2}))", new object[3]
			{
				string.Format(GetUsCultureInfo(), "{0}", new object[1] { reader.GetProviderSpecificValue(columnIndex).ToString() }),
				columnNumericPrecision[columnName],
				columnNumericScale[columnName]
			});
			break;
		case SqlDataType.DateTime:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS DateTime)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetDateTime(columnIndex).ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture)) });
			break;
		case SqlDataType.SmallDateTime:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS SmallDateTime)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetDateTime(columnIndex).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)) });
			break;
		case SqlDataType.DateTime2:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS DateTime2)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetDateTime(columnIndex).ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture)) });
			break;
		case SqlDataType.Date:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS Date)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetDateTime(columnIndex).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) });
			break;
		case SqlDataType.Time:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS Time)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetTimeSpan(columnIndex).ToString()) });
			break;
		case SqlDataType.DateTimeOffset:
			empty = string.Format(CultureInfo.InvariantCulture, "CAST({0} AS DateTimeOffset)", new object[1] { SqlSmoObject.MakeSqlStringForInsert(reader.GetDateTimeOffset(columnIndex).ToString("o", CultureInfo.InvariantCulture)) });
			break;
		case SqlDataType.Char:
		case SqlDataType.Text:
		case SqlDataType.VarChar:
		case SqlDataType.VarCharMax:
		{
			string s = reader.GetProviderSpecificValue(columnIndex).ToString();
			empty = ((options.TargetServerVersion != SqlServerVersion.Version80) ? SqlSmoObject.MakeSqlStringForInsert(s) : (options.IncludeScripts.Collation ? string.Format(CultureInfo.InvariantCulture, "CONVERT(TEXT, {0} COLLATE {1})", new object[2]
			{
				SqlSmoObject.MakeSqlStringForInsert(s),
				columnCollation[columnName]
			}) : string.Format(CultureInfo.InvariantCulture, "CONVERT(TEXT, {0})", new object[1] { SqlSmoObject.MakeSqlStringForInsert(s) })));
			break;
		}
		case SqlDataType.NChar:
		case SqlDataType.NText:
		case SqlDataType.NVarChar:
		case SqlDataType.NVarCharMax:
		case SqlDataType.SysName:
			empty = SqlSmoObject.MakeSqlStringForInsert(reader.GetProviderSpecificValue(columnIndex).ToString());
			break;
		case SqlDataType.UniqueIdentifier:
		case SqlDataType.HierarchyId:
			empty = SqlSmoObject.MakeSqlString(reader.GetProviderSpecificValue(columnIndex).ToString());
			break;
		case SqlDataType.Xml:
			empty = SqlSmoObject.MakeSqlString(((SqlXml)reader.GetProviderSpecificValue(columnIndex)).Value);
			break;
		case SqlDataType.Binary:
		case SqlDataType.Image:
		case SqlDataType.VarBinary:
		case SqlDataType.VarBinaryMax:
			empty = ByteArrayToHexString((byte[])reader[columnIndex]);
			break;
		case SqlDataType.Variant:
			empty = FormatSqlVariantValue(columnIndex);
			break;
		default:
			TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "ERROR: Attempting to script data for type " + sqlDataType);
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.DataScriptingUnsupportedDataTypeException(tableName, columnName, sqlDataType.ToString()));
		}
		return empty;
	}

	private static CultureInfo GetUsCultureInfo()
	{
		return new CultureInfo("en-US");
	}

	private static string ByteArrayToHexString(byte[] binValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("0x");
		foreach (byte b in binValue)
		{
			if (b < 16)
			{
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "0{0:X}", new object[1] { b }));
			}
			else
			{
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0:X}", new object[1] { b }));
			}
		}
		return stringBuilder.ToString();
	}

	private void CleanUp()
	{
		if (reader != null)
		{
			if (!reader.IsClosed)
			{
				reader.Close();
			}
			reader = null;
		}
		if (conn != null)
		{
			conn.Close();
		}
	}
}
