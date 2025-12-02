using System;
using System.Collections;
using System.Collections.Generic;
using NLog.Common;
using NLog.Internal;

namespace NLog.MessageTemplates;

/// <summary>
/// Parameters extracted from parsing <see cref="P:NLog.LogEventInfo.Message" /> as MessageTemplate
/// </summary>
public sealed class MessageTemplateParameters : IEnumerable<MessageTemplateParameter>, IEnumerable
{
	internal static readonly MessageTemplateParameters Empty = new MessageTemplateParameters(string.Empty, ArrayHelper.Empty<object>());

	private readonly IList<MessageTemplateParameter> _parameters;

	/// <summary>
	/// Gets the parameters at the given index
	/// </summary>
	public MessageTemplateParameter this[int index] => _parameters[index];

	/// <summary>
	/// Number of parameters
	/// </summary>
	public int Count => _parameters.Count;

	/// <summary>Indicates whether the template should be interpreted as positional
	/// (all holes are numbers) or named.</summary>
	public bool IsPositional { get; }

	/// <summary>
	/// Indicates whether the template was parsed successful, and there are no unmatched parameters
	/// </summary>
	internal bool IsValidTemplate { get; }

	/// <inheritDoc />
	public IEnumerator<MessageTemplateParameter> GetEnumerator()
	{
		return _parameters.GetEnumerator();
	}

	/// <inheritDoc />
	IEnumerator IEnumerable.GetEnumerator()
	{
		return _parameters.GetEnumerator();
	}

	/// <summary>
	/// Constructor for parsing the message template with parameters
	/// </summary>
	/// <param name="message"><see cref="P:NLog.LogEventInfo.Message" /> including any parameter placeholders</param>
	/// <param name="parameters">All <see cref="P:NLog.LogEventInfo.Parameters" /></param>
	internal MessageTemplateParameters(string message, object?[] parameters)
	{
		if (parameters == null || parameters.Length == 0)
		{
			_parameters = ArrayHelper.Empty<MessageTemplateParameter>();
			IsPositional = false;
			IsValidTemplate = true;
		}
		else
		{
			_parameters = ParseMessageTemplate(message, parameters, out var isPositional, out var isValidTemplate);
			IsPositional = isPositional;
			IsValidTemplate = isValidTemplate;
		}
	}

	/// <summary>
	/// Constructor for named parameters that already has been parsed
	/// </summary>
	internal MessageTemplateParameters(IList<MessageTemplateParameter> templateParameters, string message, object?[]? parameters)
	{
		_parameters = templateParameters ?? ArrayHelper.Empty<MessageTemplateParameter>();
		if (parameters != null && _parameters.Count != parameters.Length)
		{
			IsValidTemplate = false;
		}
	}

	/// <summary>
	/// Create MessageTemplateParameter from <paramref name="parameters" />
	/// </summary>
	private static List<MessageTemplateParameter> ParseMessageTemplate(string template, object?[] parameters, out bool isPositional, out bool isValidTemplate)
	{
		isPositional = true;
		isValidTemplate = true;
		List<MessageTemplateParameter> list = new List<MessageTemplateParameter>(parameters.Length);
		try
		{
			short num = 0;
			TemplateEnumerator templateEnumerator = new TemplateEnumerator(template);
			while (templateEnumerator.MoveNext())
			{
				if (templateEnumerator.Current.Literal.Skip == 0)
				{
					continue;
				}
				Hole hole = templateEnumerator.Current.Hole;
				if ((hole.Index != -1) & isPositional)
				{
					num = GetMaxHoleIndex(num, hole.Index);
					object holeValueSafe = GetHoleValueSafe(parameters, hole.Index, ref isValidTemplate);
					list.Add(new MessageTemplateParameter(hole.Name, holeValueSafe, hole.Format, hole.CaptureType));
					continue;
				}
				if (isPositional)
				{
					isPositional = false;
					if (num != 0)
					{
						templateEnumerator = new TemplateEnumerator(template);
						num = 0;
						list.Clear();
						continue;
					}
				}
				object holeValueSafe2 = GetHoleValueSafe(parameters, num, ref isValidTemplate);
				list.Add(new MessageTemplateParameter(hole.Name, holeValueSafe2, hole.Format, hole.CaptureType));
				num++;
			}
			if (isPositional)
			{
				if (list.Count < parameters.Length || num != parameters.Length)
				{
					isValidTemplate = false;
				}
			}
			else if (list.Count != parameters.Length)
			{
				isValidTemplate = false;
			}
			return list;
		}
		catch (Exception ex)
		{
			isValidTemplate = false;
			InternalLogger.Warn(ex, "Error when parsing a message.");
			return list;
		}
	}

	private static short GetMaxHoleIndex(short maxHoleIndex, short holeIndex)
	{
		if (maxHoleIndex == 0)
		{
			maxHoleIndex++;
		}
		if (maxHoleIndex <= holeIndex)
		{
			maxHoleIndex = holeIndex;
			maxHoleIndex++;
		}
		return maxHoleIndex;
	}

	private static object? GetHoleValueSafe(object?[] parameters, short holeIndex, ref bool isValidTemplate)
	{
		if (parameters.Length > holeIndex)
		{
			return parameters[holeIndex];
		}
		isValidTemplate = false;
		return null;
	}
}
