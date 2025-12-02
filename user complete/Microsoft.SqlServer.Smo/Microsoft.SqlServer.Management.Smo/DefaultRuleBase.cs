using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class DefaultRuleBase : ScriptSchemaObjectBase, ICreatable, IDroppable, IDropIfExists, IAlterable, IExtendedProperties, IScriptable, ITextObject
{
	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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
			SetTextMode(value, null);
		}
	}

	internal DefaultRuleBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	protected internal DefaultRuleBase()
	{
	}

	public void BindToColumn(string tablename, string colname, string tableschema)
	{
		try
		{
			if (tablename == null)
			{
				throw new ArgumentNullException("tablename");
			}
			if (colname == null)
			{
				throw new ArgumentNullException("colname");
			}
			if (tableschema == null)
			{
				throw new ArgumentNullException("tableschema");
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			if (Schema != null)
			{
				scriptingPreferences.IncludeScripts.SchemaQualify = true;
			}
			else
			{
				scriptingPreferences.IncludeScripts.SchemaQualify = false;
			}
			string empty = string.Empty;
			empty = ((tableschema.Length != 0) ? string.Format(SmoApplication.DefaultCulture, "N'[{0}].[{1}].[{2}]'", new object[3]
			{
				SqlSmoObject.SqlStringBraket(tableschema),
				SqlSmoObject.SqlStringBraket(tablename),
				SqlSmoObject.SqlStringBraket(colname)
			}) : string.Format(SmoApplication.DefaultCulture, "N'[{0}].[{1}]'", new object[2]
			{
				SqlSmoObject.SqlStringBraket(tablename),
				SqlSmoObject.SqlStringBraket(colname)
			}));
			if (this is Rule)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_bindrule @rulename=N'{0}', @objname={1}", new object[2]
				{
					SqlSmoObject.SqlString(FormatFullNameForScripting(scriptingPreferences)),
					empty
				}));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_bindefault @defname=N'{0}', @objname={1}", new object[2]
				{
					SqlSmoObject.SqlString(FormatFullNameForScripting(scriptingPreferences)),
					empty
				}));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, ex);
		}
	}

	public void BindToColumn(string tablename, string colname)
	{
		BindToColumn(tablename, colname, string.Empty);
	}

	public void BindToDataType(string datatypename, bool bindcolumns)
	{
		try
		{
			if (datatypename == null)
			{
				throw new ArgumentNullException("datatypename");
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.IncludeScripts.SchemaQualify = true;
			if (Schema != null)
			{
				scriptingPreferences.IncludeScripts.SchemaQualify = true;
			}
			else
			{
				scriptingPreferences.IncludeScripts.SchemaQualify = false;
			}
			if (this is Rule)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_bindrule N'{0}', N'[{1}]' {2}", new object[3]
				{
					SqlSmoObject.SqlString(FormatFullNameForScripting(scriptingPreferences)),
					SqlSmoObject.SqlStringBraket(datatypename),
					bindcolumns ? ", futureonly" : ""
				}));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_bindefault N'{0}', N'[{1}]' {2}", new object[3]
				{
					SqlSmoObject.SqlString(FormatFullNameForScripting(scriptingPreferences)),
					SqlSmoObject.SqlStringBraket(datatypename),
					bindcolumns ? ", futureonly" : ""
				}));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Bind, this, ex);
		}
	}

	public void UnbindFromColumn(string tablename, string colname)
	{
		UnbindFromColumn(tablename, colname, string.Empty);
	}

	public void UnbindFromColumn(string tablename, string colname, string tableschema)
	{
		try
		{
			if (tablename == null)
			{
				throw new ArgumentNullException("tablename");
			}
			if (colname == null)
			{
				throw new ArgumentNullException("colname");
			}
			if (tableschema == null)
			{
				throw new ArgumentNullException("tableschema");
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			string empty = string.Empty;
			empty = ((tableschema.Length != 0) ? string.Format(SmoApplication.DefaultCulture, "N'[{0}].[{1}].[{2}]'", new object[3]
			{
				SqlSmoObject.SqlStringBraket(tableschema),
				SqlSmoObject.SqlStringBraket(tablename),
				SqlSmoObject.SqlStringBraket(colname)
			}) : string.Format(SmoApplication.DefaultCulture, "N'[{0}].[{1}]'", new object[2]
			{
				SqlSmoObject.SqlStringBraket(tablename),
				SqlSmoObject.SqlStringBraket(colname)
			}));
			if (this is Rule)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_unbindrule @objname={0}", new object[1] { empty }));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_unbindefault @objname={0}", new object[1] { empty }));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, ex);
		}
	}

	public void UnbindFromDataType(string datatypename, bool bindcolumns)
	{
		try
		{
			if (datatypename == null)
			{
				throw new ArgumentNullException("datatypename");
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			if (this is Rule)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_unbindrule N'[{0}]' {1}", new object[2]
				{
					SqlSmoObject.SqlStringBraket(datatypename),
					bindcolumns ? ", futureonly" : ""
				}));
			}
			else
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_unbindefault N'[{0}]' {1}", new object[2]
				{
					SqlSmoObject.SqlStringBraket(datatypename),
					bindcolumns ? ", futureonly" : ""
				}));
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Unbind, this, ex);
		}
	}

	public SqlSmoObject[] EnumBoundColumns()
	{
		try
		{
			CheckObjectState();
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/Column")));
			SqlSmoObject[] array = new SqlSmoObject[enumeratorData.Rows.Count];
			Urn urn = base.Urn;
			int num = 0;
			foreach (DataRow row in enumeratorData.Rows)
			{
				string text = string.Format(SmoApplication.DefaultCulture, "{0}/Table[@Name='{1}' and @Schema='{2}']/Column[@Name='{3}']", urn.Parent, Urn.EscapeString((string)row["TableName"]), Urn.EscapeString((string)row["TableSchema"]), Urn.EscapeString((string)row["Name"]));
				array[num++] = GetServerObject().GetSmoObject(text);
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumBoundColumns, this, ex);
		}
	}

	public SqlSmoObject[] EnumBoundDataTypes()
	{
		try
		{
			CheckObjectState();
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/DataType")));
			SqlSmoObject[] array = new SqlSmoObject[enumeratorData.Rows.Count];
			Urn urn = base.Urn;
			int num = 0;
			foreach (DataRow row in enumeratorData.Rows)
			{
				string text = string.Format(SmoApplication.DefaultCulture, "{0}/UserDefinedDataType[@Name='{1}' and @Schema='{2}']", new object[3]
				{
					urn.Parent,
					Urn.EscapeString((string)row["Name"]),
					Urn.EscapeString((string)row["Schema"])
				});
				array[num++] = GetServerObject().GetSmoObject(text);
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumBoundDataTypes, this, ex);
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			ScriptIncludeHeaders(stringBuilder, sp, (this is Default) ? "Default" : "Rule");
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80) ? Scripts.INCLUDE_EXISTS_RULE_DEFAULT80 : Scripts.INCLUDE_EXISTS_RULE_DEFAULT90, new object[3]
				{
					"NOT",
					SqlSmoObject.SqlString(text),
					(this is Default) ? "IsDefault" : "IsRule"
				});
				stringBuilder.Append(sp.NewLine);
			}
		}
		StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!TextMode || sp.OldOptions.EnforceScriptingPreferences)
		{
			if (!sp.OldOptions.DdlBodyOnly)
			{
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "CREATE {0} {1} ", new object[2]
				{
					(this is Default) ? "DEFAULT" : "RULE",
					text
				});
				stringBuilder2.Append(sp.NewLine);
				stringBuilder2.Append("AS");
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				if (!sp.OldOptions.DdlBodyOnly)
				{
					stringBuilder2.Append(sp.NewLine);
				}
				stringBuilder2.Append(GetTextBody(forScripting: true));
			}
		}
		else
		{
			stringBuilder2.Append(GetTextForScript(sp, null, forceCheckNameAndManipulateIfRequired: false, ScriptHeaderType.ScriptHeaderForCreate));
		}
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat("EXEC dbo.sp_executesql N'{0}'", SqlSmoObject.SqlString(stringBuilder2.ToString()));
		}
		else
		{
			stringBuilder.Append(stringBuilder2.ToString());
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		ScriptIncludeHeaders(stringBuilder, sp, (this is Default) ? "Default" : "Rule");
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80) ? Scripts.INCLUDE_EXISTS_RULE_DEFAULT80 : Scripts.INCLUDE_EXISTS_RULE_DEFAULT90, new object[3]
			{
				"",
				SqlSmoObject.SqlString(text),
				(this is Default) ? "IsDefault" : "IsRule"
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP {0} {1}{2}", new object[3]
		{
			(this is Default) ? "DEFAULT" : "RULE",
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ThrowIfTextIsDirtyForAlter();
	}

	protected override void PostCreate()
	{
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
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
}
