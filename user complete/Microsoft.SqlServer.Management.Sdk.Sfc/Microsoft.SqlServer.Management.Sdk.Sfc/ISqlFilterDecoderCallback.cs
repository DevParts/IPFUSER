using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public interface ISqlFilterDecoderCallback
{
	bool SupportsParameterization { get; }

	string AddPropertyForFilter(string name);

	string AddConstantForFilter(string constantValue);
}
