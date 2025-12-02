using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Targets;

namespace NLog.Layouts;

/// <summary>
/// JSON attribute.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/JsonLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/JsonLayout">Documentation on NLog Wiki</seealso>
[NLogConfigurationItem]
public class JsonAttribute
{
	private readonly ValueTypeLayoutInfo _layoutInfo = new ValueTypeLayoutInfo();

	private string _name;

	private bool _includeEmptyValue;

	/// <summary>
	/// Gets or sets the name of the attribute.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="1" />
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				_name = value;
				return;
			}
			if (value.All((char chr) => char.IsLetterOrDigit(chr)))
			{
				_name = value;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			DefaultJsonSerializer.AppendStringEscape(stringBuilder, value.Trim(), escapeUnicode: false);
			_name = stringBuilder.ToString();
		}
	}

	/// <summary>
	/// Gets or sets the layout used for rendering the attribute value.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout
	{
		get
		{
			return _layoutInfo.Layout;
		}
		set
		{
			_layoutInfo.Layout = value;
		}
	}

	/// <summary>
	/// Gets or sets the result value type, for conversion of layout rendering output
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public Type? ValueType
	{
		get
		{
			return _layoutInfo.ValueType;
		}
		set
		{
			_layoutInfo.ValueType = value;
		}
	}

	/// <summary>
	/// Gets or sets the fallback value when result value is not available
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public Layout? DefaultValue
	{
		get
		{
			return _layoutInfo.DefaultValue;
		}
		set
		{
			_layoutInfo.DefaultValue = value;
		}
	}

	/// <summary>
	/// Gets or sets whether output should be encoded as Json-String-Property, or be treated as valid json.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool Encode { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to escape non-ascii characters
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool EscapeUnicode { get; set; }

	/// <summary>
	/// Should forward slashes be escaped? If true, / will be converted to \/
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	[Obsolete("Marked obsolete since forward slash are valid JSON. Marked obsolete with NLog v5.4")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool EscapeForwardSlash { get; set; }

	/// <summary>
	/// Gets or sets whether empty attribute value should be included in the output.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Empty value is either null or empty string</remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool IncludeEmptyValue
	{
		get
		{
			return _includeEmptyValue;
		}
		set
		{
			_includeEmptyValue = value;
			_layoutInfo.ForceDefaultValueNull = !value;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.JsonAttribute" /> class.
	/// </summary>
	public JsonAttribute()
		: this(string.Empty, NLog.Layouts.Layout.Empty, encode: true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.JsonAttribute" /> class.
	/// </summary>
	/// <param name="name">The name of the attribute.</param>
	/// <param name="layout">The layout of the attribute's value.</param>
	public JsonAttribute(string name, Layout layout)
		: this(name, layout, encode: true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.JsonAttribute" /> class.
	/// </summary>
	/// <param name="name">The name of the attribute.</param>
	/// <param name="layout">The layout of the attribute's value.</param>
	/// <param name="encode">Encode value with json-encode</param>
	public JsonAttribute(string name, Layout layout, bool encode)
	{
		Name = name;
		_name = Name;
		Layout = layout;
		Encode = encode;
	}

	internal bool RenderAppendJsonValue(LogEventInfo logEvent, IJsonConverter jsonConverter, StringBuilder builder)
	{
		if ((object)ValueType == null)
		{
			if (Encode)
			{
				builder.Append('"');
			}
			int length = builder.Length;
			Layout?.Render(logEvent, builder);
			if (!IncludeEmptyValue && builder.Length <= length)
			{
				return false;
			}
			if (Encode)
			{
				DefaultJsonSerializer.PerformJsonEscapeWhenNeeded(builder, length, EscapeUnicode);
				builder.Append('"');
			}
		}
		else
		{
			object obj = _layoutInfo.RenderValue(logEvent);
			if (!IncludeEmptyValue && StringHelpers.IsNullOrEmptyString(obj))
			{
				return false;
			}
			jsonConverter.SerializeObject(obj, builder);
		}
		return true;
	}
}
