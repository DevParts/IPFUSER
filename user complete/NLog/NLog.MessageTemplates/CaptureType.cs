namespace NLog.MessageTemplates;

/// <summary>
/// The type of the captured hole
/// </summary>
public enum CaptureType : byte
{
	/// <summary>
	/// Not decided
	/// </summary>
	Unknown,
	/// <summary>
	/// normal {x}
	/// </summary>
	Normal,
	/// <summary>
	///  Serialize operator {@x} (aka destructure)
	/// </summary>
	Serialize,
	/// <summary>
	/// stringification operator {$x}
	/// </summary>
	Stringify
}
