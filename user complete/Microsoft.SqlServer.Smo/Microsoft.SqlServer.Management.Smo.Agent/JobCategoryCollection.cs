using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class JobCategoryCollection : SimpleObjectCollectionBase
{
	public JobServer Parent => base.ParentInstance as JobServer;

	public JobCategory this[int index] => GetObjectByIndex(index) as JobCategory;

	public JobCategory this[string name] => GetObjectByName(name) as JobCategory;

	internal JobCategoryCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(JobCategory[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public JobCategory ItemById(int id)
	{
		return (JobCategory)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(JobCategory);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new JobCategory(this, key, state);
	}

	public void Remove(JobCategory jobCategory)
	{
		if (jobCategory == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("jobCategory"));
		}
		RemoveObj(jobCategory, new SimpleObjectKey(jobCategory.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(JobCategory jobCategory)
	{
		AddImpl(jobCategory);
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
