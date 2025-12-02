namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal interface IUrn
{
	XPathExpression XPathExpression { get; }

	string Value { get; set; }

	string DomainInstanceName { get; }
}
