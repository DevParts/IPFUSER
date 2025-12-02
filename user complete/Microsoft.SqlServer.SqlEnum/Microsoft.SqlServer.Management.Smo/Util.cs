using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

internal class Util
{
	public static string DbTypeToClrType(string strDBType)
	{
		switch (strDBType)
		{
		case "xml":
		case "nvarchar":
		case "varchar":
		case "sysname":
		case "nchar":
		case "char":
		case "ntext":
		case "text":
			return "System.String";
		case "int":
			return "System.Int32";
		case "bigint":
			return "System.Int64";
		case "bit":
			return "System.Boolean";
		case "long":
			return "System.Int32";
		case "real":
		case "float":
			return "System.Double";
		case "datetime":
		case "datetime2":
		case "date":
			return "System.DateTime";
		case "datetimeoffset":
			return "System.DateTimeOffset";
		case "time":
		case "timespan":
			return "System.TimeSpan";
		case "tinyint":
			return "System.Byte";
		case "smallint":
			return "System.Int16";
		case "uniqueidentifier":
			return "System.Guid";
		case "numeric":
		case "decimal":
			return "System.Decimal";
		case "binary":
		case "image":
		case "varbinary":
			return "System.Byte[]";
		case "sql_variant":
			return "System.Object";
		default:
			throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.UnknownType(strDBType));
		}
	}

	protected EnumResult TransformToRequest(DataSet ds, ResultType res)
	{
		if (res == ResultType.Default)
		{
			res = ResultType.DataSet;
		}
		if (ResultType.DataSet == res)
		{
			return new EnumResult(ds, res);
		}
		if (ResultType.DataTable == res)
		{
			return new EnumResult(ds.Tables[0], res);
		}
		throw new ResultTypeNotSupportedEnumeratorException(res);
	}

	public static string EscapeString(string value, char escapeCharacter)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in value)
		{
			stringBuilder.Append(c);
			if (escapeCharacter == c)
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	internal static string MakeSqlString(string value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("N'");
		stringBuilder.Append(EscapeString(value, '\''));
		stringBuilder.Append("'");
		return stringBuilder.ToString();
	}

	internal static Assembly LoadAssembly(string assemblyName)
	{
		Assembly assembly = null;
		try
		{
			string fullName = SqlEnumNetCoreExtension.GetAssembly(typeof(Util)).FullName;
			fullName = fullName.Replace(SqlEnumNetCoreExtension.GetAssembly(typeof(Util)).GetName().Name, assemblyName);
			assembly = Assembly.Load(new AssemblyName(fullName));
		}
		catch (Exception ex)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.FailedToLoadAssembly(assemblyName) + "\n\n" + ex.ToString());
		}
		if (null == assembly)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.FailedToLoadAssembly(assemblyName));
		}
		return assembly;
	}

	internal static object CreateObjectInstance(Assembly assembly, string objectType)
	{
		object obj = SqlEnumNetCoreExtension.CreateInstance(assembly, objectType, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture, null);
		if (obj == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.CouldNotInstantiateObj(objectType));
		}
		return obj;
	}

	internal static string UnEscapeString(string escapedValue, char startEscapeChar, char escapeChar, ref int index)
	{
		return UnEscapeString(escapedValue, startEscapeChar, escapeChar, '\0', ref index);
	}

	internal static string UnEscapeString(string escapedValue, char startEscapeChar, char escapeChar, char partSeperator, ref int index)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		bool flag2 = false;
		for (; index < escapedValue.Length; index++)
		{
			char c = escapedValue[index];
			if (!flag2 && startEscapeChar == c)
			{
				flag2 = true;
				continue;
			}
			if (escapeChar == c)
			{
				if (!flag)
				{
					flag = true;
					continue;
				}
				flag = false;
			}
			else if ((flag && flag2) || (c == partSeperator && !flag2))
			{
				break;
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	internal static StringCollection SplitNames(string name)
	{
		return SplitNames(name, '\0');
	}

	internal static StringCollection SplitNames(string name, char partSeperator)
	{
		if (name == null)
		{
			return null;
		}
		StringCollection stringCollection = new StringCollection();
		int index = -1;
		while (index < name.Length)
		{
			index++;
			string value = UnEscapeString(name, '[', ']', partSeperator, ref index);
			stringCollection.Insert(0, value);
		}
		return stringCollection;
	}

	internal static string EscapeLikePattern(string pattern)
	{
		StringBuilder stringBuilder = null;
		for (int i = 0; i < pattern.Length; i++)
		{
			bool flag = false;
			char c = pattern[i];
			if (c == '%' || c == '[' || c == '_')
			{
				flag = true;
			}
			if (flag && stringBuilder == null)
			{
				stringBuilder = new StringBuilder(pattern.Length * 3);
				stringBuilder.Append(pattern.Substring(0, i));
			}
			TraceHelper.Assert(!flag || stringBuilder != null);
			if (flag)
			{
				stringBuilder.Append("[");
			}
			stringBuilder?.Append(pattern[i]);
			if (flag)
			{
				stringBuilder.Append("]");
			}
		}
		if (stringBuilder != null)
		{
			return stringBuilder.ToString();
		}
		return pattern;
	}

	internal static bool IsNullOrWhiteSpace(string value)
	{
		if (value == null)
		{
			return true;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (!char.IsWhiteSpace(value[i]))
			{
				return false;
			}
		}
		return true;
	}
}
