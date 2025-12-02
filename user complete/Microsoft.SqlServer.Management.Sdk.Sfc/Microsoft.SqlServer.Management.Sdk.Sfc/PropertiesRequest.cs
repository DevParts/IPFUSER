using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class PropertiesRequest
{
	private string[] m_fields;

	private RequestFieldsTypes m_fieldsType;

	private PropertyAlias m_PropertyAlias;

	private OrderBy[] m_orderBy;

	[XmlArray]
	[XmlArrayItem(ElementName = "field", Type = typeof(string))]
	public string[] Fields
	{
		get
		{
			return m_fields;
		}
		set
		{
			m_fields = value;
		}
	}

	[XmlAttribute]
	public RequestFieldsTypes RequestFieldsTypes
	{
		get
		{
			return m_fieldsType;
		}
		set
		{
			m_fieldsType = value;
		}
	}

	public OrderBy[] OrderByList
	{
		get
		{
			return m_orderBy;
		}
		set
		{
			m_orderBy = value;
		}
	}

	public PropertyAlias PropertyAlias
	{
		get
		{
			return m_PropertyAlias;
		}
		set
		{
			m_PropertyAlias = value;
		}
	}

	public PropertiesRequest()
	{
		m_fieldsType = RequestFieldsTypes.Request;
		m_PropertyAlias = new PropertyAlias();
	}

	public PropertiesRequest(string[] fields)
	{
		m_fieldsType = RequestFieldsTypes.Request;
		m_PropertyAlias = new PropertyAlias();
		Fields = fields;
	}

	public PropertiesRequest(string[] fields, OrderBy[] orderBy)
	{
		m_fieldsType = RequestFieldsTypes.Request;
		m_PropertyAlias = new PropertyAlias();
		Fields = fields;
		OrderByList = orderBy;
	}
}
