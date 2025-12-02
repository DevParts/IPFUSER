using System.Collections.Generic;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class ScriptContainerFactory
{
	private Dictionary<Urn, ScriptContainer> scriptCollection;

	private ScriptingPreferences scriptingPreferences;

	private RetryRequestedEventHandler retry;

	private HashSet<UrnTypeKey> filteredTypes;

	public ScriptContainerFactory(ScriptingPreferences sp, HashSet<UrnTypeKey> filteredTypes, RetryRequestedEventHandler retryEvent)
	{
		scriptingPreferences = sp;
		this.filteredTypes = filteredTypes;
		retry = retryEvent;
		scriptCollection = new Dictionary<Urn, ScriptContainer>();
	}

	public void AddContainer(SqlSmoObject obj)
	{
		if (filteredTypes.Contains(new UrnTypeKey(obj.Urn)))
		{
			return;
		}
		if (scriptingPreferences.IncludeScripts.Ddl)
		{
			switch (obj.Urn.Type)
			{
			case "Table":
			{
				TableScriptContainer value4 = new TableScriptContainer(obj as Table, scriptingPreferences, retry);
				scriptCollection.Add(obj.Urn, value4);
				break;
			}
			case "Index":
			{
				IndexScriptContainer value3 = new IndexScriptContainer(obj as Index, scriptingPreferences, retry);
				scriptCollection.Add(obj.Urn, value3);
				break;
			}
			case "View":
			{
				IdBasedObjectScriptContainer value2 = new IdBasedObjectScriptContainer(obj, scriptingPreferences, retry);
				scriptCollection.Add(obj.Urn, value2);
				break;
			}
			default:
			{
				ObjectScriptContainer value = new ObjectScriptContainer(obj, scriptingPreferences, retry);
				scriptCollection.Add(obj.Urn, value);
				break;
			}
			}
		}
		else if (obj.Urn.Type.Equals(Table.UrnSuffix))
		{
			TableScriptContainer value5 = new TableScriptContainer(obj as Table, scriptingPreferences, retry);
			scriptCollection.Add(obj.Urn, value5);
		}
	}

	public bool TryGetValue(Urn key, out ScriptContainer value)
	{
		return scriptCollection.TryGetValue(key, out value);
	}
}
