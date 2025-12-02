using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using NLog.Attributes;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Defines available log levels.
/// </summary>
/// <remarks>
/// Log levels ordered by severity:<br />
/// - <see cref="F:NLog.LogLevel.Trace" /> (Ordinal = 0) : Most verbose level. Used for development and seldom enabled in production.<br />
/// - <see cref="F:NLog.LogLevel.Debug" /> (Ordinal = 1) : Debugging the application behavior from internal events of interest.<br />
/// - <see cref="F:NLog.LogLevel.Info" />  (Ordinal = 2) : Information that highlights progress or application lifetime events.<br />
/// - <see cref="F:NLog.LogLevel.Warn" />  (Ordinal = 3) : Warnings about validation issues or temporary failures that can be recovered.<br />
/// - <see cref="F:NLog.LogLevel.Error" /> (Ordinal = 4) : Errors where functionality has failed or <see cref="T:System.Exception" /> have been caught.<br />
/// - <see cref="F:NLog.LogLevel.Fatal" /> (Ordinal = 5) : Most critical level. Application is about to abort.<br />
/// </remarks>
[TypeConverter(typeof(LogLevelTypeConverter))]
public sealed class LogLevel : IComparable<LogLevel>, IComparable, IEquatable<LogLevel>, IFormattable
{
	/// <summary>
	/// Trace log level (Ordinal = 0)
	/// </summary>
	/// <remarks>
	/// Most verbose level. Used for development and seldom enabled in production.
	/// </remarks>
	public static readonly LogLevel Trace = new LogLevel("Trace", 0);

	/// <summary>
	/// Debug log level (Ordinal = 1)
	/// </summary>
	/// <remarks>
	/// Debugging the application behavior from internal events of interest.
	/// </remarks>
	public static readonly LogLevel Debug = new LogLevel("Debug", 1);

	/// <summary>
	/// Info log level (Ordinal = 2)
	/// </summary>
	/// <remarks>
	/// Information that highlights progress or application lifetime events.
	/// </remarks>
	public static readonly LogLevel Info = new LogLevel("Info", 2);

	/// <summary>
	/// Warn log level (Ordinal = 3)
	/// </summary>
	/// <remarks>
	/// Warnings about validation issues or temporary failures that can be recovered.
	/// </remarks>
	public static readonly LogLevel Warn = new LogLevel("Warn", 3);

	/// <summary>
	/// Error log level (Ordinal = 4)
	/// </summary>
	/// <remarks>
	/// Errors where functionality has failed or <see cref="T:System.Exception" /> have been caught.
	/// </remarks>
	public static readonly LogLevel Error = new LogLevel("Error", 4);

	/// <summary>
	/// Fatal log level (Ordinal = 5)
	/// </summary>
	/// <remarks>
	/// Most critical level. Application is about to abort.
	/// </remarks>
	public static readonly LogLevel Fatal = new LogLevel("Fatal", 5);

	/// <summary>
	/// Off log level (Ordinal = 6)
	/// </summary>
	public static readonly LogLevel Off = new LogLevel("Off", 6);

	private static readonly IList<LogLevel> allLevels = new List<LogLevel> { Trace, Debug, Info, Warn, Error, Fatal, Off }.AsReadOnly();

	private static readonly IList<LogLevel> allLoggingLevels = new List<LogLevel> { Trace, Debug, Info, Warn, Error, Fatal }.AsReadOnly();

	private readonly int _ordinal;

	private readonly string _name;

	/// <summary>
	/// Gets all the available log levels (Trace, Debug, Info, Warn, Error, Fatal, Off).
	/// </summary>
	public static IEnumerable<LogLevel> AllLevels => allLevels;

	/// <summary>
	///  Gets all the log levels that can be used to log events (Trace, Debug, Info, Warn, Error, Fatal)
	///  i.e <c>LogLevel.Off</c> is excluded.
	/// </summary>
	public static IEnumerable<LogLevel> AllLoggingLevels => allLoggingLevels;

	internal static LogLevel MaxLevel => Fatal;

	internal static LogLevel MinLevel => Trace;

	/// <summary>
	/// Gets the name of the log level.
	/// </summary>
	public string Name => _name;

	/// <summary>
	/// Gets the ordinal of the log level.
	/// </summary>
	public int Ordinal => _ordinal;

