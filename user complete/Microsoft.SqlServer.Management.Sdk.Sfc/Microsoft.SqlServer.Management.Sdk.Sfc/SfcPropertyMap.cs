using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcPropertyMap : ISfcSimpleMap<string, object>
{
	private readonly object instance;

	private readonly SimpleNodeAdapter adapter;

	public object this[string key] => adapter.CheckedGetProperty(instance, key);

	public SfcPropertyMap(object reference, SimpleNodeAdapter adapter)
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
