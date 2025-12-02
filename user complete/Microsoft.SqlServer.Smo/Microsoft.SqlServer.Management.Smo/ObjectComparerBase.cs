using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class ObjectComparerBase : IComparer
{
	protected IComparer stringComparer;

	internal ObjectComparerBase(IComparer stringComparer)
	{
		this.stringComparer = stringComparer;
	}

	public virtual int Compare(object obj1, object obj2)
	{
		return 0;
	}
}
