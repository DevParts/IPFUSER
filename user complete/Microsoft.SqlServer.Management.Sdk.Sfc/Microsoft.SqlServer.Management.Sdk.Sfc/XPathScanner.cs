using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal sealed class XPathScanner
{
	internal enum XPathTokenType
	{
		Comma = 44,
		Slash = 47,
		Caret = 94,
		At = 64,
		Dot = 46,
		LParens = 40,
		RParens = 41,
		LBracket = 91,
		RBracket = 93,
		Colon = 58,
		Semicolon = 59,
		Star = 42,
		Plus = 43,
		Minus = 45,
		Eq = 61,
		Lt = 60,
		Gt = 62,
		Bang = 33,
		Dollar = 36,
		Apos = 39,
		Quote = 34,
		Union = 124,
		Eof = 0,
		InvalidToken = -1,
		DotDot = -2,
		SlashSlash = -3,
		Name = -4,
		String = -5,
		Number = -6,
		Ne = -7,
		Le = -8,
		Ge = -9,
		And = -10,
		Or = -11,
		Element = -30,
		Attribute = -31,
		Textnode = -32,
		CData = -33,
		Comment = -34,
		Text = -35,
		Pi = -36,
		Node = -38,
		NodeValue = -40,
		NodeType = -42,
		NodeName = -43,
		End = -44,
		Query = -45,
		Boolean = -54,
		Negate = -57,
		AxesAncestor = -66,
		AxesAncestorSelf = -67,
		AxesAttribute = -68,
		AxesChild = -69,
		AxesDescendant = -70,
		AxesDescendantSelf = -71,
		AxesFollowing = -72,
		AxesFollowingSibling = -73,
		AxesNameSpace = -74,
		AxesParent = -75,
		AxesPreceding = -76,
		AxesPrecedingSibling = -77,
		AxesSelf = -78,
		ColonColon = -79,
		Function = -80,
		Div = -81,
		Mod = -82,
		ProcessingInstruction = -83,
		FuncLast = -84,
		FuncPosition = -85,
		FuncCount = -86,
		FuncLocalName = -87,
		FuncNameSpaceUri = -88,
		FuncName = -89,
		FuncString = -90,
		FuncBoolean = -91,
		FuncNumber = -92,
		FuncTrue = -93,
		FuncFalse = -94,
		FuncNot = -95,
		FuncID = -96,
		FuncConcat = -97,
		FuncStartsWith = -98,
		FuncContains = -99,
		FuncSubstringBefore = -100,
		FuncSubstringAfter = -101,
		FuncSubstring = -102,
		FuncStringLength = -103,
		FuncNormalizeSpace = -104,
		FuncTranslate = -105,
		FuncLang = -106,
		FuncSum = -107,
		FuncFloor = -108,
		FuncCeiling = -109,
		FuncRound = -110,
		FuncLike = -111,
		FuncIn = -112,
		Unknown = -1000
	}

	private char _Lookahead;

	private string _PchNext;

	private int _PchNindex;

	private int _PchLength;

	private string _Tstring;

	private int _PchTokenIndex;

	private int _Ul;

	private XPathTokenType _Token;

	private string _Name;

	private double _Number;

	private string _Prefix = string.Empty;

	private string _Urn = string.Empty;

	private static Hashtable _AxesTable;

	private static Hashtable _FunctionTable;

	private bool _Axes;

	internal static Hashtable AxesTable
	{
		get
		{
			if (_AxesTable == null)
			{
				_AxesTable = new Hashtable(13);
				_AxesTable.Add("child", XPathTokenType.AxesChild);
				_AxesTable.Add("descendant", XPathTokenType.AxesDescendant);
				_AxesTable.Add("parent", XPathTokenType.AxesParent);
				_AxesTable.Add("ancestor", XPathTokenType.AxesAncestor);
				_AxesTable.Add("following-sibling", XPathTokenType.AxesFollowingSibling);
				_AxesTable.Add("preceding-sibling", XPathTokenType.AxesPrecedingSibling);
				_AxesTable.Add("following", XPathTokenType.AxesFollowing);
				_AxesTable.Add("preceding", XPathTokenType.AxesPreceding);
				_AxesTable.Add("attribute", XPathTokenType.AxesAttribute);
				_AxesTable.Add("namespace", XPathTokenType.AxesNameSpace);
				_AxesTable.Add("self", XPathTokenType.AxesSelf);
				_AxesTable.Add("descendant-or-self", XPathTokenType.AxesDescendantSelf);
				_AxesTable.Add("ancestor-or-self", XPathTokenType.AxesAncestorSelf);
			}
			return _AxesTable;
		}
	}

	internal static Hashtable FunctionTable
	{
		get
		{
			if (_FunctionTable == null)
			{
				_FunctionTable = new Hashtable(36);
				_FunctionTable.Add("last", XPathTokenType.FuncLast);
				_FunctionTable.Add("position", XPathTokenType.FuncPosition);
				_FunctionTable.Add("name", XPathTokenType.FuncName);
				_FunctionTable.Add("namespace-uri", XPathTokenType.FuncNameSpaceUri);
				_FunctionTable.Add("local-name", XPathTokenType.FuncLocalName);
				_FunctionTable.Add("count", XPathTokenType.FuncCount);
				_FunctionTable.Add("id", XPathTokenType.FuncID);
				_FunctionTable.Add("string", XPathTokenType.FuncString);
				_FunctionTable.Add("concat", XPathTokenType.FuncConcat);
				_FunctionTable.Add("starts-with", XPathTokenType.FuncStartsWith);
				_FunctionTable.Add("contains", XPathTokenType.FuncContains);
				_FunctionTable.Add("substring-before", XPathTokenType.FuncSubstringBefore);
				_FunctionTable.Add("substring-after", XPathTokenType.FuncSubstringAfter);
				_FunctionTable.Add("substring", XPathTokenType.FuncSubstring);
				_FunctionTable.Add("string-length", XPathTokenType.FuncStringLength);
				_FunctionTable.Add("normalize-space", XPathTokenType.FuncNormalizeSpace);
				_FunctionTable.Add("translate", XPathTokenType.FuncTranslate);
				_FunctionTable.Add("boolean", XPathTokenType.FuncBoolean);
				_FunctionTable.Add("not", XPathTokenType.FuncNot);
				_FunctionTable.Add("true", XPathTokenType.FuncTrue);
				_FunctionTable.Add("false", XPathTokenType.FuncFalse);
				_FunctionTable.Add("lang", XPathTokenType.FuncLang);
				_FunctionTable.Add("number", XPathTokenType.FuncNumber);
				_FunctionTable.Add("sum", XPathTokenType.FuncSum);
				_FunctionTable.Add("floor", XPathTokenType.FuncFloor);
				_FunctionTable.Add("ceiling", XPathTokenType.FuncCeiling);
				_FunctionTable.Add("round", XPathTokenType.FuncRound);
				_FunctionTable.Add("like", XPathTokenType.FuncLike);
				_FunctionTable.Add("in", XPathTokenType.FuncIn);
			}
			return _FunctionTable;
		}
	}

	internal char Lookahead => _Lookahead;

	internal bool Axes => _Axes;

	internal XPathTokenType Token => _Token;

	internal string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
		}
	}

	internal double Number => _Number;

	internal string Prefix
	{
		get
		{
			return _Prefix;
		}
		set
		{
			_Prefix = value;
		}
	}

	internal string Urn
	{
		get
		{
			return _Urn;
		}
		set
		{
			_Urn = value;
		}
	}

	internal string PchToken
	{
		get
		{
			try
			{
				return _PchNext.Substring(_PchTokenIndex);
			}
			catch (ArgumentException)
			{
				return "\0";
			}
		}
	}

	internal string Tstring => _Tstring;

	internal XPathScanner()
	{
	}

	internal void SetParseString(string parsestring)
	{
		Reset();
		_PchNext = parsestring.Copy();
		_PchLength = _PchNext.Length;
	}

	internal void Reset()
	{
		_Lookahead = ' ';
		_PchNext = string.Empty;
		_PchNindex = 0;
		_Tstring = string.Empty;
		_PchTokenIndex = 0;
		_Ul = 0;
		_Token = XPathTokenType.InvalidToken;
		_Name = string.Empty;
		_Number = 0.0;
		_Prefix = string.Empty;
		_Urn = string.Empty;
		_PchLength = 0;
	}

	internal void SkipSpace()
	{
		while (XmlCharType.IsWhiteSpace(_Lookahead) && _Lookahead != 0)
		{
			Advance();
		}
	}

	internal void Advance()
	{
		if (_Lookahead != 0)
		{
			if (_PchNindex < _PchLength)
			{
				_Lookahead = _PchNext[_PchNindex];
				_PchNindex++;
			}
			else
			{
				_Lookahead = '\0';
			}
		}
	}

	internal char TestAdvance()
	{
		if (_Lookahead != 0 && _PchNindex < _PchLength)
		{
			return _PchNext[_PchNindex];
		}
		return '\0';
	}

	private void ScanString()
	{
		char lookahead = _Lookahead;
		StringBuilder stringBuilder = new StringBuilder();
		Advance();
		while (_Lookahead != 0)
		{
			if (_Lookahead == lookahead)
			{
				if (lookahead != TestAdvance())
				{
					break;
				}
				stringBuilder.Append(_Lookahead);
				Advance();
			}
			stringBuilder.Append(_Lookahead);
			Advance();
		}
		if (_Lookahead == '\0')
		{
			throw new XPathException(XPathExceptionCode.UnclosedString, PchToken);
		}
		Advance();
		_Token = XPathTokenType.String;
		_Tstring = stringBuilder.ToString().Copy();
	}

	private int ScanName(ref int len)
	{
		int pchNindex = _PchNindex;
		bool flag = false;
		if (!XmlCharType.IsStartNameChar(_Lookahead))
		{
			_Token = XPathTokenType.Unknown;
			return (int)_Token;
		}
		while (true)
		{
			switch (_Lookahead)
			{
			case '\0':
				_PchNindex++;
				break;
			case ':':
				if (':' == NextChar())
				{
					_Axes = true;
				}
				break;
			case '(':
				flag = true;
				break;
			default:
				if (XmlCharType.IsNameChar(_Lookahead))
				{
					goto IL_0125;
				}
				_Token = XPathTokenType.Unknown;
				return (int)_Token;
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '!':
			case ')':
			case '*':
			case '+':
			case ',':
			case '/':
			case ';':
			case '<':
			case '=':
			case '>':
			case '[':
			case ']':
			case '|':
				break;
			}
			break;
			IL_0125:
			Advance();
		}
		len = _PchNindex - pchNindex;
		_Name = _PchNext.Substring(pchNindex - 1, len);
		if (_Axes)
		{
			if (AxesTable.Contains(_Name))
			{
				_Token = (XPathTokenType)AxesTable[_Name];
			}
			else
			{
				_Token = XPathTokenType.Unknown;
			}
			_Name = string.Empty;
		}
		else if (flag)
		{
			_Token = XPathTokenType.Function;
		}
		else
		{
			_Token = XPathTokenType.Name;
			if (_Lookahead == ':')
			{
				_Prefix = _Name;
				Advance();
				if (ScanName(ref _Ul) == -1000)
				{
					if (_Lookahead != '*')
					{
						throw new XPathException(XPathExceptionCode.InvalidName, PchToken);
					}
					_Token = XPathTokenType.Name;
					_Name = string.Empty;
					Advance();
				}
			}
		}
		return (int)_Token;
	}

	private void ScanNumber()
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (_Lookahead != '.' && (_Lookahead > '9' || _Lookahead < '0'))
		{
			_Token = XPathTokenType.Unknown;
			return;
		}
		if (_Lookahead == '.')
		{
			flag = true;
		}
		while (true)
		{
			stringBuilder.Append(_Lookahead);
			Advance();
			if (_Lookahead > '9' || _Lookahead < '0')
			{
				if (_Lookahead != '.')
				{
					break;
				}
				if (flag)
				{
					_Token = XPathTokenType.Unknown;
					return;
				}
				flag = true;
			}
		}
		if (stringBuilder.Length == 1 && stringBuilder[0] == '.')
		{
			_Token = XPathTokenType.Unknown;
			return;
		}
		string value = stringBuilder.ToString();
		_Number = Convert.ToDouble(value, CultureInfo.InvariantCulture.NumberFormat);
		_Token = XPathTokenType.Number;
	}

	private char NextChar()
	{
		if (_PchNindex < _PchNext.Length)
		{
			return _PchNext[_PchNindex];
		}
		return '\0';
	}

	internal XPathTokenType NextToken()
	{
		SkipSpace();
		_Axes = false;
		_PchTokenIndex = _PchNindex;
		switch (_Lookahead)
		{
		case '\0':
			_Token = XPathTokenType.Eof;
			break;
		case '#':
		case '(':
		case ')':
		case '*':
		case '+':
		case ',':
		case '-':
		case ';':
		case '=':
		case '@':
		case '[':
		case ']':
			_Token = (XPathTokenType)Convert.ToInt32(_Lookahead);
			Advance();
			break;
		case ':':
			_Token = XPathTokenType.Colon;
			Advance();
			if (':' == _Lookahead)
			{
				_Token = XPathTokenType.ColonColon;
				Advance();
			}
			break;
		case '|':
			Advance();
			_Token = XPathTokenType.Union;
			break;
		case '<':
			_Token = (XPathTokenType)Convert.ToInt32(_Lookahead, CultureInfo.InvariantCulture.NumberFormat);
			Advance();
			if (_Lookahead == '=')
			{
				Advance();
				_Token = XPathTokenType.Le;
			}
			break;
		case '>':
			_Token = (XPathTokenType)Convert.ToInt32(_Lookahead);
			Advance();
			if (_Lookahead == '=')
			{
				Advance();
				_Token = XPathTokenType.Ge;
			}
			break;
		case '/':
			_Token = XPathTokenType.Slash;
			Advance();
			if (_Lookahead == '/')
			{
				Advance();
				_Token = XPathTokenType.SlashSlash;
			}
			break;
		case '!':
			Advance();
			if (_Lookahead == '=')
			{
				Advance();
				_Token = XPathTokenType.Ne;
			}
			else
			{
				_Token = XPathTokenType.Bang;
			}
			break;
		case '.':
			ScanNumber();
			if (_Token == XPathTokenType.Unknown)
			{
				if (_Lookahead == '.')
				{
					Advance();
					_Token = XPathTokenType.DotDot;
				}
				else
				{
					_Token = XPathTokenType.Dot;
				}
			}
			break;
		case '$':
			Advance();
			_Token = XPathTokenType.Dollar;
			break;
		case '"':
		case '\'':
			ScanString();
			break;
		default:
		{
			int pchNindex = _PchNindex;
			if (ScanName(ref _Ul) == -1000 && _PchNindex == pchNindex)
			{
				ScanNumber();
			}
			break;
		}
		}
		return _Token;
	}
}
