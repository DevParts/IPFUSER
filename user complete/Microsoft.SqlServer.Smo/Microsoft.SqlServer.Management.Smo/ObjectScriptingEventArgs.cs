using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ObjectScriptingEventArgs : EventArgs
{
	public Urn Current { get; private set; }

	public Urn Original { get; private set; }

	public int CurrentCount { get; private set; }

	public int Total { get; private set; }

	public ObjectScriptingType ScriptType { get; private set; }

	internal ObjectScriptingEventArgs(Urn current, Urn original, int currentCount, int total, ObjectScriptingType scriptType)
	{
		Current = current;
		Original = original;
		CurrentCount = currentCount;
		Total = total;
		ScriptType = scriptType;
	}
}
