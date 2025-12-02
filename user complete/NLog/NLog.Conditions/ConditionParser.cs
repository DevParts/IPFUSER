using System;
using System.Collections.Generic;
using System.Globalization;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Conditions;

/// <summary>
/// Condition parser. Turns a string representation of condition expression
/// into an expression tree.
/// </summary>
public class ConditionParser
{
	private readonly ConditionTokenizer _tokenizer;

	private readonly ConfigurationItemFactory _configurationItemFactory;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionParser" /> class.
	/// </summary>
	/// <param name="stringReader">The string reader.</param>
	/// <param name="configurationItemFactory">Instance of <see cref="T:NLog.Config.ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.</param>
	private ConditionParser(SimpleStringReader stringReader, ConfigurationItemFactory configurationItemFactory)
	{
		_configurationItemFactory = configurationItemFactory;
		_tokenizer = new ConditionTokenizer(stringReader);
	}

	/// <summary>
	/// Parses the specified condition string and turns it into
	/// <see cref="T:NLog.Conditions.ConditionExpression" /> tree.
	/// </summary>
	/// <param name="expressionText">The expression to be parsed.</param>
	/// <returns>The root of the expression syntax tree which can be used to get the value of the condition in a specified context.</returns>
	public static ConditionExpression ParseExpression(string expressionText)
	{
		Guard.ThrowIfNull(expressionText, "expressionText");
		return ParseExpression(expressionText, ConfigurationItemFactory.Default);
	}

	/// <summary>
	/// Parses the specified condition string and turns it into
	/// <see cref="T:NLog.Conditions.ConditionExpression" /> tree.
	/// </summary>
	/// <param name="expressionText">The expression to be parsed.</param>
	/// <param name="configurationItemFactories">Instance of <see cref="T:NLog.Config.ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.</param>
	/// <returns>The root of the expression syntax tree which can be used to get the value of the condition in a specified context.</returns>
	public static ConditionExpression ParseExpression(string expressionText, ConfigurationItemFactory configurationItemFactories)
	{
		Guard.ThrowIfNull(expressionText, "expressionText");
		ConditionParser conditionParser = new ConditionParser(new SimpleStringReader(expressionText), configurationItemFactories);
		ConditionExpression result = conditionParser.ParseExpression();
		if (!conditionParser._tokenizer.IsEOF())
		{
			throw new ConditionParseException("Unexpected token: " + conditionParser._tokenizer.TokenValue);
		}
		return result;
	}

	/// <summary>
	/// Parses the specified condition string and turns it into
	/// <see cref="T:NLog.Conditions.ConditionExpression" /> tree.
	/// </summary>
	/// <param name="stringReader">The string reader.</param>
	/// <param name="configurationItemFactories">Instance of <see cref="T:NLog.Config.ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.</param>
	/// <returns>
	/// The root of the expression syntax tree which can be used to get the value of the condition in a specified context.
	/// </returns>
	internal static ConditionExpression ParseExpression(SimpleStringReader stringReader, ConfigurationItemFactory configurationItemFactories)
	{
		return new ConditionParser(stringReader, configurationItemFactories).ParseExpression();
	}

	private ConditionMethodExpression ParseMethodPredicate(string functionName)
	{
		List<ConditionExpression> list = new List<ConditionExpression>();
		while (!_tokenizer.IsEOF() && _tokenizer.TokenType != ConditionTokenType.RightParen)
		{
			list.Add(ParseExpression());
			if (_tokenizer.TokenType != ConditionTokenType.Comma)
			{
				break;
			}
			_tokenizer.GetNextToken();
		}
		_tokenizer.Expect(ConditionTokenType.RightParen);
		try
		{
			return CreateMethodExpression(functionName, list);
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Failed to resolve condition method: '{0}'", functionName);
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new ConditionParseException("Cannot resolve function '" + functionName + "'", ex);
		}
	}

