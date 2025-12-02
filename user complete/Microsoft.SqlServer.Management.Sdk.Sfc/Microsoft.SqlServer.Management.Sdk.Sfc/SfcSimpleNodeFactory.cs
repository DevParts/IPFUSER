using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcSimpleNodeFactory
{
	private static SfcSimpleNodeFactory factory = new SfcSimpleNodeFactory();

	private static IList<SimpleNodeAdapter> DEFAULT_ADAPTERS = new List<SimpleNodeAdapter>(new SimpleNodeAdapter[2]
	{
		new SfcSimpleNodeAdapter(),
		new IAlienObjectAdapter()
	});

	public static SfcSimpleNodeFactory Factory => factory;

	public ISfcSimpleNode GetSimpleNode(object node)
	{
		foreach (SimpleNodeAdapter dEFAULT_ADAPTER in DEFAULT_ADAPTERS)
		{
			if (dEFAULT_ADAPTER.CheckedIsSupported(node))
			{
				return GetSimpleNode(node, dEFAULT_ADAPTER);
			}
		}
		throw new ArgumentException("node");
	}

	public ISfcSimpleNode GetSimpleNode(object node, SimpleNodeAdapter adapter)
	{
		if (adapter == null)
		{
			throw new ArgumentNullException("adapter");
		}
		if (!adapter.CheckedIsSupported(node))
		{
			throw new ArgumentException("adapter");
		}
		return new SfcSimpleNode(node, adapter);
	}

	public bool IsSupported(object node)
	{
		foreach (SimpleNodeAdapter dEFAULT_ADAPTER in DEFAULT_ADAPTERS)
		{
			if (dEFAULT_ADAPTER.CheckedIsSupported(node))
			{
				return true;
			}
		}
		return false;
	}
}
