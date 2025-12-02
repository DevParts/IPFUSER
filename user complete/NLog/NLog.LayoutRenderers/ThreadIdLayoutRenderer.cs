using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The identifier of the current thread.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ThreadId-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ThreadId-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("threadid")]
public class ThreadIdLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.AppendInvariant(GetValue());
	}

	private static int GetValue()
	{
		return AsyncHelpers.GetManagedThreadId();
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetValue().ToString(CultureInfo.InvariantCulture);
	}
}