	private ConditionMethodExpression CreateMethodExpression(string functionName, List<ConditionExpression> inputParameters)
	{
		if (inputParameters.Count == 0)
		{
			Func<LogEventInfo, object> func = _configurationItemFactory.ConditionMethodFactory.TryCreateInstanceWithNoParameters(functionName);
			if (func != null)
			{
				return ConditionMethodExpression.CreateMethodNoParameters(functionName, func);
			}
		}
		else if (inputParameters.Count == 1)
		{
			Func<LogEventInfo, object, object> func2 = _configurationItemFactory.ConditionMethodFactory.TryCreateInstanceWithOneParameter(functionName);
			if (func2 != null)
			{
				return ConditionMethodExpression.CreateMethodOneParameter(functionName, func2, inputParameters);
			}
		}
		else if (inputParameters.Count == 2)
		{
			Func<LogEventInfo, object, object, object> func3 = _configurationItemFactory.ConditionMethodFactory.TryCreateInstanceWithTwoParameters(functionName);
			if (func3 != null)
			{
				return ConditionMethodExpression.CreateMethodTwoParameters(functionName, func3, inputParameters);
			}
		}
		else if (inputParameters.Count == 3)
		{
			Func<LogEventInfo, object, object, object, object> func4 = _configurationItemFactory.ConditionMethodFactory.TryCreateInstanceWithThreeParameters(functionName);
			if (func4 != null)
			{
				return ConditionMethodExpression.CreateMethodThreeParameters(functionName, func4, inputParameters);
			}
		}
		int manyParameterMinCount;
		int manyParameterMaxCount;
		bool manyParameterWithLogEvent;
		Func<object[], object> func5 = _configurationItemFactory.ConditionMethodFactory.TryCreateInstanceWithManyParameters(functionName, out manyParameterMinCount, out manyParameterMaxCount, out manyParameterWithLogEvent);
		if (func5 == null)
		{
			throw new ConditionParseException("Unknown condition method '" + functionName + "'");
		}
		if (manyParameterMinCount > inputParameters.Count)
		{
			throw new ConditionParseException($"Condition method '{functionName}' requires minimum {manyParameterMinCount} parameters, but passed {inputParameters.Count}.");
		}
		if (manyParameterMaxCount < inputParameters.Count)
		{
			throw new ConditionParseException($"Condition method '{functionName}' requires maximum {manyParameterMaxCount} parameters, but passed {inputParameters.Count}.");
		}
		return ConditionMethodExpression.CreateMethodManyParameters(functionName, func5, inputParameters, manyParameterWithLogEvent);
	}

	private ConditionExpression ParseLiteralExpression()
	{
		if (_tokenizer.IsToken(ConditionTokenType.LeftParen))
		{
			_tokenizer.GetNextToken();
			ConditionExpression result = ParseExpression();
			_tokenizer.Expect(ConditionTokenType.RightParen);
			return result;
		}
		if (_tokenizer.IsToken(ConditionTokenType.Minus))
		{
			_tokenizer.GetNextToken();
			if (!_tokenizer.IsNumber())
			{
				throw new ConditionParseException($"Number expected, got {_tokenizer.TokenType}");
			}
			return ParseNumber(negative: true);
		}
		if (_tokenizer.IsNumber())
		{
			return ParseNumber(negative: false);
		}
		if (_tokenizer.TokenType == ConditionTokenType.String)
		{
			string text = _tokenizer.TokenValue.Substring(1, _tokenizer.TokenValue.Length - 2).Replace("''", "'");
			SimpleLayout simpleLayout = (string.IsNullOrEmpty(text) ? SimpleLayout.Default : new SimpleLayout(text, _configurationItemFactory));
			_tokenizer.GetNextToken();
			if (simpleLayout.IsFixedText)
			{
				return new ConditionLiteralExpression(simpleLayout.FixedText);
			}
			return new ConditionLayoutExpression(simpleLayout);
		}
		if (_tokenizer.TokenType == ConditionTokenType.Keyword)
		{
			string text2 = _tokenizer.EatKeyword();
			if (TryPlainKeywordToExpression(text2, out ConditionExpression expression) && expression != null)
			{
				return expression;
			}
			if (_tokenizer.TokenType == ConditionTokenType.LeftParen)
			{
				_tokenizer.GetNextToken();
				return ParseMethodPredicate(text2);
			}
		}
		throw new ConditionParseException("Unexpected token: " + _tokenizer.TokenValue);
	}

