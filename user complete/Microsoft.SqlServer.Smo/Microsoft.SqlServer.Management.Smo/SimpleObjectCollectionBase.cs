using System;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class SimpleObjectCollectionBase : SortedListCollectionBase
{
	internal SimpleObjectCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new SimpleObjectComparer(StringComparer));
	}

	public bool Contains(string name)
	{
		if (name == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Contains, this, new ArgumentNullException("name"));
		}
		return Contains(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || (attribute.Length == 0 && !CanHaveEmptyName(urn)))
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
