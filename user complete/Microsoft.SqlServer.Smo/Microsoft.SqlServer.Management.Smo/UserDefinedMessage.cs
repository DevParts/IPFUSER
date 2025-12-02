using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class UserDefinedMessage : MessageObjectBase, ICreatable, IDroppable, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 6, 6, 6, 6, 6, 6, 6, 6, 6, 6 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return -1;
				}
				return -1;
			}
			return propertyName switch
			{
				"ID" => 0, 
				"IsLogged" => 1, 
				"Language" => 2, 
				"LanguageID" => 3, 
				"Severity" => 4, 
				"Text" => 5, 
				_ => -1, 
			};
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[6]
			{
				new StaticMetadata("ID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("IsLogged", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("LanguageID", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("Severity", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("Text", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsLogged
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsLogged");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsLogged", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public int LanguageID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("LanguageID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LanguageID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int Severity
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("Severity");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Severity", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Text
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Text");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Text", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[SfcKey(1)]
	public int ID
	{
		get
		{
			return ((MessageObjectKey)key).ID;
		}
		set
		{
			((MessageObjectKey)key).ID = value;
			UpdateObjectState();
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[SfcKey(0)]
	public string Language
	{
		get
		{
			return ((MessageObjectKey)key).Language;
		}
		set
		{
			ValidateLanguage(value);
			((MessageObjectKey)key).Language = value;
			UpdateObjectState();
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "UserDefinedMessage";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "ID", "Language", "LanguageID" };
	}

	internal UserDefinedMessage(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public UserDefinedMessage()
	{
	}

	public UserDefinedMessage(Server server, int id)
	{
		key = new MessageObjectKey(id, MessageCollectionBase.GetDefaultLanguage());
		Parent = server;
	}

	public UserDefinedMessage(Server server, int id, string language)
	{
		key = new MessageObjectKey(id, language);
		Parent = server;
	}

	public UserDefinedMessage(Server server, int id, string language, int severity, string message)
	{
		key = new MessageObjectKey(id, language);
		Parent = server;
		Severity = severity;
		Text = message;
	}

	public UserDefinedMessage(Server server, int id, string language, int severity, string message, bool isLogged)
	{
		key = new MessageObjectKey(id, language);
		Parent = server;
		Severity = severity;
		Text = message;
		IsLogged = isLogged;
	}

	public UserDefinedMessage(Server server, int id, int language, int severity, string message)
	{
		Language language2 = server.Languages.ItemById(language);
		if (language2 == null)
		{
			throw new PropertyNotSetException("language");
		}
		key = new MessageObjectKey(id, language2.Name);
		Parent = server;
		Severity = severity;
		Text = message;
	}

	public UserDefinedMessage(Server server, int id, int language, int severity, string message, bool isLogged)
	{
		Language language2 = server.Languages.ItemById(language);
		if (language2 == null)
		{
			throw new PropertyNotSetException("language");
		}
		key = new MessageObjectKey(id, language2.Name);
		Parent = server;
		Severity = severity;
		Text = message;
		IsLogged = isLogged;
	}

	internal void ValidateState()
	{
		if (base.State != SqlSmoState.Pending && key != null && !key.Writable)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.OperationOnlyInPendingState);
		}
	}

	internal void ValidateLanguage(string language)
	{
		if (language == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("Language"));
		}
		ValidateState();
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("EXEC master.dbo.sp_addmessage ");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@msgnum={0}", new object[1] { ((MessageObjectKey)key).ID });
		string text = string.Empty;
		Property property = base.Properties.Get("LanguageID");
		if (property.Value != null)
		{
			LanguageCollection languages = ((Server)base.ParentColl.ParentInstance).Languages;
			foreach (Language item in languages)
			{
				if (item.LangID == (int)property.Value)
				{
					text = item.Name;
				}
			}
		}
		if (((MessageObjectKey)key).Language != null)
		{
			text = ((MessageObjectKey)key).Language;
		}
		if (text.Length == 0)
		{
			throw new PropertyNotSetException("Language");
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @lang=N'{0}'", new object[1] { SqlSmoObject.SqlString(text) });
		int count = 2;
		GetParameter(stringBuilder, sp, "Severity", "@severity={0}", ref count, throwIfNotSet: true);
		GetStringParameter(stringBuilder, sp, "Text", "@msgtext=N'{0}'", ref count, throwIfNotSet: true);
		GetBoolParameter(stringBuilder, sp, "IsLogged", "@with_log={0}", ref count, valueAsTrueFalse: true);
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.Append("EXEC master.dbo.sp_addmessage ");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " @msgnum={0}, @lang=N'{1}'", new object[2]
		{
			((MessageObjectKey)key).ID,
			((MessageObjectKey)key).Language
		});
		int count = 2;
		Property property = base.Properties["Severity"];
		if (property.Value != null)
		{
			if (count++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@severity={0}", new object[1] { property.Value.ToString() });
		}
		property = base.Properties["Text"];
		if (property.Value != null)
		{
			if (count++ > 0)
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@msgtext=N'{0}'", new object[1] { SqlSmoObject.SqlString(property.Value.ToString()) });
		}
		GetBoolParameter(stringBuilder, sp, "IsLogged", "@with_log={0}", ref count, valueAsTrueFalse: true);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @replace='REPLACE'");
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("Severity");
		stringCollection.Add("Text");
		stringCollection.Add("IsLogged");
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			alterQuery.Add(stringBuilder.ToString());
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = ((MessageObjectKey)key).Language;
		if (text == "us_english")
		{
			text = "all";
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_dropmessage @msgnum={0}, @lang=N'{1}'", new object[2]
		{
			((MessageObjectKey)key).ID,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	protected override void PostDrop()
	{
		string language = ((MessageObjectKey)key).Language;
		if (language == "us_english")
		{
			((SmoCollectionBase)base.ParentColl).Refresh();
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[2] { "ID", "Language" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
