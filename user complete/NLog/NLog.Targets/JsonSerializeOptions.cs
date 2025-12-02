using System;
using System.ComponentModel;

namespace NLog.Targets;

/// <summary>
/// Options for JSON serialization
/// </summary>
public class JsonSerializeOptions
{
	/// <summary>
	/// Add quotes around object keys?
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	[Obsolete("Marked obsolete with NLog 5.0.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool QuoteKeys { get; set; } = true;

	/// <summary>
	/// Format provider for value
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	[Obsolete("Marked obsolete with NLog 5.0. Should always be InvariantCulture.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IFormatProvider? FormatProvider { get; set; }

	/// <summary>
	/// Format string for value
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	[Obsolete("Marked obsolete with NLog 5.0. Should always be InvariantCulture.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string? Format { get; set; }

	/// <summary>
	/// Should non-ascii characters be encoded.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	public bool EscapeUnicode { get; set; }

	/// <summary>
	/// Should forward slashes be escaped? If <see langword="true" />, / will be converted to \/
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	[Obsolete("Marked obsolete with NLog 5.5. Should never escape forward slash")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool EscapeForwardSlash { get; set; }

	/// <summary>
	/// Serialize enum as integer value.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	public bool EnumAsInteger { get; set; }

	/// <summary>
	/// Gets or sets the option to suppress the extra spaces in the output json.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	public bool SuppressSpaces { get; set; } = true;

	/// <summary>
	/// Should dictionary keys be sanitized. All characters must either be letters, numbers or underscore character (_).
	///
	/// Any other characters will be converted to underscore character (_)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	public bool SanitizeDictionaryKeys { get; set; }

	/// <summary>
	/// How far down the rabbit hole should the Json Serializer go with object-reflection before stopping
	/// </summary>
	/// <remarks>Default: <see langword="10" /></remarks>
	public int MaxRecursionLimit { get; set; } = 10;
}
