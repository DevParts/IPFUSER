using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DependencyNode
{
	private Urn urn;

	private bool schemaBound;

	public virtual Urn Urn
	{
		get
		{
			return urn;
		}
		set
		{
			urn = value;
		}
	}

	public virtual bool IsSchemaBound
	{
		get
		{
			return schemaBound;
		}
		set
		{
			schemaBound = value;
		}
	}

	protected internal DependencyNode()
	{
	}

	internal DependencyNode(Urn urn, bool isSchemaBound)
	{
		Urn = urn;
		schemaBound = isSchemaBound;
	}
}
