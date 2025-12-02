namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Operator : AstNode
{
	internal enum Op
	{
		PLUS = 1,
		MINUS,
		MUL,
		MOD,
		DIV,
		NEGATE,
		LT,
		GT,
		LE,
		GE,
		EQ,
		NE,
		OR,
		AND,
		UNION,
		INVALID
	}

	private string[] str = new string[15]
	{
		"+", "-", "multiply", "mod", "divde", "negate", "<", ">", "<=", ">=",
		"=", "!=", "or", "and", "union"
	};

	private Op _operatorType;

	private AstNode _opnd1;

	private AstNode _opnd2;

	internal override QueryType TypeOfAst => QueryType.Operator;

	internal override RType ReturnType
	{
		get
		{
			if (_operatorType < Op.LT)
			{
				return RType.Number;
			}
			if (_operatorType < Op.UNION)
			{
				return RType.Boolean;
			}
			return RType.NodeSet;
		}
	}

	internal Op OperatorType => _operatorType;

	internal AstNode Operand1 => _opnd1;

	internal AstNode Operand2 => _opnd2;

	internal override double DefaultPriority
	{
		get
		{
			if (_operatorType == Op.UNION)
			{
				double defaultPriority = _opnd1.DefaultPriority;
				double defaultPriority2 = _opnd2.DefaultPriority;
				if (defaultPriority > defaultPriority2)
				{
					return defaultPriority;
				}
				return defaultPriority2;
			}
			return 0.5;
		}
	}

	internal Operator(Op op, AstNode opnd1, AstNode opnd2)
	{
		_operatorType = op;
		_opnd1 = opnd1;
		_opnd2 = opnd2;
	}
}
