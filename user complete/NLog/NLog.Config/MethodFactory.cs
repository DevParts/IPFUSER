using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NLog.Conditions;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Factory for locating methods.
/// </summary>
internal sealed class MethodFactory : IFactory
{
	private struct MethodDetails
	{
		public readonly MethodInfo MethodInfo;

		public readonly Func<LogEventInfo, object?> NoParameters;

		public readonly Func<LogEventInfo, object?, object?> OneParameter;

		public readonly Func<LogEventInfo, object?, object?, object?> TwoParameters;

		public readonly Func<LogEventInfo, object?, object?, object?, object?> ThreeParameters;

		public readonly Func<object?[], object?> ManyParameters;

		public readonly int ManyParameterMinCount;

		public readonly int ManyParameterMaxCount;

		public readonly bool ManyParameterWithLogEvent;

		public MethodDetails(MethodInfo methodInfo, Func<LogEventInfo, object?> noParameters, Func<LogEventInfo, object?, object?> oneParameter, Func<LogEventInfo, object?, object?, object?> twoParameters, Func<LogEventInfo, object?, object?, object?, object?> threeParameters, Func<object?[], object?> manyParameters, int manyParameterMinCount, int manyParameterMaxCount, bool manyParameterWithLogEvent)
		{
			MethodInfo = methodInfo;
			NoParameters = noParameters;
			OneParameter = oneParameter;
			TwoParameters = twoParameters;
			ThreeParameters = threeParameters;
			ManyParameters = manyParameters;
			ManyParameterMinCount = manyParameterMinCount;
			ManyParameterMaxCount = manyParameterMaxCount;
			ManyParameterWithLogEvent = manyParameterWithLogEvent;
		}
	}

	private readonly Dictionary<string, MethodDetails> _nameToMethodDetails = new Dictionary<string, MethodDetails>(StringComparer.OrdinalIgnoreCase);

	public bool Initialized { get; private set; }

	public void Initialize(Action<bool> itemRegistration)
	{
		lock (ConfigurationItemFactory.SyncRoot)
		{
			if (Initialized)
			{
				return;
			}
			try
			{
				bool obj = _nameToMethodDetails.Count == 0;
				itemRegistration(obj);
			}
			finally
			{
				Initialized = true;
			}
		}
	}

	public bool CheckTypeAliasExists(string typeAlias)
	{
		return _nameToMethodDetails.ContainsKey(typeAlias);
	}

