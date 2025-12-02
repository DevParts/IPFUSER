using System;
using System.ComponentModel;
using System.Text;
using NLog.Internal;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Base class for <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />s which wrapping other <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />s.
///
/// This expects the transformation to work on a <see cref="T:System.Text.StringBuilder" />
/// </summary>
[Obsolete("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform() instead. Marked obsolete in NLog 5.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class WrapperLayoutRendererBuilderBase : WrapperLayoutRendererBase
{
	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (builder.Length > 0)
		{
			using (AppendBuilderCreator appendBuilderCreator = new AppendBuilderCreator(mustBeEmpty: true))
			{
				RenderFormattedMessage(logEvent, appendBuilderCreator.Builder);
				TransformFormattedMesssage(logEvent, appendBuilderCreator.Builder);
				appendBuilderCreator.Builder.CopyTo(builder);
				return;
			}
		}
		RenderFormattedMessage(logEvent, builder);
		TransformFormattedMesssage(logEvent, builder);
	}

	/// <inheritdoc />
	[Obsolete("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform() instead. Marked obsolete in NLog 4.6")]
	protected virtual void TransformFormattedMesssage(LogEventInfo logEvent, StringBuilder target)
	{
		TransformFormattedMesssage(target);
	}

	/// <summary>
	/// Transforms the output of another layout.
	/// </summary>
	/// <param name="target">Output to be transform.</param>
	[Obsolete("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform() instead. Marked obsolete in NLog 4.6")]
	protected abstract void TransformFormattedMesssage(StringBuilder target);

	/// <summary>
	/// Renders the inner layout contents.
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="target"><see cref="T:System.Text.StringBuilder" /> for the result</param>
	[Obsolete("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform() instead. Marked obsolete in NLog 4.6")]
	protected virtual void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		base.Inner?.Render(logEvent, target);
	}

	/// <inheritdoc />
	protected sealed override string Transform(string text)
	{
		throw new NotSupportedException("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform()");
	}

	/// <inheritdoc />
	protected sealed override string RenderInner(LogEventInfo logEvent)
	{
		throw new NotSupportedException("Inherit from WrapperLayoutRendererBase and override RenderInnerAndTransform()");
	}
}
