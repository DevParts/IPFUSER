namespace NLog.Conditions;

/// <summary>
/// Condition <b>and</b> expression.
/// </summary>
internal sealed class ConditionAndExpression : ConditionExpression
{
	/// <summary>
	/// Gets the left hand side of the AND expression.
	/// </summary>
	public ConditionExpression Left { get; }

	/// <summary>
	/// Gets the right hand side of the AND expression.
	/// </summary>
	public ConditionExpression Right { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionAndExpression" /> class.
	/// </summary>
	/// <param name="left">Left hand side of the AND expression.</param>
	/// <param name="right">Right hand side of the AND expression.</param>
	public ConditionAndExpression(ConditionExpression left, ConditionExpression right)
	{
		Left = left;
		Right = right;
	}

	/// <summary>
	/// Returns a string representation of this expression.
	/// </summary>
	/// <returns>A concatenated '(Left) and (Right)' string.</returns>
	public override string ToString()
	{
		return $"({Left} and {Right})";
	}

	/// <summary>
	/// Evaluates the expression by evaluating <see cref="P:NLog.Conditions.ConditionAndExpression.Left" /> and <see cref="P:NLog.Conditions.ConditionAndExpression.Right" /> recursively.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The value of the conjunction operator.</returns>
	protected override object EvaluateNode(LogEventInfo context)
	{
		if (!(bool)(Left.Evaluate(context) ?? ConditionExpression.BoxedFalse))
		{
			return ConditionExpression.BoxedFalse;
		}
		if (!(bool)(Right.Evaluate(context) ?? ConditionExpression.BoxedFalse))
		{
			return ConditionExpression.BoxedFalse;
		}
		return ConditionExpression.BoxedTrue;
	}
}
