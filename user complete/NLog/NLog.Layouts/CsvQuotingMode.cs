namespace NLog.Layouts;

/// <summary>
/// Specifies CSV quoting modes.
/// </summary>
public enum CsvQuotingMode
{
	/// <summary>
	/// Quote Always (Fast)
	/// </summary>
	All,
	/// <summary>
	/// Quote nothing (Very fast)
	/// </summary>
	Nothing,
	/// <summary>
	/// Quote only whose values contain the quote symbol or the separator (Slow)
	/// </summary>
	Auto
}
