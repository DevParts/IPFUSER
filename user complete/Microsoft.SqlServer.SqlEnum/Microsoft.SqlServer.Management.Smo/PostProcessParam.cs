using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessParam : PostProcess
{
	private const string sSingleLineCommentSql = "(--[^\n]*)";

	private const string sSingleLineCommentC = "(//[^\n]*)";

	private const string sNestedMultiLineComment = "(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)";

	private const string sNestedMultiLineCommentOptimized = "(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)";

	private const string sSingleQuotedStringSql = "(N{0,1}'(('')|[^'])*)'";

	private const string sDoubleQuotedString = "(\"((\"\")|[^\"])*\")";

	private const string sBraketedWord = "(\\[((\\]\\])|[^\\]])*\\])";

	private const string sWord = "([\\w_][\\w;\\d_]*)";

	private const string sNumber = "((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1})";

	private const string sHexNumber = "(0x[0-9a-f]+)";

	private const string sParamValueQI = "(?<val>(((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))";

	private const string sParamValue = "(?<val>(((\"((\"\")|[^\"])*\"))|((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))";

	private const string sComma = "(?<comma>,)";

	private const string sParanthesis = "(\\([\\d, ]*\\))";

	private const string sEq = "(?<eq>=)";

	private const string sDelim = "(?<delim>\\b((AS)|(RETURNS))\\b)";

	private const string sParam = "(?:(?<param>@[\\w_][\\w\\d_$$@#]*)((\\s)|((--[^\n]*))|((/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)))*(AS){0,1})";

	private const string sGrammarQI = "(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)|(--[^\n]*)|(\"((\"\")|[^\"])*\")|(//[^\n]*)|(?<delim>\\b((AS)|(RETURNS))\\b)|(?:(?<param>@[\\w_][\\w\\d_$$@#]*)((\\s)|((--[^\n]*))|((/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)))*(AS){0,1})|(?<val>(((\"((\"\")|[^\"])*\"))|((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))|(?<comma>,)|(?<eq>=)|(\\([\\d, ]*\\))";

	private const string sGrammar = "(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)|(--[^\n]*)|(//[^\n]*)|(?<delim>\\b((AS)|(RETURNS))\\b)|(?:(?<param>@[\\w_][\\w\\d_$$@#]*)((\\s)|((--[^\n]*))|((/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)))*(AS){0,1})|(?<val>(((\"((\"\")|[^\"])*\"))|((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))|(?<comma>,)|(?<eq>=)|(\\([\\d, ]*\\))";

	private SortedList m_textList;

	private Regex m_r;

	private static Regex sRegexQI;

	private static Regex sRegex;

	protected override bool SupportDataReader => false;

	public PostProcessParam()
	{
		m_textList = new SortedList();
	}

	static PostProcessParam()
	{
		sRegexQI = new Regex("(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)|(--[^\n]*)|(\"((\"\")|[^\"])*\")|(//[^\n]*)|(?<delim>\\b((AS)|(RETURNS))\\b)|(?:(?<param>@[\\w_][\\w\\d_$$@#]*)((\\s)|((--[^\n]*))|((/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)))*(AS){0,1})|(?<val>(((\"((\"\")|[^\"])*\"))|((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))|(?<comma>,)|(?<eq>=)|(\\([\\d, ]*\\))", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
		sRegex = new Regex("(/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)|(--[^\n]*)|(//[^\n]*)|(?<delim>\\b((AS)|(RETURNS))\\b)|(?:(?<param>@[\\w_][\\w\\d_$$@#]*)((\\s)|((--[^\n]*))|((/\\*(([^/\\*])|(\\*(?=[^/]))|(/(?=[^\\*])))*|(/\\*(?>/\\*(?<DEPTH>)|\\*/(?<-DEPTH>)|(.|[\n])?)*(?(DEPTH)(?!))\\*/)\\*/)))*(AS){0,1})|(?<val>(((\"((\"\")|[^\"])*\"))|((N{0,1}'(('')|[^'])*)')|((0x[0-9a-f]+))|(((\\+|\\-){0,1}((\\d+\\.\\d*)|(\\d*\\.\\d+)|(\\d+))(e((\\+)|(\\-))\\d+){0,1}))|((\\[((\\]\\])|[^\\]])*\\]))|(([\\w_][\\w;\\d_]*))))|(?<comma>,)|(?<eq>=)|(\\([\\d, ]*\\))", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
	}

	private void ParseParams(string sKey, string text, bool bQI)
	{
		m_textList[sKey] = new SortedList();
		if (m_r == null)
		{
			if (bQI)
			{
				m_r = sRegexQI;
			}
			else
			{
				m_r = sRegex;
			}
		}
		string text2 = null;
		bool flag = false;
		Match match = m_r.Match(text);
		while (match.Success && !match.Groups["delim"].Success)
		{
			if (match.Groups["eq"].Success)
			{
				flag = true;
			}
			if (match.Groups["comma"].Success)
			{
				flag = false;
				text2 = null;
			}
			if (flag && text2 != null && match.Groups["val"].Success)
			{
				((SortedList)m_textList[sKey])[text2] = match.Groups["val"].Value;
				flag = false;
			}
			if (match.Groups["param"].Success)
			{
				text2 = match.Groups["param"].Value;
			}
			match = match.NextMatch();
		}
	}

	private string GetText(int id, string sDatabase, int number, object ci, ref bool bQI)
	{
		string text = null;
		ServerVersion serverVersion = ExecuteSql.GetServerVersion(ci);
		text = ((serverVersion.Major >= 9) ? string.Format(CultureInfo.InvariantCulture, "select c.definition,convert(bit,OBJECTPROPERTY(c.object_id,N'ExecIsQuotedIdentOn')) from [{0}].sys.sql_modules c where c.object_id = <msparam>{1}</msparam>", new object[2]
		{
			Util.EscapeString(sDatabase, ']'),
			id
		}) : ((number <= 0) ? string.Format(CultureInfo.InvariantCulture, "select c.text,convert(bit,OBJECTPROPERTY(c.id,N'ExecIsQuotedIdentOn')) from [{0}].dbo.syscomments c where c.id = <msparam>{1}</msparam> order by c.colid", new object[2]
		{
			Util.EscapeString(sDatabase, ']'),
			id
		}) : string.Format(CultureInfo.InvariantCulture, "select c.text,convert(bit,OBJECTPROPERTY(c.id,N'ExecIsQuotedIdentOn')) from [{0}].dbo.syscomments c where c.id = <msparam>{1}</msparam> and c.number = <msparam>{2}</msparam> order by c.colid", new object[3]
		{
			Util.EscapeString(sDatabase, ']'),
			id,
			number
		})));
		DataTable dataTable = ExecuteSql.ExecuteWithResults(text, ci);
		string text2 = string.Empty;
		foreach (DataRow row in dataTable.Rows)
		{
			text2 += row[0].ToString();
		}
		if (dataTable.Rows.Count > 0)
		{
			object obj = dataTable.Rows[0][1];
			if (!(obj is DBNull))
			{
				bQI = bool.Parse(obj.ToString());
			}
		}
		return text2;
	}

	private bool IsProcessed(string sKey)
	{
		return null != m_textList[sKey];
	}

	private string GetParam(string sKey, string sParamName)
	{
		string text = (string)((SortedList)m_textList[sKey])[sParamName];
		if (text == null)
		{
			return string.Empty;
		}
		return text;
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (data != null && !(data is DBNull))
		{
			return data;
		}
		if (GetTriggeredBool(dp, 3))
		{
			return string.Empty;
		}
		int triggeredInt = GetTriggeredInt32(dp, 0);
		string triggeredString = GetTriggeredString(dp, 1);
		string triggeredString2 = GetTriggeredString(dp, 2);
		object triggeredObject = GetTriggeredObject(dp, 4);
		int num = 0;
		num = ((!(triggeredObject is int)) ? ((short)triggeredObject) : ((int)triggeredObject));
		string sKey = triggeredInt.ToString(CultureInfo.InvariantCulture) + triggeredString + num.ToString(CultureInfo.InvariantCulture);
		if (!IsProcessed(sKey))
		{
			bool bQI = false;
			string text = GetText(triggeredInt, triggeredString, num, base.ConnectionInfo, ref bQI);
			ParseParams(sKey, text, bQI);
		}
		return GetParam(sKey, triggeredString2);
	}
}
