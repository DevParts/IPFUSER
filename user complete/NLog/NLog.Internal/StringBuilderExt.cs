using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NLog.MessageTemplates;

namespace NLog.Internal;

/// <summary>
/// Helpers for <see cref="T:System.Text.StringBuilder" />, which is used in e.g. layout renderers.
/// </summary>
internal static class StringBuilderExt
{
	private static readonly char[] charToInt = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

	internal const int Iso8601_MaxDigitCount = 7;

	private static readonly string[] _zeroPaddedDigits = (from i in Enumerable.Range(0, 60)
		select i.ToString("D2", CultureInfo.InvariantCulture)).ToArray();

	/// <summary>
	/// Renders the specified log event context item and appends it to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="builder">append to this</param>
	/// <param name="value">value to be appended</param>
	/// <param name="format">format string. If @, then serialize the value with the Default JsonConverter.</param>
	/// <param name="formatProvider">provider, for example culture</param>
	/// <param name="valueFormatter">NLog string.Format interface</param>
	public static void AppendFormattedValue(this StringBuilder builder, object? value, string? format, IFormatProvider? formatProvider, IValueFormatter valueFormatter)
	{
		if (value is string value2 && string.IsNullOrEmpty(format))
		{
			builder.Append(value2);
		}
		else if ("@".Equals(format))
		{
			valueFormatter.FormatValue(value, null, CaptureType.Serialize, formatProvider, builder);
		}
		else if (value != null)
		{
			valueFormatter.FormatValue(value, format, CaptureType.Normal, formatProvider, builder);
		}
	}

	/// <summary>
	/// Appends int without using culture, and most importantly without garbage
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="value">value to append</param>
	public static void AppendInvariant(this StringBuilder builder, int value)
	{
		if (value < 0)
		{
			builder.Append('-');
			uint value2 = (uint)(-1 - value + 1);
			builder.AppendInvariant(value2);
		}
		else
		{
			builder.AppendInvariant((uint)value);
		}
	}

	/// <summary>
	/// Appends uint without using culture, and most importantly without garbage
	///
	/// Credits Gavin Pugh  - https://www.gavpugh.com/2010/04/01/xnac-avoiding-garbage-when-working-with-stringbuilder/
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="value">value to append</param>
	public static void AppendInvariant(this StringBuilder builder, uint value)
	{
		if (value == 0)
		{
			builder.Append('0');
			return;
		}
		int digitCount = CalculateDigitCount(value);
		ApppendValueWithDigitCount(builder, value, digitCount);
	}

	private static int CalculateDigitCount(uint value)
	{
		int num = 0;
		uint num2 = value;
		do
		{
			num2 /= 10;
			num++;
		}
		while (num2 != 0);
		return num;
	}

	private static void ApppendValueWithDigitCount(StringBuilder builder, uint value, int digitCount)
	{
		builder.Append('0', digitCount);
		int num = builder.Length;
		while (digitCount > 0)
		{
			num--;
			builder[num] = charToInt[value % 10];
			value /= 10;
			digitCount--;
		}
	}

	/// <summary>
	/// Convert DateTime into UTC and format to yyyy-MM-ddTHH:mm:ss.fffffffZ - ISO 8601 Compliant Date Format (Round-Trip-Time)
	/// </summary>
	public static void AppendXmlDateTimeUtcRoundTripFixed(this StringBuilder builder, DateTime dateTime)
	{
		int fraction = (int)(dateTime.Ticks % 10000000);
		builder.AppendXmlDateTimeUtcRoundTrip(dateTime, fraction, 7);
	}

	public static void AppendXmlDateTimeUtcRoundTrip(this StringBuilder builder, DateTime dateTime)
	{
		int num = 7;
		int num2 = (int)(dateTime.Ticks % 10000000);
		if (num2 > 0)
		{
			while (num2 % 10 == 0)
			{
				num--;
				num2 /= 10;
			}
		}
		builder.AppendXmlDateTimeUtcRoundTrip(dateTime, num2, num);
	}

	private static void AppendXmlDateTimeUtcRoundTrip(this StringBuilder builder, DateTime dateTime, int fraction, int max_digit_count)
	{
		dateTime = ((dateTime.Kind != DateTimeKind.Unspecified) ? dateTime.ToUniversalTime() : new DateTime(dateTime.Ticks, DateTimeKind.Utc));
		builder.Append4DigitsZeroPadded(dateTime.Year);
		builder.Append('-');
		builder.Append2DigitsZeroPadded(dateTime.Month);
		builder.Append('-');
		builder.Append2DigitsZeroPadded(dateTime.Day);
		builder.Append('T');
		builder.Append2DigitsZeroPadded(dateTime.Hour);
		builder.Append(':');
		builder.Append2DigitsZeroPadded(dateTime.Minute);
		builder.Append(':');
		builder.Append2DigitsZeroPadded(dateTime.Second);
		if (fraction > 0)
		{
			builder.Append('.');
			int num = CalculateDigitCount((uint)fraction);
			if (max_digit_count > num)
			{
				builder.Append('0', max_digit_count - num);
			}
			ApppendValueWithDigitCount(builder, (uint)fraction, num);
		}
		builder.Append('Z');
	}

