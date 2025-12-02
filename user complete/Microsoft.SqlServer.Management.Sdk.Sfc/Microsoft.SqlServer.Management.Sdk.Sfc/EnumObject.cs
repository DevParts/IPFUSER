using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class EnumObject
{
	private SortedList m_properties;

	private StringCollection m_propertyNames;

	private Request m_req;

	private XPathExpressionBlock m_block;

	private object m_ci;

	private Urn m_Urn;

	protected internal string Name => Urn.Type;

	protected internal Urn Urn
	{
		get
		{
			return m_Urn;
		}
		set
		{
			m_Urn = value;
		}
	}

	protected SortedList FixedProperties => m_block.FixedProperties;

	public Request Request
	{
		get
		{
			return m_req;
		}
		set
		{
			m_req = value;
		}
	}

	public object ConnectionInfo
	{
		get
		{
			return m_ci;
		}
		set
		{
			m_ci = value;
		}
	}

	public FilterNode Filter
	{
		get
		{
			return m_block.Filter;
		}
		set
		{
			m_block = m_block.Copy();
			m_block.Filter = value;
		}
	}

	public abstract ResultType[] ResultTypes { get; }

	protected EnumObject()
	{
		m_properties = new SortedList(StringComparer.Ordinal);
		m_propertyNames = new StringCollection();
	}

	public virtual void Initialize(object ci, XPathExpressionBlock block)
	{
		ConnectionInfo = ci;
		m_block = block;
	}

	protected string GetFixedStringProperty(string propertyName, bool removeEscape)
	{
		string text = (FilterNodeConstant)FixedProperties[propertyName];
		if (removeEscape && text != null)
		{
			text = Urn.UnEscapeString(text);
		}
		return text;
	}

	protected void AddProperty(ObjectProperty op)
	{
		m_properties.Add(op.Name, op);
		m_propertyNames.Add(op.Name);
	}

	public ObjectProperty[] GetProperties(ObjectPropertyUsages usage)
	{
		int num = 0;
		ObjectProperty[] array = new ObjectProperty[m_propertyNames.Count];
		for (int i = 0; i < m_propertyNames.Count; i++)
		{
			ObjectProperty objectProperty = (ObjectProperty)m_properties[m_propertyNames[i]];
			if ((usage & objectProperty.Usage) != ObjectPropertyUsages.None)
			{
				array[num++] = objectProperty;
			}
		}
		ObjectProperty[] array2 = new ObjectProperty[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = array[j];
		}
		return array2;
	}

	public ObjectProperty[] GetUrnProperties()
	{
		ArrayList arrayList = new ArrayList();
		ObjectProperty objectProperty = TryGetProperty("Urn", ObjectPropertyUsages.Request);
		if (objectProperty == null)
		{
			return null;
		}
		objectProperty = TryGetProperty("Name", ObjectPropertyUsages.Request);
		if (objectProperty != null)
		{
			arrayList.Add(objectProperty);
			objectProperty = TryGetProperty("Schema", ObjectPropertyUsages.Request);
			if (objectProperty != null)
			{
				arrayList.Add(objectProperty);
			}
		}
		else
		{
			ObjectProperty[] properties = GetProperties(ObjectPropertyUsages.Request);
			if (properties.Length > 1)
			{
				arrayList.Add(properties[1]);
			}
		}
		ObjectProperty[] array = new ObjectProperty[arrayList.Count];
		for (int i = 0; i < arrayList.Count; i++)
		{
			array[i] = (ObjectProperty)arrayList[i];
		}
		return array;
	}

	protected ObjectProperty TryGetProperty(string name, ObjectPropertyUsages usage)
	{
		ObjectProperty objectProperty = (ObjectProperty)m_properties[name];
		if (objectProperty != null)
		{
			ObjectPropertyUsages objectPropertyUsages = (ObjectPropertyUsages)((objectProperty.Usage | usage) - objectProperty.Usage);
			if (objectPropertyUsages != ObjectPropertyUsages.None && ObjectPropertyUsages.Reserved1 != objectPropertyUsages)
			{
				return null;
			}
		}
		return objectProperty;
	}

	protected ObjectProperty GetProperty(string name, ObjectPropertyUsages usage)
	{
		ObjectProperty objectProperty = (ObjectProperty)m_properties[name];
		if (objectProperty == null)
		{
			throw new UnknownPropertyEnumeratorException(name);
		}
		ObjectPropertyUsages objectPropertyUsages = (ObjectPropertyUsages)((objectProperty.Usage | usage) - objectProperty.Usage);
		if (objectPropertyUsages != ObjectPropertyUsages.None && ObjectPropertyUsages.Reserved1 != objectPropertyUsages)
		{
			InvalidPropertyUsageEnumeratorException.Throw(name, objectPropertyUsages);
		}
		return objectProperty;
	}

	public bool ComputeFixedProperties()
	{
		return 0 != m_block.FixedProperties.Count;
	}

	protected internal string GetAliasPropertyName(string prop)
	{
		if (Request == null || Request.PropertyAlias == null)
		{
			return prop;
		}
		switch (Request.PropertyAlias.Kind)
		{
		case PropertyAlias.AliasKind.Prefix:
			if (Request.PropertyAlias.Prefix == null)
			{
				throw new InternalEnumeratorException(SfcStrings.InvalidPrefixAlias(prop));
			}
			return Request.PropertyAlias.Prefix + prop;
		case PropertyAlias.AliasKind.NodeName:
			return Name + '_' + prop;
		case PropertyAlias.AliasKind.Each:
		{
			int i;
			for (i = 0; i < Request.Fields.Length && !(prop == Request.Fields[i]); i++)
			{
				if (i == Request.Fields.Length)
				{
					throw new InternalEnumeratorException(SfcStrings.PropertyCannotHaveAlias(prop));
				}
				if (Request.PropertyAlias.Aliases == null || Request.PropertyAlias.Aliases.Length <= i || Request.PropertyAlias.Aliases[i] == null)
				{
					throw new InternalEnumeratorException(SfcStrings.AliasNotSpecified(prop));
				}
			}
			return Request.PropertyAlias.Aliases[i];
		}
		default:
			throw new InternalEnumeratorException(SfcStrings.InvalidAlias);
		}
	}

	public virtual Request RetrieveParentRequest()
	{
		return null;
	}

	public abstract EnumResult GetData(EnumResult erParent);

	public virtual void PostProcess(EnumResult erChildren)
	{
	}
}
