using System;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class VerifyCompleteEventArgs : EventArgs
{
	private bool verifySuccess;

	public bool VerifySuccess => verifySuccess;

	internal VerifyCompleteEventArgs(bool verifySuccess)
	{
		this.verifySuccess = verifySuccess;
	}
}
