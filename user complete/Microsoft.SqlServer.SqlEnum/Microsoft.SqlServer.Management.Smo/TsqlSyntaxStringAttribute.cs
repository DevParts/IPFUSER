using System;
using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
internal sealed class TsqlSyntaxStringAttribute : DisplayNameAttribute
{
	private string syntaxString;

	public override string DisplayName => syntaxString;

	public TsqlSyntaxStringAttribute(string syntaxString)
	{
		this.syntaxString = syntaxString;
	}
}
