using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Common;

internal interface ICommonDisplayKey
{
	string Key { get; }

	string GetDefaultKey(PropertyInfo property);

	string GetDefaultKey(Type type);

	string GetDefaultKey(FieldInfo field);
}
