using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlEnumResult : EnumResult
{
	private DataTable m_databases;

	private DataTable m_SchemaPrefixes;

	private StringCollection m_NameProperties;

	private StringCollection m_SchemaPrefixProperties;

	private bool m_LastDbLevelSet;

	private SortedList m_SpecialQuery;

	private string m_QueryHint;

	private StringCollection m_PostProcessFields;

	private DatabaseEngineType dbType;

	public StringCollection PostProcessFields
	{
		get
		{
			if (m_PostProcessFields == null)
			{
				m_PostProcessFields = new StringCollection();
			}
			return m_PostProcessFields;
		}
	}

	public StringCollection NameProperties
	{
		get
		{
			if (m_NameProperties == null)
			{
				m_NameProperties = new StringCollection();
			}
			return m_NameProperties;
		}
		set
		{
			m_NameProperties = value;
		}
	}

	public StringCollection SchemaPrefixProperties
	{
		get
		{
			if (m_SchemaPrefixProperties == null)
			{
				m_SchemaPrefixProperties = new StringCollection();
			}
			return m_SchemaPrefixProperties;
		}
		set
		{
			m_SchemaPrefixProperties = value;
		}
	}

	internal SortedList SpecialQuery => m_SpecialQuery;

	internal string QueryHint => m_QueryHint;

	public bool LastDbLevelSet
	{
		get
		{
			return m_LastDbLevelSet;
		}
		set
		{
			m_LastDbLevelSet = value;
		}
	}

	public StatementBuilder StatementBuilder
	{
		get
		{
			return (StatementBuilder)base.Data;
		}
		set
		{
			base.Data = value;
		}
	}

	public int Level
	{
		get
		{
			if (m_databases == null)
			{
				return 0;
			}
			if (0 >= m_databases.Rows.Count)
			{
				return 1;
			}
			return m_databases.Columns.Count;
		}
	}

	public DataTable Databases
	{
		get
		{
			return m_databases;
		}
		set
		{
			m_databases = value;
		}
	}

	public DataTable SchemaPrefixes
	{
		get
		{
			return m_SchemaPrefixes;
		}
		set
		{
			m_SchemaPrefixes = value;
		}
	}

	internal bool MultipleDatabases
	{
		get
		{
			if (m_databases != null)
			{
				return m_databases.Rows.Count != 1;
			}
			return false;
		}
	}

	public SqlEnumResult(object ob, ResultType resultType, DatabaseEngineType dbType)
		: base(ob, resultType)
	{
		m_LastDbLevelSet = false;
		this.dbType = dbType;
	}

	internal void AddSpecialQuery(string database, string query)
	{
		if (m_SpecialQuery == null)
		{
			m_SpecialQuery = new SortedList(StringComparer.Ordinal);
		}
		m_SpecialQuery.Add(database, query);
	}

	internal void AddQueryHint(string hint)
	{
		m_QueryHint = hint;
	}

	private string GetSql(DataRow dbs, string sql)
	{
		if (!LastDbLevelSet)
		{
			return Level switch
			{
				1 => string.Format(CultureInfo.InvariantCulture, sql, new object[1] { "db_name()" }), 
				2 => string.Format(CultureInfo.InvariantCulture, sql, new object[3]
				{
					"'" + Util.EscapeString(dbs[0].ToString(), '\'') + "'",
					Util.EscapeString(dbs[0].ToString(), ']'),
					"db_name()"
				}), 
				_ => throw new InternalEnumeratorException(StringSqlEnumerator.TooManyDbLevels), 
			};
		}
		return Level switch
		{
			1 => sql, 
			2 => string.Format(CultureInfo.InvariantCulture, sql, new object[2]
			{
				"'" + Util.EscapeString(dbs[0].ToString(), '\'') + "'",
				Util.EscapeString(dbs[0].ToString(), ']')
			}), 
			_ => throw new InternalEnumeratorException(StringSqlEnumerator.TooManyDbLevels), 
		};
	}

	private string GetUse(DataRow dbs)
	{
		return "use " + string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { Util.EscapeString(dbs[Level - 1].ToString(), ']') });
	}

	private string SubstituteSchemaPrefix(DataRow dbs, string sql)
	{
		if (dbs == null)
		{
			return sql;
		}
		return sql.Replace("[SchemaPrefix]", string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { Util.EscapeString(dbs[Level - 1].ToString(), ']') }));
	}

	private bool IsDatabaseListEqual(DataRow db1, DataRow db2)
	{
		for (int i = 0; i < Level - 1; i++)
		{
			if (db1[i].ToString() != db2[i].ToString())
			{
				return false;
			}
		}
		return true;
	}

	public StringCollection BuildSql()
	{
		StatementBuilder.AddStoredProperties();
		StringCollection stringCollection = new StringCollection();
		string queryPrefix = QueryIsolation.GetQueryPrefix();
		if (!string.IsNullOrEmpty(queryPrefix))
		{
			stringCollection.Add(queryPrefix);
		}
		if (0 >= Level)
		{
			stringCollection.Add(StatementBuilder.SqlStatement);
		}
		else
		{
			string sOrderBy = string.Empty;
			string text = null;
			if (MultipleDatabases)
			{
				text = "[#unify_temptbl" + (DateTime.Now - DateTime.MinValue).TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + "]";
				stringCollection.Add(StatementBuilder.GetCreateTemporaryTableSqlConnect(text));
				StatementBuilder.AddPrefix(" insert into " + text);
				sOrderBy = StatementBuilder.GetOrderBy();
				StatementBuilder.ClearOrderBy();
			}
			if (!string.IsNullOrEmpty(m_QueryHint))
			{
				StatementBuilder.AddQueryHint(m_QueryHint);
			}
			string sqlStatement = StatementBuilder.SqlStatement;
			DataRow dataRow = null;
			string text2 = null;
			bool flag = false;
			for (int i = 0; i < m_databases.Rows.Count; i++)
			{
				DataRow dataRow2 = m_databases.Rows[i];
				DataRow dbs = null;
				if (m_SchemaPrefixes != null && i < m_SchemaPrefixes.Rows.Count)
				{
					dbs = m_SchemaPrefixes.Rows[i];
				}
				if (dbType != DatabaseEngineType.SqlAzureDatabase)
				{
					stringCollection.Add(GetUse(dataRow2));
				}
				if (m_SpecialQuery != null && 1 == m_SpecialQuery.Count && 1 == Level && string.Compare((string)m_SpecialQuery.GetKey(0), (string)dataRow2[Level - 1], StringComparison.OrdinalIgnoreCase) == 0)
				{
					StatementBuilder statementBuilder = StatementBuilder.MakeCopy();
					statementBuilder.AddWhere((string)m_SpecialQuery[dataRow2[Level - 1]]);
					text2 = GetSql(dataRow2, statementBuilder.SqlStatement);
					dataRow = dataRow2;
					flag = true;
				}
				else if (dataRow == null || flag || !IsDatabaseListEqual(dataRow, dataRow2))
				{
					text2 = GetSql(dataRow2, sqlStatement);
					dataRow = dataRow2;
					flag = false;
				}
				text2 = SubstituteSchemaPrefix(dbs, text2);
				stringCollection.Add(text2);
			}
			if (MultipleDatabases)
			{
				stringCollection.Add(StatementBuilder.SelectAndDrop(text, sOrderBy));
			}
		}
		queryPrefix = QueryIsolation.GetQueryPostfix();
		if (!string.IsNullOrEmpty(queryPrefix))
		{
			stringCollection[stringCollection.Count - 1] = $"{stringCollection[stringCollection.Count - 1]}\n{queryPrefix}";
		}
		return stringCollection;
	}

	internal string GetSingleDatabaseSql()
	{
		if (m_databases == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.NotDbObject);
		}
		if (1 != m_databases.Rows.Count)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.NotSingleDb);
		}
		return GetSql(m_databases.Rows[0], StatementBuilder.SqlStatement);
	}
}
