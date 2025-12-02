using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal sealed class SfcDependencyRootList : KeyedCollection<SfcKeyChain, SfcInstance>
{
	public SfcDependencyRootList()
	{
	}

	public SfcDependencyRootList(ICollection<SfcInstance> collection)
	{
		foreach (SfcInstance item in collection)
		{
			Add(item);
		}
	}

	protected override SfcKeyChain GetKeyForItem(SfcInstance obj)
	{
		return obj.KeyChain;
	}

	public bool TryGetValue(SfcKeyChain kc, out SfcInstance obj)
	{
		if (base.Dictionary == null)
		{
			obj = null;
			using (IEnumerator<SfcInstance> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SfcInstance current = enumerator.Current;
					if (kc == GetKeyForItem(current))
					{
						obj = current;
						return true;
					}
				}
			}
			return false;
		}
		return base.Dictionary.TryGetValue(kc, out obj);
	}
}
