namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcObjectFactory
{
	protected abstract SfcInstance CreateImpl();

	internal SfcInstance Create(SfcInstance parent, IPropertyCollectionPopulator populator, SfcObjectState state)
	{
		SfcInstance sfcInstance = CreateImpl();
		sfcInstance.Parent = parent;
		sfcInstance.State = state;
		populator.Populate(sfcInstance.Properties);
		return sfcInstance;
	}

	internal SfcInstance Create(SfcInstance parent, SfcKey key, SfcObjectState state)
	{
		SfcInstance sfcInstance = CreateImpl();
		sfcInstance.State = state;
		sfcInstance.KeyChain = new SfcKeyChain(key, parent.KeyChain);
		return sfcInstance;
	}
}
