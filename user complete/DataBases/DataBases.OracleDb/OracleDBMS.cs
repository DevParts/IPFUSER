using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using Microsoft.VisualBasic.CompilerServices;
using Oracle.DataAccess.Client;

namespace DataBases.OracleDb;

public class OracleDBMS
{
	public enum ACCESS_TABLE_TYPE
	{
		READ_ONLY = 1,
		ADD_ONLY = 2,
		READ_WRITE = 4
	}

	private enum SQL_COMMAND_TYPE
	{
		SQL_SELECT,
		SQL_INSERT,
		SQL_UPDATE,
		SQL_DELETE
	}

	public enum DB_ACCESS_MODES
	{
		READ_WRITE,
		READ_ONLY
	}

	private OracleConnection _Cn;

	private string _ConnectionName;

	private string _DataSource;

	private string _User;

	private string _Password;

	private Dictionary<string, OracleDataAdapter> _DataAdapters;

	private bool _PopUpErrors;

	private bool _UseWindowsAuthentication;

	private int _ConnetionTimeOut;

	public string ConnectionName
	{
		get
		{
			return _ConnectionName;
		}
		set
		{
			_ConnectionName = value;
		}
	}

	public string User
	{
		get
		{
			return _User;
		}
		set
		{
			_User = value;
		}
	}

	public string Password
	{
		get
		{
			return _Password;
		}
		set
		{
			_Password = value;
		}
	}

	public int ConnectionTimeOut
	{
		get
		{
			return _ConnetionTimeOut;
		}
		set
		{
			_ConnetionTimeOut = value;
		}
	}

