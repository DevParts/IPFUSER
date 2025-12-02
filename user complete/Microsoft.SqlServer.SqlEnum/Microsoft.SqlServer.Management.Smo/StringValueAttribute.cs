using System;

namespace Microsoft.SqlServer.Management.Smo;

[AttributeUsage(AttributeTargets.Field)]
internal abstract class StringValueAttribute : Attribute
{
	private readonly string _value;

	public string Value => _value;

	protected StringValueAttribute(string value)
	{
		_value = value;
	}
}
