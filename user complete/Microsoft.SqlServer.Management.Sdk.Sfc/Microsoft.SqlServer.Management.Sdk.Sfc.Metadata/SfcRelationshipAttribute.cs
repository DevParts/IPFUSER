using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class SfcRelationshipAttribute : Attribute
{
	private SfcRelationship m_relationship;

	private SfcCardinality m_cardinality;

	private Type m_containsType;

	public SfcRelationship Relationship
	{
		get
		{
			return m_relationship;
		}
		internal set
		{
			m_relationship = value;
		}
	}

	public SfcCardinality Cardinality
	{
		get
		{
			return m_cardinality;
		}
		internal set
		{
			m_cardinality = value;
		}
	}

	public Type ContainsType
	{
		get
		{
			return m_containsType;
		}
		internal set
		{
			m_containsType = value;
		}
	}

	protected SfcRelationshipAttribute()
	{
	}

	protected SfcRelationshipAttribute(SfcRelationship relationship)
	{
		m_relationship = relationship;
	}

	protected SfcRelationshipAttribute(SfcRelationship relationship, SfcCardinality cardinality)
	{
		m_relationship = relationship;
		m_cardinality = cardinality;
	}

	protected SfcRelationshipAttribute(SfcRelationship relationship, SfcCardinality cardinality, Type containsType)
	{
		m_relationship = relationship;
		m_cardinality = cardinality;
		m_containsType = containsType;
	}
}
