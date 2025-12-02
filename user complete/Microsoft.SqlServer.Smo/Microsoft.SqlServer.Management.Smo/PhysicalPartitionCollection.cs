using System;
using System.Collections;
using System.Text;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class PhysicalPartitionCollection : PartitionNumberedObjectCollectionBase
{
	public SqlSmoObject Parent => base.ParentInstance;

	public PhysicalPartition this[int index] => GetObjectByIndex(index) as PhysicalPartition;

	private Database Database
	{
		get
		{
			if (Parent is Table)
			{
				Table table = Parent as Table;
				return table.Parent;
			}
			if (Parent is Index)
			{
				Index index = Parent as Index;
				if (index.Parent is Table)
				{
					Table table2 = index.Parent as Table;
					return table2.Parent;
				}
				if (index.Parent is View)
				{
					View view = index.Parent as View;
					return view.Parent;
				}
				return null;
			}
			return null;
		}
	}

	internal PhysicalPartitionCollection(SqlSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public void Add(PhysicalPartition physicalPartition)
	{
		base.InternalStorage.Add(new PartitionNumberedObjectKey((short)physicalPartition.PartitionNumber), physicalPartition);
	}

	public void Remove(PhysicalPartition physicalPartition)
	{
		base.InternalStorage.Remove(new PartitionNumberedObjectKey((short)physicalPartition.PartitionNumber));
	}

	public void Remove(int partitionNumber)
	{
		base.InternalStorage.Remove(new PartitionNumberedObjectKey((short)partitionNumber));
	}

	protected override Type GetCollectionElementType()
	{
		return typeof(PhysicalPartition);
	}

	internal override SqlSmoObject GetCollectionElementInstance(ObjectKeyBase key, SqlSmoState state)
	{
		return new PhysicalPartition(this, key, state);
	}

	public void CopyTo(PhysicalPartition[] array, int partitionNumberStart)
	{
		if (int.MaxValue < partitionNumberStart || partitionNumberStart < 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.PartitionNumberStartOutOfRange(int.MaxValue));
		}
		((ICollection)this).CopyTo((Array)array, partitionNumberStart - 1);
	}

	public void CopyTo(PhysicalPartition[] array)
	{
		((ICollection)this).CopyTo((Array)array, 0);
	}

	public void CopyTo(PhysicalPartition[] array, int partitionNumberStart, int partitionNumberEnd)
	{
		if (partitionNumberStart > partitionNumberEnd)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotCopyPartition(partitionNumberStart, partitionNumberEnd));
		}
		PhysicalPartition[] array2 = new PhysicalPartition[base.InternalStorage.Count - partitionNumberStart + 1];
		((ICollection)this).CopyTo((Array)array2, partitionNumberStart - 1);
		for (int i = 0; i < partitionNumberEnd - partitionNumberStart + 1; i++)
		{
			array[i] = array2[i];
		}
	}

	internal void Reset()
	{
		Refresh();
	}

	private bool IsAppropriateForCompression()
	{
		if (Parent is UserDefinedTableType)
		{
			return false;
		}
		if (Parent is Index index && (index.IsMemoryOptimizedIndex || index.HasXmlColumn(throwIfNotSet: true) || (index.ServerVersion.Major < 11 && index.HasSpatialColumn(throwIfNotSet: true))))
		{
			return false;
		}
		return true;
	}

	internal void Reset(int partitionNumber)
	{
		TraceHelper.Assert(partitionNumber > 0);
		this[partitionNumber - 1].Refresh();
	}

	internal bool IsDataCompressionStateDirty(int partitionNumber)
	{
		TraceHelper.Assert(partitionNumber > 0);
		if (!IsAppropriateForCompression())
		{
			return false;
		}
		return this[partitionNumber - 1].IsDirty("DataCompression");
	}

	internal string GetCompressionCode(int partitionNumber)
	{
		TraceHelper.Assert(partitionNumber > 0);
		return this[partitionNumber - 1].DataCompression switch
		{
			DataCompressionType.None => string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = NONE "), 
			DataCompressionType.Row => string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = ROW "), 
			DataCompressionType.Page => string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = PAGE "), 
			DataCompressionType.ColumnStore => string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE "), 
			DataCompressionType.ColumnStoreArchive => string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE_ARCHIVE "), 
			_ => string.Empty, 
		};
	}

	internal bool IsCollectionDirty()
	{
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PhysicalPartition physicalPartition = (PhysicalPartition)enumerator.Current;
				if (physicalPartition.IsDirty("DataCompression"))
				{
					return true;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return false;
	}

	internal bool IsCompressionCodeRequired(bool isOnAlter)
	{
		if (!IsAppropriateForCompression())
		{
			return false;
		}
		if (base.Count == 0)
		{
			return false;
		}
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PhysicalPartition physicalPartition = (PhysicalPartition)enumerator.Current;
				if (physicalPartition.DataCompression != DataCompressionType.None)
				{
					return true;
				}
			}
			return isOnAlter;
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}

	private string ReformatCommaString(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return string.Empty;
		}
		char[] separator = new char[1] { ',' };
		string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder stringBuilder = new StringBuilder();
		if (array.Length == 0)
		{
			return string.Empty;
		}
		stringBuilder.Append(array[0]);
		int num = int.Parse(array[0]);
		int num2 = num;
		int num3 = num;
		int num4 = array.Length;
		for (int i = 1; i < num4; i++)
		{
			num3 = int.Parse(array[i]);
			if (num3 == num2 + 1)
			{
				num2 = num3;
				continue;
			}
			if (num == num2)
			{
				stringBuilder.Append($", {num3}");
			}
			else if (num == num2 - 1)
			{
				stringBuilder.Append($", {num2}, {num3}");
			}
			else
			{
				stringBuilder.Append($" TO {num2}, {num3}");
			}
			num = (num2 = num3);
		}
		if (num != num3 && num != num2)
		{
			if (num == num2 - 1)
			{
				stringBuilder.Append($", {num2}");
			}
			else
			{
				stringBuilder.Append($" TO {num2}");
			}
		}
		return stringBuilder.ToString();
	}

	private bool IsNonDescriptiveScriptAllowed()
	{
		if (Parent.State == SqlSmoState.Existing || !Parent.IsSupportedProperty("PartitionScheme"))
		{
			return true;
		}
		TraceHelper.Assert(Parent.State == SqlSmoState.Creating);
		if (Parent is Table table)
		{
			if (!table.IsDirty("PartitionScheme"))
			{
				ValidatePhysicalPartitionObject(table.Name);
				return true;
			}
			if (string.IsNullOrEmpty(table.PartitionScheme))
			{
				ValidatePhysicalPartitionObject(table.Name);
				return true;
			}
			return false;
		}
		if (Parent is Index index)
		{
			if (!index.IsDirty("PartitionScheme"))
			{
				ValidatePhysicalPartitionObject(index.Name);
				return true;
			}
			if (string.IsNullOrEmpty(index.PartitionScheme))
			{
				ValidatePhysicalPartitionObject(index.Name);
				return true;
			}
		}
		return false;
	}

	private void ValidatePhysicalPartitionObject(string objectName)
	{
		if (base.Count > 1 || (base.Count == 1 && this[0].PartitionNumber > 1))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.PartitionSchemeNotAssignedError(objectName), this, null);
		}
	}

	internal string GetCompressionCode(bool isOnAlter, bool isOnTable, ScriptingPreferences sp)
	{
		TraceHelper.Assert(sp.Storage.DataCompression);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		string text7 = Globals.commaspace;
		IEnumerator enumerator = GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PhysicalPartition physicalPartition = (PhysicalPartition)enumerator.Current;
				switch (physicalPartition.DataCompression)
				{
				case DataCompressionType.None:
					text3 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
					{
						physicalPartition.PartitionNumber,
						Globals.commaspace
					});
					num5++;
					break;
				case DataCompressionType.Row:
					text += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
					{
						physicalPartition.PartitionNumber,
						Globals.commaspace
					});
					num++;
					break;
				case DataCompressionType.Page:
					text2 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
					{
						physicalPartition.PartitionNumber,
						Globals.commaspace
					});
					num2++;
					break;
				case DataCompressionType.ColumnStore:
					if (isOnTable && !isOnAlter)
					{
						text3 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
						{
							physicalPartition.PartitionNumber,
							Globals.commaspace
						});
						num5++;
					}
					else
					{
						text4 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
						{
							physicalPartition.PartitionNumber,
							Globals.commaspace
						});
						num3++;
					}
					break;
				case DataCompressionType.ColumnStoreArchive:
					if (isOnTable && !isOnAlter)
					{
						text3 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
						{
							physicalPartition.PartitionNumber,
							Globals.commaspace
						});
						num5++;
					}
					else
					{
						text5 += string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
						{
							physicalPartition.PartitionNumber,
							Globals.commaspace
						});
						num4++;
					}
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			text = text.Trim(text7.ToCharArray());
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = text2.Trim(text7.ToCharArray());
		}
		if (!string.IsNullOrEmpty(text3))
		{
			text3 = text3.Trim(text7.ToCharArray());
		}
		if (!string.IsNullOrEmpty(text4))
		{
			text4 = text4.Trim(text7.ToCharArray());
		}
		if (!string.IsNullOrEmpty(text5))
		{
			text5 = text5.Trim(text7.ToCharArray());
		}
		text = ReformatCommaString(text);
		text2 = ReformatCommaString(text2);
		text3 = ReformatCommaString(text3);
		text4 = ReformatCommaString(text4);
		text5 = ReformatCommaString(text5);
		if (isOnAlter || num2 > 0)
		{
			text7 = string.Format(SmoApplication.DefaultCulture, "{0}{1}", new object[2]
			{
				Globals.comma,
				sp.NewLine
			});
		}
		if (num3 > 0)
		{
			if (num3 == base.Count)
			{
				return string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE");
			}
			text6 = string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE ON PARTITIONS ({0})", new object[1] { text4 });
		}
		if (num4 > 0)
		{
			if (num4 == base.Count && IsNonDescriptiveScriptAllowed())
			{
				return string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE_ARCHIVE");
			}
			if (!string.IsNullOrEmpty(text6))
			{
				text6 += text7;
			}
			text6 += string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = COLUMNSTORE_ARCHIVE ON PARTITIONS ({0})", new object[1] { text5 });
		}
		if (num > 0)
		{
			if (num == base.Count && IsNonDescriptiveScriptAllowed())
			{
				return string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = ROW");
			}
			TraceHelper.Assert(string.IsNullOrEmpty(text6));
			text6 = string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = ROW ON PARTITIONS ({0})", new object[1] { text });
		}
		if (num2 > 0)
		{
			if (num2 == base.Count && IsNonDescriptiveScriptAllowed())
			{
				return string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = PAGE");
			}
			if (!string.IsNullOrEmpty(text6))
			{
				text6 += text7;
			}
			text6 += string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = PAGE ON PARTITIONS ({0})", new object[1] { text2 });
		}
		if (isOnAlter && num5 > 0)
		{
			if (num5 == base.Count)
			{
				return string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = NONE");
			}
			if (!string.IsNullOrEmpty(text6))
			{
				text6 += text7;
			}
			text6 += string.Format(SmoApplication.DefaultCulture, "DATA_COMPRESSION = NONE ON PARTITIONS ({0})", new object[1] { text3 });
		}
		return text6;
	}
}
