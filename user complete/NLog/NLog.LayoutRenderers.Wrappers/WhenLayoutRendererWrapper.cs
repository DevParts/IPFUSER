using System;
using System.Text;
using NLog.Conditions;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Only outputs the inner layout when the specified condition has been met.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/When-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/When-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("when")]
[AmbientProperty("When")]
[ThreadAgnostic]
public sealed class WhenLayoutRendererWrapper : WrapperLayoutRendererBase, IRawValue
{
	/// <summary>
	/// Gets or sets the condition that must be met for the <see cref="P:NLog.LayoutRenderers.Wrappers.WrapperLayoutRendererBase.Inner" /> layout to be printed.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see langword="null" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public ConditionExpression? When { get; set; }

	/// <summary>
	/// If <see cref="P:NLog.LayoutRenderers.Wrappers.WhenLayoutRendererWrapper.When" /> is not met, print this layout.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public Layout Else { get; set; } = Layout.Empty;

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		if (When == null)
		{
			throw new NLogConfigurationException("When-LayoutRenderer When-property must be assigned.");
		}
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		int length = builder.Length;
		try
		{
			if (ShouldRenderInner(logEvent))
			{
				base.Inner?.Render(logEvent, builder);
			}
			else
			{
				Else?.Render(logEvent, builder);
			}
		}
		catch
		{
			builder.Length = length;
			throw;
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	private bool ShouldRenderInner(LogEventInfo logEvent)
	{
		if (When != null)
		{
			return true.Equals(When.Evaluate(logEvent));
		}
		return true;
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		if (ShouldRenderInner(logEvent))
		{
			return TryGetRawValueFromLayout(logEvent, base.Inner, out value);
		}
		return TryGetRawValueFromLayout(logEvent, Else, out value);
	}

	private static bool TryGetRawValueFromLayout(LogEventInfo logEvent, Layout layout, out object? value)
	{
		if (layout == null)
		{
			value = null;
			return false;
		}
		return layout.TryGetRawValue(logEvent, out value);
	}
}
