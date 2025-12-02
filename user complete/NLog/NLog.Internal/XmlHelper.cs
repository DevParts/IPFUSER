using System;
using System.Globalization;
using System.Text;

namespace NLog.Internal;

/// <summary>
///  Helper class for XML
/// </summary>
internal static class XmlHelper
{
	private const char HIGH_SURROGATE_START = '\ud800';

	private const char HIGH_SURROGATE_END = '\udbff';

	private const char LOW_SURROGATE_START = '\udc00';

	private const char LOW_SURROGATE_END = '\udfff';

	private static readonly char[] XmlEscapeChars = new char[5] { '<', '>', '&', '\'', '"' };

	private static readonly char[] XmlEscapeNewlineChars = new char[7] { '<', '>', '&', '\'', '"', '\r', '\n' };

	private static readonly char[] DecimalScientificExponent = new char[2] { 'e', 'E' };

	internal static bool XmlConvertIsXmlChar(char chr)
	{
		if (chr <= '\u001f' || chr >= '\ud800')
		{
			return ExoticIsXmlChar(chr);
		}
		return true;
	}

	private static bool ExoticIsXmlChar(char chr)
	{
		if (chr < ' ')
		{
			if (chr != '\t' && chr != '\n')
			{
				return chr == '\r';
			}
			return true;
		}
		if (XmlConvertIsHighSurrogate(chr) || XmlConvertIsLowSurrogate(chr))
		{
			return false;
		}
		if (chr == '\ufffe' || chr == '\uffff')
		{
			return false;
		}
		return true;
	}

	public static bool XmlConvertIsHighSurrogate(char chr)
	{
		if (chr >= '\ud800')
		{
			return chr <= '\udbff';
		}
		return false;
	}

	public static bool XmlConvertIsLowSurrogate(char chr)
	{
		if (chr >= '\udc00')
		{
			return chr <= '\udfff';
		}
		return false;
	}

	public static bool XmlConvertIsXmlSurrogatePair(char lowChar, char highChar)
	{
		if (XmlConvertIsHighSurrogate(highChar))
		{
			return XmlConvertIsLowSurrogate(lowChar);
		}
		return false;
	}

