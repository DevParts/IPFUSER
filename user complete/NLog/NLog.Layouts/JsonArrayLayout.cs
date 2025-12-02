using System.Collections.Generic;
using System.Text;
using NLog.Config;
using NLog.Targets;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that renders LogEvent as JSON-Array
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/JsonArrayLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/JsonArrayLayout">Documentation on NLog Wiki</seealso>
[Layout("JsonArrayLayout")]
[ThreadAgnostic]
public class JsonArrayLayout : Layout
{
	private Layout[]? _precalculateLayouts;

	private IJsonConverter? _jsonConverter;

	private readonly List<Layout> _items = new List<Layout>();

	private IJsonConverter JsonConverter => _jsonConverter ?? (_jsonConverter = ResolveService<IJsonConverter>());

	/// <summary>
	/// Gets the array of items to include in JSON-Array
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(Layout), "item")]
	public IList<Layout> Items => _items;

	/// <summary>
	/// Gets or sets the option to suppress the extra spaces in the output json.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool SuppressSpaces { get; set; } = true;

	/// <summary>
	/// Gets or sets the option to render the empty object value {}
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool RenderEmptyObject { get; set; } = true;

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		base.InitializeLayout();
		_precalculateLayouts = ResolveLayoutPrecalculation(_items);
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
		_jsonConverter = null;
		_precalculateLayouts = null;
		base.CloseLayout();
	}

	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		PrecalculateBuilderInternal(logEvent, target, _precalculateLayouts);
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		int length = target.Length;
		RenderJsonFormattedMessage(logEvent, target);
		if (target.Length == length && RenderEmptyObject)
		{
			target.Append(SuppressSpaces ? "[]" : "[ ]");
		}
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return RenderAllocateBuilder(logEvent);
	}

	private void RenderJsonFormattedMessage(LogEventInfo logEvent, StringBuilder sb)
	{
		int length = sb.Length;
		foreach (Layout item in _items)
		{
			int length2 = sb.Length;
			if (length2 == length)
			{
				sb.Append(SuppressSpaces ? "[" : "[ ");
			}
			else
			{
				sb.Append(SuppressSpaces ? "," : ", ");
			}
			if (!RenderLayoutJsonValue(logEvent, item, sb))
			{
				sb.Length = length2;
			}
		}
		if (sb.Length != length)
		{
			sb.Append(SuppressSpaces ? "]" : " ]");
		}
	}

	private bool RenderLayoutJsonValue(LogEventInfo logEvent, Layout layout, StringBuilder sb)
	{
		int length = sb.Length;
		object rawValue;
		if (layout is JsonLayout)
		{
			layout.Render(logEvent, sb);
		}
		else if (layout.TryGetRawValue(logEvent, out rawValue))
		{
			if (!JsonConverter.SerializeObject(rawValue, sb))
			{
				return false;
			}
		}
		else
		{
			sb.Append('"');
			length = sb.Length;
			layout.Render(logEvent, sb);
			if (length != sb.Length)
			{
				DefaultJsonSerializer.PerformJsonEscapeWhenNeeded(sb, length, escapeUnicode: true);
				sb.Append('"');
			}
		}
		return length != sb.Length;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return ToStringWithNestedItems(_items, (Layout l) => l.ToString());
	}
}
