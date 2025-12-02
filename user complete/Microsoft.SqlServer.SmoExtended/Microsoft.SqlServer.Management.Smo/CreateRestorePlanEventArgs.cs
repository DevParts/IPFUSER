namespace Microsoft.SqlServer.Management.Smo;

public class CreateRestorePlanEventArgs
{
	public string Status;

	public CreateRestorePlanEventArgs(string status)
	{
		Status = status;
	}
}
