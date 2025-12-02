using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog.Config;

namespace NLog.Internal;

/// <summary>
/// A minimal XML reader, because .NET System.Xml.XmlReader doesn't work with AOT
/// </summary>
internal sealed class XmlParser
{
	public sealed class XmlParserElement
	{
		private IList<XmlParserElement>? _children;

		private readonly IList<KeyValuePair<string, string>>? _attributes;

		public string Name { get; set; }

		public string? InnerText { get; set; }

		public IList<XmlParserElement> Children => _children ?? ArrayHelper.Empty<XmlParserElement>();

		public IList<KeyValuePair<string, string>> Attributes => _attributes ?? ArrayHelper.Empty<KeyValuePair<string, string>>();

		public XmlParserElement(string name, IList<KeyValuePair<string, string>>? attributes)
		{
			Name = name;
			_attributes = attributes;
		}

		public void AddChild(XmlParserElement child)
		{
			if (_children == null)
			{
				_children = new List<XmlParserElement>();
			}
			_children.Add(child);
		}
	}

	private sealed class CharEnumerator : IEnumerator<char>, IDisposable, IEnumerator
	{
		private readonly TextReader _xmlSource;

		private int _lineNumber;

		private char _current;

		private char? _peek;

		private bool _endOfFile;

		public char Current
		{
			get
			{
				if (_endOfFile)
				{
					throw new XmlParserException("Invalid XML document. Unexpected end of document.");
				}
				return _current;
			}
		}

		public int LineNumber => _lineNumber;

		object IEnumerator.Current => Current;

		public bool EndOfFile => _endOfFile;

		public CharEnumerator(TextReader xmlSource)
		{
			_xmlSource = xmlSource;
			int num = xmlSource.Read();
			_current = ((num >= 0) ? ((char)num) : '\0');
			_lineNumber = ((num != 10) ? 1 : 2);
		}

		public bool MoveNext()
		{
			if (_peek.HasValue)
			{
				_current = _peek.Value;
				if (_current == '\n')
				{
					_lineNumber++;
				}
				_peek = null;
				return true;
			}
			int num = _xmlSource.Read();
			if (num < 0)
			{
				_endOfFile = true;
				return false;
			}
			_current = (char)num;
			if (_current == '\n')
			{
				_lineNumber++;
			}
			return true;
		}

		public char Peek()
		{
			if (_peek.HasValue)
			{
				return _peek.Value;
			}
			int num = _xmlSource.Read();
			if (num < 0)
			{
				return '\0';
			}
			_peek = (char)num;
			return _peek.Value;
		}

		void IEnumerator.Reset()
		{
		}

		void IDisposable.Dispose()
		{
		}
	}

	private readonly CharEnumerator _xmlSource;

	private readonly StringBuilder _stringBuilder = new StringBuilder();

	private static readonly Dictionary<string, string> _specialTokens = new Dictionary<string, string>
	{
		{ "amp", "&" },
		{ "AMP", "&" },
		{ "apos", "'" },
		{ "APOS", "'" },
		{ "quot", "\"" },
		{ "QUOT", "\"" },
		{ "lt", "<" },
		{ "LT", "<" },
		{ "gt", ">" },
		{ "GT", ">" }
	};

	public XmlParser(TextReader xmlSource)
	{
		_xmlSource = new CharEnumerator(xmlSource);
	}

	public XmlParser(string xmlSource)
	{
		_xmlSource = new CharEnumerator(new StringReader(xmlSource));
	}

