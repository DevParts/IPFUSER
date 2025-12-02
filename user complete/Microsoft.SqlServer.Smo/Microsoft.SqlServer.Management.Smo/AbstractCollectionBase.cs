using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class AbstractCollectionBase
{
	private SqlSmoObject parentInstance;

	protected internal bool initialized;

	private bool m_bIsDirty;

	public SqlSmoObject ParentInstance => parentInstance;

	internal virtual StringComparer StringComparer => parentInstance.StringComparer;

	internal abstract int NoFaultCount { get; }

	internal bool IsDirty
	{
		get
		{
			return m_bIsDirty;
		}
		set
		{
			m_bIsDirty = value;
		}
	}

	internal AbstractCollectionBase(SqlSmoObject parentInstance)
	{
		this.parentInstance = parentInstance;
		initialized = false;
		m_bIsDirty = false;
	}

	internal void MarkOutOfSync()
	{
		initialized = false;
	}

	protected internal void AddExisting(SqlSmoObject smoObj)
	{
		ImplAddExisting(smoObj);
	}

	protected abstract void ImplAddExisting(SqlSmoObject smoObj);

	internal void RemoveObject(ObjectKeyBase key)
	{
		ImplRemove(key);
	}

	internal abstract void ImplRemove(ObjectKeyBase key);

	internal abstract SqlSmoObject NoFaultLookup(ObjectKeyBase key);

	internal abstract ObjectKeyBase CreateKeyFromUrn(Urn urn);
}