	public OracleConnection GetConnection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			OracleConnection val = new OracleConnection();
			val.ConnectionString = "DATA SOURCE=" + ConnectionName + ";USER ID=" + User + ";PASSWORD=" + Password + ";";
			OracleConnection val2 = val;
			val2.ConnectionString = val2.ConnectionString + ";Connect Timeout=" + ConnectionTimeOut;
			return val;
		}
	}

	public bool PopupErrors
	{
		get
		{
			return _PopUpErrors;
		}
		set
		{
			_PopUpErrors = value;
		}
	}

	public OracleDBMS()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		_Cn = new OracleConnection();
		_DataAdapters = new Dictionary<string, OracleDataAdapter>();
		_ConnetionTimeOut = 15;
	}

	public void BulkTable(DataTable SrcTable, string TableName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		OracleBulkCopy val = new OracleBulkCopy(GetConnection.ConnectionString, (OracleBulkCopyOptions)0);
		val.DestinationTableName = TableName;
		val.WriteToServer(SrcTable);
	}

	public OracleDataReader GetDataReader(string Sql)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		OracleCommand val = new OracleCommand();
		val.Connection = GetConnection;
		val.CommandText = Sql;
		val.Connection.Open();
		OracleDataReader result;
		try
		{
			OracleDataReader val2 = val.ExecuteReader(CommandBehavior.CloseConnection);
			result = val2;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public int DoSQLAction(string Sql, int TimeOut = 30)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		OracleCommand val = new OracleCommand();
		val.Connection = GetConnection;
		val.CommandText = Sql;
		val.CommandTimeout = TimeOut;
		val.Connection.Open();
		int result;
		try
		{
			int num = val.ExecuteNonQuery();
			val.Connection.Close();
			result = num;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			val.Connection.Close();
			result = 0;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetTable(string TableName, ACCESS_TABLE_TYPE Mode)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		DataTable dataTable = new DataTable();
		OracleDataReader reader = ((Mode != ACCESS_TABLE_TYPE.ADD_ONLY) ? GetDataReader("Select * from " + TableName) : GetDataReader("Select * from " + TableName + " where rowindex<2"));
		dataTable.Load((IDataReader)reader);
		dataTable.TableName = TableName;
		if (Mode == ACCESS_TABLE_TYPE.READ_WRITE || Mode == ACCESS_TABLE_TYPE.ADD_ONLY)
		{
			try
			{
				OracleDataAdapter val = _DataAdapters[TableName];
			}
			catch (KeyNotFoundException ex)
			{
				ProjectData.SetProjectError(ex);
				KeyNotFoundException ex2 = ex;
				DataTable schemaTable = GetSchemaTable(TableName);
				OracleDataAdapter val = new OracleDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
				val.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
				val.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
				val.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
				_DataAdapters.Add(TableName, val);
				ProjectData.ClearProjectError();
			}
		}
		return dataTable;
	}

	public DataTable UpdateTable(DataTable Table)
	{
		DataTable result;
		try
		{
			OracleDataAdapter val = _DataAdapters[Table.TableName];
			DataTable changes = Table.GetChanges();
			if (changes != null)
			{
				((DbDataAdapter)(object)val).Update(changes);
			}
			Table.AcceptChanges();
			result = Table;
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	public DataTable GetSqlTable(string SqlText, string TableName)
	{
		OracleDataReader reader = null;
		DataTable dataTable = new DataTable();
		try
		{
			reader = GetDataReader(SqlText);
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
		dataTable.Load((IDataReader)reader);
		dataTable.TableName = TableName;
		return dataTable;
	}

	public OracleDataAdapter CreateAdapter(string TableName)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		DataTable dataTable = new DataTable();
		OracleDataReader dataReader = GetDataReader("Select * from " + TableName + " where rownum<2");
		dataTable.Load((IDataReader)dataReader);
		dataTable.TableName = TableName;
		dataReader.Close();
		OracleDataAdapter val;
		try
		{
			val = _DataAdapters[TableName];
		}
		catch (KeyNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			KeyNotFoundException ex2 = ex;
			DataTable schemaTable = GetSchemaTable(TableName);
			val = new OracleDataAdapter("Select * from " + TableName, _Cn.ConnectionString);
			val.InsertCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_INSERT, schemaTable);
			val.UpdateCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_UPDATE, schemaTable);
			val.DeleteCommand = DoSqlCommand(dataTable, SQL_COMMAND_TYPE.SQL_DELETE, schemaTable);
			_DataAdapters.Add(TableName, val);
			ProjectData.ClearProjectError();
		}
		return val;
	}

	public void CloseConnection()
	{
		foreach (KeyValuePair<string, OracleDataAdapter> dataAdapter in _DataAdapters)
		{
			((Component)(object)dataAdapter.Value).Dispose();
		}
	}

	private DataTable GetSchemaTable(string TableName)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		OracleCommand val = new OracleCommand();
		val.Connection = GetConnection;
		val.CommandText = "Select * from " + TableName + " where rownum<2";
		val.Connection.Open();
		DataTable result;
		try
		{
			OracleDataReader val2 = val.ExecuteReader(CommandBehavior.KeyInfo);
			DataTable schemaTable = val2.GetSchemaTable();
			val.Connection.Close();
			result = schemaTable;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			result = null;
			ProjectData.ClearProjectError();
		}
		return result;
	}

	private OracleCommand DoSqlCommand(DataTable Table, SQL_COMMAND_TYPE SqlType, DataTable StructTable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Expected O, but got Unknown
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Expected O, but got Unknown
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Expected O, but got Unknown
		OracleCommand val = new OracleCommand();
		val.CommandType = CommandType.Text;
		val.Connection = GetConnection;
		bool flag = true;
		checked
		{
			switch (unchecked((int)SqlType))
			{
			case 0:
				val.CommandText = "Select * from " + Table.TableName;
				break;
			case 1:
			{
				val.CommandText = "Insert into " + Table.TableName + "(";
				int num5 = Table.Columns.Count - 1;
				int num2 = 0;
				OracleCommand val2;
				while (true)
				{
					int num6 = num2;
					int num4 = num5;
					if (num6 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["IsRowId"]))
					{
						if (flag)
						{
							flag = false;
							val2 = val;
							val2.CommandText += Table.Columns[num2].ColumnName;
						}
						else
						{
							val2 = val;
							val2.CommandText = val2.CommandText + "," + Table.Columns[num2].ColumnName;
						}
						OracleParameter val4 = new OracleParameter();
						val4.ParameterName = "p" + Table.Columns[num2].ColumnName;
						val4.SourceColumn = Table.Columns[num2].ColumnName;
						val.Parameters.Add(val4);
					}
					num2++;
				}
				val2 = val;
				val2.CommandText += ")values(";
				flag = true;
				int num7 = Table.Columns.Count - 1;
				num2 = 0;
				while (true)
				{
					int num8 = num2;
					int num4 = num7;
					if (num8 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["IsRowId"]))
					{
						if (flag)
						{
							flag = false;
							val2 = val;
							val2.CommandText = val2.CommandText + ":p" + Table.Columns[num2].ColumnName;
						}
						else
						{
							val2 = val;
							val2.CommandText = val2.CommandText + ",:p" + Table.Columns[num2].ColumnName;
						}
					}
					num2++;
				}
				val2 = val;
				val2.CommandText += ")";
				break;
			}
			case 2:
			{
				val.CommandText = "Update " + Table.TableName + " Set ";
				int num9 = Table.Columns.Count - 1;
				int num2 = 0;
				OracleCommand val2;
				while (true)
				{
					int num10 = num2;
					int num4 = num9;
					if (num10 > num4)
					{
						break;
					}
					if (!Conversions.ToBoolean(StructTable.Rows[num2]["Iskey"]) & !Conversions.ToBoolean(StructTable.Rows[num2]["IsRowId"]))
					{
						if (flag)
						{
							flag = false;
							val2 = val;
							val2.CommandText = val2.CommandText + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						else
						{
							val2 = val;
							val2.CommandText = val2.CommandText + "," + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						OracleParameter val5 = new OracleParameter();
						val5.ParameterName = "p" + Table.Columns[num2].ColumnName;
						val5.SourceColumn = Table.Columns[num2].ColumnName;
						val.Parameters.Add(val5);
					}
					num2++;
				}
				val2 = val;
				val2.CommandText += " where ";
				flag = true;
				int num11 = Table.Columns.Count - 1;
				num2 = 0;
				while (true)
				{
					int num12 = num2;
					int num4 = num11;
					if (num12 > num4)
					{
						break;
					}
					if (Conversions.ToBoolean(StructTable.Rows[num2]["IsKey"]))
					{
						if (flag)
						{
							flag = false;
							val2 = val;
							val2.CommandText = val2.CommandText + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						else
						{
							val2 = val;
							val2.CommandText = val2.CommandText + " and " + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						OracleParameter val6 = new OracleParameter();
						val6.ParameterName = "p" + Table.Columns[num2].ColumnName;
						val6.SourceColumn = Table.Columns[num2].ColumnName;
						val.Parameters.Add(val6);
					}
					num2++;
				}
				break;
			}
			case 3:
			{
				val.CommandText = "Delete from " + Table.TableName + " where ";
				int num = Table.Columns.Count - 1;
				int num2 = 0;
				while (true)
				{
					int num3 = num2;
					int num4 = num;
					if (num3 > num4)
					{
						break;
					}
					if (Conversions.ToBoolean(StructTable.Rows[num2]["IsKey"]))
					{
						if (flag)
						{
							flag = false;
							OracleCommand val2 = val;
							val2.CommandText = val2.CommandText + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						else
						{
							OracleCommand val2 = val;
							val2.CommandText = val2.CommandText + " and " + Table.Columns[num2].ColumnName + "=:p" + Table.Columns[num2].ColumnName;
						}
						OracleParameter val3 = new OracleParameter();
						val3.ParameterName = "p" + Table.Columns[num2].ColumnName;
						val3.SourceColumn = Table.Columns[num2].ColumnName;
						val.Parameters.Add(val3);
					}
					num2++;
				}
				break;
			}
			}
			return val;
		}
	}
}
