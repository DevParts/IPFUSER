using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal static class PermissionDecode
{
	private static IDictionary<string, object> permissionTypeToEnumMapping = new Dictionary<string, object>();

	internal static T ToPermissionSetValueEnum<T>(string permissionType) where T : struct, IConvertible
	{
		string key = permissionType + typeof(T);
		if (permissionTypeToEnumMapping.ContainsKey(key))
		{
			return (T)permissionTypeToEnumMapping[key];
		}
		foreach (T value in Enum.GetValues(typeof(T)))
		{
			if (SqlEnumNetCoreExtension.GetCustomAttributes(typeof(T).GetMember(value.ToString())[0], typeof(PermissionTypeAttribute), inherit: false).FirstOrDefault() is PermissionTypeAttribute permissionTypeAttribute && permissionTypeAttribute.Value.Equals(permissionType.TrimEnd(' ')))
			{
				permissionTypeToEnumMapping[key] = value;
				return value;
			}
		}
		throw new ArgumentException(StringSqlEnumerator.UnknownPermissionType(permissionType));
	}

	internal static string PermissionCodeToPermissionName<T>(int permissionCode) where T : struct, IConvertible
	{
		if (Enum.IsDefined(typeof(T), permissionCode))
		{
			T val = (T)Enum.ToObject(typeof(T), permissionCode);
			return val.PermissionName();
		}
		TraceHelper.Trace("SqlEnum.PermissionDecode.PermissionCodeToPermissionName", 1073741824u, "Undefined permission code {0} - has it been added to {1}?", permissionCode, typeof(T));
		throw new InvalidOperationException(StringSqlEnumerator.UnknownPermissionCode(permissionCode));
	}

	internal static string PermissionCodeToPermissionType<T>(int permissionCode) where T : struct, IConvertible
	{
		if (Enum.IsDefined(typeof(T), permissionCode))
		{
			T val = (T)Enum.ToObject(typeof(T), permissionCode);
			return val.PermissionType();
		}
		TraceHelper.Trace("SqlEnum.PermissionDecode.PermissionCodeToPermissionType", 1073741824u, "Undefined permission code {0} - has it been added to {1}?", permissionCode, typeof(T));
		throw new InvalidOperationException(StringSqlEnumerator.UnknownPermissionCode(permissionCode));
	}

	internal static string PermissionName<T>(this T val) where T : struct, IConvertible
	{
		PermissionNameAttribute permissionNameAttribute = SqlEnumNetCoreExtension.GetCustomAttributes(typeof(T).GetMember(val.ToString())[0], typeof(PermissionNameAttribute), inherit: false).FirstOrDefault() as PermissionNameAttribute;
		TraceHelper.Trace("SqlEnum.PermissionDecode.PermissionName", 1073741824u, "{0} doesn't have a PermissionName attribute defined", val);
		if (permissionNameAttribute != null)
		{
			return permissionNameAttribute.Value;
		}
		return string.Empty;
	}

	internal static string PermissionType<T>(this T val) where T : struct, IConvertible
	{
		PermissionTypeAttribute permissionTypeAttribute = SqlEnumNetCoreExtension.GetCustomAttributes(typeof(T).GetMember(val.ToString())[0], typeof(PermissionTypeAttribute), inherit: false).FirstOrDefault() as PermissionTypeAttribute;
		TraceHelper.Trace("SqlEnum.PermissionDecode.PermissionName", 1073741824u, "{0} doesn't have a PermissionType attribute defined", val);
		if (permissionTypeAttribute != null)
		{
			return permissionTypeAttribute.Value;
		}
		return string.Empty;
	}
}
