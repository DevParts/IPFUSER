using System;
using System.Collections;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class DatabaseCollection : SimpleObjectCollectionBase
{
	public Server Parent => base.ParentInstance as Server;

	public Database this[int index] => GetObjectByIndex(index) as Database;

	public Database this[string name]
	{
		get
		{
			try
			{
				return GetObjectByName(name) as Database;
			}
			catch (ConnectionFailureException ex)
			{
				if (ex.InnerException is SqlException && (ex.InnerException as SqlException).Number == 4060)
				{
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.LogExCatch(ex);
					return null;
				}
				throw ex;
			}
		}
	}

	internal DatabaseCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void CopyTo(Database[] array, int index)
	{
		((ICollection)this).CopyTo((Array)array, index);
	}

	public Database ItemById(int id)
	{
		return (Database)GetItemById(id);
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(Database);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new Database(this, key, state);
	}

	public void Add(Database database)
	{
		AddImpl(database);
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
