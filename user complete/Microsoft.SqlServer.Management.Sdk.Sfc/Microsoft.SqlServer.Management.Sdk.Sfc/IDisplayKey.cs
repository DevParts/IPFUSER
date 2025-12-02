using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal interface IDisplayKey
{
	string Key { get; }

	string GetDefaultKey(PropertyInfo property);

	string GetDefaultKey(Type type);

	string GetDefaultKey(FieldInfo field);
}
