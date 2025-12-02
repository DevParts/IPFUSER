using System;
using System.Collections.Generic;

namespace NLog.Internal;

internal struct ScopeContextNestedStateCollector
{
	private IList<object> _allNestedStates;

	private List<object> _nestedStateCollector;

	public bool IsCollectorEmpty
	{
		get
		{
			if (_allNestedStates != null)
			{
				if (_allNestedStates.Count == 0)
				{
					return _nestedStateCollector == null;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsCollectorInactive => _allNestedStates == null;

	public IList<object> StartCaptureNestedStates(IScopeContextAsyncState? state)
	{
		_allNestedStates = _allNestedStates ?? Array.Empty<object>();
		while (state != null)
		{
			IList<object> list = state.CaptureNestedContext(ref this);
			if (list != null)
			{
				return list;
			}
			state = state.Parent;
		}
		return _allNestedStates;
	}

	public void PushNestedState(object state)
	{
		if (_nestedStateCollector == null)
		{
			_nestedStateCollector = new List<object>(Math.Max(4, _allNestedStates?.Count ?? 1));
			IList<object> allNestedStates = _allNestedStates;
			if (allNestedStates != null && allNestedStates.Count > 0)
			{
				for (int i = 0; i < _allNestedStates.Count; i++)
				{
					_nestedStateCollector.Add(_allNestedStates[i]);
				}
			}
			_allNestedStates = _nestedStateCollector;
		}
		_nestedStateCollector.Add(state);
	}
}
