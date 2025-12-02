using System;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class AffinityInfoBase : IAlterable, IScriptable
{
	internal DataTable table;

	private NumaNodeCollection numaCol;

	private AffinityType affinityType;

	internal DataTable AffinityInfoTable
	{
		get
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			return table;
		}
	}

	public NumaNodeCollection NumaNodes
	{
		get
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			if (numaCol == null)
			{
				numaCol = new NumaNodeCollection(this);
			}
			return numaCol;
		}
	}

	public AffinityType AffinityType
	{
		get
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			return affinityType;
		}
		set
		{
			if (table == null)
			{
				PopulateDataTable();
			}
			affinityType = value;
		}
	}

	internal abstract SqlSmoObject SmoParent { get; }

	public ExecutionManager ExecutionManager => SmoParent.ExecutionManager;

	internal abstract void PopulateDataTable();

	public virtual void Refresh()
	{
		table = null;
		numaCol = null;
	}

	internal abstract StringCollection DoAlter(ScriptingPreferences sp);

	public void Alter()
	{
		StringCollection stringCollection = new StringCollection();
		ScriptingPreferences currentServerScriptingPreferences = GetCurrentServerScriptingPreferences();
		try
		{
			stringCollection = DoAlter(currentServerScriptingPreferences);
			if (stringCollection != null)
			{
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			if (!ExecutionManager.Recording)
			{
				Refresh();
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, ex);
		}
	}

	public StringCollection Script()
	{
		return Script(GetCurrentServerScriptingPreferences());
	}

	internal StringCollection Script(ScriptingPreferences sp)
	{
		return DoAlter(sp);
	}

	public StringCollection Script(ScriptingOptions so)
	{
		return DoAlter(so.GetScriptingPreferences());
	}

	private ScriptingPreferences GetCurrentServerScriptingPreferences()
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.SetTargetServerInfo(SmoParent);
		return scriptingPreferences;
	}
}
