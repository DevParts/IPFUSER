namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class CacheElement
{
	private EnumObject element;

	private ObjectLoadInfo oli;

	private uint usage;

	private CacheKey key;

	public EnumObject EnumObject => element;

	public CacheKey CacheKey => key;

	public uint Usage => usage;

	public CacheElement(ObjectLoadInfo oli, EnumObject element, CacheKey key)
	{
		usage = 0u;
		this.oli = oli;
		this.element = element;
		this.key = key;
	}

	public void IncrementUsage()
	{
		if (usage < 15)
		{
			usage++;
		}
	}

	public void DecrementUsage()
	{
		if (usage != 0)
		{
			usage--;
		}
	}

	public string[] GetChildren()
	{
		string[] array = new string[oli.Children.Count];
		int num = 0;
		foreach (ObjectLoadInfo value in oli.Children.Values)
		{
			array[num++] = value.Name;
		}
		return array;
	}
}
