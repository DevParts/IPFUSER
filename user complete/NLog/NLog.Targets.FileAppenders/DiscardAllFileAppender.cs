using System;
using NLog.Time;

namespace NLog.Targets.FileAppenders;

internal sealed class DiscardAllFileAppender : IFileAppender, IDisposable
{
	public string FilePath { get; }

	public DateTime OpenStreamTime { get; }

	public DateTime LastWriteTime => OpenStreamTime;

	public DateTime FileLastModified => OpenStreamTime;

	public DateTime NextArchiveTime => DateTime.MaxValue;

	public long FileSize => 0L;

	public DiscardAllFileAppender(string filePath)
	{
		FilePath = filePath;
		OpenStreamTime = TimeSource.Current.Time;
	}

	public void Write(byte[] buffer, int offset, int count)
	{
	}

	public void Flush()
	{
	}

	public void Dispose()
	{
	}

	public bool VerifyFileExists()
	{
		return true;
	}

	public override string ToString()
	{
		return FilePath;
	}
}
