namespace NLog.Conditions;

/// <summary>
/// Condition message expression (represented by the <b>message</b> keyword).
/// </summary>
internal sealed class ConditionMessageExpression : ConditionExpression
{
	/// <inheritdoc />
	public override string ToString()
	{
		return "message";
	}

	/// <summary>
	/// Evaluates to the logger message.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The logger message.</returns>
	protected override object EvaluateNode(LogEventInfo context)
	{
		return context.FormattedMessage;
	}
}
