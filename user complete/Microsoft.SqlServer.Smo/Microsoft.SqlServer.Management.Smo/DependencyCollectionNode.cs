using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyCollectionNode : DependencyNode
{
	private bool isRootNode;

	public bool IsRootNode
	{
		get
		{
			return isRootNode;
		}
		set
		{
			isRootNode = value;
		}
	}

	internal bool Transactable
	{
		get
		{
			switch (Urn.Type)
			{
			case "Login":
			case "User":
			case "ApplicationRole":
			case "Role":
			case "Endpoint":
			case "FullTextCatalog":
			case "FullTextStopList":
			case "SearchPropertyList":
				return false;
			default:
				return true;
			}
		}
	}

	internal DependencyCollectionNode(Urn urn, bool isSchemaBound, bool fRoot)
		: base(urn, isSchemaBound)
	{
		IsRootNode = fRoot;
	}
}
