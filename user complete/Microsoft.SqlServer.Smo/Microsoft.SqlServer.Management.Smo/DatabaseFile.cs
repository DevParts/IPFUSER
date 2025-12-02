using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseFile : NamedSmoObject, IAlterable, IDroppable, IRenamable, IMarkForDrop
{
	private string DatabaseName
	{
		get
		{
			if (this is LogFile)
			{
				return base.ParentColl.ParentInstance.InternalName;
			}
			return base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName;
		}
	}

	internal DatabaseFile(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	protected internal DatabaseFile()
	{
	}

	public void Drop()
	{
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		CheckObjectState();
		dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}]  REMOVE FILE [{1}]", new object[2]
		{
			SqlSmoObject.SqlBraket(DatabaseName),
			SqlSmoObject.SqlBraket(Name)
		}));
	}

	internal bool ScriptDdl(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, bool scriptCreate)
	{
		return ScriptDdl(sp, ddl, bSuppressDirtyCheck, scriptCreate, "");
	}

	internal bool ScriptDdl(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, bool scriptCreate, string onlyThisProperty)
	{
		ddl.Append(Globals.LParen);
		int changeCount = 0;
		ddl.AppendFormat(SmoApplication.DefaultCulture, " NAME = N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		if (onlyThisProperty == null)
		{
			onlyThisProperty = "";
		}
		switch (onlyThisProperty)
		{
		case "FileName":
			if (!ScriptFileName(sp, ddl, bSuppressDirtyCheck, scriptCreate, ref changeCount))
			{
				return false;
			}
			break;
		case "Size":
			ScriptSize(sp, ddl, bSuppressDirtyCheck, ref changeCount);
			break;
		case "MaxSize":
			ScriptMaxSize(sp, ddl, bSuppressDirtyCheck, ref changeCount);
			break;
		case "Growth":
			ScriptGrowth(sp, ddl, bSuppressDirtyCheck, scriptCreate, ref changeCount);
			break;
		default:
			if (sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlManagedInstance && !ScriptFileName(sp, ddl, bSuppressDirtyCheck, scriptCreate, ref changeCount))
			{
				throw new PropertyNotSetException("FileName");
			}
			ScriptSize(sp, ddl, bSuppressDirtyCheck, ref changeCount);
			ScriptMaxSize(sp, ddl, bSuppressDirtyCheck, ref changeCount);
			ScriptGrowth(sp, ddl, bSuppressDirtyCheck, scriptCreate, ref changeCount);
			break;
		}
		if (changeCount > 0)
		{
			ddl.Append(Globals.RParen);
			return true;
		}
		return false;
	}

	private void AppendSize(ScriptingPreferences sp, StringBuilder ddl, double size)
	{
		bool flag = base.ServerVersion.Major == 7 || sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70;
		bool flag2 = false;
		if (size > 2147483647.0)
		{
			size /= (double)(1 << (flag ? 10 : 20));
			flag2 = true;
		}
		if (0.0 > size || 2147483647.0 < size)
		{
			throw new SmoException(ExceptionTemplatesImpl.WrongSize);
		}
		if (flag2)
		{
			ddl.AppendFormat(SmoApplication.DefaultCulture, "{0}{1} ", new object[2]
			{
				Convert.ToInt32(size, SmoApplication.DefaultCulture),
				flag ? "MB" : "GB"
			});
		}
		else
		{
			ddl.AppendFormat(SmoApplication.DefaultCulture, "{0}KB ", new object[1] { Convert.ToInt32(size, SmoApplication.DefaultCulture) });
		}
	}

	private void ScriptSize(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, ref int changeCount)
	{
		Property property = base.Properties.Get("Size");
		if (IsFileStreamBasedFile())
		{
			if (property.Dirty)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidSizeFileStream);
			}
		}
		else if (property.Value != null && (bSuppressDirtyCheck || property.Dirty))
		{
			ddl.Append(", SIZE = ");
			AppendSize(sp, ddl, (double)property.Value);
			changeCount++;
		}
	}

	private void ScriptMaxSize(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, ref int changeCount)
	{
		Property property = base.Properties.Get("MaxSize");
		if (IsFileStreamBasedFile() && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version110)
		{
			if (property.Dirty)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidMaxSizeFileStream);
			}
		}
		else if (property.Value != null && (bSuppressDirtyCheck || property.Dirty))
		{
			ddl.Append(", MAXSIZE = ");
			if ((double)property.Value == 0.0 || (double)property.Value == -1.0)
			{
				ddl.Append("UNLIMITED");
			}
			else
			{
				AppendSize(sp, ddl, (double)property.Value);
			}
			changeCount++;
		}
	}

	private void ScriptGrowth(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, bool scriptCreate, ref int changeCount)
	{
		Property property = base.Properties.Get("GrowthType");
		Property property2 = base.Properties.Get("Growth");
		if (IsFileStreamBasedFile())
		{
			if (property2.Dirty || property.Dirty)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidGrowthFileStream);
			}
			return;
		}
		if ((property.Value == null || FileGrowthType.None == (FileGrowthType)property.Value) && (bSuppressDirtyCheck || property.Dirty))
		{
			ddl.Append(", FILEGROWTH = 0");
			changeCount++;
			if (property2.Value == null || (!bSuppressDirtyCheck && !property2.Dirty) || (double)property2.Value == 0.0)
			{
				return;
			}
			throw new WrongPropertyValueException(property2);
		}
		bool flag = property.Value != null && FileGrowthType.Percent == (FileGrowthType)property.Value;
		if (property2.Value != null && (bSuppressDirtyCheck || property2.Dirty || property.Dirty))
		{
			ddl.AppendFormat(SmoApplication.DefaultCulture, ", FILEGROWTH = ");
			double num = (double)property2.Value;
			if (flag)
			{
				if (1.0 > num)
				{
					throw new SmoException(ExceptionTemplatesImpl.WrongPercentageGrowth);
				}
				ddl.AppendFormat(SmoApplication.DefaultCulture, "{0}%", new object[1] { Convert.ToString(property2.Value, SmoApplication.DefaultCulture) });
			}
			else
			{
				AppendSize(sp, ddl, num);
			}
			changeCount++;
		}
		else if (!scriptCreate && property.Dirty)
		{
			throw new SmoException(ExceptionTemplatesImpl.MustSpecifyGrowth);
		}
	}

	private bool ScriptFileName(ScriptingPreferences sp, StringBuilder ddl, bool bSuppressDirtyCheck, bool scriptCreate, ref int changeCount)
	{
		Property property = base.Properties.Get("FileName");
		if (property.Value != null && (bSuppressDirtyCheck || property.Dirty))
		{
			string s = (string)property.Value;
			ddl.AppendFormat(SmoApplication.DefaultCulture, ", FILENAME = N'{0}' ", new object[1] { SqlSmoObject.SqlString(s) });
			changeCount++;
		}
		else if (property.Value == null && scriptCreate)
		{
			return false;
		}
		return true;
	}

	private bool IsFileStreamBasedFile()
	{
		if (!(this is DataFile { Parent: var parent }))
		{
			return false;
		}
		switch (parent.FileGroupType)
		{
		case FileGroupType.RowsFileGroup:
			return false;
		case FileGroupType.FileStreamDataFileGroup:
		case FileGroupType.MemoryOptimizedDataFileGroup:
			return true;
		default:
			throw new SmoException(ExceptionTemplatesImpl.UnsupportedFileGroupType);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (base.ServerVersion.Major == 7)
		{
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE ", new object[1] { SqlSmoObject.SqlBraket(DatabaseName) });
			if (ScriptDdl(sp, stringBuilder, bSuppressDirtyCheck: false, scriptCreate: false, "FileName"))
			{
				alterQuery.Add(stringBuilder.ToString());
			}
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE ", new object[1] { SqlSmoObject.SqlBraket(DatabaseName) });
			if (ScriptDdl(sp, stringBuilder, bSuppressDirtyCheck: false, scriptCreate: false, "Size"))
			{
				alterQuery.Add(stringBuilder.ToString());
			}
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE ", new object[1] { SqlSmoObject.SqlBraket(DatabaseName) });
			if (ScriptDdl(sp, stringBuilder, bSuppressDirtyCheck: false, scriptCreate: false, "MaxSize"))
			{
				alterQuery.Add(stringBuilder.ToString());
			}
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE ", new object[1] { SqlSmoObject.SqlBraket(DatabaseName) });
			if (ScriptDdl(sp, stringBuilder, bSuppressDirtyCheck: false, scriptCreate: false, "Growth"))
			{
				alterQuery.Add(stringBuilder.ToString());
			}
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE ", new object[1] { SqlSmoObject.SqlBraket(DatabaseName) });
			if (ScriptDdl(sp, stringBuilder, bSuppressDirtyCheck: false, scriptCreate: false, null))
			{
				alterQuery.Add(stringBuilder.ToString());
			}
		}
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		if (base.ServerVersion.Major == 7)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotRenameObject(GetType().Name, base.ServerVersion.ToString()));
		}
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE (NAME=N'{1}', NEWNAME=N'{2}')", new object[3]
		{
			SqlSmoObject.SqlBraket(DatabaseName),
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	public void Shrink(int newSizeInMB, ShrinkMethod shrinkType)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			Urn urn = base.Urn;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(urn.GetNameForType("Database")) });
			stringCollection.Add(stringBuilder.ToString());
			stringBuilder.Remove(0, stringBuilder.Length);
			stringBuilder.Append("DBCC");
			string name = Name;
			switch (shrinkType)
			{
			case ShrinkMethod.Default:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SHRINKFILE2, new object[2]
				{
					SqlSmoObject.SqlString(name),
					newSizeInMB
				});
				break;
			case ShrinkMethod.NoTruncate:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SHRINKFILE2, new object[2]
				{
					SqlSmoObject.SqlString(name),
					"NOTRUNCATE"
				});
				break;
			case ShrinkMethod.TruncateOnly:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SHRINKFILE3, new object[3]
				{
					SqlSmoObject.SqlString(name),
					newSizeInMB,
					"TRUNCATEONLY"
				});
				break;
			case ShrinkMethod.EmptyFile:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SHRINKFILE2, new object[2]
				{
					SqlSmoObject.SqlString(name),
					"EMPTYFILE"
				});
				break;
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownShrinkType);
			}
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Shrink, this, ex);
		}
	}
}
