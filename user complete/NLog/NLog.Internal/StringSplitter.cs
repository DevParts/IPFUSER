using System;
using System.Collections.Generic;
using System.Text;

namespace NLog.Internal;

/// <summary>
/// Split a string
/// </summary>
internal static class StringSplitter
{
	/// <summary>
	/// Split a string, optional quoted value
	/// </summary>
	/// <param name="text">Text to split</param>
	/// <param name="splitChar">Character to split the <paramref name="text" /></param>
	/// <param name="quoteChar">Quote character</param>
	/// <param name="escapeChar">
	/// Escape for the <paramref name="quoteChar" />, not escape for the <paramref name="splitChar" />
	/// , use quotes for that.
	/// </param>
	public static IEnumerable<string> SplitQuoted(this string text, char splitChar, char quoteChar, char escapeChar)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (splitChar == quoteChar)
			{
				throw new NotSupportedException("Quote character should different from split character");
			}
			if (splitChar == escapeChar)
			{
				throw new NotSupportedException("Escape character should different from split character");
			}
			return SplitQuoted2(text, splitChar, quoteChar, escapeChar);
		}
		return ArrayHelper.Empty<string>();
	}

	/// <summary>
	/// Split a string, optional quoted value
	/// </summary>
	/// <param name="text">Text to split</param>
	/// <param name="splitChar">Character to split the <paramref name="text" /></param>
	/// <param name="quoteChar">Quote character</param>
	/// <param name="escapeChar">
	/// Escape for the <paramref name="quoteChar" />, not escape for the <paramref name="splitChar" />
	/// , use quotes for that.
	/// </param>
	private static IEnumerable<string> SplitQuoted2(string text, char splitChar, char quoteChar, char escapeChar)
	{
		bool inQuotedMode = false;
		bool flag = false;
		bool flag2 = false;
		bool doubleQuotesEscapes = escapeChar == quoteChar;
		StringBuilder item = new StringBuilder();
		foreach (char c in text)
		{
			if (c == quoteChar)
			{
				if (inQuotedMode)
				{
					if (flag && !doubleQuotesEscapes)
					{
						item.Append(c);
						flag = false;
						flag2 = false;
					}
					else if (flag2 && doubleQuotesEscapes)
					{
						item.Append(c);
						inQuotedMode = false;
						flag = false;
						flag2 = false;
					}
					else if (item.Length > 0)
					{
						inQuotedMode = false;
						yield return item.ToString();
						item.Length = 0;
						flag = false;
						flag2 = true;
					}
					else
					{
						inQuotedMode = false;
						flag = false;
						flag2 = false;
					}
				}
				else if (item.Length != 0 || flag)
				{
					item.Append(c);
					flag = false;
					flag2 = false;
				}
				else
				{
					flag = c == escapeChar;
					flag2 = true;
					inQuotedMode = true;
				}
			}
			else if (c == escapeChar)
			{
				if (flag)
				{
					item.Append(escapeChar);
				}
				flag = true;
				flag2 = false;
			}
			else if (inQuotedMode)
			{
				item.Append(c);
				flag = false;
				flag2 = false;
			}
			else if (c == splitChar)
			{
				if (flag)
				{
					item.Append(escapeChar);
				}
				if (item.Length > 0 || !flag2)
				{
					yield return item.ToString();
					item.Length = 0;
				}
				flag = false;
				flag2 = false;
			}
			else
			{
				if (flag)
				{
					item.Append(escapeChar);
				}
				item.Append(c);
				flag = false;
				flag2 = false;
			}
		}
		if (flag && !doubleQuotesEscapes)
		{
			item.Append(escapeChar);
		}
		if (inQuotedMode)
		{
			if (flag2)
			{
				item.Append(quoteChar);
			}
			else
			{
				item.Insert(0, quoteChar);
			}
		}
		if (item.Length > 0 || !flag2)
		{
			yield return item.ToString();
		}
	}
}
