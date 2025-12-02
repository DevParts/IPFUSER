using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class InvalidPropertyUsageEnumeratorException : EnumeratorException
{
	public InvalidPropertyUsageEnumeratorException()
	{
		base.HResult = 8;
	}

	public InvalidPropertyUsageEnumeratorException(string message)
		: base(message)
	{
		base.HResult = 8;
	}

	public InvalidPropertyUsageEnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
		base.HResult = 8;
	}

	private InvalidPropertyUsageEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 8;
	}

	internal static void Throw(string propertyName, ObjectPropertyUsages usage_not_resolved)
	{
		string text = string.Empty;
		if ((ObjectPropertyUsages.Request & usage_not_resolved) != ObjectPropertyUsages.None)
		{
			text = SfcStrings.UsageRequest + " ";
		}
		if ((ObjectPropertyUsages.Filter & usage_not_resolved) != ObjectPropertyUsages.None)
		{
			text = text + SfcStrings.UsageFilter + " ";
		}
		if ((ObjectPropertyUsages.OrderBy & usage_not_resolved) != ObjectPropertyUsages.None)
		{
			text = text + SfcStrings.UsageOrderBy + " ";
		}
		throw new InvalidPropertyUsageEnumeratorException(SfcStrings.PropertyUsageError(propertyName, text));
	}
}
