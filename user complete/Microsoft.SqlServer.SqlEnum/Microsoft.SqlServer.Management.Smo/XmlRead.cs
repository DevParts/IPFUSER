using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlRead
{
	protected const string ATTR_MIN_MAJOR = "min_major";

	protected const string ATTR_MAX_MAJOR = "max_major";

	protected const string ATTR_MIN_MINOR = "min_minor";

	protected const string ATTR_MAX_MINOR = "max_minor";

	protected const string ATTR_MIN_BUILD = "min_build";

	protected const string ATTR_MAX_BUILD = "max_build";

	protected const string ATTR_CLOUD_MIN_MAJOR = "cloud_min_major";

	protected const string ATTR_CLOUD_MAX_MAJOR = "cloud_max_major";

	protected const string ATTR_CLOUD_MIN_MINOR = "cloud_min_minor";

	protected const string ATTR_CLOUD_MAX_MINOR = "cloud_max_minor";

	protected const string ATTR_CLOUD_MIN_BUILD = "cloud_min_build";

	protected const string ATTR_CLOUD_MAX_BUILD = "cloud_max_build";

	protected const string ATTR_DATAWAREHOUSE_ENABLED = "datawarehouse_enabled";

	private XmlTextReader m_reader;

	private string m_alias;

	private bool m_closed;

	private ServerVersion m_version;

	private DatabaseEngineEdition m_DatabaseEngineEdition;

	private DatabaseEngineType m_databaseEngineType;

	protected XmlTextReader Reader
	{
		get
		{
			return m_reader;
		}
		set
		{
			m_reader = value;
		}
	}

	protected bool Closed
	{
		get
		{
			return m_closed;
		}
		set
		{
			m_closed = value;
		}
	}

	public DatabaseEngineType DatabaseEngineType
	{
		get
		{
			return m_databaseEngineType;
		}
		set
		{
			m_databaseEngineType = value;
		}
	}

	public ServerVersion Version
	{
		get
		{
			return m_version;
		}
		set
		{
			m_version = value;
		}
	}

	public DatabaseEngineEdition DatabaseEngineEdition
	{
		get
		{
			return m_DatabaseEngineEdition;
		}
		set
		{
			m_DatabaseEngineEdition = value;
		}
	}

	public string Alias
	{
		get
		{
			return m_alias;
		}
		set
		{
			m_alias = value;
		}
	}

	public XmlRead(XmlRead xmlReader)
	{
		Reader = xmlReader.Reader;
		Version = xmlReader.Version;
		DatabaseEngineType = xmlReader.DatabaseEngineType;
		Alias = xmlReader.Alias;
		DatabaseEngineEdition = xmlReader.DatabaseEngineEdition;
		Closed = false;
	}

	public XmlRead(ServerVersion version, string serverAlias, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		Reader = null;
		Version = version;
		Alias = serverAlias;
		Closed = false;
		DatabaseEngineType = databaseEngineType;
		DatabaseEngineEdition = databaseEngineEdition;
	}

	public XmlRead()
	{
		Closed = false;
	}

	public virtual void Close()
	{
		if (!Closed)
		{
			XmlUtility.SelectNextElement(Reader);
		}
	}

	public virtual void Skip()
	{
		if (!Closed)
		{
			XmlUtility.SelectNextElementOnLevel(Reader);
		}
	}

	protected bool IsElementWithCheckVersion(string elemName)
	{
		if (Version == null)
		{
			throw new InternalEnumeratorException(StringSqlEnumerator.NullVersionOnLoadingCfgFile);
		}
		while (XmlUtility.IsElement(Reader, "version"))
		{
			string text = Reader["min_major"];
			string text2 = Reader["cloud_min_major"];
			string text3 = Reader["datawarehouse_enabled"];
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3))
			{
				throw new InternalEnumeratorException(StringSqlEnumerator.IncorrectVersionTag(Reader.ReadOuterXml()));
			}
			bool flag = false;
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				bool result = false;
				if (text3 != null && !bool.TryParse(text3, out result))
				{
					throw new InvalidConfigurationFileEnumeratorException(string.Format(StringSqlEnumerator.InvalidAttributeValue, text3, "datawarehouse_enabled"));
				}
				flag = !result;
			}
			else
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				string empty3 = string.Empty;
				string empty4 = string.Empty;
				string empty5 = string.Empty;
				if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					text = text2;
					empty = Reader["cloud_max_major"];
					empty2 = Reader["cloud_min_minor"];
					empty3 = Reader["cloud_max_minor"];
					empty4 = Reader["cloud_min_build"];
					empty5 = Reader["cloud_max_build"];
				}
				else
				{
					empty = Reader["max_major"];
					empty2 = Reader["min_minor"];
					empty3 = Reader["max_minor"];
					empty4 = Reader["min_build"];
					empty5 = Reader["max_build"];
				}
				int major = (string.IsNullOrEmpty(text) ? int.MaxValue : int.Parse(text));
				int minor = ((!string.IsNullOrEmpty(empty2)) ? int.Parse(empty2) : 0);
				int build = ((!string.IsNullOrEmpty(empty4)) ? int.Parse(empty4) : 0);
				Version version = new Version(major, minor, build);
				int major2 = (string.IsNullOrEmpty(empty) ? int.MaxValue : int.Parse(empty));
				int minor2 = (string.IsNullOrEmpty(empty3) ? int.MaxValue : int.Parse(empty3));
				int build2 = (string.IsNullOrEmpty(empty5) ? int.MaxValue : int.Parse(empty5));
				Version version2 = new Version(major2, minor2, build2);
				Version version3 = new Version(Version.Major, Version.Minor, Version.BuildNumber);
				if (version3 < version || version2 < version3)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (!XmlUtility.SelectNextSibling(Reader))
				{
					XmlUtility.SelectNextElement(Reader);
					return false;
				}
			}
			else
			{
				XmlUtility.SelectNextElement(Reader);
			}
		}
		return XmlUtility.IsElement(Reader, elemName);
	}

	protected string GetAliasString(string str)
	{
		if (Alias == null || str == null)
		{
			return str;
		}
		return string.Format(CultureInfo.InvariantCulture, str, new object[1] { Alias });
	}

	protected static StringCollection GetFields(string fields)
	{
		string text = string.Empty;
		StringCollection stringCollection = new StringCollection();
		if (fields != null)
		{
			foreach (char c in fields)
			{
				if ('#' == c)
				{
					if (0 < text.Length)
					{
						stringCollection.Add(text);
					}
					text = string.Empty;
				}
				else
				{
					text += c;
				}
			}
		}
		return stringCollection;
	}

	protected string GetTextOfElement()
	{
		if (Reader.IsEmptyElement)
		{
			return null;
		}
		while (Reader.Read() && XmlNodeType.EndElement != Reader.NodeType && XmlNodeType.Element != Reader.NodeType)
		{
			if (XmlNodeType.Text == Reader.NodeType)
			{
				return Reader.Value;
			}
		}
		return null;
	}
}
