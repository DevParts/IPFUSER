using System;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class ParameterCollectionBase : ArrayListCollectionBase
{
	internal ParameterCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoArrayList(new SimpleObjectComparer(StringComparer), this);
	}

	public bool Contains(string name)
	{
		return Contains(new SimpleObjectKey(name));
	}

	public void Remove(string name)
	{
		CheckCollectionLock();
		Remove(new SimpleObjectKey(name));
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

	protected override void ImplAddExisting(SqlSmoObject obj)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(obj.Properties.Contains("ID"));
		int num = Convert.ToInt32(obj.GetPropValueOptional("ID", (object)(-1)), SmoApplication.DefaultCulture);
		for (int i = 0; i < base.InternalStorage.Count; i++)
		{
			SqlSmoObject byIndex = base.InternalStorage.GetByIndex(i);
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(byIndex.Properties.Contains("ID"));
			int num2 = Convert.ToInt32(byIndex.GetPropValueOptional("ID", (object)(-1)), SmoApplication.DefaultCulture);
			if (-1 != num2 && num < num2)
			{
				base.InternalStorage.InsertAt(i, obj);
				return;
			}
		}
		base.InternalStorage.InsertAt(base.InternalStorage.Count, obj);
	}
}
