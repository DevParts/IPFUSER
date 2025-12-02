using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Internal;
using NLog.Layouts;
using NLog.MessageTemplates;

namespace NLog.LayoutRenderers;

/// <summary>
/// Renders the nested states from <see cref="T:NLog.ScopeContext" /> like a callstack
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ScopeNested-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ScopeNested-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("scopenested")]
[LayoutRenderer("ndc")]
[LayoutRenderer("ndlc")]
public sealed class ScopeContextNestedStatesLayoutRenderer : LayoutRenderer
{
	private string _separator = " ";

	private string _separatorOriginal = " ";

	/// <summary>
	/// Gets or sets the number of top stack frames to be rendered.
	/// </summary>
	/// <remarks>Default: <see langword="-1" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public int TopFrames { get; set; } = -1;

	/// <summary>
	/// Gets or sets the number of bottom stack frames to be rendered.
	/// </summary>
	/// <remarks>Default: <see langword="-1" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public int BottomFrames { get; set; } = -1;

	/// <summary>
	/// Gets or sets the separator to be used for concatenating nested logical context output.
	/// </summary>
	/// <remarks>Default: <c> </c></remarks>
	/// <docgen category="Layout Options" order="100" />
	public string Separator
	{
		get
		{
			return _separatorOriginal ?? _separator;
		}
		set
		{
			_separatorOriginal = value;
			_separator = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets how to format each nested state. Ex. like JSON = @
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if (_separatorOriginal != null)
		{
			_separator = SimpleLayout.Evaluate(_separatorOriginal, base.LoggingConfiguration);
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		if (TopFrames == 1)
		{
			object obj = ScopeContext.PeekNestedState();
			if (obj != null)
			{
				AppendFormattedValue(builder, logEvent, obj, Format, Culture);
			}
			return;
		}
		IList<object> allNestedStateList = ScopeContext.GetAllNestedStateList();
		if (allNestedStateList.Count != 0)
		{
			int startPos = 0;
			int endPos = allNestedStateList.Count;
			if (TopFrames != -1)
			{
				endPos = Math.Min(TopFrames, allNestedStateList.Count);
			}
			else if (BottomFrames != -1)
			{
				startPos = allNestedStateList.Count - Math.Min(BottomFrames, allNestedStateList.Count);
			}
			AppendNestedStates(builder, allNestedStateList, startPos, endPos, logEvent);
		}
	}

	private void AppendNestedStates(StringBuilder builder, IList<object> messages, int startPos, int endPos, LogEventInfo logEvent)
	{
		bool flag = "@".Equals(Format);
		string text = _separator ?? string.Empty;
		string text2 = text;
		if (flag)
		{
			text2 = ((text2 == " ") ? ", " : ((!string.IsNullOrEmpty(text2)) ? ("," + text2) : ","));
			builder.Append('[');
			builder.Append(text);
		}
		try
		{
			string value = null;
			for (int num = endPos - 1; num >= startPos; num--)
			{
				builder.Append(value);
				if (flag)
				{
					AppendJsonFormattedValue(messages[num], Culture ?? CultureInfo.InvariantCulture, builder, text, text2);
				}
				else if (messages[num] is IEnumerable<KeyValuePair<string, object>>)
				{
					builder.Append(Convert.ToString(messages[num], GetFormatProvider(logEvent, Culture)));
				}
				else
				{
					AppendFormattedValue(builder, logEvent, messages[num], Format, Culture);
				}
				value = text2;
			}
		}
		finally
		{
			if (flag)
			{
				builder.Append(text);
				builder.Append(']');
			}
		}
	}

	private void AppendJsonFormattedValue(object nestedState, IFormatProvider formatProvider, StringBuilder builder, string separator, string itemSeparator)
	{
		if (nestedState is IEnumerable<KeyValuePair<string, object>> enumerable && HasUniqueCollectionKeys(enumerable))
		{
			builder.Append('{');
			builder.Append(separator);
			string itemSeparator2 = string.Empty;
			using (ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = new ScopeContextPropertyEnumerator<object>(enumerable))
			{
				while (scopeContextPropertyEnumerator.MoveNext())
				{
					KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
					int length = builder.Length;
					if (!AppendJsonProperty(current.Key, current.Value, builder, itemSeparator2))
					{
						builder.Length = length;
					}
					else
					{
						itemSeparator2 = itemSeparator;
					}
				}
			}
			builder.Append(separator);
			builder.Append('}');
		}
		else
		{
			builder.AppendFormattedValue(nestedState, Format, formatProvider, base.ValueFormatter);
		}
	}

	private bool AppendJsonProperty(string propertyName, object? propertyValue, StringBuilder builder, string itemSeparator)
	{
		if (string.IsNullOrEmpty(propertyName))
		{
			return false;
		}
		builder.Append(itemSeparator);
		if (!base.ValueFormatter.FormatValue(propertyName, null, CaptureType.Serialize, null, builder))
		{
			return false;
		}
		builder.Append(": ");
		if (!base.ValueFormatter.FormatValue(propertyValue, null, CaptureType.Serialize, null, builder))
		{
			return false;
		}
		return true;
	}

	private static bool HasUniqueCollectionKeys(IEnumerable<KeyValuePair<string, object>> propertyList)
	{
		if (propertyList is IDictionary<string, object>)
		{
			return true;
		}
		if (propertyList is IReadOnlyDictionary<string, object>)
		{
			return true;
		}
		if (propertyList is IReadOnlyCollection<KeyValuePair<string, object>> readOnlyCollection)
		{
			if (readOnlyCollection.Count <= 1)
			{
				return true;
			}
			if (readOnlyCollection.Count > 10)
			{
				return false;
			}
		}
		return ScopeContextPropertyEnumerator<object>.HasUniqueCollectionKeys(propertyList, StringComparer.Ordinal);
	}
}
