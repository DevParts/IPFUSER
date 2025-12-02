using System;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
public sealed class ResultTypeNotSupportedEnumeratorException : EnumeratorException
{
	private string m_type;

	public string ResultType => m_type;

	public ResultTypeNotSupportedEnumeratorException()
	{
		base.HResult = 5;
	}

	public ResultTypeNotSupportedEnumeratorException(string msg)
		: base(msg)
	{
	}

	public ResultTypeNotSupportedEnumeratorException(string msg, Exception e)
		: base(msg, e)
	{
	}

	public ResultTypeNotSupportedEnumeratorException(ResultType type)
		: base(SfcStrings.ResultNotSupported)
	{
		base.HResult = 5;
		m_type = type.ToString();
	}

	public ResultTypeNotSupportedEnumeratorException(ResultType type, Exception innerException)
		: base(SfcStrings.ResultNotSupported, innerException)
	{
		base.HResult = 5;
		m_type = type.ToString();
	}

	private ResultTypeNotSupportedEnumeratorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		base.HResult = 5;
		m_type = info.GetString("m_type");
	}
}
