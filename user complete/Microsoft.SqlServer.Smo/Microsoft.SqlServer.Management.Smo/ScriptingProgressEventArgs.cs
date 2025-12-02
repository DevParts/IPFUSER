using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptingProgressEventArgs : EventArgs
{
	private List<Urn> urnList;

	public IList<Urn> Urns => urnList.AsReadOnly();

	public ScriptingProgressStages ProgressStage { get; private set; }

	internal ScriptingProgressEventArgs(ScriptingProgressStages progressStage, List<Urn> urnList)
	{
		ProgressStage = progressStage;
		this.urnList = urnList;
	}
}
