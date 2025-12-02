using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using NLog.Common;
using NLog.Conditions;
using NLog.Config;
using NLog.Internal;
using NLog.LayoutRenderers;
using NLog.LayoutRenderers.Wrappers;

namespace NLog.Layouts;

/// <summary>
/// Parses layout strings.
/// </summary>
internal static class LayoutParser
{
	private static readonly char[] SpecialTokens = new char[4] { '$', '\\', '}', ':' };

	internal static LayoutRenderer[] CompileLayout(string value, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions, out string parsedText)
	{
		if (string.IsNullOrEmpty(value))
		{
			parsedText = string.Empty;
			return ArrayHelper.Empty<LayoutRenderer>();
		}
		if (value.Length < 128 && value.IndexOfAny(SpecialTokens) < 0)
		{
			parsedText = value;
			return new LayoutRenderer[1]
			{
				new LiteralLayoutRenderer(value)
			};
		}
		return CompileLayout(configurationItemFactory, new SimpleStringReader(value), throwConfigExceptions, isNested: false, out parsedText);
	}

	internal static LayoutRenderer[] CompileLayout(ConfigurationItemFactory configurationItemFactory, SimpleStringReader sr, bool? throwConfigExceptions, bool isNested, out string text)
	{
		List<LayoutRenderer> list = new List<LayoutRenderer>();
		StringBuilder stringBuilder = new StringBuilder();
		int position = sr.Position;
		int num;
		while ((num = sr.Peek()) != -1)
		{
			if (isNested)
			{
				if (num == 92)
				{
					sr.Read();
					int num2 = sr.Peek();
					if (EndOfLayout(num2))
					{
						sr.Read();
						stringBuilder.Append((char)num2);
					}
					else
					{
						stringBuilder.Append('\\');
					}
					continue;
				}
				if (EndOfLayout(num))
				{
					break;
				}
			}
			sr.Read();
			if (num == 36 && sr.Peek() == 123)
			{
				AddLiteral(stringBuilder, list);
				LayoutRenderer item = ParseLayoutRenderer(configurationItemFactory, sr, throwConfigExceptions);
				list.Add(item);
			}
			else
			{
				stringBuilder.Append((char)num);
			}
		}
		AddLiteral(stringBuilder, list);
		int position2 = sr.Position;
		MergeLiterals(list);
		text = sr.Substring(position, position2);
		return list.ToArray();
	}

	/// <summary>
	/// Add <see cref="T:NLog.LayoutRenderers.LiteralLayoutRenderer" /> to <paramref name="result" />
	/// </summary>
	/// <param name="literalBuf"></param>
	/// <param name="result"></param>
	private static void AddLiteral(StringBuilder literalBuf, List<LayoutRenderer> result)
	{
		if (literalBuf.Length > 0)
		{
			result.Add(new LiteralLayoutRenderer(literalBuf.ToString()));
			literalBuf.Length = 0;
		}
	}

	private static bool EndOfLayout(int ch)
	{
		if (ch != 125)
		{
			return ch == 58;
		}
		return true;
	}

	private static string ParseLayoutRendererTypeName(SimpleStringReader sr)
	{
		return sr.ReadUntilMatch((int ch) => EndOfLayout(ch));
	}

	private static string ParseParameterNameOrValue(SimpleStringReader sr)
	{
		string text = sr.ReadUntilMatch((int chr) => EndOfLayout(chr) || chr == 61 || chr == 36 || chr == 92);
		if (sr.Peek() != 36 && sr.Peek() != 92)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		ParseLayoutParameterValue(sr, stringBuilder, (int chr) => EndOfLayout(chr) || chr == 61);
		return stringBuilder.ToString();
	}

