using System;
using System.ComponentModel;
using System.Globalization;
using NLog.Internal;

namespace NLog.Targets;

/// <summary>
/// Line ending mode.
/// </summary>
[TypeConverter(typeof(LineEndingModeConverter))]
public sealed class LineEndingMode : IEquatable<LineEndingMode>
{
	/// <summary>
	/// Provides a type converter to convert <see cref="T:NLog.Targets.LineEndingMode" /> objects to and from other representations.
	/// </summary>
	public class LineEndingModeConverter : TypeConverter
	{
		/// <inheritdoc />
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (!(sourceType == typeof(string)))
			{
				return base.CanConvertFrom(context, sourceType);
			}
			return true;
		}

		/// <inheritdoc />
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is string name))
			{
				return base.ConvertFrom(context, culture, value);
			}
			return FromString(name);
		}
	}

	/// <summary>
	/// Insert platform-dependent <see cref="P:System.Environment.NewLine" /> sequence after each line.
	/// </summary>
	public static readonly LineEndingMode Default = new LineEndingMode("Default", Environment.NewLine);

	/// <summary>
	/// Insert CR LF sequence (ASCII 13, ASCII 10) after each line.
	/// </summary>
	public static readonly LineEndingMode CRLF = new LineEndingMode("CRLF", "\r\n");

	/// <summary>
	/// Insert CR character (ASCII 13) after each line.
	/// </summary>
	public static readonly LineEndingMode CR = new LineEndingMode("CR", "\r");

	/// <summary>
	/// Insert LF character (ASCII 10) after each line.
	/// </summary>
	public static readonly LineEndingMode LF = new LineEndingMode("LF", "\n");

	/// <summary>
	/// Insert null terminator (ASCII 0) after each line.
	/// </summary>
	public static readonly LineEndingMode Null = new LineEndingMode("Null", "\0");

	/// <summary>
	/// Do not insert any line ending.
	/// </summary>
	public static readonly LineEndingMode None = new LineEndingMode("None", string.Empty);

	private readonly string _name;

	private readonly string _newLineCharacters;

	/// <summary>
	/// Gets the name of the LineEndingMode instance.
	/// </summary>
	public string Name => _name;

	/// <summary>
	/// Gets the new line characters (value) of the LineEndingMode instance.
	/// </summary>
	public string NewLineCharacters => _newLineCharacters;

	private LineEndingMode()
	{
		_name = string.Empty;
		_newLineCharacters = string.Empty;
	}

	/// <summary>
	/// Initializes a new instance of <see cref="T:NLog.LogLevel" />.
	/// </summary>
	/// <param name="name">The mode name.</param>
	/// <param name="newLineCharacters">The new line characters to be used.</param>
	private LineEndingMode(string name, string newLineCharacters)
	{
		_name = name;
		_newLineCharacters = newLineCharacters;
	}

	/// <summary>
	///  Returns the <see cref="T:NLog.Targets.LineEndingMode" /> that corresponds to the supplied <paramref name="name" />.
	/// </summary>
	/// <param name="name">
	///  The textual representation of the line ending mode, such as CRLF, LF, Default etc.
	///  Name is not case sensitive.
	/// </param>
	/// <returns>The <see cref="T:NLog.Targets.LineEndingMode" /> value, that corresponds to the <paramref name="name" />.</returns>
	/// <exception cref="T:System.ArgumentOutOfRangeException">There is no line ending mode with the specified name.</exception>
	public static LineEndingMode FromString(string name)
	{
		Guard.ThrowIfNull(name, "name");
		if (name.Equals(CRLF.Name, StringComparison.OrdinalIgnoreCase))
		{
			return CRLF;
		}
		if (name.Equals(LF.Name, StringComparison.OrdinalIgnoreCase))
		{
			return LF;
		}
		if (name.Equals(CR.Name, StringComparison.OrdinalIgnoreCase))
		{
			return CR;
		}
		if (name.Equals(Default.Name, StringComparison.OrdinalIgnoreCase))
		{
			return Default;
		}
		if (name.Equals(Null.Name, StringComparison.OrdinalIgnoreCase))
		{
			return Null;
		}
		if (name.Equals(None.Name, StringComparison.OrdinalIgnoreCase))
		{
			return None;
		}
		throw new ArgumentOutOfRangeException("name", name, "LineEndingMode is out of range");
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.Targets.LineEndingMode" /> objects and returns a
	/// value indicating whether the first one is equal to the second one.
	/// </summary>
	/// <param name="mode1">The first level.</param>
	/// <param name="mode2">The second level.</param>
	/// <returns>The value of <c>mode1.NewLineCharacters == mode2.NewLineCharacters</c>.</returns>
	public static bool operator ==(LineEndingMode mode1, LineEndingMode mode2)
	{
		if ((object)mode1 == null)
		{
			return (object)mode2 == null;
		}
		if ((object)mode2 == null)
		{
			return false;
		}
		return mode1.NewLineCharacters == mode2.NewLineCharacters;
	}

	/// <summary>
	/// Compares two <see cref="T:NLog.Targets.LineEndingMode" /> objects and returns a
	/// value indicating whether the first one is not equal to the second one.
	/// </summary>
	/// <param name="mode1">The first mode</param>
	/// <param name="mode2">The second mode</param>
	/// <returns>The value of <c>mode1.NewLineCharacters != mode2.NewLineCharacters</c>.</returns>
	public static bool operator !=(LineEndingMode mode1, LineEndingMode mode2)
	{
		if ((object)mode1 == null)
		{
			return (object)mode2 != null;
		}
		if ((object)mode2 == null)
		{
			return true;
		}
		return mode1.NewLineCharacters != mode2.NewLineCharacters;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return Name;
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return _newLineCharacters?.GetHashCode() ?? 0;
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		if (obj is LineEndingMode other)
		{
			return Equals(other);
		}
		return false;
	}

	/// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
	/// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
	/// <param name="other">An object to compare with this object.</param>
	public bool Equals(LineEndingMode other)
	{
		if ((object)this != other)
		{
			return string.Equals(_newLineCharacters, other?._newLineCharacters, StringComparison.Ordinal);
		}
		return true;
	}
}
