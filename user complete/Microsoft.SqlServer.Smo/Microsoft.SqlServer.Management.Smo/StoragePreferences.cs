namespace Microsoft.SqlServer.Management.Smo;

internal class StoragePreferences
{
	internal bool FileStreamFileGroup { get; set; }

	internal bool FileStreamColumn { get; set; }

	public bool FileStream
	{
		get
		{
			if (FileStreamFileGroup)
			{
				return FileStreamColumn;
			}
			return false;
		}
		set
		{
			FileStreamColumn = value;
			FileStreamFileGroup = value;
		}
	}

	internal PartitioningScheme PartitionSchemeInternal { get; set; }

	public bool PartitionScheme
	{
		get
		{
			return (PartitionSchemeInternal & PartitioningScheme.All) == PartitioningScheme.All;
		}
		set
		{
			PartitionSchemeInternal = (value ? PartitioningScheme.All : PartitioningScheme.None);
		}
	}

	public bool DataCompression { get; set; }

	public bool FileGroup { get; set; }

	internal StoragePreferences()
	{
		Init();
	}

	private void Init()
	{
		DataCompression = true;
		PartitionSchemeInternal = PartitioningScheme.All;
		FileGroup = true;
		FileStream = true;
	}

	internal object Clone()
	{
		return MemberwiseClone();
	}
}
