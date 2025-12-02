using System.Collections;
using System.Collections.Generic;

namespace NLog.Targets;

/// <summary>
/// Writes log messages to <see cref="P:NLog.Targets.MemoryTarget.Logs" /> in memory for programmatic retrieval.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/Memory-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/Memory-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/Memory/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/Memory/Simple/Example.cs" />
/// </example>
[Target("Memory")]
public sealed class MemoryTarget : TargetWithLayoutHeaderAndFooter
{
	private sealed class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly List<T> _list = new List<T>();

		public T this[int index]
		{
			get
			{
				lock (_list)
				{
					return _list[index];
				}
			}
			set
			{
				lock (_list)
				{
					_list[index] = value;
				}
			}
		}

		public int Count => _list.Count;

		bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

		public void Add(T item)
		{
			lock (_list)
			{
				_list.Add(item);
			}
		}

		public void Add(T item, int maxListCount)
		{
			lock (_list)
			{
				if (maxListCount > 0 && _list.Count >= maxListCount)
				{
					_list.RemoveAt(0);
				}
				_list.Add(item);
			}
		}

		void ICollection<T>.Clear()
		{
			lock (_list)
			{
				_list.Clear();
			}
		}

		bool ICollection<T>.Contains(T item)
		{
			lock (_list)
			{
				return _list.Contains(item);
			}
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			lock (_list)
			{
				_list.CopyTo(array, arrayIndex);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			lock (_list)
			{
				foreach (T item in _list)
				{
					yield return item;
				}
			}
		}

		public int IndexOf(T item)
		{
			lock (_list)
			{
				return _list.IndexOf(item);
			}
		}

		public void Insert(int index, T item)
		{
			lock (_list)
			{
				_list.Insert(index, item);
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			lock (_list)
			{
				return _list.Remove(item);
			}
		}

		public void RemoveAt(int index)
		{
			lock (_list)
			{
				_list.RemoveAt(index);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (_list)
			{
				foreach (T item in _list)
				{
					yield return item;
				}
			}
		}
	}

	private readonly ThreadSafeList<string> _logs = new ThreadSafeList<string>();

	/// <summary>
	/// Gets the list of logs gathered in the <see cref="T:NLog.Targets.MemoryTarget" />.
	/// </summary>
	/// <remarks>
	/// Be careful when enumerating, as NLog target is blocked from writing during enumeration (blocks application logging)
	/// </remarks>
	public IList<string> Logs => _logs;

	/// <summary>
	/// Gets or sets the max number of items to have in memory. Zero or Negative means no limit.
	/// </summary>
	/// <remarks>Default: <c>0</c></remarks>
	/// <docgen category="Buffering Options" order="10" />
	public int MaxLogsCount { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MemoryTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public MemoryTarget()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MemoryTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public MemoryTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (base.Header != null)
		{
			_logs.Add(RenderLogEvent(base.Header, LogEventInfo.CreateNullEvent()));
		}
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		if (base.Footer != null)
		{
			_logs.Add(RenderLogEvent(base.Footer, LogEventInfo.CreateNullEvent()));
		}
		base.CloseTarget();
	}

	/// <summary>
	/// Renders the logging event message and adds to <see cref="P:NLog.Targets.MemoryTarget.Logs" />
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	protected override void Write(LogEventInfo logEvent)
	{
		_logs.Add(RenderLogEvent(Layout, logEvent), MaxLogsCount);
	}
}