	/// <summary>
	/// Initializes a new instance of <see cref="T:NLog.LogLevel" />.
	/// </summary>
	/// <param name="name">The log level name.</param>
	/// <param name="ordinal">The log level ordinal number.</param>
	private LogLevel(string name, int ordinal)
	{
		_name = name;
		_ordinal = ordinal;
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is equal to the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal == level2.Ordinal</c>.</returns>
	public static bool operator ==(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return true;
		}
		return (level1 ?? Off).Equals(level2);
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is not equal to the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal != level2.Ordinal</c>.</returns>
	public static bool operator !=(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return false;
		}
		return !(level1 ?? Off).Equals(level2);
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is greater than the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal &gt; level2.Ordinal</c>.</returns>
	public static bool operator >(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return false;
		}
		return (level1 ?? Off).CompareTo(level2) > 0;
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is greater than or equal to the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal &gt;= level2.Ordinal</c>.</returns>
	public static bool operator >=(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return true;
		}
		return (level1 ?? Off).CompareTo(level2) >= 0;
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is less than the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal &lt; level2.Ordinal</c>.</returns>
	public static bool operator <(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return false;
		}
		return (level1 ?? Off).CompareTo(level2) < 0;
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.LogLevel" /> objects
	/// and returns a value indicating whether
	/// the first one is less than or equal to the second one.
	/// </summary>
	/// <param name="level1">The first level.</param>
	/// <param name="level2">The second level.</param>
	/// <returns>The value of <c>level1.Ordinal &lt;= level2.Ordinal</c>.</returns>
	public static bool operator <=(LogLevel? level1, LogLevel? level2)
	{
		if ((object)level1 == level2)
		{
			return true;
		}
		return (level1 ?? Off).CompareTo(level2) <= 0;
	}

	/// <summary>
	/// Gets the <see cref="T:NLog.LogLevel" /> that corresponds to the specified ordinal.
	/// </summary>
	/// <param name="ordinal">The ordinal.</param>
	/// <returns>The <see cref="T:NLog.LogLevel" /> instance. For 0 it returns <see cref="F:NLog.LogLevel.Trace" />, 1 gives <see cref="F:NLog.LogLevel.Debug" /> and so on.</returns>
	public static LogLevel FromOrdinal(int ordinal)
	{
		return ordinal switch
		{
			0 => Trace, 
			1 => Debug, 
			2 => Info, 
			3 => Warn, 
			4 => Error, 
			5 => Fatal, 
			6 => Off, 
			_ => throw new ArgumentException($"Unknown loglevel: {ordinal}.", "ordinal"), 
		};
	}

	/// <summary>
	/// Returns the <see cref="T:NLog.LogLevel" /> that corresponds to the supplied <see langword="string" />.
	/// </summary>
	/// <param name="levelName">The textual representation of the log level.</param>
	/// <returns>The enumeration value.</returns>
	public static LogLevel FromString(string levelName)
	{
		Guard.ThrowIfNull(levelName, "levelName");
		if (levelName.Equals("Trace", StringComparison.OrdinalIgnoreCase))
		{
			return Trace;
		}
		if (levelName.Equals("Debug", StringComparison.OrdinalIgnoreCase))
		{
			return Debug;
		}
		if (levelName.Equals("Info", StringComparison.OrdinalIgnoreCase))
		{
			return Info;
		}
		if (levelName.Equals("Warn", StringComparison.OrdinalIgnoreCase))
		{
			return Warn;
		}
		if (levelName.Equals("Error", StringComparison.OrdinalIgnoreCase))
		{
			return Error;
		}
		if (levelName.Equals("Fatal", StringComparison.OrdinalIgnoreCase))
		{
			return Fatal;
		}
		if (levelName.Equals("Off", StringComparison.OrdinalIgnoreCase))
		{
			return Off;
		}
		if (levelName.Equals("None", StringComparison.OrdinalIgnoreCase))
		{
			return Off;
		}
		if (levelName.Equals("Information", StringComparison.OrdinalIgnoreCase))
		{
			return Info;
		}
		if (levelName.Equals("Warning", StringComparison.OrdinalIgnoreCase))
		{
			return Warn;
		}
		throw new ArgumentException("Unknown log level: " + levelName, "levelName");
	}

	/// <summary>
	/// Returns a string representation of the log level.
	/// </summary>
	/// <returns>Log level name.</returns>
	public override string ToString()
	{
		return _name;
	}

	string IFormattable.ToString(string? format, IFormatProvider? formatProvider)
	{
		if (format == null || !"D".Equals(format, StringComparison.OrdinalIgnoreCase))
		{
			return _name;
		}
		return _ordinal.ToString(CultureInfo.InvariantCulture);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return _ordinal;
	}

	/// <inheritdoc />
	public override bool Equals(object? obj)
	{
		return Equals(obj as LogLevel);
	}

	/// <summary>
	/// Determines whether the specified <see cref="T:NLog.LogLevel" /> instance is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="T:NLog.LogLevel" /> to compare with this instance.</param>
	/// <returns>Value of <see langword="true" /> if the specified <see cref="T:NLog.LogLevel" /> is equal to
	/// this instance; otherwise, <see langword="false" />.</returns>
	public bool Equals(LogLevel? other)
	{
		return _ordinal == other?._ordinal;
	}

	/// <summary>
	/// Compares the level to the other <see cref="T:NLog.LogLevel" /> object.
	/// </summary>
	/// <param name="obj">The other object.</param>
	/// <returns>
	/// A value less than zero when this logger's <see cref="P:NLog.LogLevel.Ordinal" /> is
	/// less than the other logger's ordinal, 0 when they are equal and
	/// greater than zero when this ordinal is greater than the
	/// other ordinal.
	/// </returns>
	public int CompareTo(object? obj)
	{
		return CompareTo(obj as LogLevel);
	}

	/// <summary>
	/// Compares the level to the other <see cref="T:NLog.LogLevel" /> object.
	/// </summary>
	/// <param name="other">The other object.</param>
	/// <returns>
	/// A value less than zero when this logger's <see cref="P:NLog.LogLevel.Ordinal" /> is
	/// less than the other logger's ordinal, 0 when they are equal and
	/// greater than zero when this ordinal is greater than the
	/// other ordinal.
	/// </returns>
	public int CompareTo(LogLevel? other)
	{
		return _ordinal - (other ?? Off)._ordinal;
	}
}
