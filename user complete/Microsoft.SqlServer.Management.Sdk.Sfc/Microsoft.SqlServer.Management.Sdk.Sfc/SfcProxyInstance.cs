namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcProxyInstance<K, T, TRef> : SfcInstance<K, T> where K : SfcKey where T : SfcInstance, new() where TRef : SfcInstance
{
	private TRef reference;

	public TRef Reference
	{
		get
		{
			if (reference == null)
			{
				reference = GetReferenceImpl();
			}
			return reference;
		}
	}

	public SfcProxyInstance()
	{
	}

	public SfcProxyInstance(TRef reference)
	{
		this.reference = reference;
	}

	protected abstract TRef GetReferenceImpl();
}
