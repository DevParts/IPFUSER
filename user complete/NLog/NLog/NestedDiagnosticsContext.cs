using System;
using System.ComponentModel;
using System.Linq;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Obsolete and replaced by <see cref="T:NLog.ScopeContext" /> with NLog v5.
///
/// Nested Diagnostics Context (NDC) is a stack of nested values.
/// Stores the stack in the thread-local static variable, and provides methods to output the values in layouts.
/// </summary>
[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class NestedDiagnosticsContext
{
	/// <summary>
	/// Gets the top NDC message but doesn't remove it.
	/// </summary>
	/// <returns>The top message. .</returns>
	[Obsolete("Replaced by ScopeContext.PeekNestedState. Marked obsolete on NLog 5.0")]
	public static string TopMessage => FormatHelper.ConvertToString(TopObject, null);

	/// <summary>
	/// Gets the top NDC object but doesn't remove it.
	/// </summary>
	/// <returns>The object at the top of the NDC stack if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by ScopeContext.PeekNestedState. Marked obsolete on NLog 5.0")]
	public static object? TopObject => PeekObject();

	/// <summary>
	/// Pushes the specified text on current thread NDC.
	/// </summary>
	/// <param name="text">The text to be pushed.</param>
	/// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
	[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
	public static IDisposable Push(string text)
	{
		return ScopeContext.PushNestedState(text);
	}

	/// <summary>
	/// Pushes the specified object on current thread NDC.
	/// </summary>
	/// <param name="value">The object to be pushed.</param>
	/// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
	[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
	public static IDisposable Push(object value)
	{
		return ScopeContext.PushNestedState(value);
	}

	/// <summary>
	/// Pops the top message off the NDC stack.
	/// </summary>
	/// <returns>The top message which is no longer on the stack.</returns>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static string Pop()
	{
		return Pop(null);
	}

	/// <summary>
	/// Pops the top message from the NDC stack.
	/// </summary>
	/// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when converting the value to a string.</param>
	/// <returns>The top message, which is removed from the stack, as a string value.</returns>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static string Pop(IFormatProvider? formatProvider)
	{
		return FormatHelper.ConvertToString(PopObject() ?? string.Empty, formatProvider);
	}

	/// <summary>
	/// Pops the top object off the NDC stack.
	/// </summary>
	/// <returns>The object from the top of the NDC stack, if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static object? PopObject()
	{
		return ScopeContext.PopNestedContextLegacy();
	}

	/// <summary>
	/// Peeks the first object on the NDC stack
	/// </summary>
	/// <returns>The object from the top of the NDC stack, if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by ScopeContext.PeekNestedState. Marked obsolete on NLog 5.0")]
	public static object? PeekObject()
	{
		return ScopeContext.PeekNestedState();
	}

	/// <summary>
	/// Clears current thread NDC stack.
	/// </summary>
	[Obsolete("Replaced by ScopeContext.Clear. Marked obsolete on NLog 5.0")]
	public static void Clear()
	{
		ScopeContext.ClearNestedContextLegacy();
	}

	/// <summary>
	/// Gets all messages on the stack.
	/// </summary>
	/// <returns>Array of strings on the stack.</returns>
	[Obsolete("Replaced by ScopeContext.GetAllNestedStates. Marked obsolete on NLog 5.0")]
	public static string[] GetAllMessages()
	{
		return GetAllMessages(null);
	}

	/// <summary>
	/// Gets all messages from the stack, without removing them.
	/// </summary>
	/// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when converting a value to a string.</param>
	/// <returns>Array of strings.</returns>
	[Obsolete("Replaced by ScopeContext.GetAllNestedStates. Marked obsolete on NLog 5.0")]
	public static string[] GetAllMessages(IFormatProvider? formatProvider)
	{
		object[] allObjects = GetAllObjects();
		if (allObjects.Length == 0)
		{
			return ArrayHelper.Empty<string>();
		}
		return allObjects.Select((object o) => FormatHelper.ConvertToString(o, formatProvider)).ToArray();
	}

	/// <summary>
	/// Gets all objects on the stack.
	/// </summary>
	/// <returns>Array of objects on the stack.</returns>
	[Obsolete("Replaced by ScopeContext.GetAllNestedStates. Marked obsolete on NLog 5.0")]
	public static object[] GetAllObjects()
	{
		return ScopeContext.GetAllNestedStates();
	}
}
