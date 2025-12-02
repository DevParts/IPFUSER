using System.Globalization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class SqlPropertyMetadataProvider : PropertyMetadataProvider
{
	protected int currentVersionIndex;

	protected DatabaseEngineType databaseEngineType = DatabaseEngineType.Standalone;

	protected DatabaseEngineEdition databaseEngineEdition;

	protected DatabaseEngineType DatabaseEngineType => databaseEngineType;

	protected DatabaseEngineEdition DatabaseEngineEdition => databaseEngineEdition;

	protected abstract int[] VersionCount { get; }

	public SqlPropertyMetadataProvider(ServerVersion sv, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		this.databaseEngineType = databaseEngineType;
		currentVersionIndex = PropertyMetadataProvider.GetCurrentVersionIndex(sv, databaseEngineType, databaseEngineEdition);
		this.databaseEngineEdition = databaseEngineEdition;
	}

	internal override int PropertyNameToIDLookupWithException(string propertyName, PropertyAccessPurpose pap)
	{
		int num = PropertyNameToIDLookup(propertyName);
		if (0 > num || num >= VersionCount[VersionCount.Length - 1])
		{
			throw new UnknownPropertyException(propertyName);
		}
		if (num >= VersionCount[currentVersionIndex])
		{
			throw new UnknownPropertyException(propertyName, GetExceptionText(propertyName, pap));
		}
		return num;
	}

	internal override bool TryPropertyNameToIDLookup(string propertyName, out int index)
	{
		index = PropertyNameToIDLookup(propertyName);
		if (0 > index || index >= VersionCount[VersionCount.Length - 1])
		{
			return false;
		}
		if (index >= VersionCount[currentVersionIndex])
		{
			return false;
		}
		return true;
	}

	protected string GetExceptionText(string propertyName, PropertyAccessPurpose pap)
	{
		string text = "";
		text = pap switch
		{
			PropertyAccessPurpose.Read => ExceptionTemplatesImpl.CannotReadProperty(propertyName), 
			PropertyAccessPurpose.Write => ExceptionTemplatesImpl.CannotWriteProperty(propertyName), 
			_ => ExceptionTemplatesImpl.CannotAccessProperty(propertyName), 
		};
		string serverNameFromVersionIndex = GetServerNameFromVersionIndex(currentVersionIndex);
		if (!string.IsNullOrEmpty(serverNameFromVersionIndex))
		{
			text = string.Format(CultureInfo.CurrentUICulture, "{0} {1} {2}.", new object[3]
			{
				text,
				ExceptionTemplatesImpl.PropertyAvailable,
				serverNameFromVersionIndex
			});
		}
		return text;
	}

	private string GetServerNameFromVersionIndex(int index)
	{
		string result = string.Empty;
		if (databaseEngineType == DatabaseEngineType.Standalone)
		{
			switch (index)
			{
			case 0:
				result = LocalizableResources.ServerSphinx;
				break;
			case 1:
				result = LocalizableResources.ServerShiloh;
				break;
			case 2:
				result = LocalizableResources.ServerYukon;
				break;
			case 3:
				result = LocalizableResources.ServerKatmai;
				break;
			case 4:
				result = LocalizableResources.ServerKilimanjaro;
				break;
			case 5:
				result = LocalizableResources.ServerDenali;
				break;
			case 6:
				result = LocalizableResources.ServerSQL14;
				break;
			case 7:
				result = LocalizableResources.ServerSQL15;
				break;
			case 8:
				result = LocalizableResources.ServerSQL2017;
				break;
			case 9:
				result = LocalizableResources.ServerSQLv150;
				break;
			default:
				TraceHelper.Assert(condition: false, string.Format(CultureInfo.InvariantCulture, "Unknown server version index {0}", new object[1] { index }));
				break;
			}
		}
		else if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			result = ((DatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse) ? LocalizableResources.EngineCloud : LocalizableResources.EngineDatawarehouse);
		}
		else
		{
			TraceHelper.Assert(condition: false, string.Format(CultureInfo.InvariantCulture, "Unknown DatabaseEngineType {0}", new object[1] { databaseEngineType }));
		}
		return result;
	}

	private SqlServerVersions GetSupportedVersions(int index)
	{
		TraceHelper.Assert(index >= 0 && index < VersionCount[VersionCount.Length - 1], "SuportedVersions: index out of range");
		if (index < 0 || index >= VersionCount[VersionCount.Length - 1])
		{
			return SqlServerVersions.Unknown;
		}
		SqlServerVersions sqlServerVersions = SqlServerVersions.Version150;
		if (index >= VersionCount[8])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version140;
		if (index >= VersionCount[7])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version130;
		if (index >= VersionCount[6])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version120;
		if (index >= VersionCount[5])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version110;
		if (index >= VersionCount[4])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version105;
		if (index >= VersionCount[3])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version100;
		if (index >= VersionCount[2])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version90;
		if (index >= VersionCount[1])
		{
			return sqlServerVersions;
		}
		sqlServerVersions |= SqlServerVersions.Version80;
		if (index >= VersionCount[0])
		{
			return sqlServerVersions;
		}
		return sqlServerVersions | SqlServerVersions.Version70;
	}

	internal SqlPropertyInfo GetPropertyInfo(int index)
	{
		return new SqlPropertyInfo(GetStaticMetadata(index), GetSupportedVersions(index));
	}

	private int GetCountForVersions(SqlServerVersions m_versions)
	{
		if ((m_versions | SqlServerVersions.Version150) == SqlServerVersions.Version150)
		{
			return VersionCount[9];
		}
		if ((m_versions | SqlServerVersions.Version140) == SqlServerVersions.Version140)
		{
			return VersionCount[8];
		}
		if ((m_versions | SqlServerVersions.Version130) == SqlServerVersions.Version130)
		{
			return VersionCount[7];
		}
		if ((m_versions | SqlServerVersions.Version120) == SqlServerVersions.Version120)
		{
			return VersionCount[6];
		}
		if ((m_versions | SqlServerVersions.Version110) == SqlServerVersions.Version110)
		{
			return VersionCount[5];
		}
		if ((m_versions | SqlServerVersions.Version105) == SqlServerVersions.Version105)
		{
			return VersionCount[4];
		}
		if ((m_versions | SqlServerVersions.Version100) == SqlServerVersions.Version100)
		{
			return VersionCount[3];
		}
		if ((m_versions | SqlServerVersions.Version90) == SqlServerVersions.Version90)
		{
			return VersionCount[2];
		}
		if ((m_versions | SqlServerVersions.Version80) == SqlServerVersions.Version80)
		{
			return VersionCount[1];
		}
		if ((m_versions | SqlServerVersions.Version70) == SqlServerVersions.Version70)
		{
			return VersionCount[0];
		}
		return 0;
	}

	internal SqlPropertyInfo[] EnumPropertyInfo(SqlServerVersions versions)
	{
		int countForVersions = GetCountForVersions(versions);
		SqlPropertyInfo[] array = new SqlPropertyInfo[countForVersions];
		for (int i = 0; i < countForVersions; i++)
		{
			array[i] = GetPropertyInfo(i);
		}
		return array;
	}
}
