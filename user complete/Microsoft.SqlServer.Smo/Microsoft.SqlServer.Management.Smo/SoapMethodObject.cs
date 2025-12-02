using System;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class SoapMethodObject : ScriptNameObjectBase
{
	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Namespace
	{
		get
		{
			return ((SoapMethodKey)key).Namespace;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Namespace"));
			}
			if (base.State == SqlSmoState.Pending)
			{
				((SoapMethodKey)key).Namespace = value;
				UpdateObjectState();
				return;
			}
			if (base.State == SqlSmoState.Creating && base.ObjectInSpace)
			{
				((SoapMethodKey)key).Namespace = value;
				UpdateObjectState();
				return;
			}
			throw new FailedOperationException(ExceptionTemplatesImpl.SetNamespace, this, new InvalidSmoOperationException(ExceptionTemplatesImpl.SetNamespace, base.State));
		}
	}

	internal SoapMethodObject(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal SoapMethodObject(ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
	}

	protected internal SoapMethodObject()
	{
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new SoapMethodKey(null, null);
	}
}
