using System;
using System.Collections.Generic;
using System.Text;
using NLog.Common;

namespace NLog.Targets.Wrappers;

/// <summary>
/// A base class for targets which wrap other (multiple) targets
/// and provide various forms of target routing.
/// </summary>
public abstract class CompoundTargetBase : Target
{
	/// <summary>
	/// Gets the collection of targets managed by this compound target.
	/// </summary>
	public IList<Target> Targets { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.CompoundTargetBase" /> class.
	/// </summary>
	/// <param name="targets">The targets.</param>
	protected CompoundTargetBase(params Target[] targets)
	{
		Targets = new List<Target>(targets);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return _tostring ?? (_tostring = GenerateTargetToString());
	}

	private string GenerateTargetToString()
	{
		if (string.IsNullOrEmpty(base.Name))
		{
			IList<Target> targets = Targets;
			if (targets != null && targets.Count > 0)
			{
				string value = string.Empty;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(GenerateTargetToString(targetWrapper: true));
				stringBuilder.Append('[');
				foreach (Target target in Targets)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(target.ToString());
					value = ", ";
				}
				stringBuilder.Append(']');
				return stringBuilder.ToString();
			}
		}
		return GenerateTargetToString(targetWrapper: true);
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		throw new NotSupportedException("This target must not be invoked in a synchronous way.");
	}

	/// <summary>
	/// Flush any pending log messages for all wrapped targets.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		AsyncHelpers.ForEachItemInParallel(Targets, asyncContinuation, delegate(Target t, AsyncContinuation c)
		{
			t.Flush(c);
		});
	}
}
