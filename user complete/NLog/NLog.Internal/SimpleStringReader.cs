using System;

namespace NLog.Internal;

/// <summary>
/// Simple character tokenizer.
/// </summary>
internal sealed class SimpleStringReader
{
	private readonly string _text;

	/// <summary>
	/// Current position in <see cref="P:NLog.Internal.SimpleStringReader.Text" />
	/// </summary>
	internal int Position { get; set; }

	/// <summary>
	/// Full text to be parsed
	/// </summary>
	internal string Text => _text;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Internal.SimpleStringReader" /> class.
	/// </summary>
	/// <param name="text">The text to be tokenized.</param>
	public SimpleStringReader(string text)
	{
		_text = text;
		Position = 0;
	}

	/// <summary>
	/// Check current char while not changing the position.
	/// </summary>
	/// <returns></returns>
	internal int Peek()
	{
		if (Position < _text.Length)
		{
			return _text[Position];
		}
		return -1;
	}

	/// <summary>
	/// Read the current char and change position
	/// </summary>
	/// <returns></returns>
	internal int Read()
	{
		if (Position < _text.Length)
		{
			return _text[Position++];
		}
		return -1;
	}

	/// <summary>
	/// Get the substring of the <see cref="P:NLog.Internal.SimpleStringReader.Text" />
	/// </summary>
	/// <param name="startIndex"></param>
	/// <param name="endIndex"></param>
	/// <returns></returns>
	internal string Substring(int startIndex, int endIndex)
	{
		return _text.Substring(startIndex, endIndex - startIndex);
	}

	internal string ReadUntilMatch(Func<int, bool> charFinder)
	{
		int position = Position;
		int arg;
		while ((arg = Peek()) != -1)
		{
			if (charFinder(arg))
			{
				return Substring(position, Position);
			}
			Read();
		}
		return Substring(position, Position);
	}
}
