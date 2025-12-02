using System;
using System.Collections.Generic;
using NLog.Conditions;
using NLog.Config;
using NLog.Internal;

namespace NLog.Targets;

/// <summary>
/// Highlighting rule for Win32 colorful console.
/// </summary>
[NLogConfigurationItem]
public class ConsoleWordHighlightingRule
{
	private string _text = string.Empty;

	/// <summary>
	/// Gets or sets the condition that must be met before scanning the row for highlight of words
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConditionExpression? Condition { get; set; }

	/// <summary>
	/// Gets or sets the text to be matched for Highlighting.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = (string.IsNullOrEmpty(value) ? string.Empty : value);
		}
	}

	/// <summary>
	/// Gets or sets the list of words to be matched for Highlighting.
	/// </summary>
	/// <docgen category="Highlighting Rules" order="10" />
	public List<string>? Words { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to match whole words only.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public bool WholeWords { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to ignore case when comparing texts.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public bool IgnoreCase { get; set; }

	/// <summary>
	/// Gets or sets the foreground color.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.ConsoleOutputColor.NoChange" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConsoleOutputColor ForegroundColor { get; set; } = ConsoleOutputColor.NoChange;

	/// <summary>
	/// Gets or sets the background color.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.ConsoleOutputColor.NoChange" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConsoleOutputColor BackgroundColor { get; set; } = ConsoleOutputColor.NoChange;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ConsoleWordHighlightingRule" /> class.
	/// </summary>
	public ConsoleWordHighlightingRule()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ConsoleWordHighlightingRule" /> class.
	/// </summary>
	/// <param name="text">The text to be matched..</param>
	/// <param name="foregroundColor">Color of the foreground.</param>
	/// <param name="backgroundColor">Color of the background.</param>
	public ConsoleWordHighlightingRule(string text, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
	{
		Text = text;
		ForegroundColor = foregroundColor;
		BackgroundColor = backgroundColor;
	}

	/// <summary>
	/// Scans the <paramref name="haystack" /> for words that should be highlighted.
	/// </summary>
	/// <returns>List of words with start-position (Key) and word-length (Value)</returns>
	protected internal virtual IEnumerable<KeyValuePair<int, int>>? GetWordsForHighlighting(string haystack)
	{
		if ((object)_text == string.Empty)
		{
			List<string>? words = Words;
			if (words != null && words.Count > 0)
			{
				return YieldWordMatchesForHighlighting(haystack, Words);
			}
			return null;
		}
		return YieldMatchesForHighlighting(_text, haystack);
	}

	private IEnumerable<KeyValuePair<int, int>>? YieldWordMatchesForHighlighting(string haystack, List<string> words)
	{
		IEnumerable<KeyValuePair<int, int>> enumerable = null;
		foreach (string word in words)
		{
			if (!string.IsNullOrEmpty(word))
			{
				IEnumerable<KeyValuePair<int, int>> enumerable2 = YieldMatchesForHighlighting(word, haystack);
				if (enumerable2 != null)
				{
					enumerable = ((enumerable == null) ? enumerable2 : MergeWordMatches(enumerable, enumerable2));
				}
			}
		}
		return enumerable;
	}

	private static IEnumerable<KeyValuePair<int, int>> MergeWordMatches(IEnumerable<KeyValuePair<int, int>> allMatches, IEnumerable<KeyValuePair<int, int>> needleMatch)
	{
		if (needleMatch is IList<KeyValuePair<int, int>> { Count: 1 } list)
		{
			KeyValuePair<int, int> newMatch = list[0];
			IList<KeyValuePair<int, int>> list2 = PrepareAllMatchesList(allMatches, 1);
			MergeAllNeedleMatches(list2, newMatch);
			return list2;
		}
		IList<KeyValuePair<int, int>> list3 = PrepareAllMatchesList(allMatches, 3);
		int startIndex = 0;
		foreach (KeyValuePair<int, int> item in needleMatch)
		{
			startIndex = MergeAllNeedleMatches(list3, item, startIndex);
		}
		return list3;
	}

	private static int MergeAllNeedleMatches(IList<KeyValuePair<int, int>> allMatchesList, KeyValuePair<int, int> newMatch, int startIndex = 0)
	{
		for (int i = startIndex; i < allMatchesList.Count; i++)
		{
			KeyValuePair<int, int> second = allMatchesList[i];
			if (NeedleMatchOverlaps(newMatch, second))
			{
				newMatch = MergeNeedleMatch(newMatch, second);
				allMatchesList[i] = newMatch;
				while (i < allMatchesList.Count - 1 && NeedleMatchOverlaps(newMatch, allMatchesList[i + 1]))
				{
					newMatch = MergeNeedleMatch(newMatch, allMatchesList[i + 1]);
					allMatchesList[i] = newMatch;
					allMatchesList.RemoveAt(i + 1);
				}
				return i;
			}
			if (newMatch.Key < second.Key)
			{
				allMatchesList.Insert(i, newMatch);
				return i + 1;
			}
		}
		allMatchesList.Add(newMatch);
		return allMatchesList.Count;
	}

	private static bool NeedleMatchOverlaps(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
	{
		if (first.Key < second.Key)
		{
			return first.Key + first.Value > second.Key;
		}
		return second.Key + second.Value > first.Key;
	}

	private static KeyValuePair<int, int> MergeNeedleMatch(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
	{
		if (first.Key < second.Key)
		{
			return new KeyValuePair<int, int>(first.Key, Math.Max(first.Key + first.Value, second.Key + second.Value) - first.Key);
		}
		return new KeyValuePair<int, int>(second.Key, Math.Max(first.Key + first.Value, second.Key + second.Value) - second.Key);
	}

	private static IList<KeyValuePair<int, int>> PrepareAllMatchesList(IEnumerable<KeyValuePair<int, int>> allMatches, int extraCapacity)
	{
		int num = 3;
		if (allMatches is IList<KeyValuePair<int, int>> list)
		{
			if (!list.IsReadOnly)
			{
				return list;
			}
			num = Math.Max(list.Count, num);
		}
		IList<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>(num + extraCapacity);
		foreach (KeyValuePair<int, int> allMatch in allMatches)
		{
			list2.Add(allMatch);
		}
		return list2;
	}

	private IEnumerable<KeyValuePair<int, int>>? YieldMatchesForHighlighting(string needle, string haystack)
	{
		int num = FindNextWordForHighlighting(needle, haystack, null);
		if (num < 0)
		{
			return null;
		}
		int num2 = FindNextWordForHighlighting(needle, haystack, num);
		if (num2 < 0)
		{
			return new KeyValuePair<int, int>[1] { GenerateMatch(needle, num) };
		}
		return YieldWordsForHighlighting(needle, haystack, num, num2);
	}

	private KeyValuePair<int, int> GenerateMatch(string needle, int startIndex)
	{
		int num = needle.Length;
		if (WholeWords)
		{
			for (int i = 0; i < needle.Length && !char.IsLetterOrDigit(needle[i]); i++)
			{
				num--;
				startIndex++;
			}
			if (num <= 0)
			{
				return new KeyValuePair<int, int>(startIndex - needle.Length, needle.Length);
			}
			int num2 = needle.Length - 1;
			while (num2 >= 0 && !char.IsLetterOrDigit(needle[num2]))
			{
				num--;
				num2--;
			}
		}
		return new KeyValuePair<int, int>(startIndex, num);
	}

	private IEnumerable<KeyValuePair<int, int>> YieldWordsForHighlighting(string needle, string haystack, int firstIndex, int nextIndex)
	{
		yield return GenerateMatch(needle, firstIndex);
		yield return GenerateMatch(needle, nextIndex);
		int index = nextIndex;
		while (index >= 0)
		{
			index = FindNextWordForHighlighting(needle, haystack, index);
			if (index >= 0)
			{
				yield return GenerateMatch(needle, index);
			}
		}
	}

	private int FindNextWordForHighlighting(string needle, string haystack, int? prevIndex)
	{
		int num;
		for (num = (prevIndex.HasValue ? (prevIndex.Value + needle.Length) : 0); num >= 0; num += needle.Length)
		{
			num = (IgnoreCase ? haystack.IndexOf(needle, num, StringComparison.CurrentCultureIgnoreCase) : haystack.IndexOf(needle, num, StringComparison.Ordinal));
			if (num < 0 || !WholeWords)
			{
				return num;
			}
			if (StringHelpers.IsWholeWord(haystack, needle, num) || ((num == 0 || !char.IsLetterOrDigit(needle[0])) && !char.IsLetterOrDigit(needle[needle.Length - 1])))
			{
				return num;
			}
		}
		return num;
	}

	/// <summary>
	/// Checks whether the specified log event matches the condition (if any).
	/// </summary>
	internal bool CheckCondition(LogEventInfo logEvent)
	{
		if (Condition != null)
		{
			return true.Equals(Condition.Evaluate(logEvent));
		}
		return true;
	}
}
