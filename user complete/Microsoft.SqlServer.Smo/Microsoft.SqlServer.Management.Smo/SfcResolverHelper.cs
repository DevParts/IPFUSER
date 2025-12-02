using System;
using System.Reflection;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal class SfcResolverHelper
{
	internal static Database GetDatabase(object obj)
	{
		object obj2 = obj;
		object obj3 = null;
		while (obj3 == null)
		{
			Type type = obj2.GetType();
			PropertyInfo property = type.GetProperty("Parent");
			TraceHelper.Assert(property != null, $"{type.FullName} is missing Parent property.");
			obj2 = property.GetValue(obj2, null);
			TraceHelper.Assert(obj2 != null, $"{type.FullName}.Parent property returned null.");
			if (obj2.GetType() == typeof(Database))
			{
				obj3 = obj2;
			}
		}
		return (Database)obj3;
	}

	internal static DataType GetDataType(object obj)
	{
		object obj2 = null;
		Type type = obj.GetType();
		PropertyInfo property = type.GetProperty("DataType");
		obj2 = property.GetValue(obj, null);
		if (obj2 != null && obj2.GetType() == typeof(DataType))
		{
			return (DataType)obj2;
		}
		return null;
	}

	internal static string GetSchemaName(object obj)
	{
		Type type = obj.GetType();
		PropertyInfo property = type.GetProperty("Schema");
		TraceHelper.Assert(property != null, $"{type.FullName} is missing Schema property.");
		string text = (string)property.GetValue(obj, null);
		TraceHelper.Assert(text != null, $"{type.FullName}.Schema property returned null.");
		return text;
	}
}
