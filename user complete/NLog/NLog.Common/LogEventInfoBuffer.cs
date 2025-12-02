using System;
using System.ComponentModel;
using NLog.Internal;

namespace NLog.Common;

/// <summary>
/// A cyclic buffer of <see cref="T:NLog.LogEventInfo" /> object.
/// </summary>
[Obsolete("Use AsyncRequestQueue instead. Marked obsolete on NLog 5.0")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class LogEventInfoBuffer
{
	private readonly object _lockObject = new object();

	private readonly bool _growAsNeeded;

	private readonly int _growLimit;

	private AsyncLogEventInfo[] _buffer;

	private int _getPointer;

	private int _putPointer;

	private int _count;

	/// <summary>
	/// Gets the capacity of the buffer
	/// </summary>
	public int Size => _buffer.Length;

	/// <summary>
	/// Gets the number of items in the buffer
	/// </summary>
	internal int Count
	{
		get
		{
			lock (_lockObject)
			{
				return _count;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Common.LogEventInfoBuffer" /> class.
	/// </summary>
	/// <param name="size">Buffer size.</param>
	/// <param name="growAsNeeded">Whether buffer should grow as it becomes full.</param>
	/// <param name="growLimit">The maximum number of items that the buffer can grow to.</param>
	public LogEventInfoBuffer(int size, bool growAsNeeded, int growLimit)
	{
		_growAsNeeded = growAsNeeded;
		_buffer = new AsyncLogEventInfo[size];
		_growLimit = growLimit;
		_getPointer = 0;
		_putPointer = 0;
	}

	/// <summary>
	/// Adds the specified log event to the buffer.
	/// </summary>
	/// <param name="eventInfo">Log event.</param>
	/// <returns>The number of items in the buffer.</returns>
	public int Append(AsyncLogEventInfo eventInfo)
	{
		lock (_lockObject)
		{
			if (_count >= _buffer.Length)
			{
				if (_growAsNeeded && _buffer.Length < _growLimit)
				{
					int num = _buffer.Length * 2;
					if (num >= _growLimit)
					{
						num = _growLimit;
					}
					InternalLogger.Trace("Enlarging LogEventInfoBuffer from {0} to {1}", _buffer.Length, num);
					AsyncLogEventInfo[] array = new AsyncLogEventInfo[num];
					Array.Copy(_buffer, 0, array, 0, _buffer.Length);
					_buffer = array;
				}
				else
				{
					_getPointer++;
				}
			}
			_putPointer %= _buffer.Length;
			_buffer[_putPointer] = eventInfo;
			_putPointer++;
			_count++;
			if (_count >= _buffer.Length)
			{
				_count = _buffer.Length;
			}
			return _count;
		}
	}

	/// <summary>
	/// Gets the array of events accumulated in the buffer and clears the buffer as one atomic operation.
	/// </summary>
	/// <returns>Events in the buffer.</returns>
	public AsyncLogEventInfo[] GetEventsAndClear()
	{
		lock (_lockObject)
		{
			int count = _count;
			if (count == 0)
			{
				return ArrayHelper.Empty<AsyncLogEventInfo>();
			}
			AsyncLogEventInfo[] array = new AsyncLogEventInfo[count];
			for (int i = 0; i < count; i++)
			{
				int num = (_getPointer + i) % _buffer.Length;
				AsyncLogEventInfo asyncLogEventInfo = _buffer[num];
				_buffer[num] = default(AsyncLogEventInfo);
				array[i] = asyncLogEventInfo;
			}
			_count = 0;
			_getPointer = 0;
			_putPointer = 0;
			return array;
		}
	}
}
