using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal abstract class SqlConditionedStatement : ConditionedSql
{
	private string m_sql;

	protected SqlConditionedStatement(XmlReadConditionedStatement xrcs)
	{
		SetFields(xrcs.Fields);
		m_sql = xrcs.Sql;
		AddLinkMultiple(xrcs.MultipleLink);
	}

	public string GetLocalSql(SqlObjectBase obj)
	{
		if (m_sql != null)
		{
			return m_sql;
		}
		return base.LinkMultiple.GetSqlExpression(obj);
	}
}
