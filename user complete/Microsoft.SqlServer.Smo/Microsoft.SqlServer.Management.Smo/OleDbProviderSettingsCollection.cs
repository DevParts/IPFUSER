using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class OleDbProviderSettingsCollection : SimpleObjectCollectionBase
{
	public Settings Parent => base.ParentInstance as Settings;

	public OleDbProviderSettings this[int index] => GetObjectByIndex(index) as OleDbProviderSettings;

	public OleDbProviderSettings this[string name] => GetObjectByName(name) as OleDbProviderSettings;

	internal OleDbProviderSettingsCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(OleDbProviderSettings[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(OleDbProviderSettings);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new OleDbProviderSettings(this, key, state);
	}

	public void Add(OleDbProviderSettings settings)
	{
		AddImpl(settings);
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
