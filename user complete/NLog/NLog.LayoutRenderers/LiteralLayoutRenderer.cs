using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// A string literal.
/// </summary>
/// <remarks>
/// This is used to escape '${' sequence
/// as ;${literal:text=${}'
///
/// <a href="https://github.com/NLog/NLog/wiki/Literal-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Literal-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("literal")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public class LiteralLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the literal text.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.LiteralLayoutRenderer" /> class.
	/// </summary>
	public LiteralLayoutRenderer()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.LiteralLayoutRenderer" /> class.
	/// </summary>
	/// <param name="text">The literal text value.</param>
	/// <remarks>This is used by the layout compiler.</remarks>
	public LiteralLayoutRenderer(string text)
	{
		Text = text;
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(Text);
	}
}
