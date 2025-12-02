using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoStringWriter : ISmoScriptWriter
{
	private string currentContext;

	private string _header;

	private bool _wroteHeader;

	public StringCollection FinalStringCollection { get; set; }

	public string Header
	{
		private get
		{
			return _header;
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				_header = value;
			}
		}
	}

	public SmoStringWriter()
	{
		FinalStringCollection = new StringCollection();
		currentContext = string.Empty;
	}

	public void ScriptObject(IEnumerable<string> script, Urn obj)
	{
		FinalStringCollection.AddCollection(script);
		PrependHeaderIfNeeded();
	}

	public void ScriptData(IEnumerable<string> dataScript, Urn table)
	{
		FinalStringCollection.AddCollection(dataScript);
		PrependHeaderIfNeeded();
	}

	public void ScriptContext(string databaseContext, Urn obj)
	{
		if (!databaseContext.Equals(currentContext, StringComparison.Ordinal))
		{
			FinalStringCollection.Add(databaseContext);
			currentContext = databaseContext;
		}
		PrependHeaderIfNeeded();
	}

	private void PrependHeaderIfNeeded()
	{
		if (!_wroteHeader && !string.IsNullOrEmpty(Header) && FinalStringCollection.Count > 0)
		{
			_wroteHeader = true;
			FinalStringCollection[0] = Header + System.Environment.NewLine + System.Environment.NewLine + FinalStringCollection[0];
		}
	}
}