	private static string ParseParameterStringValue(SimpleStringReader sr)
	{
		string text = sr.ReadUntilMatch((int chr) => EndOfLayout(chr) || chr == 36 || chr == 92);
		if (sr.Peek() != 36 && sr.Peek() != 92)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		if (!ParseLayoutParameterValue(sr, stringBuilder, (int chr) => EndOfLayout(chr)))
		{
			return stringBuilder.ToString();
		}
		string value = stringBuilder.ToString();
		stringBuilder.ClearBuilder();
		return EscapeUnicodeStringValue(value, stringBuilder);
	}

	private static bool ParseLayoutParameterValue(SimpleStringReader sr, StringBuilder parameterValue, Func<int, bool> endOfLayout)
	{
		bool result = false;
		int num = 0;
		int num2;
		while ((num2 = sr.Peek()) != -1 && (!endOfLayout(num2) || num != 0))
		{
			switch (num2)
			{
			case 36:
				sr.Read();
				parameterValue.Append('$');
				if (sr.Peek() == 123)
				{
					parameterValue.Append('{');
					num++;
					sr.Read();
				}
				continue;
			case 125:
				num--;
				break;
			}
			if (num2 == 92)
			{
				sr.Read();
				num2 = sr.Peek();
				if (num == 0 && (endOfLayout(num2) || num2 == 36 || num2 == 61))
				{
					parameterValue.Append((char)sr.Read());
				}
				else if (num2 != -1)
				{
					result = true;
					parameterValue.Append('\\');
					parameterValue.Append((char)sr.Read());
				}
			}
			else
			{
				parameterValue.Append((char)num2);
				sr.Read();
			}
		}
		return result;
	}

