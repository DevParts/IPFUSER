using System;
using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.Layouts;

internal class LayoutTypeValue : ILayoutTypeValue, IPropertyTypeConverter
{
	private readonly Layout _innerLayout;

	private readonly Type _valueType;

	private readonly CultureInfo? _parseValueCulture;

	private readonly string? _parseValueFormat;

	private string? _previousStringValue;

	private object? _previousValue;

	public LoggingConfiguration? LoggingConfiguration => _innerLayout.LoggingConfiguration;

	public bool ThreadAgnostic => _innerLayout.ThreadAgnostic;

	public bool ThreadAgnosticImmutable => _innerLayout.ThreadAgnosticImmutable;

	public StackTraceUsage StackTraceUsage => _innerLayout.StackTraceUsage;

	public virtual IPropertyTypeConverter ValueTypeConverter { get; }

	ILayoutTypeValue ILayoutTypeValue.InnerLayout => this;

	Type ILayoutTypeValue.InnerType => _valueType;

	public LayoutTypeValue(Layout layout, Type valueType, string? parseValueFormat, CultureInfo? parseValueCulture, IPropertyTypeConverter? valueTypeConverter)
	{
		_innerLayout = layout;
		_valueType = valueType;
		_parseValueFormat = parseValueFormat;
		_parseValueCulture = parseValueCulture;
		ValueTypeConverter = valueTypeConverter ?? this;
	}

	public object? TryParseFixedValue()
	{
		if (_innerLayout is SimpleLayout { FixedText: not null } simpleLayout && TryParseValueFromString(simpleLayout.FixedText, out object parsedValue))
		{
			return parsedValue;
		}
		return null;
	}

	protected void InitializeLayout(Layout ownerLayout)
	{
		_innerLayout.Initialize(ownerLayout.LoggingConfiguration ?? _innerLayout.LoggingConfiguration);
		_previousStringValue = null;
		_previousValue = null;
	}

	protected void Close()
	{
		_innerLayout.Close();
		_previousStringValue = null;
		_previousValue = null;
	}

	public object? RenderObjectValue(LogEventInfo logEvent, StringBuilder? stringBuilder)
	{
		if (_innerLayout.TryGetRawValue(logEvent, out object rawValue))
		{
			if (!(rawValue is string text))
			{
				if (rawValue == null)
				{
					return null;
				}
				TryParseValueFromObject(rawValue, out object parsedValue);
				return parsedValue;
			}
			if (string.IsNullOrEmpty(text))
			{
				TryParseValueFromString(text, out object parsedValue2);
				return parsedValue2;
			}
		}
		string previousStringValue = _previousStringValue;
		object previousValue = _previousValue;
		string text2 = RenderStringValue(logEvent, stringBuilder, previousStringValue);
		if (previousStringValue != null && previousStringValue == text2)
		{
			return previousValue;
		}
		if (TryParseValueFromString(text2, out object parsedValue3))
		{
			if (string.IsNullOrEmpty(previousStringValue) || (text2 != null && text2.Length < 3))
			{
				_previousValue = parsedValue3;
				_previousStringValue = text2;
			}
			return parsedValue3;
		}
		return null;
	}

	private string RenderStringValue(LogEventInfo logEvent, StringBuilder? stringBuilder, string? previousStringValue)
	{
		if (_innerLayout is IStringValueRenderer stringValueRenderer)
		{
			string formattedString = stringValueRenderer.GetFormattedString(logEvent);
			if (formattedString != null)
			{
				return formattedString;
			}
		}
		if (stringBuilder != null && stringBuilder.Length == 0)
		{
			_innerLayout.Render(logEvent, stringBuilder);
			if (stringBuilder.Length == 0)
			{
				return string.Empty;
			}
			if (previousStringValue == null || string.IsNullOrEmpty(previousStringValue) || !stringBuilder.EqualTo(previousStringValue))
			{
				return stringBuilder.ToString();
			}
			return previousStringValue;
		}
		return _innerLayout.Render(logEvent);
	}

	private bool TryParseValueFromObject(object rawValue, out object? parsedValue)
	{
		try
		{
			parsedValue = ParseValueFromObject(rawValue);
			return true;
		}
		catch (Exception ex)
		{
			parsedValue = null;
			InternalLogger.Warn(ex, "Failed converting object '{0}' of type {1} into type {2}", rawValue, rawValue?.GetType(), _valueType);
			return false;
		}
	}

	private object? ParseValueFromObject(object rawValue)
	{
		return ValueTypeConverter.Convert(rawValue, _valueType, _parseValueFormat, _parseValueCulture);
	}

	private bool TryParseValueFromString(string stringValue, out object? parsedValue)
	{
		if (string.IsNullOrEmpty(stringValue))
		{
			parsedValue = ((_valueType == typeof(string)) ? stringValue : null);
			return true;
		}
		return TryParseValueFromObject(stringValue, out parsedValue);
	}

	public override string ToString()
	{
		return _innerLayout.ToString();
	}

	object? IPropertyTypeConverter.Convert(object? propertyValue, Type propertyType, string? format, IFormatProvider? formatProvider)
	{
		return null;
	}
}
