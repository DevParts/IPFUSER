using System;
using System.Collections;

namespace NLog.Internal;

internal static class FormatHelper
{
	/// <summary>
	/// Convert object to string
	/// </summary>
	/// <param name="value">value</param>
	/// <param name="formatProvider">format for conversion.</param>
	/// <returns></returns>
	/// <remarks>
	/// If <paramref name="formatProvider" /> is <c>null</c> and <paramref name="value" /> isn't a <see cref="T:System.String" /> already, then the <see cref="T:NLog.LogFactory" /> will get a locked by <see cref="P:NLog.LogManager.Configuration" />
	/// </remarks>
	internal static string ConvertToString(object? value, IFormatProvider? formatProvider)
	{
		if (value is string result)
		{
			return result;
		}
		if (value == null)
		{
			return string.Empty;
		}
		if (formatProvider == null && value is IFormattable)
		{
			formatProvider = LogManager.LogFactory.DefaultCultureInfo;
		}
		return Convert.ToString(value, formatProvider);
	}

	internal static string TryFormatToString(object? value, string? format, IFormatProvider? formatProvider)
	{
		if (value is IFormattable formattable)
		{
			return formattable.ToString(format, formatProvider);
		}
		if (value is IConvertible convertible)
		{
			return convertible.ToString(formatProvider);
		}
		if (value is IEnumerable)
		{
			return string.Empty;
		}
		return value?.ToString() ?? string.Empty;
	}
}
