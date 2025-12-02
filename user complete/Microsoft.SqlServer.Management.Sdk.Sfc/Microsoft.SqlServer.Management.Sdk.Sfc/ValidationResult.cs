using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class ValidationResult
{
	private string text;

	private string bindingKey;

	private Exception errorDetails;

	private bool isWarning;

	public string Text => text;

	public string BindingKey => bindingKey;

	public Exception ErrorDetails => errorDetails;

	public bool IsWarning => isWarning;

	internal ValidationResult(string text, string bindingKey, Exception errorDetails, bool isWarning)
	{
		this.isWarning = isWarning;
		this.text = text;
		this.bindingKey = bindingKey;
		this.errorDetails = errorDetails;
	}

	public override string ToString()
	{
		return Text;
	}
}
