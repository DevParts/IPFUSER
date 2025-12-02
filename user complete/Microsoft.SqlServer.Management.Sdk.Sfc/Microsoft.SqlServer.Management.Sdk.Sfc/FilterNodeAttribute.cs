using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterNodeAttribute : FilterNode
{
	private string m_name;

	private static Dictionary<string, string> CachedNames = new Dictionary<string, string>(StringComparer.Ordinal);

	private static object CachedNamesLock = new object();

	public override Type NodeType => Type.Attribute;

	public string Name => m_name;

	public FilterNodeAttribute(string name)
	{
		if (CachedNames.TryGetValue(name, out m_name))
		{
			return;
		}
		lock (CachedNamesLock)
		{
			if (!CachedNames.TryGetValue(name, out m_name))
			{
				CachedNames.Add(name, name);
				m_name = name;
			}
		}
	}

	internal static bool Compare(FilterNodeAttribute a1, FilterNodeAttribute a2)
	{
		return 0 == string.Compare(a1.Name, a2.Name, StringComparison.Ordinal);
	}

	public override string ToString()
	{
		return "@" + Name;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}
}