	/// <summary>
	/// Registers the type.
	/// </summary>
	/// <param name="type">The type to register.</param>
	/// <param name="itemNamePrefix">The item name prefix.</param>
	void IFactory.RegisterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicProperties)] Type type, string itemNamePrefix)
	{
		if (!type.IsClass)
		{
			return;
		}
		IList<KeyValuePair<string, MethodInfo>> list = ExtractClassMethods<ConditionMethodsAttribute, ConditionMethodAttribute>(type);
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				string methodName = (string.IsNullOrEmpty(itemNamePrefix) ? list[i].Key : (itemNamePrefix + list[i].Key));
				RegisterDefinition(methodName, list[i].Value);
			}
		}
	}

	/// <summary>
	/// Scans a type for relevant methods with their symbolic names
	/// </summary>
	/// <typeparam name="TClassAttributeType">Include types that are marked with this attribute</typeparam>
	/// <typeparam name="TMethodAttributeType">Include methods that are marked with this attribute</typeparam>
	/// <param name="type">Class Type to scan</param>
	/// <returns>Collection of methods with their symbolic names</returns>
	private static IList<KeyValuePair<string, MethodInfo>> ExtractClassMethods<TClassAttributeType, TMethodAttributeType>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type type) where TClassAttributeType : Attribute where TMethodAttributeType : NameBaseAttribute
	{
		if (!type.IsDefined(typeof(TClassAttributeType), inherit: false))
		{
			return ArrayHelper.Empty<KeyValuePair<string, MethodInfo>>();
		}
		List<KeyValuePair<string, MethodInfo>> list = new List<KeyValuePair<string, MethodInfo>>();
		MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
		foreach (MethodInfo methodInfo in methods)
		{
			TMethodAttributeType[] array = (TMethodAttributeType[])methodInfo.GetCustomAttributes(typeof(TMethodAttributeType), inherit: false);
			foreach (TMethodAttributeType val in array)
			{
				list.Add(new KeyValuePair<string, MethodInfo>(val.Name, methodInfo));
			}
		}
		return list;
	}

	/// <summary>
	/// Clears contents of the factory.
	/// </summary>
	public void Clear()
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.Clear();
		}
	}

	internal void RegisterDefinition(string methodName, MethodInfo methodInfo)
	{
		int manyParameterMinCount;
		int manyParameterMaxCount;
		bool includeLogEvent;
		object[] defaultMethodParameters = ResolveDefaultMethodParameters(methodInfo, out manyParameterMinCount, out manyParameterMaxCount, out includeLogEvent);
		if (manyParameterMaxCount > 0)
		{
			RegisterManyParameters(methodName, (object?[] inputArgs) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, inputArgs)), manyParameterMinCount, manyParameterMaxCount, includeLogEvent, methodInfo);
		}
		if (manyParameterMinCount == 0)
		{
			if (!includeLogEvent)
			{
				RegisterNoParameters(methodName, (LogEventInfo logEvent) => InvokeMethodInfo(methodInfo, defaultMethodParameters), methodInfo);
			}
			else
			{
				RegisterNoParameters(methodName, (LogEventInfo logEvent) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, logEvent)), methodInfo);
			}
		}
		if (manyParameterMinCount <= 1 && manyParameterMaxCount >= 1)
		{
			if (!includeLogEvent)
			{
				RegisterOneParameter(methodName, (LogEventInfo logEvent, object? arg1) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, arg1)), methodInfo);
			}
			else
			{
				RegisterOneParameter(methodName, (LogEventInfo logEvent, object? arg1) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, logEvent, arg1)), methodInfo);
			}
		}
		if (manyParameterMinCount <= 2 && manyParameterMaxCount >= 2)
		{
			if (!includeLogEvent)
			{
				RegisterTwoParameters(methodName, (LogEventInfo logEvent, object? arg1, object? arg2) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, arg1, arg2)), methodInfo);
			}
			else
			{
				RegisterTwoParameters(methodName, (LogEventInfo logEvent, object? arg1, object? arg2) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, logEvent, arg1, arg2)), methodInfo);
			}
		}
		if (manyParameterMinCount > 3 || manyParameterMaxCount < 3)
		{
			return;
		}
		if (!includeLogEvent)
		{
			RegisterThreeParameters(methodName, (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, arg1, arg2, arg3)), methodInfo);
		}
		else
		{
			RegisterThreeParameters(methodName, (LogEventInfo logEvent, object? arg1, object? arg2, object? arg3) => InvokeMethodInfo(methodInfo, ResolveMethodParameters(defaultMethodParameters, logEvent, arg1, arg2, arg3)), methodInfo);
		}
	}

	private static object InvokeMethodInfo(MethodInfo methodInfo, object?[] methodArgs)
	{
		try
		{
			return methodInfo.Invoke(null, methodArgs);
		}
		catch (TargetInvocationException ex)
		{
			if (ex.InnerException == null)
			{
				throw;
			}
			throw ex.InnerException;
		}
	}

	private static object[] ResolveDefaultMethodParameters(MethodInfo methodInfo, out int manyParameterMinCount, out int manyParameterMaxCount, out bool includeLogEvent)
	{
		ParameterInfo[] parameters = methodInfo.GetParameters();
		manyParameterMinCount = 0;
		manyParameterMaxCount = parameters.Length;
		object[] array = new object[parameters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (parameters[i].IsOptional)
			{
				array[i] = parameters[i].DefaultValue;
			}
			else
			{
				manyParameterMinCount++;
			}
		}
		includeLogEvent = parameters.Length != 0 && parameters[0].ParameterType == typeof(LogEventInfo);
		if (includeLogEvent)
		{
			manyParameterMaxCount--;
			if (manyParameterMinCount > 0)
			{
				manyParameterMinCount--;
			}
		}
		return array;
	}

	private static object?[] ResolveMethodParameters(object?[] defaultMethodParameters, object?[] inputParameters)
	{
		if (defaultMethodParameters.Length == inputParameters.Length)
		{
			return inputParameters;
		}
		object[] array = new object[defaultMethodParameters.Length];
		for (int i = 0; i < inputParameters.Length; i++)
		{
			array[i] = inputParameters[i];
		}
		for (int j = inputParameters.Length; j < defaultMethodParameters.Length; j++)
		{
			array[j] = defaultMethodParameters[j];
		}
		return array;
	}

	private static object?[] ResolveMethodParameters(object?[] defaultMethodParameters, object? inputParameterArg1)
	{
		object[] array = new object[defaultMethodParameters.Length];
		array[0] = inputParameterArg1;
		for (int i = 1; i < defaultMethodParameters.Length; i++)
		{
			array[i] = defaultMethodParameters[i];
		}
		return array;
	}

	private static object?[] ResolveMethodParameters(object?[] defaultMethodParameters, object? inputParameterArg1, object? inputParameterArg2)
	{
		object[] array = new object[defaultMethodParameters.Length];
		array[0] = inputParameterArg1;
		array[1] = inputParameterArg2;
		for (int i = 2; i < defaultMethodParameters.Length; i++)
		{
			array[i] = defaultMethodParameters[i];
		}
		return array;
	}

	private static object?[] ResolveMethodParameters(object?[] defaultMethodParameters, object? inputParameterArg1, object? inputParameterArg2, object? inputParameterArg3)
	{
		object[] array = new object[defaultMethodParameters.Length];
		array[0] = inputParameterArg1;
		array[1] = inputParameterArg2;
		array[2] = inputParameterArg3;
		for (int i = 3; i < defaultMethodParameters.Length; i++)
		{
			array[i] = defaultMethodParameters[i];
		}
		return array;
	}

	private static object?[] ResolveMethodParameters(object?[] defaultMethodParameters, object? inputParameterArg1, object? inputParameterArg2, object? inputParameterArg3, object? inputParameterArg4)
	{
		object[] array = new object[defaultMethodParameters.Length];
		array[0] = inputParameterArg1;
		array[1] = inputParameterArg2;
		array[2] = inputParameterArg3;
		array[3] = inputParameterArg4;
		for (int i = 4; i < defaultMethodParameters.Length; i++)
		{
			array[i] = defaultMethodParameters[i];
		}
		return array;
	}

	public void RegisterNoParameters(string methodName, Func<LogEventInfo, object?> noParameters, MethodInfo? legacyMethodInfo = null)
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.TryGetValue(methodName, out var value);
			legacyMethodInfo = legacyMethodInfo ?? value.MethodInfo ?? noParameters.Method;
			_nameToMethodDetails[methodName] = new MethodDetails(legacyMethodInfo, noParameters, value.OneParameter, value.TwoParameters, value.ThreeParameters, value.ManyParameters, value.ManyParameterMinCount, value.ManyParameterMaxCount, value.ManyParameterWithLogEvent);
		}
	}

	public void RegisterOneParameter(string methodName, Func<LogEventInfo, object?, object?> oneParameter, MethodInfo? legacyMethodInfo = null)
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.TryGetValue(methodName, out var value);
			legacyMethodInfo = legacyMethodInfo ?? value.MethodInfo ?? oneParameter.Method;
			_nameToMethodDetails[methodName] = new MethodDetails(legacyMethodInfo, value.NoParameters, oneParameter, value.TwoParameters, value.ThreeParameters, value.ManyParameters, value.ManyParameterMinCount, value.ManyParameterMaxCount, value.ManyParameterWithLogEvent);
		}
	}

	public void RegisterTwoParameters(string methodName, Func<LogEventInfo, object?, object?, object?> twoParameters, MethodInfo? legacyMethodInfo = null)
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.TryGetValue(methodName, out var value);
			legacyMethodInfo = legacyMethodInfo ?? value.MethodInfo ?? twoParameters.Method;
			_nameToMethodDetails[methodName] = new MethodDetails(legacyMethodInfo, value.NoParameters, value.OneParameter, twoParameters, value.ThreeParameters, value.ManyParameters, value.ManyParameterMinCount, value.ManyParameterMaxCount, value.ManyParameterWithLogEvent);
		}
	}

	public void RegisterThreeParameters(string methodName, Func<LogEventInfo, object?, object?, object?, object?> threeParameters, MethodInfo? legacyMethodInfo = null)
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.TryGetValue(methodName, out var value);
			legacyMethodInfo = legacyMethodInfo ?? value.MethodInfo ?? threeParameters.Method;
			_nameToMethodDetails[methodName] = new MethodDetails(legacyMethodInfo, value.NoParameters, value.OneParameter, value.TwoParameters, threeParameters, value.ManyParameters, value.ManyParameterMinCount, value.ManyParameterMaxCount, value.ManyParameterWithLogEvent);
		}
	}

	public void RegisterManyParameters(string methodName, Func<object?[], object?> manyParameters, int manyParameterMinCount, int manyParameterMaxCount, bool manyParameterWithLogEvent, MethodInfo? legacyMethodInfo = null)
	{
		lock (_nameToMethodDetails)
		{
			_nameToMethodDetails.TryGetValue(methodName, out var value);
			legacyMethodInfo = legacyMethodInfo ?? value.MethodInfo ?? manyParameters.Method;
			_nameToMethodDetails[methodName] = new MethodDetails(legacyMethodInfo, value.NoParameters, value.OneParameter, value.TwoParameters, value.ThreeParameters, manyParameters, manyParameterMinCount, manyParameterMaxCount, manyParameterWithLogEvent);
		}
	}

	public Func<LogEventInfo, object?>? TryCreateInstanceWithNoParameters(string methodName)
	{
		lock (_nameToMethodDetails)
		{
			if (_nameToMethodDetails.TryGetValue(methodName, out var value))
			{
				return value.NoParameters;
			}
			return null;
		}
	}

	public Func<LogEventInfo, object?, object?>? TryCreateInstanceWithOneParameter(string methodName)
	{
		lock (_nameToMethodDetails)
		{
			if (_nameToMethodDetails.TryGetValue(methodName, out var value))
			{
				return value.OneParameter;
			}
			return null;
		}
	}

	public Func<LogEventInfo, object?, object?, object?>? TryCreateInstanceWithTwoParameters(string methodName)
	{
		lock (_nameToMethodDetails)
		{
			if (_nameToMethodDetails.TryGetValue(methodName, out var value))
			{
				return value.TwoParameters;
			}
			return null;
		}
	}

	public Func<LogEventInfo, object?, object?, object?, object?>? TryCreateInstanceWithThreeParameters(string methodName)
	{
		lock (_nameToMethodDetails)
		{
			if (_nameToMethodDetails.TryGetValue(methodName, out var value))
			{
				return value.ThreeParameters;
			}
			return null;
		}
	}

	public Func<object?[], object?>? TryCreateInstanceWithManyParameters(string methodName, out int manyParameterMinCount, out int manyParameterMaxCount, out bool manyParameterWithLogEvent)
	{
		lock (_nameToMethodDetails)
		{
			if (_nameToMethodDetails.TryGetValue(methodName, out var methodDetails))
			{
				if (methodDetails.ManyParameters != null)
				{
					manyParameterMaxCount = methodDetails.ManyParameterMaxCount;
					manyParameterMinCount = methodDetails.ManyParameterMinCount;
					manyParameterWithLogEvent = methodDetails.ManyParameterWithLogEvent;
					return methodDetails.ManyParameters;
				}
				if (methodDetails.ThreeParameters != null)
				{
					manyParameterMaxCount = 3;
					manyParameterMinCount = ((methodDetails.TwoParameters == null) ? 3 : 2);
					manyParameterWithLogEvent = true;
					return (object?[] args) => methodDetails.ThreeParameters((LogEventInfo)(args[0] ?? throw new ArgumentNullException("LogEventInfo")), args[1], args[2], args[3]);
				}
				if (methodDetails.TwoParameters != null)
				{
					manyParameterMaxCount = 2;
					manyParameterMinCount = ((methodDetails.OneParameter != null) ? 1 : 2);
					manyParameterWithLogEvent = true;
					return (object?[] args) => methodDetails.TwoParameters((LogEventInfo)(args[0] ?? throw new ArgumentNullException("LogEventInfo")), args[1], args[2]);
				}
				if (methodDetails.OneParameter != null)
				{
					manyParameterMaxCount = 1;
					manyParameterMinCount = ((methodDetails.NoParameters == null) ? 1 : 0);
					manyParameterWithLogEvent = true;
					return (object?[] args) => methodDetails.OneParameter((LogEventInfo)(args[0] ?? throw new ArgumentNullException("LogEventInfo")), args[1]);
				}
				if (methodDetails.NoParameters != null)
				{
					manyParameterMaxCount = 0;
					manyParameterMinCount = 0;
					manyParameterWithLogEvent = true;
					return (object?[] args) => methodDetails.NoParameters((LogEventInfo)(args[0] ?? throw new ArgumentNullException("LogEventInfo")));
				}
			}
			manyParameterMinCount = 0;
			manyParameterMaxCount = 0;
			manyParameterWithLogEvent = false;
			return null;
		}
	}
}
