using System;
using System.Collections.Generic;
using System.Text;
using NLog.Internal;

namespace NLog.Conditions;

internal sealed class ConditionMethodExpression : ConditionExpression
{
	private interface IEvaluateMethod
	{
		object? EvaluateNode(LogEventInfo logEvent);
	}

	private sealed class EvaluateMethodNoParameters : IEvaluateMethod
	{
		private readonly Func<LogEventInfo, object?> _method;

		public EvaluateMethodNoParameters(Func<LogEventInfo, object?> method)
		{
			_method = Guard.ThrowIfNull(method, "method");
		}

		public object? EvaluateNode(LogEventInfo logEvent)
		{
			return _method(logEvent);
		}
	}

	private sealed class EvaluateMethodOneParameter : IEvaluateMethod
	{
		private readonly Func<LogEventInfo, object?, object?> _method;

		private readonly Func<LogEventInfo, object?> _methodParameter;

		public EvaluateMethodOneParameter(Func<LogEventInfo, object?, object?> method, Func<LogEventInfo, object?> methodParameter)
		{
			_method = Guard.ThrowIfNull(method, "method");
			_methodParameter = Guard.ThrowIfNull(methodParameter, "methodParameter");
		}

		public object? EvaluateNode(LogEventInfo logEvent)
		{
			object arg = _methodParameter(logEvent);
			return _method(logEvent, arg);
		}
	}

	private sealed class EvaluateMethodTwoParameters : IEvaluateMethod
	{
		private readonly Func<LogEventInfo, object?, object?, object?> _method;

		private readonly Func<LogEventInfo, object?> _methodParameterArg1;

		private readonly Func<LogEventInfo, object?> _methodParameterArg2;

		public EvaluateMethodTwoParameters(Func<LogEventInfo, object?, object?, object?> method, Func<LogEventInfo, object?> methodParameterArg1, Func<LogEventInfo, object?> methodParameterArg2)
		{
			_method = Guard.ThrowIfNull(method, "method");
			_methodParameterArg1 = Guard.ThrowIfNull(methodParameterArg1, "methodParameterArg1");
			_methodParameterArg2 = Guard.ThrowIfNull(methodParameterArg2, "methodParameterArg2");
		}

		public object? EvaluateNode(LogEventInfo logEvent)
		{
			object arg = _methodParameterArg1(logEvent);
			object arg2 = _methodParameterArg2(logEvent);
			return _method(logEvent, arg, arg2);
		}
	}

	private sealed class EvaluateMethodThreeParameters : IEvaluateMethod
	{
		private readonly Func<LogEventInfo, object?, object?, object?, object?> _method;

		private readonly Func<LogEventInfo, object?> _methodParameterArg1;

		private readonly Func<LogEventInfo, object?> _methodParameterArg2;

		private readonly Func<LogEventInfo, object?> _methodParameterArg3;

		public EvaluateMethodThreeParameters(Func<LogEventInfo, object?, object?, object?, object?> method, Func<LogEventInfo, object?> methodParameterArg1, Func<LogEventInfo, object?> methodParameterArg2, Func<LogEventInfo, object?> methodParameterArg3)
		{
			_method = Guard.ThrowIfNull(method, "method");
			_methodParameterArg1 = Guard.ThrowIfNull(methodParameterArg1, "methodParameterArg1");
			_methodParameterArg2 = Guard.ThrowIfNull(methodParameterArg2, "methodParameterArg2");
			_methodParameterArg3 = Guard.ThrowIfNull(methodParameterArg3, "methodParameterArg3");
		}

		public object? EvaluateNode(LogEventInfo logEvent)
		{
			object arg = _methodParameterArg1(logEvent);
			object arg2 = _methodParameterArg2(logEvent);
			object arg3 = _methodParameterArg3(logEvent);
			return _method(logEvent, arg, arg2, arg3);
		}
	}

	private sealed class EvaluateMethodManyParameters : IEvaluateMethod
	{
		private readonly Func<object?[], object?> _method;

		private readonly IList<ConditionExpression> _methodParameters;

		private readonly bool _includeLogEvent;

		public EvaluateMethodManyParameters(Func<object?[], object?> method, IList<ConditionExpression> inputParameters, bool includeLogEvent)
		{
			_method = Guard.ThrowIfNull(method, "method");
			_methodParameters = Guard.ThrowIfNull(inputParameters, "inputParameters");
			_includeLogEvent = includeLogEvent;
		}

