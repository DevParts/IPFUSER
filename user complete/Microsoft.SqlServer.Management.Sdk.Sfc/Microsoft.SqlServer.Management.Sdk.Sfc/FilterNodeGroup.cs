using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class FilterNodeGroup : FilterNodeChildren
{
	public override Type NodeType => Type.Group;

	public FilterNode Node => base.Children[0];

	internal FilterNodeGroup()
	{
	}

	public FilterNodeGroup(FilterNode node)
		: base(new FilterNode[1] { node })
	{
	}

	internal static bool Compare(FilterNodeGroup f1, FilterNodeGroup f2, CompareOptions compInfo, CultureInfo cultureInfo)
	{
		return FilterNodeChildren.Compare(f1, f2, compInfo, cultureInfo);
	}

	public override string ToString()
	{
		return "(" + Node.ToString() + ")";
	}
}
