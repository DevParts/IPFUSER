using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class XPathException : EnumeratorException
{
	private string[] _pArgs;

	public XPathExceptionCode ErrorCode => (XPathExceptionCode)base.HResult;

	public override string Message
	{
		get
		{
			string text = " (";
			if (_pArgs != null)
			{
				string[] pArgs = _pArgs;
				foreach (string text2 in pArgs)
				{
					text += text2;
					text += ';';
				}
			}
			if (';' == text[text.Length - 1])
			{
				text = text.Remove(text.Length - 1, 1);
			}
			text += ')';
			XPathExceptionCode errorCode = ErrorCode;
			if (errorCode == XPathExceptionCode.UnclosedString)
			{
				return SfcStrings.XPathUnclosedString + text;
			}
			return SfcStrings.XPathSyntaxError + text;
		}
	}

	public XPathException()
	{
		base.HResult = 0;
	}

	public XPathException(string msg)
		: base(msg)
	{
	}

	public XPathException(string msg, Exception e)
		: base(msg, e)
	{
	}

	protected XPathException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	internal XPathException(XPathExceptionCode hr)
	{
		base.HResult = (int)hr;
	}

	internal XPathException(XPathExceptionCode hr, string[] args)
	{
		_pArgs = args;
		base.HResult = (int)hr;
	}

	internal XPathException(XPathExceptionCode hr, string arg)
	{
		_pArgs = new string[1];
		_pArgs[0] = arg;
		base.HResult = (int)hr;
	}
}
