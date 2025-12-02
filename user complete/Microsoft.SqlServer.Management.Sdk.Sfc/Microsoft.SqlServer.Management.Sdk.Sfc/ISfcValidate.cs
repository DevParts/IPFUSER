namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcValidate
{
	ValidationState Validate(string methodName, params object[] arguments);
}
