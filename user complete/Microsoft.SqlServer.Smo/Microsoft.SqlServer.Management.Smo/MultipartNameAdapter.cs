using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class MultipartNameAdapter : IDmfAdapter, IMultipartNameFacet, IDmfFacet, IRefreshable
{
	private const string cName = "Name";

	private const string cSchema = "Schema";

	private ScriptSchemaObjectBase wrappedObject;

	private string schema = string.Empty;

	public string Name => wrappedObject.Name;

	public string Schema => wrappedObject.Schema;

	public MultipartNameAdapter(Table obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(View obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(UserDefinedFunction obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(StoredProcedure obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(Synonym obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(Sequence obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(UserDefinedType obj)
	{
		wrappedObject = obj;
	}

	public MultipartNameAdapter(XmlSchemaCollection obj)
	{
		wrappedObject = obj;
	}

	public void Refresh()
	{
		wrappedObject.Refresh();
	}
}
