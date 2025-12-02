using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class ObjectProperty
{
	private string m_name;

	private string m_type;

	private bool m_expensive;

	private bool m_readOnly;

	private bool m_extendedType;

	private bool m_readOnlyAfterCreation;

	private short m_keyIndex;

	private PropertyMode m_propMode;

	private ObjectPropertyUsages m_usage;

	private string m_defaultValue;

	private string m_referenceTemplate;

	private string m_referenceType;

	private string m_referenceKeys;

	private string m_referenceTemplateParameters;

	[XmlAttribute]
	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	[XmlAttribute]
	public string Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	[XmlAttribute]
	public bool ReadOnlyAfterCreation
	{
		get
		{
			return m_readOnlyAfterCreation;
		}
		set
		{
			m_readOnlyAfterCreation = value;
		}
	}

	[XmlAttribute]
	public short KeyIndex
	{
		get
		{
			return m_keyIndex;
		}
		set
		{
			m_keyIndex = value;
		}
	}

	[XmlAttribute]
	public PropertyMode PropertyMode
	{
		get
		{
			return m_propMode;
		}
		set
		{
			m_propMode = value;
		}
	}

	[XmlAttribute]
	public bool Expensive
	{
		get
		{
			return m_expensive;
		}
		set
		{
			m_expensive = value;
		}
	}

	[XmlAttribute]
	public bool ReadOnly
	{
		get
		{
			return m_readOnly;
		}
		set
		{
			m_readOnly = value;
		}
	}

	[XmlAttribute]
	public bool ExtendedType
	{
		get
		{
			return m_extendedType;
		}
		set
		{
			m_extendedType = value;
		}
	}

	[XmlAttribute]
	public ObjectPropertyUsages Usage
	{
		get
		{
			return m_usage;
		}
		set
		{
			m_usage = value;
		}
	}

	[XmlAttribute]
	public string DefaultValue
	{
		get
		{
			return m_defaultValue;
		}
		set
		{
			m_defaultValue = value;
		}
	}

	[XmlAttribute]
	public string ReferenceTemplate
	{
		get
		{
			return m_referenceTemplate;
		}
		set
		{
			m_referenceTemplate = value;
		}
	}

	[XmlAttribute]
	public string ReferenceType
	{
		get
		{
			return m_referenceType;
		}
		set
		{
			m_referenceType = value;
		}
	}

	[XmlAttribute]
	public string ReferenceKeys
	{
		get
		{
			return m_referenceKeys;
		}
		set
		{
			m_referenceKeys = value;
		}
	}

	[XmlAttribute]
	public string ReferenceTemplateParameters
	{
		get
		{
			return m_referenceTemplateParameters;
		}
		set
		{
			m_referenceTemplateParameters = value;
		}
	}
}
