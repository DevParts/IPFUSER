using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcDomain : ISfcDomainLite, ISfcHasConnection
{
	Type GetType(string typeName);

	SfcKey GetKey(IUrnFragment urnFragment);

	ISfcExecutionEngine GetExecutionEngine();

	SfcTypeMetadata GetTypeMetadata(string typeName);

	bool UseSfcStateManagement();
}
