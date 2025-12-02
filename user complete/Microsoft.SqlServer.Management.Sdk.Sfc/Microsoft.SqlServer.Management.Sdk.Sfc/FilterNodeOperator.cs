using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterNodeOperator : FilterNodeChildren
{
	public new enum Type
	{
		LT,
		GT,
		LE,
		GE,
		EQ,
		NE,
		OR,
		And,
		NEG
	}

	private Type m_opType;

	public override FilterNode.Type NodeType => FilterNode.Type.Operator;

	public Type OpType => m_opType;

	public FilterNode Left => base.Children[0];

	public FilterNode Right => base.Children[1];

	internal FilterNodeOperator(Type opType)
	{
		m_opType = opType;
	}

	public FilterNodeOperator(Type opType, FilterNode left, FilterNode right)
		: base(new FilterNode[2] { left, right })
	{
		m_opType = opType;
	}

	internal static bool Compare(FilterNodeOperator f1, FilterNodeOperator f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		if (f1.OpType != f2.OpType)
		{
			return false;
		}
		return FilterNodeChildren.Compare(f1, f2, compInfo, cultureInfo);
	}

	public static string OpTypeToString(Type type)
	{
		return type switch
		{
			Type.And => " and ", 
			Type.EQ => "=", 
			Type.GE => ">=", 
			Type.GT => ">", 
			Type.LE => "<=", 
			Type.LT => "<", 
			Type.NE => "!=", 
			Type.OR => " or ", 
			_ => string.Empty, 
		};
	}

	public override string ToString()
	{
		return (Type.NEG == OpType) ? $"-{Left.ToString()}" : $"{Left.ToString()}{OpTypeToString(OpType)}{Right.ToString()}";
	}

	public override int GetHashCode()
	{
		return OpType.GetHashCode() ^ base.GetHashCode();
	}
}
