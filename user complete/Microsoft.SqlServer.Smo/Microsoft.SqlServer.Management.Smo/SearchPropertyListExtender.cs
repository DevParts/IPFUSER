using System;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class SearchPropertyListExtender : SmoObjectExtender<SearchPropertyList>, ISfcValidate
{
	private StringCollection databaseNamesToSelect;

	private StringCollection propertyListNamesToSelect;

	private string selectedDatabaseName = string.Empty;

	private string selectedPropertyListName = string.Empty;

	private DataTable searchProperties;

	private ValidationState gridValidationState;

	private SearchPropertyListValidator searchPropertyListValidator;

	private string sortingExpression = "Name ASC";

	private bool isEmptyList = true;

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
	public string SelectedDatabaseName
	{
		get
		{
			return selectedDatabaseName;
		}
		set
		{
			if (selectedDatabaseName != value)
			{
				selectedDatabaseName = value;
				InitPropertyListNamesToSelect();
			}
		}
	}

	[ExtendedProperty]
	public StringCollection DatabaseNamesToSelect
	{
		get
		{
			if (databaseNamesToSelect == null)
			{
				databaseNamesToSelect = new StringCollection();
				Database database = base.Parent.Parent;
				foreach (Database database2 in database.Parent.Databases)
				{
					databaseNamesToSelect.Add(database2.Name);
				}
			}
			return databaseNamesToSelect;
		}
	}

	[ExtendedProperty]
	public string SelectedPropertyListName
	{
		get
		{
			return selectedPropertyListName;
		}
		set
		{
			selectedPropertyListName = value;
		}
	}

	[ExtendedProperty]
	public StringCollection PropertyListNamesToSelect => propertyListNamesToSelect;

	[ExtendedProperty]
	public DataTable SearchProperties
	{
		get
		{
			if (searchProperties == null)
			{
				searchProperties = new DataTable();
				searchProperties.Locale = SmoApplication.DefaultCulture;
				searchProperties.Columns.Add("RowId", typeof(int)).AutoIncrement = true;
				searchProperties.Columns.Add("Name");
				searchProperties.Columns.Add("PropertySetGuid");
				searchProperties.Columns.Add("IntID", typeof(SearchPropertyIntIDType));
				searchProperties.Columns.Add("Description");
				searchProperties.PrimaryKey = new DataColumn[1] { searchProperties.Columns[0] };
				foreach (SearchProperty searchProperty in base.Parent.SearchProperties)
				{
					DataRow dataRow = searchProperties.NewRow();
					dataRow["Name"] = searchProperty.Name;
					dataRow["PropertySetGuid"] = searchProperty.PropertySetGuid;
					dataRow["IntID"] = new SearchPropertyIntIDType(searchProperty.IntID.ToString(SmoApplication.DefaultCulture));
					dataRow["Description"] = searchProperty.Description;
					searchProperties.Rows.Add(dataRow);
				}
				searchProperties.ColumnChanging += searchPropertyListValidator.SearchPropertiesColumnChangingValidationHandler;
				searchProperties.ColumnChanged += searchPropertyListValidator.SearchPropertiesColumnChangedValidationHandler;
				searchProperties.RowDeleting += searchPropertyListValidator.SearchPropertiesRowDeleteValidationHandler;
			}
			return searchProperties;
		}
		set
		{
			searchProperties = value;
		}
	}

	[ExtendedProperty]
	public DataTable SortedSearchProperties
	{
		get
		{
			SearchProperties.DefaultView.Sort = SortingExpression;
			DataTable dataTable = SearchProperties.DefaultView.ToTable();
			dataTable.PrimaryKey = new DataColumn[1] { dataTable.Columns[0] };
			dataTable.ColumnChanging += searchPropertyListValidator.SearchPropertiesColumnChangingValidationHandler;
			dataTable.ColumnChanged += searchPropertyListValidator.SearchPropertiesColumnChangedValidationHandler;
			dataTable.RowDeleting += searchPropertyListValidator.SearchPropertiesRowDeleteValidationHandler;
			foreach (DataRow row in dataTable.Rows)
			{
				int rowId = Convert.ToInt32(row["RowId"], SmoApplication.DefaultCulture);
				row.RowError = searchPropertyListValidator.GetValidationErrorsMessage(rowId);
			}
			SearchProperties.DefaultView.Sort = "";
			return dataTable;
		}
	}

	[ExtendedProperty]
	public string SortingExpression
	{
		internal get
		{
			return sortingExpression;
		}
		set
		{
			sortingExpression = value;
		}
	}

	[ExtendedProperty]
	public bool IsEmptyList
	{
		get
		{
			return isEmptyList;
		}
		set
		{
			isEmptyList = value;
		}
	}

	[ExtendedProperty]
	public string DatabaseName => base.Parent.Parent.Name;

	[ExtendedProperty]
	public ServerConnection ConnectionContext => base.Parent.Parent.GetServerObject().ConnectionContext;

	[ExtendedProperty]
	public ValidationState GridValidationState
	{
		get
		{
			if (gridValidationState == null)
			{
				gridValidationState = new ValidationState();
			}
			return gridValidationState;
		}
		set
		{
			gridValidationState = value;
		}
	}

	public SearchPropertyListExtender()
	{
		Initialize();
	}

	public SearchPropertyListExtender(SearchPropertyList SearchPropertyList)
		: base(SearchPropertyList)
	{
		Initialize();
	}

	private void Initialize()
	{
		searchPropertyListValidator = new SearchPropertyListValidator(this);
		searchPropertyListValidator.Initialize(base.Parent.SearchProperties);
		GridValidationState = new ValidationState();
		SelectedDatabaseName = base.Parent.Parent.Name;
	}

	private void InitPropertyListNamesToSelect()
	{
		propertyListNamesToSelect = new StringCollection();
		Database database = base.Parent.Parent.Parent.Databases[SelectedDatabaseName];
		SearchPropertyListCollection searchPropertyLists = database.SearchPropertyLists;
		foreach (SearchPropertyList item in searchPropertyLists)
		{
			propertyListNamesToSelect.Add(item.Name);
		}
		SelectedPropertyListName = string.Empty;
	}

	internal DataRow GetRow(int rowId)
	{
		return SearchProperties.Select("RowId = " + rowId)[0];
	}

	public void ApplyChanges()
	{
		base.Parent.Alter();
		foreach (string deletedSearchPropertyName in searchPropertyListValidator.DeletedSearchPropertyNames)
		{
			base.Parent.SearchProperties[deletedSearchPropertyName].Drop();
		}
		foreach (int updatedRow in searchPropertyListValidator.UpdatedRows)
		{
			DataRow row = GetRow(updatedRow);
			string name = row["Name"] as string;
			string propertySetGuid = row["PropertySetGuid"] as string;
			int intID = Convert.ToInt32(row["IntID"].ToString(), SmoApplication.DefaultCulture);
			string description = row["Description"] as string;
			base.Parent.SearchProperties[name].Drop();
			new SearchProperty(base.Parent, name, propertySetGuid, intID, description).Create();
		}
		foreach (int newRow in searchPropertyListValidator.NewRows)
		{
			DataRow row2 = GetRow(newRow);
			string name2 = row2["Name"] as string;
			string propertySetGuid2 = row2["PropertySetGuid"] as string;
			int intID2 = Convert.ToInt32(row2["IntID"].ToString(), SmoApplication.DefaultCulture);
			string text = row2["Description"] as string;
			if (text == null)
			{
				text = string.Empty;
			}
			new SearchProperty(base.Parent, name2, propertySetGuid2, intID2, text).Create();
		}
		base.Parent.SearchProperties.Refresh();
	}

	public ValidationState Validate(string methodName, params object[] arguments)
	{
		if (string.IsNullOrEmpty(base.Parent.Name) || base.Parent.Name.Length > 256)
		{
			return new ValidationState(ExceptionTemplatesImpl.SearchPropertyListNameNotValid(256), "Name");
		}
		if (string.IsNullOrEmpty(base.Parent.Name.Trim()))
		{
			return new ValidationState(ExceptionTemplatesImpl.SearchPropertyListNameAllWhiteSpaces, "Name");
		}
		if (!IsEmptyList && string.IsNullOrEmpty(SelectedPropertyListName))
		{
			return new ValidationState(ExceptionTemplatesImpl.EmptySourceSearchPropertyListName, "SelectedPropertyListName");
		}
		return GridValidationState;
	}
}
