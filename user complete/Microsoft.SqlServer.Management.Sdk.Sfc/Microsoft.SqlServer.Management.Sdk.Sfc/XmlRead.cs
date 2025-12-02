using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlRead
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

	private DatabaseEngineType m_databaseEngineType;

	private DatabaseEngineEdition m_databaseEngineEdition;

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

	public DatabaseEngineEdition DatabaseEngineEdition
	{
		get
		{
			return m_databaseEngineEdition;
		}
		set
		{
			m_databaseEngineEdition = value;
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
		DatabaseEngineEdition = xmlReader.DatabaseEngineEdition;
		Alias = xmlReader.Alias;
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
			throw new InternalEnumeratorException(SfcStrings.NullVersionOnLoadingCfgFile);
		}
		while (XmlUtility.IsElement(Reader, "version"))
		{
			string text = Reader["min_major"];
			string text2 = Reader["cloud_min_major"];
			string text3 = Reader["datawarehouse_enabled"];
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3))
			{
				throw new InternalEnumeratorException(SfcStrings.IncorrectVersionTag(Reader.ReadOuterXml()));
			}
			bool flag = false;
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				bool result = false;
				if (text3 != null && !bool.TryParse(text3, out result))
				{
					throw new InvalidConfigurationFileEnumeratorException(string.Format(SfcStrings.InvalidAttributeValue, text3, "datawarehouse_enabled"));
				}
				flag = !result;
			}
			else
			{
				DatabaseEngineType databaseEngineType = DatabaseEngineType;
				string text4;
				string text5;
				string text6;
				string text7;
				string text8;
				string text9;
				if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					text4 = text2;
					text5 = Reader["cloud_max_major"];
					text6 = Reader["cloud_min_minor"];
					text7 = Reader["cloud_max_minor"];
					text8 = Reader["cloud_min_build"];
					text9 = Reader["cloud_max_build"];
				}
				else
				{
					text4 = text;
					text5 = Reader["max_major"];
					text6 = Reader["min_minor"];
					text7 = Reader["max_minor"];
					text8 = Reader["min_build"];
					text9 = Reader["max_build"];
				}
				int major = (string.IsNullOrEmpty(text4) ? int.MaxValue : int.Parse(text4));
				int minor = ((!string.IsNullOrEmpty(text6)) ? int.Parse(text6) : 0);
				int build = ((!string.IsNullOrEmpty(text8)) ? int.Parse(text8) : 0);
				Version version = new Version(major, minor, build);
				int major2 = (string.IsNullOrEmpty(text5) ? int.MaxValue : int.Parse(text5));
				int minor2 = (string.IsNullOrEmpty(text7) ? int.MaxValue : int.Parse(text7));
				int build2 = (string.IsNullOrEmpty(text9) ? int.MaxValue : int.Parse(text9));
				Version version2 = new Version(major2, minor2, build2);
				Version version3 = new Version(Version.Major, Version.Minor, Version.BuildNumber);
				flag = version3 < version || version2 < version3;
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
