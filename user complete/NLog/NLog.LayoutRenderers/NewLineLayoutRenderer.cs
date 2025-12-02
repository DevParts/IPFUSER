using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// A newline literal.
/// </summary>
[LayoutRenderer("newline")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public class NewLineLayoutRenderer : LayoutRenderer
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.AppendLine();
	}
}
