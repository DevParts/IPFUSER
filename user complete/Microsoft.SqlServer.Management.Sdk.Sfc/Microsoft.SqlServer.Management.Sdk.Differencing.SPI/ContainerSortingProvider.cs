using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public abstract class ContainerSortingProvider : Provider
{
	public abstract bool AreGraphsSupported(ISfcSimpleNode source, ISfcSimpleNode target);

	public void SortLists(ISfcSimpleList source, ISfcSimpleList target, out IEnumerable<ISfcSimpleNode> sortedSource, out IEnumerable<ISfcSimpleNode> sortedTarget)
	{
		IEnumerable<ISfcSimpleNode> enumerable = null;
		IEnumerable<ISfcSimpleNode> enumerable2 = null;
		if (source.GetEnumerator().MoveNext())
		{
			List<ISfcSimpleNode> list = new List<ISfcSimpleNode>(source);
			list.Sort(GetComparer(source, source));
			enumerable = list;
		}
		else
		{
			enumerable = source;
		}
		if (target.GetEnumerator().MoveNext())
		{
			List<ISfcSimpleNode> list2 = new List<ISfcSimpleNode>(target);
			list2.Sort(GetComparer(target, target));
			enumerable2 = list2;
		}
		else
		{
			enumerable2 = target;
		}
		sortedSource = enumerable;
		sortedTarget = enumerable2;
	}

	public abstract IComparer<ISfcSimpleNode> GetComparer(ISfcSimpleList list, ISfcSimpleList list2);
}
