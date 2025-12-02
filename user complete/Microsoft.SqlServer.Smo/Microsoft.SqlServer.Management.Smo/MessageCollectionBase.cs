using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class MessageCollectionBase : SortedListCollectionBase
{
	internal MessageCollectionBase(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoSortedList(new MessageObjectComparer(StringComparer));
	}

	public void Remove(int id)
	{
		Remove(new MessageObjectKey(id, GetDefaultLanguage()));
	}

	public void Remove(int id, string language)
	{
		Remove(new MessageObjectKey(id, language));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("ID");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("ID", urn.Type));
		}
		string text = urn.GetAttribute("Language");
		if (text == null || text.Length == 0)
		{
			text = GetDefaultLanguage();
		}
		return new MessageObjectKey(int.Parse(attribute, SmoApplication.DefaultCulture), text);
	}

	internal static string GetDefaultLanguage()
	{
		return "us_english";
	}

	public bool Contains(int id, string language)
	{
		return Contains(new MessageObjectKey(id, language));
	}

	public bool Contains(int id, int languageId)
	{
		Language language = base.ParentInstance.GetServerObject().Languages.ItemById(languageId);
		if (language != null)
		{
			return Contains(new MessageObjectKey(id, language.Name));
		}
		throw new SmoException(ExceptionTemplatesImpl.UnknownLanguageId(languageId.ToString(SmoApplication.DefaultCulture)));
	}
}
