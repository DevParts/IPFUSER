using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobStepCollection : ParameterCollectionBase
{
	public Job Parent => base.ParentInstance as Job;

	public JobStep this[int index] => GetObjectByIndex(index) as JobStep;

	public JobStep this[string name] => GetObjectByKey(new SimpleObjectKey(name)) as JobStep;

	internal JobStepCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(JobStep[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(JobStep);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new JobStep(this, key, state);
	}

	public void Add(JobStep jobStep)
	{
		AddImpl(jobStep);
	}

	public void Add(JobStep jobStep, string insertAtColumnName)
	{
		AddImpl(jobStep, new SimpleObjectKey(insertAtColumnName));
	}

	public void Add(JobStep jobStep, int insertAtPosition)
	{
		AddImpl(jobStep, insertAtPosition);
	}

	public void Remove(JobStep jobStep)
	{
		if (jobStep == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("jobStep"));
		}
		RemoveObj(jobStep, jobStep.key);
	}

	public JobStep ItemById(int id)
	{
		return (JobStep)GetItemById(id);
	}
}
