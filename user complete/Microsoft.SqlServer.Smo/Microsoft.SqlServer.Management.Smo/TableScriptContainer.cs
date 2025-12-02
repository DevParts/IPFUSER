using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.SqlServer.Management.Smo;

internal class TableScriptContainer : IdBasedObjectScriptContainer
{
	public IEnumerable<string> DataScript { get; private set; }

	public ScriptFragment BindingsScript { get; protected set; }

	public TableScriptContainer(Table table, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
		: base(table, sp, retryEvent)
	{
	}

	protected override void Initialize(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		if (sp.IncludeScripts.Data)
		{
			ScriptData(obj, sp, retryEvent);
			if (!sp.IncludeScripts.Ddl)
			{
				if (sp.IncludeScripts.DatabaseContext)
				{
					base.DatabaseContext = ScriptMaker.ScriptDatabaseContext(obj, isScriptingPermission: false);
				}
				ScriptDropData(obj, sp);
			}
			else
			{
				base.Initialize(obj, sp, retryEvent);
			}
			if (sp.IncludeScripts.Ddl && sp.OldOptions.Bindings)
			{
				BindingsScript = GenerateScript(sp, ((Table)obj).ScriptBindings, retryEvent, obj.Urn);
			}
		}
		else
		{
			base.Initialize(obj, sp, retryEvent);
		}
	}

	private void ScriptData(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		if (sp.Behavior == ScriptBehavior.Drop)
		{
			return;
		}
		try
		{
			try
			{
				DataScript = new DataScriptCollection(new DataEnumerator((Table)obj, sp));
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || retryEvent == null)
				{
					throw;
				}
				RetryRequestedEventArgs e = new RetryRequestedEventArgs(obj.Urn, (ScriptingPreferences)sp.Clone());
				retryEvent(this, e);
				if (e.ShouldRetry)
				{
					DataScript = ScriptMaker.SurroundWithRetryTexts(new DataScriptCollection(new DataEnumerator((Table)obj, e.ScriptingPreferences)), e);
					return;
				}
				throw;
			}
		}
		catch (Exception ex2)
		{
			if (ObjectScriptContainer.ThrowException(sp, ex2))
			{
				throw;
			}
		}
	}

	private void ScriptDropData(SqlSmoObject obj, ScriptingPreferences sp)
	{
		if (sp.Behavior == ScriptBehavior.Create)
		{
			return;
		}
		try
		{
			StringCollection script = ((Table)obj).ScriptDropData(sp);
			base.DropScript = new ScriptFragment(script);
		}
		catch (Exception ex)
		{
			base.DropScript = new ScriptFragment(ex);
			if (ObjectScriptContainer.ThrowException(sp, ex))
			{
				throw;
			}
		}
	}
}