		public object? EvaluateNode(LogEventInfo logEvent)
		{
			int num = (_includeLogEvent ? 1 : 0);
			object[] array = new object[_methodParameters.Count + num];
			if (_includeLogEvent)
			{
				array[0] = logEvent;
			}
			for (int i = 0; i < _methodParameters.Count; i++)
			{
				object obj = _methodParameters[i].Evaluate(logEvent);
				array[num++] = obj;
			}
			return _method(array);
		}
	}

	private readonly IEvaluateMethod _method;

	public string MethodName { get; }

	/// <summary>
	/// Gets the method parameters
	/// </summary>
	public IList<ConditionExpression> MethodParameters { get; }

	private ConditionMethodExpression(string methodName, IList<ConditionExpression> methodParameters, IEvaluateMethod method)
	{
		MethodName = Guard.ThrowIfNull(methodName, "methodName");
		_method = Guard.ThrowIfNull(method, "method");
		MethodParameters = Guard.ThrowIfNull(methodParameters, "methodParameters");
	}

	/// <inheritdoc />
	protected override object? EvaluateNode(LogEventInfo context)
	{
		return _method.EvaluateNode(context);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(MethodName);
		stringBuilder.Append('(');
		string value = string.Empty;
		foreach (ConditionExpression methodParameter in MethodParameters)
		{
			stringBuilder.Append(value);
			stringBuilder.Append(methodParameter);
			value = ", ";
		}
		stringBuilder.Append(')');
		return stringBuilder.ToString();
	}

	public static ConditionMethodExpression CreateMethodNoParameters(string conditionMethodName, Func<LogEventInfo, object?> method)
	{
		return new ConditionMethodExpression(conditionMethodName, ArrayHelper.Empty<ConditionExpression>(), new EvaluateMethodNoParameters(method));
	}

	public static ConditionMethodExpression CreateMethodNoParameters(string conditionMethodName, Func<LogEventInfo, bool> method)
	{
		return CreateMethodNoParameters(conditionMethodName, (LogEventInfo evt) => (!method(evt)) ? ConditionExpression.BoxedFalse : ConditionExpression.BoxedTrue);
	}

	public static ConditionMethodExpression CreateMethodOneParameter(string conditionMethodName, Func<LogEventInfo, object?, object?> method, IList<ConditionExpression> methodParameters)
	{
		ConditionExpression methodParameter = methodParameters[0];
		return new ConditionMethodExpression(conditionMethodName, methodParameters, new EvaluateMethodOneParameter(method, (LogEventInfo logEvent) => methodParameter.Evaluate(logEvent)));
	}

	public static ConditionMethodExpression CreateMethodTwoParameters(string conditionMethodName, Func<LogEventInfo, object?, object?, object?> method, IList<ConditionExpression> methodParameters)
	{
		ConditionExpression methodParameterArg1 = methodParameters[0];
		ConditionExpression methodParameterArg2 = methodParameters[1];
		return new ConditionMethodExpression(conditionMethodName, methodParameters, new EvaluateMethodTwoParameters(method, (LogEventInfo logEvent) => methodParameterArg1.Evaluate(logEvent), (LogEventInfo logEvent) => methodParameterArg2.Evaluate(logEvent)));
	}

	public static ConditionMethodExpression CreateMethodThreeParameters(string conditionMethodName, Func<LogEventInfo, object?, object?, object?, object?> method, IList<ConditionExpression> methodParameters)
	{
		ConditionExpression methodParameterArg1 = methodParameters[0];
		ConditionExpression methodParameterArg2 = methodParameters[1];
		ConditionExpression methodParameterArg3 = methodParameters[2];
		return new ConditionMethodExpression(conditionMethodName, methodParameters, new EvaluateMethodThreeParameters(method, (LogEventInfo logEvent) => methodParameterArg1.Evaluate(logEvent), (LogEventInfo logEvent) => methodParameterArg2.Evaluate(logEvent), (LogEventInfo logEvent) => methodParameterArg3.Evaluate(logEvent)));
	}

	public static ConditionMethodExpression CreateMethodManyParameters(string conditionMethodName, Func<object?[], object?> method, IList<ConditionExpression> methodParameters, bool includeLogEvent)
	{
		return new ConditionMethodExpression(conditionMethodName, methodParameters, new EvaluateMethodManyParameters(method, methodParameters, includeLogEvent));
	}
}
