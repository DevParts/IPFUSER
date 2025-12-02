using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class FullTextService : ScriptNameObjectBase, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 3, 4, 6, 7, 7, 7, 7, 7, 7, 7 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 7 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("AllowUnsignedBinaries", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CatalogUpgradeOption", expensive: false, readOnly: false, typeof(FullTextCatalogUpgradeOption)),
			new StaticMetadata("ConnectTimeout", expensive: false, readOnly: false, typeof(TimeSpan)),
			new StaticMetadata("DataTimeout", expensive: false, readOnly: false, typeof(TimeSpan)),
			new StaticMetadata("DefaultPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("LoadOSResourcesEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ResourceUsage", expensive: false, readOnly: false, typeof(ResourceUsage))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("ConnectTimeout", expensive: false, readOnly: false, typeof(TimeSpan)),
			new StaticMetadata("DefaultPath", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ResourceUsage", expensive: false, readOnly: false, typeof(ResourceUsage)),
			new StaticMetadata("DataTimeout", expensive: false, readOnly: false, typeof(TimeSpan)),
			new StaticMetadata("AllowUnsignedBinaries", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("LoadOSResourcesEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CatalogUpgradeOption", expensive: false, readOnly: false, typeof(FullTextCatalogUpgradeOption))
		};

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return -1;
				}
				return propertyName switch
				{
					"AllowUnsignedBinaries" => 0, 
					"CatalogUpgradeOption" => 1, 
					"ConnectTimeout" => 2, 
					"DataTimeout" => 3, 
					"DefaultPath" => 4, 
					"LoadOSResourcesEnabled" => 5, 
					"ResourceUsage" => 6, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ConnectTimeout" => 0, 
				"DefaultPath" => 1, 
				"ResourceUsage" => 2, 
				"DataTimeout" => 3, 
				"AllowUnsignedBinaries" => 4, 
				"LoadOSResourcesEnabled" => 5, 
				"CatalogUpgradeOption" => 6, 
				_ => -1, 
			};
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AllowUnsignedBinaries
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AllowUnsignedBinaries");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AllowUnsignedBinaries", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public FullTextCatalogUpgradeOption CatalogUpgradeOption
	{
		get
		{
			return (FullTextCatalogUpgradeOption)base.Properties.GetValueWithNullReplacement("CatalogUpgradeOption");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CatalogUpgradeOption", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public TimeSpan ConnectTimeout
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("ConnectTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConnectTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public TimeSpan DataTimeout
	{
		get
		{
			return (TimeSpan)base.Properties.GetValueWithNullReplacement("DataTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DefaultPath => (string)base.Properties.GetValueWithNullReplacement("DefaultPath");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool LoadOSResourcesEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("LoadOSResourcesEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LoadOSResourcesEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ResourceUsage ResourceUsage
	{
		get
		{
			return (ResourceUsage)base.Properties.GetValueWithNullReplacement("ResourceUsage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ResourceUsage", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	public static string UrnSuffix => "FullTextService";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal FullTextService(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	public void Cleanup()
	{
		try
		{
			if (base.ServerVersion.Major >= 7 && base.ServerVersion.Major <= 8)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add("EXEC master.dbo.sp_fulltext_service @action=N'clean_up'");
				ExecutionManager.ExecuteNonQuery(stringCollection);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Cleanup, this, ex);
		}
	}

	public void UpdateLanguageResources()
	{
		try
		{
			if (base.ServerVersion.Major >= 9)
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add("EXEC master.dbo.sp_fulltext_service @action=N'update_languages'");
				ExecutionManager.ExecuteNonQuery(stringCollection);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateLanguageResources, this, ex);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptService(queries, sp);
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			ScriptService(queries, sp);
		}
	}

	private void ScriptService(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		Property property;
		if (base.ServerVersion.Major >= 9)
		{
			if ((property = base.Properties.Get("AllowUnsignedBinaries")).Value != null && ((sp.ScriptForAlter && property.Dirty) || (!sp.ScriptForAlter && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'verify_signature', @value={0}", new object[1] { (!(bool)property.Value) ? 1 : 0 });
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if ((property = base.Properties.Get("LoadOSResourcesEnabled")).Value != null && ((sp.ScriptForAlter && property.Dirty) || (!sp.ScriptForAlter && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'load_os_resources', @value={0}", new object[1] { ((bool)property.Value) ? 1 : 0 });
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		if (base.ServerVersion.Major >= 7 && base.ServerVersion.Major <= 8 && (property = base.Properties.Get("ConnectTimeout")).Value != null && ((sp.ScriptForAlter && property.Dirty) || (!sp.ScriptForAlter && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version70 && sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)))
		{
			TimeSpan timeSpan = (TimeSpan)property.Value;
			if (timeSpan.TotalSeconds != 0.0 || sp.ScriptForAlter)
			{
				int num = 1;
				int num2 = 32767;
				if (timeSpan.TotalSeconds < (double)num || timeSpan.TotalSeconds > (double)num2)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidPropertyNumberRange("ConnectTimeout", num.ToString(SmoApplication.DefaultCulture), num2.ToString(SmoApplication.DefaultCulture)));
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'connect_timeout', @value={0}", new object[1] { Convert.ToInt32(timeSpan.TotalSeconds) });
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		if (base.ServerVersion.Major == 8 && (property = base.Properties.Get("DataTimeout")).Value != null && ((sp.ScriptForAlter && property.Dirty) || (!sp.ScriptForAlter && sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80)))
		{
			TimeSpan timeSpan2 = (TimeSpan)property.Value;
			if (timeSpan2.TotalSeconds != 0.0 || sp.ScriptForAlter)
			{
				int num3 = 1;
				int num4 = 32767;
				if (timeSpan2.TotalSeconds < (double)num3 || timeSpan2.TotalSeconds > (double)num4)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.InvalidPropertyNumberRange("DataTimeout", num3.ToString(SmoApplication.DefaultCulture), num4.ToString(SmoApplication.DefaultCulture)));
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'data_timeout', @value={0}", new object[1] { timeSpan2.TotalSeconds });
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		if ((property = base.Properties.Get("ResourceUsage")).Value != null && ((sp.ScriptForAlter && property.Dirty) || !sp.ScriptForAlter) && (int)property.Value > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'resource_usage', @value={0}", new object[1] { (int)property.Value });
			stringBuilder.Append(sp.NewLine);
			queries.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
		if (base.ServerVersion.Major < 10 || sp.TargetServerVersionInternal < SqlServerVersionInternal.Version100)
		{
			return;
		}
		property = base.Properties.Get("CatalogUpgradeOption");
		if (property.Value != null)
		{
			int num5 = (int)property.Value;
			if (!Enum.IsDefined(typeof(FullTextCatalogUpgradeOption), num5))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("CatalogUpgradeOption"));
			}
			if (!sp.ScriptForAlter || property.Dirty)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_fulltext_service @action=N'upgrade_option', @value={0}", new object[1] { num5 });
				stringBuilder.Append(sp.NewLine);
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
	}

	public DataTable EnumLanguages()
	{
		try
		{
			ThrowIfBelowVersion90();
			Request req = new Request(string.Concat(base.Urn, "/Language"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLanguages, this, ex);
		}
	}

	public DataTable EnumSemanticLanguages()
	{
		try
		{
			ThrowIfBelowVersion110();
			Request req = new Request(string.Concat(base.Urn, "/SemanticLanguage"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumSemanticLanguages, this, ex);
		}
	}
}
