using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcContainerMap : ISfcSimpleMap<string, ISfcSimpleList>
{
	private readonly object instance;

	private readonly SimpleNodeAdapter adapter;

	public ISfcSimpleList this[string key] => new SfcChildren(instance, key, adapter);

	public SfcContainerMap(object reference, SimpleNodeAdapter adapter)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		if (adapter == null)
		{
			throw new ArgumentNullException("adapter");
		}
		instance = reference;
		this.adapter = adapter;
	}
}
