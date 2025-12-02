using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadDoc : XmlRead
{
	private Stream m_fs;

	public XmlReadSettings Settings
	{
		get
		{
			if (!IsElementWithCheckVersion("settings"))
			{
				throw new InvalidConfigurationFileEnumeratorException(SfcStrings.MissingSection("settings"));
			}
			return new XmlReadSettings(this);
		}
	}

	public XmlReadProperties Properties
	{
		get
		{
			if (!IsElementWithCheckVersion("properties") && !XmlUtility.IsElement(base.Reader, "properties"))
			{
				throw new InvalidConfigurationFileEnumeratorException(SfcStrings.MissingSection("properties"));
			}
			XmlUtility.SelectNextElement(base.Reader);
			return new XmlReadProperties(this);
		}
	}

	public XmlReadDoc(ServerVersion version, string serverAlias, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		: base(version, serverAlias, databaseEngineType, databaseEngineEdition)
	{
	}

	public void LoadFile(Assembly a, string strFile)
	{
		m_fs = a.GetManifestResourceStream(strFile);
		if (m_fs == null)
		{
			throw new InvalidConfigurationFileEnumeratorException(SfcStrings.FailedToLoadResFile(strFile));
		}
		base.Reader = new XmlTextReader(m_fs)
		{
			DtdProcessing = DtdProcessing.Prohibit
		};
		if (XmlUtility.SelectNextElement(base.Reader))
		{
			if (!XmlUtility.IsElement(base.Reader, "EnumObject"))
			{
				throw new InvalidConfigurationFileEnumeratorException(SfcStrings.EnumObjectTagNotFound);
			}
			string text = base.Reader["cloud_min_major"];
			string text2 = base.Reader["min_major"];
			string text3 = base.Reader["datawarehouse_enabled"];
			if (string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text3))
			{
				throw new InternalEnumeratorException(SfcStrings.IncorrectVersionTag(base.Reader.ReadOuterXml()));
			}
			if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				bool result = false;
				if (text3 != null && !bool.TryParse(text3, out result))
				{
					throw new InvalidConfigurationFileEnumeratorException(string.Format(SfcStrings.InvalidAttributeValue, text3, "datawarehouse_enabled"));
				}
				if (!result)
				{
					throw new InvalidVersionEnumeratorException(SfcStrings.ObjectNotSupportedOnSqlDw);
				}
			}
			else
			{
				DatabaseEngineType databaseEngineType = base.DatabaseEngineType;
				string text4;
				string text5;
				if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					text4 = text;
					text5 = base.Reader["cloud_max_major"];
				}
				else
				{
					text4 = text2;
					text5 = base.Reader["max_major"];
				}
				int num = (string.IsNullOrEmpty(text4) ? int.MaxValue : int.Parse(text4, CultureInfo.InvariantCulture));
				int num2 = (string.IsNullOrEmpty(text5) ? int.MaxValue : int.Parse(text5, CultureInfo.InvariantCulture));
				if (num > base.Version.Major || num2 < base.Version.Major)
				{
					switch (base.Version.Major)
					{
					case 9:
						throw new InvalidVersionEnumeratorException(SfcStrings.InvalidSqlServer(SfcStrings.SqlServer90Name));
					case 8:
						throw new InvalidVersionEnumeratorException(SfcStrings.InvalidSqlServer(SfcStrings.SqlServer80Name));
					default:
						throw new InvalidVersionEnumeratorException(SfcStrings.InvalidVersion(base.Version.ToString()));
					}
				}
			}
			if (XmlUtility.SelectNextElement(base.Reader))
			{
				return;
			}
		}
		throw new InvalidConfigurationFileEnumeratorException(SfcStrings.InvalidConfigurationFile);
	}

	public override void Close()
	{
		base.Reader = null;
		m_fs.Close();
	}

	public bool ReadUnion()
	{
		if (IsElementWithCheckVersion("union") && XmlUtility.SelectNextElement(base.Reader))
		{
			return true;
		}
		return false;
	}
}
