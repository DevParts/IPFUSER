using System;

namespace NLog.Conditions;

/// <summary>
/// A bunch of utility methods (mostly predicates) which can be used in
/// condition expressions. Partially inspired by XPath 1.0.
/// </summary>
[ConditionMethods]
public static class ConditionMethods
{
	/// <summary>
	/// Compares two values for equality.
	/// </summary>
	/// <param name="firstValue">The first value.</param>
	/// <param name="secondValue">The second value.</param>
	/// <returns><b>true</b> when two objects are equal, <b>false</b> otherwise.</returns>
	[ConditionMethod("equals")]
	public static bool Equals2(object? firstValue, object? secondValue)
	{
		if (firstValue != secondValue)
		{
			return firstValue?.Equals(secondValue) ?? false;
		}
		return true;
	}

	/// <summary>
	/// Compares two strings for equality.
	/// </summary>
	/// <param name="firstValue">The first string.</param>
	/// <param name="secondValue">The second string.</param>
	/// <param name="ignoreCase">Optional. If <c>true</c>, case is ignored; if <c>false</c> (default), case is significant.</param>
	/// <returns><b>true</b> when two strings are equal, <b>false</b> otherwise.</returns>
	[ConditionMethod("strequals")]
	public static bool Equals2(string? firstValue, string? secondValue, bool ignoreCase = false)
	{
		return string.Equals(firstValue, secondValue, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the second string is a substring of the first one.
	/// </summary>
	/// <param name="haystack">The first string.</param>
	/// <param name="needle">The second string.</param>
	/// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
	/// <returns><b>true</b> when the second string is a substring of the first string, <b>false</b> otherwise.</returns>
	[ConditionMethod("contains")]
	public static bool Contains(string? haystack, string? needle, bool ignoreCase = true)
	{
		if (haystack == null)
		{
			return false;
		}
		return haystack.IndexOf(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
	}

	/// <summary>
	/// Gets or sets a value indicating whether the second string is a prefix of the first one.
	/// </summary>
	/// <param name="haystack">The first string.</param>
	/// <param name="needle">The second string.</param>
	/// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
	/// <returns><b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.</returns>
	[ConditionMethod("starts-with")]
	public static bool StartsWith(string? haystack, string? needle, bool ignoreCase = true)
	{
		return haystack?.StartsWith(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false;
	}

	/// <summary>
	/// Gets or sets a value indicating whether the second string is a suffix of the first one.
	/// </summary>
	/// <param name="haystack">The first string.</param>
	/// <param name="needle">The second string.</param>
	/// <param name="ignoreCase">Optional. If <c>true</c> (default), case is ignored; if <c>false</c>, case is significant.</param>
	/// <returns><b>true</b> when the second string is a prefix of the first string, <b>false</b> otherwise.</returns>
	[ConditionMethod("ends-with")]
	public static bool EndsWith(string? haystack, string? needle, bool ignoreCase = true)
	{
		return haystack?.EndsWith(needle, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false;
	}

	/// <summary>
	/// Returns the length of a string.
	/// </summary>
	/// <param name="text">A string whose lengths is to be evaluated.</param>
	/// <returns>The length of the string.</returns>
	[ConditionMethod("length")]
	public static int Length(string? text)
	{
		return text?.Length ?? 0;
	}
}
