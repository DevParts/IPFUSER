using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
public class SqlSupport
{
	private SqlSupport()
	{
	}

	public static CompareOptions GetCompareOptionsFromCollation(string collation)
	{
		CompareOptions compareOptions = CompareOptions.None;
		if (collation.Length == 0)
		{
			compareOptions |= CompareOptions.IgnoreCase;
		}
		else
		{
			string[] array = collation.Split('_');
			if (array != null)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					switch (array2[i])
					{
					case "CI":
						flag = false;
						break;
					case "AI":
						flag2 = false;
						break;
					case "KS":
						flag3 = true;
						break;
					case "WS":
						flag4 = true;
						break;
					case "BIN":
					case "BIN2":
						flag5 = true;
						break;
					}
				}
				if (flag5)
				{
					compareOptions = CompareOptions.Ordinal;
				}
				else
				{
					if (!flag)
					{
						compareOptions |= CompareOptions.IgnoreCase;
					}
					if (!flag2)
					{
						compareOptions |= CompareOptions.IgnoreNonSpace;
					}
					if (!flag3)
					{
						compareOptions |= CompareOptions.IgnoreKanaType;
					}
					if (!flag4)
					{
						compareOptions |= CompareOptions.IgnoreWidth;
					}
				}
			}
		}
		return compareOptions;
	}
}
