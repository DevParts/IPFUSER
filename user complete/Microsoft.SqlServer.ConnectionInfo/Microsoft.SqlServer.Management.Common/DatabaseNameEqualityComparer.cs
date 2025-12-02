using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Common;

internal class DatabaseNameEqualityComparer : IEqualityComparer<string>
{
	private ServerComparer serverComparer;

	public DatabaseNameEqualityComparer(ServerComparer serverComparer)
	{
		this.serverComparer = serverComparer;
	}

	bool IEqualityComparer<string>.Equals(string x, string y)
	{
		return ((IComparer<string>)serverComparer).Compare(x, y) == 0;
	}

	int IEqualityComparer<string>.GetHashCode(string s)
	{
		if ((serverComparer.CompareOptions & CompareOptions.IgnoreCase) == CompareOptions.IgnoreCase)
		{
			return s.StringToUpper(serverComparer.CultureInfo).GetHashCode();
		}
		return s.GetHashCode();
	}
}