	public XmlParserElement LoadDocument(out IList<XmlParserElement>? processingInstructions)
	{
		try
		{
			TryReadProcessingInstructions(out processingInstructions);
			if (!TryReadStartElement(out string name, out List<KeyValuePair<string, string>> attributes))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse root start-tag");
			}
			Stack<XmlParserElement> stack = new Stack<XmlParserElement>();
			XmlParserElement xmlParserElement = new XmlParserElement(name ?? string.Empty, attributes);
			stack.Push(xmlParserElement);
			bool flag = true;
			while (flag)
			{
				flag = false;
				if (TryReadEndElement(xmlParserElement.Name))
				{
					flag = true;
					stack.Pop();
					if (stack.Count == 0)
					{
						break;
					}
					xmlParserElement = stack.Peek();
				}
				try
				{
					if (TryReadInnerText(out string innerText))
					{
						flag = true;
						xmlParserElement.InnerText += innerText;
					}
					if (TryReadStartElement(out string name2, out List<KeyValuePair<string, string>> attributes2))
					{
						flag = true;
						xmlParserElement = new XmlParserElement(name2 ?? string.Empty, attributes2);
						stack.Peek().AddChild(xmlParserElement);
						stack.Push(xmlParserElement);
					}
				}
				catch (XmlParserException ex)
				{
					throw new XmlParserException(ex.Message + " - Start-tag: " + xmlParserElement.Name);
				}
			}
			if (!flag)
			{
				throw new XmlParserException("Invalid XML document. Cannot parse end-tag: " + xmlParserElement.Name);
			}
			SkipWhiteSpaces();
			while (_xmlSource.Peek() == '!' && _xmlSource.Current == '<')
			{
				_xmlSource.MoveNext();
				SkipXmlComment();
			}
			if (_xmlSource.MoveNext())
			{
				throw new XmlParserException("Invalid XML document. Unexpected characters after end-tag: " + xmlParserElement.Name);
			}
			return xmlParserElement;
		}
		catch (XmlParserException ex2)
		{
			throw new XmlParserException(ex2.Message + $" - Line: {_xmlSource.LineNumber}");
		}
	}

	public bool TryReadProcessingInstructions(out IList<XmlParserElement>? processingInstructions)
	{
		SkipWhiteSpaces();
		processingInstructions = null;
		while (_xmlSource.Current == '<')
		{
			if (_xmlSource.Peek() == '!')
			{
				_xmlSource.MoveNext();
				SkipXmlComment();
				continue;
			}
			if (_xmlSource.Peek() != '?')
			{
				break;
			}
			if (!TryBeginReadStartElement(out string name, processingInstruction: true))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML processing instruction");
			}
			if (name == null || string.IsNullOrEmpty(name) || name.Length == 1 || name[0] != '?')
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML processing instruction");
			}
			name = name.Substring(1);
			TryReadAttributes(out List<KeyValuePair<string, string>> attributes, expectsProcessingInstruction: true);
			if (!SkipChar('?'))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML processing instruction: " + name);
			}
			if (!SkipChar('>'))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML processing instruction: " + name);
			}
			XmlParserElement item = new XmlParserElement(name, attributes);
			processingInstructions = processingInstructions ?? new List<XmlParserElement>();
			processingInstructions.Add(item);
			SkipWhiteSpaces();
		}
		return processingInstructions != null;
	}

	/// <summary>
	/// Reads a start element.
	/// </summary>
	/// <returns><see langword="true" /> if start element was found.</returns>
	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	public bool TryReadStartElement(out string? name, out List<KeyValuePair<string, string>>? attributes)
	{
		SkipWhiteSpaces();
		if (TryBeginReadStartElement(out name))
		{
			try
			{
				TryReadAttributes(out attributes);
				SkipChar('>');
			}
			catch (XmlParserException ex)
			{
				throw new XmlParserException(ex.Message + " - Cannot parse attributes for Start-tag: " + name);
			}
			return true;
		}
		name = null;
		attributes = null;
		return false;
	}

	/// <summary>
	/// Skips an end element.
	/// </summary>
	/// <param name="name">The name of the element to skip.</param>
	/// <returns><see langword="true" /> if an end element was skipped; otherwise, <see langword="false" />.</returns>
	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	public bool TryReadEndElement(string name)
	{
		SkipWhiteSpaces();
		if (_xmlSource.Current == '<' && _xmlSource.Peek() != '/')
		{
			return false;
		}
		if (_xmlSource.Current == '/' && _xmlSource.Peek() == '>')
		{
			if (SkipChar('/'))
			{
				return SkipChar('>');
			}
			return false;
		}
		if (!SkipChar('<'))
		{
			return false;
		}
		if (!SkipChar('/'))
		{
			throw new XmlParserException("Invalid XML document. Cannot parse end-tag: " + name);
		}
		for (int i = 0; i < name.Length; i++)
		{
			if (_xmlSource.Current != name[i])
			{
				throw new XmlParserException("Invalid XML document. Cannot parse end-tag: " + name);
			}
			if (!_xmlSource.MoveNext())
			{
				throw new XmlParserException("Invalid XML document. Cannot parse end-tag: " + name);
			}
		}
		if (!SkipChar('>'))
		{
			throw new XmlParserException("Invalid XML document. Cannot parse end-tag: " + name);
		}
		return true;
	}

	/// <summary>
	/// Reads content of an element.
	/// </summary>
	/// <returns>The content of the element.</returns>
	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	public bool TryReadInnerText(out string innerText)
	{
		char current = _xmlSource.Current;
		SkipWhiteSpaces();
		innerText = ReadUntilChar('<', includeSpaces: true);
		while (_xmlSource.Current == '<' && _xmlSource.Peek() == '!')
		{
			_xmlSource.MoveNext();
			current = _xmlSource.Current;
			if (_xmlSource.Peek() == '-')
			{
				SkipXmlComment();
			}
			else
			{
				if (_xmlSource.Peek() != '[')
				{
					throw new XmlParserException("Invalid XML document. Cannot parse XML comment");
				}
				innerText += ReadCDATA();
			}
			innerText += ReadUntilChar('<', includeSpaces: true);
		}
		SkipWhiteSpaces();
		if (string.IsNullOrEmpty(innerText) && _xmlSource.Current == '<')
		{
			return current != '<';
		}
		return true;
	}

	private string ReadCDATA()
	{
		if (!SkipCDATA())
		{
			throw new XmlParserException("Invalid XML document. Cannot parse XML CDATA");
		}
		_stringBuilder.ClearBuilder();
		do
		{
			if (_xmlSource.Current == ']' && _xmlSource.Peek() == ']')
			{
				_xmlSource.MoveNext();
				if (_xmlSource.Peek() == '>')
				{
					_xmlSource.MoveNext();
					_xmlSource.MoveNext();
					break;
				}
				_stringBuilder.Append(']');
			}
			_stringBuilder.Append(_xmlSource.Current);
		}
		while (_xmlSource.MoveNext());
		string result = _stringBuilder.ToString();
		SkipWhiteSpaces();
		return result;
	}

	private bool SkipCDATA()
	{
		if (!SkipChar('!'))
		{
			return false;
		}
		if (!SkipChar('['))
		{
			return false;
		}
		if (!SkipChar('C'))
		{
			return false;
		}
		if (!SkipChar('D'))
		{
			return false;
		}
		if (!SkipChar('A'))
		{
			return false;
		}
		if (!SkipChar('T'))
		{
			return false;
		}
		if (!SkipChar('A'))
		{
			return false;
		}
		if (!SkipChar('['))
		{
			return false;
		}
		return true;
	}

	private void SkipXmlComment()
	{
		if (!SkipChar('!') || !SkipChar('-') || !SkipChar('-'))
		{
			throw new XmlParserException("Invalid XML document. Cannot parse XML comment");
		}
		while (_xmlSource.MoveNext() && (!SkipChar('-') || !SkipChar('-') || !SkipChar('>')))
		{
		}
		SkipWhiteSpaces();
	}

	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	private bool TryReadAttributes(out List<KeyValuePair<string, string>>? attributes, bool expectsProcessingInstruction = false)
	{
		SkipWhiteSpaces();
		attributes = null;
		while (_xmlSource.Current != '>' && _xmlSource.Current != '/' && (!expectsProcessingInstruction || _xmlSource.Current != '?'))
		{
			string text = ReadUntilChar('=').Trim();
			if (string.IsNullOrEmpty(text))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML attribute");
			}
			if (!SkipChar('='))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse XML attribute");
			}
			bool flag = false;
			SkipWhiteSpaces();
			if (!SkipChar('"'))
			{
				if (!SkipChar('\''))
				{
					throw new XmlParserException("Invalid XML document. Cannot parse XML attribute: " + text);
				}
				flag = true;
			}
			try
			{
				string value = ReadUntilChar(flag ? '\'' : '"', includeSpaces: true);
				_xmlSource.MoveNext();
				attributes = attributes ?? new List<KeyValuePair<string, string>>();
				attributes.Add(new KeyValuePair<string, string>(text, value));
				SkipWhiteSpaces();
			}
			catch (XmlParserException ex)
			{
				throw new XmlParserException(ex.Message + " - XML attribute: " + text);
			}
		}
		return attributes != null;
	}

	/// <summary>
	/// Consumer of this method should handle safe position.
	/// </summary>
	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	private bool TryBeginReadStartElement(out string? name, bool processingInstruction = false)
	{
		if (_xmlSource.Current != '<' || _xmlSource.Peek() == '/' || _xmlSource.Peek() == '!')
		{
			name = null;
			return false;
		}
		_xmlSource.MoveNext();
		SkipWhiteSpaces();
		_stringBuilder.ClearBuilder();
		do
		{
			char current = _xmlSource.Current;
			if (CharIsSpace(current) || current == '/' || current == '>')
			{
				break;
			}
			if (processingInstruction && current == '?')
			{
				if (_stringBuilder.Length != 0)
				{
					throw new XmlParserException($"Invalid XML document. Cannot parse XML start-tag with character: {current}");
				}
			}
			else if (!IsValidXmlNameChar(current))
			{
				throw new XmlParserException($"Invalid XML document. Cannot parse XML start-tag with character: {current}");
			}
			_stringBuilder.Append(current);
		}
		while (_xmlSource.MoveNext());
		name = _stringBuilder.ToString();
		if (string.IsNullOrEmpty(name))
		{
			throw new XmlParserException("Invalid XML document. Cannot parse XML start-tag");
		}
		return true;
	}

	private bool SkipChar(char c)
	{
		if (_xmlSource.Current != c)
		{
			return false;
		}
		_xmlSource.MoveNext();
		return true;
	}

	private bool SkipWhiteSpaces()
	{
		bool result = false;
		while (!_xmlSource.EndOfFile && CharIsSpace(_xmlSource.Current) && _xmlSource.MoveNext())
		{
			result = true;
		}
		return result;
	}

	/// <exception cref="T:System.Exception">Something unexpected has failed.</exception>
	private string ReadUntilChar(char expectedChar, bool includeSpaces = false)
	{
		_stringBuilder.ClearBuilder();
		bool flag = false;
		do
		{
			char current = _xmlSource.Current;
			if (current == expectedChar)
			{
				break;
			}
			if (!includeSpaces && CharIsSpace(current))
			{
				SkipWhiteSpaces();
				if (_xmlSource.Current == expectedChar)
				{
					break;
				}
				throw new XmlParserException("Invalid XML document. Cannot parse attribute-name with white-space");
			}
			if (!includeSpaces && !IsValidXmlNameChar(current))
			{
				throw new XmlParserException($"Invalid XML document. Cannot parse attribute-name with character: {current}");
			}
			if (current == '<' && (!includeSpaces || expectedChar == '<'))
			{
				throw new XmlParserException("Invalid XML document. Cannot parse value with '<', maybe encode to &lt;");
			}
			if (includeSpaces && current == '&')
			{
				_xmlSource.MoveNext();
				string xmlToken;
				if (_xmlSource.Current == '#' && char.IsDigit(_xmlSource.Peek()))
				{
					int num = TryParseUnicodeValue();
					if (num != 0)
					{
						_stringBuilder.Append((char)num);
					}
				}
				else if (_xmlSource.Current == '#' && (_xmlSource.Peek() == 'x' || _xmlSource.Peek() == 'X'))
				{
					_xmlSource.MoveNext();
					int num2 = TryParseUnicodeValueHex();
					if (num2 != 0)
					{
						_stringBuilder.Append((char)num2);
					}
				}
				else if (TryParseSpecialXmlToken(out xmlToken))
				{
					_stringBuilder.Append(xmlToken);
				}
				else
				{
					_stringBuilder.Append('&');
					if (_xmlSource.Current == expectedChar)
					{
						break;
					}
					_stringBuilder.Append(_xmlSource.Current);
				}
				continue;
			}
			if (includeSpaces && expectedChar == '<')
			{
				if (_stringBuilder.Length == 0 && CharIsSpace(current))
				{
					continue;
				}
				flag = !flag && CharIsSpace(current);
			}
			_stringBuilder.Append(current);
		}
		while (_xmlSource.MoveNext());
		string text = _stringBuilder.ToString();
		if (!flag)
		{
			return text;
		}
		return text.TrimEnd(ArrayHelper.Empty<char>());
	}

	private static bool IsValidXmlNameChar(char chr)
	{
		if (char.IsLetter(chr) || char.IsDigit(chr))
		{
			return true;
		}
		switch (chr)
		{
		case '-':
		case '.':
		case ':':
		case '_':
			return true;
		default:
			return false;
		}
	}

	private int TryParseUnicodeValue()
	{
		int num = 0;
		while (_xmlSource.MoveNext() && _xmlSource.Current != ';')
		{
			if (_xmlSource.Current < '0' || _xmlSource.Current > '9')
			{
				throw new XmlParserException("Invalid XML document. Cannot parse unicode-char digit-value");
			}
			num *= 10;
			num += _xmlSource.Current - 48;
		}
		if (num >= 65535)
		{
			throw new XmlParserException("Invalid XML document. Unicode value exceeds maximum allowed value");
		}
		return num;
	}

	private int TryParseUnicodeValueHex()
	{
		int num = 0;
		while (_xmlSource.MoveNext() && _xmlSource.Current != ';')
		{
			num *= 16;
			char c = char.ToUpperInvariant(_xmlSource.Current);
			if (c >= 'A' && c <= 'F')
			{
				num += c - 65 + 10;
				continue;
			}
			if (c >= '0' && c <= '9')
			{
				num += c - 48;
				continue;
			}
			throw new XmlParserException("Invalid XML document. Cannot parse unicode-char hex-value");
		}
		if (num >= 65535)
		{
			throw new XmlParserException("Invalid XML document. Unicode value exceeds maximum allowed value");
		}
		return num;
	}

	private bool TryParseSpecialXmlToken(out string? xmlToken)
	{
		foreach (KeyValuePair<string, string> specialToken in _specialTokens)
		{
			if (_xmlSource.Current != specialToken.Key[0] || _xmlSource.Peek() != specialToken.Key[1])
			{
				continue;
			}
			string key = specialToken.Key;
			foreach (char c in key)
			{
				if (!SkipChar(c))
				{
					throw new XmlParserException("Invalid XML document. Cannot parse special token: " + specialToken.Key);
				}
			}
			if (_xmlSource.Current != ';')
			{
				throw new XmlParserException("Invalid XML document. Cannot parse special token: " + specialToken.Key);
			}
			xmlToken = specialToken.Value;
			return true;
		}
		xmlToken = null;
		return false;
	}

	private static bool CharIsSpace(char c)
	{
		switch (c)
		{
		case '\t':
		case '\n':
		case '\r':
		case ' ':
			return true;
		default:
			return char.IsWhiteSpace(c);
		}
	}
}
