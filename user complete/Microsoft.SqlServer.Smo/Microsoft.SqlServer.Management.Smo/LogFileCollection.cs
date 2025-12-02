using System;
using System.Collections;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class LogFileCollection : SimpleObjectCollectionBase
{
	public Database Parent => base.ParentInstance as Database;

	public LogFile this[int index] => GetObjectByIndex(index) as LogFile;

	public LogFile this[string name] => GetObjectByName(name) as LogFile;

	internal LogFileCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(LogFile[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public LogFile ItemById(int id)
	{
		return (LogFile)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(LogFile);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new LogFile(this, key, state);
	}

	public void Remove(LogFile logFile)
	{
		if (logFile == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveCollection, this, new ArgumentNullException("logFile"));
		}
		RemoveObj(logFile, new SimpleObjectKey(logFile.Name));
	}

	public void Remove(string name)
	{
		Remove(new SimpleObjectKey(name));
	}

	public void Add(LogFile logFile)
	{
		AddImpl(logFile);
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
