using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class SqlConditionedStatementPrefix : SqlConditionedStatement
{
	public SqlConditionedStatementPrefix(XmlReadConditionedStatement xrcs)
		: base(xrcs)
	{
	}

	public static void AddAll(ConditionedSqlList list, XmlReadConditionedStatementPrefix xrcs)
	{
		if (xrcs != null)
		{
			do
			{
				list.Add(new SqlConditionedStatementPrefix(xrcs));
			}
			while (xrcs.Next());
		}
	}

	public override void AddHit(string field, SqlObjectBase obj, StatementBuilder sb)
	{
		sb.AddPrefix(GetLocalSql(obj));
	}
}
