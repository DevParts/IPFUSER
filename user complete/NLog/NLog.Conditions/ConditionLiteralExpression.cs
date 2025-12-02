using System;
using System.Globalization;

namespace NLog.Conditions;

/// <summary>
/// Condition literal expression (numeric, <b>LogLevel.XXX</b>, <b>true</b> or <b>false</b>).
/// </summary>
internal sealed class ConditionLiteralExpression : ConditionExpression
{
	public static readonly ConditionLiteralExpression Null = new ConditionLiteralExpression(null);

	public static readonly ConditionLiteralExpression True = new ConditionLiteralExpression(ConditionExpression.BoxedTrue);

	public static readonly ConditionLiteralExpression False = new ConditionLiteralExpression(ConditionExpression.BoxedFalse);

	/// <summary>
	/// Gets the literal value.
	/// </summary>
	/// <value>The literal value.</value>
	public object? LiteralValue { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionLiteralExpression" /> class.
	/// </summary>
	/// <param name="literalValue">Literal value.</param>
	public ConditionLiteralExpression(object? literalValue)
	{
		LiteralValue = literalValue;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (LiteralValue == null)
		{
			return "null";
		}
		if (LiteralValue is string text)
		{
			return "'" + text + "'";
		}
		if (LiteralValue is char c)
		{
			return $"'{c}'";
		}
		return Convert.ToString(LiteralValue, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Evaluates the expression.
	/// </summary>
	/// <param name="context">Evaluation context. Ignored.</param>
	/// <returns>The literal value as passed in the constructor.</returns>
	protected override object? EvaluateNode(LogEventInfo context)
	{
		return LiteralValue;
	}
}
