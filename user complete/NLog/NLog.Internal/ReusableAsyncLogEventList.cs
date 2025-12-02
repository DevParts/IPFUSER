using System;
using System.Collections.Generic;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Controls a single allocated AsyncLogEventInfo-List for reuse (only one active user)
/// </summary>
internal sealed class ReusableAsyncLogEventList : ReusableObjectCreator<IList<AsyncLogEventInfo>>
{
	public ReusableAsyncLogEventList(int initialCapacity)
		: base((Func<IList<AsyncLogEventInfo>>)(() => new List<AsyncLogEventInfo>(initialCapacity)), (Action<IList<AsyncLogEventInfo>>)delegate(IList<AsyncLogEventInfo> l)
		{
			l.Clear();
		})
	{
	}
}
