using System.Threading;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Distributes log events to targets in a round-robin fashion.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/RoundRobinGroup-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/RoundRobinGroup-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes the messages to be written to either file1.txt or file2.txt.
/// Each odd message is written to file2.txt, each even message goes to file1.txt.
/// </p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/RoundRobinGroup/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/RoundRobinGroup/Simple/Example.cs" />
/// </example>
[Target("RoundRobinGroup", IsCompound = true)]
public class RoundRobinGroupTarget : CompoundTargetBase
{
	private int _currentTarget = -1;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RoundRobinGroupTarget" /> class.
	/// </summary>
	public RoundRobinGroupTarget()
		: this(ArrayHelper.Empty<Target>())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RoundRobinGroupTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="targets">The targets.</param>
	public RoundRobinGroupTarget(string name, params Target[] targets)
		: this(targets)
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RoundRobinGroupTarget" /> class.
	/// </summary>
	/// <param name="targets">The targets.</param>
	public RoundRobinGroupTarget(params Target[] targets)
		: base(targets)
	{
	}

	/// <summary>
	/// Ensures forwarding happens without holding lock
	/// </summary>
	/// <param name="logEvent"></param>
	protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
	{
		Write(logEvent);
	}

	/// <summary>
	/// Forwards the write to one of the targets from
	/// the <see cref="N:NLog.Targets" /> collection.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <remarks>
	/// The writes are routed in a round-robin fashion.
	/// The first log event goes to the first target, the second
	/// one goes to the second target and so on looping to the
	/// first target when there are no more targets available.
	/// In general request N goes to Targets[N % Targets.Count].
	/// </remarks>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		if (base.Targets.Count == 0)
		{
			logEvent.Continuation(null);
			return;
		}
		int index = (int)((uint)Interlocked.Increment(ref _currentTarget) % base.Targets.Count);
		base.Targets[index].WriteAsyncLogEvent(logEvent);
	}
}
