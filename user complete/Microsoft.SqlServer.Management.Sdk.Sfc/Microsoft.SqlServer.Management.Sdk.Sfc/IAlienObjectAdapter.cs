using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal class IAlienObjectAdapter : SimpleNodeAdapter
{
	public override bool IsSupported(object node)
	{
		if (node is IAlienObject)
		{
			return true;
		}
		return false;
	}

	public override Urn GetUrn(object reference)
	{
		IAlienObject alienObject = reference as IAlienObject;
		return alienObject.GetUrn();
	}

	public override object GetProperty(object instance, string propertyName)
	{
		IAlienObject alienObject = instance as IAlienObject;
		try
		{
			Type propertyType = alienObject.GetPropertyType(propertyName);
			return alienObject.GetPropertyValue(propertyName, propertyType);
		}
		catch (TargetInvocationException ex)
		{
			TraceHelper.LogExCatch(ex);
			return null;
		}
	}
}
