using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class ParameterBase : ScriptNameObjectBase, IExtendedProperties, IMarkForDrop
{
	private const char longUderscoreChar = '\uff3f';

	private bool bUseOutput;

	private bool bUseDefault;

	private bool bIsReadOnly;

	private DataType dataType;

	public static string UrnSuffix => "Param";

	internal bool UseOutput
	{
		get
		{
			return bUseOutput;
		}
		set
		{
			bUseOutput = value;
		}
	}

	internal bool UseDefault
	{
		get
		{
			return bUseDefault;
		}
		set
		{
			bUseDefault = value;
		}
	}

	internal bool UseIsReadOnly
	{
		get
		{
			return bIsReadOnly;
		}
		set
		{
			bIsReadOnly = value;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(0)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	[SfcReference(typeof(UserDefinedType), typeof(UserDefinedTypeResolver), "Resolve", new string[] { })]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	[SfcReference(typeof(UserDefinedDataType), typeof(UserDefinedDataTypeResolver), "Resolve", new string[] { })]
	[SfcReference(typeof(UserDefinedTableType), typeof(UserDefinedTableTypeResolver), "Resolve", new string[] { })]
	public virtual DataType DataType
	{
		get
		{
			return GetDataType(ref dataType);
		}
		set
		{
			SetDataType(ref dataType, value);
		}
	}

	internal override string ScriptName
	{
		get
		{
			return base.ScriptName;
		}
		set
		{
			((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).CheckTextModeAccess("ScriptName");
			base.ScriptName = value;
		}
	}

	internal ParameterBase(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		Init();
	}

	internal ParameterBase()
	{
		Init();
	}

	private void Init()
	{
		bUseOutput = true;
		bUseDefault = true;
		bIsReadOnly = true;
	}

	internal override void ValidateName(string name)
	{
		base.ValidateName(name);
		CheckParamName(name);
	}

	protected void CheckParamName(string paramName)
	{
		bool flag = true;
		char[] array = paramName.ToCharArray();
		foreach (char c in array)
		{
			if (flag)
			{
				if (c != '@')
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.WrongPropertyValueException("Name", paramName));
				}
				flag = false;
			}
			else if (!char.IsLetterOrDigit(c) && c != '_' && c != '@' && c != '#' && c != '$' && c != '\uff3f')
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.WrongPropertyValueException("Name", paramName));
			}
		}
	}

	internal override void ScriptDdl(StringCollection queries, ScriptingPreferences sp)
	{
		CheckObjectState();
		InitializeKeepDirtyValues();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = ((ScriptName != null && ScriptName.Length > 0) ? ScriptName : Name);
		CheckParamName(text);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { text });
		UserDefinedDataType.AppendScriptTypeDefinition(stringBuilder, sp, this, DataType.SqlDataType);
		if (bUseDefault)
		{
			string text2 = (string)GetPropValueOptional("DefaultValue");
			if (text2 != null)
			{
				if (base.Properties["DefaultValue"].Dirty || (isParentClrImplemented() && GetPropValueOptional("HasDefaultValue", defaultValue: false)))
				{
					if (text2.Equals("null", StringComparison.OrdinalIgnoreCase))
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " = {0}", new object[1] { text2 });
					}
					else
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " = {0}", new object[1] { MakeSqlStringIfRequired(text2) });
					}
				}
				else if (text2.Length > 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " = {0}", new object[1] { text2 });
				}
			}
		}
		if (bUseOutput && base.Properties.Get("IsOutputParameter").Value != null && (bool)base.Properties.Get("IsOutputParameter").Value)
		{
			stringBuilder.Append(" OUTPUT");
		}
		if (bIsReadOnly && !(this is NumberedStoredProcedureParameter) && base.ServerVersion.Major >= 10 && base.Properties.Get("IsReadOnly").Value != null && (bool)base.Properties.Get("IsReadOnly").Value)
		{
			stringBuilder.Append(" READONLY");
		}
		queries.Add(stringBuilder.ToString());
	}

	private string MakeSqlStringIfRequired(string defaultValue)
	{
		switch (DataType.SqlDataType)
		{
		case SqlDataType.DateTime:
		case SqlDataType.NChar:
		case SqlDataType.NText:
		case SqlDataType.NVarChar:
		case SqlDataType.NVarCharMax:
		case SqlDataType.SmallDateTime:
		case SqlDataType.Text:
		case SqlDataType.VarChar:
		case SqlDataType.VarCharMax:
		case SqlDataType.SysName:
		case SqlDataType.Date:
		case SqlDataType.Time:
		case SqlDataType.DateTimeOffset:
		case SqlDataType.DateTime2:
			return SqlSmoObject.MakeSqlString(defaultValue);
		default:
			return defaultValue;
		}
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (this is NumberedStoredProcedureParameter)
		{
			return base.GetPropagateInfo(action);
		}
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8 || DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public override void Refresh()
	{
		base.Refresh();
		dataType = null;
	}

	protected virtual bool isParentClrImplemented()
	{
		return false;
	}
}
