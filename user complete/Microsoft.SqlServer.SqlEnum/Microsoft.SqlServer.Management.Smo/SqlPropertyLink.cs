using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlPropertyLink : ConditionedSql
{
	private enum JoinType
	{
		Classic,
		Inner,
		Left
	}

	private string m_table;

	private string m_filter;

	private string m_alias;

	private bool m_bExpressionIsForTableName;

	private JoinType m_joinType;

	public string Table
	{
		get
		{
			return m_table;
		}
		set
		{
			m_table = value;
		}
	}

	public SqlPropertyLink(XmlReadPropertyLink xrpl)
	{
		SetFields(xrpl.Fields);
		m_table = xrpl.Table;
		m_alias = xrpl.TableAlias;
		m_bExpressionIsForTableName = xrpl.ExpressionIsForTableName;
		if (!m_bExpressionIsForTableName)
		{
			if (m_table != null)
			{
				m_joinType = JoinType.Classic;
			}
			else
			{
				m_table = xrpl.InnerJoin;
				if (m_table != null)
				{
					m_joinType = JoinType.Inner;
				}
				else
				{
					m_table = xrpl.LeftJoin;
					if (m_table != null)
					{
						m_joinType = JoinType.Left;
					}
				}
			}
		}
		m_filter = xrpl.Filter;
		AddLinkMultiple(xrpl.MultipleLink);
	}

	public SqlPropertyLink(XmlReadProperty xrp)
	{
		SetFields(new StringCollection { xrp.Name });
		m_table = xrp.Table;
		m_joinType = JoinType.Classic;
		m_filter = xrp.Link;
	}

	public SqlPropertyLink(XmlReadSettings xrs)
	{
		SetFields(new StringCollection());
		m_table = xrs.MainTable;
		m_joinType = JoinType.Classic;
		m_filter = xrs.AdditionalFilter;
	}

	public static void AddAll(ConditionedSqlList list, XmlReadPropertyLink xrpl)
	{
		do
		{
			list.Add(new SqlPropertyLink(xrpl));
		}
		while (xrpl.Next());
	}

	public static void Add(ConditionedSqlList list, XmlReadProperty xrp)
	{
		if (xrp.HasPropertyLink)
		{
			list.Add(new SqlPropertyLink(xrp));
		}
	}

	public static void Add(ConditionedSqlList list, XmlReadSettings xrs)
	{
		if (xrs.HasPropertyLink)
		{
			list.Add(new SqlPropertyLink(xrs));
		}
	}

	public string GetTableNameWithAlias(SqlObjectBase obj)
	{
		string text = (m_bExpressionIsForTableName ? base.LinkMultiple.GetSqlExpression(obj) : m_table);
		if (m_alias == null)
		{
			return text;
		}
		return text + " AS " + m_alias;
	}

	public string GetFilter(SqlObjectBase obj)
	{
		if (base.LinkMultiple == null || m_bExpressionIsForTableName)
		{
			return m_filter;
		}
		return base.LinkMultiple.GetSqlExpression(obj);
	}

	public override void AddHit(string field, SqlObjectBase obj, StatementBuilder sb)
	{
		obj.AddLinkProperties(LinkFieldType.Local, base.LinkFields);
		string text = GetFilter(obj);
		string tableNameWithAlias = GetTableNameWithAlias(obj);
		if (tableNameWithAlias == null || tableNameWithAlias.Length <= 0)
		{
			sb.AddWhere(text);
			return;
		}
		if (sb.IsFirstJoin)
		{
			if (obj.ParentLink != null)
			{
				text = ((text != null && text.Length > 0) ? ("(" + text + ") AND (" + obj.ParentLink.LinkMultiple.GetSqlExpression(obj) + ")") : obj.ParentLink.LinkMultiple.GetSqlExpression(obj));
			}
			else if (m_joinType == JoinType.Classic)
			{
				sb.AddFrom(tableNameWithAlias);
				if (text != null && text.Length > 0)
				{
					sb.AddWhere(text);
				}
				return;
			}
		}
		string text2 = ((JoinType.Left != m_joinType) ? "INNER JOIN " : "LEFT OUTER JOIN ");
		string value = text2 + tableNameWithAlias + string.Format(CultureInfo.InvariantCulture, " ON {0}", new object[1] { text });
		sb.AddJoin(value);
	}
}
