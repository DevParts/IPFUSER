using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
[CLSCompliant(false)]
public class SfcReferenceSelectorAttribute : Attribute
{
	private string[] m_args;

	private string m_pathExpression;

	private string m_field;

	public string PathExpression => m_pathExpression;

	public object[] Arguments => m_args;

	public string Field => m_field;

	public SfcReferenceSelectorAttribute(string pathExpression, string field, params string[] parameters)
	{
		m_pathExpression = pathExpression;
		m_field = field;
		m_args = parameters;
	}
}
