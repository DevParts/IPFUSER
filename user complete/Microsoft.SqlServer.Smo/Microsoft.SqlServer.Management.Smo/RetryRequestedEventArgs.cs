using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class RetryRequestedEventArgs : EventArgs
{
	public Urn Urn { get; private set; }

	public ScriptingPreferences ScriptingPreferences { get; private set; }

	public bool ShouldRetry { get; set; }

	public string PreText { get; set; }

	public string PostText { get; set; }

	public RetryRequestedEventArgs(Urn urn, ScriptingPreferences scriptingPreferences)
	{
		if (scriptingPreferences == null)
		{
			throw new ArgumentNullException("scriptingPreferences");
		}
		Urn = urn;
		ScriptingPreferences = scriptingPreferences;
	}
}
