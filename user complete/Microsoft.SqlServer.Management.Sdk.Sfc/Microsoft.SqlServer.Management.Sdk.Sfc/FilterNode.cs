using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public abstract class FilterNode
{
	public enum Type
	{
		Attribute,
		Constant,
		Operator,
		Function,
		Group
	}

	public abstract Type NodeType { get; }

	public static bool Compare(FilterNode f1, FilterNode f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		if (f1 == null && f2 == null)
		{
			return true;
		}
		if (f1 == null || f2 == null)
		{
			return false;
		}
		if (f1.NodeType != f2.NodeType)
		{
			return false;
		}
		return f1.NodeType switch
		{
			Type.Attribute => FilterNodeAttribute.Compare((FilterNodeAttribute)f1, (FilterNodeAttribute)f2), 
			Type.Constant => FilterNodeConstant.Compare((FilterNodeConstant)f1, (FilterNodeConstant)f2, compInfo, cultureInfo), 
			Type.Operator => FilterNodeOperator.Compare((FilterNodeOperator)f1, (FilterNodeOperator)f2, compInfo, cultureInfo), 
			Type.Function => FilterNodeFunction.Compare((FilterNodeFunction)f1, (FilterNodeFunction)f2, compInfo, cultureInfo), 
			Type.Group => FilterNodeGroup.Compare((FilterNodeGroup)f1, (FilterNodeGroup)f2, compInfo, cultureInfo), 
			_ => throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnknowNodeType), 
		};
	}

	public override string ToString()
	{
		return string.Empty;
	}
}
