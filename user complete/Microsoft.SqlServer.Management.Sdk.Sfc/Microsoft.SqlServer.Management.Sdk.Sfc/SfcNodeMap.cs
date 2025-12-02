using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcNodeMap : ISfcSimpleMap<string, ISfcSimpleNode>
{
	private readonly object instance;

	private readonly SimpleNodeAdapter adapter;

	public ISfcSimpleNode this[string key]
	{
		get
		{
			object obj = adapter.CheckedGetObject(instance, key);
			if (obj != null)
			{
				return new SfcSimpleNode(obj, adapter);
			}
			return null;
		}
	}

	public SfcNodeMap(object reference, SimpleNodeAdapter adapter)
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
