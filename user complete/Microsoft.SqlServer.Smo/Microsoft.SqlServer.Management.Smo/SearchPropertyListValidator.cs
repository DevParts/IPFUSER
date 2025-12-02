using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SearchPropertyListValidator
{
	[Flags]
	private enum ValidationError
	{
		None = 0,
		NameError = 1,
		GuidError = 2,
		IntIdError = 4,
		DescriptionError = 8,
		GuidIntIdError = 0x10
	}

	private SearchPropertyListExtender searchPropertyListExtender;

	private SearchPropertyCollection existingSearchProperties;

	private SmoSet<string> deletedSearchPropertyNames = new SmoSet<string>();

	private SmoSet<int> updatedRows = new SmoSet<int>();

	private SmoSet<int> newRows = new SmoSet<int>();

	private Dictionary<int, ValidationError> validationErrors = new Dictionary<int, ValidationError>();

	private Dictionary<string, List<int>> matchingRowsForNames = new Dictionary<string, List<int>>();

	private Dictionary<string, List<int>> matchingRowsForGuidIntId = new Dictionary<string, List<int>>();

	private int nameErrors;

	private int guidErrors;

	private int intIdErrors;

	private int descriptionErrors;

	private int guidIntIdErrors;

	private int changingRowId = -1;

	public SmoSet<string> DeletedSearchPropertyNames => deletedSearchPropertyNames;

	public SmoSet<int> UpdatedRows => updatedRows;

	public SmoSet<int> NewRows => newRows;

	public SearchPropertyListValidator(SearchPropertyListExtender searchPropertyListExtender)
	{
		this.searchPropertyListExtender = searchPropertyListExtender;
	}

	public void Initialize(SearchPropertyCollection existingSearchProperties)
	{
		this.existingSearchProperties = existingSearchProperties;
		foreach (DataRow row in searchPropertyListExtender.SearchProperties.Rows)
		{
			string searchPropertyName = row["Name"] as string;
			AddMatchingRowForName(searchPropertyName, (int)row["RowId"]);
			string guidIntId = (row["PropertySetGuid"] as string) + row["IntID"].ToString();
			AddMatchingRowForGuidIntId(guidIntId, (int)row["RowId"]);
		}
	}

	private bool AddMatchingRowForName(string searchPropertyName, int rowId)
	{
		bool result = true;
		if (!matchingRowsForNames.ContainsKey(searchPropertyName))
		{
			matchingRowsForNames.Add(searchPropertyName, new List<int>());
		}
		List<int> list = matchingRowsForNames[searchPropertyName];
		list.Add(rowId);
		if (list.Count > 1)
		{
			result = false;
		}
		return result;
	}

	private void RemoveMatchingRowForName(string searchPropertyName, int rowId)
	{
		List<int> list = matchingRowsForNames[searchPropertyName];
		list.Remove(rowId);
		if (list.Count == 1)
		{
			int num = list[0];
			AddOrRemoveValidationError(num, ValidationError.NameError, isValid: true, ref nameErrors);
			searchPropertyListExtender.GetRow(num).RowError = GetValidationErrorsMessage(num);
		}
		else if (list.Count == 0)
		{
			matchingRowsForNames.Remove(searchPropertyName);
			if (existingSearchProperties.Contains(searchPropertyName))
			{
				deletedSearchPropertyNames.Add(searchPropertyName);
				updatedRows.Remove(rowId);
			}
			else
			{
				newRows.Remove(rowId);
			}
		}
	}

	private bool AddMatchingRowForGuidIntId(string guidIntId, int rowId)
	{
		bool result = true;
		if (!matchingRowsForGuidIntId.ContainsKey(guidIntId))
		{
			matchingRowsForGuidIntId.Add(guidIntId, new List<int>());
		}
		List<int> list = matchingRowsForGuidIntId[guidIntId];
		list.Add(rowId);
		if (list.Count > 1)
		{
			result = false;
		}
		return result;
	}

	private void RemoveMatchingRowForGuidIntId(string guidIntId, int rowId)
	{
		List<int> list = matchingRowsForGuidIntId[guidIntId];
		list.Remove(rowId);
		if (list.Count == 1)
		{
			int num = list[0];
			AddOrRemoveValidationError(num, ValidationError.GuidIntIdError, isValid: true, ref guidIntIdErrors);
			searchPropertyListExtender.GetRow(num).RowError = GetValidationErrorsMessage(num);
		}
		else if (list.Count == 0)
		{
			matchingRowsForNames.Remove(guidIntId);
			if (existingSearchProperties.Contains(guidIntId))
			{
				deletedSearchPropertyNames.Add(guidIntId);
				updatedRows.Remove(rowId);
			}
			else
			{
				newRows.Remove(rowId);
			}
		}
	}

	public string GetValidationErrorsMessage(int rowId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (validationErrors.ContainsKey(rowId))
		{
			_ = validationErrors[rowId];
			if ((validationErrors[rowId] & ValidationError.NameError) == ValidationError.NameError)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyNameNotValid(256));
				stringBuilder.AppendLine();
			}
			if ((validationErrors[rowId] & ValidationError.GuidError) == ValidationError.GuidError)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertySetGuidNotValid);
				stringBuilder.AppendLine();
			}
			if ((validationErrors[rowId] & ValidationError.IntIdError) == ValidationError.IntIdError)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyIntIDNotValid);
				stringBuilder.AppendLine();
			}
			if ((validationErrors[rowId] & ValidationError.DescriptionError) == ValidationError.DescriptionError)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyDescriptionNotValid(512));
				stringBuilder.AppendLine();
			}
			if ((validationErrors[rowId] & ValidationError.GuidIntIdError) == ValidationError.GuidIntIdError)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyGuidIntIdNotValid);
				stringBuilder.AppendLine();
			}
		}
		return stringBuilder.ToString().Trim();
	}

	public void SearchPropertiesColumnChangingValidationHandler(object sender, DataColumnChangeEventArgs e)
	{
		DataRow row = e.Row;
		InitializeRow(row);
		int rowId = (int)row["RowId"];
		string columnName = e.Column.ColumnName;
		string text = string.Empty;
		if (e.ProposedValue != null)
		{
			text = e.ProposedValue.ToString();
		}
		if ((IsEquals(columnName, "PropertySetGuid") || IsEquals(columnName, "IntID")) && IsValidGuidAndIntIds(rowId))
		{
			string guidIntId = (row["PropertySetGuid"] as string) + GetNormalizedIntId(row["IntID"].ToString());
			RemoveMatchingRowForGuidIntId(guidIntId, rowId);
		}
		switch (columnName)
		{
		case "Name":
		{
			string searchPropertyName = row["Name"] as string;
			if (IsValidNameLength(searchPropertyName))
			{
				RemoveMatchingRowForName(searchPropertyName, rowId);
			}
			ValidateName(rowId, text);
			break;
		}
		case "PropertySetGuid":
			ValidateGuid(rowId, text);
			text += GetNormalizedIntId(row["IntID"].ToString());
			ValidateGuidIntId(rowId, text);
			break;
		case "IntID":
			ValidateIntID(rowId, text);
			text = (row["PropertySetGuid"] as string) + GetNormalizedIntId(text);
			ValidateGuidIntId(rowId, text);
			break;
		case "Description":
			ValidateDescription(rowId, text);
			break;
		}
	}

	public void SearchPropertiesColumnChangedValidationHandler(object sender, DataColumnChangeEventArgs e)
	{
		DataRow row = e.Row;
		int num = (int)row["RowId"];
		string validationErrorsMessage = GetValidationErrorsMessage(num);
		if (string.IsNullOrEmpty(validationErrorsMessage))
		{
			row.ClearErrors();
			string name = row["Name"] as string;
			if (existingSearchProperties.Contains(name))
			{
				if (deletedSearchPropertyNames.Contains(name))
				{
					deletedSearchPropertyNames.Remove(name);
				}
				SearchProperty searchProperty = existingSearchProperties[name];
				if (!IsEquals(row["PropertySetGuid"] as string, searchProperty.PropertySetGuid.ToString()) || !IsEquals(row["IntID"].ToString(), searchProperty.IntID.ToString(SmoApplication.DefaultCulture)) || !IsEquals(row["Description"] as string, searchProperty.Description))
				{
					updatedRows.Add(num);
				}
				else
				{
					updatedRows.Remove(num);
				}
			}
			else
			{
				newRows.Add(num);
			}
		}
		else
		{
			row.RowError = validationErrorsMessage;
		}
		SetGridValidationState();
	}

	private void InitializeRow(DataRow row)
	{
		int num = (int)row["RowId"];
		if (num != changingRowId && row.RowState == DataRowState.Detached)
		{
			changingRowId = num;
			if (string.IsNullOrEmpty(row["Name"] as string))
			{
				AddOrRemoveValidationError(num, ValidationError.NameError, isValid: false, ref nameErrors);
			}
			if (string.IsNullOrEmpty(row["PropertySetGuid"] as string))
			{
				AddOrRemoveValidationError(num, ValidationError.GuidError, isValid: false, ref guidErrors);
				AddOrRemoveValidationError(num, ValidationError.GuidIntIdError, isValid: false, ref guidIntIdErrors);
			}
			if (string.IsNullOrEmpty(row["IntID"].ToString()))
			{
				AddOrRemoveValidationError(num, ValidationError.IntIdError, isValid: false, ref intIdErrors);
				AddOrRemoveValidationError(num, ValidationError.GuidIntIdError, isValid: false, ref guidIntIdErrors);
			}
		}
	}

	public void SearchPropertiesRowDeleteValidationHandler(object sender, DataRowChangeEventArgs e)
	{
		DataRow row = e.Row;
		int rowId = (int)row["RowId"];
		string searchPropertyName = row["Name"] as string;
		if (IsValidNameLength(searchPropertyName))
		{
			RemoveMatchingRowForName(searchPropertyName, rowId);
		}
		if (IsValidGuidAndIntIds(rowId))
		{
			string guidIntId = (row["PropertySetGuid"] as string) + GetNormalizedIntId(row["IntID"].ToString());
			RemoveMatchingRowForGuidIntId(guidIntId, rowId);
		}
		ResetValiationErrors(rowId);
		SetGridValidationState();
	}

	private void SetGridValidationState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (nameErrors != 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyNameNotValid(256));
		}
		else if (guidErrors != 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertySetGuidNotValid);
		}
		else if (intIdErrors != 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyIntIDNotValid);
		}
		else if (descriptionErrors != 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyDescriptionNotValid(512));
		}
		else if (guidIntIdErrors != 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.SearchPropertyGuidIntIdNotValid);
		}
		string text = stringBuilder.ToString().Trim();
		if (string.IsNullOrEmpty(text))
		{
			searchPropertyListExtender.GridValidationState = new ValidationState();
		}
		else
		{
			searchPropertyListExtender.GridValidationState = new ValidationState(text, "SearchProperties");
		}
	}

	private bool AddValidationError(int rowID, ValidationError error)
	{
		bool result = false;
		if (!validationErrors.ContainsKey(rowID))
		{
			validationErrors.Add(rowID, error);
			result = true;
		}
		else if ((validationErrors[rowID] & error) != error)
		{
			validationErrors[rowID] |= error;
			result = true;
		}
		return result;
	}

	private bool RemoveValidationError(int rowID, ValidationError error)
	{
		bool result = false;
		if (validationErrors.ContainsKey(rowID) && (validationErrors[rowID] & error) == error)
		{
			validationErrors[rowID] &= ~error;
			result = true;
		}
		return result;
	}

	private void AddOrRemoveValidationError(int rowID, ValidationError error, bool isValid, ref int errorCount)
	{
		if (!isValid)
		{
			if (AddValidationError(rowID, error))
			{
				errorCount++;
			}
		}
		else if (RemoveValidationError(rowID, error))
		{
			errorCount--;
		}
	}

	private void ValidateName(int rowId, string proposedValue)
	{
		bool flag = true;
		flag = IsValidNameLength(proposedValue) && AddMatchingRowForName(proposedValue, rowId);
		AddOrRemoveValidationError(rowId, ValidationError.NameError, flag, ref nameErrors);
	}

	private string GetNormalizedIntId(string intId)
	{
		try
		{
			intId = Convert.ToInt32(intId, SmoApplication.DefaultCulture).ToString(SmoApplication.DefaultCulture);
		}
		catch (FormatException)
		{
		}
		catch (OverflowException)
		{
		}
		return intId;
	}

	private void ValidateGuidIntId(int rowId, string proposedValue)
	{
		bool flag = true;
		flag = IsValidGuidAndIntIds(rowId) && AddMatchingRowForGuidIntId(proposedValue, rowId);
		AddOrRemoveValidationError(rowId, ValidationError.GuidIntIdError, flag, ref guidIntIdErrors);
	}

	private void ValidateGuid(int rowId, string proposedValue)
	{
		bool isValid = true;
		if (string.IsNullOrEmpty(proposedValue))
		{
			isValid = false;
		}
		else
		{
			try
			{
				new Guid(proposedValue);
			}
			catch (FormatException)
			{
				isValid = false;
			}
		}
		AddOrRemoveValidationError(rowId, ValidationError.GuidError, isValid, ref guidErrors);
	}

	private void ValidateIntID(int rowId, string proposedValue)
	{
		bool isValid = true;
		int num = 0;
		if (string.IsNullOrEmpty(proposedValue))
		{
			isValid = false;
		}
		else
		{
			try
			{
				num = Convert.ToInt32(proposedValue, SmoApplication.DefaultCulture);
			}
			catch (FormatException)
			{
				isValid = false;
			}
			catch (OverflowException)
			{
				isValid = false;
			}
		}
		if (num < 0)
		{
			isValid = false;
		}
		AddOrRemoveValidationError(rowId, ValidationError.IntIdError, isValid, ref intIdErrors);
	}

	private void ValidateDescription(int rowId, string proposedValue)
	{
		bool isValid = true;
		if (!string.IsNullOrEmpty(proposedValue) && proposedValue.Length > 512)
		{
			isValid = false;
		}
		AddOrRemoveValidationError(rowId, ValidationError.DescriptionError, isValid, ref descriptionErrors);
	}

	private bool IsValidGuidAndIntIds(int rowId)
	{
		bool flag = true;
		bool flag2 = true;
		if (validationErrors.ContainsKey(rowId))
		{
			flag = (validationErrors[rowId] & ValidationError.GuidError) != ValidationError.GuidError;
			flag2 = (validationErrors[rowId] & ValidationError.IntIdError) != ValidationError.IntIdError;
		}
		return flag && flag2;
	}

	private bool IsValidNameLength(string searchPropertyName)
	{
		if (!string.IsNullOrEmpty(searchPropertyName))
		{
			return searchPropertyName.Length <= 256;
		}
		return false;
	}

	private void ResetValiationErrors(int rowId)
	{
		AddOrRemoveValidationError(rowId, ValidationError.NameError, isValid: true, ref nameErrors);
		AddOrRemoveValidationError(rowId, ValidationError.GuidError, isValid: true, ref guidErrors);
		AddOrRemoveValidationError(rowId, ValidationError.IntIdError, isValid: true, ref intIdErrors);
		AddOrRemoveValidationError(rowId, ValidationError.DescriptionError, isValid: true, ref descriptionErrors);
		AddOrRemoveValidationError(rowId, ValidationError.GuidIntIdError, isValid: true, ref guidIntIdErrors);
	}

	private bool IsEquals(string s1, string s2)
	{
		return NetCoreHelpers.StringCompare(s1, s2, ignoreCase: false, SmoApplication.DefaultCulture) == 0;
	}
}
