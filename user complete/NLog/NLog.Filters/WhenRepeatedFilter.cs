using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NLog.Internal;

namespace NLog.Filters;

/// <summary>
/// Matches when the result of the calculated layout has been repeated a moment ago
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/WhenRepeated-Filter">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/WhenRepeated-Filter">Documentation on NLog Wiki</seealso>
[Filter("whenRepeated")]
public class WhenRepeatedFilter : LayoutBasedFilter
{
	/// <summary>
	/// Filter Value State (mutable)
	/// </summary>
	private sealed class FilterInfo
	{
		public StringBuilder StringBuffer { get; }

		public LogLevel? LogLevel { get; private set; }

		private DateTime LastLogTime { get; set; }

		private DateTime LastFilterTime { get; set; }

		public int FilterCount { get; private set; }

		public FilterInfo(StringBuilder stringBuilder)
		{
			StringBuffer = stringBuilder;
		}

		public void Refresh(LogLevel logLevel, DateTime logTimeStamp, int filterCount)
		{
			if (filterCount == 0)
			{
				LastLogTime = logTimeStamp;
				LogLevel = logLevel;
			}
			else if ((object)LogLevel == null || logLevel.Ordinal > LogLevel.Ordinal)
			{
				LogLevel = logLevel;
			}
			LastFilterTime = logTimeStamp;
			FilterCount = filterCount;
		}

		public bool IsObsolete(DateTime logEventTime, int timeoutSeconds)
		{
			if (FilterCount == 0)
			{
				return HasExpired(logEventTime, timeoutSeconds);
			}
			if ((logEventTime - LastFilterTime).TotalSeconds > (double)timeoutSeconds)
			{
				return HasExpired(logEventTime, timeoutSeconds * 2);
			}
			return false;
		}

		public bool HasExpired(DateTime logEventTime, int timeoutSeconds)
		{
			return (logEventTime - LastLogTime).TotalSeconds > (double)timeoutSeconds;
		}
	}

	/// <summary>
	/// Filter Lookup Key (immutable)
	/// </summary>
	private struct FilterInfoKey : IEquatable<FilterInfoKey>
	{
		private readonly StringBuilder? _stringBuffer;

		public readonly string? StringValue;

		public readonly int StringHashCode;

		public FilterInfoKey(StringBuilder? stringBuffer, string? stringValue, int? stringHashCode = null)
		{
			_stringBuffer = stringBuffer;
			StringValue = stringValue;
			if (stringHashCode.HasValue)
			{
				StringHashCode = stringHashCode.Value;
			}
			else if (stringBuffer != null)
			{
				int num = stringBuffer.Length.GetHashCode();
				int num2 = Math.Min(stringBuffer.Length, 100);
				for (int i = 0; i < num2; i++)
				{
					num ^= stringBuffer[i].GetHashCode();
				}
				StringHashCode = num;
			}
			else
			{
				StringHashCode = StringComparer.Ordinal.GetHashCode(StringValue);
			}
		}

		public override int GetHashCode()
		{
			return StringHashCode;
		}

		public bool Equals(FilterInfoKey other)
		{
			if (StringValue != null)
			{
				return string.Equals(StringValue, other.StringValue, StringComparison.Ordinal);
			}
			if (_stringBuffer != null && other._stringBuffer != null)
			{
				if (_stringBuffer.Capacity != other._stringBuffer.Capacity)
				{
					return _stringBuffer.EqualTo(other._stringBuffer);
				}
				return _stringBuffer.Equals(other._stringBuffer);
			}
			if (_stringBuffer == other._stringBuffer)
			{
				return (object)StringValue == other.StringValue;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is FilterInfoKey other)
			{
				return Equals(other);
			}
			return false;
		}
	}

	private const int MaxInitialRenderBufferLength = 16384;

	private bool? _optimizeBufferReuse;

	private readonly ReusableBuilderCreator ReusableLayoutBuilder = new ReusableBuilderCreator();

	private readonly Dictionary<FilterInfoKey, FilterInfo> _repeatFilter = new Dictionary<FilterInfoKey, FilterInfo>(1000);

	private readonly Stack<KeyValuePair<FilterInfoKey, FilterInfo>> _objectPool = new Stack<KeyValuePair<FilterInfoKey, FilterInfo>>(1000);

	/// <summary>
	/// How long before a filter expires, and logging is accepted again
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public int TimeoutSeconds { get; set; } = 10;

