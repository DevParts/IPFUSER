using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class MessageObjectKey : ObjectKeyBase
{
	private int messageID;

	private string language;

	internal static StringCollection fields;

	public int ID
	{
		get
		{
			return messageID;
		}
		set
		{
			messageID = value;
		}
	}

	public string Language
	{
		get
		{
			return language;
		}
		set
		{
			language = value;
		}
	}

	public override string UrnFilter
	{
		get
		{
			if (language != null && language.Length > 0)
			{
				return string.Format(SmoApplication.DefaultCulture, "@ID={0} and @Language='{1}'", new object[2]
				{
					messageID,
					Urn.EscapeString(language)
				});
			}
			return string.Format(SmoApplication.DefaultCulture, "@ID={0}", new object[1] { messageID });
		}
	}

	public override bool IsNull
	{
		get
		{
			if (messageID != 0)
			{
				return null == language;
			}
			return true;
		}
	}

	public MessageObjectKey(int messageID, string language)
	{
		this.messageID = messageID;
		this.language = language;
	}

	static MessageObjectKey()
	{
		fields = new StringCollection();
		fields.Add("ID");
		fields.Add("Language");
	}

	public override StringCollection GetFieldNames()
	{
		return fields;
	}

	public override string ToString()
	{
		if (language != null)
		{
			return string.Format(SmoApplication.DefaultCulture, "{0}:'{1}'", new object[2]
			{
				messageID,
				SqlSmoObject.SqlString(language)
			});
		}
		return messageID.ToString(SmoApplication.DefaultCulture);
	}

	public override ObjectKeyBase Clone()
	{
		return new MessageObjectKey(ID, Language);
	}

	internal override void Validate(Type objectType)
	{
	}

	public override ObjectComparerBase GetComparer(IComparer stringComparer)
	{
		return new MessageObjectComparer(stringComparer);
	}
}
