namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SfcCollectionInfo
{
	private string m_displayName;

	private object m_collection;

	public string DisplayName => m_displayName;

	public object Collection => m_collection;

	public SfcCollectionInfo(string displayName, object collection)
	{
		m_displayName = displayName;
		m_collection = collection;
	}
}
