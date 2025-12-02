using System;
using System.Text;
using System.Threading;

namespace NLog.Internal;

internal sealed class StringBuilderPool
{
	/// <summary>
	/// Keeps track of acquired pool item
	/// </summary>
	public struct ItemHolder : IDisposable
	{
		public readonly StringBuilder Item;

		private readonly StringBuilderPool? _owner;

		private readonly int _poolIndex;

		public ItemHolder(StringBuilder stringBuilder, StringBuilderPool? owner, int poolIndex)
		{
			Item = stringBuilder;
			_owner = owner;
			_poolIndex = poolIndex;
		}

		/// <summary>
		/// Releases pool item back into pool
		/// </summary>
		public void Dispose()
		{
			_owner?.Release(Item, _poolIndex);
		}
	}

	private StringBuilder? _fastPool;

	private readonly StringBuilder?[] _slowPool;

	private readonly int _maxBuilderCapacity;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="poolCapacity">Max number of items</param>
	/// <param name="initialBuilderCapacity">Initial StringBuilder Size</param>
	/// <param name="maxBuilderCapacity">Max StringBuilder Size</param>
	public StringBuilderPool(int poolCapacity, int initialBuilderCapacity = 1024, int maxBuilderCapacity = 40960)
	{
		_fastPool = new StringBuilder(10 * initialBuilderCapacity);
		_slowPool = new StringBuilder[poolCapacity];
		for (int i = 0; i < _slowPool.Length; i++)
		{
			_slowPool[i] = new StringBuilder(initialBuilderCapacity);
		}
		_maxBuilderCapacity = maxBuilderCapacity;
	}

	/// <summary>
	/// Takes StringBuilder from pool
	/// </summary>
	/// <returns>Allow return to pool</returns>
	public ItemHolder Acquire()
	{
		StringBuilder fastPool = _fastPool;
		if (fastPool == null || fastPool != Interlocked.CompareExchange(ref _fastPool, null, fastPool))
		{
			for (int i = 0; i < _slowPool.Length; i++)
			{
				fastPool = _slowPool[i];
				if (fastPool != null && fastPool == Interlocked.CompareExchange(ref _slowPool[i], null, fastPool))
				{
					return new ItemHolder(fastPool, this, i);
				}
			}
			return new ItemHolder(new StringBuilder(), null, 0);
		}
		return new ItemHolder(fastPool, this, -1);
	}

	/// <summary>
	/// Releases StringBuilder back to pool at its right place
	/// </summary>
	private void Release(StringBuilder stringBuilder, int poolIndex)
	{
		if (stringBuilder.Length > _maxBuilderCapacity)
		{
			int num = ((poolIndex == -1) ? (_maxBuilderCapacity * 10) : _maxBuilderCapacity);
			if (stringBuilder.Length > num)
			{
				stringBuilder.Remove(0, stringBuilder.Length - 1);
				if (stringBuilder.Capacity > num)
				{
					stringBuilder.Capacity = _maxBuilderCapacity;
				}
			}
		}
		stringBuilder.ClearBuilder();
		if (poolIndex == -1)
		{
			_fastPool = stringBuilder;
		}
		else
		{
			_slowPool[poolIndex] = stringBuilder;
		}
	}
}
