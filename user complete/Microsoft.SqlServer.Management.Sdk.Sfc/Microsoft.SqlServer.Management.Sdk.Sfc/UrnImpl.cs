using System;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class UrnImpl : IUrn
{
	private string m_urn;

	private XPathExpression m_xpath;

	public XPathExpression XPathExpression
	{
		get
		{
			if (m_xpath == null)
			{
				m_xpath = new XPathExpression();
				m_xpath.Compile(Value);
			}
			return m_xpath;
		}
	}

	[XmlAttribute]
	public string Value
	{
		get
		{
			return m_urn;
		}
		set
		{
			m_xpath = null;
			m_urn = value;
		}
	}

	public string DomainInstanceName
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public UrnImpl()
	{
		m_xpath = null;
		m_urn = string.Empty;
	}

	public UrnImpl(string value)
	{
		m_xpath = null;
		m_urn = value;
	}
}
