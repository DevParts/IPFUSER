using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class MailProfileCollection : SimpleObjectCollectionBase
{
	public SqlMail Parent => base.ParentInstance as SqlMail;

	public MailProfile this[int index] => GetObjectByIndex(index) as MailProfile;

	public MailProfile this[string name] => GetObjectByName(name) as MailProfile;

	internal MailProfileCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(MailProfile[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public MailProfile ItemById(int id)
	{
		return (MailProfile)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(MailProfile);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new MailProfile(this, key, state);
	}

	public void Add(MailProfile mailProfile)
	{
		AddImpl(mailProfile);
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
