using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[Flags]
public enum SfcPropertyFlags
{
	None = 0,
	Required = 0x10,
	Expensive = 0x20,
	Computed = 0x40,
	Encrypted = 0x80,
	ReadOnlyAfterCreation = 0x100,
	Data = 0x200,
	Standalone = 0x400,
	SqlAzureDatabase = 0x800,
	Design = 0x1000,
	Deploy = 0x2000
}
