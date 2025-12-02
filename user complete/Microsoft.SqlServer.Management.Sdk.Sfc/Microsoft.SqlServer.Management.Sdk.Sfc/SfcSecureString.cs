using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public static class SfcSecureString
{
	internal const char SmlEscaper = '_';

	private static Regex stringRegex = new Regex("<\\?char\\s\\d+\\?>");

	private static Regex numberRegex = new Regex("<\\?char(\\s)(?<number>(\\d+))\\?>");

	private static string EscapeImpl(string s, char cEsc)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in s)
		{
			stringBuilder.Append(c);
			if (cEsc == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static string EscapeSquote(string value)
	{
		return EscapeImpl(value, '\'');
	}

	public static string EscapeBracket(string value)
	{
		return EscapeImpl(value, ']');
	}

	public static string SmlEscape(string originalString)
	{
		if (string.IsNullOrEmpty(originalString))
		{
			return null;
		}
		new StringBuilder();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in originalString)
		{
			switch (c)
			{
			case '_':
				stringBuilder.Append('_');
				stringBuilder.Append('_');
				break;
			case '.':
				stringBuilder.Append('_');
				stringBuilder.Append('.');
				break;
			case '/':
				stringBuilder.Append('_');
				stringBuilder.Append('/');
				break;
			case '#':
				stringBuilder.Append('_');
				stringBuilder.Append('a');
				break;
			case ':':
				stringBuilder.Append('_');
				stringBuilder.Append('b');
				break;
			case '?':
				stringBuilder.Append('_');
				stringBuilder.Append('c');
				break;
			case '@':
				stringBuilder.Append('_');
				stringBuilder.Append('d');
				break;
			case '&':
				stringBuilder.Append("&amp;");
				break;
			case '>':
				stringBuilder.Append("&gt;");
				break;
			case '<':
				stringBuilder.Append("&lt;");
				break;
			case '\'':
				stringBuilder.Append("&apos;");
				break;
			case '"':
				stringBuilder.Append("&quot;");
				break;
			default:
				stringBuilder.Append(c);
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public static string SmlUnEscape(string escapedString)
	{
		if (string.IsNullOrEmpty(escapedString))
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		for (int i = 0; i < escapedString.Length; i++)
		{
			if (escapedString[i] == '_' && i + 1 < escapedString.Length)
			{
				switch (escapedString[++i])
				{
				case 'a':
					stringBuilder.Append('#');
					break;
				case 'b':
					stringBuilder.Append(':');
					break;
				case 'c':
					stringBuilder.Append('?');
					break;
				case 'd':
					stringBuilder.Append('@');
					break;
				default:
					stringBuilder.Append(escapedString[i]);
					break;
				}
			}
			else if (escapedString[i] == '&')
			{
				text = escapedString.Substring(i + 1);
				if (text.StartsWith("amp;", StringComparison.Ordinal))
				{
					stringBuilder.Append('&');
					i += "amp;".Length;
				}
				else if (text.StartsWith("gt;", StringComparison.Ordinal))
				{
					stringBuilder.Append('>');
					i += "gt;".Length;
				}
				else if (text.StartsWith("lt;", StringComparison.Ordinal))
				{
					stringBuilder.Append('<');
					i += "lt;".Length;
				}
				else if (text.StartsWith("apos;", StringComparison.Ordinal))
				{
					stringBuilder.Append('\'');
					i += "apos;".Length;
				}
				else if (text.StartsWith("amp;", StringComparison.Ordinal))
				{
					stringBuilder.Append('"');
					i += "amp;".Length;
				}
				else
				{
					stringBuilder.Append('&');
				}
			}
			else
			{
				stringBuilder.Append(escapedString[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public static string XmlEscape(string originalString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char value in originalString)
		{
			int num = Convert.ToInt32(value);
			if ((num >= 1 && num <= 8) || (num >= 11 && num <= 31) || (num >= 127 && num <= 132) || (num >= 134 && num <= 159))
			{
				stringBuilder.Append("<?char " + num + "?>");
			}
			else
			{
				stringBuilder.Append(value);
			}
		}
		return stringBuilder.ToString();
	}

	public static string XmlUnEscape(string escapedString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		escapedString = escapedString.Replace("<?char 13?>", "\r");
		MatchCollection matchCollection = numberRegex.Matches(escapedString);
		List<int> list = new List<int>();
		foreach (Match item in matchCollection)
		{
			list.Add(Convert.ToInt32(item.Groups["number"].ToString()));
		}
		int num = 0;
		string[] array = stringRegex.Split(escapedString);
		foreach (string value in array)
		{
			stringBuilder.Append(value);
			if (num < list.Count)
			{
				stringBuilder.Append(Convert.ToChar(list[num++]));
			}
		}
		return stringBuilder.ToString();
	}
}
