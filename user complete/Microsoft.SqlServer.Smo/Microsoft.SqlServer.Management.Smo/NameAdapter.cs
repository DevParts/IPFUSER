using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class NameAdapter : IDmfAdapter, INameFacet, IDmfFacet, IRefreshable
{
	private const string cName = "Name";

	private NamedSmoObject wrappedObject;

	public string Name => wrappedObject.Name;

	public NameAdapter(Table obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Index obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(StoredProcedure obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Trigger obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(SqlAssembly obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(View obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(UserDefinedFunction obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Synonym obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Sequence obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(UserDefinedType obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Rule obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Default obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(User obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(AsymmetricKey obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(SymmetricKey obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Certificate obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(DatabaseRole obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(ApplicationRole obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(Schema obj)
	{
		wrappedObject = obj;
	}

	public NameAdapter(XmlSchemaCollection obj)
	{
		wrappedObject = obj;
	}

	public void Refresh()
	{
		wrappedObject.Refresh();
	}
}
