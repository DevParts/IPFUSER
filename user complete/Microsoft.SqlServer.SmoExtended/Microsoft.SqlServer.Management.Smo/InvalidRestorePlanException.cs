namespace Microsoft.SqlServer.Management.Smo;

public sealed class InvalidRestorePlanException : SmoException
{
	internal InvalidRestorePlanException(object source, string reason)
		: base(reason)
	{
	}
}
