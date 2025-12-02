using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal class DataScriptCollection : IEnumerable<string>, IEnumerable
{
	private DataEnumerator dataEnumerator;

	private Table table;

	private ScriptingPreferences options;

	public DataScriptCollection(Table table, ScriptingPreferences options)
	{
		if (table == null)
		{
			throw new ArgumentNullException(ExceptionTemplatesImpl.NullParameterTable);
		}
		if (options == null)
		{
			throw new ArgumentNullException(ExceptionTemplatesImpl.NullParameterScriptingOptions);
		}
		this.table = table;
		this.options = options;
	}

	public DataScriptCollection(DataEnumerator dataEnumerator)
	{
		if (dataEnumerator == null)
		{
			throw new ArgumentNullException("dataEnumerator");
		}
		this.dataEnumerator = dataEnumerator;
	}

	public IEnumerator<string> GetEnumerator()
	{
		if (dataEnumerator == null)
		{
			dataEnumerator = new DataEnumerator(table, options);
		}
		return dataEnumerator;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (dataEnumerator == null)
		{
			dataEnumerator = new DataEnumerator(table, options);
		}
		return dataEnumerator;
	}
}
