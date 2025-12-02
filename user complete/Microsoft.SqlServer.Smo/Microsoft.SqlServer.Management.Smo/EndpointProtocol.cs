using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class EndpointProtocol : SqlSmoObject
{
	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Endpoint Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Endpoint;
		}
	}

	internal EndpointProtocol(Endpoint parentEndpoint, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentEndpoint;
		SetServerObject(parentEndpoint.GetServerObject());
		m_comparer = ((Endpoint)singletonParent).Parent.Databases["master"].StringComparer;
	}

	public override string ToString()
	{
		return Parent.ToString();
	}

	internal abstract void Script(StringBuilder sb, ScriptingPreferences sp);
}
