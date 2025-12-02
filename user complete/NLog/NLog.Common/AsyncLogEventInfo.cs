using System;

namespace NLog.Common;

/// <summary>
/// Represents the logging event with asynchronous continuation.
/// </summary>
public struct AsyncLogEventInfo : IEquatable<AsyncLogEventInfo>
{
	/// <summary>
	/// Gets the log event.
	/// </summary>
	public LogEventInfo LogEvent { get; }

	/// <summary>
	/// Gets the continuation.
	/// </summary>
	public AsyncContinuation Continuation { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Common.AsyncLogEventInfo" /> struct.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <param name="continuation">The continuation.</param>
	public AsyncLogEventInfo(LogEventInfo logEvent, AsyncContinuation continuation)
	{
		LogEvent = logEvent;
		Continuation = continuation;
	}

	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="eventInfo1">The event info1.</param>
	/// <param name="eventInfo2">The event info2.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator ==(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
	{
		return eventInfo1.Equals(eventInfo2);
	}

	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="eventInfo1">The event info1.</param>
	/// <param name="eventInfo2">The event info2.</param>
	/// <returns>The result of the operator.</returns>
	public static bool operator !=(AsyncLogEventInfo eventInfo1, AsyncLogEventInfo eventInfo2)
	{
		return !eventInfo1.Equals(eventInfo2);
	}

	/// <inheritdoc />
	public bool Equals(AsyncLogEventInfo other)
	{
		if ((object)Continuation == other.Continuation)
		{
			return LogEvent == other.LogEvent;
		}
		return false;
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		if (obj is AsyncLogEventInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return LogEvent.GetHashCode() ^ Continuation.GetHashCode();
	}
}
