namespace NLog.Conditions;

/// <summary>
/// Condition <b>not</b> expression.
/// </summary>
internal sealed class ConditionNotExpression : ConditionExpression
{
	/// <summary>
	/// Gets the expression to be negated.
	/// </summary>
	/// <value>The expression.</value>
	public ConditionExpression Expression { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionNotExpression" /> class.
	/// </summary>
	/// <param name="expression">The expression.</param>
	public ConditionNotExpression(ConditionExpression expression)
	{
		Expression = expression;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"(not {Expression})";
	}

	/// <inheritdoc />
	protected override object EvaluateNode(LogEventInfo context)
	{
		if (!(bool)(Expression.Evaluate(context) ?? ConditionExpression.BoxedFalse))
		{
			return ConditionExpression.BoxedTrue;
		}
		return ConditionExpression.BoxedFalse;
	}
}
