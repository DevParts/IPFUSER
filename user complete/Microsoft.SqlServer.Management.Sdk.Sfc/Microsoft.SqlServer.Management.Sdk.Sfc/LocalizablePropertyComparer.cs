using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class LocalizablePropertyComparer : IComparer
{
	public int Compare(object a, object b)
	{
		if (a == null || b == null || !(a is LocalizablePropertyDescriptor) || !(b is LocalizablePropertyDescriptor))
		{
			throw new ArgumentException();
		}
		LocalizablePropertyDescriptor localizablePropertyDescriptor = (LocalizablePropertyDescriptor)a;
		LocalizablePropertyDescriptor localizablePropertyDescriptor2 = (LocalizablePropertyDescriptor)b;
		int num = 0;
		if (localizablePropertyDescriptor.DisplayOrdinal < localizablePropertyDescriptor2.DisplayOrdinal)
		{
			return -1;
		}
		if (localizablePropertyDescriptor2.DisplayOrdinal < localizablePropertyDescriptor.DisplayOrdinal)
		{
			return 1;
		}
		return string.Compare(localizablePropertyDescriptor.DisplayName, localizablePropertyDescriptor2.DisplayName, StringComparison.CurrentCulture);
	}
}
