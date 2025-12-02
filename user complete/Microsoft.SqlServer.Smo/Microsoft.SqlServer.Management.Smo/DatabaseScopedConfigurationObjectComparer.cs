using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class DatabaseScopedConfigurationObjectComparer : ObjectComparerBase
{
	internal DatabaseScopedConfigurationObjectComparer()
		: base(null)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return string.Compare((obj1 as SimpleObjectKey).Name, (obj2 as SimpleObjectKey).Name, StringComparison.OrdinalIgnoreCase);
	}
}
