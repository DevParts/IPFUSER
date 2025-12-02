using System.Text;
using System.Threading;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The name of the current thread.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ThreadName-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ThreadName-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("threadname")]
public class ThreadNameLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(GetStringValue());
	}

	private static string GetStringValue()
	{
		return Thread.CurrentThread.Name;
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetStringValue();
	}
}
