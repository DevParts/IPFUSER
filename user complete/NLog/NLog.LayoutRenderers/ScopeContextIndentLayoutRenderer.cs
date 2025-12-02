using System.Collections.Generic;
using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers;

/// <summary>
/// Renders the nested states from <see cref="T:NLog.ScopeContext" /> like a callstack
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ScopeIndent-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ScopeIndent-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("scopeindent")]
public sealed class ScopeContextIndentLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the indent token.
	/// </summary>
	/// <remarks>Default: <c>  </c></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public Layout Indent { get; set; } = Layout.FromLiteral("  ");

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string text = null;
		IList<object> allNestedStateList = ScopeContext.GetAllNestedStateList();
		for (int i = 0; i < allNestedStateList.Count; i++)
		{
			text = text ?? Indent?.Render(logEvent) ?? string.Empty;
			builder.Append(text);
		}
	}
}
