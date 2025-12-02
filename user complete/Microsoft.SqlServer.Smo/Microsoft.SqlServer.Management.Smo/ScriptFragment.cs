using System;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptFragment
{
	private StringCollection script;

	private Exception ex;

	public StringCollection Script
	{
		get
		{
			if (ex == null)
			{
				return script;
			}
			throw ex;
		}
	}

	public ScriptFragment(StringCollection script)
	{
		if (script == null)
		{
			throw new ArgumentNullException("script");
		}
		this.script = script;
	}

	public ScriptFragment(Exception ex)
	{
		if (ex == null)
		{
			throw new ArgumentNullException("ex");
		}
		this.ex = ex;
	}
}
