using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

public class SfcMetadataRelation : SfcMetadataDiscovery
{
	private string m_propertyName;

	private SfcCardinality m_cardinality;

	private SfcRelationship m_relationship;

	private SfcPropertyFlags m_propertyFlags;

	private Type m_containerType;

	private object m_defaultValue;

	private AttributeCollection m_attributes;

	public string PropertyName => m_propertyName;

	public SfcCardinality Cardinality => m_cardinality;

	public SfcRelationship Relationship => m_relationship;

	public SfcPropertyFlags PropertyFlags => m_propertyFlags;

	public object PropertyDefaultValue => m_defaultValue;

	public AttributeCollection RelationshipAttributes => m_attributes;

	public Type ContainerType => m_containerType;

	internal bool IsSfcProperty
	{
		get
		{
			foreach (Attribute relationshipAttribute in RelationshipAttributes)
			{
				if (relationshipAttribute.GetType() == typeof(SfcPropertyAttribute))
				{
					return true;
				}
			}
			return false;
		}
	}

	public new bool IsBrowsable
	{
		get
		{
			SfcBrowsableAttribute[] array = (SfcBrowsableAttribute[])base.Type.GetCustomAttributes(typeof(SfcBrowsableAttribute), inherit: false);
			if (array.Length > 0)
			{
				return array[0].IsBrowsable;
			}
			if (Cardinality == SfcCardinality.None || Cardinality == SfcCardinality.One || Cardinality == SfcCardinality.ZeroToOne)
			{
				foreach (SfcMetadataRelation relation in Relations)
				{
					if (relation.Relationship != SfcRelationship.None && relation.Relationship != SfcRelationship.Ignore && relation.Relationship != SfcRelationship.ParentObject && relation.IsBrowsable)
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
	}

	public bool SupportsDesignMode => base.Type.GetInterface("ISfcSupportsDesignMode") != null;

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, SfcRelationship relationship, Type containerType, SfcPropertyFlags flags, object defaultValue, AttributeCollection attributes)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = cardinality;
		m_relationship = relationship;
		m_containerType = containerType;
		m_propertyFlags = flags;
		m_defaultValue = defaultValue;
		m_attributes = attributes;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, SfcRelationship relationship, Type containerType, SfcPropertyFlags flags, AttributeCollection attributes)
		: this(propertyName, type, cardinality, relationship, containerType, flags, null, attributes)
	{
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, SfcRelationship relationship, Type containerType, AttributeCollection attributes)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = cardinality;
		m_relationship = relationship;
		m_containerType = containerType;
		m_attributes = attributes;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, SfcRelationship relationship, Type containerType)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = cardinality;
		m_relationship = relationship;
		m_containerType = containerType;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, SfcRelationship relationship, AttributeCollection attributes)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = cardinality;
		m_relationship = relationship;
		m_attributes = attributes;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcCardinality cardinality, AttributeCollection attributes)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = cardinality;
		m_relationship = SfcRelationship.Object;
		m_attributes = attributes;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcPropertyFlags flags, AttributeCollection attributes)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = SfcCardinality.One;
		m_relationship = SfcRelationship.None;
		m_propertyFlags = flags;
		m_attributes = attributes;
	}

	public SfcMetadataRelation(string propertyName, Type type, SfcPropertyFlags flags)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = SfcCardinality.One;
		m_relationship = SfcRelationship.None;
		m_propertyFlags = flags;
	}

	public SfcMetadataRelation(string propertyName, Type type)
		: base(type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_propertyName = propertyName;
		m_cardinality = SfcCardinality.One;
		m_relationship = SfcRelationship.Object;
	}

	public object Resolve(object instance)
	{
		foreach (Attribute relationshipAttribute in RelationshipAttributes)
		{
			if (relationshipAttribute is SfcReferenceAttribute)
			{
				SfcReferenceAttribute sfcReferenceAttribute = relationshipAttribute as SfcReferenceAttribute;
				return sfcReferenceAttribute.Resolve(instance);
			}
		}
		PropertyInfo property = instance.GetType().GetProperty(PropertyName);
		TraceHelper.Assert(property != null);
		return property.GetValue(instance, null);
	}

	public T Resolve<T, S>(S instance)
	{
		foreach (Attribute relationshipAttribute in RelationshipAttributes)
		{
			if (relationshipAttribute is SfcReferenceAttribute)
			{
				SfcReferenceAttribute sfcReferenceAttribute = relationshipAttribute as SfcReferenceAttribute;
				return sfcReferenceAttribute.Resolve<T, S>(instance);
			}
		}
		PropertyInfo property = instance.GetType().GetProperty(PropertyName);
		TraceHelper.Assert(property != null);
		return (T)property.GetValue(instance, null);
	}

	public IEnumerable ResolveCollection(object instance)
	{
		foreach (Attribute relationshipAttribute in RelationshipAttributes)
		{
			if (relationshipAttribute is SfcReferenceCollectionAttribute)
			{
				SfcReferenceCollectionAttribute sfcReferenceCollectionAttribute = (SfcReferenceCollectionAttribute)relationshipAttribute;
				return sfcReferenceCollectionAttribute.ResolveCollection(instance);
			}
		}
		PropertyInfo property = instance.GetType().GetProperty(PropertyName);
		TraceHelper.Assert(property != null);
		return (IEnumerable)property.GetValue(instance, null);
	}

	public IEnumerable<T> ResolveCollection<T, S>(S instance)
	{
		foreach (Attribute relationshipAttribute in RelationshipAttributes)
		{
			if (relationshipAttribute is SfcReferenceCollectionAttribute)
			{
				SfcReferenceCollectionAttribute sfcReferenceCollectionAttribute = (SfcReferenceCollectionAttribute)relationshipAttribute;
				return sfcReferenceCollectionAttribute.ResolveCollection<T, S>(instance);
			}
		}
		PropertyInfo property = instance.GetType().GetProperty(PropertyName);
		TraceHelper.Assert(property != null);
		return (IEnumerable<T>)property.GetValue(instance, null);
	}
}
