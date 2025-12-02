using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[Flags]
internal enum SfcElementFlags
{
	None = 0,
	Standalone = 0x10,
	SqlAzureDatabase = 0x20
}
