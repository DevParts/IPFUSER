using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class AlertCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public Alert this[int index] => GetObjectByIndex(index) as Alert;

	public Alert this[string name] => GetObjectByName(name) as Alert;

	internal AlertCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Alert[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Alert ItemById(int id)
	{
		return (Alert)GetItemById(id);
	}

	public StringCollection Script()
	{
		return Script(new ScriptingOptions());
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		if (base.Count <= 0)
		{
			return new StringCollection();
		}
		SqlSmoObject[] array = new SqlSmoObject[base.Count];
		int num = 0;
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SqlSmoObject sqlSmoObject = (SqlSmoObject)enumerator.Current;
				array[num++] = sqlSmoObject;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		Scripter scripter = new Scripter(array[0].GetServerObject());
		scripter.Options = scriptingOptions;
		return scripter.Script(array);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Alert);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Alert(this, key, state);
	}

	public void Add(Alert alert)
	{
		AddImpl(alert);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		return GetObjectByKey(new SimpleObjectKey(name));
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		if (attribute == null || attribute.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyMustBeSpecifiedInUrn("Name", urn.Type));
		}
		return new SimpleObjectKey(attribute);
	}
}
