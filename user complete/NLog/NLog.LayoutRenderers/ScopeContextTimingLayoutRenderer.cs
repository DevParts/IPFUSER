using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NLog.Internal;
using NLog.Time;

namespace NLog.LayoutRenderers;

/// <summary>
/// <see cref="T:NLog.ScopeContext" /> Timing Renderer (Async scope)
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ScopeTiming-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ScopeTiming-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("scopetiming")]
[LayoutRenderer("ndlctiming")]
public sealed class ScopeContextTimingLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets whether to only include the duration of the last scope created
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool CurrentScope { get; set; }

	/// <summary>
	/// Gets or sets whether to just display the scope creation time, and not the duration
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool StartTime { get; set; }

	/// <summary>
	/// Gets or sets whether to just display the scope creation time, and not the duration
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by StartTime. Marked obsolete on NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ScopeBeginTime
	{
		get
		{
			return StartTime;
		}
		set
		{
			StartTime = value;
		}
	}

	/// <summary>
	/// Gets or sets the TimeSpan format. Can be any argument accepted by TimeSpan.ToString(format).
	///
	/// When Format has not been specified, then it will render TimeSpan.TotalMilliseconds
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		TimeSpan? timeSpan = (CurrentScope ? ScopeContext.PeekInnerNestedDuration() : ScopeContext.PeekOuterNestedDuration());
		if (!timeSpan.HasValue)
		{
			return;
		}
		if (timeSpan.Value < TimeSpan.Zero)
		{
			timeSpan = TimeSpan.Zero;
		}
		if (StartTime)
		{
			builder.Append(TimeSource.Current.Time.Subtract(timeSpan.Value).ToString(Format, Culture));
		}
		else if (string.IsNullOrEmpty(Format))
		{
			double totalMilliseconds = timeSpan.Value.TotalMilliseconds;
			if (Culture == CultureInfo.InvariantCulture)
			{
				RenderAppendDurationMs(builder, totalMilliseconds);
			}
			else
			{
				builder.Append(totalMilliseconds.ToString("0.###", Culture));
			}
		}
		else
		{
			builder.Append(timeSpan.Value.ToString(Format, Culture));
		}
	}

	private static void RenderAppendDurationMs(StringBuilder builder, double scopeDurationMs)
	{
		long num = (long)scopeDurationMs;
		if (num >= 0 && num < uint.MaxValue)
		{
			builder.AppendInvariant((uint)num);
		}
		else
		{
			builder.Append(num);
		}
		int num2 = (int)((scopeDurationMs - (double)num) * 1000.0);
		if (num2 > 0)
		{
			builder.Append('.');
			if (num2 < 100)
			{
				builder.Append('0');
			}
			if (num2 < 10)
			{
				builder.Append('0');
			}
			builder.AppendInvariant(num2);
		}
		else
		{
			builder.Append(".0");
		}
	}
}
