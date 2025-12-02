using System;
using System.Collections;
using System.Collections.Generic;
using NLog.Config;
using NLog.Targets;

namespace NLog.Internal;

internal sealed class SetupConfigurationTargetBuilder : ISetupConfigurationTargetBuilder, IList<Target>, ICollection<Target>, IEnumerable<Target>, IEnumerable
{
	private readonly IList<Target> _targets = new List<Target>();

	private string? _targetName;

	public LoggingConfiguration Configuration { get; }

	public LogFactory LogFactory { get; }

	public IList<Target> Targets => this;

	Target IList<Target>.this[int index]
	{
		get
		{
			return _targets[index];
		}
		set
		{
			_targets[index] = value;
		}
	}

	int ICollection<Target>.Count => _targets.Count;

	bool ICollection<Target>.IsReadOnly => _targets.IsReadOnly;

	public SetupConfigurationTargetBuilder(LogFactory logFactory, LoggingConfiguration configuration, string? targetName = null)
	{
		Configuration = configuration;
		LogFactory = logFactory;
		_targetName = (string.IsNullOrEmpty(targetName) ? null : targetName);
	}

	private void UpdateTargetName(Target item)
	{
		if (_targetName != null && !string.IsNullOrEmpty(_targetName))
		{
			item.Name = _targetName;
			_targetName = string.Empty;
		}
		else if (_targetName == string.Empty)
		{
			throw new ArgumentException("Cannot apply the same Target-Name to multiple targets");
		}
	}

	void ICollection<Target>.Add(Target item)
	{
		UpdateTargetName(item);
		_targets.Add(item);
	}

	void ICollection<Target>.Clear()
	{
		_targets.Clear();
	}

	bool ICollection<Target>.Contains(Target item)
	{
		return _targets.Contains(item);
	}

	void ICollection<Target>.CopyTo(Target[] array, int arrayIndex)
	{
		_targets.CopyTo(array, arrayIndex);
	}

	IEnumerator<Target> IEnumerable<Target>.GetEnumerator()
	{
		return _targets.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _targets.GetEnumerator();
	}

	int IList<Target>.IndexOf(Target item)
	{
		return _targets.IndexOf(item);
	}

	void IList<Target>.Insert(int index, Target item)
	{
		UpdateTargetName(item);
		_targets.Insert(index, item);
	}

	bool ICollection<Target>.Remove(Target item)
	{
		return _targets.Remove(item);
	}

	void IList<Target>.RemoveAt(int index)
	{
		_targets.RemoveAt(index);
	}
}
