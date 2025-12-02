using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
public class DatabaseSnapshotExtender : SmoObjectExtender<Database>, ISfcValidate
{
	private ReadOnlyCollection<DataFile> files;

	private string name;

	[ExtendedProperty]
	public ReadOnlyCollection<DataFile> Files
	{
		get
		{
			if (files == null)
			{
				files = CreateFilesCollection();
			}
			return files;
		}
	}

	public DatabaseSnapshotExtender()
	{
	}

	public DatabaseSnapshotExtender(Database database)
		: base(database)
	{
		name = database.Name;
	}

	private ReadOnlyCollection<DataFile> CreateFilesCollection()
	{
		List<DataFile> list = new List<DataFile>();
		if (!string.IsNullOrEmpty(base.Parent.DatabaseSnapshotBaseName))
		{
			base.Parent.FileGroups.Clear();
			if (base.Parent.Parent != null)
			{
				Database database = base.Parent.Parent.Databases[base.Parent.DatabaseSnapshotBaseName];
				if (database != null)
				{
					foreach (FileGroup fileGroup3 in database.FileGroups)
					{
						FileGroup fileGroup2 = new FileGroup(base.Parent, fileGroup3.Name);
						fileGroup2.IsDefault = fileGroup3.IsDefault;
						fileGroup2.ReadOnly = fileGroup3.ReadOnly;
						base.Parent.FileGroups.Add(fileGroup2);
						foreach (DataFile file in fileGroup3.Files)
						{
							DataFile dataFile2 = new DataFile(fileGroup2, file.Name);
							dataFile2.FileName = Path.Combine(Path.GetDirectoryName(file.FileName), base.Parent.Name + "-" + (string.IsNullOrEmpty(file.Name) ? "Primary" : file.Name) + ".ss");
							dataFile2.Growth = file.Growth;
							dataFile2.GrowthType = file.GrowthType;
							dataFile2.MaxSize = file.MaxSize;
							dataFile2.Size = file.Size;
							dataFile2.IsPrimaryFile = file.IsPrimaryFile;
							fileGroup2.Files.Add(dataFile2);
							list.Add(dataFile2);
						}
					}
				}
			}
		}
		return new ReadOnlyCollection<DataFile>(list);
	}

	protected override void parent_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
		case "DatabaseSnapshotBaseName":
			files = CreateFilesCollection();
			break;
		case "Name":
			ProcessDataFileNames();
			name = base.Parent.Name;
			break;
		}
		base.parent_PropertyChanged(sender, e);
	}

	private void ProcessDataFileNames()
	{
		foreach (DataFile file in Files)
		{
			ProcessDataFileName(file, name, base.Parent.Name);
		}
	}

	private void ProcessDataFileName(DataFile dataFile, string oldName, string newName)
	{
		string fileName = dataFile.FileName;
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		Path.GetExtension(fileName);
		if (fileNameWithoutExtension.StartsWith(oldName, StringComparison.OrdinalIgnoreCase))
		{
			dataFile.FileName = Path.Combine(Path.GetDirectoryName(fileName), newName + ((fileNameWithoutExtension.Length > oldName.Length) ? fileNameWithoutExtension.Substring(oldName.Length) : "") + Path.GetExtension(fileName));
		}
	}

	ValidationState ISfcValidate.Validate(string methodName, params object[] arguments)
	{
		ValidationState validationState = base.Parent.Validate(methodName, arguments);
		if (string.IsNullOrEmpty(base.Parent.Name))
		{
			validationState.AddError(ExceptionTemplatesImpl.PropertyNotSet("Name", "Snapshot"), "Name");
		}
		return validationState;
	}
}
