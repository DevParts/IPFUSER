using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class Request : PropertiesRequest
{
	private Urn m_urn;

	private ResultType m_resultType;

	private PropertiesRequest[] m_ParentPropertiesRequests;

	[XmlElement]
	public Urn Urn
	{
		get
		{
			return m_urn;
		}
		set
		{
			m_urn = value;
		}
	}

	public PropertiesRequest[] ParentPropertiesRequests
	{
		get
		{
			return m_ParentPropertiesRequests;
		}
		set
		{
			m_ParentPropertiesRequests = value;
		}
	}

	[XmlAttribute]
	public ResultType ResultType
	{
		get
		{
			return m_resultType;
		}
		set
		{
			m_resultType = value;
		}
	}

	public Request()
	{
		m_resultType = ResultType.Default;
		base.PropertyAlias = null;
	}

	public Request(Urn urn)
	{
		Urn = urn;
		m_resultType = ResultType.Default;
		base.PropertyAlias = null;
	}

	public Request(Urn urn, string[] fields)
		: base(fields, null)
	{
		Urn = urn;
		m_resultType = ResultType.Default;
		base.PropertyAlias = null;
	}

	public Request(Urn urn, string[] fields, OrderBy[] orderBy)
		: base(fields, orderBy)
	{
		Urn = urn;
		m_resultType = ResultType.Default;
		base.PropertyAlias = null;
	}

	internal Request ShallowClone()
	{
		Request request = new Request();
		request.Urn = Urn;
		request.Fields = base.Fields;
		request.OrderByList = base.OrderByList;
		request.ResultType = ResultType;
		request.PropertyAlias = base.PropertyAlias;
		request.ParentPropertiesRequests = ParentPropertiesRequests;
		request.RequestFieldsTypes = base.RequestFieldsTypes;
		request.PropertyAlias = base.PropertyAlias;
		return request;
	}
}
