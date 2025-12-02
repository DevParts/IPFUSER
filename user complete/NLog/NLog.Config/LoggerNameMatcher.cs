using System;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Encapsulates <see cref="P:NLog.Config.LoggingRule.LoggerNamePattern" /> and the logic to match the actual logger name
/// All subclasses defines immutable objects.
/// Concrete subclasses defines various matching rules through <see cref="M:NLog.Config.LoggerNameMatcher.NameMatches(System.String)" />
/// </summary>
internal abstract class LoggerNameMatcher
{
	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that never matches.
	/// Used when pattern is null
	/// </summary>
	private sealed class NoneLoggerNameMatcher : LoggerNameMatcher
	{
		public static readonly NoneLoggerNameMatcher Instance = new NoneLoggerNameMatcher();

		protected override string MatchMode => "None";

		private NoneLoggerNameMatcher()
			: base(string.Empty, null)
		{
		}

		public override bool NameMatches(string loggerName)
		{
			return false;
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that always matches.
	/// Used when pattern is '*'
	/// </summary>
	private sealed class AllLoggerNameMatcher : LoggerNameMatcher
	{
		public static readonly AllLoggerNameMatcher Instance = new AllLoggerNameMatcher();

		protected override string MatchMode => "All";

		private AllLoggerNameMatcher()
			: base("*", null)
		{
		}

		public override bool NameMatches(string loggerName)
		{
			return true;
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that matches with a case-sensitive Equals
	/// Used when pattern is a string without wildcards '?' '*'
	/// </summary>
	private sealed class EqualsLoggerNameMatcher : LoggerNameMatcher
	{
		protected override string MatchMode => "Equals";

		public EqualsLoggerNameMatcher(string pattern)
			: base(pattern, pattern)
		{
		}

		public override bool NameMatches(string loggerName)
		{
			return loggerName?.Equals(_matchingArgument, StringComparison.Ordinal) ?? (_matchingArgument == null);
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that matches with a case-sensitive StartsWith
	/// Used when pattern is a string like "*foobar"
	/// </summary>
	private sealed class StartsWithLoggerNameMatcher : LoggerNameMatcher
	{
		protected override string MatchMode => "StartsWith";

		public StartsWithLoggerNameMatcher(string pattern)
			: base(pattern, pattern.Substring(0, pattern.Length - 1))
		{
		}

		public override bool NameMatches(string loggerName)
		{
			return loggerName?.StartsWith(_matchingArgument, StringComparison.Ordinal) ?? (_matchingArgument == null);
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that matches with a case-sensitive EndsWith
	/// Used when pattern is a string like "foobar*"
	/// </summary>
	private sealed class EndsWithLoggerNameMatcher : LoggerNameMatcher
	{
		protected override string MatchMode => "EndsWith";

		public EndsWithLoggerNameMatcher(string pattern)
			: base(pattern, pattern.Substring(1))
		{
		}

		public override bool NameMatches(string loggerName)
		{
			return loggerName?.EndsWith(_matchingArgument, StringComparison.Ordinal) ?? (_matchingArgument == null);
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that matches with a case-sensitive Contains
	/// Used when pattern is a string like "*foobar*"
	/// </summary>
	private sealed class ContainsLoggerNameMatcher : LoggerNameMatcher
	{
		protected override string MatchMode => "Contains";

		public ContainsLoggerNameMatcher(string pattern)
			: base(pattern, pattern.Substring(1, pattern.Length - 2))
		{
		}

		public override bool NameMatches(string loggerName)
		{
			if (loggerName == null)
			{
				return _matchingArgument == null;
			}
			return loggerName.IndexOf(_matchingArgument, StringComparison.Ordinal) >= 0;
		}
	}

	/// <summary>
	/// Defines a <see cref="T:NLog.Config.LoggerNameMatcher" /> that matches with a complex wildcards combinations:
	/// <list type="bullet">
	/// <item>'*' means zero or more occurrences of any character</item>
	/// <item>'?' means exactly one occurrence of any character</item>
	/// </list>
	/// used when pattern is a string containing any number of '?' or '*' in any position
	/// i.e. "*Server[*].Connection[?]"
	/// </summary>
	private sealed class MultiplePatternLoggerNameMatcher : LoggerNameMatcher
	{
		private readonly string _wildCardPattern;

		protected override string MatchMode => "MultiplePattern";

		public MultiplePatternLoggerNameMatcher(string pattern)
			: base(pattern, pattern)
		{
			_wildCardPattern = Guard.ThrowIfNull(pattern, "pattern");
		}

		public override bool NameMatches(string loggerName)
		{
			return MatchingName(0, _wildCardPattern.Length, loggerName, 0, loggerName?.Length ?? 0);
		}

		private bool MatchingName(int wildcardStart, int wildcardEnd, string loggerName, int loggerNameStart, int loggerNameEnd)
		{
			for (int i = wildcardStart; i < wildcardEnd; i++)
			{
				char c = _wildCardPattern[i];
				if (c == '*')
				{
					break;
				}
				if (loggerNameStart >= loggerNameEnd)
				{
					return false;
				}
				char c2 = loggerName[loggerNameStart];
				if (c != '?' && c != c2)
				{
					return false;
				}
				loggerNameStart++;
				wildcardStart++;
			}
			for (int num = wildcardEnd - 1; num >= wildcardStart; num--)
			{
				char c3 = _wildCardPattern[num];
				if (c3 == '*')
				{
					break;
				}
				if (loggerNameStart >= loggerNameEnd)
				{
					return false;
				}
				char c4 = loggerName[loggerNameEnd - 1];
				if (c3 != '?' && c3 != c4)
				{
					return false;
				}
				loggerNameEnd--;
				wildcardEnd--;
			}
			char c5 = '*';
			for (int j = wildcardStart; j < wildcardEnd; j++)
			{
				c5 = _wildCardPattern[j];
				if (c5 != '*')
				{
					break;
				}
				wildcardStart++;
			}
			if (c5 == '*')
			{
				return true;
			}
			for (int k = loggerNameStart; k < loggerNameEnd; k++)
			{
				char c6 = loggerName[k];
				if (c5 == '?' || c6 == c5)
				{
					return MatchingName(wildcardStart, wildcardEnd, loggerName, k, loggerNameEnd);
				}
			}
			return false;
		}
	}

	protected readonly string? _matchingArgument;

	private readonly string _toString;

	public static LoggerNameMatcher Off => NoneLoggerNameMatcher.Instance;

	/// <summary>
	/// Returns the argument passed to <see cref="M:NLog.Config.LoggerNameMatcher.Create(System.String)" />
	/// </summary>
	public string Pattern { get; }

	protected abstract string MatchMode { get; }

	/// <summary>
	/// Creates a concrete <see cref="T:NLog.Config.LoggerNameMatcher" /> based on <paramref name="loggerNamePattern" />.
	/// </summary>
	/// <remarks>
	/// Rules used to select the concrete implementation returned:
	/// <list type="number">
	/// <item>if <paramref name="loggerNamePattern" /> is null =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.NoneLoggerNameMatcher" /> (never matches)</item>
	/// <item>if <paramref name="loggerNamePattern" /> doesn't contains any '*' nor '?' =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.EqualsLoggerNameMatcher" /> (matches only on case sensitive equals)</item>
	/// <item>if <paramref name="loggerNamePattern" /> == '*' =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.AllLoggerNameMatcher" /> (always matches)</item>
	/// <item>if <paramref name="loggerNamePattern" /> doesn't contain '?'
	/// <list type="number">
	///     <item>if <paramref name="loggerNamePattern" /> contains exactly 2 '*' one at the beginning and one at the end (i.e. "*foobar*) =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.ContainsLoggerNameMatcher" /></item>
	///     <item>if <paramref name="loggerNamePattern" /> contains exactly 1 '*' at the beginning (i.e. "*foobar") =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.EndsWithLoggerNameMatcher" /></item>
	///     <item>if <paramref name="loggerNamePattern" /> contains exactly 1 '*' at the end (i.e. "foobar*") =&gt; returns <see cref="T:NLog.Config.LoggerNameMatcher.StartsWithLoggerNameMatcher" /></item>
	/// </list>
	/// </item>
	/// <item>returns <see cref="T:NLog.Config.LoggerNameMatcher.MultiplePatternLoggerNameMatcher" /></item>
	/// </list>
	/// </remarks>
	/// <param name="loggerNamePattern">
	/// It may include one or more '*' or '?' wildcards at any position.
	/// <list type="bullet">
	/// <item>'*' means zero or more occurrences of any character</item>
	/// <item>'?' means exactly one occurrence of any character</item>
	/// </list>
	/// </param>
	/// <returns>A concrete <see cref="T:NLog.Config.LoggerNameMatcher" /></returns>
	public static LoggerNameMatcher Create(string loggerNamePattern)
	{
		if (loggerNamePattern == null)
		{
			return NoneLoggerNameMatcher.Instance;
		}
		if (loggerNamePattern.Trim() == "*")
		{
			return AllLoggerNameMatcher.Instance;
		}
		int num = loggerNamePattern.IndexOf('*');
		int num2 = loggerNamePattern.IndexOf('*', num + 1);
		int num3 = loggerNamePattern.IndexOf('?');
		if (num < 0 && num3 < 0)
		{
			return new EqualsLoggerNameMatcher(loggerNamePattern);
		}
		if (num3 < 0)
		{
			if (num == 0 && num2 == loggerNamePattern.Length - 1)
			{
				return new ContainsLoggerNameMatcher(loggerNamePattern);
			}
			if (num == 0 && num2 < 0)
			{
				return new EndsWithLoggerNameMatcher(loggerNamePattern);
			}
			if (num == loggerNamePattern.Length - 1 && num2 < 0)
			{
				return new StartsWithLoggerNameMatcher(loggerNamePattern);
			}
		}
		return new MultiplePatternLoggerNameMatcher(loggerNamePattern);
	}

	protected LoggerNameMatcher(string pattern, string? matchingArgument)
	{
		Pattern = pattern;
		_matchingArgument = matchingArgument;
		_toString = "logNamePattern: (" + matchingArgument + ":" + MatchMode + ")";
	}

	public override string ToString()
	{
		return _toString;
	}

	/// <summary>
	/// Checks whether given name matches the logger name pattern.
	/// </summary>
	/// <param name="loggerName">String to be matched.</param>
	/// <returns>A value of <see langword="true" /> when the name matches, <see langword="false" /> otherwise.</returns>
	public abstract bool NameMatches(string loggerName);
}
