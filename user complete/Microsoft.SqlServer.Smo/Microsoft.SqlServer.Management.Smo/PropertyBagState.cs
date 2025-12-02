namespace Microsoft.SqlServer.Management.Smo;

internal enum PropertyBagState
{
	Empty = 1,
	Lazy = 2,
	Full = 4,
	Unknown = 0x10
}
