namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcDomainLite : ISfcHasConnection
{
	string DomainName { get; }

	string DomainInstanceName { get; }

	int GetLogicalVersion();
}
