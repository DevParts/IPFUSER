using System;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.Conditions;

/// <summary>
/// Base class for representing nodes in condition expression trees.
/// </summary>
/// <seealso href="https://github.com/NLog/NLog/wiki/Conditions">Documentation on NLog Wiki</seealso>
[NLogConfigurationItem]
public abstract class ConditionExpression
{
	internal static readonly object BoxedTrue = true;

	internal static readonly object BoxedFalse = false;

	/// <summary>
	/// Converts condition text to a condition expression tree.
	/// </summary>
	/// <param name="conditionExpressionText">Condition text to be converted.</param>
	/// <returns>Condition expression tree.</returns>
	public static implicit operator ConditionExpression?(string? conditionExpressionText)
	{
		if (conditionExpressionText == null)
		{
			return null;
		}
		return ConditionParser.ParseExpression(conditionExpressionText);
	}

	/// <summary>
	/// Evaluates the expression.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>Expression result.</returns>
	public object? Evaluate(LogEventInfo context)
	{
		try
		{
			return EvaluateNode(context);
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Exception occurred when evaluating condition");
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new ConditionEvaluationException("Exception occurred when evaluating condition", ex);
		}
	}

	/// <summary>
	/// Returns a string representation of the expression.
	/// </summary>
	public abstract override string ToString();

	/// <summary>
	/// Evaluates the expression.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>Expression result.</returns>
	protected abstract object? EvaluateNode(LogEventInfo context);
}
