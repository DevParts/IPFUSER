using System;
using System.Reflection;
using System.Security;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public sealed class RegisteredServer : RegSvrSmoObject, ICreatable, IAlterable, IDroppable
{
	internal class PropertyMetadataProvider : Microsoft.SqlServer.Management.Smo.PropertyMetadataProvider
	{
		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("AuthenticationType", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptedPassword", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Login", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ServerInstance", expensive: false, readOnly: false, typeof(string))
		};

		public override int Count => 5;

		public override int PropertyNameToIDLookup(string propertyName)
		{
			return propertyName switch
			{
				"AuthenticationType" => 0, 
				"Description" => 1, 
				"EncryptedPassword" => 2, 
				"Login" => 3, 
				"ServerInstance" => 4, 
				_ => -1, 
			};
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			return staticMetadata[id];
		}
	}

	private SecureString m_Password;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ServerGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ServerGroup;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Description
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Description");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Description", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Login
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Login");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Login", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ServerInstance
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ServerInstance");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ServerInstance", value);
		}
	}

	public override Urn Urn => new Urn(string.Format(SmoApplication.DefaultCulture, "{0}/{1}[@Path='{2}' and @Name='{3}']", ServerType.UrnSuffix, UrnSuffix, Urn.EscapeString(CollectionPath), Urn.EscapeString(base.Name)));

	internal static string UrnSuffix => "RegisteredServer";

	protected internal override Urn UrnSkeleton => new Urn(string.Format(SmoApplication.DefaultCulture, "{0}/{1}", new object[2]
	{
		ServerType.UrnSuffix,
		UrnSuffix
	}));

	internal int AuthenticationType
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("AuthenticationType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AuthenticationType", value);
		}
	}

	internal string EncryptedPassword => (string)base.Properties.GetValueWithNullReplacement("EncryptedPassword");

	public SecureString SecurePassword
	{
		get
		{
			if (m_Password == null)
			{
				m_Password = new SecureString();
			}
			return m_Password;
		}
		set
		{
			if (value != null)
			{
				m_Password = value.Copy();
			}
			else
			{
				m_Password = null;
			}
		}
	}

	public bool LoginSecure
	{
		get
		{
			return ServerType.WindowsAuthentication == AuthenticationType;
		}
		set
		{
			if (value)
			{
				AuthenticationType = ServerType.WindowsAuthentication;
			}
			else
			{
				AuthenticationType = ServerType.SqlAuthentication;
			}
		}
	}

	public RegisteredServer()
	{
	}

	public RegisteredServer(ServerGroup serverGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = serverGroup;
	}

	internal override Microsoft.SqlServer.Management.Smo.PropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider();
	}

	public RegisteredServer(string name)
		: base(SqlServerRegistrations.RegisteredServers, name)
	{
	}

	public RegisteredServer(RegisteredServerCollection regServers, string name)
		: base(regServers, name)
	{
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
			if (urn.Type == "RegisteredServer")
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

	private void SetRegServerObj(ServerInstanceRegistrationInfo regSvrItem)
	{
		UIConnectionInfo connectionInfo = regSvrItem.ConnectionInfo;
		((RegistrationInfo)regSvrItem).FriendlyName = base.Name;
		connectionInfo.ServerType = RegSvrConnectionInfo.SqlServerTypeGuid;
		Property property = base.Properties.Get("Description");
		if (property.Value != null)
		{
			((RegistrationInfo)regSvrItem).Description = (string)property.Value;
		}
		property = base.Properties.Get("AuthenticationType");
		if (property.Value != null)
		{
			connectionInfo.AuthenticationType = (int)property.Value;
		}
		else if (ServerType.WindowsAuthentication != connectionInfo.AuthenticationType && ServerType.SqlAuthentication != connectionInfo.AuthenticationType)
		{
			connectionInfo.AuthenticationType = ServerType.WindowsAuthentication;
		}
		property = base.Properties.Get("ServerInstance");
		if (property.Value != null)
		{
			connectionInfo.ServerName = (string)property.Value;
		}
		else if (connectionInfo.ServerName == null)
		{
			throw new PropertyNotSetException("ServerInstance");
		}
		property = base.Properties.Get("Login");
		if (property.Value != null)
		{
			connectionInfo.UserName = (string)property.Value;
		}
		SqlSecureString sqlSecureString = new SqlSecureString(SecurePassword);
		string text = (string)sqlSecureString;
		if (text.Length > 0)
		{
			connectionInfo.PersistPassword = true;
			connectionInfo.Password = text;
		}
	}

	public void Create()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		CheckObjectState();
		try
		{
			ParentRegistrationInfo registrationInfoFromPath = ConnectionInfo.GetRegistrationInfoFromPath((Urn)base.ParentColl.ParentInstance.CollectionPath);
			ServerInstanceRegistrationInfo val = new ServerInstanceRegistrationInfo();
			SetRegServerObj(val);
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
			ServerInstanceRegistrationInfo val = (ServerInstanceRegistrationInfo)(object)((obj is ServerInstanceRegistrationInfo) ? obj : null);
			if (val != null)
			{
				SetRegServerObj(val);
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
			ServerInstanceRegistrationInfo val = (ServerInstanceRegistrationInfo)(object)((obj is ServerInstanceRegistrationInfo) ? obj : null);
			if (val != null)
			{
				ConnectionInfo.RemoveNode((RegistrationInfo)(object)val, registrationInfoFromPath);
			}
			base.ParentColl.RemoveInternal(base.Name);
		}
		catch (Exception ex)
		{
			RegSvrSmoObject.FilterException(ex);
			throw new SmoException(ExceptionTemplatesImpl.InnerRegSvrException, ex);
		}
		SetState(SqlSmoState.Dropped);
	}
}
