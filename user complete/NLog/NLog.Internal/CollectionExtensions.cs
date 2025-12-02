using System;
using System.Collections.Generic;

namespace NLog.Internal;

internal static class CollectionExtensions
{
	/// <summary>
	/// Memory optimized filtering
	/// </summary>
	/// <remarks>Passing state too avoid delegate capture and memory-allocations.</remarks>
	public static IList<TItem> Filter<TItem, TState>(this IList<TItem> items, TState state, Func<TItem, TState, bool> filter)
	{
		bool flag = false;
		List<TItem> list = null;
		for (int i = 0; i < items.Count; i++)
		{
			TItem val = items[i];
			if (filter(val, state))
			{
				if (flag && list == null)
				{
					list = new List<TItem>();
				}
				list?.Add(val);
				continue;
			}
			if (!flag && i > 0)
			{
				list = new List<TItem>();
				for (int j = 0; j < i; j++)
				{
					list.Add(items[j]);
				}
			}
			flag = true;
		}
		IList<TItem> list2 = list;
		IList<TItem> list3 = list2;
		if (list3 == null)
		{
			if (!flag)
			{
				return items;
			}
			list2 = ArrayHelper.Empty<TItem>();
			list3 = list2;
		}
		return list3;
	}
}
