namespace Microsoft.SqlServer.Management.Smo;

internal class PropertyStorageFactory
{
	internal static PropertyStorageBase Create(SmoObjectBase parent, int propCount)
	{
		if (!(parent is IPropertyDataDispatch dispatch))
		{
			return new PropertyBag(propCount);
		}
		return new PropertyDispatcher(propCount, dispatch);
	}
}
