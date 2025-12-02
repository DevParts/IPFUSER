using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Smo.UnSafeInternals;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlEnumDependencies : IEnumDependencies
{
	private object m_ci;

	private string m_server;

	private string m_database;

	private SortedList m_tempDependencies;

	private static readonly SqlEnumDependenciesSingleton sqlEnumDependenciesSingleton = new SqlEnumDependenciesSingleton();

	private ServerVersion m_targetVersion;

	private StringComparer svrComparer;

	private StringComparer dbComparer;

	private bool isCloud;

	private bool IsDbCloud
	{
		get
		{
			return isCloud;
		}
		set
		{
			isCloud = value;
		}
	}

	private static SortedList TypeConvertTable
	{
		get
		{
			if (sqlEnumDependenciesSingleton.m_typeConvertTable == null)
			{
				sqlEnumDependenciesSingleton.m_typeConvertTable = new SortedList();
				sqlEnumDependenciesSingleton.m_typeConvertTable[3] = new SqlTypeConvert("Table", 3, "Table");
				sqlEnumDependenciesSingleton.m_typeConvertTable[0] = new SqlTypeConvert("UserDefinedFunction", 0, "UserDefinedFunction");
				sqlEnumDependenciesSingleton.m_typeConvertTable[2] = new SqlTypeConvert("View", 2, "View");
				sqlEnumDependenciesSingleton.m_typeConvertTable[4] = new SqlTypeConvert("StoredProcedure", 4, "StoredProcedure");
				sqlEnumDependenciesSingleton.m_typeConvertTable[6] = new SqlTypeConvert("Default", 6, "Default");
				sqlEnumDependenciesSingleton.m_typeConvertTable[7] = new SqlTypeConvert("Rule", 7, "Rule");
				sqlEnumDependenciesSingleton.m_typeConvertTable[8] = new SqlTypeConvert("Trigger", 8, "Table/Trigger");
				sqlEnumDependenciesSingleton.m_typeConvertTable[11] = new SqlTypeConvert("UserDefinedAggregate", 11, "UserDefinedAggregate");
				sqlEnumDependenciesSingleton.m_typeConvertTable[12] = new SqlTypeConvert("Synonym", 12, "Synonym");
				sqlEnumDependenciesSingleton.m_typeConvertTable[101] = new SqlTypeConvert("UserDefinedDataType", 101, "UserDefinedDataType");
				sqlEnumDependenciesSingleton.m_typeConvertTable[102] = new SqlTypeConvert("XmlSchemaCollection", 102, "XmlSchemaCollection");
				sqlEnumDependenciesSingleton.m_typeConvertTable[103] = new SqlTypeConvert("UserDefinedType", 103, "UserDefinedType");
				sqlEnumDependenciesSingleton.m_typeConvertTable[1000] = new SqlTypeConvert("SqlAssembly", 1000, "SqlAssembly");
				sqlEnumDependenciesSingleton.m_typeConvertTable[201] = new SqlTypeConvert("PartitionScheme", 201, "PartitionScheme");
				sqlEnumDependenciesSingleton.m_typeConvertTable[202] = new SqlTypeConvert("PartitionFunction", 202, "PartitionFunction");
				sqlEnumDependenciesSingleton.m_typeConvertTable[104] = new SqlTypeConvert("UserDefinedTableType", 104, "UserDefinedTableType");
				sqlEnumDependenciesSingleton.m_typeConvertTable[1001] = new SqlTypeConvert("UnresolvedEntity", 1001, "UnresolvedEntity");
				sqlEnumDependenciesSingleton.m_typeConvertTable[203] = new SqlTypeConvert("DdlTrigger", 203, "DdlTrigger");
				sqlEnumDependenciesSingleton.m_typeConvertTable[204] = new SqlTypeConvert("PlanGuide", 204, "PlanGuide");
				sqlEnumDependenciesSingleton.m_typeConvertTable[13] = new SqlTypeConvert("Sequence", 13, "Sequence");
				sqlEnumDependenciesSingleton.m_typeConvertTable[20] = new SqlTypeConvert("SecurityPolicy", 20, "SecurityPolicy");
			}
			return sqlEnumDependenciesSingleton.m_typeConvertTable;
		}
	}

	public SqlEnumDependencies()
	{
		m_tempDependencies = new SortedList();
	}

	private static int TypeToNo(string type)
	{
		switch (type)
		{
		case "Table":
			return 3;
		case "UserDefinedFunction":
			return 0;
		case "View":
			return 2;
		case "StoredProcedure":
			return 4;
		case "Default":
			return 6;
		case "Rule":
			return 7;
		case "Trigger":
			return 8;
		case "UserDefinedAggregate":
			return 11;
		case "Synonym":
			return 12;
		case "UserDefinedDataType":
			return 101;
		case "XmlSchemaCollection":
			return 102;
		case "UserDefinedType":
			return 103;
		case "SqlAssembly":
			return 1000;
		case "PartitionScheme":
			return 201;
		case "PartitionFunction":
			return 202;
		case "UserDefinedTableType":
			return 104;
		case "UnresolvedEntity":
			return 1001;
		case "DdlTrigger":
			return 203;
		case "PlanGuide":
			return 204;
		case "Sequence":
			return 13;
		case "SecurityPolicy":
			return 20;
		default:
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (SqlTypeConvert value in TypeConvertTable.Values)
			{
				if (num++ > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value.Name);
			}
			throw new InternalEnumeratorException(StringSqlEnumerator.UnsupportedTypeDepDiscovery(type, stringBuilder.ToString()));
		}
		}
	}

	public DependencyChainCollection EnumDependencies(object ci, DependencyRequest rd)
	{
		if (SqlContext.IsAvailable)
		{
			throw new Exception(StringSqlEnumerator.SmoSQLCLRUnAvailable);
		}
		m_ci = ci;
		m_targetVersion = ExecuteSql.GetServerVersion(m_ci);
		IsDbCloud = ExecuteSql.GetDatabaseEngineType(ci) == DatabaseEngineType.SqlAzureDatabase;
		SqlRequest sqlRequest = new SqlRequest();
		sqlRequest.ResultType = ResultType.Reserved1;
		Enumerator enumerator = new Enumerator();
		m_server = rd.Urns[0].GetNameForType("Server");
		m_database = rd.Urns[0].GetNameForType("Database");
		if (m_database == null)
		{
			TypeToNo(rd.Urns[0].Type);
			throw new InternalEnumeratorException(StringSqlEnumerator.DatabaseNameMustBeSpecifiedinTheUrn(rd.Urns[0]));
		}
		dbComparer = GetStringCulture(m_database);
		svrComparer = GetStringCulture("MASTER");
		StringCollection stringCollection = new StringCollection();
		if (!IsDbCloud)
		{
			stringCollection.Add(GetUseString(m_database));
		}
		if (m_targetVersion.Major >= 10)
		{
			stringCollection.Add("CREATE TABLE #tempdep (objid int NOT NULL, objname sysname NOT NULL, objschema sysname NULL, objdb sysname NOT NULL, objtype smallint NOT NULL)\n");
		}
		else
		{
			stringCollection.Add("CREATE TABLE #tempdep (objid int NOT NULL, objtype smallint NOT NULL)\n");
		}
		stringCollection.Add("BEGIN TRANSACTION");
		long num = 0L;
		StringBuilder stringBuilder = new StringBuilder();
		int num2 = 1000;
		Urn[] urns = rd.Urns;
		for (int i = 0; i < urns.Length; i++)
		{
			Urn urn = (sqlRequest.Urn = urns[i]);
			string text = urn.Type;
			if (text == "Default" && urn.Parent.Type == "Column")
			{
				text = "DefaultConstraint";
			}
			int num3 = TypeToNo(text);
			if (m_targetVersion.Major >= 10)
			{
				if (num3 == TypeToNo("Trigger") || num3 == TypeToNo("PartitionScheme") || num3 == TypeToNo("PartitionFunction") || num3 == TypeToNo("SqlAssembly") || num3 == TypeToNo("DdlTrigger") || num3 == TypeToNo("PlanGuide"))
				{
					sqlRequest.Fields = new string[2] { "ID", "Name" };
				}
				else
				{
					sqlRequest.Fields = new string[3] { "ID", "Name", "Schema" };
				}
				SqlEnumResult sqlEnumResult = (SqlEnumResult)enumerator.Process(ci, sqlRequest);
				sqlEnumResult.StatementBuilder.AddStoredProperties();
				if (num3 == TypeToNo("Trigger") || num3 == TypeToNo("PartitionScheme") || num3 == TypeToNo("PartitionFunction") || num3 == TypeToNo("SqlAssembly") || num3 == TypeToNo("DdlTrigger") || num3 == TypeToNo("PlanGuide"))
				{
					sqlEnumResult.StatementBuilder.AddFields("null");
				}
				sqlEnumResult.StatementBuilder.AddFields("db_name()");
				sqlEnumResult.StatementBuilder.AddFields(num3.ToString(CultureInfo.InvariantCulture));
				sqlEnumResult.StatementBuilder.AddPrefix("INSERT INTO #tempdep ");
				try
				{
					stringBuilder.Append(sqlEnumResult.GetSingleDatabaseSql());
				}
				catch (InternalEnumeratorException innerException)
				{
					throw new InternalEnumeratorException(StringSqlEnumerator.InvalidUrnForDepends(urn), innerException);
				}
			}
			else
			{
				string attribute = urn.GetAttribute("ID", urn.Type);
				if (attribute != null && attribute.Length > 0)
				{
					stringBuilder.Append("INSERT INTO #tempdep(objid,objtype) VALUES(");
					int.Parse(attribute, CultureInfo.InvariantCulture);
					stringBuilder.Append(attribute);
					stringBuilder.Append(",");
					stringBuilder.Append(num3.ToString());
					stringBuilder.Append(")");
					stringBuilder.AppendLine();
				}
				else
				{
					sqlRequest.Fields = new string[1] { "ID" };
					SqlEnumResult sqlEnumResult2 = (SqlEnumResult)enumerator.Process(ci, sqlRequest);
					sqlEnumResult2.StatementBuilder.AddStoredProperties();
					sqlEnumResult2.StatementBuilder.AddFields(num3.ToString(CultureInfo.InvariantCulture));
					sqlEnumResult2.StatementBuilder.AddPrefix("INSERT INTO #tempdep ");
					try
					{
						stringBuilder.Append(sqlEnumResult2.GetSingleDatabaseSql());
					}
					catch (InternalEnumeratorException innerException2)
					{
						throw new InternalEnumeratorException(StringSqlEnumerator.InvalidUrnForDepends(urn), innerException2);
					}
				}
			}
			num++;
			if (num > 0 && 0 == num % num2)
			{
				stringCollection.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		if (num > 0 && 0 != num % num2)
		{
			stringCollection.Add(stringBuilder.ToString());
		}
		stringBuilder = null;
		stringCollection.Add("COMMIT TRANSACTION");
		string resourceFileName = string.Empty;
		if (IsDbCloud)
		{
			resourceFileName = "CloudDependency.sql";
		}
		else if (m_targetVersion.Major <= 8)
		{
			resourceFileName = "ShilohDependency.sql";
		}
		else if (m_targetVersion.Major == 9)
		{
			resourceFileName = "YukonDependency.sql";
		}
		else if (m_targetVersion.Major == 10)
		{
			resourceFileName = "KatmaiDependency.sql";
		}
		else if (m_targetVersion.Major >= 11)
		{
			resourceFileName = "SQL11Dependency.sql";
		}
		string text2 = string.Format(CultureInfo.InvariantCulture, "declare @find_referencing_objects int\nset @find_referencing_objects = {0}\n", new object[1] { (!rd.ParentDependencies) ? 1 : 0 });
		StreamReader streamReader = new StreamReader(ManagementUtil.LoadResourceFromAssembly(Assembly.GetExecutingAssembly(), resourceFileName));
		text2 += streamReader.ReadToEnd();
		streamReader.Dispose();
		stringCollection.Add(text2);
		DataTable dt = ((!IsDbCloud) ? ExecuteSql.ExecuteWithResults(stringCollection, ci) : ExecuteSql.ExecuteWithResults(stringCollection, ci, m_database));
		DependencyChainCollection result = BuildResult(dt);
		if (!IsDbCloud)
		{
			ExecuteSql.ExecuteImmediate("USE [master]", ci);
		}
		return result;
	}

	private string MakeSqlString(string s)
	{
		return string.Format(CultureInfo.InvariantCulture, "N'{0}'", new object[1] { EscapeString(s, '\'') });
	}

	private string EscapeString(string s, char cEsc)
	{
		if (s == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in s)
		{
			stringBuilder.Append(c);
			if (cEsc == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private StringComparer GetStringCulture(string database)
	{
		StringComparer result = StringComparer.InvariantCultureIgnoreCase;
		StringBuilder stringBuilder = new StringBuilder();
		if (m_targetVersion.Major > 8)
		{
			stringBuilder.AppendFormat("SELECT collation_name FROM sys.databases where name={0}", MakeSqlString(database));
		}
		else
		{
			stringBuilder.AppendFormat("SELECT CAST(DATABASEPROPERTYEX(dtb.name, 'Collation') AS sysname) AS [collation_name] FROM master.dbo.sysdatabases AS dtb WHERE(dtb.name={0})", MakeSqlString(database));
		}
		DataTable dataTable = ExecuteSql.ExecuteWithResults(stringBuilder.ToString(), m_ci);
		if (dataTable.Rows.Count != 0)
		{
			string text = (string)dataTable.Rows[0][0];
			if (text.Contains("_CS_"))
			{
				result = StringComparer.InvariantCulture;
			}
		}
		else
		{
			result = StringComparer.InvariantCulture;
		}
		return result;
	}

	private static SqlTypeConvert FindByNo(int no)
	{
		return (SqlTypeConvert)TypeConvertTable[no];
	}

	private DependencyChainCollection BuildResult(DataTable dt)
	{
		DependencyChainCollection dependencyChainCollection = new DependencyChainCollection();
		DependencyChainCollection dependencyChainCollection2 = new DependencyChainCollection();
		Type typeFromHandle = typeof(DBNull);
		if (m_targetVersion.Major >= 10)
		{
			foreach (DataRow row in dt.Rows)
			{
				ServerDbSchemaName serverDbSchemaName = BuildKey(row, forParent: true);
				Dependency dependency = (Dependency)m_tempDependencies[serverDbSchemaName];
				if (dependency == null)
				{
					string text = null;
					text = BuildUrn(row, forParent: true);
					dependency = new Dependency();
					dependency.Urn = text;
					m_tempDependencies[serverDbSchemaName] = dependency;
					dependencyChainCollection2.Add(dependency);
				}
				dependency.IsSchemaBound = (bool)row["schema_bound"];
				ServerDbSchemaName serverDbSchemaName2 = BuildKey(row, forParent: false);
				if (serverDbSchemaName.CompareTo(serverDbSchemaName2) != 0)
				{
					Dependency dependency2 = (Dependency)m_tempDependencies[serverDbSchemaName2];
					if (dependency2 != null)
					{
						dependency2.Links.Add(dependency);
						continue;
					}
					dependency2 = new Dependency();
					dependency2.Urn = BuildUrn(row, forParent: false);
					m_tempDependencies[serverDbSchemaName2] = dependency2;
					dependencyChainCollection2.Add(dependency2);
					dependency2.Links.Add(dependency);
				}
			}
			dependencyChainCollection2.Reverse();
		}
		else
		{
			foreach (DataRow row2 in dt.Rows)
			{
				IDKey iDKey = BuildIDKey(row2, forParent: false);
				Dependency dependency3 = (Dependency)m_tempDependencies[iDKey];
				if (dependency3 == null)
				{
					string text2 = null;
					text2 = BuildUrn(iDKey.type, row2);
					dependency3 = new Dependency();
					dependency3.Urn = text2;
					m_tempDependencies[iDKey] = dependency3;
					dependencyChainCollection2.Add(dependency3);
				}
				if (typeFromHandle != row2["relative_id"].GetType())
				{
					iDKey = BuildIDKey(row2, forParent: true);
					Dependency dependency4 = (Dependency)m_tempDependencies[iDKey];
					if (dependency4 != null)
					{
						dependency3.Links.Add(dependency4);
						continue;
					}
					dependency3.Links.Add(iDKey);
					dependencyChainCollection.Add(dependency3);
				}
			}
			ResolveDeferredLinks(dependencyChainCollection);
		}
		return dependencyChainCollection2;
	}

	private IDKey BuildIDKey(DataRow row, bool forParent)
	{
		try
		{
			int id = (int)(forParent ? row["relative_id"] : row["object_id"]);
			short type = (short)(forParent ? row["relative_type"] : row["object_type"]);
			return new IDKey(id, type);
		}
		catch (InvalidCastException innerException)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.CouldNotGetInfoFromDependencyRow(DumpRow(row)), innerException);
		}
	}

	private ServerDbSchemaName BuildKey(DataRow row, bool forParent)
	{
		try
		{
			string serverName = (string)(forParent ? row["object_svr"] : row["relative_svr"]);
			string dbName = (string)(forParent ? row["object_db"] : row["relative_db"]);
			string schemaName = (string)(forParent ? row["object_schema"] : row["relative_schema"]);
			string name = (string)(forParent ? row["object_name"] : row["relative_name"]);
			int id = (int)(forParent ? row["object_id"] : row["relative_id"]);
			short type = (short)(forParent ? row["object_type"] : row["relative_type"]);
			return new ServerDbSchemaName(serverName, dbName, schemaName, name, id, type, svrComparer, dbComparer);
		}
		catch (InvalidCastException innerException)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.CouldNotGetInfoFromDependencyRow(DumpRow(row)), innerException);
		}
	}

	private static string DumpRow(DataRow row)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (DataColumn column in row.Table.Columns)
		{
			stringBuilder.Append(column.Caption);
			stringBuilder.Append("(");
			stringBuilder.Append(row[column.Caption].GetType());
			stringBuilder.Append(")");
			stringBuilder.Append(" ");
			stringBuilder.Append(row[column.Caption]);
			stringBuilder.Append(".");
		}
		return stringBuilder.ToString();
	}

	private void ResolveDeferredLinks(DependencyChainCollection deferredLink)
	{
		TraceHelper.Assert(m_targetVersion.Major < 10, "ResolvedDeferredLinks should not be called for version >= Katmai");
		foreach (Dependency item in deferredLink)
		{
			for (int i = 0; i < item.Links.Count; i++)
			{
				if (((ArrayList)item.Links)[i] is IDKey key)
				{
					((ArrayList)item.Links)[i] = m_tempDependencies[key];
				}
			}
		}
	}

	private Urn GetUrnByQuery(IDKey idk)
	{
		SqlTypeConvert sqlTypeConvert = FindByNo(idk.type);
		Request request = new Request();
		request.Fields = new string[1] { "Urn" };
		request.ResultType = ResultType.DataTable;
		if (m_server != null)
		{
			request.Urn = string.Format(CultureInfo.InvariantCulture, "Server[@Name='{0}']/Database[@Name='{1}']/{2}[@ID={3}]", Urn.EscapeString(m_server), Urn.EscapeString(m_database), sqlTypeConvert.Path, idk.id);
		}
		else
		{
			request.Urn = string.Format(CultureInfo.InvariantCulture, "Server/Database[@Name='{0}']/{1}[@ID={2}]", new object[3]
			{
				Urn.EscapeString(m_database),
				sqlTypeConvert.Path,
				idk.id
			});
		}
		Enumerator enumerator = new Enumerator();
		DataTable dataTable = enumerator.Process(m_ci, request);
		return (string)dataTable.Rows[0][0];
	}

	private Urn BuildUrn(int type, DataRow row)
	{
		TraceHelper.Assert(m_targetVersion.Major < 10, "BuildUrn should never be called by server version >= Katmai");
		SqlTypeConvert sqlTypeConvert = FindByNo(type);
		string text = null;
		text = ((m_server == null) ? string.Format(CultureInfo.InvariantCulture, "Server/Database[@Name='{0}']/", new object[1] { Urn.EscapeString(m_database) }) : string.Format(CultureInfo.InvariantCulture, "Server[@Name='{0}']/Database[@Name='{1}']/", new object[2]
		{
			Urn.EscapeString(m_server),
			Urn.EscapeString(m_database)
		}));
		try
		{
			if ("Trigger" == sqlTypeConvert.Name)
			{
				text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}' and @Schema='{2}']/Trigger[@Name='{3}']", FindByNo((short)row["relative_type"]).Name, Urn.EscapeString((string)row["relative_name"]), Urn.EscapeString((string)row["relative_schema"]), Urn.EscapeString((string)row["object_name"]));
				return text;
			}
			if (typeof(DBNull) == row["object_schema"].GetType())
			{
				text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}']", new object[2]
				{
					sqlTypeConvert.Name,
					Urn.EscapeString((string)row["object_name"])
				});
				return text;
			}
			text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}' and @Schema='{2}']", new object[3]
			{
				sqlTypeConvert.Name,
				Urn.EscapeString((string)row["object_name"]),
				Urn.EscapeString((string)row["object_schema"])
			});
		}
		catch (InvalidCastException innerException)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.CouldNotGetInfoFromDependencyRow(DumpRow(row)), innerException);
		}
		return text;
	}

	private Urn BuildUrn(DataRow row, bool forParent)
	{
		TraceHelper.Assert(m_targetVersion.Major >= 10, "BuildUrn should be called by server version >= Katmai only");
		string text = null;
		string text2 = (forParent ? ((string)row["object_svr"]) : ((string)row["relative_svr"]));
		string text3 = (forParent ? ((string)row["object_db"]) : ((string)row["relative_db"]));
		string text4 = (forParent ? ((string)row["object_schema"]) : ((string)row["relative_schema"]));
		string value = (forParent ? ((string)row["object_name"]) : ((string)row["relative_name"]));
		short no = (forParent ? ((short)row["object_type"]) : ((short)row["relative_type"]));
		SqlTypeConvert sqlTypeConvert = FindByNo(no);
		text = ((string.Empty != text2) ? string.Format(CultureInfo.InvariantCulture, "Server[@Name='{0}']/", new object[1] { Urn.EscapeString(text2) }) : ((m_server == null) ? string.Format(CultureInfo.InvariantCulture, "Server/") : string.Format(CultureInfo.InvariantCulture, "Server[@Name='{0}']/", new object[1] { Urn.EscapeString(m_server) })));
		text = ((!(string.Empty != text3)) ? (text + string.Format(CultureInfo.InvariantCulture, "Database[@Name='{0}']/", new object[1] { Urn.EscapeString(m_database) })) : (text + string.Format(CultureInfo.InvariantCulture, "Database[@Name='{0}']/", new object[1] { Urn.EscapeString(text3) })));
		try
		{
			if ("Trigger" == sqlTypeConvert.Name)
			{
				string value2 = (string)row["pschema"];
				string value3 = (string)row["pname"];
				int no2 = (int)row["ptype"];
				text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}' and @Schema='{2}']/Trigger[@Name='{3}']", FindByNo(no2).Name, Urn.EscapeString(value3), Urn.EscapeString(value2), Urn.EscapeString(value));
				return text;
			}
			if (string.Empty == text4)
			{
				text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}']", new object[2]
				{
					sqlTypeConvert.Name,
					Urn.EscapeString(value)
				});
				return text;
			}
			text += string.Format(CultureInfo.InvariantCulture, "{0}[@Name='{1}' and @Schema='{2}']", new object[3]
			{
				sqlTypeConvert.Name,
				Urn.EscapeString(value),
				Urn.EscapeString(text4)
			});
		}
		catch (InvalidCastException innerException)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.CouldNotGetInfoFromDependencyRow(DumpRow(row)), innerException);
		}
		return text;
	}

	private string GetUseString(string name)
	{
		return string.Format(CultureInfo.InvariantCulture, "use [{0}]\n", new object[1] { Util.EscapeString(name, ']') });
	}
}
