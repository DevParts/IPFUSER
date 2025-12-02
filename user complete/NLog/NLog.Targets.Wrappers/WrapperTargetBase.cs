using System;
using NLog.Common;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Base class for targets wrap other (single) targets.
/// </summary>
public abstract class WrapperTargetBase : Target
{
	private Target? _wrappedTarget;

	/// <summary>
	/// Gets or sets the target that is wrapped by this target.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see langword="null" /></remarks>
	/// <docgen category="General Options" order="11" />
	public Target? WrappedTarget
	{
		get
		{
			return _wrappedTarget;
		}
		set
		{
			_wrappedTarget = value;
			_tostring = null;
		}
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return _tostring ?? (_tostring = GenerateTargetToString());
	}

	private string GenerateTargetToString()
	{
		if (WrappedTarget == null)
		{
			return GenerateTargetToString(targetWrapper: true);
		}
		if (string.IsNullOrEmpty(base.Name))
		{
			return $"{GenerateTargetToString(targetWrapper: true, string.Empty)}_{WrappedTarget}";
		}
		return GenerateTargetToString(targetWrapper: true, string.Empty) + "_" + WrappedTarget.GenerateTargetToString(targetWrapper: false, base.Name);
	}

	/// <inheritdoc />
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		if (WrappedTarget == null)
		{
			asyncContinuation(null);
		}
		else
		{
			WrappedTarget.Flush(asyncContinuation);
		}
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		if (WrappedTarget == null)
		{
			throw new NLogConfigurationException(GetType().Name + "(Name=" + base.Name + "): No wrapped Target configured.");
		}
		base.InitializeTarget();
	}

	/// <summary>
	/// Writes logging event to the log target. Must be overridden in inheriting
	/// classes.
	/// </summary>
	/// <param name="logEvent">Logging event to be written out.</param>
	protected sealed override void Write(LogEventInfo logEvent)
	{
		throw new NotSupportedException("This target must not be invoked in a synchronous way.");
	}
}
