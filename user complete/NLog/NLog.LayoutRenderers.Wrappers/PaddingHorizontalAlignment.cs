namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Horizontal alignment for padding layout renderers.
/// </summary>
public enum PaddingHorizontalAlignment
{
	/// <summary>
	/// When layout text is too long, align it to the left
	/// (remove characters from the right).
	/// </summary>
	Left,
	/// <summary>
	/// When layout text is too long, align it to the right
	/// (remove characters from the left).
	/// </summary>
	Right
}
