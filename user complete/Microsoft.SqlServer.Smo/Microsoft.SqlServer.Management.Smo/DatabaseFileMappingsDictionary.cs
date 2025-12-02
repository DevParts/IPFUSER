using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
public sealed class DatabaseFileMappingsDictionary : Dictionary<string, string>
{
	private Dictionary<string, string> databaseFileMappingsDictionary;

	public new string this[string sourceFilePath]
	{
		get
		{
			return databaseFileMappingsDictionary[sourceFilePath];
		}
		set
		{
			databaseFileMappingsDictionary[sourceFilePath] = value;
		}
	}

	public DatabaseFileMappingsDictionary()
	{
		databaseFileMappingsDictionary = new Dictionary<string, string>();
	}

	public new bool ContainsKey(string sourceFilePath)
	{
		return databaseFileMappingsDictionary.ContainsKey(sourceFilePath);
	}

	public new void Add(string sourceFilePath, string targetFilePath)
	{
		databaseFileMappingsDictionary.Add(sourceFilePath, targetFilePath);
	}
}
