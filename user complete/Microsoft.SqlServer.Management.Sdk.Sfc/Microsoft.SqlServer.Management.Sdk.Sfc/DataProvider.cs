using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class DataProvider : IDataReader, IDisposable, IDataRecord
{
	public enum RetriveMode
	{
		RetriveDataReader,
		RetriveDataTable
	}

	private struct ColumnDataManipulation
	{
		public Type targetType;

		public Type sourceType;
	}

	private ExecuteSql m_execSql;

	private DataTable m_table;

	private SqlDataReader m_dataReader;

	private int nonTriggeredPropertiesCount;

	private object[] rowData;

	private ColumnDataManipulation[] rowDataManipulation;

	private bool m_bHasTypeCast;

	private bool m_bHasPostProcess;

	private RetriveMode m_RetriveMode;

	private int m_nCurentCachePos;

	private SqlCommand m_command;

	public int Depth => m_dataReader.Depth;

	public bool IsClosed => m_dataReader.IsClosed;

	public int RecordsAffected => -1;

	public int FieldCount => nonTriggeredPropertiesCount;

	public object this[int idx] => rowData[idx];

	public object this[string name] => this[m_dataReader.GetOrdinal(name)];

	public DataProvider(StatementBuilder sb)
	{
		Init(sb, RetriveMode.RetriveDataReader);
	}

	public DataProvider(StatementBuilder sb, RetriveMode rm)
	{
		Init(sb, rm);
	}

	private void Init(StatementBuilder sb, RetriveMode rm)
	{
		m_RetriveMode = rm;
		m_dataReader = null;
		m_command = null;
		nonTriggeredPropertiesCount = sb.NonTriggeredProperties;
		if (RetriveMode.RetriveDataTable == m_RetriveMode)
		{
			rowData = new object[sb.ParentProperties.Count];
		}
		else
		{
			rowData = new object[nonTriggeredPropertiesCount];
		}
		InitRowDataManipulation(sb.ParentProperties, sb.PostProcessList);
		InitSchemaTable(sb.ParentProperties);
	}

	public void SetConnectionAndQuery(ExecuteSql execSql, string query)
	{
		m_execSql = execSql;
		m_dataReader = m_execSql.GetDataReader(query, out m_command);
		if (RetriveMode.RetriveDataTable != m_RetriveMode)
		{
			return;
		}
		try
		{
			while (ReadInternal())
			{
				ManipulateRowDataType();
				DataRow dataRow = m_table.NewRow();
				for (int i = 0; i < rowData.Length; i++)
				{
					if (rowData[i] == null)
					{
						dataRow[i] = DBNull.Value;
					}
					else
					{
						dataRow[i] = rowData[i];
					}
				}
				m_table.Rows.Add(dataRow);
			}
			m_command.Dispose();
			m_command = null;
		}
		finally
		{
			rowData = null;
			if (m_command != null)
			{
				m_command.Cancel();
				m_command.Dispose();
				m_command = null;
			}
			m_dataReader.Close();
			m_dataReader = null;
			m_execSql.Disconnect();
			m_execSql = null;
		}
	}

	public void InitRowDataManipulation(ArrayList parentProperties, SortedList postProcessList)
	{
		rowDataManipulation = new ColumnDataManipulation[nonTriggeredPropertiesCount];
		m_bHasTypeCast = false;
		m_bHasPostProcess = postProcessList != null && postProcessList.Count > 0;
		SortedList sortedList = null;
		if (m_bHasPostProcess)
		{
			sortedList = new SortedList(StringComparer.Ordinal);
			for (int i = nonTriggeredPropertiesCount; i < parentProperties.Count; i++)
			{
				sortedList[((SqlObjectProperty)parentProperties[i]).Alias] = i;
			}
		}
		int num = 0;
		foreach (SqlObjectProperty parentProperty in parentProperties)
		{
			if (Util.DbTypeToClrType(parentProperty.DBType) != parentProperty.Type)
			{
				Type targetType = SfcRegistration.TryGetObjectTypeFromFullName(parentProperty.Type);
				rowDataManipulation[num].targetType = targetType;
				rowDataManipulation[num].sourceType = Type.GetType(Util.DbTypeToClrType(parentProperty.DBType));
				m_bHasTypeCast = true;
			}
			if (++num >= nonTriggeredPropertiesCount)
			{
				break;
			}
		}
	}

	public void InitSchemaTable(ArrayList parentProperties)
	{
		m_table = new DataTable();
		m_table.Locale = CultureInfo.InvariantCulture;
		int num = 0;
		foreach (SqlObjectProperty parentProperty in parentProperties)
		{
			string fullTypeName = parentProperty.Type;
			if (parentProperty.ExtendedType)
			{
				fullTypeName = "System.Int32";
			}
			Type dataType = SfcRegistration.TryGetObjectTypeFromFullName(fullTypeName);
			m_table.Columns.Add(new DataColumn(parentProperty.Alias, dataType));
			if (m_RetriveMode == RetriveMode.RetriveDataReader && ++num >= nonTriggeredPropertiesCount)
			{
				break;
			}
		}
	}

	internal object GetTrigeredValue(int i)
	{
		if (RetriveMode.RetriveDataTable == m_RetriveMode)
		{
			return m_table.Rows[m_nCurentCachePos][i];
		}
		return m_dataReader.GetValue(i);
	}

	internal object GetDataFromStorage(int i)
	{
		if (RetriveMode.RetriveDataTable == m_RetriveMode)
		{
			return m_table.Rows[m_nCurentCachePos][i];
		}
		return rowData[i];
	}

	internal void SetDataInStorage(int i, object data)
	{
		if (RetriveMode.RetriveDataTable == m_RetriveMode)
		{
			m_table.Rows[m_nCurentCachePos][i] = data;
		}
		else
		{
			rowData[i] = data;
		}
	}

	private bool ReadInternal()
	{
		bool flag = m_dataReader.Read();
		if (flag)
		{
			m_dataReader.GetValues(rowData);
		}
		return flag;
	}

	private void ManipulateRowDataPostProcess()
	{
	}

	private void ManipulateRowDataType()
	{
		if (!m_bHasTypeCast)
		{
			return;
		}
		for (int i = 0; i < nonTriggeredPropertiesCount; i++)
		{
			if (!(null != rowDataManipulation[i].targetType) || rowData[i] is DBNull)
			{
				continue;
			}
			if (rowDataManipulation[i].targetType.IsEnum())
			{
				rowData[i] = Enum.ToObject(rowDataManipulation[i].targetType, rowData[i]);
				continue;
			}
			if (rowData[i] is IConvertible convertible && rowDataManipulation[i].targetType.IsPrimitive())
			{
				rowData[i] = convertible.ToType(rowDataManipulation[i].targetType, CultureInfo.CurrentCulture);
				continue;
			}
			Type[] types = new Type[1] { rowDataManipulation[i].sourceType };
			ConstructorInfo constructor = rowDataManipulation[i].targetType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, types, null);
			if (null != constructor)
			{
				rowData[i] = constructor.Invoke(new object[1] { rowData[i] });
			}
			else
			{
				if (!(typeof(string) == rowDataManipulation[i].sourceType))
				{
					continue;
				}
				constructor = rowDataManipulation[i].targetType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[0], null);
				if (!(null != constructor))
				{
					continue;
				}
				object obj = constructor.Invoke(new object[0]);
				if (obj != null)
				{
					MethodInfo method = rowDataManipulation[i].targetType.GetMethod("Parse", new Type[1] { typeof(string) });
					if (null != method)
					{
						method.Invoke(obj, new object[1] { rowData[i] });
						rowData[i] = obj;
					}
				}
			}
		}
	}

	private void ManipulateRowData()
	{
		ManipulateRowDataType();
		ManipulateRowDataPostProcess();
	}

	public void Close()
	{
		m_table = null;
		if (m_dataReader != null && !m_dataReader.IsClosed)
		{
			if (m_command != null)
			{
				m_command.Cancel();
			}
			m_dataReader.Close();
		}
		if (m_execSql != null)
		{
			m_execSql.Disconnect();
			m_execSql = null;
		}
	}

	public DataTable GetSchemaTable()
	{
		DataTable schemaTable = m_dataReader.GetSchemaTable();
		while (nonTriggeredPropertiesCount >= 1 && schemaTable.Rows.Count > nonTriggeredPropertiesCount)
		{
			schemaTable.Rows.RemoveAt(schemaTable.Rows.Count - 1);
		}
		return schemaTable;
	}

	public bool NextResult()
	{
		return false;
	}

	public bool Read()
	{
		bool flag = ReadInternal();
		if (flag)
		{
			ManipulateRowData();
		}
		return flag;
	}

	internal DataTable GetTable()
	{
		int count = m_table.Rows.Count;
		for (m_nCurentCachePos = 0; m_nCurentCachePos < count; m_nCurentCachePos++)
		{
			ManipulateRowDataPostProcess();
		}
		int count2 = m_table.Columns.Count;
		for (int num = count2 - 1; num >= nonTriggeredPropertiesCount; num--)
		{
			m_table.Columns.RemoveAt(num);
		}
		return m_table;
	}

	public void Dispose()
	{
		if (m_table != null)
		{
			m_table.Dispose();
		}
		if (m_command != null)
		{
			m_command.Dispose();
		}
		if (m_dataReader != null)
		{
			m_dataReader.Dispose();
		}
	}

	private int AdjustIndex(int i)
	{
		if (nonTriggeredPropertiesCount > 0 && i > nonTriggeredPropertiesCount)
		{
			i = m_dataReader.FieldCount + 1;
		}
		return i;
	}

	public bool GetBoolean(int i)
	{
		return (bool)this[i];
	}

	public byte GetByte(int i)
	{
		return (byte)this[i];
	}

	public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
	{
		byte[] array = (byte[])this[i];
		int num = ((length <= array.Length) ? length : array.Length);
		if (fieldOffset > int.MaxValue || fieldOffset < int.MinValue)
		{
			throw new ArgumentOutOfRangeException("fieldOffset");
		}
		Array.Copy(buffer, bufferoffset, array, (int)fieldOffset, num);
		return num;
	}

	public char GetChar(int i)
	{
		return (char)this[i];
	}

	public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
	{
		char[] array = (char[])this[i];
		int num = ((length <= array.Length) ? length : array.Length);
		if (fieldoffset > int.MaxValue || fieldoffset < int.MinValue)
		{
			throw new ArgumentOutOfRangeException("fieldOffset");
		}
		Array.Copy(buffer, bufferoffset, array, (int)fieldoffset, num);
		return num;
	}

	public IDataReader GetData(int i)
	{
		return null;
	}

	public string GetDataTypeName(int i)
	{
		i = AdjustIndex(i);
		return m_dataReader.GetDataTypeName(i);
	}

	public DateTime GetDateTime(int i)
	{
		return (DateTime)this[i];
	}

	public decimal GetDecimal(int i)
	{
		return (decimal)this[i];
	}

	public double GetDouble(int i)
	{
		return (double)this[i];
	}

	public Type GetFieldType(int i)
	{
		i = AdjustIndex(i);
		return GetSchemaTable().Columns[i].DataType;
	}

	public float GetFloat(int i)
	{
		return (float)this[i];
	}

	public Guid GetGuid(int i)
	{
		return (Guid)this[i];
	}

	public short GetInt16(int i)
	{
		return (short)this[i];
	}

	public int GetInt32(int i)
	{
		return (int)this[i];
	}

	public long GetInt64(int i)
	{
		return (long)this[i];
	}

	public string GetName(int i)
	{
		i = AdjustIndex(i);
		return m_dataReader.GetName(i);
	}

	public int GetOrdinal(string name)
	{
		return AdjustIndex(m_dataReader.GetOrdinal(name));
	}

	public string GetString(int i)
	{
		return (string)this[i];
	}

	public object GetValue(int i)
	{
		return this[i];
	}

	public int GetValues(object[] values)
	{
		int num = values.Length;
		int num2 = ((nonTriggeredPropertiesCount < num) ? nonTriggeredPropertiesCount : num);
		for (int i = 0; i < num2; i++)
		{
			values[i] = this[i];
		}
		for (int j = num2; j < nonTriggeredPropertiesCount; j++)
		{
			values[j] = null;
		}
		return num2;
	}

	public bool IsDBNull(int i)
	{
		i = AdjustIndex(i);
		return m_dataReader.IsDBNull(i);
	}
}
