using System;

namespace NLog.Targets.FileArchiveHandlers;

internal sealed class DisabledFileArchiveHandler : IFileArchiveHandler
{
	public static readonly IFileArchiveHandler Default = new DisabledFileArchiveHandler();

	public int ArchiveBeforeOpenFile(string newFileName, LogEventInfo firstLogEvent, DateTime? previousFileLastModified, int newSequenceNumber)
	{
		return 0;
	}
}
