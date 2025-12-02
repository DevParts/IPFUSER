using System;

namespace NLog.Internal;

internal static class ArrayHelper
{
	internal static T[] Empty<T>()
	{
		return Array.Empty<T>();
	}
}
