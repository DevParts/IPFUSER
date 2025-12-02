using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class ValidationState
{
	private List<ValidationResult> results = new List<ValidationResult>();

	public IList<ValidationResult> Results => results;

	public bool HasErrors
	{
		get
		{
			foreach (ValidationResult result in results)
			{
				if (!result.IsWarning)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool HasWarnings
	{
		get
		{
			foreach (ValidationResult result in results)
			{
				if (result.IsWarning)
				{
					return true;
				}
			}
			return false;
		}
	}

	public ValidationState()
	{
	}

	public ValidationState(string message, string bindingKey, bool isWarning)
	{
		if (isWarning)
		{
			AddWarning(message, bindingKey);
		}
		else
		{
			AddError(message, bindingKey);
		}
	}

	public ValidationState(Exception error, string bindingKey, bool isWarning)
	{
		if (isWarning)
		{
			AddWarning(error, bindingKey);
		}
		else
		{
			AddError(error, bindingKey);
		}
	}

	public ValidationState(string message, Exception error, string bindingKey, bool isWarning)
	{
		if (isWarning)
		{
			AddWarning(message, error, bindingKey);
		}
		else
		{
			AddError(message, error, bindingKey);
		}
	}

	public ValidationState(string message, string bindingKey)
	{
		AddError(message, bindingKey);
	}

	public ValidationState(Exception error, string bindingKey)
	{
		AddError(error, bindingKey);
	}

	public ValidationState(string message, Exception error, string bindingKey)
	{
		AddError(message, error, bindingKey);
	}

	public void AddError(string message, string bindingKey)
	{
		AddError(message, null, bindingKey);
	}

	public void AddError(Exception error, string bindingKey)
	{
		AddError(null, error, bindingKey);
	}

	public void AddError(string message, Exception error, string bindingKey)
	{
		string text = message;
		if (text == null && error != null)
		{
			text = error.Message;
		}
		results.Add(new ValidationResult(text, bindingKey, error, isWarning: false));
	}

	public void AddWarning(string message, string bindingKey)
	{
		AddWarning(message, null, bindingKey);
	}

	public void AddWarning(Exception error, string bindingKey)
	{
		AddWarning(null, error, bindingKey);
	}

	public void AddWarning(string message, Exception error, string bindingKey)
	{
		string text = message;
		if (text == null && error != null)
		{
			text = error.Message;
		}
		results.Add(new ValidationResult(text, bindingKey, error, isWarning: true));
	}
}
