using System;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Differencing.SPI;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class OnlineSmoAvailablePropertyProvider : AvailablePropertyValueProvider
{
	public override bool IsGraphSupported(ISfcSimpleNode node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		return node.ObjectReference is SqlSmoObject && !((SqlSmoObject)node.ObjectReference).IsDesignMode;
	}

	public override bool IsValueAvailable(ISfcSimpleNode node, string propName)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		if (propName == null)
		{
			throw new ArgumentNullException("propName");
		}
		if (propName == string.Empty)
		{
			throw new ArgumentException("propName");
		}
		if (node.ObjectReference is SqlSmoObject)
		{
			try
			{
				((SqlSmoObject)node.ObjectReference).Properties.GetPropertyObject(propName);
			}
			catch (Exception ex)
			{
				if (IsSystemGeneratedException(ex))
				{
					throw;
				}
				return !(ex is UnknownPropertyException);
			}
		}
		return true;
	}

	private static bool IsSystemGeneratedException(Exception e)
	{
		if (e is OutOfMemoryException)
		{
			return true;
		}
		if (e is StackOverflowException)
		{
			return true;
		}
		if (e is COMException || e is SEHException)
		{
			return true;
		}
		return false;
	}
}
