using System;
using System.Globalization;
using System.Reflection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class PropertyMetadataProvider
{
	protected enum StandaloneVersionIndex
	{
		v70,
		v80,
		v90,
		v100,
		v105,
		v110,
		v120,
		v130,
		v140,
		v150
	}

	protected enum CloudVersionIndex
	{
		v100,
		v110,
		v120
	}

	internal static int[] defaultSingletonArray;

	public abstract int Count { get; }

	public abstract int PropertyNameToIDLookup(string propertyName);

	public abstract StaticMetadata GetStaticMetadata(int id);

	internal static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		return new StaticMetadata[0];
	}

	internal static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		return defaultSingletonArray;
	}

	internal virtual int PropertyNameToIDLookupWithException(string propertyName, PropertyAccessPurpose pap)
	{
		int num = PropertyNameToIDLookup(propertyName);
		if (0 > num || num >= Count)
		{
			throw new UnknownPropertyException(propertyName);
		}
		return num;
	}

	internal int PropertyNameToIDLookupWithException(string propertyName)
	{
		return PropertyNameToIDLookupWithException(propertyName, PropertyAccessPurpose.Unknown);
	}

	internal virtual bool TryPropertyNameToIDLookup(string propertyName, out int index)
	{
		index = PropertyNameToIDLookup(propertyName);
		if (index < 0)
		{
			return false;
		}
		return true;
	}

	internal static bool CheckPropertyValid(Type type, string propertyName, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		MethodInfo method = type.GetMethod("GetStaticMetadataArray", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		StaticMetadata[] array = new StaticMetadata[0];
		if (method != null)
		{
			array = method.Invoke(null, new object[2] { databaseEngineType, databaseEngineEdition }) as StaticMetadata[];
		}
		int num = Array.FindIndex(array, new StaticMetadata(propertyName).Match);
		method = type.GetMethod("GetVersionArray", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		int[] array2 = new int[0];
		if (method != null)
		{
			array2 = method.Invoke(null, new object[2] { databaseEngineType, databaseEngineEdition }) as int[];
		}
		int currentVersionIndex = GetCurrentVersionIndex(version, databaseEngineType, databaseEngineEdition);
		if (0 > num || array2.Length < currentVersionIndex || num >= array2[currentVersionIndex])
		{
			return false;
		}
		return true;
	}

	internal static int GetCurrentVersionIndex(ServerVersion sv, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		int result = 0;
		switch (databaseEngineType)
		{
		case DatabaseEngineType.Standalone:
			switch (sv.Major)
			{
			case 7:
				result = 0;
				break;
			case 8:
				result = 1;
				break;
			case 9:
				result = 2;
				break;
			case 10:
				if (sv.Minor == 0)
				{
					result = 3;
				}
				else if (sv.Minor == 50)
				{
					result = 4;
				}
				break;
			case 11:
				result = 5;
				break;
			case 12:
				result = 6;
				break;
			case 13:
				result = 7;
				break;
			case 14:
				result = 8;
				break;
			case 15:
				result = 9;
				break;
			default:
				result = 9;
				break;
			}
			break;
		case DatabaseEngineType.SqlAzureDatabase:
			result = ((databaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse) ? (sv.Major switch
			{
				10 => 0, 
				11 => 1, 
				12 => 2, 
				_ => 2, 
			}) : 0);
			break;
		default:
			TraceHelper.Assert(condition: false, string.Format(CultureInfo.InvariantCulture, "Unknown DatabaseEngineType {0} when getting current version index", new object[1] { databaseEngineType.ToString() }));
			break;
		}
		return result;
	}

	static PropertyMetadataProvider()
	{
		int[] array = new int[7];
		defaultSingletonArray = array;
	}
}
