namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcScriptCollector
{
	T OpenWriter<T>();

	T OpenWriter<T>(bool append);
}
