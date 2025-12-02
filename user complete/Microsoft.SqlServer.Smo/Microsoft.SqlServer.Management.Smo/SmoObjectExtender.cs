using System;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class SmoObjectExtender<T> : SfcObjectExtender<T> where T : SqlSmoObject, new()
{
	public SmoObjectExtender()
	{
		Initialize();
	}

	public SmoObjectExtender(T obj)
		: base(obj)
	{
		Initialize();
	}

	private void Initialize()
	{
		try
		{
			RegisterParentProperty(typeof(T).GetProperty("Name"));
		}
		catch (Exception)
		{
		}
	}

	protected void PropagateAlterToChildren(StringCollection script, bool scriptParent = false)
	{
		ScriptingPreferences sp = new ScriptingPreferences(base.Parent);
		if (scriptParent)
		{
			T val = base.Parent;
			val.ScriptAlter(script, sp);
		}
		T val2 = base.Parent;
		val2.PropagateScript(script, sp, SqlSmoObject.PropagateAction.Alter);
	}

	protected override ISfcPropertySet GetParentSfcPropertySet()
	{
		if (base.Parent != null)
		{
			try
			{
				T val = base.Parent;
				val.CheckPendingState();
			}
			catch
			{
				T val2 = base.Parent;
				T val3 = base.Parent;
				return new SqlPropertyCollection(val2, val3.GetPropertyMetadataProvider());
			}
		}
		return base.GetParentSfcPropertySet();
	}
}
