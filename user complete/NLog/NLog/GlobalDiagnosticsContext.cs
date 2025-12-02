using System;
using System.Collections.Generic;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Global Diagnostics Context - a dictionary structure to hold per-application-instance values.
/// </summary>
public static class GlobalDiagnosticsContext
{
	private static readonly object _lockObject = new object();

	private static Dictionary<string, object?> _dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

	private static Dictionary<string, object?>? _dictReadOnly;

	/// <summary>
	/// Sets the value with the specified key in the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	public static void Set(string item, string? value)
	{
		Set(item, (object?)value);
	}

	/// <summary>
	/// Sets the value with the specified key in the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	public static void Set(string item, object? value)
	{
		Guard.ThrowIfNull(item, "item");
		lock (_lockObject)
		{
			GetWritableDict()[item] = value;
		}
	}

	/// <summary>
	/// Gets the value with the specified key in the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>The value of <paramref name="item" />, if defined; otherwise <see cref="F:System.String.Empty" />.</returns>
	/// <remarks>If the value isn't a <see cref="T:System.String" /> already, this call locks the <see cref="T:NLog.LogFactory" /> for reading the <see cref="P:NLog.Config.LoggingConfiguration.DefaultCultureInfo" /> needed for converting to <see cref="T:System.String" />. </remarks>
	public static string Get(string item)
	{
		Guard.ThrowIfNull(item, "item");
		return Get(item, null);
	}

	/// <summary>
	/// Gets the value with the specified key in the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="formatProvider"><see cref="T:System.IFormatProvider" /> to use when converting the item's value to a string.</param>
	/// <returns>The value of <paramref name="item" /> as a string, if defined; otherwise <see cref="F:System.String.Empty" />.</returns>
	/// <remarks>If <paramref name="formatProvider" /> is <c>null</c> and the value isn't a <see cref="T:System.String" /> already, this call locks the <see cref="T:NLog.LogFactory" /> for reading the <see cref="P:NLog.Config.LoggingConfiguration.DefaultCultureInfo" /> needed for converting to <see cref="T:System.String" />. </remarks>
	public static string Get(string item, IFormatProvider? formatProvider)
	{
		Guard.ThrowIfNull(item, "item");
		return FormatHelper.ConvertToString(GetObject(item), formatProvider);
	}

	/// <summary>
	/// Gets the value with the specified key in the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>The item value, if defined; otherwise <c>null</c>.</returns>
	public static object? GetObject(string item)
	{
		Guard.ThrowIfNull(item, "item");
		GetReadOnlyDict().TryGetValue(item, out object value);
		return value;
	}

	/// <summary>
	/// Gets all key-names from Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <returns>A collection of the names of all items in the Global Diagnostics Context.</returns>
	public static ICollection<string> GetNames()
	{
		return GetReadOnlyDict().Keys;
	}

	/// <summary>
	/// Determines whether the Global Diagnostics Context (GDC) dictionary contains the specified key.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>A boolean indicating whether the specified item exists in current thread GDC.</returns>
	public static bool Contains(string item)
	{
		Guard.ThrowIfNull(item, "item");
		return GetReadOnlyDict().ContainsKey(item);
	}

	/// <summary>
	/// Removes the value with the specified key from the Global Diagnostics Context (GDC) dictionary
	/// </summary>
	/// <param name="item">Item name.</param>
	public static void Remove(string item)
	{
		Guard.ThrowIfNull(item, "item");
		lock (_lockObject)
		{
			if (_dict.ContainsKey(item))
			{
				GetWritableDict().Remove(item);
			}
		}
	}

	/// <summary>
	/// Clears the content of the Global Diagnostics Context (GDC) dictionary.
	/// </summary>
	public static void Clear()
	{
		lock (_lockObject)
		{
			if (_dict.Count != 0)
			{
				GetWritableDict(clearDictionary: true).Clear();
			}
		}
	}

	internal static Dictionary<string, object?> GetReadOnlyDict()
	{
		Dictionary<string, object> dictionary = _dictReadOnly;
		if (dictionary == null)
		{
			lock (_lockObject)
			{
				dictionary = (_dictReadOnly = _dict);
			}
		}
		return dictionary;
	}

	private static Dictionary<string, object?> GetWritableDict(bool clearDictionary = false)
	{
		if (_dictReadOnly != null)
		{
			_dict = CopyDictionaryOnWrite(clearDictionary);
			_dictReadOnly = null;
		}
		return _dict;
	}

	private static Dictionary<string, object?> CopyDictionaryOnWrite(bool clearDictionary)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>((!clearDictionary) ? (_dict.Count + 1) : 0, _dict.Comparer);
		if (!clearDictionary)
		{
			foreach (KeyValuePair<string, object> item in _dict)
			{
				dictionary[item.Key] = item.Value;
			}
		}
		return dictionary;
	}
}
