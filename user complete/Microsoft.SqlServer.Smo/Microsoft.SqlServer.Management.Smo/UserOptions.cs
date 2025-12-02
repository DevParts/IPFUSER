using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("UserOption")]
public sealed class UserOptions : SqlSmoObject, ISfcSupportsDesignMode, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 15 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[15]
		{
			new StaticMetadata("AbortOnArithmeticErrors", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AbortTransactionOnError", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullDefaultOff", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullDefaultOn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNulls", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPadding", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarnings", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CursorCloseOnCommit", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DisableDefaultConstraintCheck", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IgnoreArithmeticErrors", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ImplicitTransactions", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NoCount", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbort", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifier", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[15]
		{
			new StaticMetadata("AbortOnArithmeticErrors", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AbortTransactionOnError", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullDefaultOff", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullDefaultOn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNulls", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPadding", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarnings", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CursorCloseOnCommit", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DisableDefaultConstraintCheck", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IgnoreArithmeticErrors", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ImplicitTransactions", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NoCount", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbort", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifier", expensive: false, readOnly: false, typeof(bool))
		};

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
				return propertyName switch
				{
					"AbortOnArithmeticErrors" => 0, 
					"AbortTransactionOnError" => 1, 
					"AnsiNullDefaultOff" => 2, 
					"AnsiNullDefaultOn" => 3, 
					"AnsiNulls" => 4, 
					"AnsiPadding" => 5, 
					"AnsiWarnings" => 6, 
					"ConcatenateNullYieldsNull" => 7, 
					"CursorCloseOnCommit" => 8, 
					"DisableDefaultConstraintCheck" => 9, 
					"IgnoreArithmeticErrors" => 10, 
					"ImplicitTransactions" => 11, 
					"NoCount" => 12, 
					"NumericRoundAbort" => 13, 
					"QuotedIdentifier" => 14, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AbortOnArithmeticErrors" => 0, 
				"AbortTransactionOnError" => 1, 
				"AnsiNullDefaultOff" => 2, 
				"AnsiNullDefaultOn" => 3, 
				"AnsiNulls" => 4, 
				"AnsiPadding" => 5, 
				"AnsiWarnings" => 6, 
				"ConcatenateNullYieldsNull" => 7, 
				"CursorCloseOnCommit" => 8, 
				"DisableDefaultConstraintCheck" => 9, 
				"IgnoreArithmeticErrors" => 10, 
				"ImplicitTransactions" => 11, 
				"NoCount" => 12, 
				"NumericRoundAbort" => 13, 
				"QuotedIdentifier" => 14, 
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
	}

	private bool overrideValueChecking;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool AbortOnArithmeticErrors
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AbortOnArithmeticErrors");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AbortOnArithmeticErrors", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool AbortTransactionOnError
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AbortTransactionOnError");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AbortTransactionOnError", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool AnsiNullDefaultOff
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullDefaultOff");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullDefaultOff", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool AnsiNullDefaultOn
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullDefaultOn");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullDefaultOn", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool AnsiNulls
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNulls");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNulls", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool AnsiPadding
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiPadding");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiPadding", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool AnsiWarnings
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiWarnings");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiWarnings", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool ConcatenateNullYieldsNull
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ConcatenateNullYieldsNull");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConcatenateNullYieldsNull", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool CursorCloseOnCommit
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("CursorCloseOnCommit");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CursorCloseOnCommit", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool DisableDefaultConstraintCheck
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DisableDefaultConstraintCheck");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DisableDefaultConstraintCheck", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool IgnoreArithmeticErrors
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IgnoreArithmeticErrors");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IgnoreArithmeticErrors", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool ImplicitTransactions
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ImplicitTransactions");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ImplicitTransactions", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool NoCount
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NoCount");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NoCount", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "false")]
	public bool NumericRoundAbort
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NumericRoundAbort");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericRoundAbort", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy, "true")]
	public bool QuotedIdentifier
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifier");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifier", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
		internal set
		{
			singletonParent = value;
			SetServerObject(((Server)singletonParent).GetServerObject());
		}
	}

	public static string UrnSuffix => "UserOption";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"AbortOnArithmeticErrors" => false, 
			"AbortTransactionOnError" => false, 
			"AnsiNullDefaultOff" => false, 
			"AnsiNullDefaultOn" => true, 
			"AnsiNulls" => true, 
			"AnsiPadding" => true, 
			"AnsiWarnings" => true, 
			"ConcatenateNullYieldsNull" => true, 
			"CursorCloseOnCommit" => false, 
			"DisableDefaultConstraintCheck" => false, 
			"IgnoreArithmeticErrors" => false, 
			"ImplicitTransactions" => true, 
			"NoCount" => false, 
			"NumericRoundAbort" => false, 
			"QuotedIdentifier" => true, 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	internal UserOptions()
	{
	}

	internal UserOptions(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	public void Alter()
	{
		overrideValueChecking = false;
		AlterImpl();
	}

	public void Alter(bool overrideValueChecking)
	{
		this.overrideValueChecking = overrideValueChecking;
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptProperties(query, sp);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		ScriptProperties(query, sp);
	}

	private void ScriptProperties(StringCollection query, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			throw new UnsupportedEngineTypeException(ExceptionTemplatesImpl.ScriptingNotSupportedOnCloud(typeof(UserOptions).Name)).SetHelpContext("ScriptingNotSupportedOnCloud");
		}
		Initialize(allProperties: true);
		new StringBuilder();
		InitializeKeepDirtyValues();
		object[][] array = new object[16][]
		{
			new object[2] { "DisableDefaultConstraintCheck", 1 },
			new object[2] { "ImplicitTransactions", 2 },
			new object[2] { "CursorCloseOnCommit", 4 },
			new object[2] { "AnsiWarnings", 8 },
			new object[2] { "AnsiPadding", 16 },
			new object[2] { "AnsiNulls", 32 },
			new object[2] { "AbortOnArithmeticErrors", 64 },
			new object[2] { "IgnoreArithmeticErrors", 128 },
			new object[2] { "QuotedIdentifier", 256 },
			new object[2] { "NoCount", 512 },
			new object[2] { "AnsiNullDefaultOn", 1024 },
			new object[2] { "AnsiNullDefaultOff", 2048 },
			new object[2] { "ConcatenateNullYieldsNull", 4096 },
			new object[2] { "NumericRoundAbort", 8192 },
			new object[2] { "AbortTransactionOnError", 16384 },
			new object[2] { "", 0 }
		};
		bool flag = false;
		int num = 0;
		for (int i = 0; ((string)array[i][0]).Length > 0; i++)
		{
			Property property = base.Properties.Get((string)array[i][0]);
			flag |= property.Dirty;
			num += (((bool)property.Value) ? ((int)array[i][1]) : 0);
		}
		string text = string.Empty;
		if (overrideValueChecking)
		{
			text = " WITH OVERRIDE";
		}
		if (flag || !sp.ScriptForAlter)
		{
			if (base.ServerVersion.Major > 8)
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SRV_SET_OPTIONS90, new object[1] { num }) + text);
			}
			else
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SRV_SET_OPTIONS80, new object[1] { num }) + text);
			}
		}
	}

	protected internal override string GetDBName()
	{
		return string.Empty;
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
		string[] fields = new string[1] { "AnsiNulls" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}
