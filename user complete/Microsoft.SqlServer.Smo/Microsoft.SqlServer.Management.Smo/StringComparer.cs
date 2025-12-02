using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

internal class StringComparer : IComparer, IComparer<string>, IEqualityComparer<string>
{
	private CultureInfo m_cultureInfo;

	private CompareOptions m_options;

	internal CultureInfo CultureInfo => m_cultureInfo;

	internal CompareOptions CompareOptions => m_options;

	internal StringComparer(string collation, int lcid)
	{
		ChangeCollation(collation, lcid);
	}

	internal void ChangeCollation(string newCollation, int lcid)
	{
		m_options = CompareOptions.None;
		if (newCollation.Length == 0)
		{
			m_options |= CompareOptions.IgnoreCase;
		}
		else
		{
			string[] array = newCollation.Split('_');
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
					m_options = CompareOptions.Ordinal;
				}
				else
				{
					if (!flag)
					{
						m_options |= CompareOptions.IgnoreCase;
					}
					if (!flag2)
					{
						m_options |= CompareOptions.IgnoreNonSpace;
					}
					if (!flag3)
					{
						m_options |= CompareOptions.IgnoreKanaType;
					}
					if (!flag4)
					{
						m_options |= CompareOptions.IgnoreWidth;
					}
				}
			}
		}
		m_cultureInfo = new CultureInfo(lcid);
	}

	public int Compare(object x, object y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x != null && y == null)
		{
			return 1;
		}
		if (x == null && y != null)
		{
			return -1;
		}
		return m_cultureInfo.CompareInfo.Compare((string)x, (string)y, m_options);
	}

	public int Compare(string x, string y)
	{
		return m_cultureInfo.CompareInfo.Compare(x, y, m_options);
	}

	public bool Equals(string x, string y)
	{
		return Compare(x, y) == 0;
	}

	public int GetHashCode(string obj)
	{
		return obj.GetHashCode();
	}
}
