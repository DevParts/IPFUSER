namespace NLog.MessageTemplates;

/// <summary>
/// A hole that will be replaced with a value
/// </summary>
internal struct Hole
{
	/// <summary>Parameter name sent to structured loggers.</summary>
	/// <remarks>This is everything between "{" and the first of ",:}".
	/// Including surrounding spaces and names that are numbers.</remarks>
	public readonly string Name;

	/// <summary>Format to render the parameter.</summary>
	/// <remarks>This is everything between ":" and the first unescaped "}"</remarks>
	public readonly string? Format;

	/// <summary>
	/// Type
	/// </summary>
	public readonly CaptureType CaptureType;

	/// <summary>When the template is positional, this is the parsed name of this parameter.</summary>
	/// <remarks>For named templates, the value of Index is undefined.</remarks>
	public readonly short Index;

	/// <summary>Alignment to render the parameter, by default 0.</summary>
	/// <remarks>This is the parsed value between "," and the first of ":}"</remarks>
	public readonly short Alignment;

	/// <summary>
	/// Constructor
	/// </summary>
	public Hole(string name, string? format, CaptureType captureType, short parameterIndex, short alignment)
	{
		Name = name;
		Format = format;
		CaptureType = captureType;
		Index = parameterIndex;
		Alignment = alignment;
	}
}