	/// <summary>
	/// Clears the provider StringBuilder
	/// </summary>
	/// <param name="builder"></param>
	public static void ClearBuilder(this StringBuilder builder)
	{
		try
		{
			builder.Clear();
		}
		catch
		{
			if (builder.Length > 1)
			{
				builder.Remove(0, builder.Length - 1);
			}
			builder.Remove(0, builder.Length);
		}
	}

	/// <summary>
	/// Copies the contents of the StringBuilder to the MemoryStream using the specified encoding (Without BOM/Preamble)
	/// </summary>
	/// <param name="builder">StringBuilder source</param>
	/// <param name="ms">MemoryStream destination</param>
	/// <param name="encoding">Encoding used for converter string into byte-stream</param>
	/// <param name="transformBuffer">Helper char-buffer to minimize memory allocations</param>
	public static void CopyToStream(this StringBuilder builder, MemoryStream ms, Encoding encoding, char[] transformBuffer)
	{
		int maxByteCount = encoding.GetMaxByteCount(builder.Length);
		long num = ms.Position;
		ms.SetLength(num + maxByteCount);
		for (int i = 0; i < builder.Length; i += transformBuffer.Length)
		{
			int num2 = Math.Min(builder.Length - i, transformBuffer.Length);
			builder.CopyTo(i, transformBuffer, 0, num2);
			maxByteCount = encoding.GetBytes(transformBuffer, 0, num2, ms.GetBuffer(), (int)num);
			num += maxByteCount;
		}
		ms.Position = num;
		if (num != ms.Length)
		{
			ms.SetLength(num);
		}
	}

	public static void CopyToBuffer(this StringBuilder builder, char[] destination, int destinationIndex)
	{
		builder.CopyTo(0, destination, destinationIndex, builder.Length);
	}

	/// <summary>
	/// Copies the contents of the StringBuilder to the destination StringBuilder
	/// </summary>
	/// <param name="builder">StringBuilder source</param>
	/// <param name="destination">StringBuilder destination</param>
	public static void CopyTo(this StringBuilder builder, StringBuilder destination)
	{
		int length = builder.Length;
		if (length <= 0)
		{
			return;
		}
		destination.EnsureCapacity(length + destination.Length);
		if (length < 8)
		{
			for (int i = 0; i < length; i++)
			{
				destination.Append(builder[i]);
			}
			return;
		}
		if (length < 512)
		{
			destination.Append(builder.ToString());
			return;
		}
		char[] array = new char[256];
		for (int j = 0; j < length; j += array.Length)
		{
			int num = Math.Min(length - j, array.Length);
			builder.CopyTo(j, array, 0, num);
			destination.Append(array, 0, num);
		}
	}

