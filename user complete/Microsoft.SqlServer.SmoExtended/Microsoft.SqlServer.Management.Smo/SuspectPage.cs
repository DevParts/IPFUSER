using System;

namespace Microsoft.SqlServer.Management.Smo;

public class SuspectPage : IComparable<SuspectPage>
{
	public int FileID { get; private set; }

	public long PageID { get; private set; }

	public SuspectPage(int fileID, long pageID)
	{
		FileID = fileID;
		PageID = pageID;
	}

	public override string ToString()
	{
		return string.Format(SmoApplication.DefaultCulture, "{0}:{1}", new object[2] { FileID, PageID });
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is SuspectPage))
		{
			return false;
		}
		SuspectPage suspectPage = obj as SuspectPage;
		if (FileID == suspectPage.FileID)
		{
			return PageID == suspectPage.PageID;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return PageID.GetHashCode() ^ FileID.GetHashCode();
	}

	public void Validate()
	{
		if (FileID <= 0 || PageID < 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.InvalidSuspectpage);
		}
		if (PageID == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotRestoreFileBootPage(FileID, PageID));
		}
		if (FileID == 1 && PageID == 9)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotRestoreDatabaseBootPage(FileID, PageID));
		}
	}

	public int CompareTo(SuspectPage other)
	{
		if (Equals(other))
		{
			return 0;
		}
		if (FileID > other.FileID)
		{
			return 1;
		}
		if (FileID < other.FileID)
		{
			return -1;
		}
		if (PageID > other.PageID)
		{
			return 1;
		}
		if (PageID < other.PageID)
		{
			return -1;
		}
		return 0;
	}
}
