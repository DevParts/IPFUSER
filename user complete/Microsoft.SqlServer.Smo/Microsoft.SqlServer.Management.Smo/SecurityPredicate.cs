using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class SecurityPredicate : SqlSmoObject, IAlterable, ICreatable, IDroppable, IDropIfExists, IMarkForDrop
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 7, 7, 7 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 7 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("PredicateDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PredicateOperation", expensive: false, readOnly: false, typeof(SecurityPredicateOperation)),
			new StaticMetadata("PredicateType", expensive: false, readOnly: false, typeof(SecurityPredicateType)),
			new StaticMetadata("SecurityPredicateID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TargetObjectID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TargetObjectName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("TargetObjectSchema", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("PredicateDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PredicateOperation", expensive: false, readOnly: false, typeof(SecurityPredicateOperation)),
			new StaticMetadata("PredicateType", expensive: false, readOnly: false, typeof(SecurityPredicateType)),
			new StaticMetadata("SecurityPredicateID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TargetObjectID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TargetObjectName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("TargetObjectSchema", expensive: false, readOnly: false, typeof(string))
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
					"PredicateDefinition" => 0, 
					"PredicateOperation" => 1, 
					"PredicateType" => 2, 
					"SecurityPredicateID" => 3, 
					"TargetObjectID" => 4, 
					"TargetObjectName" => 5, 
					"TargetObjectSchema" => 6, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"PredicateDefinition" => 0, 
				"PredicateOperation" => 1, 
				"PredicateType" => 2, 
				"SecurityPredicateID" => 3, 
				"TargetObjectID" => 4, 
				"TargetObjectName" => 5, 
				"TargetObjectSchema" => 6, 
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
	public SecurityPolicy Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as SecurityPolicy;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SecurityPredicateOperation PredicateOperation
	{
		get
		{
			return (SecurityPredicateOperation)base.Properties.GetValueWithNullReplacement("PredicateOperation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PredicateOperation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SecurityPredicateType PredicateType
	{
		get
		{
			return (SecurityPredicateType)base.Properties.GetValueWithNullReplacement("PredicateType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PredicateType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int SecurityPredicateID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("SecurityPredicateID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SecurityPredicateID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int TargetObjectID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("TargetObjectID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TargetObjectID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TargetObjectName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("TargetObjectName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TargetObjectName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TargetObjectSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("TargetObjectSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TargetObjectSchema", value);
		}
	}

	public static string UrnSuffix => "SecurityPredicate";

	internal string FullQualifiedTargetName => $"[{SqlSmoObject.SqlBraket(TargetObjectSchema)}].[{SqlSmoObject.SqlBraket(TargetObjectName)}]";

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string PredicateDefinition
	{
		get
		{
			string text = (string)base.Properties.GetValueWithNullReplacement("PredicateDefinition");
			if (text != null)
			{
				while (text[0] == '(' && text[text.Length - 1] == ')')
				{
					text = text.Substring(1, text.Length - 2);
				}
			}
			return text;
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PredicateDefinition", value);
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[6] { "PredicateOperation", "PredicateType", "SecurityPredicateID", "TargetObjectID", "TargetObjectName", "TargetObjectSchema" };
	}

	internal SecurityPredicate(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public SecurityPredicate(SecurityPolicy parent, Table table, string predicateDefinition)
		: this(parent, table.Schema, table.Name, table.ID, predicateDefinition)
	{
	}

	public SecurityPredicate(SecurityPolicy parent, string targetObjectSchema, string targetObjectName, int targetObjectId, string predicateDefinition)
	{
		SetParentImpl(parent);
		if (parent.SecurityPredicates.Count == 0)
		{
			SecurityPredicateID = 1;
		}
		else
		{
			SecurityPredicateID = parent.SecurityPredicates[parent.SecurityPredicates.Count - 1].SecurityPredicateID + 1;
		}
		key = new SecurityPredicateObjectKey(SecurityPredicateID);
		TargetObjectName = targetObjectName;
		TargetObjectSchema = targetObjectSchema;
		TargetObjectID = targetObjectId;
		PredicateDefinition = predicateDefinition;
		PredicateType = SecurityPredicateType.Filter;
		PredicateOperation = SecurityPredicateOperation.All;
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
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
		string fullQualifiedName = Parent.FullQualifiedName;
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("SecurityPolicy", fullQualifiedName, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SECURITY_PREDICATE, new object[3]
			{
				string.Empty,
				TargetObjectID,
				Parent.ID
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SECURITY POLICY {0}", new object[1] { fullQualifiedName });
		ScriptPredicate(stringBuilder, sp);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		string fullQualifiedName = Parent.FullQualifiedName;
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("SecurityPolicy", fullQualifiedName, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		Convert.ToInt32(GetPropValue("TargetObjectID"));
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SECURITY_PREDICATE, new object[3] { "NOT", TargetObjectID, Parent.ID });
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SECURITY POLICY {0}", new object[1] { fullQualifiedName });
		ScriptPredicate(stringBuilder, sp, forCreate: true);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty() && base.State != SqlSmoState.Creating)
		{
			string fullQualifiedName = Parent.FullQualifiedName;
			StringBuilder stringBuilder = new StringBuilder();
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader("SecurityPolicy", fullQualifiedName, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SECURITY POLICY {0}", new object[1] { fullQualifiedName });
			ScriptPredicate(stringBuilder, sp);
			alterQuery.Add(stringBuilder.ToString());
		}
	}

	internal void ScriptPredicate(StringBuilder sb, ScriptingPreferences sp, bool forCreate = false)
	{
		TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(SecurityPredicateType));
		TypeConverter typeConverter2 = SmoManagementUtil.GetTypeConverter(typeof(SecurityPredicateOperation));
		string arg = typeConverter.ConvertToInvariantString(PredicateType);
		string text = typeConverter2.ConvertToInvariantString(PredicateOperation);
		string predicateDefinition = PredicateDefinition;
		string fullQualifiedTargetName = FullQualifiedTargetName;
		sb.Append(Globals.newline);
		if (base.State == SqlSmoState.Creating || forCreate)
		{
			sb.AppendFormat("ADD {0} PREDICATE {1} ON {2}", arg, predicateDefinition, fullQualifiedTargetName);
		}
		else if (base.State == SqlSmoState.ToBeDropped || sp.Behavior == ScriptBehavior.Drop || sp.Behavior == ScriptBehavior.DropAndCreate)
		{
			sb.AppendFormat("DROP {0} PREDICATE ON {1}", arg, fullQualifiedTargetName);
		}
		else
		{
			sb.AppendFormat("ALTER {0} PREDICATE {1} ON {2}", arg, predicateDefinition, fullQualifiedTargetName);
		}
		if (text != string.Empty)
		{
			sb.AppendFormat(" {0}", text);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[6] { "TargetObjectName", "TargetObjectSchema", "TargetObjectID", "PredicateDefinition", "PredicateType", "PredicateOperation" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
