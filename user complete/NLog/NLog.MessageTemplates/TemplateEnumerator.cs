using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog.Internal;

namespace NLog.MessageTemplates;

/// <summary>
/// Parse templates.
/// </summary>
internal struct TemplateEnumerator : IEnumerator<LiteralHole>, IDisposable, IEnumerator
{
	private static readonly char[] HoleDelimiters = new char[3] { '}', ':', ',' };

	private static readonly char[] TextDelimiters = new char[2] { '{', '}' };

	private string _template;

	private int _length;

	private int _position;

	private int _literalLength;

	private LiteralHole _current;

	private const short Zero = 0;

	public string Template => _template;

	/// <summary>
	/// Gets the current literal/hole in the template
	/// </summary>
	public LiteralHole Current => _current;

	object IEnumerator.Current => _current;

	/// <summary>
	/// Parse a template.
	/// </summary>
	/// <param name="template">Template to be parsed.</param>
	/// <exception cref="T:System.ArgumentNullException">When <paramref name="template" /> is null.</exception>
	/// <returns>Template, never null</returns>
	public TemplateEnumerator(string template)
	{
		_template = Guard.ThrowIfNull(template, "template");
		_length = _template.Length;
		_position = 0;
		_literalLength = 0;
		_current = default(LiteralHole);
	}

	/// <summary>
	/// Clears the enumerator
	/// </summary>
	public void Dispose()
	{
		_template = string.Empty;
		_length = 0;
		Reset();
	}

	/// <summary>
	/// Restarts the enumerator of the template
	/// </summary>
	public void Reset()
	{
		_position = 0;
		_literalLength = 0;
		_current = default(LiteralHole);
	}

	/// <summary>
	/// Moves to the next literal/hole in the template
	/// </summary>
	/// <returns>Found new element [true/false]</returns>
	public bool MoveNext()
	{
		try
		{
			while (_position < _length)
			{
				switch (Peek())
				{
				case '{':
					ParseOpenBracketPart();
					return true;
				case '}':
					ParseCloseBracketPart();
					return true;
				}
				ParseTextPart();
			}
			if (_literalLength != 0)
			{
				AddLiteral();
				return true;
			}
			return false;
		}
		catch (IndexOutOfRangeException)
		{
			throw new TemplateParserException("Unexpected end of template.", _position, _template);
		}
	}

	private void AddLiteral()
	{
		_current = new LiteralHole(new Literal(_literalLength, 0), default(Hole));
		_literalLength = 0;
	}

	private void ParseTextPart()
	{
		_literalLength = SkipUntil(TextDelimiters, required: false);
	}

	private void ParseOpenBracketPart()
	{
		Skip('{');
		switch (Peek())
		{
		case '{':
			Skip('{');
			_literalLength++;
			AddLiteral();
			break;
		case '@':
			Skip('@');
			ParseHole(CaptureType.Serialize);
			break;
		case '$':
			Skip('$');
			ParseHole(CaptureType.Stringify);
			break;
		default:
			ParseHole(CaptureType.Normal);
			break;
		}
	}

	private void ParseCloseBracketPart()
	{
		Skip('}');
		if (Read() != '}')
		{
			throw new TemplateParserException("Unexpected '}}' ", _position - 2, _template);
		}
		_literalLength++;
		AddLiteral();
	}

	private void ParseHole(CaptureType type)
	{
		int position = _position;
		int parameterIndex;
		string name = ParseName(out parameterIndex);
		int num = 0;
		string format = null;
		if (Peek() != '}')
		{
			num = ((Peek() == ',') ? ParseAlignment() : 0);
			format = ((Peek() == ':') ? ParseFormat() : null);
			Skip('}');
		}
		else
		{
			_position++;
		}
		int skip = _position - position + ((type == CaptureType.Normal) ? 1 : 2);
		_current = new LiteralHole(new Literal(_literalLength, skip), new Hole(name, format, type, (short)parameterIndex, (short)num));
		_literalLength = 0;
	}

	private string ParseName(out int parameterIndex)
	{
		parameterIndex = -1;
		char c = Peek();
		if (c >= '0' && c <= '9')
		{
			int position = _position;
			int num = ReadInt();
			switch (Peek())
			{
			case ',':
			case ':':
			case '}':
				parameterIndex = num;
				return ParameterIndexToString(parameterIndex);
			case ' ':
				SkipSpaces();
				c = Peek();
				if (c == '}' || c == ':' || c == ',')
				{
					parameterIndex = num;
				}
				break;
			}
			_position = position;
		}
		return ReadUntil(HoleDelimiters);
	}

	private static string ParameterIndexToString(int parameterIndex)
	{
		return parameterIndex switch
		{
			0 => "0", 
			1 => "1", 
			2 => "2", 
			3 => "3", 
			4 => "4", 
			5 => "5", 
			6 => "6", 
			7 => "7", 
			8 => "8", 
			9 => "9", 
			_ => parameterIndex.ToString(CultureInfo.InvariantCulture), 
		};
	}

	/// <summary>
	/// Parse format after hole name/index. Handle the escaped { and } in the format. Don't read the last }
	/// </summary>
	/// <returns></returns>
	private string ParseFormat()
	{
		Skip(':');
		string text = ReadUntil(TextDelimiters);
		while (true)
		{
			switch (Read())
			{
			case '}':
				if (_position < _length && Peek() == '}')
				{
					Skip('}');
					text += "}";
					break;
				}
				_position--;
				return text;
			case '{':
			{
				char c = Peek();
				if (c == '{')
				{
					Skip('{');
					text += "{";
					break;
				}
				throw new TemplateParserException($"Expected '{{' but found '{c}' instead in format.", _position, _template);
			}
			}
			text += ReadUntil(TextDelimiters);
		}
	}

	private int ParseAlignment()
	{
		Skip(',');
		SkipSpaces();
		int result = ReadInt();
		SkipSpaces();
		char c = Peek();
		if (c != ':' && c != '}')
		{
			throw new TemplateParserException($"Expected ':' or '}}' but found '{c}' instead.", _position, _template);
		}
		return result;
	}

	private char Peek()
	{
		return _template[_position];
	}

	private char Read()
	{
		return _template[_position++];
	}

	private void Skip(char c)
	{
		_position++;
	}

	private void SkipSpaces()
	{
		while (_template[_position] == ' ')
		{
			_position++;
		}
	}

	private int SkipUntil(char[] search, bool required = true)
	{
		int position = _position;
		int num = _template.IndexOfAny(search, _position);
		if (num == -1 && required)
		{
			string text = string.Join(", ", search.Select((char c) => "'" + c + "'").ToArray());
			throw new TemplateParserException("Reached end of template while expecting one of " + text + ".", _position, _template);
		}
		_position = ((num == -1) ? _length : num);
		return _position - position;
	}

	private int ReadInt()
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < 12; i++)
		{
			char c = Peek();
			if (c < '0' || c > '9')
			{
				if (i > 0 && !flag)
				{
					return num;
				}
				if (i > 1 && flag)
				{
					return -num;
				}
				if (i != 0 || c != '-')
				{
					break;
				}
				flag = true;
				_position++;
			}
			else
			{
				_position++;
				num = num * 10 + (c - 48);
			}
		}
		throw new TemplateParserException("An integer is expected", _position, _template);
	}

	private string ReadUntil(char[] search, bool required = true)
	{
		int position = _position;
		return _template.Substring(position, SkipUntil(search, required));
	}
}
