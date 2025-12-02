using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class LanguageCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Language this[int index] => GetObjectByIndex(index) as Language;

	public Language this[string name] => GetObjectByName(name) as Language;

	internal LanguageCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Language[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Language ItemById(int id)
	{
		return (Language)GetItemById(id, "LocaleID");
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Language);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Language(this, key, state);
	}

	public void Add(Language language)
	{
		AddImpl(language);
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
