using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public enum ResultType
{
	Default,
	DataSet,
	DataTable,
	IDataReader,
	XmlDocument,
	Reserved1,
	Reserved2
}
