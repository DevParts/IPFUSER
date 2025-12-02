using System;
using System.IO;

namespace NLog.Internal;

/// <summary>
/// Controls a single allocated MemoryStream for reuse (only one active user)
/// </summary>
internal sealed class ReusableStreamCreator : ReusableObjectCreator<MemoryStream>, IDisposable
{
	public ReusableStreamCreator()
		: base((Func<MemoryStream>)(() => new MemoryStream(4096)), (Action<MemoryStream>)delegate(MemoryStream ms)
		{
			ResetCapacity(ms);
		})
	{
	}

	public ReusableStreamCreator(bool batchStream)
		: base((Func<MemoryStream>)(() => new MemoryStream(4096)), (Action<MemoryStream>)delegate(MemoryStream ms)
		{
			ResetBatchCapacity(ms);
		})
	{
	}

	private static void ResetCapacity(MemoryStream memoryStream)
	{
		memoryStream.Position = 0L;
		memoryStream.SetLength(0L);
		if (memoryStream.Capacity > 1000000)
		{
			memoryStream.Capacity = 81920;
		}
	}

	private static void ResetBatchCapacity(MemoryStream memoryStream)
	{
		memoryStream.Position = 0L;
		memoryStream.SetLength(0L);
		if (memoryStream.Capacity > 10000000)
		{
			memoryStream.Capacity = 81920;
		}
	}

	void IDisposable.Dispose()
	{
		_reusableObject?.Dispose();
	}
}
