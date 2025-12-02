using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Differencing.SPI;

public class MetadataNodeItemNamesProvider : NodeItemNamesAdapterProvider
{
	public override bool IsGraphSupported(ISfcSimpleNode source)
	{
		if (source.ObjectReference is IAlienObject)
		{
			return true;
		}
		if (source.ObjectReference is SfcInstance)
		{
			return true;
		}
		return false;
	}

	public override bool IsContainerInNatrualOrder(ISfcSimpleNode node, string name)
	{
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(node.ObjectReference.GetType());
		foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
		{
			if (!name.Equals(relation.PropertyName))
			{
				continue;
			}
			foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
			{
				if (relationshipAttribute is SfcObjectAttribute { NaturalOrder: not false })
				{
					return true;
				}
			}
		}
		return false;
	}

	public override IEnumerable<string> GetRelatedContainerNames(ISfcSimpleNode source)
	{
		List<string> list = new List<string>();
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(source.ObjectReference.GetType());
		foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
		{
			switch (relation.Relationship)
			{
			case SfcRelationship.ObjectContainer:
			case SfcRelationship.ChildContainer:
				foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
				{
					if (relationshipAttribute is SfcObjectAttribute sfcObjectAttribute && (sfcObjectAttribute.Deploy || sfcObjectAttribute.Design))
					{
						list.Add(relation.PropertyName);
						break;
					}
				}
				break;
			}
		}
		return list;
	}

	public override IEnumerable<string> GetRelatedObjectNames(ISfcSimpleNode source)
	{
		List<string> list = new List<string>();
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(source.ObjectReference.GetType());
		foreach (SfcMetadataRelation relation in sfcMetadataDiscovery.Relations)
		{
			switch (relation.Relationship)
			{
			case SfcRelationship.Object:
			case SfcRelationship.ChildObject:
				foreach (Attribute relationshipAttribute in relation.RelationshipAttributes)
				{
					if (relationshipAttribute is SfcObjectAttribute sfcObjectAttribute && (sfcObjectAttribute.Deploy || sfcObjectAttribute.Design))
					{
						list.Add(relation.PropertyName);
						break;
					}
				}
				break;
			}
		}
		return list;
	}

	public override IEnumerable<string> GetPropertyNames(ISfcSimpleNode source)
	{
		List<string> list = new List<string>();
		SfcMetadataDiscovery sfcMetadataDiscovery = new SfcMetadataDiscovery(source.ObjectReference.GetType());
		List<SfcMetadataRelation> properties = sfcMetadataDiscovery.Properties;
		foreach (SfcMetadataRelation item in properties)
		{
			foreach (Attribute relationshipAttribute in item.RelationshipAttributes)
			{
				if (relationshipAttribute is SfcPropertyAttribute sfcPropertyAttribute && (sfcPropertyAttribute.Deploy || sfcPropertyAttribute.Design))
				{
					list.Add(item.PropertyName);
				}
			}
		}
		return list;
	}
}
