using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
internal class SqlConditionedStatementPostfix : SqlConditionedStatement
{
	public SqlConditionedStatementPostfix(XmlReadConditionedStatement xrcs)
		: base(xrcs)
	{
	}

	public static void AddAll(ConditionedSqlList list, XmlReadConditionedStatementPostfix xrcs)
	{
		if (xrcs != null)
		{
			do
			{
				list.Add(new SqlConditionedStatementPostfix(xrcs));
			}
			while (xrcs.Next());
		}
	}

	public override void AddHit(string field, SqlObjectBase obj, StatementBuilder sb)
	{
		sb.AddPostfix(GetLocalSql(obj));
	}
}