	/// <summary>
	/// Scans the StringBuilder for the position of needle character
	/// </summary>
	/// <param name="builder">StringBuilder source</param>
	/// <param name="needle">needle character to search for</param>
	/// <param name="startPos"></param>
	/// <returns>Index of the first occurrence (Else -1)</returns>
	public static int IndexOf(this StringBuilder builder, char needle, int startPos = 0)
	{
		int length = builder.Length;
		for (int i = startPos; i < length; i++)
		{
			if (builder[i] == needle)
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// Scans the StringBuilder for the position of needle character
	/// </summary>
	/// <param name="builder">StringBuilder source</param>
	/// <param name="needles">needle characters to search for</param>
	/// <param name="startPos"></param>
	/// <returns>Index of the first occurrence (Else -1)</returns>
	public static int IndexOfAny(this StringBuilder builder, char[] needles, int startPos = 0)
	{
		int length = builder.Length;
		for (int i = startPos; i < length; i++)
		{
			if (CharArrayContains(builder[i], needles))
			{
				return i;
			}
		}
		return -1;
	}

	private static bool CharArrayContains(char searchChar, char[] needles)
	{
		for (int i = 0; i < needles.Length; i++)
		{
			if (needles[i] == searchChar)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Compares the contents of two StringBuilders
	/// </summary>
	/// <remarks>
	/// Correct implementation of <see cref="M:System.Text.StringBuilder.Equals(System.Text.StringBuilder)" /> that also works when <see cref="P:System.Text.StringBuilder.Capacity" /> is not the same
	/// </remarks>
	/// <returns><see langword="true" /> when content is the same</returns>
	public static bool EqualTo(this StringBuilder builder, StringBuilder other)
	{
		int length = builder.Length;
		if (length != other.Length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (builder[i] != other[i])
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Compares the contents of a StringBuilder and a String
	/// </summary>
	/// <returns><see langword="true" /> when content is the same</returns>
	public static bool EqualTo(this StringBuilder builder, string other)
	{
		if (builder.Length != other.Length)
		{
			return false;
		}
		int num = 0;
		foreach (char c in other)
		{
			if (builder[num++] != c)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Append a number and pad with 0 to 2 digits
	/// </summary>
	/// <param name="builder">append to this</param>
	/// <param name="number">the number</param>
	internal static void Append2DigitsZeroPadded(this StringBuilder builder, int number)
	{
		if (number < 0 || number >= _zeroPaddedDigits.Length)
		{
			builder.Append((char)(number / 10 + 48));
			builder.Append((char)(number % 10 + 48));
		}
		else
		{
			builder.Append(_zeroPaddedDigits[number]);
		}
	}

	/// <summary>
	/// Append a number and pad with 0 to 4 digits
	/// </summary>
	/// <param name="builder">append to this</param>
	/// <param name="number">the number</param>
	internal static void Append4DigitsZeroPadded(this StringBuilder builder, int number)
	{
		builder.Append((char)(number / 1000 % 10 + 48));
		builder.Append((char)(number / 100 % 10 + 48));
		builder.Append((char)(number / 10 % 10 + 48));
		builder.Append((char)(number % 10 + 48));
	}

	/// <summary>
	/// Append a numeric type (byte, int, double, decimal) as string
	/// </summary>
	internal static void AppendNumericInvariant(this StringBuilder sb, IConvertible value, TypeCode objTypeCode)
	{
		switch (objTypeCode)
		{
		case TypeCode.Byte:
			sb.AppendInvariant(value.ToByte(CultureInfo.InvariantCulture));
			break;
		case TypeCode.SByte:
			sb.AppendInvariant(value.ToSByte(CultureInfo.InvariantCulture));
			break;
		case TypeCode.Int16:
			sb.AppendInvariant(value.ToInt16(CultureInfo.InvariantCulture));
			break;
		case TypeCode.Int32:
			sb.AppendInvariant(value.ToInt32(CultureInfo.InvariantCulture));
			break;
		case TypeCode.Int64:
		{
			long num2 = value.ToInt64(CultureInfo.InvariantCulture);
			if (num2 < int.MaxValue && num2 > int.MinValue)
			{
				sb.AppendInvariant((int)num2);
			}
			else
			{
				sb.Append(num2);
			}
			break;
		}
		case TypeCode.UInt16:
			sb.AppendInvariant(value.ToUInt16(CultureInfo.InvariantCulture));
			break;
		case TypeCode.UInt32:
			sb.AppendInvariant(value.ToUInt32(CultureInfo.InvariantCulture));
			break;
		case TypeCode.UInt64:
		{
			ulong num = value.ToUInt64(CultureInfo.InvariantCulture);
			if (num < uint.MaxValue)
			{
				sb.AppendInvariant((uint)num);
			}
			else
			{
				sb.Append(num);
			}
			break;
		}
		case TypeCode.Single:
		{
			float floatValue = value.ToSingle(CultureInfo.InvariantCulture);
			AppendFloatInvariant(sb, floatValue);
			break;
		}
		case TypeCode.Double:
		{
			double doubleValue = value.ToDouble(CultureInfo.InvariantCulture);
			AppendDoubleInvariant(sb, doubleValue);
			break;
		}
		case TypeCode.Decimal:
			AppendDecimalInvariant(sb, value.ToDecimal(CultureInfo.InvariantCulture));
			break;
		default:
			sb.Append(XmlHelper.XmlConvertToString(value, objTypeCode));
			break;
		}
	}

	private static void AppendDecimalInvariant(StringBuilder sb, decimal decimalValue)
	{
		if (Math.Truncate(decimalValue) == decimalValue && decimalValue > -2147483648m && decimalValue < 2147483647m)
		{
			sb.AppendInvariant(Convert.ToInt32(decimalValue));
			sb.Append(".0");
		}
		else
		{
			sb.Append(XmlHelper.XmlConvertToString(decimalValue));
		}
	}

	private static void AppendDoubleInvariant(StringBuilder sb, double doubleValue)
	{
		if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
		{
			sb.Append(XmlHelper.XmlConvertToString(doubleValue));
		}
		else if (Math.Truncate(doubleValue) == doubleValue && doubleValue > -2147483648.0 && doubleValue < 2147483647.0)
		{
			sb.AppendInvariant(Convert.ToInt32(doubleValue));
			sb.Append(".0");
		}
		else
		{
			sb.Append(XmlHelper.XmlConvertToString(doubleValue));
		}
	}

	private static void AppendFloatInvariant(StringBuilder sb, float floatValue)
	{
		if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
		{
			sb.Append(XmlHelper.XmlConvertToString(floatValue));
		}
		else if (Math.Truncate(floatValue) == (double)floatValue && floatValue > -2.1474836E+09f && floatValue < 2.1474836E+09f)
		{
			sb.AppendInvariant(Convert.ToInt32(floatValue));
			sb.Append(".0");
		}
		else
		{
			sb.Append(XmlHelper.XmlConvertToString(floatValue));
		}
	}

	public static void TrimRight(this StringBuilder sb, int startPos = 0)
	{
		int num = sb.Length - 1;
		while (num >= startPos && char.IsWhiteSpace(sb[num]))
		{
			num--;
		}
		if (num < sb.Length - 1)
		{
			sb.Length = num + 1;
		}
	}
}
