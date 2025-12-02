using System;

namespace NLog.Config;

/// <summary>
/// Convert object-value into specified type
/// </summary>
public interface IPropertyTypeConverter
{
	/// <summary>
	/// Parses the input value and converts into the wanted type
	/// </summary>
	/// <param name="propertyValue">Input Value</param>
	/// <param name="propertyType">Wanted Type</param>
	/// <param name="format">Format to use when parsing</param>
	/// <param name="formatProvider">Culture to use when parsing</param>
	/// <returns>Output value with wanted type</returns>
	object? Convert(object? propertyValue, Type propertyType, string? format, IFormatProvider? formatProvider);
}
