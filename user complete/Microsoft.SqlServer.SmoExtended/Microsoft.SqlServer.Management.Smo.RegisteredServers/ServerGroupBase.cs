using System;
using System.Reflection;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

public class ServerGroupBase : RegSvrSmoObject, ICreatable, IAlterable, IDroppable
{
	private RegisteredServerCollection registeredServers;

	private ServerGroupCollection serverGroup;

	internal override string CollectionPath
	{
		get
		{
			if (base.ParentColl == null)
			{
				return string.Empty;
			}
			string text = base.ParentColl.ParentInstance.CollectionPath;
			if (text.Length > 0)
			{
				text += '/';
			}
			return text + string.Format(SmoApplication.DefaultCulture, "ServerGroup[@Name='{0}']", new object[1] { Urn.EscapeString(base.Name) });
		}
	}

	public RegisteredServerCollection RegisteredServers
	{
		get
		{
			CheckObjectState();
			if (registeredServers == null)
			{
				registeredServers = new RegisteredServerCollection(this);
			}
			return registeredServers;
		}
	}

	public ServerGroupCollection ServerGroups
	{
		get
		{
			CheckObjectState();
			if (serverGroup == null)
			{
				serverGroup = new ServerGroupCollection(this);
			}
			return serverGroup;
		}
	}

	internal ServerGroupBase(RegSvrCollectionBase parentColl, string name)
		: base(parentColl, name)
	{
	}

	internal ServerGroupBase()
	{
	}

	protected internal void ClearCollections()
	{
		serverGroup = new ServerGroupCollection(this);
		registeredServers = new RegisteredServerCollection(this);
	}

	public RegSvrSmoObject GetSmoObject(Urn urn)
	{
		if (null == urn)
		{
			throw new ArgumentNullException("urn");
		}
		return GetSmoObjectRec(urn);
	}

	private RegSvrSmoObject GetSmoObjectRec(Urn urn)
	{
		if (null == urn.Parent)
		{
			if (urn.Type == "ServerGroup")
			{
				if (urn.GetAttribute("Name") == base.Name)
				{
					return this;
				}
				throw new SmoException(ExceptionTemplatesImpl.InvalidServerUrn(urn.GetAttribute("Name")));
			}
			throw new SmoException(ExceptionTemplatesImpl.InvalidUrn(urn.Type));
		}
		RegSvrSmoObject smoObjectRec = GetSmoObjectRec(urn.Parent);
		string type = urn.Type;
		string nameForType = urn.GetNameForType(type);
		string text = type;
		object obj = smoObjectRec.GetType().InvokeMember(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, smoObjectRec, new object[0], SmoApplication.DefaultCulture);
		if (obj == null)
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.MissingObjectExceptionText(text, smoObjectRec.GetType().ToString(), null)).SetHelpContext("MissingObjectExceptionText");
		}
		RegSvrSmoObject regSvrSmoObject = (RegSvrSmoObject)obj.GetType().InvokeMember("GetObjectByName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, obj, new object[1] { nameForType }, SmoApplication.DefaultCulture);
		if (regSvrSmoObject == null)
		{
			throw new MissingObjectException(ExceptionTemplatesImpl.MissingObjectExceptionText(nameForType, smoObjectRec.GetType().ToString(), null)).SetHelpContext("MissingObjectExceptionText");
		}
		return regSvrSmoObject;
	}

	private void SetServerGroupObj(GroupRegistrationInfo groupRegInfo)
	{
		((RegistrationInfo)groupRegInfo).FriendlyName = base.Name;
		Property property = base.Properties.Get("Description");
		((RegistrationInfo)groupRegInfo).Description = (string)property.Value;
	}

	public void Create()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		CheckObjectState();
		try
		{
			ParentRegistrationInfo registrationInfoFromPath = ConnectionInfo.GetRegistrationInfoFromPath((Urn)base.ParentColl.ParentInstance.CollectionPath);
			GroupRegistrationInfo val = new GroupRegistrationInfo();
			SetServerGroupObj(val);
			ConnectionInfo.AddNode((RegistrationInfo)(object)val, registrationInfoFromPath);
			base.ParentColl.AddInternal(this);
			SetState(SqlSmoState.Existing);
		}
		catch (Exception ex)
		{
			RegSvrSmoObject.FilterException(ex);
			throw new SmoException(ExceptionTemplatesImpl.InnerRegSvrException, ex);
		}
	}

	public void Alter()
	{
		CheckObjectState();
		try
		{
			ParentRegistrationInfo registrationInfoFromPath = ConnectionInfo.GetRegistrationInfoFromPath((Urn)base.ParentColl.ParentInstance.CollectionPath);
			RegistrationInfo obj = registrationInfoFromPath.Children[base.Name];
			GroupRegistrationInfo val = (GroupRegistrationInfo)(object)((obj is GroupRegistrationInfo) ? obj : null);
			if (val != null)
			{
				SetServerGroupObj(val);
				ConnectionInfo.Save((RegistrationInfo)(object)val);
			}
		}
		catch (Exception ex)
		{
			RegSvrSmoObject.FilterException(ex);
			throw new SmoException(ExceptionTemplatesImpl.InnerRegSvrException, ex);
		}
	}

	public void Drop()
	{
		CheckObjectState();
		try
		{
			ParentRegistrationInfo registrationInfoFromPath = ConnectionInfo.GetRegistrationInfoFromPath((Urn)base.ParentColl.ParentInstance.CollectionPath);
			RegistrationInfo obj = registrationInfoFromPath.Children[base.Name];
			GroupRegistrationInfo val = (GroupRegistrationInfo)(object)((obj is GroupRegistrationInfo) ? obj : null);
			if (val != null)
			{
				ConnectionInfo.RemoveNode((RegistrationInfo)(object)val, registrationInfoFromPath);
			}
			base.ParentColl.RemoveInternal(base.Name);
			SetState(SqlSmoState.Dropped);
		}
		catch (Exception ex)
		{
			RegSvrSmoObject.FilterException(ex);
			throw new SmoException(ExceptionTemplatesImpl.InnerRegSvrException, ex);
		}
	}
}
