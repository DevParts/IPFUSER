using System;
using System.Collections.Generic;
using System.ComponentModel;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Obsolete and replaced by <see cref="T:NLog.ScopeContext" /> with NLog v5.
///
/// Mapped Diagnostics Logical Context (MDLC) is a dictionary of keys and values.
/// Stores the dictionary in the logical thread callcontext, and provides methods to output dictionary values in layouts.
/// Allows for maintaining state across asynchronous tasks and call contexts.
/// </summary>
/// <remarks>
/// Ideally, these changes should be incorporated as a new version of the MappedDiagnosticsContext class in the original
/// NLog library so that state can be maintained for multiple threads in asynchronous situations.
/// </remarks>
[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class MappedDiagnosticsLogicalContext
{
	/// <summary>
	/// Gets the current logical context named item, as <see cref="T:System.String" />.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>The value of <paramref name="item" />, if defined; otherwise <see cref="F:System.String.Empty" />.</returns>
	/// <remarks>If the value isn't a <see cref="T:System.String" /> already, this call locks the <see cref="T:NLog.LogFactory" /> for reading the <see cref="P:NLog.Config.LoggingConfiguration.DefaultCultureInfo" /> needed for converting to <see cref="T:System.String" />. </remarks>
	[Obsolete("Replaced by ScopeContext.TryGetProperty. Marked obsolete on NLog 5.0")]
	public static string Get(string item)
	{
		return Get(item, null);
	}

	/// <summary>
	/// Gets the current logical context named item, as <see cref="T:System.String" />.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when converting a value to a string.</param>
	/// <returns>The value of <paramref name="item" />, if defined; otherwise <see cref="F:System.String.Empty" />.</returns>
	/// <remarks>If <paramref name="formatProvider" /> is <c>null</c> and the value isn't a <see cref="T:System.String" /> already, this call locks the <see cref="T:NLog.LogFactory" /> for reading the <see cref="P:NLog.Config.LoggingConfiguration.DefaultCultureInfo" /> needed for converting to <see cref="T:System.String" />. </remarks>
	[Obsolete("Replaced by ScopeContext.TryGetProperty. Marked obsolete on NLog 5.0")]
	public static string Get(string item, IFormatProvider? formatProvider)
	{
		return FormatHelper.ConvertToString(GetObject(item), formatProvider);
	}

	/// <summary>
	/// Gets the current logical context named item, as <see cref="T:System.Object" />.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>The value of <paramref name="item" />, if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by ScopeContext.TryGetProperty. Marked obsolete on NLog 5.0")]
	public static object? GetObject(string item)
	{
		Guard.ThrowIfNull(item, "item");
		if (ScopeContext.TryGetProperty(item, out object value))
		{
			return value;
		}
		return null;
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	/// <returns>&gt;An <see cref="T:System.IDisposable" /> that can be used to remove the item from the current logical context.</returns>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static IDisposable SetScoped(string item, string? value)
	{
		Guard.ThrowIfNull(item, "item");
		return MappedDiagnosticsLogicalContext.SetScoped<string>(item, value);
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	/// <returns>&gt;An <see cref="T:System.IDisposable" /> that can be used to remove the item from the current logical context.</returns>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static IDisposable SetScoped(string item, object? value)
	{
		Guard.ThrowIfNull(item, "item");
		return MappedDiagnosticsLogicalContext.SetScoped<object>(item, value);
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	/// <returns>&gt;An <see cref="T:System.IDisposable" /> that can be used to remove the item from the current logical context.</returns>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static IDisposable SetScoped<T>(string item, T? value)
	{
		Guard.ThrowIfNull(item, "item");
		return ScopeContext.PushProperty(item, value);
	}

	/// <summary>
	/// Updates the current logical context with multiple items in single operation
	/// </summary>
	/// <param name="items">.</param>
	/// <returns>&gt;An <see cref="T:System.IDisposable" /> that can be used to remove the item from the current logical context (null if no items).</returns>
	[Obsolete("Replaced by ScopeContext.PushProperties or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static IDisposable SetScoped(IReadOnlyList<KeyValuePair<string, object?>> items)
	{
		return ScopeContext.PushProperties(items);
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static void Set(string item, string? value)
	{
		MappedDiagnosticsLogicalContext.Set<string>(item, value);
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static void Set(string item, object? value)
	{
		MappedDiagnosticsLogicalContext.Set<object>(item, value);
	}

	/// <summary>
	/// Sets the current logical context item to the specified value.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <param name="value">Item value.</param>
	[Obsolete("Replaced by ScopeContext.PushProperty or Logger.PushScopeProperty using ${scopeproperty}. Marked obsolete on NLog 5.0")]
	public static void Set<T>(string item, T? value)
	{
		Guard.ThrowIfNull(item, "item");
		ScopeContext.SetMappedContextLegacy(item, value);
	}

	/// <summary>
	/// Returns all item names
	/// </summary>
	/// <returns>A collection of the names of all items in current logical context.</returns>
	[Obsolete("Replaced by ScopeContext.GetAllProperties. Marked obsolete on NLog 5.0")]
	public static ICollection<string> GetNames()
	{
		return ScopeContext.GetKeysMappedContextLegacy();
	}

	/// <summary>
	/// Checks whether the specified <paramref name="item" /> exists in current logical context.
	/// </summary>
	/// <param name="item">Item name.</param>
	/// <returns>A boolean indicating whether the specified <paramref name="item" /> exists in current logical context.</returns>
	[Obsolete("Replaced by ScopeContext.TryGetProperty. Marked obsolete on NLog 5.0")]
	public static bool Contains(string item)
	{
		Guard.ThrowIfNull(item, "item");
		object value;
		return ScopeContext.TryGetProperty(item, out value);
	}

	/// <summary>
	/// Removes the specified <paramref name="item" /> from current logical context.
	/// </summary>
	/// <param name="item">Item name.</param>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushProperty. Marked obsolete on NLog 5.0")]
	public static void Remove(string item)
	{
		Guard.ThrowIfNull(item, "item");
		ScopeContext.RemoveMappedContextLegacy(item);
	}

	/// <summary>
	/// Clears the content of current logical context.
	/// </summary>
	[Obsolete("Replaced by ScopeContext.Clear(). Marked obsolete on NLog 5.0")]
	public static void Clear()
	{
		Clear(free: true);
	}

	/// <summary>
	/// Clears the content of current logical context.
	/// </summary>
	/// <param name="free">Free the full slot.</param>
	[Obsolete("Replaced by ScopeContext.Clear(). Marked obsolete on NLog 5.0")]
	public static void Clear(bool free)
	{
		ScopeContext.ClearMappedContextLegacy();
	}
}
