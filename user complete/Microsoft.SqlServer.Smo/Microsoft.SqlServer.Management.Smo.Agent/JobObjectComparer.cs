using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.Agent;

internal class JobObjectComparer : ObjectComparerBase
{
	internal JobObjectComparer(IComparer comparer)
		: base(comparer)
	{
	}

	public override int Compare(object obj1, object obj2)
	{
		JobObjectKey jobObjectKey = obj1 as JobObjectKey;
		JobObjectKey jobObjectKey2 = obj2 as JobObjectKey;
		if (stringComparer.Compare(jobObjectKey.Name, jobObjectKey2.Name) == 0 && jobObjectKey.CategoryID == jobObjectKey2.CategoryID)
		{
			return 0;
		}
		return 1;
	}
}
