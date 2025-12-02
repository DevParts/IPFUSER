using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace NLog.Internal;

/// <summary>
/// Helpers for <see cref="T:System.String" />.
/// </summary>
internal static class StringHelpers
{
	/// <summary>
	/// IsNullOrWhiteSpace, including for .NET 3.5
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	[ContractAnnotation("value:null => true")]
	internal static bool IsNullOrWhiteSpace(string? value)
	{
		return string.IsNullOrWhiteSpace(value);
	}

	internal static string[] SplitAndTrimTokens(this string value, char delimiter)
	{
		if (IsNullOrWhiteSpace(value))
		{
			return ArrayHelper.Empty<string>();
		}
		if (value.IndexOf(delimiter) == -1)
		{
			return new string[1] { value.Trim() };
		}
		string[] array = value.Split(new char[1] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			if (string.IsNullOrEmpty(array[i]))
			{
				return (from s in array
					where !IsNullOrWhiteSpace(s)
					select s.Trim()).ToArray();
			}
		}
		return array;
	}

	/// <summary>
	/// Replace string with <paramref name="comparison" />
	/// </summary>
	/// <returns>The same reference of nothing has been replaced.</returns>
	public static string Replace(string str, string oldValue, string newValue, StringComparison comparison, bool wholeWords = false)
	{
		Guard.ThrowIfNull(str, "str");
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		Guard.ThrowIfNullOrEmpty(oldValue, "oldValue");
		StringBuilder stringBuilder = null;
		int num = 0;
		int num2;
		for (num2 = str.IndexOf(oldValue, comparison); num2 != -1; num2 = str.IndexOf(oldValue, num2, comparison))
		{
			if (!wholeWords || IsWholeWord(str, oldValue, num2))
			{
				stringBuilder = stringBuilder ?? new StringBuilder(str.Length);
				if (num >= str.Length)
				{
					break;
				}
				stringBuilder.Append(str, num, num2 - num);
				stringBuilder.Append(newValue);
				num2 += oldValue.Length;
				num = num2;
				if (num2 >= str.Length)
				{
					break;
				}
			}
			else
			{
				num2 += oldValue.Length;
			}
		}
		if (stringBuilder == null)
		{
			return str;
		}
		if (num < str.Length)
		{
			stringBuilder.Append(str, num, str.Length - num);
		}
		return stringBuilder.ToString();
	}

	public static bool IsWholeWord(string str, string token, int index)
	{
		if (index + token.Length != str.Length)
		{
			if (!char.IsLetterOrDigit(str[index + token.Length]))
			{
				if (index != 0)
				{
					return !char.IsLetterOrDigit(str[index - 1]);
				}
				return true;
			}
			return false;
		}
		return true;
	}

	/// <summary>Concatenates all the elements of a string array, using the specified separator between each element. </summary>
	/// <param name="separator">The string to use as a separator. <paramref name="separator" /> is included in the returned string only if <paramref name="values" /> has more than one element.</param>
	/// <param name="values">An collection that contains the elements to concatenate. </param>
	/// <returns>A string that consists of the elements in <paramref name="values" /> delimited by the <paramref name="separator" /> string. If <paramref name="values" /> is an empty array, the method returns <see cref="F:System.String.Empty" />.</returns>
	/// <exception cref="T:System.ArgumentNullException">
	/// <paramref name="values" /> is <see langword="null" />. </exception>
	internal static string Join(string separator, IEnumerable<string> values)
	{
		return string.Join(separator, values);
	}

	internal static bool IsNullOrEmptyString(object? objectValue)
	{
		if (objectValue != null && string.Empty != objectValue)
		{
			if (objectValue is string text)
			{
				return text.Length == 0;
			}
			return false;
		}
		return true;
	}
}
