namespace Microsoft.SqlServer.Management.Smo;

internal interface IScriptStringBuilderParameter
{
	string GetKey();

	string ToScript();
}
