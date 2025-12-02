using System;
using System.Collections.Generic;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.Targets;

/// <summary>
/// The base class for all targets which call methods (local or remote).
/// Manages parameters and type coercion.
/// </summary>
public abstract class MethodCallTargetBase : Target
{
	/// <summary>
	/// Gets the array of parameters to be passed.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(MethodCallParameter), "parameter")]
	public IList<MethodCallParameter> Parameters { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MethodCallTargetBase" /> class.
	/// </summary>
	protected MethodCallTargetBase()
	{
		Parameters = new List<MethodCallParameter>();
	}

	/// <summary>
	/// Prepares an array of parameters to be passed based on the logging event and calls DoInvoke().
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		object[] array = ((Parameters.Count > 0) ? new object[Parameters.Count] : ArrayHelper.Empty<object>());
		for (int i = 0; i < array.Length; i++)
		{
			try
			{
				object obj = Parameters[i].RenderValue(logEvent.LogEvent);
				array[i] = obj;
			}
			catch (Exception ex)
			{
				if (ex.MustBeRethrownImmediately())
				{
					throw;
				}
				InternalLogger.Warn(ex, "{0}: Failed to get parameter value {1}", this, Parameters[i].Name);
				throw;
			}
		}
		DoInvoke(array, logEvent);
	}

	/// <summary>
	/// Calls the target DoInvoke method, and handles AsyncContinuation callback
	/// </summary>
	/// <param name="parameters">Method call parameters.</param>
	/// <param name="logEvent">The logging event.</param>
	protected virtual void DoInvoke(object?[] parameters, AsyncLogEventInfo logEvent)
	{
		try
		{
			DoInvoke(parameters);
			logEvent.Continuation(null);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "DoInvoke"))
			{
				throw;
			}
			logEvent.Continuation(exception);
		}
	}

	/// <summary>
	/// Calls the target method. Must be implemented in concrete classes.
	/// </summary>
	/// <param name="parameters">Method call parameters.</param>
	protected abstract void DoInvoke(object?[] parameters);
}
