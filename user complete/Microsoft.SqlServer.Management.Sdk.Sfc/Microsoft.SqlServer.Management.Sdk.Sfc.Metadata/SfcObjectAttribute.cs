using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcObjectAttribute : SfcRelationshipAttribute
{
	private SfcObjectFlags m_flags;

	public SfcObjectFlags Flags
	{
		get
		{
			return m_flags;
		}
		set
		{
			m_flags = value;
		}
	}

	public bool Design
	{
		get
		{
			return (m_flags & SfcObjectFlags.Design) == SfcObjectFlags.Design;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcObjectFlags.Design) : (m_flags & ~SfcObjectFlags.Design));
		}
	}

	public bool Deploy
	{
		get
		{
			return (m_flags & SfcObjectFlags.Deploy) == SfcObjectFlags.Deploy;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcObjectFlags.Deploy) : (m_flags & ~SfcObjectFlags.Deploy));
		}
	}

	public bool NaturalOrder
	{
		get
		{
			return (m_flags & SfcObjectFlags.NaturalOrder) == SfcObjectFlags.NaturalOrder;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcObjectFlags.NaturalOrder) : (m_flags & ~SfcObjectFlags.NaturalOrder));
		}
	}

	public SfcObjectAttribute(SfcObjectFlags flags)
		: this(SfcObjectRelationship.Object, SfcObjectCardinality.One, flags)
	{
		Flags = flags;
	}

	public SfcObjectAttribute()
		: this(SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(SfcObjectCardinality cardinality, SfcObjectFlags flags)
		: this(SfcObjectRelationship.Object, cardinality)
	{
		Flags = flags;
	}

	public SfcObjectAttribute(SfcObjectCardinality cardinality)
		: this(cardinality, SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(SfcObjectRelationship relationship, SfcObjectFlags flags)
	{
		SfcRelationship relationship2 = SfcRelationship.None;
		switch (relationship)
		{
		case SfcObjectRelationship.ChildObject:
			relationship2 = SfcRelationship.ChildObject;
			break;
		case SfcObjectRelationship.Object:
			relationship2 = SfcRelationship.Object;
			break;
		case SfcObjectRelationship.ParentObject:
			relationship2 = SfcRelationship.ParentObject;
			break;
		}
		base.Relationship = relationship2;
		base.Cardinality = SfcCardinality.One;
		Flags = flags;
	}

	public SfcObjectAttribute(SfcObjectRelationship relationship)
		: this(relationship, SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(SfcObjectRelationship relationship, SfcObjectCardinality cardinality, SfcObjectFlags flags)
	{
		SfcRelationship relationship2 = SfcRelationship.None;
		SfcCardinality cardinality2 = SfcCardinality.None;
		switch (relationship)
		{
		case SfcObjectRelationship.ChildObject:
			relationship2 = SfcRelationship.ChildObject;
			break;
		case SfcObjectRelationship.Object:
			relationship2 = SfcRelationship.Object;
			break;
		case SfcObjectRelationship.ParentObject:
			relationship2 = SfcRelationship.ParentObject;
			break;
		}
		base.Relationship = relationship2;
		switch (cardinality)
		{
		case SfcObjectCardinality.One:
			cardinality2 = SfcCardinality.One;
			break;
		case SfcObjectCardinality.ZeroToOne:
			cardinality2 = SfcCardinality.ZeroToOne;
			break;
		}
		base.Cardinality = cardinality2;
		Flags = flags;
	}

	public SfcObjectAttribute(SfcObjectRelationship relationship, SfcObjectCardinality cardinality)
		: this(relationship, cardinality, SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(Type containsType, SfcObjectFlags flags)
	{
		base.Cardinality = SfcCardinality.ZeroToAny;
		base.ContainsType = containsType;
		base.Relationship = SfcRelationship.ObjectContainer;
		Flags = flags;
	}

	public SfcObjectAttribute(Type containsType)
		: this(containsType, SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(SfcContainerCardinality cardinality, Type containsType, SfcObjectFlags flags)
	{
		SfcCardinality cardinality2 = SfcCardinality.None;
		switch (cardinality)
		{
		case SfcContainerCardinality.OneToAny:
			cardinality2 = SfcCardinality.OneToAny;
			break;
		case SfcContainerCardinality.ZeroToAny:
			cardinality2 = SfcCardinality.ZeroToAny;
			break;
		}
		base.Cardinality = cardinality2;
		base.ContainsType = containsType;
		base.Relationship = SfcRelationship.ObjectContainer;
		Flags = flags;
	}

	public SfcObjectAttribute(SfcContainerCardinality cardinality, Type containsType)
		: this(cardinality, containsType, SfcObjectFlags.None)
	{
	}

	public SfcObjectAttribute(SfcContainerRelationship relationship, SfcContainerCardinality cardinality, Type containsType, SfcObjectFlags flags)
	{
		SfcRelationship relationship2 = SfcRelationship.None;
		SfcCardinality cardinality2 = SfcCardinality.None;
		switch (relationship)
		{
		case SfcContainerRelationship.ChildContainer:
			relationship2 = SfcRelationship.ChildContainer;
			break;
		case SfcContainerRelationship.ObjectContainer:
			relationship2 = SfcRelationship.ObjectContainer;
			break;
		}
		base.Relationship = relationship2;
		switch (cardinality)
		{
		case SfcContainerCardinality.OneToAny:
			cardinality2 = SfcCardinality.OneToAny;
			break;
		case SfcContainerCardinality.ZeroToAny:
			cardinality2 = SfcCardinality.ZeroToAny;
			break;
		}
		base.Cardinality = cardinality2;
		base.ContainsType = containsType;
		Flags = flags;
	}

	public SfcObjectAttribute(SfcContainerRelationship relationship, SfcContainerCardinality cardinality, Type containsType)
		: this(relationship, cardinality, containsType, SfcObjectFlags.None)
	{
	}
}
