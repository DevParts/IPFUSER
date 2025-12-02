using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class ScriptSchemaObjectBase : ScriptNameObjectBase
{
	private string m_sScriptSchema = string.Empty;

	internal virtual string ScriptSchema
	{
		get
		{
			CheckObjectState();
			return m_sScriptSchema;
		}
		set
		{
			CheckObjectState();
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("ScriptSchema"));
			}
			m_sScriptSchema = value;
		}
	}

	[CLSCompliant(false)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	[SfcKey(0)]
	public virtual string Schema
	{
		get
		{
			return ((SchemaObjectKey)key).Schema;
		}
		set
		{
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidSchema);
			}
			if (base.State == SqlSmoState.Pending)
			{
				((SchemaObjectKey)key).Schema = value;
				return;
			}
			if (base.State == SqlSmoState.Creating && base.ObjectInSpace)
			{
				((SchemaObjectKey)key).Schema = value;
				return;
			}
			throw new FailedOperationException(ExceptionTemplatesImpl.SetSchema, this, new InvalidSmoOperationException(ExceptionTemplatesImpl.SetSchema, base.State));
		}
	}

	[SfcKey(1)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	public override string Name
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

	internal override string FullQualifiedName => string.Format(SmoApplication.DefaultCulture, "[{0}].[{1}]", new object[2]
	{
		SqlSmoObject.SqlBraket(Schema),
		SqlSmoObject.SqlBraket(Name)
	});

	internal ScriptSchemaObjectBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState eState)
		: base(parentColl, key, eState)
	{
	}

	protected internal ScriptSchemaObjectBase()
	{
	}

	internal override string FormatFullNameForScripting(ScriptingPreferences sp)
	{
		CheckObjectState();
		string text = string.Empty;
		if (sp.IncludeScripts.SchemaQualify)
		{
			string schema = GetSchema(sp);
			if (schema.Length > 0)
			{
				text = SqlSmoObject.MakeSqlBraket(schema);
				text += Globals.Dot;
			}
		}
		return text + base.FormatFullNameForScripting(sp);
	}

	internal override void ScriptChangeOwner(StringCollection queries, ScriptingPreferences sp)
	{
		Property propertyOptional = GetPropertyOptional("Owner");
		if (propertyOptional.IsNull || (!propertyOptional.Dirty && sp.ScriptForAlter))
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
		{
			bool flag = true;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER AUTHORIZATION ON {0}", new object[1] { base.PermissionPrefix });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { FormatFullNameForScripting(sp) });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TO ");
			if (propertyOptional.Dirty)
			{
				flag = (string)propertyOptional.Value == string.Empty;
			}
			else if (base.ServerVersion.Major > 8)
			{
				Property property = base.Properties.Get("IsSchemaOwned");
				if (!property.IsNull)
				{
					flag = (bool)property.Value;
				}
			}
			else
			{
				flag = false;
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { flag ? " SCHEMA OWNER " : SqlSmoObject.MakeSqlBraket((string)propertyOptional.Value) });
		}
		else
		{
			ScriptOwnerForShiloh(stringBuilder, sp, (string)propertyOptional.Value);
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal string GetSchema(ScriptingPreferences sp)
	{
		if (!sp.ForDirectExecution && 0 < ScriptSchema.Length)
		{
			return ScriptSchema;
		}
		if (Schema != null)
		{
			return Schema;
		}
		return string.Empty;
	}

	internal void SetSchema(string newSchema)
	{
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	internal void ChangeSchema(string newSchema, bool bCheckExisting)
	{
		if (newSchema == null || newSchema.Length == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.InvalidSchema);
		}
		if (newSchema == ((SchemaObjectKey)key).Schema)
		{
			return;
		}
		SchemaCollectionBase schemaCollectionBase = base.ParentColl as SchemaCollectionBase;
		if (Schema != null && Schema.Length != 0 && schemaCollectionBase != null && schemaCollectionBase.Contains(Name, Schema))
		{
			if (bCheckExisting && SqlSmoState.Existing != base.State)
			{
				throw new SmoException(ExceptionTemplatesImpl.FailedToChangeSchema);
			}
			if (SqlSmoState.Existing == base.State && !base.IsDesignMode)
			{
				StringCollection queries = ScriptChangeSchema(Schema, newSchema);
				ExecutionManager.ExecuteNonQuery(queries);
			}
			if (!ExecutionManager.Recording)
			{
				schemaCollectionBase.RemoveObject(Name, Schema);
				((SchemaObjectKey)key).Schema = newSchema;
				schemaCollectionBase.AddExisting(this);
			}
		}
		else
		{
			((SchemaObjectKey)key).Schema = newSchema;
		}
	}

	private StringCollection ScriptChangeSchema(string oldSchema, string newSchema)
	{
		StringCollection stringCollection = new StringCollection();
		string text = string.Format(SmoApplication.DefaultCulture, "[{0}].[{1}]", new object[2]
		{
			SqlSmoObject.SqlBraket(oldSchema),
			SqlSmoObject.SqlBraket(Name)
		});
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		if (base.ServerVersion.Major < 9)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_changeobjectowner @objname=N'{0}', @newowner=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(text),
				SqlSmoObject.SqlString(newSchema)
			}));
		}
		else if (this is UserDefinedType)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER SCHEMA {0} TRANSFER TYPE :: {1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(newSchema),
				text
			}));
		}
		else
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER SCHEMA {0} TRANSFER {1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(newSchema),
				text
			}));
		}
		return stringCollection;
	}

	internal override ObjectKeyBase GetEmptyKey()
	{
		return new SchemaObjectKey(null, null);
	}
}
