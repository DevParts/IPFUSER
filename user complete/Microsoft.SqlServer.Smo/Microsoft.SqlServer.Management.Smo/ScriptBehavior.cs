using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum ScriptBehavior
{
	Create = 1,
	Drop = 2,
	DropAndCreate = 3,
	CreateOrAlter = 4
}
