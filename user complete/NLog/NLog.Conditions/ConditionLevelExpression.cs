namespace NLog.Conditions;

/// <summary>
/// Condition level expression (represented by the <b>level</b> keyword).
/// </summary>
internal sealed class ConditionLevelExpression : ConditionExpression
{
	/// <inheritdoc />
	public override string ToString()
	{
		return "level";
	}

	/// <summary>
	/// Evaluates to the current log level.
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The <see cref="T:NLog.LogLevel" /> object representing current log level.</returns>
	protected override object EvaluateNode(LogEventInfo context)
	{
		return context.Level;
	}
}
