using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class PlanGuide : NamedSmoObject, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable, IExtendedProperties
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 9, 10, 10, 10, 10, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 9 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[9]
		{
			new StaticMetadata("Hints", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Parameters", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeBatch", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeObjectName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeSchemaName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeType", expensive: false, readOnly: false, typeof(PlanGuideType)),
			new StaticMetadata("Statement", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("Hints", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Parameters", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeBatch", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeObjectName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeSchemaName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ScopeType", expensive: false, readOnly: false, typeof(PlanGuideType)),
			new StaticMetadata("Statement", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
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
					"Hints" => 0, 
					"ID" => 1, 
					"IsDisabled" => 2, 
					"Parameters" => 3, 
					"ScopeBatch" => 4, 
					"ScopeObjectName" => 5, 
					"ScopeSchemaName" => 6, 
					"ScopeType" => 7, 
					"Statement" => 8, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Hints" => 0, 
				"ID" => 1, 
				"IsDisabled" => 2, 
				"Parameters" => 3, 
				"ScopeBatch" => 4, 
				"ScopeObjectName" => 5, 
				"ScopeSchemaName" => 6, 
				"ScopeType" => 7, 
				"Statement" => 8, 
				"PolicyHealthState" => 9, 
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

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Database;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Hints
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Hints");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Hints", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDisabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDisabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDisabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Parameters
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Parameters");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Parameters", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ScopeBatch
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ScopeBatch");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ScopeBatch", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ScopeObjectName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ScopeObjectName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ScopeObjectName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ScopeSchemaName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ScopeSchemaName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ScopeSchemaName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public PlanGuideType ScopeType
	{
		get
		{
			return (PlanGuideType)base.Properties.GetValueWithNullReplacement("ScopeType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ScopeType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Statement
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Statement");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Statement", value);
		}
	}

	public static string UrnSuffix => "PlanGuide";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion100();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	public PlanGuide()
	{
	}

	public PlanGuide(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[7] { "Hints", "Parameters", "ScopeBatch", "ScopeObjectName", "ScopeSchemaName", "ScopeType", "Statement" };
	}

	internal PlanGuide(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override void ValidateName(string name)
	{
		base.ValidateName(name);
		if (name.Length == 0)
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(name));
		}
		CheckPlanGuideName(name);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major > 9) ? ExtendedProperties : null, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		Property property = base.Properties.Get("IsDisabled");
		if (property.Dirty && property.Value != null)
		{
			bool flag = (bool)property.Value;
			alterQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_CONTROLPLANGUIDE_NAME, new object[2]
			{
				flag ? SqlSmoObject.MakeSqlString("DISABLE") : SqlSmoObject.MakeSqlString("ENABLE"),
				SqlSmoObject.MakeSqlString(SqlSmoObject.MakeSqlBraket(Name))
			}));
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, Name, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PLANGUIDE, new object[2]
			{
				"NOT",
				SqlSmoObject.MakeSqlString(Name)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SP_CREATEPLANGUIDE, new object[1] { SqlSmoObject.MakeSqlString(SqlSmoObject.MakeSqlBraket(Name)) });
		string text = (string)base.Properties.GetValueWithNullReplacement("Statement");
		if (text != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @stmt = {1}", new object[2]
			{
				Globals.comma,
				SqlSmoObject.MakeSqlString(text)
			});
			object valueWithNullReplacement = base.Properties.GetValueWithNullReplacement("ScopeType");
			string empty = string.Empty;
			if (valueWithNullReplacement != null)
			{
				empty = valueWithNullReplacement.ToString().ToUpperInvariant();
				PlanGuideType planGuideType = (PlanGuideType)valueWithNullReplacement;
				if (!Enum.IsDefined(typeof(PlanGuideType), planGuideType))
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("PlanGuideType"));
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @type = {1}", new object[2]
				{
					Globals.comma,
					SqlSmoObject.MakeSqlString(empty)
				});
				string text2 = string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				bool flag = false;
				bool flag2 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90;
				if (base.Properties.Get("ScopeObjectName").Value != null)
				{
					text2 = (string)base.Properties["ScopeObjectName"].Value;
				}
				if (base.Properties.Get("ScopeSchemaName").Value != null)
				{
					text3 = (string)base.Properties["ScopeSchemaName"].Value;
				}
				if (base.Properties.Get("ScopeBatch").Value != null)
				{
					text4 = (string)base.Properties["ScopeBatch"].Value;
					flag = base.Properties["ScopeBatch"].Dirty;
				}
				switch (planGuideType)
				{
				case PlanGuideType.Object:
					if (text4.Length > 0)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.PropertyNotValidException("ScopeBatch", "ScopeType", empty));
					}
					if (text2.Length > 0)
					{
						if (text3.Length > 0)
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @module_or_batch = N'{1}{2}{3}'", Globals.comma, SqlSmoObject.SqlString(SqlSmoObject.MakeSqlBraket(text3)), Globals.Dot, SqlSmoObject.SqlString(SqlSmoObject.MakeSqlBraket(text2)));
						}
						else
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @module_or_batch = {1}", new object[2]
							{
								Globals.comma,
								SqlSmoObject.MakeSqlString(SqlSmoObject.MakeSqlBraket(text2))
							});
						}
						break;
					}
					throw new PropertyNotSetException("ScopeObjectName");
				case PlanGuideType.Sql:
					if (text2.Length > 0 || text3.Length > 0)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.PropertiesNotValidException("ScopeObjectName and ScopeSchemaName", "ScopeType", empty));
					}
					if (text4.Length > 0)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @module_or_batch = {1}", new object[2]
						{
							Globals.comma,
							SqlSmoObject.MakeSqlString(text4)
						});
					}
					else if (flag2)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @module_or_batch = NULL", new object[1] { Globals.comma });
					}
					break;
				case PlanGuideType.Template:
					if ((flag && text4.Length > 0) || text2.Length > 0 || text3.Length > 0)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.PropertiesNotValidException("ScopeBatch, ScopeObjectName and ScopeSchemaName", "ScopeType", empty));
					}
					if (flag2)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @module_or_batch = NULL", new object[1] { Globals.comma });
					}
					break;
				}
				string text5 = (string)base.Properties.Get("Parameters").Value;
				if (text5 != null && 0 < text5.Length)
				{
					if (planGuideType == PlanGuideType.Object)
					{
						throw new WrongPropertyValueException(ExceptionTemplatesImpl.PropertyNotValidException("Parameters", "ScopeType", empty));
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @params = {1}", new object[2]
					{
						Globals.comma,
						SqlSmoObject.MakeSqlString(text5)
					});
				}
				else
				{
					if (planGuideType == PlanGuideType.Template)
					{
						throw new PropertyNotSetException("Parameters");
					}
					if (flag2)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @params = NULL", new object[1] { Globals.comma });
					}
				}
				string text6 = (string)base.Properties.Get("Hints").Value;
				if (text6 != null && 0 < text6.Length)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @hints = {1}", new object[2]
					{
						Globals.comma,
						SqlSmoObject.MakeSqlString(text6)
					});
				}
				else if (flag2)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} @hints = NULL", new object[1] { Globals.comma });
				}
				if (sp.IncludeScripts.ExistenceCheck)
				{
					stringBuilder.Append(sp.NewLine);
					stringBuilder.Append(Scripts.END);
					stringBuilder.Append(sp.NewLine);
				}
				createQuery.Add(stringBuilder.ToString());
				object value = base.Properties.Get("IsDisabled").Value;
				if (value != null && (bool)value)
				{
					createQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_CONTROLPLANGUIDE_NAME, new object[2]
					{
						SqlSmoObject.MakeSqlString("DISABLE"),
						SqlSmoObject.MakeSqlString(SqlSmoObject.MakeSqlBraket(Name))
					}));
				}
				return;
			}
			throw new PropertyNotSetException("ScopeType");
		}
		throw new PropertyNotSetException("Statement");
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, Name, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_PLANGUIDE, new object[2]
			{
				"",
				SqlSmoObject.MakeSqlString(Name)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SP_CONTROLPLANGUIDE_NAME, new object[2]
		{
			SqlSmoObject.MakeSqlString("DROP"),
			SqlSmoObject.MakeSqlString(SqlSmoObject.MakeSqlBraket(Name))
		});
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	private void CheckPlanGuideName(string planGuideName)
	{
		if (planGuideName.StartsWith("#", StringComparison.Ordinal))
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.PlanGuideNameCannotStartWithHash(planGuideName));
		}
	}

	public bool ValidatePlanGuide()
	{
		DataRow errorInfo;
		return ValidatePlanGuide(out errorInfo);
	}

	public bool ValidatePlanGuide(out DataRow errorInfo)
	{
		try
		{
			CheckObjectStateImpl(throwIfNotCreated: true);
			ThrowIfBelowVersion100();
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "select * from sys.fn_validate_plan_guide({0})", new object[1] { ID }));
			DataTable dataTable = ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
			errorInfo = null;
			bool result = true;
			if (dataTable.Rows.Count > 0)
			{
				errorInfo = dataTable.Rows[0];
				result = false;
			}
			return result;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PlanGuide, this, ex);
		}
	}
}
