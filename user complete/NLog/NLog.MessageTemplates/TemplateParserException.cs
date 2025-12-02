using System;

namespace NLog.MessageTemplates;

/// <summary>
/// Error when parsing a template.
/// </summary>
public class TemplateParserException : Exception
{
	/// <summary>
	/// Current index when the error occurred.
	/// </summary>
	public int Index { get; }

	/// <summary>
	/// The template we were parsing
	/// </summary>
	public string Template { get; }

	/// <summary>
	/// New exception
	/// </summary>
	/// <param name="message">The message to be shown.</param>
	/// <param name="index">Current index when the error occurred.</param>
	/// <param name="template"></param>
	public TemplateParserException(string message, int index, string template)
		: base(message)
	{
		Index = index;
		Template = template;
	}
}
