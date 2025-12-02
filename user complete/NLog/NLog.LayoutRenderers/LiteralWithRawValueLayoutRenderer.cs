using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// A string literal with a fixed raw value
/// </summary>
[ThreadAgnostic]
internal sealed class LiteralWithRawValueLayoutRenderer : LiteralLayoutRenderer, IRawValue
{
	private readonly bool _rawValueSuccess;

	private readonly object? _rawValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.LiteralLayoutRenderer" /> class.
	/// </summary>
	/// <param name="text">The literal text value.</param>
	/// <param name="rawValueSuccess"></param>
	/// <param name="rawValue">Fixed raw value</param>
	/// <remarks>This is used by the layout compiler.</remarks>
	public LiteralWithRawValueLayoutRenderer(string text, bool rawValueSuccess, object? rawValue)
	{
		_rawValueSuccess = rawValueSuccess;
		_rawValue = rawValue;
		base.Text = text;
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		value = _rawValue;
		return _rawValueSuccess;
	}
}
