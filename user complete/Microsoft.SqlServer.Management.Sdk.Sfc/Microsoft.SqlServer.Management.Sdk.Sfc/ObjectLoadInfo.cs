using System.Collections;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class ObjectLoadInfo
{
	public string Name;

	public string Assembly;

	public string InitData;

	public string ImplementClass;

	public uint UniqueKey;

	public Assembly AssemblyReference;

	public bool typeAllowsRecursion;

	public SortedList Children;

	public ObjectLoadInfo()
	{
		Children = new SortedList();
	}
}
