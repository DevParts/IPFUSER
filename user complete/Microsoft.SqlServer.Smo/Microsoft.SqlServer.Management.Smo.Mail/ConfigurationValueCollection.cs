using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class ConfigurationValueCollection : SimpleObjectCollectionBase
{
	public SqlMail Parent => base.ParentInstance as SqlMail;

	public ConfigurationValue this[int index] => GetObjectByIndex(index) as ConfigurationValue;

	public ConfigurationValue this[string name] => GetObjectByName(name) as ConfigurationValue;

	internal ConfigurationValueCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(ConfigurationValue[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(ConfigurationValue);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new ConfigurationValue(this, key, state);
	}

	public void Add(ConfigurationValue configurationValue)
	{
		AddImpl(configurationValue);
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
