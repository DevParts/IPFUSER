using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[Flags]
public enum SfcObjectFlags
{
	None = 0,
	NaturalOrder = 0x10,
	Design = 0x20,
	Deploy = 0x40
}