	/// <summary>
	/// removes any unusual unicode characters that can't be encoded into XML
	/// </summary>
	private static string RemoveInvalidXmlChars(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			if (!XmlConvertIsXmlChar(c))
			{
				if (i + 1 >= text.Length || !XmlConvertIsXmlSurrogatePair(text[i + 1], c))
				{
					return CreateValidXmlString(text);
				}
				i++;
			}
		}
		return text;
	}

	/// <summary>
	/// Cleans string of any invalid XML chars found
	/// </summary>
	/// <param name="text">unclean string</param>
	/// <returns>string with only valid XML chars</returns>
	private static string CreateValidXmlString(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		foreach (char c in text)
		{
			if (XmlConvertIsXmlChar(c))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	internal static void PerformXmlEscapeWhenNeeded(StringBuilder builder, int startPos, bool xmlEncodeNewlines)
	{
		if (RequiresXmlEscape(builder, startPos, xmlEncodeNewlines))
		{
			string text = builder.ToString(startPos, builder.Length - startPos);
			builder.Length = startPos;
			EscapeXmlString(text, xmlEncodeNewlines, builder);
		}
	}

	private static bool RequiresXmlEscape(StringBuilder target, int startPos, bool xmlEncodeNewlines)
	{
		for (int i = startPos; i < target.Length; i++)
		{
			switch (target[i])
			{
			case '"':
			case '&':
			case '\'':
			case '<':
			case '>':
				return true;
			case '\n':
			case '\r':
				if (xmlEncodeNewlines)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	internal static string EscapeXmlString(string text, bool xmlEncodeNewlines, StringBuilder? result = null)
	{
		if (result == null && SmallAndNoEscapeNeeded(text, xmlEncodeNewlines))
		{
			return text;
		}
		StringBuilder stringBuilder = result ?? new StringBuilder(text.Length);
		for (int i = 0; i < text.Length; i++)
		{
			switch (text[i])
			{
			case '<':
				stringBuilder.Append("&lt;");
				break;
			case '>':
				stringBuilder.Append("&gt;");
				break;
			case '&':
				stringBuilder.Append("&amp;");
				break;
			case '\'':
				stringBuilder.Append("&apos;");
				break;
			case '"':
				stringBuilder.Append("&quot;");
				break;
			case '\r':
				if (xmlEncodeNewlines)
				{
					stringBuilder.Append("&#13;");
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
				break;
			case '\n':
				if (xmlEncodeNewlines)
				{
					stringBuilder.Append("&#10;");
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
				break;
			default:
				stringBuilder.Append(text[i]);
				break;
			}
		}
		if (result != null)
		{
			return string.Empty;
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// Pretest, small text and not escape needed
	/// </summary>
	/// <param name="text"></param>
	/// <param name="xmlEncodeNewlines"></param>
	/// <returns></returns>
	private static bool SmallAndNoEscapeNeeded(string text, bool xmlEncodeNewlines)
	{
		if (text.Length < 4096)
		{
			return text.IndexOfAny(xmlEncodeNewlines ? XmlEscapeNewlineChars : XmlEscapeChars) < 0;
		}
		return false;
	}

	/// <summary>
	/// Converts object value to invariant format, and strips any invalid xml-characters
	/// </summary>
	/// <param name="value">Object value</param>
	/// <returns>Object value converted to string</returns>
	internal static string XmlConvertToStringSafe(object? value)
	{
		return XmlConvertToString(value, safeConversion: true);
	}

	/// <summary>
	/// Converts object value to invariant format (understood by JavaScript)
	/// </summary>
	/// <param name="value">Object value</param>
	/// <returns>Object value converted to string</returns>
	internal static string XmlConvertToString(object? value)
	{
		if (!(value is string result))
		{
			return XmlConvertToString(value, safeConversion: false);
		}
		return result;
	}

	internal static string XmlConvertToString(float value)
	{
		if (float.IsInfinity(value) || float.IsNaN(value))
		{
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
		return EnsureDecimalPlace(value.ToString("R", NumberFormatInfo.InvariantInfo));
	}

	internal static string XmlConvertToString(double value)
	{
		if (double.IsInfinity(value) || double.IsNaN(value))
		{
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
		return EnsureDecimalPlace(value.ToString("R", NumberFormatInfo.InvariantInfo));
	}

	internal static string XmlConvertToString(decimal value)
	{
		return EnsureDecimalPlace(value.ToString(null, NumberFormatInfo.InvariantInfo));
	}

	/// <summary>
	/// Converts DateTime to ISO 8601 format in UTC timezone.
	/// </summary>
	internal static string XmlConvertToString(DateTime value)
	{
		value = ((value.Kind != DateTimeKind.Unspecified) ? value.ToUniversalTime() : new DateTime(value.Ticks, DateTimeKind.Utc));
		return value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Converts object value to invariant format (understood by JavaScript)
	/// </summary>
	/// <param name="value">Object value</param>
	/// <param name="objTypeCode">Object TypeCode</param>
	/// <param name="safeConversion">Check and remove unusual unicode characters from the result string.</param>
	/// <returns>Object value converted to string</returns>
	internal static string XmlConvertToString(IConvertible? value, TypeCode objTypeCode, bool safeConversion = false)
	{
		if (objTypeCode == TypeCode.Empty || value == null)
		{
			return "null";
		}
		switch (objTypeCode)
		{
		case TypeCode.Boolean:
			if (!value.ToBoolean(CultureInfo.InvariantCulture))
			{
				return "false";
			}
			return "true";
		case TypeCode.Byte:
			return value.ToByte(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.SByte:
			return value.ToSByte(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.Int16:
			return value.ToInt16(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.Int32:
			return value.ToInt32(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.Int64:
			return value.ToInt64(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.UInt16:
			return value.ToUInt16(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.UInt32:
			return value.ToUInt32(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.UInt64:
			return value.ToUInt64(CultureInfo.InvariantCulture).ToString(null, NumberFormatInfo.InvariantInfo);
		case TypeCode.Single:
			return XmlConvertToString(value.ToSingle(CultureInfo.InvariantCulture));
		case TypeCode.Double:
			return XmlConvertToString(value.ToDouble(CultureInfo.InvariantCulture));
		case TypeCode.Decimal:
			return XmlConvertToString(value.ToDecimal(CultureInfo.InvariantCulture));
		case TypeCode.DateTime:
			return XmlConvertToString(value.ToDateTime(CultureInfo.InvariantCulture));
		case TypeCode.Char:
			if (!safeConversion)
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}
			return RemoveInvalidXmlChars(value.ToString(CultureInfo.InvariantCulture));
		case TypeCode.String:
			if (!safeConversion)
			{
				return value.ToString(CultureInfo.InvariantCulture);
			}
			return RemoveInvalidXmlChars(value.ToString(CultureInfo.InvariantCulture));
		default:
			return XmlConvertToStringInvariant(value, safeConversion);
		}
	}

	private static string XmlConvertToString(object? value, bool safeConversion)
	{
		try
		{
			IConvertible convertible = value as IConvertible;
			TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((value != null) ? 1 : 0));
			if (typeCode != TypeCode.Object)
			{
				return XmlConvertToString(convertible, typeCode, safeConversion);
			}
			return XmlConvertToStringInvariant(value, safeConversion);
		}
		catch
		{
			return string.Empty;
		}
	}

	private static string XmlConvertToStringInvariant(object? value, bool safeConversion)
	{
		try
		{
			string text = Convert.ToString(value, CultureInfo.InvariantCulture);
			return safeConversion ? RemoveInvalidXmlChars(text) : text;
		}
		catch
		{
			return string.Empty;
		}
	}

	/// <summary>
	/// XML elements must follow these naming rules:
	///  - Element names are case-sensitive
	///  - Element names must start with a letter or underscore
	///  - Element names can contain letters, digits, hyphens, underscores, and periods
	///  - Element names cannot contain spaces
	/// </summary>
	/// <param name="xmlElementName"></param>
	internal static string XmlConvertToElementName(string xmlElementName)
	{
		if (string.IsNullOrEmpty(xmlElementName))
		{
			return xmlElementName;
		}
		xmlElementName = RemoveInvalidXmlChars(xmlElementName);
		bool flag = true;
		StringBuilder stringBuilder = null;
		for (int i = 0; i < xmlElementName.Length; i++)
		{
			char c = xmlElementName[i];
			if (char.IsLetter(c))
			{
				stringBuilder?.Append(c);
				continue;
			}
			bool flag2 = false;
			switch (c)
			{
			case ':':
				if (i != 0 && flag)
				{
					flag = false;
					stringBuilder?.Append(c);
					continue;
				}
				break;
			case '_':
				stringBuilder?.Append(c);
				continue;
			case '-':
			case '.':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				if (i != 0)
				{
					stringBuilder?.Append(c);
					continue;
				}
				flag2 = true;
				break;
			}
			if (stringBuilder == null)
			{
				stringBuilder = CreateStringBuilder(xmlElementName, i);
			}
			stringBuilder.Append('_');
			if (flag2)
			{
				stringBuilder.Append(c);
			}
		}
		stringBuilder?.TrimRight();
		return stringBuilder?.ToString() ?? xmlElementName;
		static StringBuilder CreateStringBuilder(string orgValue, int num)
		{
			StringBuilder stringBuilder2 = new StringBuilder(orgValue.Length);
			if (num > 0)
			{
				stringBuilder2.Append(orgValue, 0, num);
			}
			return stringBuilder2;
		}
	}

	private static string EnsureDecimalPlace(string text)
	{
		if (text.IndexOf('.') != -1 || text.IndexOfAny(DecimalScientificExponent) != -1)
		{
			return text;
		}
		if (text.Length == 1)
		{
			switch (text[0])
			{
			case '0':
				return "0.0";
			case '1':
				return "1.0";
			case '2':
				return "2.0";
			case '3':
				return "3.0";
			case '4':
				return "4.0";
			case '5':
				return "5.0";
			case '6':
				return "6.0";
			case '7':
				return "7.0";
			case '8':
				return "8.0";
			case '9':
				return "9.0";
			}
		}
		return text + ".0";
	}

	public static void RemoveInvalidXmlIfNeeded(StringBuilder builder, int orgLength)
	{
		for (int i = orgLength; i < builder.Length; i++)
		{
			if (!XmlConvertIsXmlChar(builder[i]))
			{
				string text = builder.ToString(i, builder.Length - i);
				builder.Length = i;
				text = RemoveInvalidXmlChars(text);
				builder.Append(text);
				break;
			}
		}
	}

	public static void EscapeCDataIfNeeded(StringBuilder builder, int orgLength)
	{
		for (int i = orgLength; i < builder.Length; i++)
		{
			if (builder[i] == ']' && i + 2 < builder.Length && builder[i + 1] == ']' && builder[i + 2] == '>')
			{
				string text = builder.ToString(i, builder.Length - i);
				builder.Length = i;
				text = text.Replace("]]>", "]]]]><![CDATA[>");
				builder.Append(text);
				break;
			}
		}
	}

	public static string EscapeCData(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "<![CDATA[]]>";
		}
		if (text.Contains("]]>"))
		{
			text = text.Replace("]]>", "]]]]><![CDATA[>");
		}
		return "<![CDATA[" + text + "]]>";
	}
}
