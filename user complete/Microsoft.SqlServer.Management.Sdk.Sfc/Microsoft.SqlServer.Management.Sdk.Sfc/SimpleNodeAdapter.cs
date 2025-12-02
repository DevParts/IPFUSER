using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SimpleNodeAdapter
{
	public abstract bool IsSupported(object reference);

	public abstract Urn GetUrn(object reference);

	public virtual object GetProperty(object reference, string propertyName)
	{
		try
		{
			PropertyInfo property = reference.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
			return property.GetValue(reference, null);
		}
		catch (TargetInvocationException)
		{
			return null;
		}
	}

	public virtual object GetObject(object reference, string childName)
	{
		try
		{
			PropertyInfo property = reference.GetType().GetProperty(childName, BindingFlags.Instance | BindingFlags.Public);
			return property.GetValue(reference, null);
		}
		catch (TargetInvocationException)
		{
			return null;
		}
	}

	public virtual IEnumerable GetEnumerable(object reference, string enumName)
	{
		try
		{
			PropertyInfo property = reference.GetType().GetProperty(enumName, BindingFlags.Instance | BindingFlags.Public);
			object value = property.GetValue(reference, null);
			return value as IEnumerable;
		}
		catch (TargetInvocationException)
		{
			return null;
		}
	}

	public virtual bool IsCriteriaMatched(object reference)
	{
		return true;
	}

	internal object CheckedGetProperty(object reference, string propertyName)
	{
		try
		{
			return GetProperty(reference, propertyName);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return null;
		}
	}

	internal object CheckedGetObject(object reference, string childName)
	{
		try
		{
			return GetObject(reference, childName);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return null;
		}
	}

	internal IEnumerable CheckedGetEnumerable(object reference, string enumName)
	{
		try
		{
			return GetEnumerable(reference, enumName);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return null;
		}
	}

	internal Urn CheckedGetUrn(object reference)
	{
		try
		{
			return GetUrn(reference);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return null;
		}
	}

	internal bool CheckedIsCriteriaMatched(object reference)
	{
		try
		{
			return IsCriteriaMatched(reference);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return false;
		}
	}

	internal bool CheckedIsSupported(object reference)
	{
		try
		{
			return IsSupported(reference);
		}
		catch (Exception ex)
		{
			if (IsSystemGeneratedException(ex))
			{
				throw ex;
			}
			TraceHelper.LogExCatch(ex);
			return false;
		}
	}

	internal static bool IsSystemGeneratedException(Exception e)
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