	/// <summary>
	/// Max length of filter values, will truncate if above limit
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public int MaxLength { get; set; } = 1000;

	/// <summary>
	/// Applies the configured action to the initial logevent that starts the timeout period.
	/// Used to configure that it should ignore all events until timeout.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public bool IncludeFirst { get; set; }

	/// <summary>
	/// Max number of unique filter values to expect simultaneously
	/// </summary>
	/// <docgen category="Filtering Options" order="100" />
	public int MaxFilterCacheSize { get; set; } = 50000;

	/// <summary>
	/// Default number of unique filter values to expect, will automatically increase if needed
	/// </summary>
	/// <docgen category="Filtering Options" order="100" />
	public int DefaultFilterCacheSize { get; set; } = 1000;

	/// <summary>
	/// Insert FilterCount value into <see cref="P:NLog.LogEventInfo.Properties" /> when an event is no longer filtered
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public string FilterCountPropertyName { get; set; } = string.Empty;

	/// <summary>
	/// Append FilterCount to the <see cref="P:NLog.LogEventInfo.Message" /> when an event is no longer filtered
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public string FilterCountMessageAppendFormat { get; set; } = string.Empty;

	/// <summary>
	/// Reuse internal buffers, and doesn't have to constantly allocate new buffers
	/// </summary>
	/// <docgen category="Filtering Options" order="100" />
	[Obsolete("No longer used, and always returns true. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool OptimizeBufferReuse
	{
		get
		{
			return _optimizeBufferReuse ?? true;
		}
		set
		{
			_optimizeBufferReuse = (value ? new bool?(true) : ((bool?)null));
		}
	}

	/// <summary>
	/// Default buffer size for the internal buffers
	/// </summary>
	/// <docgen category="Filtering Options" order="100" />
	public int OptimizeBufferDefaultLength { get; set; } = 1000;

	/// <summary>
	/// Checks whether log event should be logged or not. In case the LogEvent has just been repeated.
	/// </summary>
	/// <param name="logEvent">Log event.</param>
	/// <returns>
	/// <see cref="F:NLog.Filters.FilterResult.Ignore" /> - if the log event should be ignored<br />
	/// <see cref="F:NLog.Filters.FilterResult.Neutral" /> - if the filter doesn't want to decide<br />
	/// <see cref="F:NLog.Filters.FilterResult.Log" /> - if the log event should be logged<br />
	/// .</returns>
	protected override FilterResult Check(LogEventInfo logEvent)
	{
		FilterResult result = FilterResult.Neutral;
		bool flag = false;
		lock (_repeatFilter)
		{
			ReusableObjectCreator<StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
			try
			{
				if (lockOject.Result.Capacity != OptimizeBufferDefaultLength)
				{
					if (OptimizeBufferDefaultLength < 16384)
					{
						OptimizeBufferDefaultLength = MaxLength;
						while (OptimizeBufferDefaultLength < lockOject.Result.Capacity && OptimizeBufferDefaultLength < 16384)
						{
							OptimizeBufferDefaultLength *= 2;
						}
					}
					lockOject.Result.Capacity = OptimizeBufferDefaultLength;
				}
				FilterInfoKey key = RenderFilterInfoKey(logEvent, lockOject.Result);
				if (!_repeatFilter.TryGetValue(key, out FilterInfo value))
				{
					value = CreateFilterInfo(logEvent);
					if (value.StringBuffer != null)
					{
						value.StringBuffer.ClearBuilder();
						int num = Math.Min(lockOject.Result.Length, MaxLength);
						for (int i = 0; i < num; i++)
						{
							value.StringBuffer.Append(lockOject.Result[i]);
						}
					}
					value.Refresh(logEvent.Level, logEvent.TimeStamp, 0);
					_repeatFilter.Add(new FilterInfoKey(value.StringBuffer, key.StringValue, key.StringHashCode), value);
					flag = true;
				}
				else
				{
					if (IncludeFirst)
					{
						flag = value.IsObsolete(logEvent.TimeStamp, TimeoutSeconds);
					}
					result = RefreshFilterInfo(logEvent, value);
				}
			}
			finally
			{
				((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
			}
		}
		if (IncludeFirst && flag)
		{
			result = base.Action;
		}
		return result;
	}

	/// <summary>
	/// Uses object pooling, and prunes stale filter items when the pool runs dry
	/// </summary>
	private FilterInfo CreateFilterInfo(LogEventInfo logEvent)
	{
		if (_objectPool.Count == 0 && _repeatFilter.Count > DefaultFilterCacheSize)
		{
			int val = ((_repeatFilter.Count > MaxFilterCacheSize) ? (TimeoutSeconds * 2 / 3) : TimeoutSeconds);
			PruneFilterCache(logEvent, Math.Max(1, val));
			if (_repeatFilter.Count > MaxFilterCacheSize)
			{
				PruneFilterCache(logEvent, Math.Max(1, TimeoutSeconds / 2));
			}
		}
		FilterInfo filterInfo;
		if (_objectPool.Count == 0)
		{
			filterInfo = new FilterInfo(new StringBuilder(OptimizeBufferDefaultLength));
		}
		else
		{
			filterInfo = _objectPool.Pop().Value;
			if (filterInfo.StringBuffer != null && filterInfo.StringBuffer.Capacity != OptimizeBufferDefaultLength)
			{
				filterInfo.StringBuffer.Capacity = OptimizeBufferDefaultLength;
			}
		}
		return filterInfo;
	}

	/// <summary>
	/// Remove stale filter-value from the cache, and fill them into the pool for reuse
	/// </summary>
	private void PruneFilterCache(LogEventInfo logEvent, int aggressiveTimeoutSeconds)
	{
		foreach (KeyValuePair<FilterInfoKey, FilterInfo> item in _repeatFilter)
		{
			if (item.Value.IsObsolete(logEvent.TimeStamp, aggressiveTimeoutSeconds))
			{
				_objectPool.Push(item);
			}
		}
		foreach (KeyValuePair<FilterInfoKey, FilterInfo> item2 in _objectPool)
		{
			_repeatFilter.Remove(item2.Key);
		}
		if (_repeatFilter.Count * 2 > DefaultFilterCacheSize && DefaultFilterCacheSize < MaxFilterCacheSize)
		{
			DefaultFilterCacheSize *= 2;
		}
		while (_objectPool.Count != 0 && _objectPool.Count > DefaultFilterCacheSize)
		{
			_objectPool.Pop();
		}
	}

	/// <summary>
	/// Renders the Log Event into a filter value, that is used for checking if just repeated
	/// </summary>
	private FilterInfoKey RenderFilterInfoKey(LogEventInfo logEvent, StringBuilder targetBuilder)
	{
		if (targetBuilder != null)
		{
			base.Layout.Render(logEvent, targetBuilder);
			if (targetBuilder.Length > MaxLength)
			{
				targetBuilder.Length = MaxLength;
			}
			return new FilterInfoKey(targetBuilder, null);
		}
		string text = base.Layout.Render(logEvent);
		if (text.Length > MaxLength)
		{
			text = text.Substring(0, MaxLength);
		}
		return new FilterInfoKey(null, text);
	}

	/// <summary>
	/// Repeated LogEvent detected. Checks if it should activate filter-action
	/// </summary>
	private FilterResult RefreshFilterInfo(LogEventInfo logEvent, FilterInfo filterInfo)
	{
		if (filterInfo.HasExpired(logEvent.TimeStamp, TimeoutSeconds) || logEvent.Level > filterInfo.LogLevel)
		{
			int num = filterInfo.FilterCount;
			if (num > 0 && filterInfo.IsObsolete(logEvent.TimeStamp, TimeoutSeconds))
			{
				num = 0;
			}
			filterInfo.Refresh(logEvent.Level, logEvent.TimeStamp, 0);
			if (num > 0)
			{
				if (!string.IsNullOrEmpty(FilterCountPropertyName))
				{
					if (!logEvent.Properties.TryGetValue(FilterCountPropertyName, out object value))
					{
						logEvent.Properties[FilterCountPropertyName] = num;
					}
					else if (value is int val)
					{
						num = Math.Max(val, num);
						logEvent.Properties[FilterCountPropertyName] = num;
					}
				}
				if (!string.IsNullOrEmpty(FilterCountMessageAppendFormat) && logEvent.Message != null)
				{
					logEvent.Message += string.Format(FilterCountMessageAppendFormat, num.ToString(CultureInfo.InvariantCulture));
				}
			}
			return FilterResult.Neutral;
		}
		filterInfo.Refresh(logEvent.Level, logEvent.TimeStamp, filterInfo.FilterCount + 1);
		return base.Action;
	}
}
