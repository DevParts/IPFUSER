using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class UserDefinedMessageCollection : MessageCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public UserDefinedMessage this[int index] => GetObjectByIndex(index) as UserDefinedMessage;

	public UserDefinedMessage this[int id, string language] => GetObjectByKey(new MessageObjectKey(id, language)) as UserDefinedMessage;

	internal UserDefinedMessageCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public UserDefinedMessage ItemByIdAndLanguage(int id, string language)
	{
		return this[id, language];
	}

	public UserDefinedMessage ItemByIdAndLanguageId(int id, int languageId)
	{
		Language language = (base.ParentInstance as Server).Languages.ItemById(languageId);
		if (language != null)
		{
			return this[id, language.Name];
		}
		throw new FailedOperationException(ExceptionTemplatesImpl.ObjectDoesNotExist("LanguageID", languageId.ToString(SmoApplication.DefaultCulture)));
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(UserDefinedMessage);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new UserDefinedMessage(this, key, state);
	}

	public void CopyTo(UserDefinedMessage[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}
}
