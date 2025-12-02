using NLog.Internal;

namespace NLog.MessageTemplates;

/// <summary>
/// Description of a single parameter extracted from a MessageTemplate
/// </summary>
public struct MessageTemplateParameter
{
	/// <summary>
	/// Parameter Name extracted from <see cref="P:NLog.LogEventInfo.Message" />
	/// This is everything between "{" and the first of ",:}".
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Parameter Value extracted from the <see cref="P:NLog.LogEventInfo.Parameters" />-array
	/// </summary>
	public object? Value { get; }

	/// <summary>
	/// Format to render the parameter.
	/// This is everything between ":" and the first unescaped "}"
	/// </summary>
	public string? Format { get; }

	/// <summary>
	/// Parameter method that should be used to render the parameter
	/// See also <see cref="T:NLog.IValueFormatter" />
	/// </summary>
	public CaptureType CaptureType { get; }

	/// <summary>
	/// Returns index for <see cref="P:NLog.LogEventInfo.Parameters" />, when <see cref="P:NLog.MessageTemplates.MessageTemplateParameters.IsPositional" />
	/// </summary>
	public int? PositionalIndex
	{
		get
		{
			switch (Name)
			{
			case "0":
				return 0;
			case "1":
				return 1;
			case "2":
				return 2;
			case "3":
				return 3;
			case "4":
				return 4;
			case "5":
				return 5;
			case "6":
				return 6;
			case "7":
				return 7;
			case "8":
				return 8;
			case "9":
				return 9;
			default:
			{
				string name = Name;
				if (name != null && name.Length >= 1 && Name[0] >= '0' && Name[0] <= '9' && int.TryParse(Name, out var result))
				{
					return result;
				}
				return null;
			}
			}
		}
	}

	/// <summary>
	/// Constructs a single message template parameter
	/// </summary>
	/// <param name="name">Parameter Name</param>
	/// <param name="value">Parameter Value</param>
	/// <param name="format">Parameter Format</param>
	internal MessageTemplateParameter(string name, object? value, string? format)
	{
		Name = Guard.ThrowIfNull(name, "name");
		Value = value;
		Format = format;
		CaptureType = CaptureType.Normal;
	}

	/// <summary>
	/// Constructs a single message template parameter
	/// </summary>
	/// <param name="name">Parameter Name</param>
	/// <param name="value">Parameter Value</param>
	/// <param name="format">Parameter Format</param>
	/// <param name="captureType">Parameter CaptureType</param>
	public MessageTemplateParameter(string name, object? value, string? format, CaptureType captureType)
	{
		Name = Guard.ThrowIfNull(name, "name");
		Value = value;
		Format = format;
		CaptureType = captureType;
	}
}
