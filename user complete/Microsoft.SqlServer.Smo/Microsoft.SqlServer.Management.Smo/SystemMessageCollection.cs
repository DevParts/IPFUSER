using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo;

public class SystemMessageCollection : MessageCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public SystemMessage this[int index] => GetObjectByIndex(index) as SystemMessage;

	public SystemMessage this[int id, string language] => GetObjectByKey(new MessageObjectKey(id, language)) as SystemMessage;

	internal SystemMessageCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public SystemMessage ItemByIdAndLanguage(int id, string language)
	{
		return this[id, language];
	}

	public SystemMessage ItemByIdAndLanguageId(int id, int languageId)
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
		return typeof(SystemMessage);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new SystemMessage(this, key, state);
	}

	public void CopyTo(SystemMessage[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}
}
