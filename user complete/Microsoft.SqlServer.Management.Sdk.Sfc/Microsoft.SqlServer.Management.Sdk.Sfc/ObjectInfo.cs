using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class ObjectInfo
{
	private string[] m_children;

	private ObjectProperty[] m_properties;

	private ObjectProperty[] m_urnproperties;

	private ResultType[] m_resultTypes;

	[XmlElement]
	public string[] Children
	{
		get
		{
			return m_children;
		}
		set
		{
			m_children = value;
		}
	}

	[XmlElement]
	public ObjectProperty[] Properties
	{
		get
		{
			return m_properties;
		}
		set
		{
			m_properties = value;
		}
	}

	[XmlElement]
	public ResultType[] ResultTypes
	{
		get
		{
			return m_resultTypes;
		}
		set
		{
			m_resultTypes = value;
		}
	}

	public ObjectProperty[] UrnProperties
	{
		get
		{
			return m_urnproperties;
		}
		set
		{
			m_urnproperties = value;
		}
	}
}
