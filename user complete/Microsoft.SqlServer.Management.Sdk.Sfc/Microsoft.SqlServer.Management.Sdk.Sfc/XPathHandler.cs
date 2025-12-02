using System.Collections;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class XPathHandler
{
	private struct ParamInfo
	{
		private int _Minargs;

		private int _Maxargs;

		private AstNode.RType[] _ArgTypes;

		internal int Minargs => _Minargs;

		internal int Maxargs => _Maxargs;

		internal AstNode.RType[] ArgTypes => _ArgTypes;

		internal ParamInfo(int minargs)
		{
			_Minargs = minargs;
			_Maxargs = 0;
			_ArgTypes = null;
		}

		internal ParamInfo(int minargs, int maxargs, AstNode.RType[] argTypes)
		{
			_Minargs = minargs;
			_Maxargs = maxargs;
			_ArgTypes = argTypes;
		}
	}

	private const string _OrString = "or";

	private const string _AndString = "and";

	private XPathScanner _Scanner;

	private static Hashtable _MethodTable;

	private static readonly AstNode.RType[] temparray1 = new AstNode.RType[1] { AstNode.RType.Error };

	private static readonly AstNode.RType[] temparray2 = new AstNode.RType[1] { AstNode.RType.NodeSet };

	private static readonly AstNode.RType[] temparray3 = new AstNode.RType[1] { AstNode.RType.Any };

	private static readonly AstNode.RType[] temparray4 = new AstNode.RType[1] { AstNode.RType.String };

	private static readonly AstNode.RType[] temparray5 = new AstNode.RType[2]
	{
		AstNode.RType.String,
		AstNode.RType.String
	};

	private static readonly AstNode.RType[] temparray6 = new AstNode.RType[3]
	{
		AstNode.RType.String,
		AstNode.RType.Number,
		AstNode.RType.Number
	};

	private static readonly AstNode.RType[] temparray7 = new AstNode.RType[3]
	{
		AstNode.RType.String,
		AstNode.RType.String,
		AstNode.RType.String
	};

	private static readonly AstNode.RType[] temparray8 = new AstNode.RType[1] { AstNode.RType.Boolean };

	private static readonly AstNode.RType[] temparray9;

	internal static Hashtable MethodTable
	{
		get
		{
			if (_MethodTable == null)
			{
				_MethodTable = new Hashtable(27);
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncLast, new ParamInfo(0, 0, temparray1));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncPosition, new ParamInfo(0, 0, temparray1));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncName, new ParamInfo(0, 1, temparray2));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncNameSpaceUri, new ParamInfo(0, 1, temparray2));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncLocalName, new ParamInfo(0, 1, temparray2));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncCount, new ParamInfo(1, 1, temparray2));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncSum, new ParamInfo(1, 1, temparray2));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncNumber, new ParamInfo(0, 1, temparray3));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncBoolean, new ParamInfo(1, 1, temparray3));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncID, new ParamInfo(1, 1, temparray3));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncString, new ParamInfo(0, 1, temparray3));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncConcat, new ParamInfo(2, 100, temparray4));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncLang, new ParamInfo(1, 1, temparray4));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncStringLength, new ParamInfo(0, 1, temparray4));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncNormalizeSpace, new ParamInfo(0, 1, temparray4));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncStartsWith, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncContains, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncLike, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncIn, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncSubstringBefore, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncSubstringAfter, new ParamInfo(2, 2, temparray5));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncSubstring, new ParamInfo(2, 3, temparray6));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncTranslate, new ParamInfo(3, 3, temparray7));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncNot, new ParamInfo(1, 1, temparray8));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncTrue, new ParamInfo(0, 0, temparray8));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncFalse, new ParamInfo(0, 0, temparray8));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncFloor, new ParamInfo(1, 1, temparray9));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncCeiling, new ParamInfo(1, 1, temparray9));
				_MethodTable.Add(XPathScanner.XPathTokenType.FuncRound, new ParamInfo(1, 1, temparray9));
			}
			return _MethodTable;
		}
	}

	internal XPathHandler()
	{
	}

	private void NameReset()
	{
		_Scanner.Urn = string.Empty;
		_Scanner.Prefix = string.Empty;
		_Scanner.Name = string.Empty;
	}

	internal AstNode Run(XPathScanner scanner)
	{
		_Scanner = scanner;
		_Scanner.Advance();
		if (_Scanner.NextToken() == XPathScanner.XPathTokenType.Eof)
		{
			throw new XPathException(XPathExceptionCode.TokenExpected);
		}
		AstNode result = ParseXPointer(null);
		if (_Scanner.Token != XPathScanner.XPathTokenType.Eof)
		{
			throw new XPathException(XPathExceptionCode.InvalidToken, _Scanner.PchToken);
		}
		return result;
	}

	private AstNode ParseLocationPath(AstNode qyInput)
	{
		AstNode astNode = null;
		AstNode astNode2 = null;
		if (_Scanner.Token == XPathScanner.XPathTokenType.Slash)
		{
			qyInput = new Root();
			_Scanner.NextToken();
			if (_Scanner.Token == XPathScanner.XPathTokenType.Eof)
			{
				return qyInput;
			}
			astNode2 = ParseRelativeLocationPath(qyInput);
			if (astNode2 != null)
			{
				return astNode2;
			}
			return qyInput;
		}
		if (_Scanner.Token == XPathScanner.XPathTokenType.SlashSlash)
		{
			qyInput = new Root();
			astNode = new Axis(Axis.AxisType.DescendantOrSelf, qyInput);
			_Scanner.NextToken();
			if (_Scanner.Token == XPathScanner.XPathTokenType.Eof || XPathScanner.XPathTokenType.Union == _Scanner.Token)
			{
				throw new XPathException(XPathExceptionCode.TokenExpected, _Scanner.PchToken);
			}
			return ParseRelativeLocationPath(astNode);
		}
		return ParseRelativeLocationPath(qyInput);
	}

	private AstNode ParseRelativeLocationPath(AstNode qyInput)
	{
		AstNode astNode = ParseStep(qyInput, forQyCond: false);
		if (astNode == null)
		{
			return null;
		}
		if (XPathScanner.XPathTokenType.SlashSlash == _Scanner.Token)
		{
			_Scanner.NextToken();
			astNode = new Axis(Axis.AxisType.DescendantOrSelf, astNode);
			astNode = ParseRelativeLocationPath(astNode);
		}
		if (XPathScanner.XPathTokenType.Slash == _Scanner.Token)
		{
			_Scanner.NextToken();
			astNode = ParseRelativeLocationPath(astNode);
			if (astNode == null)
			{
				throw new XPathException(XPathExceptionCode.QueryExpected, _Scanner.PchToken);
			}
		}
		return astNode;
	}

	private AstNode ParseStep(AstNode qyInput, bool forQyCond)
	{
		AstNode astNode = null;
		if (forQyCond && XPathScanner.XPathTokenType.Star != _Scanner.Token && XPathScanner.XPathTokenType.Name != _Scanner.Token)
		{
			throw new XPathException(XPathExceptionCode.TokenExpected, _Scanner.PchToken);
		}
		if (XPathScanner.XPathTokenType.Dot == _Scanner.Token)
		{
			astNode = new Axis(Axis.AxisType.Self, qyInput);
			_Scanner.NextToken();
			return astNode;
		}
		if (XPathScanner.XPathTokenType.DotDot == _Scanner.Token)
		{
			astNode = new Axis(Axis.AxisType.Parent, qyInput);
			_Scanner.NextToken();
			return astNode;
		}
		astNode = ParseBasis(qyInput);
		if (astNode == null)
		{
			return astNode;
		}
		while (XPathScanner.XPathTokenType.LBracket == _Scanner.Token)
		{
			AstNode condition = ParsePredicate(astNode);
			if (astNode.ReturnType != AstNode.RType.NodeSet)
			{
				throw new XPathException(XPathExceptionCode.NodeSetExpected, _Scanner.PchToken);
			}
			astNode = new Filter(astNode, condition);
		}
		return astNode;
	}

	private AstNode ParseBasis(AstNode qyInput)
	{
		AstNode astNode = null;
		XPathNodeType type = XPathNodeType.Element;
		switch (_Scanner.Token)
		{
		case XPathScanner.XPathTokenType.Function:
			IsValidType(ref type);
			astNode = new Axis(Axis.AxisType.Child, qyInput, _Scanner.Urn, _Scanner.Prefix, _Scanner.Name, type);
			NameReset();
			_Scanner.NextToken();
			break;
		case XPathScanner.XPathTokenType.Name:
			astNode = new Axis(Axis.AxisType.Child, qyInput, _Scanner.Urn, _Scanner.Prefix, _Scanner.Name, type);
			NameReset();
			_Scanner.NextToken();
			break;
		case XPathScanner.XPathTokenType.At:
			NameReset();
			_Scanner.NextToken();
			if (_Scanner.Token == XPathScanner.XPathTokenType.Name || _Scanner.Token == XPathScanner.XPathTokenType.Star || _Scanner.Token == XPathScanner.XPathTokenType.Function)
			{
				if ('(' == _Scanner.Lookahead)
				{
					IsValidType(ref type);
				}
				else
				{
					type = XPathNodeType.Attribute;
				}
				astNode = new Axis(Axis.AxisType.Attribute, qyInput, _Scanner.Urn, _Scanner.Prefix, _Scanner.Name, type);
				NameReset();
				_Scanner.NextToken();
				break;
			}
			throw new XPathException(XPathExceptionCode.NodeTestExpected, _Scanner.PchToken);
		case XPathScanner.XPathTokenType.Star:
			astNode = new Axis(Axis.AxisType.Child, qyInput, string.Empty, string.Empty, string.Empty, XPathNodeType.Element);
			_Scanner.NextToken();
			break;
		default:
			astNode = ConstructAxesQuery(qyInput);
			break;
		}
		return astNode;
	}

	private AstNode ConstructAxesQuery(AstNode qyInput)
	{
		throw new XPathException(XPathExceptionCode.InvalidToken, _Scanner.PchToken);
	}

	private AstNode ParsePredicate(AstNode qyInput)
	{
		AstNode astNode = null;
		CheckToken(XPathScanner.XPathTokenType.LBracket);
		_Scanner.NextToken();
		astNode = ParseXPointer(qyInput);
		CheckToken(XPathScanner.XPathTokenType.RBracket);
		_Scanner.NextToken();
		return astNode;
	}

	private AstNode ParseXPointer(AstNode qyInput)
	{
		AstNode astNode = ParseAndExpr(qyInput);
		if (astNode == null)
		{
			throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
		}
		if (_Scanner.Token == XPathScanner.XPathTokenType.Name && _Scanner.Prefix.Length <= 0 && _Scanner.Name.Equals("or"))
		{
			_Scanner.NextToken();
			AstNode astNode2 = ParseXPointer(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			astNode = new Operator(Operator.Op.OR, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseAndExpr(AstNode qyInput)
	{
		AstNode astNode = ParseEqualityExpr(qyInput);
		if (_Scanner.Token == XPathScanner.XPathTokenType.Name && _Scanner.Prefix.Length <= 0 && _Scanner.Name.Equals("and"))
		{
			if (astNode == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			_Scanner.NextToken();
			AstNode astNode2 = ParseAndExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			astNode = new Operator(Operator.Op.AND, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseEqualityExpr(AstNode qyInput)
	{
		AstNode astNode = ParseRelationalExpr(qyInput);
		Operator.Op op = Operator.Op.INVALID;
		if (astNode == null)
		{
			return null;
		}
		if (_Scanner.Token == XPathScanner.XPathTokenType.Eq)
		{
			op = Operator.Op.EQ;
		}
		else if (_Scanner.Token == XPathScanner.XPathTokenType.Ne)
		{
			op = Operator.Op.NE;
		}
		if (op != Operator.Op.INVALID)
		{
			_Scanner.NextToken();
			AstNode astNode2 = ParseEqualityExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			astNode = new Operator(op, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseRelationalExpr(AstNode qyInput)
	{
		AstNode astNode = ParseAdditiveExpr(qyInput);
		Operator.Op op = Operator.Op.INVALID;
		if (astNode == null)
		{
			return null;
		}
		switch (_Scanner.Token)
		{
		case XPathScanner.XPathTokenType.Lt:
			op = Operator.Op.LT;
			break;
		case XPathScanner.XPathTokenType.Le:
			op = Operator.Op.LE;
			break;
		case XPathScanner.XPathTokenType.Gt:
			op = Operator.Op.GT;
			break;
		case XPathScanner.XPathTokenType.Ge:
			op = Operator.Op.GE;
			break;
		}
		if (op != Operator.Op.INVALID)
		{
			_Scanner.NextToken();
			AstNode astNode2 = ParseRelationalExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			astNode = new Operator(op, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseAdditiveExpr(AstNode qyInput)
	{
		AstNode astNode = ParseMultiplicativeExpr(qyInput);
		Operator.Op op = Operator.Op.INVALID;
		if (astNode == null)
		{
			return null;
		}
		if (_Scanner.Token == XPathScanner.XPathTokenType.Plus)
		{
			op = Operator.Op.PLUS;
		}
		else if (_Scanner.Token == XPathScanner.XPathTokenType.Minus)
		{
			op = Operator.Op.MINUS;
		}
		if (op != Operator.Op.INVALID)
		{
			_Scanner.NextToken();
			AstNode astNode2 = ParseAdditiveExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.NumberExpected, _Scanner.PchToken);
			}
			astNode = new Operator(op, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseMultiplicativeExpr(AstNode qyInput)
	{
		AstNode astNode = ParseUnaryExpr(qyInput);
		Operator.Op op = Operator.Op.INVALID;
		if (astNode == null)
		{
			return null;
		}
		if (_Scanner.Token == XPathScanner.XPathTokenType.Star)
		{
			op = Operator.Op.MUL;
		}
		else if (_Scanner.Token == XPathScanner.XPathTokenType.Name)
		{
			if (_Scanner.Name.Equals("div"))
			{
				op = Operator.Op.DIV;
			}
			else if (_Scanner.Name.Equals("mod"))
			{
				op = Operator.Op.MOD;
			}
		}
		if (Operator.Op.INVALID != op)
		{
			_Scanner.NextToken();
			AstNode astNode2 = ParseMultiplicativeExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.NumberExpected, _Scanner.PchToken);
			}
			astNode = new Operator(op, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParseUnaryExpr(AstNode qyInput)
	{
		AstNode astNode = null;
		if (XPathScanner.XPathTokenType.Minus == _Scanner.Token)
		{
			_Scanner.NextToken();
			astNode = ParseUnaryExpr(qyInput);
			if (astNode == null)
			{
				throw new XPathException(XPathExceptionCode.NumberExpected, _Scanner.PchToken);
			}
			return new Operator(Operator.Op.NEGATE, astNode, null);
		}
		return ParseUnionExpr(qyInput);
	}

	private AstNode ParseUnionExpr(AstNode qyInput)
	{
		AstNode astNode = ParsePathExpr(qyInput);
		AstNode astNode2 = null;
		if (XPathScanner.XPathTokenType.Union == _Scanner.Token)
		{
			if (astNode == null)
			{
				throw new XPathException(XPathExceptionCode.TokenExpected, _Scanner.PchToken);
			}
			if (astNode.ReturnType != AstNode.RType.NodeSet)
			{
				throw new XPathException(XPathExceptionCode.InvalidToken, _Scanner.PchToken);
			}
			_Scanner.NextToken();
			astNode2 = ParseUnionExpr(qyInput);
			if (astNode2 == null)
			{
				throw new XPathException(XPathExceptionCode.TokenExpected, _Scanner.PchToken);
			}
			if (astNode2.ReturnType != AstNode.RType.NodeSet)
			{
				throw new XPathException(XPathExceptionCode.InvalidToken, _Scanner.PchToken);
			}
			astNode = new Operator(Operator.Op.UNION, astNode, astNode2);
		}
		return astNode;
	}

	private AstNode ParsePathExpr(AstNode qyInput)
	{
		AstNode astNode;
		if ((XPathScanner.XPathTokenType.Function == _Scanner.Token && !IsNodeType()) || XPathScanner.XPathTokenType.Dollar == _Scanner.Token || XPathScanner.XPathTokenType.LParens == _Scanner.Token || XPathScanner.XPathTokenType.Number == _Scanner.Token || XPathScanner.XPathTokenType.String == _Scanner.Token)
		{
			astNode = ParseFilterExpr(qyInput);
			if (_Scanner.Token == XPathScanner.XPathTokenType.Slash)
			{
				_Scanner.NextToken();
				astNode = ParseRelativeLocationPath(astNode);
			}
			else if (_Scanner.Token == XPathScanner.XPathTokenType.SlashSlash)
			{
				AstNode qyInput2 = new Axis(Axis.AxisType.DescendantOrSelf, astNode);
				_Scanner.NextToken();
				astNode = ParseLocationPath(qyInput2);
			}
		}
		else
		{
			astNode = ParseLocationPath(null);
		}
		return astNode;
	}

	private bool IsNodeType()
	{
		if (!(_Scanner.Name == "node") && !(_Scanner.Name == "text") && !(_Scanner.Name == "processing-instruction"))
		{
			return _Scanner.Name == "comment";
		}
		return true;
	}

	private AstNode ParseFilterExpr(AstNode qyInput)
	{
		AstNode astNode = ParsePrimaryExpr(qyInput);
		while (XPathScanner.XPathTokenType.LBracket == _Scanner.Token)
		{
			AstNode condition = ParsePredicate(astNode);
			astNode = new Filter(astNode, condition);
		}
		return astNode;
	}

	private AstNode ParsePrimaryExpr(AstNode qyInput)
	{
		AstNode astNode = null;
		switch (_Scanner.Token)
		{
		case XPathScanner.XPathTokenType.String:
			astNode = new Operand(Urn.UnEscapeString(_Scanner.Tstring));
			break;
		case XPathScanner.XPathTokenType.Number:
			astNode = new Operand(_Scanner.Number);
			break;
		case XPathScanner.XPathTokenType.LParens:
			_Scanner.NextToken();
			astNode = ParseXPointer(qyInput);
			if (astNode == null)
			{
				return null;
			}
			if (astNode.TypeOfAst != AstNode.QueryType.ConstantOperand)
			{
				astNode = new Group(astNode);
			}
			CheckToken(XPathScanner.XPathTokenType.RParens);
			break;
		case XPathScanner.XPathTokenType.Dollar:
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.Name);
			astNode = new Operand(_Scanner.Name, _Scanner.Prefix);
			break;
		case XPathScanner.XPathTokenType.FuncFalse:
			astNode = new Function(Function.FunctionType.FuncFalse);
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.LParens);
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.RParens);
			break;
		case XPathScanner.XPathTokenType.FuncTrue:
			astNode = new Function(Function.FunctionType.FuncTrue);
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.LParens);
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.RParens);
			break;
		case XPathScanner.XPathTokenType.FuncLast:
			_Scanner.SkipSpace();
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.LParens);
			_Scanner.NextToken();
			CheckToken(XPathScanner.XPathTokenType.RParens);
			astNode = new Function(Function.FunctionType.FuncLast);
			break;
		default:
			astNode = ParseMethod(null);
			break;
		}
		_Scanner.NextToken();
		return astNode;
	}

	private AstNode ParseMethod(AstNode qyInput)
	{
		if (_Scanner.Lookahead != '(')
		{
			throw new XPathException(XPathExceptionCode.FunctionExpected, _Scanner.PchToken);
		}
		if (_Scanner.Prefix.Length > 0 || !XPathScanner.FunctionTable.Contains(_Scanner.Name))
		{
			return ParseXsltMethod(qyInput);
		}
		XPathScanner.XPathTokenType xPathTokenType = (XPathScanner.XPathTokenType)XPathScanner.FunctionTable[_Scanner.Name];
		if (!MethodTable.Contains(xPathTokenType))
		{
			return ParseXsltMethod(qyInput);
		}
		_Scanner.NextToken();
		AstNode astNode = null;
		ParamInfo paramInfo = (ParamInfo)MethodTable[xPathTokenType];
		ArrayList arrayList = new ArrayList();
		int num = 0;
		bool flag = true;
		int num2 = 0;
		_Scanner.NextToken();
		if (xPathTokenType == XPathScanner.XPathTokenType.FuncConcat)
		{
			flag = false;
			num2 = 0;
		}
		while (XPathScanner.XPathTokenType.RParens != _Scanner.Token)
		{
			if (flag)
			{
				num2 = num;
				if (paramInfo.Maxargs == num)
				{
					throw new XPathException(XPathExceptionCode.InvalidNumArgs, _Scanner.PchToken);
				}
			}
			astNode = ParseXPointer(qyInput);
			if (astNode == null)
			{
				throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
			}
			if (paramInfo.ArgTypes[num2] != AstNode.RType.Any && astNode.ReturnType != paramInfo.ArgTypes[num2])
			{
				switch (paramInfo.ArgTypes[num2])
				{
				case AstNode.RType.NodeSet:
					if (astNode.ReturnType != AstNode.RType.Variable && (!(astNode is Function) || astNode.ReturnType != AstNode.RType.Error))
					{
						throw new XPathException(XPathExceptionCode.InvalidArgument, _Scanner.PchToken);
					}
					break;
				case AstNode.RType.String:
					astNode = new Function(Function.FunctionType.FuncString, astNode);
					break;
				case AstNode.RType.Number:
					astNode = new Function(Function.FunctionType.FuncNumber, astNode);
					break;
				case AstNode.RType.Boolean:
					astNode = new Function(Function.FunctionType.FuncBoolean, astNode);
					break;
				}
			}
			if (XPathScanner.XPathTokenType.Comma == _Scanner.Token)
			{
				_Scanner.NextToken();
				if (XPathScanner.XPathTokenType.RParens == _Scanner.Token)
				{
					throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
				}
			}
			arrayList.Add(astNode);
			num++;
		}
		if (paramInfo.Minargs > num)
		{
			throw new XPathException(XPathExceptionCode.InvalidNumArgs, _Scanner.PchToken);
		}
		return new Function((Function.FunctionType)(0 - xPathTokenType - 84), arrayList);
	}

	private void CheckToken(XPathScanner.XPathTokenType t)
	{
		if (_Scanner.Token != t)
		{
			throw new XPathException(XPathExceptionCode.InvalidToken, _Scanner.PchToken);
		}
	}

	private void IsValidType(ref XPathNodeType type)
	{
		if (_Scanner.Urn.Length > 0 || _Scanner.Prefix.Length > 0)
		{
			throw new XPathException(XPathExceptionCode.NodeTestExpected, _Scanner.PchToken);
		}
		_Scanner.NextToken();
		CheckToken(XPathScanner.XPathTokenType.LParens);
		if (_Scanner.Name == "node")
		{
			_Scanner.Name = string.Empty;
			type = XPathNodeType.All;
		}
		else if (_Scanner.Name == "text")
		{
			_Scanner.Name = string.Empty;
			type = XPathNodeType.Text;
		}
		else if (_Scanner.Name == "processing-instruction")
		{
			type = XPathNodeType.ProcessingInstruction;
			if (')' != _Scanner.Lookahead)
			{
				_Scanner.NextToken();
				CheckToken(XPathScanner.XPathTokenType.String);
				_Scanner.Name = _Scanner.Tstring.Copy();
			}
			else
			{
				_Scanner.Name = string.Empty;
			}
		}
		else
		{
			if (!(_Scanner.Name == "comment"))
			{
				throw new XPathException(XPathExceptionCode.NodeTestExpected, _Scanner.PchToken);
			}
			_Scanner.Name = string.Empty;
			type = XPathNodeType.Comment;
		}
		_Scanner.NextToken();
		CheckToken(XPathScanner.XPathTokenType.RParens);
	}

	private AstNode ParseXsltMethod(AstNode qyInput)
	{
		AstNode astNode = null;
		ArrayList arrayList = new ArrayList();
		int num = 0;
		string name = _Scanner.Name;
		string prefix = _Scanner.Prefix;
		NameReset();
		_Scanner.NextToken();
		_Scanner.NextToken();
		while (XPathScanner.XPathTokenType.RParens != _Scanner.Token)
		{
			astNode = ParseXPointer(qyInput);
			if (XPathScanner.XPathTokenType.Comma == _Scanner.Token)
			{
				_Scanner.NextToken();
				if (XPathScanner.XPathTokenType.RParens == _Scanner.Token)
				{
					throw new XPathException(XPathExceptionCode.ExpressionExpected, _Scanner.PchToken);
				}
			}
			arrayList.Add(astNode);
			num++;
		}
		return new Function(prefix, name, arrayList);
	}

	static XPathHandler()
	{
		AstNode.RType[] array = new AstNode.RType[1];
		temparray9 = array;
	}
}
