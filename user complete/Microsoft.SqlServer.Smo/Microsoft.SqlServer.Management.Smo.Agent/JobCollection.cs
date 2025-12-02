using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public class JobCollection : ArrayListCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public Job this[int index] => GetObjectByIndex(index) as Job;

	public Job this[string name] => GetObjectByName(name) as Job;

	public Job this[string name, int categoryID]
	{
		get
		{
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				Job job = (Job)enumerator.Current;
				if (StringComparer.Compare(job.Name, name) == 0 && job.CategoryID == categoryID)
				{
					return job;
				}
			}
			return null;
		}
	}

	internal JobCollection(SqlSmoObject parent)
		: base(parent)
	{
	}

	protected override void InitInnerCollection()
	{
		base.InternalStorage = new SmoArrayList(new JobObjectComparer(StringComparer), this);
	}

	public bool Contains(string name)
	{
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Job job = (Job)enumerator.Current;
			if (StringComparer.Compare(job.Name, name) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(string name, int categoryID)
	{
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Job job = (Job)enumerator.Current;
			if (StringComparer.Compare(job.Name, name) == 0 && job.CategoryID == categoryID)
			{
				return true;
			}
		}
		return false;
	}

	internal override ObjectKeyBase CreateKeyFromUrn(Urn urn)
	{
		string attribute = urn.GetAttribute("Name");
		string attribute2 = urn.GetAttribute("CategoryID");
		int categoryID = ((attribute2 == null) ? (-1) : int.Parse(attribute2, SmoApplication.DefaultCulture));
		return new JobObjectKey(attribute, categoryID);
	}

	public Job ItemById(Guid id)
	{
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Job job = (Job)enumerator.Current;
			Property property = job.Properties.Get("JobID");
			if (property.Value != null)
			{
				if (id.Equals(property.Value))
				{
					return job;
				}
				continue;
			}
			if (job.State != SqlSmoState.Creating)
			{
				job.Initialize(allProperties: true);
			}
			if (property.Value != null && id.Equals(property.Value))
			{
				return job;
			}
		}
		return null;
	}

	public void CopyTo(Job[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
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
		return typeof(Job);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Job(this, key, state);
	}

	public void Add(Job job)
	{
		AddImpl(job);
	}

	internal SqlSmoObject GetObjectByName(string name)
	{
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Job job = (Job)enumerator.Current;
			if (StringComparer.Compare(job.Name, name) == 0)
			{
				return job;
			}
		}
		return null;
	}

	internal override SqlSmoObject GetObjectByKey(ObjectKeyBase key)
	{
		JobObjectKey jobObjectKey = (JobObjectKey)key;
		IEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Job job = (Job)enumerator.Current;
			if (StringComparer.Compare(job.Name, jobObjectKey.Name) == 0)
			{
				if (jobObjectKey.CategoryID == -1)
				{
					return job;
				}
				if (job.CategoryID == jobObjectKey.CategoryID)
				{
					return job;
				}
			}
		}
		return null;
	}
}
