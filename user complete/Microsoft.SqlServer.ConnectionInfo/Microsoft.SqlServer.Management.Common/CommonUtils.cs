using System.Globalization;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

internal static class CommonUtils
{
	internal static string MakeSqlBraket(string s)
	{
		return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[1] { EscapeString(s, "]") });
	}

	internal static string MakeSqlString(string value)
	{
		if (value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("N'");
		stringBuilder.Append(EscapeString(value, "'"));
		stringBuilder.Append("'");
		return stringBuilder.ToString();
	}

	internal static string EscapeString(string s, string esc)
	{
		if (s == null)
		{
			return null;
		}
		string newValue = esc + esc;
		StringBuilder stringBuilder = new StringBuilder(s);
		stringBuilder.Replace(esc, newValue);
		return stringBuilder.ToString();
	}
}
