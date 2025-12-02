using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ScriptingErrorEventArgs : EventArgs
{
	private Urn current;

	private Exception innerException;

	public Urn Current => current;

	public Exception InnerException => innerException;

	internal ScriptingErrorEventArgs(Urn current, Exception innerException)
	{
		this.current = current;
		this.innerException = innerException;
	}
}
