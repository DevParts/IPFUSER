using System;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class SequenceExtender : SmoObjectExtender<Sequence>, ISfcValidate
{
	private StringCollection datatypeNames;

	private string selectedDatatypeName = string.Empty;

	private int defaultPrecision = 18;

	private string selectedNumericPrecision = string.Empty;

	private bool hasMinimumValue;

	private bool hasMaximumValue;

	private string sequenceDatatypeName = string.Empty;

	private string sequenceNumericPrecision = string.Empty;

	private bool hasRestartValue;

	private object originalStartValue = string.Empty;

	private object permissionPageDataContainer;

	private object extendedPropertyPageDataContainer;

	private bool extendedPropertyPageIsDirty;

	[ExtendedProperty]
	public SqlSmoState State => base.Parent.State;

	[ExtendedProperty]
	public SqlSmoObject CurrentObject => base.Parent;

	[ExtendedProperty]
	public string Name
	{
		get
		{
			return base.Parent.Name;
		}
		set
		{
			base.Parent.Name = value;
		}
	}

	[ExtendedProperty]
	public string Schema
	{
		get
		{
			return base.Parent.Schema;
		}
		set
		{
			base.Parent.Schema = value;
		}
	}

	[ExtendedProperty]
	public string DatabaseName => base.Parent.Parent.Name;

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.GetServerObject().ConnectionContext;

	[ExtendedProperty]
	public StringCollection DatatypeNames
	{
		get
		{
			if (datatypeNames == null)
			{
				datatypeNames = new StringCollection();
				Database database = base.Parent.Parent;
				if (database != null)
				{
					datatypeNames.Add("tinyint");
					datatypeNames.Add("smallint");
					datatypeNames.Add("int");
					datatypeNames.Add("bigint");
					datatypeNames.Add("decimal");
					datatypeNames.Add("numeric");
					Server serverObject = database.GetServerObject();
					serverObject.SetDefaultInitFields(typeof(UserDefinedDataType), "NumericScale");
					serverObject.SetDefaultInitFields(typeof(UserDefinedDataType), "SystemType");
					foreach (UserDefinedDataType userDefinedDataType in database.UserDefinedDataTypes)
					{
						string systemType = userDefinedDataType.SystemType;
						string schema = userDefinedDataType.Schema;
						string name = userDefinedDataType.Name;
						switch (systemType.ToLower(SmoApplication.DefaultCulture))
						{
						case "tinyint":
						case "smallint":
						case "int":
						case "bigint":
							datatypeNames.Add(UddtFullName(schema, name));
							break;
						case "decimal":
						case "numeric":
							if (userDefinedDataType.NumericScale == 0)
							{
								datatypeNames.Add(UddtFullName(schema, name));
							}
							break;
						}
					}
				}
			}
			return datatypeNames;
		}
	}

	[ExtendedProperty]
	public string SelectedDatatypeName
	{
		get
		{
			if (string.IsNullOrEmpty(selectedDatatypeName))
			{
				return "bigint";
			}
			return selectedDatatypeName;
		}
		set
		{
			selectedDatatypeName = value;
			switch (selectedDatatypeName.ToLower(SmoApplication.DefaultCulture))
			{
			case "tinyint":
			case "smallint":
			case "int":
			case "bigint":
			{
				SqlDataType sqlDataType = DataType.SqlToEnum(selectedDatatypeName);
				base.Parent.DataType = new DataType(sqlDataType);
				return;
			}
			case "decimal":
			case "numeric":
			{
				SqlDataType sqlDataType2 = DataType.SqlToEnum(selectedDatatypeName);
				base.Parent.DataType = new DataType(sqlDataType2);
				return;
			}
			}
			UserDefinedDataType uDDT = GetUDDT(SelectedDatatypeName);
			if (uDDT != null)
			{
				base.Parent.DataType = new DataType(uDDT);
			}
		}
	}

	[ExtendedProperty]
	public int DefaultPrecision
	{
		get
		{
			switch (SelectedDatatypeName.ToLower(SmoApplication.DefaultCulture))
			{
			case "tinyint":
				defaultPrecision = 3;
				break;
			case "smallint":
				defaultPrecision = 5;
				break;
			case "int":
				defaultPrecision = 10;
				break;
			case "bigint":
				defaultPrecision = 19;
				break;
			case "decimal":
			case "numeric":
				defaultPrecision = 18;
				break;
			default:
			{
				UserDefinedDataType uDDT = GetUDDT(SelectedDatatypeName);
				if (uDDT != null)
				{
					defaultPrecision = uDDT.NumericPrecision;
				}
				break;
			}
			}
			return defaultPrecision;
		}
	}

	[ExtendedProperty]
	public string SelectedNumericPrecision
	{
		get
		{
			return selectedNumericPrecision;
		}
		set
		{
			selectedNumericPrecision = value;
			base.Parent.DataType.NumericPrecision = Convert.ToInt32(selectedNumericPrecision, SmoApplication.DefaultCulture);
		}
	}

	[ExtendedProperty]
	public bool HasMinimumValue
	{
		get
		{
			if (base.Parent.State == SqlSmoState.Existing && !string.IsNullOrEmpty(base.Parent.MinValue.ToString()))
			{
				hasMinimumValue = true;
			}
			return hasMinimumValue;
		}
		set
		{
			hasMinimumValue = value;
			if (!hasMinimumValue)
			{
				base.Parent.MinValue = string.Empty;
			}
		}
	}

	[ExtendedProperty]
	public bool HasMaximumValue
	{
		get
		{
			if (base.Parent.State == SqlSmoState.Existing && !string.IsNullOrEmpty(base.Parent.MaxValue.ToString()))
			{
				hasMaximumValue = true;
			}
			return hasMaximumValue;
		}
		set
		{
			hasMaximumValue = value;
			if (!hasMaximumValue)
			{
				base.Parent.MaxValue = string.Empty;
			}
		}
	}

	[ExtendedProperty]
	public string SequenceDatatypeName
	{
		get
		{
			if (base.Parent.State == SqlSmoState.Existing)
			{
				sequenceDatatypeName = UddtFullName(base.Parent.DataType.Schema, base.Parent.DataType.Name);
			}
			return sequenceDatatypeName;
		}
	}

	[ExtendedProperty]
	public string SequenceNumericPrecision
	{
		get
		{
			if (base.Parent.State == SqlSmoState.Existing)
			{
				sequenceNumericPrecision = Convert.ToString(base.Parent.DataType.NumericPrecision, SmoApplication.DefaultCulture);
			}
			return sequenceNumericPrecision;
		}
	}

	[ExtendedProperty]
	public bool HasRestartValue
	{
		get
		{
			return hasRestartValue;
		}
		set
		{
			hasRestartValue = value;
		}
	}

	[ExtendedProperty]
	public object OriginalStartValue
	{
		get
		{
			return originalStartValue;
		}
		set
		{
			originalStartValue = value;
		}
	}

	[ExtendedProperty]
	public object PermissionPageOnRunNow
	{
		get
		{
			return this.permissionPageOnRunNow;
		}
		set
		{
			this.permissionPageOnRunNow = (EventHandler)value;
		}
	}

	[ExtendedProperty]
	public object PermissionPageDataContainer
	{
		get
		{
			return permissionPageDataContainer;
		}
		set
		{
			permissionPageDataContainer = value;
		}
	}

	[ExtendedProperty]
	public object ExtendedPropertyPageOnRunNow
	{
		get
		{
			return this.extendedPropertyPageOnRunNow;
		}
		set
		{
			this.extendedPropertyPageOnRunNow = (EventHandler)value;
		}
	}

	[ExtendedProperty]
	public object ExtendedPropertyPageCommitCellEdits
	{
		get
		{
			return this.extendedPropertyPageCommitCellEdits;
		}
		set
		{
			this.extendedPropertyPageCommitCellEdits = (EventHandler)value;
		}
	}

	[ExtendedProperty]
	public object ExtendedPropertyPageDataContainer
	{
		get
		{
			return extendedPropertyPageDataContainer;
		}
		set
		{
			extendedPropertyPageDataContainer = value;
		}
	}

	[ExtendedProperty]
	public bool ExtendedPropertyPageIsDirty
	{
		get
		{
			return extendedPropertyPageIsDirty;
		}
		set
		{
			extendedPropertyPageIsDirty = value;
		}
	}

	private event EventHandler permissionPageOnRunNow;

	private event EventHandler extendedPropertyPageOnRunNow;

	private event EventHandler extendedPropertyPageCommitCellEdits;

	public SequenceExtender()
	{
	}

	public SequenceExtender(Sequence sequence)
		: base(sequence)
	{
	}

	private UserDefinedDataType GetUDDT(string datatypename)
	{
		Database database = base.Parent.Parent;
		foreach (UserDefinedDataType userDefinedDataType in database.UserDefinedDataTypes)
		{
			if (string.Compare(UddtFullName(userDefinedDataType.Schema, userDefinedDataType.Name), selectedDatatypeName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return userDefinedDataType;
			}
		}
		return null;
	}

	private static string UddtFullName(string schema, string name)
	{
		string empty = string.Empty;
		if (!string.IsNullOrEmpty(schema))
		{
			return string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2] { schema, name });
		}
		return string.Format(SmoApplication.DefaultCulture, "{0}", new object[1] { name });
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (methodName.Equals("Create") && string.IsNullOrEmpty(base.Parent.Name))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterSequenceName, "Name");
		}
		if (HasMinimumValue && string.IsNullOrEmpty(base.Parent.MinValue.ToString()))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterMinValue, "MinValue");
		}
		if (HasMaximumValue && string.IsNullOrEmpty(base.Parent.MaxValue.ToString()))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterMaxValue, "MaxValue");
		}
		if (methodName.Equals("Alter") && string.IsNullOrEmpty(base.Parent.IncrementValue.ToString()))
		{
			return new ValidationState(ExceptionTemplatesImpl.EnterIncrementValue, "IncrementValue");
		}
		return base.Parent.Validate(methodName, arguments);
	}
}
