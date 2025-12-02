using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public class ScriptNameObjectBase : NamedSmoObject
{
	private enum ScriptDDLPartialOptions
	{
		ScriptBody,
		ScriptHeaderForAlter,
		ScriptHeaderForCreate,
		ScriptHeaderForCreateOrAlter
	}

	public enum ScriptHeaderType
	{
		ScriptHeaderForAlter,
		ScriptHeaderForCreate,
		ScriptHeaderForCreateOrAlter
	}

	private string m_sScriptName = string.Empty;

	private bool m_textMode;

	private bool m_isTextModeInitialized;

	private int m_headerCutIndex = -1;

	private string m_textHeader;

	private string m_textBody;

	private bool m_isTextDirty;

	internal virtual string ScriptName
	{
		get
		{
			CheckObjectState();
			return m_sScriptName;
		}
		set
		{
			CheckObjectState();
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("ScriptName"));
			}
			if (this is Table)
			{
				Table.CheckTableName(value);
			}
			m_sScriptName = value;
		}
	}

	internal ScriptNameObjectBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState eState)
		: base(parentColl, key, eState)
	{
	}

	internal ScriptNameObjectBase(ObjectKeyBase key, SqlSmoState eState)
		: base(key, eState)
	{
	}

	protected internal ScriptNameObjectBase()
	{
	}

	protected void AutoGenerateName()
	{
		Name = Guid.NewGuid().ToString();
		SetIsSystemNamed(flag: true);
	}

	protected void SetIsSystemNamed(bool flag)
	{
		int index = base.Properties.LookupID("IsSystemNamed", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, flag);
		base.Properties.SetRetrieved(index, val: true);
	}

	protected bool GetIsSystemNamed()
	{
		try
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSystemNamed");
		}
		catch (Exception)
		{
			return key.IsNull;
		}
	}

	internal override string GetName(ScriptingPreferences sp)
	{
		if (sp != null && !sp.ForDirectExecution && ScriptName.Length > 0)
		{
			return ScriptName;
		}
		if (Name != null)
		{
			return Name;
		}
		return string.Empty;
	}

	internal void AddConstraintName(StringBuilder sb, ScriptingPreferences sp)
	{
		if (ScriptConstraintWithName(sp))
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, " CONSTRAINT {0} ", new object[1] { FormatFullNameForScripting(sp) });
		}
	}

	internal bool ScriptConstraintWithName(ScriptingPreferences sp)
	{
		bool flag = false;
		if (!sp.Table.SystemNamesForConstraints)
		{
			object propValueOptional = GetPropValueOptional("IsSystemNamed");
			if (propValueOptional != null)
			{
				flag = (bool)propValueOptional;
			}
			else if (base.IsDesignMode)
			{
				flag = GetIsSystemNamed();
			}
		}
		return !flag;
	}

	internal virtual string GetScriptIncludeExists(ScriptingPreferences sp, string tableName, bool forCreate)
	{
		return string.Empty;
	}

	internal void ConstraintScriptCreate(string scriptBody, StringCollection createQuery, ScriptingPreferences sp)
	{
		if (scriptBody.Length == 0)
		{
			return;
		}
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		string text = tableViewBase.FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(GetScriptIncludeExists(sp, text, forCreate: true));
			stringBuilder.Append(Globals.newline);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ", new object[1] { text });
		if (sp.Table.ConstraintsWithNoCheck)
		{
			stringBuilder.Append(" WITH NOCHECK");
		}
		else
		{
			object propValueOptional = GetPropValueOptional("IsChecked");
			if (propValueOptional != null)
			{
				bool flag = (bool)propValueOptional;
				stringBuilder.Append(flag ? " WITH CHECK" : " WITH NOCHECK");
			}
		}
		stringBuilder.Append(" ADD ");
		stringBuilder.Append(scriptBody);
		createQuery.Add(stringBuilder.ToString());
		if (!ScriptConstraintWithName(sp))
		{
			return;
		}
		object propValueOptional2 = GetPropValueOptional("IsEnabled");
		if (propValueOptional2 != null)
		{
			bool flag2 = (bool)propValueOptional2;
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder2.Append(GetScriptIncludeExists(sp, text, forCreate: false));
				stringBuilder2.Append(Globals.newline);
			}
			stringBuilder2.Append(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} {1} CONSTRAINT {2}", new object[3]
			{
				text,
				flag2 ? "CHECK" : "NOCHECK",
				FormatFullNameForScripting(sp, nameIsIndentifier: true)
			}));
			createQuery.Add(stringBuilder2.ToString());
		}
	}

	internal void ConstraintScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		Property property = base.Properties.Get("IsEnabled");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			bool flag = (bool)property.Value;
			alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} {1} CONSTRAINT [{2}]", new object[3]
			{
				tableViewBase.FullQualifiedName,
				flag ? "CHECK" : "NOCHECK",
				SqlSmoObject.SqlBraket(Name)
			}));
		}
	}

	protected void AppendWithOption(StringBuilder sb, string propName, string optionText, ref bool needsComma)
	{
		if (GetPropValueOptional(propName, defaultValue: false))
		{
			AppendWithCommaText(sb, optionText, ref needsComma);
		}
	}

	protected void AppendWithCommaText(StringBuilder sb, string optionText, ref bool needsComma)
	{
		AppendCommaText(sb, optionText, ref needsComma, "WITH");
	}

	protected void AppendCommaText(StringBuilder sb, string optionText, ref bool needsComma, string beginWord)
	{
		if (!needsComma)
		{
			sb.Append(beginWord + " ");
		}
		else
		{
			sb.Append(Globals.commaspace);
		}
		sb.Append(optionText);
		needsComma = true;
	}

	internal void ScriptAnsiQI(SqlSmoObject o, ScriptingPreferences sp, StringCollection queries, StringBuilder sb, out object ansiNull, out object qi, bool skipSetOptions = false)
	{
		ansiNull = null;
		qi = null;
		bool flag = false;
		bool flag2 = false;
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			flag = null != o.Properties.Get("AnsiNullsStatus").Value;
			flag2 = null != o.Properties.Get("QuotedIdentifierStatus").Value;
		}
		Server server = (Server)o.ParentColl.ParentInstance.ParentColl.ParentInstance;
		if (skipSetOptions)
		{
			return;
		}
		if (flag)
		{
			if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
			{
				ansiNull = server.UserOptions.AnsiNulls;
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)o.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
			queries.Add(sb.ToString());
			sb.Length = 0;
		}
		if (flag2)
		{
			if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
			{
				qi = server.UserOptions.QuotedIdentifier;
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)o.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
			queries.Add(sb.ToString());
			sb.Length = 0;
		}
	}

	internal void ScriptInformativeHeaders(ScriptingPreferences sp, StringBuilder sb)
	{
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly && sp.IncludeScripts.Header)
		{
			sb.Append(ExceptionTemplates.IncludeHeader(GetType().Name, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			sb.Append(sp.NewLine);
		}
	}

	internal bool AddScriptExecuteAs(StringBuilder sb, ScriptingPreferences sp, PropertyCollection col, ref bool needsComma)
	{
		Property property = col.Get("ExecutionContext");
		if (!sp.Security.ExecuteAs || property.Value == null)
		{
			return false;
		}
		ExecutionContext executionContext = (ExecutionContext)property.Value;
		AppendWithCommaText(sb, "EXECUTE AS ", ref needsComma);
		switch (executionContext)
		{
		case ExecutionContext.Caller:
			sb.Append("CALLER");
			break;
		case ExecutionContext.Owner:
			sb.Append("OWNER");
			break;
		case ExecutionContext.Self:
			sb.Append("SELF");
			break;
		case ExecutionContext.ExecuteAsUser:
		{
			string text = (string)GetPropValue("ExecutionContextPrincipal");
			if (string.Empty == text)
			{
				throw new PropertyNotSetException("ExecutionContextPrincipal");
			}
			sb.AppendFormat("N'{0}'", SqlSmoObject.SqlString(text));
			break;
		}
		}
		return true;
	}

	internal bool AddScriptServerDdlExecuteAs(StringBuilder sb, ScriptingPreferences sp, PropertyCollection col, ref bool needsComma)
	{
		Property property = col.Get("ExecutionContext");
		if (!sp.Security.ExecuteAs || property.Value == null)
		{
			return false;
		}
		ServerDdlTriggerExecutionContext serverDdlTriggerExecutionContext = (ServerDdlTriggerExecutionContext)property.Value;
		AppendWithCommaText(sb, "EXECUTE AS ", ref needsComma);
		switch (serverDdlTriggerExecutionContext)
		{
		case ServerDdlTriggerExecutionContext.Caller:
			sb.Append("CALLER");
			break;
		case ServerDdlTriggerExecutionContext.Self:
			sb.Append("SELF");
			break;
		case ServerDdlTriggerExecutionContext.ExecuteAsLogin:
		{
			string text = (string)GetPropValue("ExecutionContextLogin");
			if (string.Empty == text)
			{
				throw new PropertyNotSetException("ExecutionContextLogin");
			}
			sb.AppendFormat("N'{0}'", SqlSmoObject.SqlString(text));
			break;
		}
		}
		return true;
	}

	internal bool AddScriptDatabaseDdlExecuteAs(StringBuilder sb, ScriptingPreferences sp, PropertyCollection col, ref bool needsComma)
	{
		Property property = col.Get("ExecutionContext");
		if (!sp.Security.ExecuteAs || property.Value == null)
		{
			return false;
		}
		DatabaseDdlTriggerExecutionContext databaseDdlTriggerExecutionContext = (DatabaseDdlTriggerExecutionContext)property.Value;
		AppendWithCommaText(sb, "EXECUTE AS ", ref needsComma);
		switch (databaseDdlTriggerExecutionContext)
		{
		case DatabaseDdlTriggerExecutionContext.Caller:
			sb.Append("CALLER");
			break;
		case DatabaseDdlTriggerExecutionContext.Self:
			sb.Append("SELF");
			break;
		case DatabaseDdlTriggerExecutionContext.ExecuteAsUser:
		{
			string text = (string)GetPropValue("ExecutionContextUser");
			if (string.Empty == text)
			{
				throw new PropertyNotSetException("ExecutionContextUser");
			}
			sb.AppendFormat("N'{0}'", SqlSmoObject.SqlString(text));
			break;
		}
		}
		return true;
	}

	internal bool IsCreate(ScriptHeaderType scriptHeaderType)
	{
		if (scriptHeaderType != ScriptHeaderType.ScriptHeaderForCreate)
		{
			return scriptHeaderType == ScriptHeaderType.ScriptHeaderForCreateOrAlter;
		}
		return true;
	}

	internal bool IsOrAlterSupported(ScriptingPreferences sp)
	{
		return sp.TargetServerVersion >= SqlServerVersion.Version130;
	}

	protected void SetTextMode(bool textMode, SmoCollectionBase[] collList)
	{
		if (!m_textMode || CheckTextModeSupport())
		{
			m_textMode = GetTextMode();
			if (textMode != m_textMode)
			{
				SwitchTextMode(textMode, collList);
				m_textMode = textMode;
			}
		}
	}

	protected bool GetTextMode()
	{
		if (!m_isTextModeInitialized)
		{
			m_textMode = GetServerObject().DefaultTextMode;
			if (m_textMode && !CheckTextModeSupport())
			{
				m_textMode = false;
			}
			m_isTextModeInitialized = true;
		}
		return m_textMode;
	}

	protected bool CheckTextModeSupport()
	{
		if (SqlSmoState.Existing == base.State && base.Properties.Contains("ImplementationType") && ImplementationType.SqlClr == (ImplementationType)GetPropValue("ImplementationType"))
		{
			return false;
		}
		return true;
	}

	internal void CheckTextModeAccess(string propName)
	{
		if (GetTextMode())
		{
			throw new PropertyWriteException(propName, GetType().Name, Name, ExceptionTemplatesImpl.ReasonIntextMode);
		}
	}

	private string ScriptDDLPartialInternal(ScriptDDLPartialOptions options)
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		StringCollection stringCollection = new StringCollection();
		scriptingPreferences.SuppressDirtyCheck = false;
		scriptingPreferences.SetTargetServerInfo(this);
		scriptingPreferences.OldOptions.DdlBodyOnly = ScriptDDLPartialOptions.ScriptBody == options;
		scriptingPreferences.OldOptions.DdlHeaderOnly = !scriptingPreferences.OldOptions.DdlBodyOnly;
		scriptingPreferences.ForDirectExecution = true;
		scriptingPreferences.ScriptForCreateDrop = true;
		ScriptCreate(stringCollection, scriptingPreferences);
		if (ScriptDDLPartialOptions.ScriptHeaderForAlter == options)
		{
			TraceHelper.Assert(stringCollection.Count > 0, "queries.Count > 0 failed. queries.Count=" + stringCollection.Count);
			stringCollection[0] = ModifyTextForAlter(stringCollection[0], 0);
		}
		else if (ScriptDDLPartialOptions.ScriptHeaderForCreateOrAlter == options)
		{
			TraceHelper.Assert(stringCollection.Count > 0, "queries.Count > 0 failed. queries.Count=" + stringCollection.Count);
			stringCollection[0] = ModifyTextForCreateOrAlter(stringCollection[0], 0);
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				stringBuilder.Append(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		return stringBuilder.ToString();
	}

	private int GetHeaderCutIndex(string text)
	{
		if (m_headerCutIndex >= 0)
		{
			return m_headerCutIndex;
		}
		int num = DdlTextParser.ParseDdlHeader(text);
		if (num <= 0 || num > text.Length)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SyntaxErrorInTextHeader(GetType().Name, Name));
		}
		return num;
	}

	private void ResetTextData()
	{
		m_headerCutIndex = -1;
		m_isTextDirty = false;
		m_textHeader = null;
		m_textBody = null;
	}

	protected void ThrowIfTextIsDirtyForAlter()
	{
		if (m_isTextDirty)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropNotModifiable("Text", GetType().Name));
		}
	}

	protected override void CleanObject()
	{
		base.CleanObject();
		m_isTextDirty = false;
	}

	public override void Refresh()
	{
		base.Refresh();
		ResetTextData();
	}

	protected void SetTextHeader(string textHeader)
	{
		if (!GetTextMode())
		{
			throw new PropertyWriteException("TextHeader", GetType().Name, Name, ExceptionTemplatesImpl.ReasonNotIntextMode);
		}
		SetTextHeaderInternal(textHeader);
	}

	private void SetTextHeaderInternal(string textHeader)
	{
		if (textHeader == null)
		{
			throw new ArgumentNullException();
		}
		m_textHeader = textHeader;
		m_isTextDirty = true;
	}

	protected void SetTextBody(string textBody)
	{
		if (!ForceTextModeOnTextBody() && !GetTextMode())
		{
			throw new PropertyWriteException("TextBody", GetType().Name, Name, ExceptionTemplatesImpl.ReasonNotIntextMode);
		}
		SetTextBodyInternal(textBody);
	}

	private void SetTextBodyInternal(string textBody)
	{
		if (textBody == null)
		{
			throw new ArgumentNullException();
		}
		m_textBody = textBody;
		m_isTextDirty = true;
	}

	protected bool ForceTextModeOnTextBody()
	{
		if (base.Properties.Contains("ImplementationType"))
		{
			return ImplementationType.TransactSql == GetPropValueOptional("ImplementationType", ImplementationType.TransactSql);
		}
		return true;
	}

	protected string GetTextBody()
	{
		return GetTextBody(forScripting: false);
	}

	protected string GetTextBody(bool forScripting)
	{
		if (!ForceTextModeOnTextBody() && !GetTextMode())
		{
			try
			{
				return ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptBody);
			}
			catch (PropertyCannotBeRetrievedException)
			{
				throw new PropertyCannotBeRetrievedException("TextBody", this);
			}
		}
		if (m_textBody == null)
		{
			string textProperty = GetTextProperty("TextBody", bThrowIfCreating: false);
			if (textProperty != null && textProperty.Length > 0)
			{
				int headerCutIndex = GetHeaderCutIndex(textProperty);
				m_textBody = textProperty.Substring(headerCutIndex, textProperty.Length - headerCutIndex);
			}
			else if (forScripting)
			{
				throw new PropertyNotSetException("TextBody");
			}
		}
		return m_textBody;
	}

	protected string GetTextHeader(bool forAlter)
	{
		return GetTextHeader((!forAlter) ? ScriptHeaderType.ScriptHeaderForCreate : ScriptHeaderType.ScriptHeaderForAlter);
	}

	protected string GetTextHeader(ScriptHeaderType scriptHeaderType)
	{
		if (!GetTextMode())
		{
			try
			{
				switch (scriptHeaderType)
				{
				case ScriptHeaderType.ScriptHeaderForCreate:
					return ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptHeaderForCreate);
				case ScriptHeaderType.ScriptHeaderForAlter:
					return ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptHeaderForAlter);
				case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
					if (!(this is ICreateOrAlterable))
					{
						throw new FailedOperationException(ExceptionTemplatesImpl.ScriptHeaderTypeNotSupported(scriptHeaderType.ToString(), GetType().Name, Name));
					}
					return ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptHeaderForCreateOrAlter);
				}
			}
			catch (PropertyCannotBeRetrievedException)
			{
				throw new PropertyCannotBeRetrievedException("TextHeader", this);
			}
		}
		if (m_textHeader == null)
		{
			string textProperty = GetTextProperty("TextHeader", bThrowIfCreating: false);
			if (textProperty != null)
			{
				int headerCutIndex = GetHeaderCutIndex(textProperty);
				m_textHeader = textProperty.Substring(0, headerCutIndex);
			}
		}
		if (ShouldScriptForNonCreate(scriptHeaderType))
		{
			return CheckAndManipulateText((m_textHeader != null) ? m_textHeader : string.Empty, null, new ScriptingPreferences(), scriptHeaderType);
		}
		if (m_textHeader == null)
		{
			return string.Empty;
		}
		return m_textHeader;
	}

	protected void SetCollectionTextMode(bool newTextModeValue, SmoCollectionBase coll)
	{
		if (newTextModeValue)
		{
			coll.LockCollection(ExceptionTemplatesImpl.ReasonIntextMode);
		}
		else
		{
			coll.UnlockCollection();
		}
	}

	protected void SwitchTextMode(bool newTextModeValue, SmoCollectionBase[] collList)
	{
		if (collList != null)
		{
			foreach (SmoCollectionBase coll in collList)
			{
				SetCollectionTextMode(newTextModeValue, coll);
			}
		}
		if (!newTextModeValue)
		{
			m_textHeader = null;
			if (!ForceTextModeOnTextBody())
			{
				m_isTextDirty = false;
				m_textBody = null;
			}
			return;
		}
		try
		{
			m_textHeader = ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptHeaderForCreate);
		}
		catch (PropertyNotSetException)
		{
			m_textHeader = string.Empty;
		}
		if (!ForceTextModeOnTextBody())
		{
			try
			{
				m_textBody = ScriptDDLPartialInternal(ScriptDDLPartialOptions.ScriptBody);
			}
			catch (PropertyNotSetException)
			{
				m_textBody = string.Empty;
			}
		}
		m_isTextDirty = true;
	}

	internal static void Validate_set_TextObjectDDLProperty(Property prop, object newValue)
	{
		((ScriptNameObjectBase)prop.Parent.m_parent).CheckTextModeAccess(prop.Name);
	}

	internal static void Validate_set_ChildTextObjectDDLProperty(Property prop, object newValue)
	{
		((ScriptNameObjectBase)((SqlSmoObject)prop.Parent.m_parent).ParentColl.ParentInstance).CheckTextModeAccess(prop.Name);
	}

	private bool ShouldScriptForNonCreate(ScriptHeaderType scriptHeaderType)
	{
		if (scriptHeaderType != ScriptHeaderType.ScriptHeaderForAlter)
		{
			return ScriptHeaderType.ScriptHeaderForCreateOrAlter == scriptHeaderType;
		}
		return true;
	}

	private string ModifyTextForAlter(string text, int indexCreate)
	{
		TraceHelper.Assert(indexCreate >= 0, "indexCreate >= 0 failed, indexCreate=" + indexCreate);
		TraceHelper.Assert(indexCreate <= text.Length - Scripts.CREATE.Length, "The statement \"" + text + "\" is shorter than \"" + Scripts.CREATE + "\"");
		TraceHelper.Assert(Scripts.CREATE == text.Substring(indexCreate, Scripts.CREATE.Length).ToUpper(SmoApplication.DefaultCulture), "\"CREATE\" == text.Substring(indexCreate, 6).ToUpper() failed. text=" + text);
		text = text.Remove(indexCreate, Scripts.CREATE.Length);
		return text.Insert(indexCreate, Scripts.ALTER);
	}

	private string ModifyTextForCreateOrAlter(string text, int indexCreate)
	{
		if (indexCreate < 0 || indexCreate > text.Length - Scripts.CREATE.Length)
		{
			throw new SmoException(ExceptionTemplatesImpl.InvalidIndexSpecifiedForModifyingTextToCreateOrAlter(indexCreate, 0, text.Length - Scripts.CREATE.Length));
		}
		if (Scripts.CREATE != text.Substring(indexCreate, Scripts.CREATE.Length).ToUpper(SmoApplication.DefaultCulture))
		{
			throw new SmoException(ExceptionTemplatesImpl.InvalidTextForModifyingToCreateOrAlter);
		}
		if (Scripts.CREATE_OR_ALTER != text.Substring(indexCreate, Scripts.CREATE_OR_ALTER.Length).ToUpper(SmoApplication.DefaultCulture))
		{
			text = text.Remove(indexCreate, Scripts.CREATE.Length);
			text = text.Insert(indexCreate, Scripts.CREATE_OR_ALTER);
		}
		return text;
	}

	protected bool GetIsTextDirty()
	{
		if (!m_isTextDirty)
		{
			return base.IsTouched;
		}
		return true;
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return GetIsTextDirty();
		}
		return true;
	}

	internal string GetTextForScript(ScriptingPreferences sp, string[] expectedObjectTypes, bool forceCheckNameAndManipulateIfRequired, ScriptHeaderType scriptHeaderType)
	{
		TraceHelper.Assert(GetTextMode(), "true == GetTextMode() failed");
		if (!sp.ForDirectExecution && !sp.OldOptions.EnforceScriptingPreferences)
		{
			string ddlText = BuildText(sp);
			if (forceCheckNameAndManipulateIfRequired)
			{
				if (!DdlTextParser.CheckDdlHeader(ddlText, GetQuotedIdentifier(), IsOrAlterSupported(sp), out var headerInfo))
				{
					throw new FailedOperationException(ExceptionTemplatesImpl.SyntaxErrorInTextHeader(GetType().Name, Name));
				}
				CheckObjectSupportability(headerInfo, sp);
				CheckAndManipulateName(ref ddlText, sp, ref headerInfo, forceScriptingName: false);
			}
			return ddlText;
		}
		if (expectedObjectTypes == null)
		{
			expectedObjectTypes = new string[1] { GetType().Name };
		}
		return CheckAndManipulateText(BuildText(sp), expectedObjectTypes, sp, scriptHeaderType);
	}

	protected virtual bool CheckObjectDirty()
	{
		return true;
	}

	private void CheckObjectSupportability(DdlTextParserHeaderInfo headerInfo, ScriptingPreferences sp)
	{
		if (this is StoredProcedure && !string.IsNullOrEmpty(headerInfo.procedureNumber) && sp.TargetEngineIsAzureSqlDw())
		{
			throw new UnsupportedEngineEditionException(ExceptionTemplatesImpl.NotSupportedForSqlDw(typeof(NumberedStoredProcedure).Name)).SetHelpContext("NotSupportedForSqlDw");
		}
	}

	private string BuildText(ScriptingPreferences sp)
	{
		TraceHelper.Assert(GetTextMode(), "true == GetTextMode() failed");
		if (!GetIsTextDirty() && !sp.OldOptions.DdlBodyOnly && !sp.OldOptions.DdlHeaderOnly)
		{
			if (base.IsObjectDirty() && CheckObjectDirty())
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.WrongPropertyValueExceptionText("TextMode", "true"));
			}
			return GetTextProperty("TextHeader", sp);
		}
		string text = string.Empty;
		if (!sp.OldOptions.DdlBodyOnly)
		{
			text = GetTextHeader(forAlter: false);
			TraceHelper.Assert(null != text, "null == textHeader");
			if (text.Length <= 0)
			{
				throw new PropertyNotSetException("TextHeader");
			}
		}
		StringBuilder stringBuilder = new StringBuilder(text);
		string empty = string.Empty;
		if (!sp.OldOptions.DdlHeaderOnly)
		{
			empty = GetTextBody();
			TraceHelper.Assert(null != empty, "null == textBody");
			if (text.Length > 0 && !char.IsWhiteSpace(text[text.Length - 1]) && empty.Length > 0 && !char.IsWhiteSpace(empty[0]))
			{
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append(empty);
		}
		return stringBuilder.ToString();
	}

	protected virtual string GetBraketNameForText()
	{
		return SqlSmoObject.MakeSqlBraket(Name);
	}

	internal void CheckNameInTextCorrectness(string expectedName, string expectedSchema, string foundName, string foundSchema, string foundProcedureNumber)
	{
		TraceHelper.Assert(GetTextMode(), "true == GetTextMode() failed");
		TraceHelper.Assert(expectedName != null && expectedName.Length > 1 && expectedName[0] == '[');
		TraceHelper.Assert(expectedSchema != null && (expectedSchema.Length == 0 || (expectedSchema.Length > 1 && expectedSchema[0] == '[')));
		TraceHelper.Assert(foundName != null && foundName.Length > 1 && foundName[0] == '[');
		TraceHelper.Assert(foundSchema != null && (foundSchema.Length == 0 || (foundSchema.Length > 1 && foundSchema[0] == '[')));
		TraceHelper.Assert(foundProcedureNumber != null && (foundProcedureNumber.Length == 0 || (foundProcedureNumber.Length > 1 && foundProcedureNumber[0] == ';')));
		TraceHelper.Assert((this is ScriptSchemaObjectBase && expectedSchema.Length > 0) || (!(this is ScriptSchemaObjectBase) && expectedSchema.Length == 0 && foundSchema.Length == 0) || base.State == SqlSmoState.Creating || this is Trigger);
		if (";1" == foundProcedureNumber)
		{
			foundProcedureNumber = "";
		}
		string y = foundName + foundProcedureNumber;
		if (base.StringComparer.Compare(expectedName, y) != 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.IncorrectTextHeader(GetType().Name, Name, "name", "Name"));
		}
		if (expectedSchema.Length <= 0)
		{
			return;
		}
		TraceHelper.Assert(this is ScriptSchemaObjectBase);
		if (foundSchema.Length > 0)
		{
			if (base.StringComparer.Compare(expectedSchema, foundSchema) != 0)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.IncorrectTextHeader(GetType().Name, Name, "schema", "Schema"));
			}
			return;
		}
		ScriptSchemaObjectBase scriptSchemaObjectBase = (ScriptSchemaObjectBase)this;
		SchemaCollectionBase schemaCollectionBase = (SchemaCollectionBase)scriptSchemaObjectBase.ParentColl;
		if (base.StringComparer.Compare(expectedSchema, SqlSmoObject.MakeSqlBraket(schemaCollectionBase.GetDefaultSchema())) != 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.IncorrectTextHeader(GetType().Name, Name, "schema", "Schema"));
		}
	}

	protected void CheckTextCorrectness(string ddlText, bool enforceCreate, bool checkName, string[] expectedObjectTypes, out DdlTextParserHeaderInfo headerInfo)
	{
		CheckTextCorrectness(ddlText, enforceCreate, checkName, isOrAlterSupported: false, expectedObjectTypes, out headerInfo);
	}

	protected void CheckTextCorrectness(string ddlText, bool enforceCreate, bool checkName, bool isOrAlterSupported, string[] expectedObjectTypes, out DdlTextParserHeaderInfo headerInfo)
	{
		TraceHelper.Assert(GetTextMode(), "true == GetTextMode() failed");
		if (!DdlTextParser.CheckDdlHeader(ddlText, GetQuotedIdentifier(), isOrAlterSupported, out headerInfo))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SyntaxErrorInTextHeader(GetType().Name, Name));
		}
		if (enforceCreate && !headerInfo.scriptForCreate)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SyntaxErrorInTextHeader(GetType().Name, Name));
		}
		if (expectedObjectTypes != null)
		{
			bool flag = false;
			foreach (string strA in expectedObjectTypes)
			{
				if (string.Compare(strA, headerInfo.objectType, StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.SyntaxErrorInTextHeader(GetType().Name, Name));
			}
		}
		if (checkName)
		{
			string expectedSchema = ((this is ScriptSchemaObjectBase scriptSchemaObjectBase) ? SqlSmoObject.MakeSqlBraket(scriptSchemaObjectBase.Schema) : string.Empty);
			CheckNameInTextCorrectness(GetBraketNameForText(), expectedSchema, headerInfo.name, headerInfo.schema, headerInfo.procedureNumber);
			if (string.Compare("TRIGGER", headerInfo.objectType, StringComparison.OrdinalIgnoreCase) == 0 && this is Trigger trigger)
			{
				TableViewBase tableViewBase = (TableViewBase)trigger.Parent;
				tableViewBase.CheckNameInTextCorrectness(tableViewBase.GetBraketNameForText(), SqlSmoObject.MakeSqlBraket(tableViewBase.Schema), headerInfo.nameSecondary, headerInfo.schemaSecondary, string.Empty);
			}
		}
	}

	private string CheckAndManipulateText(string ddlText, string[] expectedObjectTypes, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		TraceHelper.Assert(GetTextMode(), "true == GetTextMode() failed");
		bool enforceCreate = ScriptHeaderType.ScriptHeaderForCreate == scriptHeaderType || !sp.ForDirectExecution;
		bool forDirectExecution = sp.ForDirectExecution;
		CheckTextCorrectness(ddlText, enforceCreate, forDirectExecution, IsOrAlterSupported(sp), expectedObjectTypes, out var headerInfo);
		CheckObjectSupportability(headerInfo, sp);
		if (!forDirectExecution)
		{
			CheckAndManipulateName(ref ddlText, sp, ref headerInfo, forceScriptingName: true);
		}
		if (scriptHeaderType == ScriptHeaderType.ScriptHeaderForAlter && headerInfo.scriptForCreate)
		{
			ddlText = ModifyTextForAlter(ddlText, headerInfo.indexCreate);
		}
		else if (ScriptHeaderType.ScriptHeaderForCreateOrAlter == scriptHeaderType && !headerInfo.scriptContainsOrAlter)
		{
			ddlText = ModifyTextForCreateOrAlter(ddlText, headerInfo.indexCreate);
		}
		return ddlText;
	}

	private bool IsSchemaNameSame(string schemaName)
	{
		if (string.IsNullOrEmpty(schemaName))
		{
			return true;
		}
		ScriptSchemaObjectBase scriptSchemaObjectBase = this as ScriptSchemaObjectBase;
		bool result = false;
		if (scriptSchemaObjectBase != null)
		{
			result = base.StringComparer.Compare(SqlSmoObject.MakeSqlBraket(scriptSchemaObjectBase.Schema), schemaName) == 0;
		}
		return result;
	}

	private void CheckAndManipulateName(ref string ddlText, ScriptingPreferences sp, ref DdlTextParserHeaderInfo headerInfo, bool forceScriptingName)
	{
		if (!forceScriptingName && base.StringComparer.Compare(GetBraketNameForText(), headerInfo.name) == 0 && IsSchemaNameSame(headerInfo.schema))
		{
			return;
		}
		if (string.Compare("TRIGGER", headerInfo.objectType, StringComparison.OrdinalIgnoreCase) == 0)
		{
			TraceHelper.Assert(headerInfo.indexNameStartSecondary > 0 && headerInfo.indexNameEndSecondary > 0 && headerInfo.indexNameEndSecondary > headerInfo.indexNameStartSecondary);
			if (this is Trigger trigger)
			{
				TableViewBase tableViewBase = (TableViewBase)trigger.Parent;
				ddlText = ddlText.Remove(headerInfo.indexNameStartSecondary, headerInfo.indexNameEndSecondary - headerInfo.indexNameStartSecondary);
				if (headerInfo.databaseSecondary != null && headerInfo.databaseSecondary.Length > 0)
				{
					Database database = tableViewBase.ParentColl.ParentInstance as Database;
					string text = headerInfo.databaseSecondary;
					if (database != null)
					{
						text = database.FormatFullNameForScripting(sp);
					}
					ddlText = ddlText.Insert(headerInfo.indexNameStartSecondary, text + "." + tableViewBase.FormatFullNameForScripting(sp));
				}
				else
				{
					ddlText = ddlText.Insert(headerInfo.indexNameStartSecondary, tableViewBase.FormatFullNameForScripting(sp));
				}
			}
		}
		TraceHelper.Assert(headerInfo.indexNameStart > 0 && headerInfo.indexNameEnd > 0 && headerInfo.indexNameEnd > headerInfo.indexNameStart);
		ddlText = ddlText.Remove(headerInfo.indexNameStart, headerInfo.indexNameEnd - headerInfo.indexNameStart);
		ddlText = ddlText.Insert(headerInfo.indexNameStart, FormatFullNameForScripting(sp));
	}

	private bool GetQuotedIdentifier()
	{
		if (!base.Properties.Contains("QuotedIdentifierStatus"))
		{
			return false;
		}
		return GetPropValueOptional("QuotedIdentifierStatus", defaultValue: false);
	}

	protected string GetTextProperty(string requestingProperty)
	{
		return GetTextProperty(requestingProperty, bThrowIfCreating: true);
	}

	internal string GetTextProperty(string requestingProperty, ScriptingPreferences sp)
	{
		return GetTextProperty(requestingProperty, sp, bThrowIfCreating: true);
	}

	private string GetTextProperty(string requestingProperty, bool bThrowIfCreating)
	{
		return GetTextProperty(requestingProperty, null, bThrowIfCreating);
	}

	private string GetExecutionContextString(ServerDdlTriggerExecutionContext ec)
	{
		string result = string.Empty;
		switch (ec)
		{
		case ServerDdlTriggerExecutionContext.Caller:
			result = "CALLER";
			break;
		case ServerDdlTriggerExecutionContext.Self:
			result = "SELF";
			break;
		case ServerDdlTriggerExecutionContext.ExecuteAsLogin:
		{
			string text = (string)GetPropValue("ExecutionContextPrincipal");
			result = ((!(string.Empty == text)) ? "USER" : string.Format(CultureInfo.InvariantCulture, "USER ({0})", new object[1] { text }));
			break;
		}
		}
		return result;
	}

	private string GetExecutionContextString(DatabaseDdlTriggerExecutionContext ec)
	{
		string result = string.Empty;
		switch (ec)
		{
		case DatabaseDdlTriggerExecutionContext.Caller:
			result = "CALLER";
			break;
		case DatabaseDdlTriggerExecutionContext.Self:
			result = "SELF";
			break;
		case DatabaseDdlTriggerExecutionContext.ExecuteAsUser:
		{
			string text = (string)GetPropValue("ExecutionContextPrincipal");
			result = ((!(string.Empty == text)) ? "USER" : string.Format(CultureInfo.InvariantCulture, "USER ({0})", new object[1] { text }));
			break;
		}
		}
		return result;
	}

	private string GetExecutionContextString(ExecutionContext ec)
	{
		string result = string.Empty;
		switch (ec)
		{
		case ExecutionContext.Caller:
			result = "CALLER";
			break;
		case ExecutionContext.Owner:
			result = "OWNER";
			break;
		case ExecutionContext.Self:
			result = "SELF";
			break;
		case ExecutionContext.ExecuteAsUser:
		{
			string text = (string)GetPropValue("ExecutionContextPrincipal");
			result = ((!(string.Empty == text)) ? "USER" : string.Format(CultureInfo.InvariantCulture, "USER ({0})", new object[1] { text }));
			break;
		}
		}
		return result;
	}

	private string GetTextProperty(string requestingProperty, ScriptingPreferences sp, bool bThrowIfCreating)
	{
		if (base.IsDesignMode)
		{
			return GetTextPropertyDesignMode(requestingProperty, sp, bThrowIfCreating);
		}
		if (base.ServerVersion.Major < 9 && base.State != SqlSmoState.Creating && base.Properties.Contains("IsEncrypted") && (bool)GetPropValue("IsEncrypted"))
		{
			throw new PropertyCannotBeRetrievedException(requestingProperty, this, ExceptionTemplatesImpl.ReasonTextIsEncrypted);
		}
		string text = (string)GetPropValueOptional("Text");
		if (base.Properties.Contains("ExecutionContext") && sp != null && sp.TargetServerVersion == SqlServerVersion.Version80)
		{
			Property property = base.Properties["ExecutionContext"];
			if (property.Value != null)
			{
				string message = string.Empty;
				bool flag = true;
				string sqlServerName = SqlSmoObject.GetSqlServerName(sp);
				string objectName = FormatFullNameForScripting(sp, nameIsIndentifier: true);
				switch (GetType().Name)
				{
				case "StoredProcedure":
				{
					ExecutionContext executionContext = (ExecutionContext)property.Value;
					if (executionContext != ExecutionContext.Caller)
					{
						message = ExceptionTemplatesImpl.StoredProcedureDownlevelExecutionContext(objectName, GetExecutionContextString(executionContext), sqlServerName);
						flag = false;
					}
					break;
				}
				case "UserDefinedFunction":
				{
					ExecutionContext executionContext2 = (ExecutionContext)property.Value;
					if (executionContext2 != ExecutionContext.Caller)
					{
						message = ExceptionTemplatesImpl.UserDefinedFunctionDownlevelExecutionContext(objectName, GetExecutionContextString(executionContext2), sqlServerName);
						flag = false;
					}
					break;
				}
				case "Trigger":
				{
					ExecutionContext executionContext3 = (ExecutionContext)property.Value;
					if (executionContext3 != ExecutionContext.Caller)
					{
						message = ExceptionTemplatesImpl.TriggerDownlevelExecutionContext(objectName, GetExecutionContextString(executionContext3), sqlServerName);
						flag = false;
					}
					break;
				}
				case "DatabaseDdlTrigger":
				{
					DatabaseDdlTriggerExecutionContext databaseDdlTriggerExecutionContext = (DatabaseDdlTriggerExecutionContext)property.Value;
					if (databaseDdlTriggerExecutionContext != DatabaseDdlTriggerExecutionContext.Caller)
					{
						message = ExceptionTemplatesImpl.TriggerDownlevelExecutionContext(objectName, GetExecutionContextString(databaseDdlTriggerExecutionContext), sqlServerName);
						flag = false;
					}
					break;
				}
				case "ServerDdlTrigger":
				{
					ServerDdlTriggerExecutionContext serverDdlTriggerExecutionContext = (ServerDdlTriggerExecutionContext)property.Value;
					if (serverDdlTriggerExecutionContext != ServerDdlTriggerExecutionContext.Caller)
					{
						message = ExceptionTemplatesImpl.TriggerDownlevelExecutionContext(objectName, GetExecutionContextString(serverDdlTriggerExecutionContext), sqlServerName);
						flag = false;
					}
					break;
				}
				}
				if (!flag)
				{
					throw new InvalidSmoOperationException(message);
				}
			}
		}
		if (text == null || text.Length <= 0)
		{
			if (base.State != SqlSmoState.Creating)
			{
				if (base.Properties.Contains("IsEncrypted") && (bool)GetPropValue("IsEncrypted"))
				{
					throw new PropertyCannotBeRetrievedException(requestingProperty, this, ExceptionTemplatesImpl.ReasonTextIsEncrypted);
				}
				throw new PropertyCannotBeRetrievedException(requestingProperty, this);
			}
			if (bThrowIfCreating)
			{
				throw new PropertyNotSetException(requestingProperty);
			}
		}
		return text;
	}

	private string GetTextPropertyDesignMode(string requestingProperty, ScriptingPreferences sp, bool bThrowIfCreating)
	{
		if (sp != null && sp.TargetServerVersion == SqlServerVersion.Version80)
		{
			string message = ExceptionTemplatesImpl.InvalidVersionSmoOperation(LocalizableResources.ServerShiloh);
			throw new UnsupportedVersionException(message);
		}
		string text = GetPropValueOptional("Text") as string;
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (m_textHeader != null && m_textBody != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, m_textHeader);
			if (m_textHeader.Length > 0 && !char.IsWhiteSpace(m_textHeader[m_textHeader.Length - 1]) && m_textBody.Length > 0 && !char.IsWhiteSpace(m_textBody[0]))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, m_textBody);
		}
		if (string.IsNullOrEmpty(stringBuilder.ToString()))
		{
			if (base.State != SqlSmoState.Creating)
			{
				throw new PropertyCannotBeRetrievedException(requestingProperty, this);
			}
			if (bThrowIfCreating)
			{
				throw new PropertyNotSetException(requestingProperty);
			}
		}
		return stringBuilder.ToString();
	}
}
