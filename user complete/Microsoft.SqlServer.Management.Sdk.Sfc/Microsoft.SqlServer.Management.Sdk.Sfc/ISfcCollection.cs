namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public interface ISfcCollection
{
	SfcInstance Parent { get; }

	bool Initialized { get; set; }

	int Count { get; }

	string GetCollectionElementNameImpl();

	SfcObjectFactory GetElementFactory();

	void EnsureInitialized();

	void Add(SfcInstance sfcInstance);

	void Remove(SfcInstance sfcInstance);

	void RemoveElement(SfcInstance sfcInstance);

	void Rename(SfcInstance sfcInstance, SfcKey newKey);

	bool GetExisting(SfcKey key, out SfcInstance sfcInstance);

	SfcInstance GetObjectByKey(SfcKey key);

	void PrepareMerge();

	bool AddShadow(SfcInstance sfcInstance);

	void FinishMerge();
}
