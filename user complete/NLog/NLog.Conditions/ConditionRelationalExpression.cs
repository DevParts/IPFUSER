using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NLog.Common;

namespace NLog.Conditions;

/// <summary>
/// Condition relational (<b>==</b>, <b>!=</b>, <b>&lt;</b>, <b>&lt;=</b>,
/// <b>&gt;</b> or <b>&gt;=</b>) expression.
/// </summary>
internal sealed class ConditionRelationalExpression : ConditionExpression
{
	/// <summary>
	/// Dictionary from type to index. Lower index should be tested first.
	/// </summary>
	private static Dictionary<Type, int> TypePromoteOrder = BuildTypeOrderDictionary();

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
	/// Gets the relational operator.
	/// </summary>
	/// <value>The operator.</value>
	public ConditionRelationalOperator RelationalOperator { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Conditions.ConditionRelationalExpression" /> class.
	/// </summary>
	/// <param name="leftExpression">The left expression.</param>
	/// <param name="rightExpression">The right expression.</param>
	/// <param name="relationalOperator">The relational operator.</param>
	public ConditionRelationalExpression(ConditionExpression leftExpression, ConditionExpression rightExpression, ConditionRelationalOperator relationalOperator)
	{
		LeftExpression = leftExpression;
		RightExpression = rightExpression;
		RelationalOperator = relationalOperator;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"({LeftExpression} {GetOperatorString()} {RightExpression})";
	}

	/// <inheritdoc />
	protected override object EvaluateNode(LogEventInfo context)
	{
		object? leftValue = LeftExpression.Evaluate(context);
		object rightValue = RightExpression.Evaluate(context);
		if (!Compare(leftValue, rightValue, RelationalOperator))
		{
			return ConditionExpression.BoxedFalse;
		}
		return ConditionExpression.BoxedTrue;
	}

	/// <summary>
	/// Compares the specified values using specified relational operator.
	/// </summary>
	/// <param name="leftValue">The first value.</param>
	/// <param name="rightValue">The second value.</param>
	/// <param name="relationalOperator">The relational operator.</param>
	/// <returns>Result of the given relational operator.</returns>
	private static bool Compare(object? leftValue, object? rightValue, ConditionRelationalOperator relationalOperator)
	{
		IComparer ordinal = StringComparer.Ordinal;
		PromoteTypes(ref leftValue, ref rightValue);
		return relationalOperator switch
		{
			ConditionRelationalOperator.Equal => ordinal.Compare(leftValue, rightValue) == 0, 
			ConditionRelationalOperator.NotEqual => ordinal.Compare(leftValue, rightValue) != 0, 
			ConditionRelationalOperator.Greater => ordinal.Compare(leftValue, rightValue) > 0, 
			ConditionRelationalOperator.GreaterOrEqual => ordinal.Compare(leftValue, rightValue) >= 0, 
			ConditionRelationalOperator.LessOrEqual => ordinal.Compare(leftValue, rightValue) <= 0, 
			ConditionRelationalOperator.Less => ordinal.Compare(leftValue, rightValue) < 0, 
			_ => throw new NotSupportedException($"Relational operator {relationalOperator} is not supported."), 
		};
	}

	/// <summary>
	/// Promote values to the type needed for the comparison, e.g. parse a string to int.
	/// </summary>
	/// <param name="leftValue"></param>
	/// <param name="rightValue"></param>
	private static void PromoteTypes(ref object? leftValue, ref object? rightValue)
	{
		if (leftValue == rightValue || leftValue == null || rightValue == null)
		{
			return;
		}
		Type type = leftValue.GetType();
		Type type2 = rightValue.GetType();
		if (type == type2)
		{
			return;
		}
		int order = GetOrder(type);
		int order2 = GetOrder(type2);
		if (order < order2)
		{
			if (TryPromoteTypes(ref rightValue, type, ref leftValue, type2))
			{
				return;
			}
		}
		else if (TryPromoteTypes(ref leftValue, type2, ref rightValue, type))
		{
			return;
		}
		throw new ConditionEvaluationException("Cannot find common type for '" + type.Name + "' and '" + type2.Name + "'.");
	}

	/// <summary>
	/// Promotes <paramref name="val" /> to type
	/// </summary>
	/// <param name="val"></param>
	/// <param name="type1"></param>
	/// <returns>success?</returns>
	private static bool TryPromoteType(ref object val, Type type1)
	{
		try
		{
			if (type1 == typeof(DateTime))
			{
				val = Convert.ToDateTime(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(double))
			{
				val = Convert.ToDouble(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(float))
			{
				val = Convert.ToSingle(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(decimal))
			{
				val = Convert.ToDecimal(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(long))
			{
				val = Convert.ToInt64(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(int))
			{
				val = Convert.ToInt32(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(bool))
			{
				val = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
				return true;
			}
			if (type1 == typeof(LogLevel))
			{
				string levelName = Convert.ToString(val, CultureInfo.InvariantCulture);
				val = LogLevel.FromString(levelName);
				return true;
			}
			if (type1 == typeof(string))
			{
				val = Convert.ToString(val, CultureInfo.InvariantCulture);
				return true;
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Debug("conversion of {0} to {1} failed - {2}", val, type1.Name, ex.Message);
		}
		return false;
	}

	/// <summary>
	/// Try to promote both values. First try to promote <paramref name="val1" /> to <paramref name="type1" />,
	///  when failed, try <paramref name="val2" /> to <paramref name="type2" />.
	/// </summary>
	/// <returns></returns>
	private static bool TryPromoteTypes(ref object val1, Type type1, ref object val2, Type type2)
	{
		if (!TryPromoteType(ref val1, type1))
		{
			return TryPromoteType(ref val2, type2);
		}
		return true;
	}

	/// <summary>
	/// Get the order for the type for comparison.
	/// </summary>
	/// <param name="type1"></param>
	/// <returns>index, 0 to max int. Lower is first</returns>
	private static int GetOrder(Type type1)
	{
		if (TypePromoteOrder.TryGetValue(type1, out var value))
		{
			return value;
		}
		return int.MaxValue;
	}

	/// <summary>
	/// Build the dictionary needed for the order of the types.
	/// </summary>
	/// <returns></returns>
	private static Dictionary<Type, int> BuildTypeOrderDictionary()
	{
		List<Type> list = new List<Type>
		{
			typeof(DateTime),
			typeof(double),
			typeof(float),
			typeof(decimal),
			typeof(long),
			typeof(int),
			typeof(bool),
			typeof(LogLevel),
			typeof(string)
		};
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			dictionary.Add(list[i], i);
		}
		return dictionary;
	}

	/// <summary>
	/// Get the string representing the current <see cref="T:NLog.Conditions.ConditionRelationalOperator" />
	/// </summary>
	/// <returns></returns>
	private string GetOperatorString()
	{
		return RelationalOperator switch
		{
			ConditionRelationalOperator.Equal => "==", 
			ConditionRelationalOperator.NotEqual => "!=", 
			ConditionRelationalOperator.Greater => ">", 
			ConditionRelationalOperator.Less => "<", 
			ConditionRelationalOperator.GreaterOrEqual => ">=", 
			ConditionRelationalOperator.LessOrEqual => "<=", 
			_ => throw new NotSupportedException($"Relational operator {RelationalOperator} is not supported."), 
		};
	}
}
