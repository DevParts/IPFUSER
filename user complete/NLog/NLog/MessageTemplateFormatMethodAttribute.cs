using System;

namespace NLog;

/// <summary>
/// Mark a parameter of a method for message templating
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class MessageTemplateFormatMethodAttribute : Attribute
{
	/// <summary>
	/// The name of the parameter that should be as treated as message-template-string
	/// </summary>
	public string ParameterName { get; }

	/// <param name="parameterName">
	/// Specifies which parameter of an annotated method should be treated as message-template-string
	/// </param>
	public MessageTemplateFormatMethodAttribute(string parameterName)
	{
		ParameterName = parameterName;
	}
}
