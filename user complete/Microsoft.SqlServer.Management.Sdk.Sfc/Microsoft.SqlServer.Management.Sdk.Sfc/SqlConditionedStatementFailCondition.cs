using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
internal class SqlConditionedStatementFailCondition : SqlConditionedStatement
{
	public SqlConditionedStatementFailCondition(XmlReadConditionedStatement xrcs)
		: base(xrcs)
	{
	}

	public static void AddAll(ConditionedSqlList list, XmlReadConditionedStatementFailCondition xrcs)
	{
		if (xrcs != null)
		{
			do
			{
				list.Add(new SqlConditionedStatementFailCondition(xrcs));
			}
			while (xrcs.Next());
		}
	}

	public override void AddHit(string field, SqlObjectBase obj, StatementBuilder sb)
	{
		sb.AddCondition(GetLocalSql(obj));
	}
}
