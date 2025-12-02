namespace Microsoft.SqlServer.Management.Common;

public interface IRestrictedAccess
{
	bool SingleConnection { get; set; }
}
