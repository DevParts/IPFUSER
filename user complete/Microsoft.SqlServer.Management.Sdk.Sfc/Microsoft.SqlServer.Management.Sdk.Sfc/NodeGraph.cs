using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class NodeGraph : KeyedCollection<SfcKeyChain, DepNode>
{
	protected override SfcKeyChain GetKeyForItem(DepNode node)
	{
		return node.Keychain;
	}

	public bool TryGetValue(SfcKeyChain kc, out DepNode node)
	{
		if (base.Dictionary == null)
		{
			node = null;
			using (IEnumerator<DepNode> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DepNode current = enumerator.Current;
					if (kc == GetKeyForItem(current))
					{
						node = current;
						return true;
					}
				}
			}
			return false;
		}
		return base.Dictionary.TryGetValue(kc, out node);
	}
}
