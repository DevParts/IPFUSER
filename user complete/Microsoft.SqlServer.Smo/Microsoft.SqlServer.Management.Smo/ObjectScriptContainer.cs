using System;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ObjectScriptContainer : ScriptContainer
{
	public ObjectScriptContainer(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		Initialize(obj, sp, retryEvent);
	}

	protected virtual void Initialize(SqlSmoObject obj, ScriptingPreferences sp, RetryRequestedEventHandler retryEvent)
	{
		if (sp.Behavior == ScriptBehavior.Drop || sp.Behavior == ScriptBehavior.DropAndCreate)
		{
			base.DropScript = GenerateScript(sp, obj.ScriptDropInternal, retryEvent, obj.Urn);
		}
		if (sp.Behavior == ScriptBehavior.Create || sp.Behavior == ScriptBehavior.DropAndCreate)
		{
			base.CreateScript = GenerateScript(sp, delegate(StringCollection x, ScriptingPreferences y)
			{
				obj.ScriptCreateInternal(x, y, skipPropagateScript: true);
			}, retryEvent, obj.Urn);
		}
		if (sp.IncludeScripts.DatabaseContext)
		{
			base.DatabaseContext = ScriptMaker.ScriptDatabaseContext(obj, isScriptingPermission: false);
		}
	}

	protected ScriptFragment GenerateScript(ScriptingPreferences sp, ScriptGenerator scriptGenerator, RetryRequestedEventHandler retryEvent, Urn urn)
	{
		try
		{
			try
			{
				StringCollection stringCollection = new StringCollection();
				scriptGenerator(stringCollection, sp);
				return new ScriptFragment(stringCollection);
			}
			catch (Exception ex)
			{
				if (ex is OutOfMemoryException || retryEvent == null)
				{
					throw;
				}
				RetryRequestedEventArgs e = new RetryRequestedEventArgs(urn, (ScriptingPreferences)sp.Clone());
				retryEvent(this, e);
				if (e.ShouldRetry)
				{
					StringCollection stringCollection2 = new StringCollection();
					scriptGenerator(stringCollection2, e.ScriptingPreferences);
					ScriptMaker.SurroundWithRetryTexts(stringCollection2, e);
					return new ScriptFragment(stringCollection2);
				}
				throw;
			}
		}
		catch (Exception ex2)
		{
			if (ThrowException(sp, ex2))
			{
				throw;
			}
			return new ScriptFragment(ex2);
		}
	}

	protected static bool ThrowException(ScriptingPreferences sp, Exception ex)
	{
		if (!(ex is OutOfMemoryException))
		{
			return !sp.ContinueOnScriptingError;
		}
		return true;
	}
}
