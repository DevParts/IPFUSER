namespace NLog.Targets;

/// <summary>
/// Modes of archiving files based on time.
/// </summary>
public enum FileArchivePeriod
{
	/// <summary>
	/// Don't archive based on time.
	/// </summary>
	None,
	/// <summary>
	/// Archive every new year.
	/// </summary>
	Year,
	/// <summary>
	/// Archive every new month.
	/// </summary>
	Month,
	/// <summary>
	/// Archive every new day.
	/// </summary>
	Day,
	/// <summary>
	/// Archive every new hour.
	/// </summary>
	Hour,
	/// <summary>
	/// Archive every new minute.
	/// </summary>
	Minute,
	/// <summary>
	/// Archive every Sunday.
	/// </summary>
	Sunday,
	/// <summary>
	/// Archive every Monday.
	/// </summary>
	Monday,
	/// <summary>
	/// Archive every Tuesday.
	/// </summary>
	Tuesday,
	/// <summary>
	/// Archive every Wednesday.
	/// </summary>
	Wednesday,
	/// <summary>
	/// Archive every Thursday.
	/// </summary>
	Thursday,
	/// <summary>
	/// Archive every Friday.
	/// </summary>
	Friday,
	/// <summary>
	/// Archive every Saturday.
	/// </summary>
	Saturday
}
