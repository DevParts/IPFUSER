using System;

namespace NLog.Internal;

/// <summary>
/// Controls a single allocated char[]-buffer for reuse (only one active user)
/// </summary>
internal sealed class ReusableBufferCreator : ReusableObjectCreator<char[]>
{
	public ReusableBufferCreator(int initialCapacity)
		: base((Func<char[]>)(() => new char[initialCapacity]), (Action<char[]>)delegate
		{
		})
	{
	}
}
