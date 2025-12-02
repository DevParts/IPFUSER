using System;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Sends log messages to a randomly selected target.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/RandomizeGroup-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/RandomizeGroup-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes the messages to be written to either file1.txt or file2.txt
/// chosen randomly on a per-message basis.
/// </p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/RandomizeGroup/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/RandomizeGroup/Simple/Example.cs" />
/// </example>
[Target("RandomizeGroup", IsCompound = true)]
public class RandomizeGroupTarget : CompoundTargetBase
{
	private readonly Random _random = new Random();

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RandomizeGroupTarget" /> class.
	/// </summary>
	public RandomizeGroupTarget()
		: this(ArrayHelper.Empty<Target>())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RandomizeGroupTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="targets">The targets.</param>
	public RandomizeGroupTarget(string name, params Target[] targets)
		: this(targets)
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RandomizeGroupTarget" /> class.
	/// </summary>
	/// <param name="targets">The targets.</param>
	public RandomizeGroupTarget(params Target[] targets)
		: base(targets)
	{
	}

	/// <summary>
	/// Forwards the log event to one of the sub-targets.
	/// The sub-target is randomly chosen.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		if (base.Targets.Count == 0)
		{
			logEvent.Continuation(null);
			return;
		}
		int index;
		lock (_random)
		{
			index = _random.Next(base.Targets.Count);
		}
		base.Targets[index].WriteAsyncLogEvent(logEvent);
	}
}
