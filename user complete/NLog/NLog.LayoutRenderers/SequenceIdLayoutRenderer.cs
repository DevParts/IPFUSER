using System;
using System.ComponentModel;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The sequence ID
///
/// Marked obsolete with NLog 6.0, instead use ${counter}
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/SequenceId-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/SequenceId-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("sequenceid")]
[ThreadAgnostic]
[Obsolete("Use ${counter:sequence=global} instead of ${sequenceid}. Marked obsolete with NLog 6.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class SequenceIdLayoutRenderer : LayoutRenderer, IRawValue
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.AppendInvariant(GetValue(logEvent));
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	private static int GetValue(LogEventInfo logEvent)
	{
		return logEvent.SequenceID;
	}
}
