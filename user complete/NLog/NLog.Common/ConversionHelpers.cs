using System;
using NLog.Internal;

namespace NLog.Common;

/// <summary>
/// String Conversion Helpers
/// </summary>
public static class ConversionHelpers
{
	/// <summary>
	/// Converts input string value into <see cref="T:System.Enum" />. Parsing is case-insensitive.
	/// </summary>
	/// <param name="inputValue">Input value</param>
	/// <param name="resultValue">Output value</param>
	/// <param name="defaultValue">Default value</param>
	/// <returns>Returns <see langword="false" /> if the input value could not be parsed</returns>
	public static bool TryParseEnum<TEnum>(string inputValue, out TEnum resultValue, TEnum defaultValue = default(TEnum)) where TEnum : struct
	{
		if (!TryParseEnum<TEnum>(inputValue, ignoreCase: true, out resultValue))
		{
			resultValue = defaultValue;
			return false;
		}
		return true;
	}

	/// <summary>
	/// Converts input string value into <see cref="T:System.Enum" />. Parsing is case-insensitive.
	/// </summary>
	/// <param name="inputValue">Input value</param>
	/// <param name="enumType">The type of the enum</param>
	/// <param name="resultValue">Output value. Null if parse failed</param>
	internal static bool TryParseEnum(string inputValue, Type enumType, out object? resultValue)
	{
		if (StringHelpers.IsNullOrWhiteSpace(inputValue))
		{
			resultValue = null;
			return false;
		}
		try
		{
			resultValue = Enum.Parse(enumType, inputValue, ignoreCase: true);
			return true;
		}
		catch (ArgumentException)
		{
			resultValue = null;
			return false;
		}
	}

	/// <summary>
	/// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
	/// </summary>
	/// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
	/// <param name="inputValue">The string representation of the enumeration name or underlying value to convert.</param>
	/// <param name="ignoreCase"><see langword="true" /> to ignore case; <see langword="false" /> to consider case.</param>
	/// <param name="resultValue">When this method returns, result contains an object of type TEnum whose value is represented by value if the parse operation succeeds. If the parse operation fails, result contains the default value of the underlying type of TEnum. Note that this value need not be a member of the TEnum enumeration. This parameter is passed uninitialized.</param>
	/// <returns><see langword="true" /> if the value parameter was converted successfully; otherwise, <see langword="false" />.</returns>
	/// <remarks>Wrapper because Enum.TryParse is not present in .net 3.5</remarks>
	internal static bool TryParseEnum<TEnum>(string inputValue, bool ignoreCase, out TEnum resultValue) where TEnum : struct
	{
		if (StringHelpers.IsNullOrWhiteSpace(inputValue))
		{
			resultValue = default(TEnum);
			return false;
		}
		return Enum.TryParse<TEnum>(inputValue, ignoreCase, out resultValue);
	}

	/// <summary>
	/// Enum.TryParse implementation for .net 3.5
	/// </summary>
	/// <returns></returns>
	/// <remarks>Don't uses reflection</remarks>
	private static bool TryParseEnum_net3<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
	{
		Type typeFromHandle = typeof(TEnum);
		if (!typeFromHandle.IsEnum)
		{
			throw new ArgumentException("Type '" + typeFromHandle.FullName + "' is not an enum");
		}
		if (StringHelpers.IsNullOrWhiteSpace(value))
		{
			result = default(TEnum);
			return false;
		}
		try
		{
			result = (TEnum)Enum.Parse(typeFromHandle, value, ignoreCase);
			return true;
		}
		catch (Exception)
		{
			result = default(TEnum);
			return false;
		}
	}
}
