using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Obsolete and replaced by <see cref="T:NLog.ScopeContext" /> with NLog v5.
///
/// Nested Diagnostics Logical Context (NDLC) is a stack of nested values.
/// Stores the stack in the logical thread callcontexte, and provides methods to output the values in layouts.
/// </summary>
[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class NestedDiagnosticsLogicalContext
{
	[Obsolete("Required to be compatible with legacy NLog versions, when using remoting. Marked obsolete on NLog 5.0")]
	private interface INestedContext : IDisposable
	{
		INestedContext Parent { get; }

		int FrameLevel { get; }

		object? Value { get; }

		long CreatedTimeUtcTicks { get; }
	}

	[Serializable]
	[Obsolete("Required to be compatible with legacy NLog versions, when using remoting. Marked obsolete on NLog 5.0")]
	private sealed class NestedContext<T> : INestedContext, IDisposable
	{
		private int _disposed;

		public INestedContext Parent { get; }

		public T Value { get; }

		public long CreatedTimeUtcTicks { get; }

		public int FrameLevel { get; }

		object? INestedContext.Value => Value;

		public NestedContext(INestedContext parent, T value)
		{
			Parent = parent;
			Value = value;
			CreatedTimeUtcTicks = DateTime.UtcNow.Ticks;
			FrameLevel = ((parent == null) ? 1 : (parent.FrameLevel + 1));
		}

		void IDisposable.Dispose()
		{
			if (Interlocked.Exchange(ref _disposed, 1) != 1)
			{
				PopObject();
			}
		}

		public override string ToString()
		{
			return ((object)Value)?.ToString() ?? "null";
		}
	}

	/// <summary>
	/// Pushes the specified value on current stack
	/// </summary>
	/// <param name="value">The value to be pushed.</param>
	/// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
	[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
	public static IDisposable Push<T>(T value)
	{
		return ScopeContext.PushNestedState(value);
	}

	/// <summary>
	/// Pushes the specified value on current stack
	/// </summary>
	/// <param name="value">The value to be pushed.</param>
	/// <returns>An instance of the object that implements IDisposable that returns the stack to the previous level when IDisposable.Dispose() is called. To be used with C# using() statement.</returns>
	[Obsolete("Replaced by ScopeContext.PushNestedState or Logger.PushScopeNested using ${scopenested}. Marked obsolete on NLog 5.0")]
	public static IDisposable PushObject(object value)
	{
		return Push(value);
	}

	/// <summary>
	/// Pops the top message off the NDLC stack.
	/// </summary>
	/// <returns>The top message which is no longer on the stack.</returns>
	/// <remarks>this methods returns a object instead of string, this because of backwards-compatibility</remarks>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static object? Pop()
	{
		return PopObject();
	}

	/// <summary>
	/// Pops the top message from the NDLC stack.
	/// </summary>
	/// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when converting the value to a string.</param>
	/// <returns>The top message, which is removed from the stack, as a string value.</returns>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static string Pop(IFormatProvider formatProvider)
	{
		return FormatHelper.ConvertToString(PopObject() ?? string.Empty, formatProvider);
	}

	/// <summary>
	/// Pops the top message off the current NDLC stack
	/// </summary>
	/// <returns>The object from the top of the NDLC stack, if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by dispose of return value from ScopeContext.PushNestedState or Logger.PushScopeNested. Marked obsolete on NLog 5.0")]
	public static object? PopObject()
	{
		return ScopeContext.PopNestedContextLegacy();
	}

	/// <summary>
	/// Peeks the top object on the current NDLC stack
	/// </summary>
	/// <returns>The object from the top of the NDLC stack, if defined; otherwise <c>null</c>.</returns>
	[Obsolete("Replaced by ScopeContext.PeekNestedState. Marked obsolete on NLog 5.0")]
	public static object? PeekObject()
	{
		return ScopeContext.PeekNestedState();
	}

	/// <summary>
	/// Clears current stack.
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
		return (from o in GetAllObjects()
			select FormatHelper.ConvertToString(o, formatProvider)).ToArray();
	}

	/// <summary>
	/// Gets all objects on the stack. The objects are not removed from the stack.
	/// </summary>
	/// <returns>Array of objects on the stack.</returns>
	[Obsolete("Replaced by ScopeContext.GetAllNestedStates. Marked obsolete on NLog 5.0")]
	public static object[] GetAllObjects()
	{
		return ScopeContext.GetAllNestedStates();
	}
}
