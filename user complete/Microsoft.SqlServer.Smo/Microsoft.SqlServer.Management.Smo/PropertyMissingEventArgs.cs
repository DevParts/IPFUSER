using System;

namespace Microsoft.SqlServer.Management.Smo;

public class PropertyMissingEventArgs : EventArgs
{
	public string PropertyName { get; private set; }

	public string TypeName { get; private set; }

	internal PropertyMissingEventArgs(string propertyName, string typeName)
	{
		PropertyName = propertyName;
		TypeName = typeName;
	}
}
