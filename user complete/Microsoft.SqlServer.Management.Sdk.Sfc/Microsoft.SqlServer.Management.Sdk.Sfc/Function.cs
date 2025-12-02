using System.Collections;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Function : AstNode
{
	internal enum FunctionType
	{
		FuncLast,
		FuncPosition,
		FuncCount,
		FuncLocalName,
		FuncNameSpaceUri,
		FuncName,
		FuncString,
		FuncBoolean,
		FuncNumber,
		FuncTrue,
		FuncFalse,
		FuncNot,
		FuncID,
		FuncConcat,
		FuncStartsWith,
		FuncContains,
		FuncSubstringBefore,
		FuncSubstringAfter,
		FuncSubstring,
		FuncStringLength,
		FuncNormalize,
		FuncTranslate,
		FuncLang,
		FuncSum,
		FuncFloor,
		FuncCeiling,
		FuncRound,
		FuncLike,
		FuncIn,
		FuncUserDefined,
		Error
	}

	private FunctionType _functionType = FunctionType.Error;

	private ArrayList _argumentList;

	private string[] str = new string[29]
	{
		"last()", "position()", "count()", "localname()", "namespaceuri()", "name()", "string()", "boolean()", "number()", "true()",
		"false()", "not()", "id()", "concat()", "starts-with()", "contains()", "substring-before()", "substring-after()", "substring()", "string-length()",
		"normalize-space()", "translate()", "lang()", "sum()", "floor()", "celing()", "round()", "like()", "in()"
	};

	private string _Name;

	private string _Prefix;

	internal override QueryType TypeOfAst => QueryType.Function;

	internal override RType ReturnType => _functionType switch
	{
		FunctionType.FuncLast => RType.NodeSet, 
		FunctionType.FuncPosition => RType.Number, 
		FunctionType.FuncCount => RType.Number, 
		FunctionType.FuncID => RType.NodeSet, 
		FunctionType.FuncLocalName => RType.String, 
		FunctionType.FuncNameSpaceUri => RType.String, 
		FunctionType.FuncName => RType.String, 
		FunctionType.FuncString => RType.String, 
		FunctionType.FuncBoolean => RType.Boolean, 
		FunctionType.FuncNumber => RType.Number, 
		FunctionType.FuncTrue => RType.Boolean, 
		FunctionType.FuncFalse => RType.Boolean, 
		FunctionType.FuncNot => RType.Boolean, 
		FunctionType.FuncConcat => RType.String, 
		FunctionType.FuncStartsWith => RType.Boolean, 
		FunctionType.FuncContains => RType.Boolean, 
		FunctionType.FuncSubstringBefore => RType.String, 
		FunctionType.FuncSubstringAfter => RType.String, 
		FunctionType.FuncSubstring => RType.String, 
		FunctionType.FuncStringLength => RType.Number, 
		FunctionType.FuncNormalize => RType.String, 
		FunctionType.FuncTranslate => RType.String, 
		FunctionType.FuncLang => RType.Boolean, 
		FunctionType.FuncSum => RType.Number, 
		FunctionType.FuncFloor => RType.Number, 
		FunctionType.FuncCeiling => RType.Number, 
		FunctionType.FuncRound => RType.Number, 
		FunctionType.FuncLike => RType.Boolean, 
		FunctionType.FuncIn => RType.Boolean, 
		_ => RType.Error, 
	};

	internal FunctionType TypeOfFunction => _functionType;

	internal ArrayList ArgumentList => _argumentList;

	internal string Name
	{
		get
		{
			if (_functionType != FunctionType.FuncUserDefined)
			{
				return str[(int)_functionType];
			}
			return _Name;
		}
	}

	internal Function(FunctionType ftype, ArrayList argumentList)
	{
		_functionType = ftype;
		_argumentList = new ArrayList(argumentList);
	}

	internal Function(string prefix, string name, ArrayList argumentList)
	{
		_functionType = FunctionType.FuncUserDefined;
		_Prefix = prefix;
		_Name = name;
		_argumentList = new ArrayList(argumentList);
	}

	internal Function(FunctionType ftype)
	{
		_functionType = ftype;
	}

	internal Function(FunctionType ftype, AstNode arg)
	{
		_functionType = ftype;
		_argumentList = new ArrayList();
		_argumentList.Add(arg);
	}
}
