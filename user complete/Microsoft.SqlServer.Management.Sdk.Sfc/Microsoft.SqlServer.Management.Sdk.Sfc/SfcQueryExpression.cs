using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcQueryExpression : IXmlSerializable
{
	private Urn expression;

	public XPathExpression Expression => expression.XPathExpression;

	public string ExpressionSkeleton => expression.XPathExpression.ExpressionSkeleton;

	public SfcQueryExpression()
	{
	}

	public SfcQueryExpression(string path)
	{
		expression = new Urn(path);
		if (!expression.IsValidUrn())
		{
			throw new SfcInvalidQueryExpressionException();
		}
	}

	public void SetExpression(XPathExpression expression)
	{
		this.expression = new Urn(expression.ToString());
		if (!this.expression.IsValidUrn())
		{
			throw new SfcInvalidQueryExpressionException();
		}
	}

	public override string ToString()
	{
		return expression.ToString();
	}

	internal Urn ToUrn()
	{
		return expression;
	}

	public string GetLeafTypeName()
	{
		if (expression.ToString().StartsWith("Server", StringComparison.Ordinal))
		{
			TraceHelper.Assert(condition: false);
		}
		return expression.Type;
	}

	XmlSchema IXmlSerializable.GetSchema()
	{
		return null;
	}

	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		expression = new Urn(SfcSecureString.XmlUnEscape(reader.ReadElementContentAsString()));
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		writer.WriteRaw("<SfcQueryExpression>");
		writer.WriteRaw(Expression.ToString());
		writer.WriteRaw("</SfcQueryExpression>");
	}
}
