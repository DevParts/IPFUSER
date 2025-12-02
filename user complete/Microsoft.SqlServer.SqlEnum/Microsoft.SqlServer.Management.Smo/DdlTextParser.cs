using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class DdlTextParser
{
	private const string sWord = "(?<word>[\\w_@#][\\w\\d_$$@#]*)";

	private const string sProcNumber = "(?<number_proc>;[\\d]+)";

	private static readonly string sSingleLineCommentSql;

	private static readonly string sSingleLineCommentC;

	private static readonly string sNestedMultiLineComment;

	private static readonly string sNestedMultiLineCommentOptimized;

	private static readonly string sSingleQuotedString;

	private static readonly string sDoubleQuotedString;

	private static readonly string sBraketedWord;

	private static readonly string sDot;

	private static readonly string sPharanthesis;

	private static readonly string sParam;

	private static readonly string sExec;

	private static readonly string sReturns;

	private static readonly string sReturnsTable;

	private static readonly string sReturn;

	private static readonly string sDelim1;

	private static readonly string sDelim2;

	internal static readonly DdlTextParserSingleton ddlTextParserSingleton;

	private static readonly string sNestedMultiLineCommentOptimized_end;

	private static readonly string sSpace_end;

	private static readonly string sWord_end;

	private static readonly string sEol_end;

	private static readonly string sSingleLineComment_end;

	private static readonly string sReject_end;

	static DdlTextParser()
	{
		sSingleLineCommentSql = "(--[^\n\r]*)";
		sSingleLineCommentC = "(//[^\n\r]*)";
		sNestedMultiLineComment = "(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)";
		sNestedMultiLineCommentOptimized = "(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*\\*/|" + sNestedMultiLineComment + ")";
		sSingleQuotedString = "('(('')|[^'])*)'";
		sDoubleQuotedString = "(?<quoted_word>\"((\"\")|[^\"])*\")";
		sBraketedWord = "(?<braket_word>\\[((\\]\\])|[^\\]])*\\])";
		sDot = "(?<dot>\\.)";
		sPharanthesis = "(?<parant_open>\\()|(?<parant_close>\\))";
		sParam = "(?:(?<param>@[\\w_@#][\\w\\d_$$@#]*)((\\s)|(" + sSingleLineCommentSql + ")|(" + sNestedMultiLineCommentOptimized + "))*(AS){0,1})";
		sExec = "(?<exec>\\bEXECUTE\\b)";
		sReturns = "(?<returns>\\bRETURNS\\b)";
		sReturnsTable = "(?<returns_table>\\bRETURNS\\s+TABLE\\b)";
		sReturn = "(?<return>\\bRETURN\\b)";
		sDelim1 = "(?<delim1>\\bAS\\b\\s*)";
		sDelim2 = "(?<delim2>\\bBEGIN\\b)";
		ddlTextParserSingleton = new DdlTextParserSingleton();
		sNestedMultiLineCommentOptimized_end = sNestedMultiLineCommentOptimized;
		sSpace_end = "([ \\t]+)";
		sWord_end = "(?<word>[a-zA-Z0-9]+)";
		sEol_end = "(?<eol>\\n)";
		sSingleLineComment_end = "(?<slcomm>--)";
		sReject_end = "(?<reject>.)";
		string pattern = sNestedMultiLineCommentOptimized + "|" + sSingleLineCommentSql + "|" + sSingleQuotedString + "|" + sDoubleQuotedString + "|" + sBraketedWord + "|" + sSingleLineCommentC + "|" + sParam + "|" + sDelim1 + "|" + sDelim2 + "|" + sPharanthesis + "|" + sExec + "|" + sReturnsTable + "|" + sReturns + "|" + sReturn + "|" + sDot + "|(?<word>[\\w_@#][\\w\\d_$$@#]*)|(?<number_proc>;[\\d]+)";
		ddlTextParserSingleton.regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
		string pattern2 = sNestedMultiLineCommentOptimized_end + "|" + sSpace_end + "|" + sWord_end + "|" + sEol_end + "|" + sSingleLineComment_end + "|" + sReject_end;
		ddlTextParserSingleton.m_r_end = new Regex(pattern2, RegexOptions.ExplicitCapture | RegexOptions.RightToLeft);
	}

	private static bool ReadNameFromDdl(ref Match m, bool useQuotedIdentifier, ref DdlTextParserHeaderInfo headerInfo)
	{
		m = m.NextMatch();
		while (m.Success)
		{
			if (m.Groups["word"].Success)
			{
				headerInfo.name = "[" + Util.EscapeString(m.Groups["word"].Value, ']') + "]";
				headerInfo.indexNameStart = m.Groups["word"].Index;
				headerInfo.indexNameEnd = headerInfo.indexNameStart + m.Groups["word"].Length;
				break;
			}
			if (m.Groups["braket_word"].Success)
			{
				headerInfo.name = m.Groups["braket_word"].Value;
				headerInfo.indexNameStart = m.Groups["braket_word"].Index;
				headerInfo.indexNameEnd = headerInfo.indexNameStart + m.Groups["braket_word"].Length;
				break;
			}
			if (m.Groups["quoted_word"].Success)
			{
				if (!useQuotedIdentifier)
				{
					return false;
				}
				headerInfo.name = m.Groups["quoted_word"].Value;
				headerInfo.indexNameStart = m.Groups["quoted_word"].Index;
				headerInfo.indexNameEnd = headerInfo.indexNameStart + m.Groups["quoted_word"].Length;
				int index = 0;
				headerInfo.name = "[" + Util.UnEscapeString(headerInfo.name, '"', '"', ref index) + "]";
				break;
			}
			m = m.NextMatch();
		}
		if (m.Success)
		{
			m = m.NextMatch();
			if (m.Groups["number_proc"].Success)
			{
				headerInfo.procedureNumber = m.Groups["number_proc"].Value;
				headerInfo.indexNameEnd += m.Groups["number_proc"].Length;
			}
		}
		return headerInfo.name.Length > 0;
	}

	private static bool ReadFullNameFromDdl(ref Match m, bool useQuotedIdentifier, ref DdlTextParserHeaderInfo headerInfo)
	{
		if (!ReadNameFromDdl(ref m, useQuotedIdentifier, ref headerInfo))
		{
			return false;
		}
		if (m.Groups["dot"].Success)
		{
			headerInfo.schema = headerInfo.name;
			headerInfo.name = string.Empty;
			int indexNameStart = headerInfo.indexNameStart;
			headerInfo.procedureNumber = string.Empty;
			if (!ReadNameFromDdl(ref m, useQuotedIdentifier, ref headerInfo))
			{
				return false;
			}
			headerInfo.indexNameStart = indexNameStart;
			if (m.Groups["dot"].Success)
			{
				headerInfo.database = headerInfo.schema;
				headerInfo.schema = headerInfo.name;
				headerInfo.name = string.Empty;
				headerInfo.procedureNumber = string.Empty;
				if (!ReadNameFromDdl(ref m, useQuotedIdentifier, ref headerInfo))
				{
					return false;
				}
				headerInfo.indexNameStart = indexNameStart;
			}
		}
		TraceHelper.Assert(headerInfo.name.Length > 0 && headerInfo.indexNameStart > 0 && headerInfo.indexNameEnd > headerInfo.indexNameStart);
		return true;
	}

	public static bool CheckDdlHeader(string objectText, bool useQuotedIdentifier, out DdlTextParserHeaderInfo headerInfo)
	{
		return CheckDdlHeader(objectText, useQuotedIdentifier, isOrAlterSupported: false, out headerInfo);
	}

	public static bool CheckDdlHeader(string objectText, bool useQuotedIdentifier, bool isOrAlterSupported, out DdlTextParserHeaderInfo headerInfo)
	{
		headerInfo = default(DdlTextParserHeaderInfo);
		headerInfo.indexCreate = -1;
		headerInfo.scriptContainsOrAlter = false;
		headerInfo.indexOrAlterStart = -1;
		headerInfo.indexOrAlterEnd = -1;
		headerInfo.indexNameEnd = -1;
		headerInfo.indexNameStart = -1;
		headerInfo.name = "";
		headerInfo.objectType = "";
		headerInfo.procedureNumber = "";
		headerInfo.schema = "";
		headerInfo.scriptForCreate = true;
		headerInfo.indexNameEndSecondary = -1;
		headerInfo.indexNameStartSecondary = -1;
		headerInfo.nameSecondary = "";
		headerInfo.schemaSecondary = "";
		Match m = ddlTextParserSingleton.regex.Match(objectText);
		while (m.Success)
		{
			if (m.Groups["word"].Success)
			{
				if (!headerInfo.scriptForCreate || (headerInfo.indexCreate != -1 && !isOrAlterSupported))
				{
					break;
				}
				if (string.Compare("create", m.Groups["word"].Value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					headerInfo.indexCreate = m.Groups["word"].Index;
				}
				else if (isOrAlterSupported && headerInfo.indexCreate != -1)
				{
					if (string.Compare("or", m.Groups["word"].Value, StringComparison.OrdinalIgnoreCase) == 0)
					{
						headerInfo.indexOrAlterStart = m.Groups["word"].Index;
					}
					else
					{
						if (string.Compare("alter", m.Groups["word"].Value, StringComparison.OrdinalIgnoreCase) != 0)
						{
							if ((headerInfo.indexOrAlterStart == -1 && headerInfo.indexOrAlterEnd == -1) || headerInfo.scriptContainsOrAlter)
							{
								break;
							}
							return false;
						}
						if (headerInfo.indexOrAlterStart == -1)
						{
							return false;
						}
						headerInfo.indexOrAlterEnd = m.Groups["word"].Index;
						headerInfo.scriptContainsOrAlter = true;
					}
				}
				else
				{
					if (string.Compare("alter", m.Groups["word"].Value, StringComparison.OrdinalIgnoreCase) != 0)
					{
						return false;
					}
					headerInfo.scriptForCreate = false;
					headerInfo.indexCreate = m.Groups["word"].Index;
				}
			}
			m = m.NextMatch();
		}
		while (m.Success)
		{
			if (m.Groups["word"].Success)
			{
				headerInfo.objectType = m.Groups["word"].Value;
				break;
			}
			m = m.NextMatch();
		}
		bool flag = ReadFullNameFromDdl(ref m, useQuotedIdentifier, ref headerInfo);
		if (!flag || string.Compare("TRIGGER", headerInfo.objectType, StringComparison.OrdinalIgnoreCase) != 0)
		{
			TraceHelper.Assert(headerInfo.objectType.Length > 0 && headerInfo.name.Length > 0 && headerInfo.indexCreate >= 0 && headerInfo.indexNameStart > 0 && headerInfo.indexNameEnd > headerInfo.indexNameStart && headerInfo.indexNameStartSecondary == -1);
			return flag;
		}
		while (m.Success)
		{
			if (m.Groups["word"].Success)
			{
				if (string.Compare("ON", m.Groups["word"].Value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					break;
				}
				return false;
			}
			m = m.NextMatch();
		}
		DdlTextParserHeaderInfo headerInfo2 = new DdlTextParserHeaderInfo
		{
			indexNameStart = -1,
			indexNameEnd = -1,
			schema = "",
			name = ""
		};
		flag = ReadFullNameFromDdl(ref m, useQuotedIdentifier, ref headerInfo2);
		headerInfo.indexNameStartSecondary = headerInfo2.indexNameStart;
		headerInfo.indexNameEndSecondary = headerInfo2.indexNameEnd;
		headerInfo.schemaSecondary = headerInfo2.schema;
		headerInfo.nameSecondary = headerInfo2.name;
		headerInfo.databaseSecondary = headerInfo2.database;
		TraceHelper.Assert(headerInfo.objectType.Length > 0 && headerInfo.name.Length > 0 && headerInfo.indexCreate >= 0 && headerInfo.indexNameStart > 0 && headerInfo.indexNameEnd > headerInfo.indexNameStart && headerInfo.nameSecondary.Length > 0 && headerInfo.indexNameStartSecondary > 0 && headerInfo.indexNameEndSecondary > headerInfo.indexNameStartSecondary);
		return flag;
	}

	public static int ParseDdlHeader(string objectText)
	{
		int num = 0;
		ddlTextParserSingleton.hasParanthesis = false;
		bool flag = false;
		ddlTextParserSingleton.returnTableVariableName = null;
		bool flag2 = false;
		bool flag3 = false;
		for (Match match = ddlTextParserSingleton.regex.Match(objectText); match.Success; match = match.NextMatch())
		{
			if (num > 0)
			{
				if (match.Groups["parant_close"].Success)
				{
					num--;
				}
				else if (match.Groups["parant_open"].Success)
				{
					num++;
				}
				if (num > 0)
				{
					continue;
				}
			}
			if (match.Groups["delim1"].Success)
			{
				if (!flag)
				{
					return match.Index + match.Length;
				}
				flag = false;
				continue;
			}
			if (match.Groups["delim2"].Success)
			{
				return match.Index;
			}
			if (match.Groups["exec"].Success)
			{
				flag = true;
			}
			else if (match.Groups["parant_open"].Success)
			{
				num++;
				ddlTextParserSingleton.hasParanthesis = true;
			}
			else if (match.Groups["returns"].Success)
			{
				flag2 = true;
			}
			else if (flag2 && match.Groups["param"].Success)
			{
				ddlTextParserSingleton.returnTableVariableName = match.Groups["param"].Value;
				flag2 = false;
			}
			else if (match.Groups["returns_table"].Success)
			{
				flag3 = true;
			}
			else if (flag3 && match.Groups["return"].Success)
			{
				return match.Index;
			}
		}
		return 0;
	}

	public static int ParseCheckOption(string ddlText)
	{
		string[] array = new string[3] { "option", "check", "with" };
		int num = 0;
		int num2 = 0;
		int result = -1;
		bool flag = false;
		Match match = ddlTextParserSingleton.m_r_end.Match(ddlText);
		while (match.Success)
		{
			if (!flag && 3 < num && match.Groups["reject"].Success)
			{
				flag = true;
			}
			if (!flag && 3 > num && match.Groups["word"].Success)
			{
				if (string.Compare(array[num++], match.Value, StringComparison.OrdinalIgnoreCase) != 0)
				{
					flag = true;
				}
				if (3 == num)
				{
					result = match.Index;
				}
			}
			if (match.Groups["slcomm"].Success)
			{
				num = num2;
				flag = false;
			}
			if (match.Groups["eol"].Success)
			{
				if (flag)
				{
					return -1;
				}
				if (num == 3)
				{
					return result;
				}
				num2 = num;
			}
			match = match.NextMatch();
		}
		if (flag)
		{
			return -1;
		}
		if (3 == num)
		{
			return result;
		}
		return -1;
	}
}
