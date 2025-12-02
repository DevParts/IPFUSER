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
public sealed class ExternalFileFormat : NamedSmoObject, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 10 };

		private static int sqlDwPropertyCount = 11;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateFormat", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Encoding", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FieldTerminator", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FirstRow", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("FormatType", expensive: false, readOnly: false, typeof(ExternalFileFormatType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RowTerminator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SerDeMethod", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StringDelimiter", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UseTypeDefault", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateFormat", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Encoding", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FieldTerminator", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FormatType", expensive: false, readOnly: false, typeof(ExternalFileFormatType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RowTerminator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SerDeMethod", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StringDelimiter", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UseTypeDefault", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateFormat", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Encoding", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("FieldTerminator", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FormatType", expensive: false, readOnly: false, typeof(ExternalFileFormatType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("RowTerminator", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SerDeMethod", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("StringDelimiter", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UseTypeDefault", expensive: false, readOnly: false, typeof(bool))
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
					return propertyName switch
					{
						"DataCompression" => 0, 
						"DateFormat" => 1, 
						"Encoding" => 2, 
						"FieldTerminator" => 3, 
						"FirstRow" => 4, 
						"FormatType" => 5, 
						"ID" => 6, 
						"RowTerminator" => 7, 
						"SerDeMethod" => 8, 
						"StringDelimiter" => 9, 
						"UseTypeDefault" => 10, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"DataCompression" => 0, 
					"DateFormat" => 1, 
					"Encoding" => 2, 
					"FieldTerminator" => 3, 
					"FormatType" => 4, 
					"ID" => 5, 
					"RowTerminator" => 6, 
					"SerDeMethod" => 7, 
					"StringDelimiter" => 8, 
					"UseTypeDefault" => 9, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DataCompression" => 0, 
				"DateFormat" => 1, 
				"Encoding" => 2, 
				"FieldTerminator" => 3, 
				"FormatType" => 4, 
				"ID" => 5, 
				"RowTerminator" => 6, 
				"SerDeMethod" => 7, 
				"StringDelimiter" => 8, 
				"UseTypeDefault" => 9, 
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

	private const string FirstRowName = "FirstRow";

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DataCompression
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DataCompression");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataCompression", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DateFormat
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DateFormat");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DateFormat", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Encoding => (string)base.Properties.GetValueWithNullReplacement("Encoding");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FieldTerminator
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FieldTerminator");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FieldTerminator", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ExternalFileFormatType FormatType
	{
		get
		{
			return (ExternalFileFormatType)base.Properties.GetValueWithNullReplacement("FormatType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FormatType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string RowTerminator => (string)base.Properties.GetValueWithNullReplacement("RowTerminator");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SerDeMethod
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("SerDeMethod");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SerDeMethod", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string StringDelimiter
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("StringDelimiter");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StringDelimiter", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool UseTypeDefault
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("UseTypeDefault");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UseTypeDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation)]
	public int FirstRow
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("FirstRow");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FirstRow", value);
		}
	}

	public static string UrnSuffix => "ExternalFileFormat";

	public ExternalFileFormat()
	{
	}

	public ExternalFileFormat(Database database, string name)
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
		return new string[1] { "FirstRow" };
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	internal ExternalFileFormat(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ExternalFileFormat(Database parent, string name, ExternalFileFormatType formatType)
	{
		Parent = parent;
		base.Name = name;
		FormatType = formatType;
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(typeof(ExternalFileFormat), sp);
		string text = FormatFullNameForScripting(sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_EXTERNAL_FILE_FORMAT, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("DROP EXTERNAL FILE FORMAT " + text);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(typeof(ExternalFileFormat), sp);
		ExternalFileFormatType externalFileFormatType = ExternalFileFormatType.DelimitedText;
		ValidateProperty("FormatType", sp);
		if (IsSupportedProperty("FormatType", sp))
		{
			externalFileFormatType = (ExternalFileFormatType)GetPropValue("FormatType");
			if (!Enum.IsDefined(typeof(ExternalFileFormatType), externalFileFormatType))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration(externalFileFormatType.ToString()));
			}
		}
		string text = FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_EXTERNAL_FILE_FORMAT, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(ExternalFileFormatType));
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE EXTERNAL FILE FORMAT {0} WITH ", new object[1] { text });
		stringBuilder.Append(Globals.LParen);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FORMAT_TYPE = {0}", new object[1] { typeConverter.ConvertToInvariantString(externalFileFormatType) });
		CheckConflictingProperties(sp);
		ProcessOptionalProperties(externalFileFormatType, stringBuilder, sp);
		stringBuilder.Append(Globals.RParen);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	private void AddPropertyToScript(string propertyValue, string sqlString, StringBuilder fileFormatOptions)
	{
		if (fileFormatOptions.Length > 0)
		{
			fileFormatOptions.Append(", ");
		}
		fileFormatOptions.AppendFormat(SmoApplication.DefaultCulture, sqlString, new object[1] { propertyValue });
	}

	private void CheckConflictingProperties(ScriptingPreferences sp)
	{
		if (!IsSupportedProperty("FormatType", sp))
		{
			return;
		}
		Property propertyOptional = GetPropertyOptional("FormatType");
		if (propertyOptional.IsNull)
		{
			return;
		}
		Property property = null;
		if (((ExternalFileFormatType)propertyOptional.Value == ExternalFileFormatType.DelimitedText || (ExternalFileFormatType)propertyOptional.Value == ExternalFileFormatType.Orc || (ExternalFileFormatType)propertyOptional.Value == ExternalFileFormatType.Parquet) && IsSupportedProperty("SerDeMethod", sp))
		{
			property = GetPropertyOptional("SerDeMethod");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (string)property.Value, new List<string>
			{
				null,
				string.Empty
			}))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
		if ((ExternalFileFormatType)propertyOptional.Value != ExternalFileFormatType.RcFile && (ExternalFileFormatType)propertyOptional.Value != ExternalFileFormatType.Orc && (ExternalFileFormatType)propertyOptional.Value != ExternalFileFormatType.Parquet)
		{
			return;
		}
		if (IsSupportedProperty("FieldTerminator", sp))
		{
			property = GetPropertyOptional("FieldTerminator");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (string)property.Value, new List<string>
			{
				null,
				string.Empty
			}))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
		if (IsSupportedProperty("StringDelimiter", sp))
		{
			property = GetPropertyOptional("StringDelimiter");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (string)property.Value, new List<string>
			{
				null,
				string.Empty
			}))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
		if (IsSupportedProperty("DateFormat", sp))
		{
			property = GetPropertyOptional("DateFormat");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (string)property.Value, new List<string>
			{
				null,
				string.Empty
			}))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
		if (IsSupportedProperty("UseTypeDefault", sp))
		{
			property = GetPropertyOptional("UseTypeDefault");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (bool)property.Value, new List<bool> { false }))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
		if (IsSupportedProperty("FirstRow", sp))
		{
			property = GetPropertyOptional("FirstRow");
			if (!property.IsNull && !IsPropertyDefaultValue(property, (int)property.Value, new List<int> { 0 }))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingExternalFileFormatProperties, new object[3]
				{
					property.Name,
					property.Value.ToString(),
					propertyOptional.Value.ToString()
				}));
			}
		}
	}

	private bool IsPropertyDefaultValue<T>(Property prop, T value, List<T> defaultValues)
	{
		if (!prop.IsNull)
		{
			foreach (T defaultValue in defaultValues)
			{
				if (EqualityComparer<T>.Default.Equals((T)prop.Value, defaultValue))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ProcessOptionalProperties(ExternalFileFormatType externalFileFormatType, StringBuilder script, ScriptingPreferences sp)
	{
		switch (externalFileFormatType)
		{
		case ExternalFileFormatType.DelimitedText:
			ValidateDelimitedTextProperties(script, sp);
			break;
		case ExternalFileFormatType.Orc:
		case ExternalFileFormatType.Parquet:
			ValidateOrcOrParquetProperties(script, sp);
			break;
		case ExternalFileFormatType.RcFile:
			ValidateRcFileProperties(script, sp);
			break;
		default:
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration(externalFileFormatType.ToString()));
		}
	}

	private void ValidateDelimitedTextProperties(StringBuilder script, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		List<string> list = new List<string>();
		list.Add(null);
		list.Add(string.Empty);
		List<string> defaultValues = list;
		ValidateOptionalProperty("FieldTerminator", "FIELD_TERMINATOR = {0}", defaultValues, stringBuilder, sp);
		ValidateOptionalProperty("StringDelimiter", "STRING_DELIMITER = {0}", defaultValues, stringBuilder, sp);
		ValidateOptionalProperty("DateFormat", "DATE_FORMAT = {0}", defaultValues, stringBuilder, sp);
		ValidateOptionalProperty("FirstRow", "FIRST_ROW = {0}", new List<int> { 1 }, stringBuilder, sp, quotePropertyValue: false);
		if (IsSupportedProperty("UseTypeDefault", sp) && !GetPropertyOptional("UseTypeDefault").IsNull)
		{
			bool flag = (bool)GetPropValueOptional("UseTypeDefault");
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "USE_TYPE_DEFAULT = {0}", new object[1] { flag });
		}
		string text = stringBuilder.ToString();
		if (!string.IsNullOrEmpty(text))
		{
			script.AppendFormat(SmoApplication.DefaultCulture, ", FORMAT_OPTIONS ({0})", new object[1] { text });
		}
		ValidateOptionalProperty("DataCompression", "DATA_COMPRESSION = {0}", defaultValues, script, sp);
	}

	private void ValidateOptionalProperty<T>(string propertyName, string sqlString, List<T> defaultValues, StringBuilder fileFormatOptions, ScriptingPreferences sp, bool quotePropertyValue = true)
	{
		if (IsSupportedProperty(propertyName, sp))
		{
			Property propertyOptional = GetPropertyOptional(propertyName);
			if (!propertyOptional.IsNull && !IsPropertyDefaultValue(propertyOptional, (T)propertyOptional.Value, defaultValues))
			{
				AddPropertyToScript(quotePropertyValue ? Util.MakeSqlString(Convert.ToString(propertyOptional.Value, SmoApplication.DefaultCulture)) : propertyOptional.Value.ToString(), sqlString, fileFormatOptions);
			}
		}
	}

	private void ValidateOrcOrParquetProperties(StringBuilder script, ScriptingPreferences sp)
	{
		List<string> list = new List<string>();
		list.Add(null);
		list.Add(string.Empty);
		List<string> defaultValues = list;
		ValidateOptionalProperty("DataCompression", "DATA_COMPRESSION = {0}", defaultValues, script, sp);
	}

	private void ValidateProperty(string propertyName, ScriptingPreferences sp)
	{
		if (IsSupportedProperty(propertyName, sp) && GetPropertyOptional(propertyName).IsNull)
		{
			throw new ArgumentNullException(propertyName);
		}
	}

	private void ValidateRcFileProperties(StringBuilder script, ScriptingPreferences sp)
	{
		List<string> list = new List<string>();
		list.Add(null);
		list.Add(string.Empty);
		List<string> defaultValues = list;
		ValidateProperty("SerDeMethod", sp);
		ValidateOptionalProperty("SerDeMethod", "SERDE_METHOD = {0}", defaultValues, script, sp);
		ValidateOptionalProperty("DataCompression", "DATA_COMPRESSION = {0}", defaultValues, script, sp);
	}
}
