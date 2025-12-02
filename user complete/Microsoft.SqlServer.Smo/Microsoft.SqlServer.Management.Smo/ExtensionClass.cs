using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal static class ExtensionClass
{
	internal static void AddCollection(this StringCollection strcol1, StringCollection strcol2)
	{
		StringEnumerator enumerator = strcol2.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				strcol1.Add(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	internal static void AddCollection(this StringCollection collection, IEnumerable<string> enumerableString)
	{
		foreach (string item in enumerableString)
		{
			collection.Add(item);
		}
	}
}
