using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class DdlTriggerBase : ScriptNameObjectBase, ICreatable, ICreateOrAlterable, IAlterable, IDroppable, IDropIfExists, IScriptable, ITextObject
{
	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public virtual string TextBody
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
	public virtual string TextHeader
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
	public virtual bool TextMode
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

	internal DdlTriggerBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	protected internal DdlTriggerBase()
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptTrigger(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
	}

	public void CreateOrAlter()
	{
		CreateOrAlterImpl();
	}

	internal override void ScriptCreateOrAlter(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptTrigger(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
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
			ScriptTrigger(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter);
		}
	}

	private bool ShouldScriptBodyAtAlter()
	{
		if (IsEventSetDirty())
		{
			return true;
		}
		if (GetIsTextDirty())
		{
			return true;
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("AnsiNullsStatus");
		stringCollection.Add("QuotedIdentifierStatus");
		stringCollection.Add("IsEncrypted");
		stringCollection.Add("ExecutionContext");
		if (this is DatabaseDdlTrigger)
		{
			stringCollection.Add("ExecutionContextUser");
		}
		else if (this is ServerDdlTrigger)
		{
			stringCollection.Add("ExecutionContextLogin");
		}
		stringCollection.Add("ImplementationType");
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringCollection.Add("AssemblyName");
			stringCollection.Add("ClassName");
			stringCollection.Add("MethodName");
		}
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			return true;
		}
		return false;
	}

	protected virtual bool IsEventSetDirty()
	{
		return false;
	}

	private bool GetInsteafOfValue(ScriptingPreferences sp)
	{
		return false;
	}

	private void ScriptTrigger(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.DdlTriggerDownlevel(FormatFullNameForScripting(sp, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(sp)));
		if (IsCreate(scriptHeaderType) || ShouldScriptBodyAtAlter())
		{
			GetInternalDDL(queries, sp, scriptHeaderType);
		}
		if (!sp.IncludeScripts.CreateDdlTriggerDisabled)
		{
			Property property = null;
			property = ((base.State != SqlSmoState.Creating && !base.IsDesignMode) ? base.Properties["IsEnabled"] : base.Properties.Get("IsEnabled"));
			if (property.Value != null && (property.Dirty || !sp.ScriptForAlter))
			{
				queries.Add(ScriptEnableDisableCommand((bool)property.Value, sp));
			}
		}
		else
		{
			queries.Add(ScriptEnableDisableCommand(isEnabled: false, sp));
		}
	}

	internal string ScriptEnableDisableCommand(bool isEnabled, ScriptingPreferences sp)
	{
		return string.Format(SmoApplication.DefaultCulture, "{0} TRIGGER {1} ON {2}", new object[3]
		{
			isEnabled ? "ENABLE" : "DISABLE",
			FormatFullNameForScripting(sp),
			(this is DatabaseDdlTrigger) ? "DATABASE" : "ALL SERVER"
		});
	}

	internal abstract string GetIfNotExistStatement(ScriptingPreferences sp, string prefix);

	protected override bool CheckObjectDirty()
	{
		if (this is DatabaseDdlTrigger && IsObjectDirty())
		{
			foreach (Property property in base.Properties)
			{
				if (property.Name != "IsEnabled" && property.Dirty)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void GetInternalDDL(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		bool flag = IsCreate(scriptHeaderType);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag2 = false;
		bool flag3 = false;
		string text = FormatFullNameForScripting(sp);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			if (sp.IncludeScripts.Header)
			{
				string objectType = GetType().InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture) as string;
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(objectType, text, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			flag2 = null != base.Properties.Get("AnsiNullsStatus").Value;
			flag3 = null != base.Properties.Get("QuotedIdentifierStatus").Value;
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				Server serverObject = GetServerObject();
				_ = serverObject.UserOptions.AnsiNulls;
				_ = serverObject.UserOptions.QuotedIdentifier;
			}
			if (flag2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)base.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if (flag3)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)base.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		bool flag4 = true;
		Property property;
		if ((property = base.Properties.Get("ImplementationType")).Value != null && ImplementationType.SqlClr == (ImplementationType)property.Value)
		{
			if (base.ServerVersion.Major < 9)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ClrNotSupported("ImplementationType", base.ServerVersion.ToString()));
			}
			flag4 = false;
			if (base.Properties.Get("Text").Dirty)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoPropertyChangeForDotNet("TextBody"));
			}
		}
		if (!TextMode)
		{
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (!sp.OldOptions.DdlBodyOnly)
			{
				bool needsComma = false;
				if (flag && sp.IncludeScripts.ExistenceCheck)
				{
					stringBuilder.AppendLine(GetIfNotExistStatement(sp, "NOT"));
					stringBuilder.AppendLine("EXECUTE dbo.sp_executesql N'");
				}
				switch (scriptHeaderType)
				{
				case ScriptHeaderType.ScriptHeaderForCreate:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.CREATE,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForAlter:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.ALTER,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
					SqlSmoObject.ThrowIfCreateOrAlterUnsupported(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateOrAlterDownlevel("DDL Trigger", SqlSmoObject.GetSqlServerName(sp)));
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.CREATE_OR_ALTER,
						text
					});
					break;
				default:
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
				}
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, " ON {0}", new object[1] { (this is DatabaseDdlTrigger) ? "DATABASE" : "ALL SERVER" });
				if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && flag4)
				{
					object value = base.Properties.Get("IsEncrypted").Value;
					if (value != null && (bool)value)
					{
						stringBuilder2.Append(" WITH ENCRYPTION ");
						needsComma = true;
					}
				}
				if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					stringBuilder2.Append(" ");
					if (this is ServerDdlTrigger)
					{
						AddScriptServerDdlExecuteAs(stringBuilder2, sp, base.Properties, ref needsComma);
					}
					else if (this is DatabaseDdlTrigger)
					{
						AddScriptDatabaseDdlExecuteAs(stringBuilder2, sp, base.Properties, ref needsComma);
					}
				}
				stringBuilder2.Append(Globals.newline);
				stringBuilder2.Append(" FOR ");
				AddDdlTriggerEvents(stringBuilder2, sp);
				stringBuilder2.Append(" AS ");
				stringBuilder2.Append(sp.NewLine);
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				if (flag4)
				{
					if (flag && sp.IncludeScripts.ExistenceCheck)
					{
						stringBuilder2.Append(GetTextBody(forScripting: true));
						stringBuilder2.Append(sp.NewLine);
						stringBuilder.Append(SqlSmoObject.SqlString(stringBuilder2.ToString()));
						stringBuilder.Append("'");
					}
					else
					{
						stringBuilder.Append(stringBuilder2.ToString());
						stringBuilder.Append(GetTextBody(forScripting: true));
					}
				}
				else
				{
					if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
					{
						stringBuilder2.Append(" EXTERNAL NAME ");
						string text2 = (string)GetPropValue("AssemblyName");
						if (string.Empty == text2)
						{
							throw new PropertyNotSetException("AssemblyName");
						}
						stringBuilder2.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text2));
						text2 = (string)GetPropValue("ClassName");
						if (string.Empty == text2)
						{
							throw new PropertyNotSetException("ClassName");
						}
						stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
						text2 = (string)GetPropValue("MethodName");
						if (string.Empty == text2)
						{
							throw new PropertyNotSetException(text2);
						}
						stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
					}
					if (flag && sp.IncludeScripts.ExistenceCheck)
					{
						stringBuilder.Append(SqlSmoObject.SqlString(stringBuilder2.ToString()));
						stringBuilder.Append("'");
					}
					else
					{
						stringBuilder.Append(stringBuilder2.ToString());
					}
				}
				stringBuilder.Append(sp.NewLine);
			}
		}
		else
		{
			string textForScript = GetTextForScript(sp, new string[1] { "trigger" }, forceCheckNameAndManipulateIfRequired: false, scriptHeaderType);
			if (flag && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendLine(GetIfNotExistStatement(sp, "NOT"));
				stringBuilder.AppendLine("EXECUTE dbo.sp_executesql N'");
				stringBuilder.Append(SqlSmoObject.SqlString(textForScript));
				stringBuilder.Append("'");
			}
			else
			{
				stringBuilder.Append(textForScript);
			}
		}
		queries.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
	}

	internal virtual void AddDdlTriggerEvents(StringBuilder sb, ScriptingPreferences sp)
	{
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
		CheckObjectState();
		return GetTextHeader(scriptHeaderType);
	}
}
