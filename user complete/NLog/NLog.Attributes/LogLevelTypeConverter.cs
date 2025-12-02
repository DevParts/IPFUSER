using System;
using System.ComponentModel;
using System.Globalization;

namespace NLog.Attributes;

/// <summary>
/// Support <see cref="T:NLog.LogLevel" /> implementation of <see cref="T:System.IConvertible" />
/// </summary>
public class LogLevelTypeConverter : TypeConverter
{
	/// <inheritdoc />
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (!(sourceType == typeof(string)) && !IsNumericType(sourceType))
		{
			return base.CanConvertFrom(context, sourceType);
		}
		return true;
	}

	/// <inheritdoc />
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		Type type = value?.GetType();
		if (typeof(string).Equals(type))
		{
			return LogLevel.FromString(value?.ToString() ?? string.Empty);
		}
		if (IsNumericType(type))
		{
			return LogLevel.FromOrdinal(Convert.ToInt32(value, CultureInfo.InvariantCulture));
		}
		return base.ConvertFrom(context, culture, value);
	}

	/// <inheritdoc />
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (!(destinationType == typeof(string)) && !IsNumericType(destinationType))
		{
			return base.CanConvertTo(context, destinationType);
		}
		return true;
	}

	/// <inheritdoc />
	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is LogLevel logLevel)
		{
			if (destinationType == typeof(string))
			{
				return logLevel.ToString();
			}
			if (IsNumericType(destinationType))
			{
				return Convert.ChangeType(logLevel.Ordinal, destinationType, culture);
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	private static bool IsNumericType(Type? sourceType)
	{
		if ((object)sourceType == null)
		{
			return false;
		}
		if (typeof(int).Equals(sourceType))
		{
			return true;
		}
		if (typeof(uint).Equals(sourceType))
		{
			return true;
		}
		if (typeof(long).Equals(sourceType))
		{
			return true;
		}
		if (typeof(ulong).Equals(sourceType))
		{
			return true;
		}
		if (typeof(short).Equals(sourceType))
		{
			return true;
		}
		if (typeof(ushort).Equals(sourceType))
		{
			return true;
		}
		return false;
	}
}