	private static string ParseParameterValue(SimpleStringReader sr)
	{
		string text = sr.ReadUntilMatch((int ch) => EndOfLayout(ch) || ch == 92);
		if (sr.Peek() == 92)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder(text);
			int num;
			while ((num = sr.Peek()) != -1 && !EndOfLayout(num))
			{
				if (num == 92)
				{
					sr.Read();
					num = sr.Peek();
					if (EndOfLayout(num))
					{
						stringBuilder.Append((char)sr.Read());
					}
					else if (num != -1)
					{
						flag = true;
						stringBuilder.Append('\\');
						stringBuilder.Append((char)sr.Read());
					}
				}
				else
				{
					stringBuilder.Append((char)num);
					sr.Read();
				}
			}
			text = stringBuilder.ToString();
			if (flag)
			{
				stringBuilder.Length = 0;
				text = EscapeUnicodeStringValue(text, stringBuilder);
			}
		}
		return text;
	}

	private static string EscapeUnicodeStringValue(string value, StringBuilder? nameBuf = null)
	{
		bool flag = false;
		nameBuf = nameBuf ?? new StringBuilder(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (flag)
			{
				flag = false;
				switch (c)
				{
				case '"':
				case '\'':
				case ':':
				case '\\':
				case '{':
				case '}':
					nameBuf.Append(c);
					break;
				case '0':
					nameBuf.Append('\0');
					break;
				case 'a':
					nameBuf.Append('\a');
					break;
				case 'b':
					nameBuf.Append('\b');
					break;
				case 'f':
					nameBuf.Append('\f');
					break;
				case 'n':
					nameBuf.Append('\n');
					break;
				case 'r':
					nameBuf.Append('\r');
					break;
				case 't':
					nameBuf.Append('\t');
					break;
				case 'u':
				{
					char unicode3 = GetUnicode(value, 4, ref i);
					nameBuf.Append(unicode3);
					break;
				}
				case 'U':
				{
					char unicode2 = GetUnicode(value, 8, ref i);
					nameBuf.Append(unicode2);
					break;
				}
				case 'x':
				{
					char unicode = GetUnicode(value, 4, ref i);
					nameBuf.Append(unicode);
					break;
				}
				case 'v':
					nameBuf.Append('\v');
					break;
				default:
					nameBuf.Append(c);
					break;
				}
			}
			else if (c == '\\')
			{
				flag = true;
			}
			else
			{
				nameBuf.Append(c);
			}
		}
		if (flag)
		{
			nameBuf.Append('\\');
		}
		return nameBuf.ToString();
	}

	private static char GetUnicode(string value, int maxDigits, ref int currentIndex)
	{
		int num = 0;
		maxDigits = Math.Min(value.Length - 1, currentIndex + maxDigits);
		while (currentIndex < maxDigits)
		{
			int num2 = value[currentIndex + 1];
			if (num2 >= 48 && num2 <= 57)
			{
				num2 -= 48;
			}
			else if (num2 >= 97 && num2 <= 102)
			{
				num2 = num2 - 97 + 10;
			}
			else
			{
				if (num2 < 65 || num2 > 70)
				{
					break;
				}
				num2 = num2 - 65 + 10;
			}
			num = num * 16 + num2;
			currentIndex++;
		}
		return (char)num;
	}

	private static LayoutRenderer ParseLayoutRenderer(ConfigurationItemFactory configurationItemFactory, SimpleStringReader stringReader, bool? throwConfigExceptions)
	{
		int num = stringReader.Read();
		string text = ParseLayoutRendererTypeName(stringReader);
		LayoutRenderer layoutRenderer = GetLayoutRenderer(text, configurationItemFactory, throwConfigExceptions);
		Dictionary<Type, LayoutRenderer> wrappers = null;
		List<LayoutRenderer> orderedWrappers = null;
		string previousParameterName = null;
		num = stringReader.Read();
		while (num != -1 && num != 125)
		{
			string text2 = ParseParameterNameOrValue(stringReader);
			if (stringReader.Peek() == 61)
			{
				stringReader.Read();
				text2 = text2.Trim();
				LayoutRenderer targetObject = layoutRenderer;
				if (!PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutRenderer, text2, out PropertyInfo result) && TryResolveAmbientLayoutWrapper(text2, configurationItemFactory, ref wrappers, ref orderedWrappers, out LayoutRenderer layoutRenderer2) && layoutRenderer2 != null && PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutRenderer2, text2, out result))
				{
					targetObject = layoutRenderer2;
				}
				if ((object)result == null)
				{
					string value = ParseParameterValue(stringReader);
					if (!string.IsNullOrEmpty(text2) || !StringHelpers.IsNullOrWhiteSpace(value))
					{
						NLogConfigurationException ex = new NLogConfigurationException("${" + text + "} cannot assign unknown property '" + text2 + "='");
						if (throwConfigExceptions ?? ex.MustBeRethrown())
						{
							throw ex;
						}
					}
				}
				else
				{
					object obj = ParseLayoutRendererPropertyValue(configurationItemFactory, stringReader, throwConfigExceptions, text, result);
					if (obj is string stringValue)
					{
						PropertyHelper.SetPropertyFromString(targetObject, result, stringValue, configurationItemFactory);
					}
					else if (obj != null)
					{
						PropertyHelper.SetPropertyValueForObject(targetObject, obj, result);
					}
				}
			}
			else
			{
				text2 = SetDefaultPropertyValue(text2, layoutRenderer, configurationItemFactory, throwConfigExceptions);
			}
			previousParameterName = ValidatePreviousParameterName(previousParameterName, text2, layoutRenderer, throwConfigExceptions);
			num = stringReader.Read();
		}
		return BuildCompleteLayoutRenderer(configurationItemFactory, layoutRenderer, orderedWrappers);
	}

	private static LayoutRenderer BuildCompleteLayoutRenderer(ConfigurationItemFactory configurationItemFactory, LayoutRenderer layoutRenderer, List<LayoutRenderer>? orderedWrappers = null)
	{
		if (orderedWrappers != null)
		{
			layoutRenderer = ApplyWrappers(configurationItemFactory, layoutRenderer, orderedWrappers);
		}
		if (CanBeConvertedToLiteral(configurationItemFactory, layoutRenderer))
		{
			layoutRenderer = ConvertToLiteral(layoutRenderer);
		}
		return layoutRenderer;
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2072")]
	private static object? ParseLayoutRendererPropertyValue(ConfigurationItemFactory configurationItemFactory, SimpleStringReader stringReader, bool? throwConfigExceptions, string targetTypeName, PropertyInfo propertyInfo)
	{
		if (typeof(Layout).IsAssignableFrom(propertyInfo.PropertyType))
		{
			string text;
			Layout layout = new SimpleLayout(CompileLayout(configurationItemFactory, stringReader, throwConfigExceptions, isNested: true, out text), text);
			if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Layout<>))
			{
				layout = (Layout)Activator.CreateInstance(propertyInfo.PropertyType, BindingFlags.Instance | BindingFlags.Public, null, new object[1] { layout }, null);
			}
			return layout;
		}
		if (typeof(ConditionExpression).IsAssignableFrom(propertyInfo.PropertyType))
		{
			try
			{
				return ConditionParser.ParseExpression(stringReader, configurationItemFactory);
			}
			catch (ConditionParseException ex)
			{
				NLogConfigurationException ex2 = new NLogConfigurationException("${" + targetTypeName + "} cannot parse ConditionExpression for property '" + propertyInfo.Name + "='. Error: " + ex.Message, ex);
				if (throwConfigExceptions ?? ex2.MustBeRethrown())
				{
					throw ex2;
				}
				return null;
			}
		}
		if (typeof(string).IsAssignableFrom(propertyInfo.PropertyType))
		{
			return ParseParameterStringValue(stringReader);
		}
		return ParseParameterValue(stringReader);
	}

	private static string? ValidatePreviousParameterName(string? previousParameterName, string parameterName, LayoutRenderer layoutRenderer, bool? throwConfigExceptions)
	{
		if (parameterName != null && parameterName.Equals(previousParameterName, StringComparison.OrdinalIgnoreCase))
		{
			NLogConfigurationException ex = new NLogConfigurationException("'" + layoutRenderer?.GetType()?.Name + "' has same property '" + parameterName + "=' assigned twice");
			if (throwConfigExceptions ?? ex.MustBeRethrown())
			{
				throw ex;
			}
		}
		else
		{
			previousParameterName = parameterName ?? previousParameterName;
		}
		return previousParameterName;
	}

	private static bool TryResolveAmbientLayoutWrapper(string propertyName, ConfigurationItemFactory configurationItemFactory, ref Dictionary<Type, LayoutRenderer>? wrappers, ref List<LayoutRenderer>? orderedWrappers, out LayoutRenderer? layoutRenderer)
	{
		if (!configurationItemFactory.AmbientRendererFactory.TryCreateInstance(propertyName, out LayoutRenderer result) || result == null)
		{
			layoutRenderer = null;
			return false;
		}
		wrappers = wrappers ?? new Dictionary<Type, LayoutRenderer>();
		orderedWrappers = orderedWrappers ?? new List<LayoutRenderer>();
		Type type = result.GetType();
		if (!wrappers.TryGetValue(type, out layoutRenderer))
		{
			wrappers[type] = result;
			orderedWrappers.Add(result);
			layoutRenderer = result;
		}
		return true;
	}

	private static LayoutRenderer GetLayoutRenderer(string typeName, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions)
	{
		LayoutRenderer result = null;
		try
		{
			if (throwConfigExceptions == false && !configurationItemFactory.LayoutRendererFactory.TryCreateInstance(typeName, out result))
			{
				InternalLogger.Debug("Failed to create LayoutRenderer with unknown type-alias: '{0}'", typeName);
				return new LiteralLayoutRenderer(string.Empty);
			}
			result = configurationItemFactory.LayoutRendererFactory.CreateInstance(typeName);
		}
		catch (NLogConfigurationException exception)
		{
			if (throwConfigExceptions ?? exception.MustBeRethrown())
			{
				throw;
			}
		}
		catch (Exception ex)
		{
			NLogConfigurationException ex2 = new NLogConfigurationException("Failed to parse layout containing type: " + typeName + " - " + ex.Message, ex);
			if (throwConfigExceptions ?? ex2.MustBeRethrown())
			{
				throw ex2;
			}
		}
		return result ?? new LiteralLayoutRenderer(string.Empty);
	}

	private static string SetDefaultPropertyValue(string value, LayoutRenderer layoutRenderer, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions)
	{
		if (PropertyHelper.TryGetPropertyInfo(configurationItemFactory, layoutRenderer, string.Empty, out PropertyInfo result) && result != null)
		{
			if (!typeof(Layout).IsAssignableFrom(result.PropertyType) && value.IndexOf('\\') >= 0)
			{
				value = EscapeUnicodeStringValue(value);
			}
			PropertyHelper.SetPropertyFromString(layoutRenderer, result, value, configurationItemFactory);
			return result.Name;
		}
		NLogConfigurationException ex = new NLogConfigurationException("'" + layoutRenderer?.GetType()?.Name + "' has no default property to assign value " + value);
		if (throwConfigExceptions ?? ex.MustBeRethrown())
		{
			throw ex;
		}
		return string.Empty;
	}

	private static LayoutRenderer ApplyWrappers(ConfigurationItemFactory configurationItemFactory, LayoutRenderer lr, List<LayoutRenderer> orderedWrappers)
	{
		for (int num = orderedWrappers.Count - 1; num >= 0; num--)
		{
			WrapperLayoutRendererBase wrapperLayoutRendererBase = (WrapperLayoutRendererBase)orderedWrappers[num];
			InternalLogger.Trace("Wrapping {0} with {1}", lr.GetType(), wrapperLayoutRendererBase.GetType());
			if (CanBeConvertedToLiteral(configurationItemFactory, lr))
			{
				lr = ConvertToLiteral(lr);
			}
			wrapperLayoutRendererBase.Inner = new SimpleLayout(new LayoutRenderer[1] { lr }, string.Empty);
			lr = wrapperLayoutRendererBase;
		}
		return lr;
	}

	private static bool CanBeConvertedToLiteral(ConfigurationItemFactory configurationItemFactory, LayoutRenderer lr)
	{
		foreach (IRenderable item in ObjectGraphScanner.FindReachableObjects<IRenderable>(configurationItemFactory, aggressiveSearch: true, new object[1] { lr }))
		{
			Type type = item.GetType();
			if (!(type == typeof(SimpleLayout)) && !type.IsDefined(typeof(AppDomainFixedOutputAttribute), inherit: false))
			{
				return false;
			}
		}
		return true;
	}

	private static void MergeLiterals(List<LayoutRenderer> list)
	{
		int num = 0;
		while (num + 1 < list.Count)
		{
			if (list[num] is LiteralLayoutRenderer literalLayoutRenderer && list[num + 1] is LiteralLayoutRenderer literalLayoutRenderer2)
			{
				literalLayoutRenderer.Text += literalLayoutRenderer2.Text;
				if (literalLayoutRenderer is LiteralWithRawValueLayoutRenderer literalWithRawValueLayoutRenderer)
				{
					list[num] = new LiteralLayoutRenderer(literalWithRawValueLayoutRenderer.Text);
				}
				list.RemoveAt(num + 1);
			}
			else
			{
				num++;
			}
		}
	}

	private static LayoutRenderer ConvertToLiteral(LayoutRenderer renderer)
	{
		LogEventInfo logEvent = LogEventInfo.CreateNullEvent();
		string text = renderer.Render(logEvent);
		if (renderer is IRawValue rawValue)
		{
			object value;
			bool rawValueSuccess = rawValue.TryGetRawValue(logEvent, out value);
			return new LiteralWithRawValueLayoutRenderer(text, rawValueSuccess, value);
		}
		return new LiteralLayoutRenderer(text);
	}
}
