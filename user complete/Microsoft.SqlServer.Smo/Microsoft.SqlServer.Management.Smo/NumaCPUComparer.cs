using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

internal class NumaCPUComparer : IComparer
{
	int IComparer.Compare(object object1, object object2)
	{
		return (int)object1 - (int)object2;
	}
}
