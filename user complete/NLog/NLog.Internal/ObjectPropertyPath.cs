namespace NLog.Internal;

internal struct ObjectPropertyPath
{
	public string[]? PathNames { get; private set; }

	/// <summary>
	/// Object Path to check
	/// </summary>
	public string? Value
	{
		get
		{
			string[]? pathNames = PathNames;
			if (pathNames == null || pathNames.Length == 0)
			{
				return null;
			}
			return string.Join(".", PathNames);
		}
		set
		{
			PathNames = (StringHelpers.IsNullOrWhiteSpace(value) ? null : value?.SplitAndTrimTokens('.'));
		}
	}
}
