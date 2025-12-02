using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PrefetchEventArgs : EventArgs
{
	private string smoType;

	private string filterConditionText;

	internal string Type => smoType;

	internal string FilterConditionText => filterConditionText;

	internal PrefetchEventArgs(string smoType, string filterConditionText)
	{
		this.smoType = smoType;
		this.filterConditionText = filterConditionText;
	}
}
