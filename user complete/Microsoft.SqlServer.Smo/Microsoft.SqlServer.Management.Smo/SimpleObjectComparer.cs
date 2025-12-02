using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class SimpleObjectComparer : ObjectComparerBase
{
	internal SimpleObjectComparer(IComparer stringComparer)
		: base(stringComparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		return stringComparer.Compare((obj1 as SimpleObjectKey).Name, (obj2 as SimpleObjectKey).Name);
	}
}
