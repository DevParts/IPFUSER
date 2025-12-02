using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class NamedSmoObject : SqlSmoObject
{
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	[SfcKey(0)]
	public virtual string Name
	{
		get
		{
			return ((SimpleObjectKey)key).Name;
		}
		set
		{
			try
			{
				ValidateName(value);
				if (ShouldNotifyPropertyChange)
				{
					if (Name != value)
					{
						((SimpleObjectKey)key).Name = value;
						OnPropertyChanged("Name");
					}
				}
				else
				{
					((SimpleObjectKey)key).Name = value;
				}
				UpdateObjectState();
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.SetName, this, ex);
			}
		}
	}

	internal override string FullQualifiedName => string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(Name) });

	internal override string InternalName => string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { Name });

	internal string PermissionPrefix
	{
		get
		{
			string text = null;
			switch (GetType().Name)
			{
			case "ExternalLibrary":
				text = "EXTERNAL LIBRARY";
				break;
			case "SqlAssembly":
				text = "ASSEMBLY";
				break;
			case "UserDefinedDataType":
				text = "TYPE";
				break;
			case "UserDefinedTableType":
				text = "TYPE";
				break;
			case "UserDefinedType":
				text = "TYPE";
				break;
			case "FullTextCatalog":
				text = "FULLTEXT CATALOG";
				break;
			case "Login":
				text = "LOGIN";
				break;
			case "ServerRole":
				text = "SERVER ROLE";
				break;
			case "Schema":
				text = "SCHEMA";
				break;
			case "HttpEndpoint":
				text = "ENDPOINT";
				break;
			case "XmlSchemaCollection":
				text = "XML SCHEMA COLLECTION";
				break;
			case "Certificate":
				text = "CERTIFICATE";
				break;
			case "ApplicationRole":
				text = "APPLICATION ROLE";
				break;
			case "User":
				text = "USER";
				break;
			case "DatabaseRole":
				text = "ROLE";
				break;
			case "SymmetricKey":
				text = "SYMMETRIC KEY";
				break;
			case "AsymmetricKey":
				text = "ASYMMETRIC KEY";
				break;
			case "Endpoint":
				text = "ENDPOINT";
				break;
			case "MessageType":
				text = "MESSAGE TYPE";
				break;
			case "ServiceContract":
				text = "CONTRACT";
				break;
			case "BrokerService":
				text = "SERVICE";
				break;
			case "ServiceRoute":
				text = "ROUTE";
				break;
			case "RemoteServiceBinding":
				text = "REMOTE SERVICE BINDING";
				break;
			case "FullTextStopList":
				text = "FULLTEXT STOPLIST";
				break;
			case "SearchPropertyList":
				text = "SEARCH PROPERTY LIST";
				break;
			case "Database":
				text = "DATABASE";
				break;
			case "AvailabilityGroup":
				text = AvailabilityGroup.AvailabilityGroupScript;
				break;
			}
			if (text != null)
			{
				return text + "::";
			}
			return string.Empty;
		}
	}

	internal NamedSmoObject(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal NamedSmoObject(ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
	}

	protected internal NamedSmoObject()
	{
	}

	internal virtual void ValidateName(string name)
	{
		if (name == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Name"));
		}
		if (base.State != SqlSmoState.Pending && base.State != SqlSmoState.Creating)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
		}
	}

	protected void RenameImpl(string newName)
	{
		try
		{
			CheckObjectState();
			string name = Name;
			string oldUrn = base.Urn;
			RenameImplWorker(newName);
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectRenamed())
			{
				SmoApplication.eventsSingleton.CallObjectRenamed(GetServerObject(), new ObjectRenamedEventArgs(base.Urn, this, name, newName, oldUrn));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Rename, this, ex);
		}
	}

	protected void RenameImplWorker(string newName)
	{
		if (newName == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("newName"));
		}
		ObjectKeyBase objectKeyBase = key;
		ExecuteRenameQuery(newName);
		if (!ExecutionManager.Recording)
		{
			((SimpleObjectKey)key).Name = newName;
			if (base.ParentColl != null)
			{
				base.ParentColl.RemoveObject(objectKeyBase);
				base.ParentColl.AddExisting(this);
			}
		}
	}

	protected virtual void ExecuteRenameQuery(string newName)
	{
		StringCollection stringCollection = new StringCollection();
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.SetTargetServerInfo(this);
		ScriptRename(stringCollection, scriptingPreferences, newName);
		if (stringCollection.Count > 0 && !base.IsDesignMode)
		{
			ExecuteNonQuery(stringCollection, !(this is Database), executeForAlter: false);
		}
	}

	internal virtual void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		throw new InvalidOperationException();
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new SimpleObjectKey(null);
	}

	internal virtual string FormatFullNameForScripting(ScriptingPreferences sp)
	{
		return FormatFullNameForScripting(sp, nameIsIndentifier: true);
	}

	internal string FormatFullNameForScripting(ScriptingPreferences sp, bool nameIsIndentifier)
	{
		CheckObjectState();
		if (nameIsIndentifier)
		{
			return SqlSmoObject.MakeSqlBraket(GetName(sp));
		}
		return SqlSmoObject.MakeSqlString(GetName(sp));
	}

	internal virtual string GetName(ScriptingPreferences sp)
	{
		return Name;
	}

	internal void ScriptOwner(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptChangeOwner(queries, sp);
	}

	protected void SetSchemaOwned()
	{
		if (!ExecutionManager.Recording && base.IsDesignMode && IsVersion90AndAbove())
		{
			string value = base.Properties.Get("Owner").Value as string;
			bool flag = false;
			if (string.IsNullOrEmpty(value))
			{
				flag = true;
			}
			int index = base.Properties.LookupID("IsSchemaOwned", PropertyAccessPurpose.Write);
			base.Properties.SetValue(index, flag);
			base.Properties.SetRetrieved(index, val: true);
		}
	}

	internal virtual void ScriptChangeOwner(StringCollection queries, ScriptingPreferences sp)
	{
		Property propertyOptional = GetPropertyOptional("Owner");
		if (!propertyOptional.IsNull && (propertyOptional.Dirty || !sp.ScriptForAlter))
		{
			ScriptChangeOwner(queries, (string)propertyOptional.Value, sp);
		}
	}

	internal virtual void ScriptChangeOwner(StringCollection queries, string newOwner, ScriptingPreferences sp = null)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if ((sp != null && sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80) || (sp == null && IsVersion90AndAbove()))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER AUTHORIZATION ON {0}", new object[1] { PermissionPrefix });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { FormatFullNameForScripting(sp) });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TO ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(newOwner) });
		}
		else
		{
			ScriptOwnerForShiloh(stringBuilder, sp, newOwner);
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal virtual void ScriptOwnerForShiloh(StringBuilder sb, ScriptingPreferences sp, string newOwner)
	{
		sb.AppendFormat(SmoApplication.DefaultCulture, "EXEC sp_changeobjectowner {0} , {1} ", new object[2]
		{
			SqlSmoObject.MakeSqlString(FormatFullNameForScripting(sp)),
			SqlSmoObject.MakeSqlString(newOwner)
		});
	}
}
