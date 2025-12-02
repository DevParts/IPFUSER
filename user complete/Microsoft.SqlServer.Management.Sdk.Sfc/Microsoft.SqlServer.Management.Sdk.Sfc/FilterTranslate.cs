using System.Collections;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class FilterTranslate
{
	private static FilterNodeOperator.Type XPathOpToFilterOp(Operator.Op op)
	{
		return op switch
		{
			Operator.Op.LT => FilterNodeOperator.Type.LT, 
			Operator.Op.GT => FilterNodeOperator.Type.GT, 
			Operator.Op.LE => FilterNodeOperator.Type.LE, 
			Operator.Op.GE => FilterNodeOperator.Type.GE, 
			Operator.Op.EQ => FilterNodeOperator.Type.EQ, 
			Operator.Op.NE => FilterNodeOperator.Type.NE, 
			Operator.Op.OR => FilterNodeOperator.Type.OR, 
			Operator.Op.AND => FilterNodeOperator.Type.And, 
			Operator.Op.NEGATE => FilterNodeOperator.Type.NEG, 
			_ => throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnknownOperator), 
		};
	}

	private static FilterNodeFunction.Type XPathFuncToFilterFunction(Function.FunctionType tfunc)
	{
		return tfunc switch
		{
			Function.FunctionType.FuncTrue => FilterNodeFunction.Type.True, 
			Function.FunctionType.FuncFalse => FilterNodeFunction.Type.False, 
			Function.FunctionType.FuncString => FilterNodeFunction.Type.String, 
			Function.FunctionType.FuncContains => FilterNodeFunction.Type.Contains, 
			Function.FunctionType.FuncNot => FilterNodeFunction.Type.Not, 
			Function.FunctionType.FuncBoolean => FilterNodeFunction.Type.Boolean, 
			Function.FunctionType.FuncLike => FilterNodeFunction.Type.Like, 
			Function.FunctionType.FuncIn => FilterNodeFunction.Type.In, 
			Function.FunctionType.FuncUserDefined => FilterNodeFunction.Type.UserDefined, 
			_ => throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnknownFunction), 
		};
	}

	public static FilterNode decode(AstNode node)
	{
		if (node == null)
		{
			return null;
		}
		switch (node.TypeOfAst)
		{
		case AstNode.QueryType.Filter:
		{
			Filter filter = (Filter)node;
			return decode(filter.Condition);
		}
		case AstNode.QueryType.Operator:
		{
			Operator obj2 = (Operator)node;
			FilterNodeOperator filterNodeOperator = new FilterNodeOperator(XPathOpToFilterOp(obj2.OperatorType));
			filterNodeOperator.Add(decode(obj2.Operand1));
			filterNodeOperator.Add(decode(obj2.Operand2));
			return filterNodeOperator;
		}
		case AstNode.QueryType.ConstantOperand:
		{
			Operand operand = (Operand)node;
			if (operand.ReturnType == AstNode.RType.Number)
			{
				return new FilterNodeConstant(operand.OperandValue, FilterNodeConstant.ObjectType.Number);
			}
			if (AstNode.RType.String == operand.ReturnType)
			{
				return new FilterNodeConstant(operand.OperandValue, FilterNodeConstant.ObjectType.String);
			}
			if (AstNode.RType.Boolean == operand.ReturnType)
			{
				return new FilterNodeConstant(operand.OperandValue, FilterNodeConstant.ObjectType.Boolean);
			}
			throw new InvalidQueryExpressionEnumeratorException(SfcStrings.VariablesNotSupported);
		}
		case AstNode.QueryType.Group:
		{
			Group obj = (Group)node;
			FilterNodeGroup filterNodeGroup = new FilterNodeGroup();
			filterNodeGroup.Add(decode(obj.GroupNode));
			return filterNodeGroup;
		}
		case AstNode.QueryType.Axis:
			return decode((Axis)node);
		case AstNode.QueryType.Function:
			return decode((Function)node);
		default:
			throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnknownElemType);
		}
	}

	private static FilterNode decode(Axis ax)
	{
		if (Axis.AxisType.Attribute == ax.TypeOfAxis)
		{
			return new FilterNodeAttribute(ax.Name);
		}
		if (Axis.AxisType.Child == ax.TypeOfAxis)
		{
			throw new InvalidQueryExpressionEnumeratorException(SfcStrings.ChildrenNotSupported);
		}
		throw new InvalidQueryExpressionEnumeratorException(SfcStrings.UnsupportedExpresion);
	}

	private static FilterNode decode(Function func)
	{
		FilterNodeFunction filterNodeFunction = new FilterNodeFunction(XPathFuncToFilterFunction(func.TypeOfFunction), func.Name);
		ArrayList argumentList = func.ArgumentList;
		for (int i = 0; i < argumentList.Count; i++)
		{
			filterNodeFunction.Add(decode((AstNode)argumentList[i]));
		}
		return filterNodeFunction;
	}
}
