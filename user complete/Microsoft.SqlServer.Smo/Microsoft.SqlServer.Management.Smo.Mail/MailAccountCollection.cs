using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class MailAccountCollection : SimpleObjectCollectionBase
{
	public SqlMail Parent => base.ParentInstance as SqlMail;

	public MailAccount this[int index] => GetObjectByIndex(index) as MailAccount;

	public MailAccount this[string name] => GetObjectByName(name) as MailAccount;

	internal MailAccountCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(MailAccount[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public MailAccount ItemById(int id)
	{
		return (MailAccount)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(MailAccount);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new MailAccount(this, key, state);
	}

	public void Add(MailAccount mailAccount)
	{
		AddImpl(mailAccount);
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
