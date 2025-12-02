using System.Collections.Generic;
using System.Text;
using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// A layout containing one or more nested layouts.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/CompoundLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/CompoundLayout">Documentation on NLog Wiki</seealso>
[Layout("CompoundLayout")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public class CompoundLayout : Layout
{
	private Layout[]? _precalculateLayouts;

	private readonly List<Layout> _layouts = new List<Layout>();

	/// <summary>
	/// Gets the inner layouts.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(Layout), "layout")]
	public IList<Layout> Layouts => _layouts;

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		foreach (Layout layout in _layouts)
		{
			layout.Initialize(base.LoggingConfiguration);
		}
		base.InitializeLayout();
		_precalculateLayouts = ResolveLayoutPrecalculation(_layouts);
	}

	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		PrecalculateBuilderInternal(logEvent, target, _precalculateLayouts);
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return RenderAllocateBuilder(logEvent);
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		foreach (Layout layout in _layouts)
		{
			layout.Render(logEvent, target);
		}
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
		_precalculateLayouts = null;
		foreach (Layout layout in _layouts)
		{
			layout.Close();
		}
		base.CloseLayout();
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return ToStringWithNestedItems(_layouts, (Layout l) => l.ToString());
	}
}
