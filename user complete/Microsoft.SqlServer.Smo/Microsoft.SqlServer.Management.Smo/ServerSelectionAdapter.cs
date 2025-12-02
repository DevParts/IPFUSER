using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class ServerSelectionAdapter : ServerAdapterBase, IDmfAdapter, IServerSelectionFacet, IDmfFacet
{
	public int BuildNumber => base.Server.BuildNumber;

	public string Edition => base.Server.Edition;

	public bool IsCaseSensitive => base.Server.IsCaseSensitive;

	public string Language => base.Server.Language;

	public string OSVersion => base.Server.OSVersion;

	public string Platform => base.Server.Platform;

	public int VersionMajor => base.Server.VersionMajor;

	public int VersionMinor => base.Server.VersionMinor;

	public ServerSelectionAdapter(Server obj)
		: base(obj)
	{
	}
}
