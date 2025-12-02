using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class TransferWriter : ISmoScriptWriter
{
	private delegate bool WriteToCollection(ObjectScriptingEventArgs e, out List<string> collection);

	private TransferBase transfer;

	private ScriptMaker scriptMaker;

	private bool dataScriptingStarted;

	private int orderedUrns;

	private bool DropMode;

	private bool originalExistenceCheck;

	private IEnumerable<string> lastScriptFragment;

	private Dictionary<string, WriteToCollection> actions;

	public List<string> Prologue { get; private set; }

	public List<string> Epilogue { get; private set; }

	public List<string> PreTransaction { get; private set; }

	public List<string> PostTransaction { get; private set; }

	public List<Urn> Tables { get; private set; }

	public string Header { private get; set; }

	public TransferWriter(TransferBase transfer, ScriptMaker scriptMaker)
	{
		if (transfer == null)
		{
			throw new ArgumentNullException("transfer");
		}
		if (scriptMaker == null)
		{
			throw new ArgumentNullException("scriptMaker");
		}
		this.transfer = transfer;
		this.scriptMaker = scriptMaker;
		if (scriptMaker.Preferences.Behavior == ScriptBehavior.DropAndCreate)
		{
			DropMode = true;
			originalExistenceCheck = scriptMaker.Preferences.IncludeScripts.ExistenceCheck;
			this.scriptMaker.Preferences.IncludeScripts.ExistenceCheck = true;
		}
		string item = string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(this.transfer.DestinationDatabase) });
		Prologue = new List<string> { item };
		Epilogue = new List<string> { item };
		PreTransaction = (this.transfer.CreateTargetDatabase ? new List<string>() : new List<string> { item });
		PostTransaction = new List<string> { item };
		Tables = new List<Urn>();
		InitializeActions();
	}

	private void InitializeActions()
	{
		actions = new Dictionary<string, WriteToCollection>();
		actions.Add("User", HandleSecurityObject);
		actions.Add("ApplicationRole", HandleSecurityObject);
		actions.Add("Role", HandleSecurityObject);
		actions.Add("Login", HandleLogin);
		actions.Add("FullTextCatalog", HandleFullTextObject);
		actions.Add("FullTextStopList", HandleFullTextObject);
		actions.Add("SearchPropertyList", HandleFullTextObject);
		actions.Add("FullTextIndex", HandleFullTextObject);
		actions.Add("Endpoint", HandleEndPoint);
		actions.Add("Database", HandleDatabase);
		actions.Add("ExtendedProperty", HandleExtendedProperty);
	}

	private bool HandleSecurityObject(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = PreTransaction;
		return true;
	}

	private bool HandleLogin(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = null;
		if (DropMode && transfer.PreserveLogins)
		{
			return false;
		}
		collection = PreTransaction;
		return HandleSecurityObject(e, out collection);
	}

	private bool HandleFullTextObject(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = (DropMode ? PreTransaction : PostTransaction);
		return true;
	}

	private bool HandleEndPoint(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = null;
		if (DropMode)
		{
			collection = PreTransaction;
			return true;
		}
		Endpoint endpoint = transfer.Database.Parent.GetSmoObject(e.Current) as Endpoint;
		ScriptingPreferences scriptingPreferences = scriptMaker.Preferences.Clone() as ScriptingPreferences;
		scriptingPreferences.Behavior = ScriptBehavior.Create;
		scriptingPreferences.IncludeScripts.Owner = false;
		scriptingPreferences.IncludeScripts.Permissions = false;
		StringCollection stringCollection = new StringCollection();
		endpoint.ScriptCreateInternal(stringCollection, scriptingPreferences, skipPropagateScript: true);
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				PreTransaction.Add(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		stringCollection.Clear();
		endpoint.ScriptChangeOwner(stringCollection, scriptingPreferences);
		endpoint.AddScriptPermission(stringCollection, scriptingPreferences);
		StringEnumerator enumerator2 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current2 = enumerator2.Current;
				PostTransaction.Add(current2);
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable2)
			{
				disposable2.Dispose();
			}
		}
		return false;
	}

	private bool HandleDatabase(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = PreTransaction;
		if (e.Current == transfer.Database.Urn)
		{
			if (DropMode)
			{
				return false;
			}
			if (e.ScriptType == ObjectScriptingType.Object)
			{
				if (e.Original.Parent.Type.Equals("databasereadonly"))
				{
					collection = PostTransaction;
				}
				else
				{
					Database database = transfer.Database.Parent.GetSmoObject(e.Current) as Database;
					ScriptingPreferences scriptingPreferences = scriptMaker.Preferences.Clone() as ScriptingPreferences;
					scriptingPreferences.Behavior = ScriptBehavior.Create;
					scriptingPreferences.IncludeScripts.Owner = false;
					scriptingPreferences.IncludeScripts.Permissions = false;
					scriptingPreferences.IncludeScripts.ExistenceCheck = false;
					StringCollection stringCollection = new StringCollection();
					database.ScriptCreateInternal(stringCollection, scriptingPreferences, skipPropagateScript: true);
					EnumerableContainer enumerableContainer = new EnumerableContainer();
					enumerableContainer.Add(stringCollection);
					enumerableContainer.Add(new List<string> { string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(transfer.DestinationDatabase) }) });
					lastScriptFragment = enumerableContainer;
				}
			}
			else if (e.ScriptType == ObjectScriptingType.OwnerShip)
			{
				if (!transfer.PreserveDbo)
				{
					return false;
				}
				collection = PostTransaction;
			}
			else if (e.ScriptType == ObjectScriptingType.Permission)
			{
				collection = PostTransaction;
			}
		}
		return true;
	}

	private bool HandleExtendedProperty(ObjectScriptingEventArgs e, out List<string> collection)
	{
		collection = null;
		switch (e.Current.Type)
		{
		case "FullTextCatalog":
		case "FullTextStopList":
		case "SearchPropertyList":
			collection = (DropMode ? PreTransaction : PostTransaction);
			break;
		default:
			collection = (DropMode ? PreTransaction : (dataScriptingStarted ? Epilogue : Prologue));
			break;
		}
		return true;
	}

	private void scriptMaker_ScriptingProgress(object sender, ScriptingProgressEventArgs e)
	{
		if (scriptMaker.Preferences.Behavior == ScriptBehavior.DropAndCreate && e.ProgressStage == ScriptingProgressStages.OrderingDone)
		{
			orderedUrns = e.Urns.Count;
		}
	}

	private void scriptMaker_ObjectScripting(object sender, ObjectScriptingEventArgs e)
	{
		HandleScriptingEvent(e);
		CheckDropCreateState(e);
	}

	private void HandleScriptingEvent(ObjectScriptingEventArgs e)
	{
		if (e.ScriptType != ObjectScriptingType.None && (!DropMode || !transfer.CreateTargetDatabase || e.Current.XPathExpression.Length < 3 || !(e.Current.XPathExpression[1].Name == "Database")))
		{
			List<string> collection = null;
			if (e.ScriptType == ObjectScriptingType.Data)
			{
				collection = Epilogue;
			}
			else
			{
				WriteToCollection value = null;
				if (actions.TryGetValue(e.Current.Type, out value))
				{
					if (!value(e, out collection))
					{
						collection = null;
					}
				}
				else
				{
					collection = (DropMode ? PreTransaction : (dataScriptingStarted ? Epilogue : Prologue));
				}
			}
			if (lastScriptFragment != null)
			{
				collection?.AddRange(lastScriptFragment);
			}
		}
		lastScriptFragment = null;
	}

	private void CheckDropCreateState(ObjectScriptingEventArgs e)
	{
		if (scriptMaker.Preferences.Behavior == ScriptBehavior.DropAndCreate)
		{
			orderedUrns--;
			if (orderedUrns == 0)
			{
				DropMode = false;
				scriptMaker.Preferences.IncludeScripts.ExistenceCheck = originalExistenceCheck;
			}
		}
	}

	public void ScriptObject(IEnumerable<string> script, Urn obj)
	{
		lastScriptFragment = script;
	}

	public void ScriptData(IEnumerable<string> dataScript, Urn table)
	{
		dataScriptingStarted = true;
		Tables.Add(table);
	}

	public void ScriptContext(string databaseContext, Urn obj)
	{
		throw new InvalidOperationException();
	}

	internal void SetEvents()
	{
		scriptMaker.ObjectScripting += scriptMaker_ObjectScripting;
		scriptMaker.ScriptingProgress += scriptMaker_ScriptingProgress;
	}

	internal void ResetEvents()
	{
		scriptMaker.ObjectScripting -= scriptMaker_ObjectScripting;
		scriptMaker.ScriptingProgress -= scriptMaker_ScriptingProgress;
	}
}
