using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class SfcSimpleNode : ISfcSimpleNode
{
	private readonly object instance;

	private readonly SimpleNodeAdapter adapter;

	private ISfcSimpleMap<string, object> properties;

	private ISfcSimpleMap<string, ISfcSimpleList> container;

	private ISfcSimpleMap<string, ISfcSimpleNode> objects;

	public object ObjectReference => instance;

	public Urn Urn => adapter.CheckedGetUrn(instance);

	public ISfcSimpleMap<string, object> Properties
	{
		get
		{
			if (properties == null)
			{
				properties = new SfcPropertyMap(instance, adapter);
			}
			return properties;
		}
	}

	public ISfcSimpleMap<string, ISfcSimpleList> RelatedContainers
	{
		get
		{
			if (container == null)
			{
				container = new SfcContainerMap(instance, adapter);
			}
			return container;
		}
	}

	public ISfcSimpleMap<string, ISfcSimpleNode> RelatedObjects
	{
		get
		{
			if (objects == null)
			{
				objects = new SfcNodeMap(instance, adapter);
			}
			return objects;
		}
	}

	internal SfcSimpleNode(object reference, SimpleNodeAdapter adapter)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		if (adapter == null)
		{
			throw new ArgumentNullException("adapter");
		}
		if (reference is IAlienObject || reference is SfcInstance)
		{
			instance = reference;
			this.adapter = adapter;
			return;
		}
		throw new ArgumentException("reference");
	}

	public override string ToString()
	{
		return instance.ToString();
	}
}
