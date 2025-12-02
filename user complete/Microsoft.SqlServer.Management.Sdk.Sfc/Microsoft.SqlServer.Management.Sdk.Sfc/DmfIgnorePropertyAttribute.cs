using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class DmfIgnorePropertyAttribute : Attribute
{
}
