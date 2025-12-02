using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ExtendedPropertyCollection : SimpleObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public ExtendedProperty this[int index] => GetObjectByIndex(index) as ExtendedProperty;

	public ExtendedProperty this[string name] => GetObjectByName(name) as ExtendedProperty;

	internal ExtendedPropertyCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ExtendedProperty[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ExtendedProperty);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ExtendedProperty(this, key, state);
	}

	public void Remove(ExtendedProperty extendedProperty)
	{
		if (extendedProperty == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("extendedProperty"));
		}
		RemoveObj(extendedProperty, new SimpleObjectKey(extendedProperty.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(ExtendedProperty extendedProperty)
	{
		AddImpl(extendedProperty);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
