using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class StatementBuilder
{
	private StringBuilder m_urn;

	private StringBuilder m_prefix;

	private StringBuilder m_fields;

	private StringBuilder m_from;

	private StringBuilder m_where;

	private StringBuilder m_postfix;

	private StringBuilder m_orderBy;

	private ArrayList m_ParentProps;

	private int m_NonTriggeredProps;

	private StringBuilder m_condition;

	private SortedList m_postProcess;

	private bool m_bDistinct;

	private bool m_bStoredPropsAdded;

	private StringBuilder m_InternalSelect;

	private bool bFirstJoinIsClassic;

	internal ArrayList ParentProperties => m_ParentProps;

	internal int NonTriggeredProperties => m_NonTriggeredProps;

	internal SortedList PostProcessList => m_postProcess;

	public bool Distinct
	{
		get
		{
			return m_bDistinct;
		}
		set
		{
			m_bDistinct = value;
		}
	}

	public bool IsFirstJoin => IsEmpty(m_from);

	public StringBuilder From
	{
		get
		{
			return m_from;
		}
		set
		{
			m_from = value;
		}
	}

	public string SqlStatement
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!IsEmpty(m_condition))
			{
				stringBuilder.Append("if ");
				stringBuilder.Append(m_condition);
				stringBuilder.Append("\nbegin\n");
				stringBuilder.Append(GetCreateTemporaryTableSqlConnect("#empty_result"));
				stringBuilder.Append("\n");
				stringBuilder.Append(SelectAndDrop("#empty_result", string.Empty));
				stringBuilder.Append("\nreturn\nend\n");
			}
			if (!IsEmpty(m_prefix))
			{
				stringBuilder.Append(m_prefix);
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(InternalSelect());
			if (!IsEmpty(m_postfix))
			{
				stringBuilder.Append("\n");
				stringBuilder.Append(m_postfix);
			}
			return stringBuilder.ToString();
		}
	}

	public string SqlPostfix => m_postfix.ToString();

	public StatementBuilder()
	{
		m_urn = new StringBuilder();
		m_prefix = new StringBuilder();
		m_fields = new StringBuilder();
		m_from = new StringBuilder();
		m_where = new StringBuilder();
		m_postfix = new StringBuilder();
		m_orderBy = new StringBuilder();
		m_condition = new StringBuilder();
		m_ParentProps = new ArrayList();
		m_postProcess = new SortedList(StringComparer.Ordinal);
		m_NonTriggeredProps = 0;
		Distinct = false;
		m_bStoredPropsAdded = false;
		m_InternalSelect = null;
		bFirstJoinIsClassic = false;
	}

	internal void SetInternalSelect(StringBuilder sql)
	{
		m_InternalSelect = sql;
	}

	public bool IsEmpty(StringBuilder s)
	{
		if (s != null)
		{
			return s.Length <= 0;
		}
		return true;
	}

	protected internal static void AddElement(StringBuilder str, string value, string delimStart, string delimEnd, string delimElems)
	{
		if (str.Length > 0)
		{
			str.Append(delimElems);
		}
		str.Append(delimStart);
		str.Append(value);
		str.Append(delimEnd);
	}

	public void AddUrn(string value)
	{
		AddElement(m_urn, value, string.Empty, string.Empty, "+'/'+");
	}

	public void AddPrefix(string value)
	{
		AddElement(m_prefix, value, string.Empty, "\n", "\n");
	}

	public void AddCondition(string value)
	{
		AddElement(m_condition, value, "(", ")", "or");
	}

	public void AddPostfix(string value)
	{
		AddElement(m_postfix, value, string.Empty, "\n", "\n");
	}

	public void AddFields(string value)
	{
		AddElement(m_fields, value, string.Empty, string.Empty, ",\n");
	}

	public void AddFrom(string value)
	{
		if (IsFirstJoin)
		{
			bFirstJoinIsClassic = true;
		}
		AddElement(m_from, value, string.Empty, string.Empty, ",\n");
	}

	public void AddJoin(string value)
	{
		AddElement(m_from, value, string.Empty, string.Empty, "\n");
	}

	public void AddWhere(string value)
	{
		AddElement(m_where, value, "(", ")", "and");
	}

	public void AddOrderBy(string str)
	{
		AddElement(m_orderBy, str, string.Empty, string.Empty, ",");
	}

	private void AddOrderBy(string orderByValue, OrderBy.Direction dir)
	{
		orderByValue = ((dir != OrderBy.Direction.Asc) ? (orderByValue + " DESC") : (orderByValue + " ASC"));
		AddOrderBy(orderByValue);
	}

	public void AddOrderBy(string prop, string orderByValue, OrderBy.Direction dir)
	{
		foreach (SqlObjectProperty parentProperty in ParentProperties)
		{
			if (prop == parentProperty.Name)
			{
				AddOrderBy(string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { Util.EscapeString(parentProperty.Alias, ']') }), dir);
				return;
			}
		}
		AddOrderBy(orderByValue, dir);
	}

	public void AddProperty(string name, string value)
	{
		if (value != null)
		{
			if (name != null)
			{
				AddFields(string.Format(CultureInfo.InvariantCulture, "{1} AS [{0}]", new object[2]
				{
					Util.EscapeString(name, ']'),
					value
				}));
			}
			else
			{
				AddFields(value);
			}
		}
	}

	public void Merge(StatementBuilder sb)
	{
		if (!IsEmpty(m_prefix))
		{
			sb.AddPrefix(m_prefix.ToString());
		}
		m_prefix = sb.m_prefix;
		if (!IsEmpty(sb.m_fields))
		{
			StringBuilder fields = m_fields;
			m_fields = sb.m_fields;
			if (!IsEmpty(fields))
			{
				AddFields(fields.ToString());
			}
		}
		int num = sb.m_ParentProps.Count - sb.m_NonTriggeredProps;
		m_NonTriggeredProps += sb.NonTriggeredProperties;
		if (num > 0)
		{
			m_ParentProps.InsertRange(0, sb.m_ParentProps.GetRange(0, sb.m_NonTriggeredProps));
			m_ParentProps.InsertRange(m_NonTriggeredProps, sb.m_ParentProps.GetRange(sb.m_NonTriggeredProps, num));
		}
		else
		{
			m_ParentProps.InsertRange(0, sb.m_ParentProps);
		}
		foreach (DictionaryEntry item in sb.m_postProcess)
		{
			m_postProcess[item.Key] = item.Value;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (!IsEmpty(sb.m_from))
		{
			stringBuilder.Append(sb.m_from);
			if (bFirstJoinIsClassic)
			{
				stringBuilder.Append(",");
			}
			stringBuilder.Append("\n");
		}
		if (!IsEmpty(m_from))
		{
			stringBuilder.Append(m_from);
		}
		m_from = stringBuilder;
		bFirstJoinIsClassic = sb.bFirstJoinIsClassic;
		if (!IsEmpty(sb.m_where))
		{
			if (IsEmpty(m_where))
			{
				m_where.Append(sb.m_where);
			}
			else
			{
				AddWhere(sb.m_where.ToString());
			}
		}
		if (!IsEmpty(sb.m_orderBy))
		{
			StringBuilder orderBy = m_orderBy;
			m_orderBy = sb.m_orderBy;
			if (!IsEmpty(orderBy))
			{
				AddOrderBy(orderBy.ToString());
			}
		}
		if (!IsEmpty(sb.m_postfix))
		{
			AddPostfix(sb.m_postfix.ToString());
		}
		m_urn.Append(sb.m_urn);
		if (!IsEmpty(sb.m_condition))
		{
			AddCondition(sb.m_condition.ToString());
		}
	}

	internal StringBuilder InternalSelect()
	{
		if (m_InternalSelect != null)
		{
			return m_InternalSelect;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (IsEmpty(m_fields))
		{
			return stringBuilder;
		}
		stringBuilder.Append("SELECT\n");
		if (m_bDistinct)
		{
			stringBuilder.Append("distinct ");
		}
		stringBuilder.Append(m_fields);
		if (!IsEmpty(m_from))
		{
			stringBuilder.Append("\nFROM\n");
			stringBuilder.Append(m_from);
		}
		if (!IsEmpty(m_where))
		{
			stringBuilder.Append("\nWHERE\n");
			stringBuilder.Append(m_where);
		}
		if (!IsEmpty(m_orderBy))
		{
			stringBuilder.Append("\nORDER BY\n");
			stringBuilder.Append(m_orderBy);
		}
		return stringBuilder;
	}

	internal void ClearPrefixPostfix()
	{
		m_prefix = null;
		m_postfix = null;
		m_condition = null;
	}

	public StatementBuilder MakeCopy()
	{
		StatementBuilder statementBuilder = new StatementBuilder();
		statementBuilder.m_urn.Append(m_urn);
		statementBuilder.m_prefix.Append(m_prefix);
		statementBuilder.m_fields.Append(m_fields);
		statementBuilder.bFirstJoinIsClassic = bFirstJoinIsClassic;
		int num = m_ParentProps.Count - m_NonTriggeredProps;
		if (num > 0)
		{
			statementBuilder.m_ParentProps.InsertRange(statementBuilder.m_NonTriggeredProps, m_ParentProps.GetRange(0, m_NonTriggeredProps));
			statementBuilder.m_ParentProps.AddRange(m_ParentProps.GetRange(m_NonTriggeredProps, num));
		}
		else
		{
			statementBuilder.m_ParentProps.AddRange(m_ParentProps);
		}
		statementBuilder.m_NonTriggeredProps += NonTriggeredProperties;
		foreach (DictionaryEntry item in m_postProcess)
		{
			statementBuilder.m_postProcess[item.Key] = item.Value;
		}
		statementBuilder.m_from.Append(m_from);
		statementBuilder.m_where.Append(m_where);
		statementBuilder.m_orderBy.Append(m_orderBy);
		statementBuilder.m_postfix.Append(m_postfix);
		return statementBuilder;
	}

	internal void StoreParentProperty(SqlObjectProperty sop, bool bTriggered)
	{
		m_ParentProps.Add(sop);
		if (!bTriggered)
		{
			m_NonTriggeredProps++;
		}
	}

	public string GetSqlNoPrefixPostfix()
	{
		AddStoredProperties();
		ClearPrefixPostfix();
		return SqlStatement;
	}

	internal void AddStoredProperties()
	{
		if (m_bStoredPropsAdded)
		{
			return;
		}
		m_bStoredPropsAdded = true;
		foreach (SqlObjectProperty parentProp in m_ParentProps)
		{
			AddProperty(parentProp.Alias, parentProp.SessionValue);
		}
	}

	internal string GetOrderBy()
	{
		return m_orderBy.ToString();
	}

	internal void ClearOrderBy()
	{
		m_orderBy.Length = 0;
	}

	internal string GetCreateTemporaryTableSqlConnect(string tableName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("\ncreate table ");
		stringBuilder.Append(tableName);
		stringBuilder.Append("(");
		bool bFirst = true;
		if (ParentProperties != null)
		{
			foreach (SqlObjectProperty parentProperty in ParentProperties)
			{
				AddColumn(stringBuilder, parentProperty, ref bFirst, bUseAlias: true);
			}
		}
		stringBuilder.Append(")\n");
		return stringBuilder.ToString();
	}

	private void AddColumn(StringBuilder sqlCreate, SqlObjectProperty prop, ref bool bFirst, bool bUseAlias)
	{
		if (bFirst)
		{
			bFirst = false;
		}
		else
		{
			sqlCreate.Append(",");
		}
		string text = prop.Name;
		if (bUseAlias)
		{
			text = prop.Alias;
		}
		if (prop.Size == null)
		{
			sqlCreate.Append(string.Format(CultureInfo.InvariantCulture, "[{0}] {1} NULL", new object[2] { text, prop.DBType }));
		}
		else
		{
			sqlCreate.Append(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}({2}) NULL", new object[3] { text, prop.DBType, prop.Size }));
		}
	}

	internal static string SelectAndDrop(string tableName, string sOrderBy)
	{
		StatementBuilder statementBuilder = new StatementBuilder();
		statementBuilder.AddFields("*");
		statementBuilder.AddFrom(tableName);
		statementBuilder.AddPostfix("drop table " + tableName);
		statementBuilder.AddOrderBy(sOrderBy);
		return statementBuilder.SqlStatement;
	}

	public void ClearFailCondition()
	{
		m_condition = new StringBuilder();
	}
}
