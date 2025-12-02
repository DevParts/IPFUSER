namespace NLog.Conditions;

/// <summary>
/// Condition <b>or</b> expression.
/// </summary>
internal sealed class ConditionOrExpression : ConditionExpression
{
	/// <summary>
	/// Gets the left expression.
	/// </summary>
	/// <value>The left expression.</value>
	public ConditionExpression LeftExpression { get; }

	/// <summary>
	/// Gets the right expression.
	/// </summary>
	/// <value>The right expression.</value>
	public ConditionExpression RightExpression { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionOrExpression" /> class.
	/// </summary>
	/// <param name="left">Left hand side of the OR expression.</param>
	/// <param name="right">Right hand side of the OR expression.</param>
	public ConditionOrExpression(ConditionExpression left, ConditionExpression right)
	{
		LeftExpression = left;
		RightExpression = right;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"({LeftExpression} or {RightExpression})";
	}

	/// <summary>
	/// Evaluates the expression by evaluating <see cref="P:NLog.Conditions.ConditionOrExpression.LeftExpression" /> and <see cref="P:NLog.Conditions.ConditionOrExpression.RightExpression" /> recursively.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The value of the alternative operator.</returns>
	protected override object EvaluateNode(LogEventInfo context)
	{
		if ((bool)(LeftExpression.Evaluate(context) ?? ConditionExpression.BoxedFalse))
		{
			return ConditionExpression.BoxedTrue;
		}
		if ((bool)(RightExpression.Evaluate(context) ?? ConditionExpression.BoxedFalse))
		{
			return ConditionExpression.BoxedTrue;
		}
		return ConditionExpression.BoxedFalse;
	}
}
