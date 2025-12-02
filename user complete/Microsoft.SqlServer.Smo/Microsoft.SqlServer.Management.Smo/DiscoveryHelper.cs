namespace Microsoft.SqlServer.Management.Smo;

internal static class DiscoveryHelper
{
	internal static bool IsSystemObject(object obj)
	{
		if (obj is SqlSmoObject)
		{
			return ((SqlSmoObject)obj).IsSystemObjectInternal();
		}
		return false;
	}
}
