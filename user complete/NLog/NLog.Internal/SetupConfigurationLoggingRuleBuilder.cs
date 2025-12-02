using System.Collections;
using System.Collections.Generic;
using NLog.Config;
using NLog.Targets;

namespace NLog.Internal;

internal sealed class SetupConfigurationLoggingRuleBuilder : ISetupConfigurationLoggingRuleBuilder, ISetupConfigurationTargetBuilder, IList<Target>, ICollection<Target>, IEnumerable<Target>, IEnumerable
{
	/// <inheritdoc />
	public LoggingRule LoggingRule { get; }

	/// <inheritdoc />
	public LoggingConfiguration Configuration { get; }

	/// <inheritdoc />
	public LogFactory LogFactory { get; }

	/// <summary>
	/// Collection of targets that should be written to
	/// </summary>
	public IList<Target> Targets => this;

	Target IList<Target>.this[int index]
	{
		get
		{
			return LoggingRule.Targets[index];
		}
		set
		{
			LoggingRule.Targets[index] = value;
		}
	}

	int ICollection<Target>.Count => LoggingRule.Targets.Count;

	bool ICollection<Target>.IsReadOnly => LoggingRule.Targets.IsReadOnly;

	public SetupConfigurationLoggingRuleBuilder(LogFactory logFactory, LoggingConfiguration configuration, string? loggerNamePattern = null, string? ruleName = null)
	{
		LoggingRule = new LoggingRule(ruleName)
		{
			LoggerNamePattern = (loggerNamePattern ?? "*")
		};
		Configuration = configuration;
		LogFactory = logFactory;
	}

	void ICollection<Target>.Add(Target item)
	{
		if (!Configuration.LoggingRules.Contains(LoggingRule))
		{
			Configuration.LoggingRules.Add(LoggingRule);
		}
		LoggingRule.Targets.Add(item);
	}

	void ICollection<Target>.Clear()
	{
		LoggingRule.Targets.Clear();
	}

	bool ICollection<Target>.Contains(Target item)
	{
		return LoggingRule.Targets.Contains(item);
	}

	void ICollection<Target>.CopyTo(Target[] array, int arrayIndex)
	{
		LoggingRule.Targets.CopyTo(array, arrayIndex);
	}

	IEnumerator<Target> IEnumerable<Target>.GetEnumerator()
	{
		return LoggingRule.Targets.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return LoggingRule.Targets.GetEnumerator();
	}

	int IList<Target>.IndexOf(Target item)
	{
		return LoggingRule.Targets.IndexOf(item);
	}

	void IList<Target>.Insert(int index, Target item)
	{
		LoggingRule.Targets.Insert(index, item);
	}

	bool ICollection<Target>.Remove(Target item)
	{
		return LoggingRule.Targets.Remove(item);
	}

	void IList<Target>.RemoveAt(int index)
	{
		LoggingRule.Targets.RemoveAt(index);
	}
}
