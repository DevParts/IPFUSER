using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class SoapPayloadMethodCollection : SoapMethodCollectionBase
{
	public SoapPayload Parent => base.ParentInstance as SoapPayload;

	public SoapPayloadMethod this[string name] => GetObjectByKey(new SoapMethodKey(name, SoapMethodCollectionBase.GetDefaultNamespace())) as SoapPayloadMethod;

	public SoapPayloadMethod this[string name, string methodNamespace] => GetObjectByKey(new SoapMethodKey(name, methodNamespace)) as SoapPayloadMethod;

	public SoapPayloadMethod this[int index] => GetObjectByIndex(index) as SoapPayloadMethod;

	internal SoapPayloadMethodCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(SoapPayloadMethod);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SoapPayloadMethod(this, key, state);
	}

	public void CopyTo(SoapPayloadMethod[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public void Remove(SoapPayloadMethod soapMethod)
	{
		if (soapMethod == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("soapMethod"));
		}
		RemoveObj(soapMethod, soapMethod.key);
	}

	public void Add(SoapPayloadMethod soapMethod)
	{
		AddImpl(soapMethod);
	}
}