	/// <summary>
	/// Try stringed keyword to <see cref="T:NLog.Conditions.ConditionExpression" />
	/// </summary>
	/// <param name="keyword"></param>
	/// <param name="expression"></param>
	/// <returns>success?</returns>
	private bool TryPlainKeywordToExpression(string keyword, out ConditionExpression? expression)
	{
		if (string.Equals(keyword, "level", StringComparison.OrdinalIgnoreCase))
		{
			expression = new ConditionLevelExpression();
			return true;
		}
		if (string.Equals(keyword, "logger", StringComparison.OrdinalIgnoreCase))
		{
			expression = new ConditionLoggerNameExpression();
			return true;
		}
		if (string.Equals(keyword, "message", StringComparison.OrdinalIgnoreCase))
		{
			expression = new ConditionMessageExpression();
			return true;
		}
		if (string.Equals(keyword, "exception", StringComparison.OrdinalIgnoreCase))
		{
			expression = new ConditionExceptionExpression();
			return true;
		}
		if (string.Equals(keyword, "loglevel", StringComparison.OrdinalIgnoreCase))
		{
			_tokenizer.Expect(ConditionTokenType.Dot);
			expression = new ConditionLiteralExpression(LogLevel.FromString(_tokenizer.EatKeyword()));
			return true;
		}
		if (string.Equals(keyword, "true", StringComparison.OrdinalIgnoreCase))
		{
			expression = ConditionLiteralExpression.True;
			return true;
		}
		if (string.Equals(keyword, "false", StringComparison.OrdinalIgnoreCase))
		{
			expression = ConditionLiteralExpression.False;
			return true;
		}
		if (string.Equals(keyword, "null", StringComparison.OrdinalIgnoreCase))
		{
			expression = ConditionLiteralExpression.Null;
			return true;
		}
		expression = null;
		return false;
	}

	/// <summary>
	/// Parse number
	/// </summary>
	/// <param name="negative">negative number? minus should be parsed first.</param>
	/// <returns></returns>
	private ConditionExpression ParseNumber(bool negative)
	{
		string tokenValue = _tokenizer.TokenValue;
		_tokenizer.GetNextToken();
		if (tokenValue.IndexOf('.') >= 0)
		{
			double num = double.Parse(tokenValue, CultureInfo.InvariantCulture);
			return new ConditionLiteralExpression(negative ? (0.0 - num) : num);
		}
		int num2 = int.Parse(tokenValue, CultureInfo.InvariantCulture);
		return new ConditionLiteralExpression(negative ? (-num2) : num2);
	}

	private ConditionExpression ParseBooleanRelation()
	{
		ConditionExpression conditionExpression = ParseLiteralExpression();
		if (_tokenizer.IsToken(ConditionTokenType.EqualTo))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.Equal);
		}
		if (_tokenizer.IsToken(ConditionTokenType.NotEqual))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.NotEqual);
		}
		if (_tokenizer.IsToken(ConditionTokenType.LessThan))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.Less);
		}
		if (_tokenizer.IsToken(ConditionTokenType.GreaterThan))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.Greater);
		}
		if (_tokenizer.IsToken(ConditionTokenType.LessThanOrEqualTo))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.LessOrEqual);
		}
		if (_tokenizer.IsToken(ConditionTokenType.GreaterThanOrEqualTo))
		{
			_tokenizer.GetNextToken();
			return new ConditionRelationalExpression(conditionExpression, ParseLiteralExpression(), ConditionRelationalOperator.GreaterOrEqual);
		}
		return conditionExpression;
	}

	private ConditionExpression ParseBooleanPredicate()
	{
		if (_tokenizer.IsKeyword("not") || _tokenizer.IsToken(ConditionTokenType.Not))
		{
			_tokenizer.GetNextToken();
			return new ConditionNotExpression(ParseBooleanPredicate());
		}
		return ParseBooleanRelation();
	}

	private ConditionExpression ParseBooleanAnd()
	{
		ConditionExpression conditionExpression = ParseBooleanPredicate();
		while (_tokenizer.IsKeyword("and") || _tokenizer.IsToken(ConditionTokenType.And))
		{
			_tokenizer.GetNextToken();
			conditionExpression = new ConditionAndExpression(conditionExpression, ParseBooleanPredicate());
		}
		return conditionExpression;
	}

	private ConditionExpression ParseBooleanOr()
	{
		ConditionExpression conditionExpression = ParseBooleanAnd();
		while (_tokenizer.IsKeyword("or") || _tokenizer.IsToken(ConditionTokenType.Or))
		{
			_tokenizer.GetNextToken();
			conditionExpression = new ConditionOrExpression(conditionExpression, ParseBooleanAnd());
		}
		return conditionExpression;
	}

	private ConditionExpression ParseBooleanExpression()
	{
		return ParseBooleanOr();
	}

	private ConditionExpression ParseExpression()
	{
		return ParseBooleanExpression();
	}
}
