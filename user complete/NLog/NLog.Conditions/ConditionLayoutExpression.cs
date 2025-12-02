using System.Text;
using System.Threading;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Conditions;

/// <summary>
/// Condition layout expression (represented by a string literal
/// with embedded ${}).
/// </summary>
internal sealed class ConditionLayoutExpression : ConditionExpression
{
	private readonly SimpleLayout _simpleLayout;

	private StringBuilder? _fastObjectPool;

	/// <summary>
	/// Gets the layout.
	/// </summary>
	/// <value>The layout.</value>
	public Layout Layout => _simpleLayout;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionLayoutExpression" /> class.
	/// </summary>
	/// <param name="layout">The layout.</param>
	public ConditionLayoutExpression(SimpleLayout layout)
	{
		_simpleLayout = layout;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return "'" + _simpleLayout.ToString() + "'";
	}

	/// <summary>
	/// Evaluates the expression by rendering the formatted output from
	/// the <see cref="P:NLog.Conditions.ConditionLayoutExpression.Layout" />
	/// </summary>
	/// <param name="context">Evaluation context.</param>
	/// <returns>The output rendered from the layout.</returns>
	protected override object EvaluateNode(LogEventInfo context)
	{
		if (_simpleLayout.IsSimpleStringText || !_simpleLayout.ThreadAgnostic)
		{
			return _simpleLayout.Render(context);
		}
		StringBuilder stringBuilder = Interlocked.Exchange(ref _fastObjectPool, null) ?? new StringBuilder();
		try
		{
			_simpleLayout.Render(context, stringBuilder);
			return stringBuilder.ToString();
		}
		finally
		{
			stringBuilder.ClearBuilder();
			Interlocked.Exchange(ref _fastObjectPool, stringBuilder);
		}
	}
}
