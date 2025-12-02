using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ChildrenDiscoveryEventArgs : EventArgs
{
	public Urn Parent { get; private set; }

	public IEnumerable<Urn> Children { get; private set; }

	internal ChildrenDiscoveryEventArgs(Urn parent, IEnumerable<Urn> children)
	{
		Parent = parent;
		Children = children;
	}
}
