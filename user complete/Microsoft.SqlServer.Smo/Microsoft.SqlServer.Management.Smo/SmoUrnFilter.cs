using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoUrnFilter : ISmoFilter
{
	internal HashSet<UrnTypeKey> filteredTypes;

	public Server Server { get; set; }

	public IEnumerable<Urn> Filter(IEnumerable<Urn> urns)
	{
		if (filteredTypes.Count == 0)
		{
			return urns;
		}
		List<Urn> list = new List<Urn>();
		foreach (Urn urn in urns)
		{
			if (!filteredTypes.Contains(new UrnTypeKey(urn)))
			{
				list.Add(urn);
			}
		}
		return list;
	}

	public SmoUrnFilter(Server srv)
	{
		Server = srv;
		filteredTypes = new HashSet<UrnTypeKey>();
	}

	public void AddFilteredType(string urnType, string parentType)
	{
		UrnTypeKey item = new UrnTypeKey(urnType, parentType);
		if (!filteredTypes.Contains(item))
		{
			filteredTypes.Add(item);
		}
	}

	public void RemoveFilteredType(string urnType, string parentType)
	{
		UrnTypeKey item = new UrnTypeKey(urnType, parentType);
		if (filteredTypes.Contains(item))
		{
			filteredTypes.Remove(item);
		}
	}
}
