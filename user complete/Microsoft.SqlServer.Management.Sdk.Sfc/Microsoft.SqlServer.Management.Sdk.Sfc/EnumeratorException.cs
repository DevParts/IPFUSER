using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class EnumeratorException : SqlServerManagementException
{
	public EnumeratorException()
	{
	}

	public EnumeratorException(string message)
		: base(message)
	{
	}

	public EnumeratorException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected EnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	internal static void FilterException(Exception e)
	{
		if (e is OutOfMemoryException)
		{
			throw e;
		}
	}
}
