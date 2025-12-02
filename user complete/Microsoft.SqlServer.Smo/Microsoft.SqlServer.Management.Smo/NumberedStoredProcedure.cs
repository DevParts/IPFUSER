using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElementType("Numbered")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class NumberedStoredProcedure : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IAlterable, ITextObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 5 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Name", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Number", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Name", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Number", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string))
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
					"BodyStartIndex" => 0, 
					"IsEncrypted" => 1, 
					"Name" => 2, 
					"Number" => 3, 
					"Text" => 4, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"BodyStartIndex" => 0, 
				"IsEncrypted" => 1, 
				"Name" => 2, 
				"Number" => 3, 
				"Text" => 4, 
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

	private NumberedStoredProcedureParameterCollection m_Params;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEncrypted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEncrypted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEncrypted", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public StoredProcedure Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as StoredProcedure;
		}
		internal set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public short Number => ((NumberedObjectKey)key).Number;

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(NumberedStoredProcedureParameter))]
	public NumberedStoredProcedureParameterCollection Parameters
	{
		get
		{
			CheckObjectState();
			if (m_Params == null)
			{
				m_Params = new NumberedStoredProcedureParameterCollection(this);
				SetCollectionTextMode(TextMode, m_Params);
			}
			return m_Params;
		}
	}

	public static string UrnSuffix => "Numbered";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public override string Name
	{
		get
		{
			return Parent.Name + ";" + Number;
		}
		set
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, null);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string TextBody
	{
		get
		{
			CheckObjectState();
			return GetTextBody();
		}
		set
		{
			CheckObjectState();
			SetTextBody(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string TextHeader
	{
		get
		{
			CheckObjectState();
			return GetTextHeader(forAlter: false);
		}
		set
		{
			CheckObjectState();
			SetTextHeader(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool TextMode
	{
		get
		{
			CheckObjectState();
			return GetTextMode();
		}
		set
		{
			CheckObjectState();
			SetTextMode(value, new SmoCollectionBase[1] { Parameters });
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "Name" };
	}

	internal NumberedStoredProcedure()
	{
	}

	internal NumberedStoredProcedure(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public NumberedStoredProcedure(StoredProcedure storedProcedure, short number)
	{
		SetParentImpl(storedProcedure);
		key = new NumberedObjectKey(number);
		SetState(SqlSmoState.Creating);
		base.Properties.SetValue(base.Properties.LookupID("Name", PropertyAccessPurpose.Write), Parent.Name + ";" + number);
	}

	private void ScriptSP(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		ScriptInternal(queries, sp, scriptHeaderType);
	}

	internal override string FormatFullNameForScripting(ScriptingPreferences sp)
	{
		return Parent.FormatFullNameForScripting(sp) + ";" + Number;
	}

	private void ScriptSPHeaderInternal(StringBuilder sb, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		switch (scriptHeaderType)
		{
		case ScriptHeaderType.ScriptHeaderForCreate:
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} PROCEDURE {1}", new object[2]
			{
				Scripts.CREATE,
				FormatFullNameForScripting(sp)
			});
			break;
		case ScriptHeaderType.ScriptHeaderForAlter:
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} PROCEDURE {1}", new object[2]
			{
				Scripts.ALTER,
				FormatFullNameForScripting(sp)
			});
			break;
		case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
			throw new SmoException(ExceptionTemplatesImpl.ScriptHeaderTypeNotSupported(scriptHeaderType.ToString(), GetType().Name, Name));
		default:
			throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
		}
		sb.Append(sp.NewLine);
		bool flag = true;
		StringCollection stringCollection = new StringCollection();
		sp.DataType.XmlNamespaces = true;
		foreach (NumberedStoredProcedureParameter parameter in Parameters)
		{
			if (parameter.State != SqlSmoState.ToBeDropped && !(string.Empty == parameter.Name))
			{
				if (!flag)
				{
					sb.Append(",");
					sb.Append(sp.NewLine);
				}
				flag = false;
				parameter.ScriptDdlInternal(stringCollection, sp);
				sb.AppendFormat(SmoApplication.DefaultCulture, "\t");
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
		if (!flag)
		{
			sb.Append(sp.NewLine);
		}
		bool needsComma = false;
		AppendWithOption(sb, "IsEncrypted", "ENCRYPTION", ref needsComma);
		if (needsComma)
		{
			sb.Append(sp.NewLine);
		}
		sb.Append("AS");
	}

	private void ScriptSPBodyInternal(StringBuilder sb)
	{
		sb.Append(GetTextBody(forScripting: true));
	}

	private void ScriptInternal(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		FormatFullNameForScripting(sp);
		ScriptInternalCreateDdl(queries, sp, scriptHeaderType);
		stringBuilder.Length = 0;
	}

	public override string ToString()
	{
		return FormatFullNameForScripting(new ScriptingPreferences());
	}

	internal void ScriptInternalCreateDdl(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		string text = SqlSmoObject.SqlString(Parent.FormatFullNameForScripting(sp));
		StringBuilder stringBuilder = new StringBuilder();
		ScriptInformativeHeaders(sp, stringBuilder);
		ScriptAnsiQI(Parent, sp, queries, stringBuilder, out var _, out var _);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_NUMBERED_PROCEDURE80, new object[3] { "NOT", text, Number });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_NUMBERED_PROCEDURE90, new object[3] { "NOT", text, Number });
			}
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("BEGIN");
			stringBuilder.Append(Globals.newline);
		}
		StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!TextMode || (sp.OldOptions.EnforceScriptingPreferences && sp.DataType.UserDefinedDataTypesToBaseType))
		{
			if (!sp.OldOptions.DdlBodyOnly)
			{
				ScriptSPHeaderInternal(stringBuilder2, sp, scriptHeaderType);
				stringBuilder2.Append(sp.NewLine);
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				ScriptSPBodyInternal(stringBuilder2);
			}
		}
		else
		{
			stringBuilder2.Append(GetTextForScript(sp, new string[2] { "procedure", "proc" }, forceCheckNameAndManipulateIfRequired: false, scriptHeaderType));
		}
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "execute sp_executesql @statement = {0} ", new object[1] { SqlSmoObject.MakeSqlString(stringBuilder2.ToString()) });
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("END");
		}
		else
		{
			stringBuilder.Append(stringBuilder2.ToString());
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptSP(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			InitializeKeepDirtyValues();
			ScriptSP(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter);
		}
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return SqlSmoObject.IsCollectionDirty(Parameters);
		}
		return true;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo(Parameters, bWithScript: false)
		};
	}

	public string ScriptHeader(bool forAlter)
	{
		CheckObjectState();
		return GetTextHeader(forAlter);
	}

	public string ScriptHeader(ScriptHeaderType scriptHeaderType)
	{
		if (ScriptHeaderType.ScriptHeaderForCreateOrAlter == scriptHeaderType)
		{
			throw new NotSupportedException(ExceptionTemplatesImpl.CreateOrAlterNotSupported(GetType().Name));
		}
		return GetTextHeader(scriptHeaderType);
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "IsEncrypted")
		{
			ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
		}
	}

	protected override string GetBraketNameForText()
	{
		return SqlSmoObject.MakeSqlBraket(Parent.Name) + ";" + Number;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[1] { "Number" };
	}
}
