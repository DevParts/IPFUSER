using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;
using Microsoft.SqlServer.Smo.UnSafeInternals;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadDoc : XmlRead
{
	private Stream m_fs;

	public XmlReadSettings Settings
	{
		get
		{
			if (!IsElementWithCheckVersion("settings"))
			{
				throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.MissingSection("settings"));
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
				throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.MissingSection("properties"));
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
		m_fs = ManagementUtil.LoadResourceFromAssembly(a, strFile);
		if (m_fs == null)
		{
			throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.FailedToLoadResFile(strFile));
		}
		LoadInternal();
	}

	public void LoadXml(Stream xmlStream)
	{
		m_fs = xmlStream;
		LoadInternal();
	}

	private void LoadInternal()
	{
		base.Reader = new XmlTextReader(m_fs)
		{
			DtdProcessing = DtdProcessing.Prohibit
		};
		if (XmlUtility.SelectNextElement(base.Reader))
		{
			if (!XmlUtility.IsElement(base.Reader, "EnumObject"))
			{
				throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.EnumObjectTagNotFound);
			}
			string text = base.Reader["cloud_min_major"];
			string text2 = base.Reader["min_major"];
			string text3 = base.Reader["datawarehouse_enabled"];
			if (string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text3))
			{
				throw new InternalEnumeratorException(StringSqlEnumerator.IncorrectVersionTag(base.Reader.ReadOuterXml()));
			}
			if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				bool result = false;
				if (text3 != null && !bool.TryParse(text3, out result))
				{
					throw new InvalidConfigurationFileEnumeratorException(string.Format(StringSqlEnumerator.InvalidAttributeValue, text3, "datawarehouse_enabled"));
				}
				if (!result)
				{
					throw new InvalidVersionEnumeratorException(StringSqlEnumerator.ObjectNotSupportedOnSqlDw);
				}
			}
			else
			{
				string empty = string.Empty;
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					text2 = text;
					empty = base.Reader["cloud_max_major"];
				}
				else
				{
					empty = base.Reader["max_major"];
				}
				if (string.IsNullOrEmpty(empty))
				{
					empty = int.MaxValue.ToString();
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = int.MaxValue.ToString();
				}
				if (int.Parse(text2, CultureInfo.InvariantCulture) > base.Version.Major || int.Parse(empty, CultureInfo.InvariantCulture) < base.Version.Major)
				{
					switch (base.Version.Major)
					{
					case 9:
						throw new InvalidVersionEnumeratorException(StringSqlEnumerator.InvalidSqlServer(StringSqlEnumerator.SqlServer90Name));
					case 8:
						throw new InvalidVersionEnumeratorException(StringSqlEnumerator.InvalidSqlServer(StringSqlEnumerator.SqlServer80Name));
					default:
						if (base.DatabaseEngineType == DatabaseEngineType.Standalone)
						{
							throw new InvalidVersionEnumeratorException(StringSqlEnumerator.InvalidVersion(base.Version.ToString()));
						}
						throw new InvalidVersionEnumeratorException(StringSqlEnumerator.InvalidVersion(base.Version.ToString() + " " + base.DatabaseEngineType));
					}
				}
			}
			if (XmlUtility.SelectNextElement(base.Reader))
			{
				return;
			}
		}
		throw new InvalidConfigurationFileEnumeratorException(StringSqlEnumerator.InvalidConfigurationFile);
	}

	public override void Close()
	{
		base.Reader = null;
		m_fs.Dispose();
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
