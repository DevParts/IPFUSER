namespace NLog.Conditions;

/// <summary>
/// Condition message expression (represented by the <b>exception</b> keyword).
/// </summary>
internal sealed class ConditionExceptionExpression : ConditionExpression
{
	/// <inheritdoc />
	public override string ToString()
	{
		return "exception";
	}

	/// <summary>
	/// Evaluates the current <see cref="P:NLog.LogEventInfo.Exception" />.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The <see cref="P:NLog.LogEventInfo.Exception" /> object.</returns>
	protected override object? EvaluateNode(LogEventInfo context)
	{
		return context.Exception;
	}
}
