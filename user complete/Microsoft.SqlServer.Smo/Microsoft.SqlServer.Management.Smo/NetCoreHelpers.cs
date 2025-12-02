using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Microsoft.SqlServer.Management.Smo;

internal static class NetCoreHelpers
{
	private const int DefaultFileStreamBufferSize = 4096;

	private static CultureInfo invariantCulture = CultureInfo.InvariantCulture;

	public static CultureInfo InvariantCulture => invariantCulture;

	public static int InvariantCultureLcid => CultureInfo.InvariantCulture.LCID;

	public static XmlWriter CreateXmlWriter(TextWriter textWriter, XmlWriterSettings xmlSettings)
	{
		return XmlWriter.Create(textWriter, xmlSettings);
	}

	public static System.StringComparer GetStringComparer(this CultureInfo culture, bool ignoreCase)
	{
		return System.StringComparer.Create(culture, ignoreCase);
	}

	public static Assembly GetAssembly(this Type type)
	{
		return type.Assembly;
	}

	public static Type GetBaseType(this Type type)
	{
		return type.BaseType;
	}

	public static Type[] GetGenericArguments(this Type type)
	{
		return type.GetGenericArguments();
	}

	public static bool GetIsAssignableFrom(this Type type, Type c)
	{
		return type.IsAssignableFrom(c);
	}

	public static bool GetIsClass(this Type type)
	{
		return type.IsClass;
	}

	public static bool GetIsEnum(this Type type)
	{
		return type.IsEnum;
	}

	public static bool GetIsGenericType(this Type type)
	{
		return type.IsGenericType;
	}

	public static bool GetIsNestedPrivate(this Type type)
	{
		return type.IsNestedPrivate;
	}

	public static bool GetIsPrimitive(this Type type)
	{
		return type.IsPrimitive;
	}

	public static bool GetIsSealed(this Type type)
	{
		return type.IsSealed;
	}

	public static bool GetIsValueType(this Type type)
	{
		return type.IsValueType;
	}

	public static StreamWriter CreateStreamWriter(string path, bool appendToFile)
	{
		return new StreamWriter(path, appendToFile);
	}

	public static StreamWriter CreateStreamWriter(string path, bool appendToFile, Encoding encoding)
	{
		return new StreamWriter(path, appendToFile, encoding);
	}

	private static FileStream CreateFileStream(string path, bool appendToFile)
	{
		FileMode mode = (appendToFile ? FileMode.Append : FileMode.Create);
		return new FileStream(path, mode, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan);
	}

	public static int StringCompare(string x, string y, bool ignoreCase, CultureInfo culture)
	{
		System.StringComparer stringComparer = culture.GetStringComparer(ignoreCase);
		return stringComparer.Compare(x, y);
	}
}
