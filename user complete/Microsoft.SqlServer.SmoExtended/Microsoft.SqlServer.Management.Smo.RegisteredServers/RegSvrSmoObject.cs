using System;
using System.Data;
using System.Reflection;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public abstract class RegSvrSmoObject : SmoObjectBase
{
	private RegSvrCollectionBase parentColl;

	internal ObjectKeyBase key;

	internal PropertyCollection m_properties;

	internal RegSvrCollectionBase ParentColl => parentColl;

	internal virtual string CollectionPath
	{
		get
		{
			if (ParentColl == null)
			{
				return string.Empty;
			}
			return ParentColl.ParentInstance.CollectionPath;
		}
	}

	internal virtual RegSvrConnectionInfo ConnectionInfo => ParentColl.ParentInstance.ConnectionInfo;

	public virtual Urn Urn => null;

	protected internal virtual Urn UrnSkeleton => null;

	public string Name
	{
		get
		{
			return InternalName;
		}
		set
		{
			try
			{
				ValidateName(value);
				((SimpleObjectKey)key).Name = value;
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, ex);
			}
		}
	}

	internal string InternalName => ((SimpleObjectKey)key).Name;

	public PropertyCollection Properties
	{
		get
		{
			if (m_properties == null)
			{
				m_properties = new PropertyCollection(this, GetPropertyMetadataProvider());
			}
			return m_properties;
		}
	}

	protected internal RegSvrSmoObject(RegSvrCollectionBase parentColl, string name)
	{
		propertyBagState = PropertyBagState.Empty;
		key = new SimpleObjectKey(name);
		this.parentColl = parentColl;
		UpdateObjectState();
	}

	protected internal RegSvrSmoObject()
	{
		key = new SimpleObjectKey(null);
		propertyBagState = PropertyBagState.Empty;
	}

	internal virtual PropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return null;
	}

	internal void SetObjectKey(ObjectKeyBase key)
	{
		this.key = key;
	}

	internal void ValidateName(string name)
	{
		if (name == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Name"));
		}
		if (base.State != SqlSmoState.Pending)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
		}
	}

	protected internal void SetParentImpl(RegSvrSmoObject newParent)
	{
		if (newParent == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("newParent"));
		}
		if (base.State != SqlSmoState.Pending)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
		}
		if (newParent.State == SqlSmoState.Pending)
		{
			throw new InvalidSmoOperationException();
		}
		if (this is ServerGroup)
		{
			parentColl = ((ServerGroupBase)newParent).ServerGroups;
		}
		else
		{
			parentColl = ((ServerGroupBase)newParent).RegisteredServers;
		}
		UpdateObjectState();
	}

	protected void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && !key.IsNull && parentColl != null)
		{
			SetState(SqlSmoState.Creating);
		}
	}

	internal static void FilterException(Exception e)
	{
		if (e is OutOfMemoryException)
		{
			throw e;
		}
	}

	internal override object OnPropertyMissing(string propname, bool useDefaultValue)
	{
		switch (propertyBagState)
		{
		case PropertyBagState.Empty:
			Initialize();
			break;
		case PropertyBagState.Lazy:
			Initialize();
			break;
		case PropertyBagState.Full:
			throw new InternalSmoErrorException(ExceptionTemplatesImpl.FullPropertyBag(propname));
		}
		return Properties.Get(propname).Value;
	}

	public bool Initialize()
	{
		if (propertyBagState == PropertyBagState.Full)
		{
			return false;
		}
		string[] fields = null;
		OrderBy[] array = null;
		bool flag = ImplInitialize(fields, array);
		if (flag)
		{
			propertyBagState = PropertyBagState.Full;
		}
		return flag;
	}

	protected virtual bool ImplInitialize(string[] fields, OrderBy[] orderby)
	{
		Urn urn = Urn;
		Request request = new Request();
		request.Urn = urn;
		request.Fields = fields;
		request.OrderByList = orderby;
		Enumerator enumerator = new Enumerator();
		DataSet dataSet = enumerator.Process(ConnectionInfo, request);
		DataTable dataTable = dataSet.Tables[0];
		if (dataTable.Rows.Count < 1)
		{
			Trace("Failed to Initialize urn " + urn);
			return false;
		}
		AddObjectProps(dataTable.Columns, dataTable.Rows[0]);
		return true;
	}

	internal void AddObjectProps(DataColumnCollection columns, DataRow dr)
	{
		int num = 0;
		for (int i = 0; i < columns.Count; i++)
		{
			DataColumn dataColumn = columns[i];
			if (1 > num && string.Compare(dataColumn.ColumnName, "Name", StringComparison.Ordinal) == 0)
			{
				num++;
				continue;
			}
			Property property = Properties.Get(dataColumn.ColumnName);
			if (property == null)
			{
				throw new InternalSmoErrorException(ExceptionTemplatesImpl.UnknownProperty(dataColumn.ColumnName, GetType().ToString()));
			}
			if (property.Enumeration)
			{
				property.SetValue(Enum.ToObject(property.Type, Convert.ToInt32(dr[i], SmoApplication.DefaultCulture)));
			}
			else if (DBNull.Value.Equals(dr[i]))
			{
				if (property.Type.Equals(typeof(string)))
				{
					property.SetValue(string.Empty);
				}
				else
				{
					property.SetValue(null);
				}
			}
			else
			{
				property.SetValue(dr[i]);
			}
			property.SetRetrieved(retrieved: true);
		}
	}

	protected internal bool IsObjectInitialized()
	{
		if ((base.State != SqlSmoState.Existing && base.State != SqlSmoState.ToBeDropped) || propertyBagState == PropertyBagState.Full)
		{
			return true;
		}
		return false;
	}

	public virtual void Refresh()
	{
		m_properties = null;
		propertyBagState = PropertyBagState.Empty;
		Initialize();
	}

	protected void CheckObjectState()
	{
		if (base.State == SqlSmoState.Dropped)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectDroppedExceptionText(GetType().ToString(), Name));
		}
		if (base.State == SqlSmoState.Pending)
		{
			throw new InvalidSmoOperationException("", SqlSmoState.Pending);
		}
	}

	internal void EnumChildren(string childTypeName, RegSvrCollectionBase coll)
	{
		Type type = Type.GetType(childTypeName, throwOnError: true);
		Request request = new Request();
		string text = (string)type.InvokeMember("UrnSuffix", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, new object[0], SmoApplication.DefaultCulture);
		request.Urn = string.Format(SmoApplication.DefaultCulture, "{0}/{1}[@Path='{2}']", new object[3]
		{
			ServerType.UrnSuffix,
			text,
			Urn.EscapeString(CollectionPath)
		});
		request.Fields = new string[1] { "Name" };
		request.OrderByList = new OrderBy[1]
		{
			new OrderBy("Name", OrderBy.Direction.Asc)
		};
		Enumerator enumerator = new Enumerator();
		DataSet dataSet = enumerator.Process(ConnectionInfo, request);
		DataTable dataTable = dataSet.Tables[0];
		foreach (DataRow row in dataTable.Rows)
		{
			object[] args = new object[2]
			{
				coll,
				(string)row["Name"]
			};
			object obj = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, args, null);
			if (obj == null)
			{
				throw new InternalSmoErrorException(ExceptionTemplatesImpl.CantCreateType(childTypeName));
			}
			if (dataTable.Columns.Count > 1)
			{
				((RegSvrSmoObject)obj).AddObjectProps(dataTable.Columns, row);
			}
			((RegSvrSmoObject)obj).SetState(PropertyBagState.Empty);
			((RegSvrSmoObject)obj).SetState(SqlSmoState.Existing);
			coll.AddInternal((RegSvrSmoObject)obj);
		}
	}

	protected static void Trace(string traceText)
	{
	}
}
