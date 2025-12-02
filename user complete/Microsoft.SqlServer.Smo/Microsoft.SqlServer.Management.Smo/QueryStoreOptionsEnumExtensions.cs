using System;

namespace Microsoft.SqlServer.Management.Smo;

internal static class QueryStoreOptionsEnumExtensions
{
	internal static string ToSyntaxString(this QueryStoreOperationMode queryStoreOperationMode)
	{
		return queryStoreOperationMode switch
		{
			QueryStoreOperationMode.Off => "OFF", 
			QueryStoreOperationMode.ReadOnly => "READ_ONLY", 
			QueryStoreOperationMode.ReadWrite => "READ_WRITE", 
			_ => throw new InvalidOperationException($"Unknown QueryStoreOperationMode enum value {queryStoreOperationMode}"), 
		};
	}

	internal static string ToSyntaxString(this QueryStoreCaptureMode queryStoreCaptureMode)
	{
		return queryStoreCaptureMode switch
		{
			QueryStoreCaptureMode.All => "ALL", 
			QueryStoreCaptureMode.Auto => "AUTO", 
			QueryStoreCaptureMode.None => "NONE", 
			_ => throw new InvalidOperationException($"Unknown QueryStoreCaptureMode enum value {queryStoreCaptureMode}"), 
		};
	}

	internal static string ToSyntaxString(this QueryStoreSizeBasedCleanupMode queryStoreSizeBasedCleanupMode)
	{
		return queryStoreSizeBasedCleanupMode switch
		{
			QueryStoreSizeBasedCleanupMode.Off => "OFF", 
			QueryStoreSizeBasedCleanupMode.Auto => "AUTO", 
			_ => throw new InvalidOperationException($"Unknown QueryStoreSizeBasedCleanupMode enum value {queryStoreSizeBasedCleanupMode}"), 
		};
	}
}
