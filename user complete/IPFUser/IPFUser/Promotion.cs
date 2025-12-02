using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

public class Promotion
{
	public struct Splitting
	{
		public int Split1;

		public int Split2;

		public int Split3;

		public int Split4;
	}

	public int Id;

	public DataTable FilesImported;

	public DataTable Artworks;

	private DataTable Data;

	/// <summary>
	/// Devuelve los códigos totales de los que dispone la promoción
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public object TotalCodes
	{
		get
		{
			checked
			{
				int num = FilesImported.Rows.Count - 1;
				int Codes = default(int);
				for (int i = 0; i <= num; i++)
				{
					Codes = Conversions.ToInteger(Operators.AddObject(Codes, FilesImported.Rows[i]["TotalCodes"]));
				}
				return Codes;
			}
		}
	}

	/// <summary>
	/// Devuelve los códigos consumidos por la promoción
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public object ConsumedCodes
	{
		get
		{
			checked
			{
				int num = FilesImported.Rows.Count - 1;
				int Codes = default(int);
				for (int i = 0; i <= num; i++)
				{
					Codes = Conversions.ToInteger(Operators.AddObject(Codes, FilesImported.Rows[i]["Consumed"]));
				}
				return Codes;
			}
		}
	}

	public int RecordLength
	{
		get
		{
			if (Data.Rows[0]["RecordLength"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["RecordLength"]);
			}
			return 0;
		}
		set
		{
			Data.Rows[0]["RecordLength"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int Split1
	{
		get
		{
			if (Data.Rows[0]["Split1"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["Split1"]);
			}
			return 0;
		}
		set
		{
			Data.Rows[0]["Split1"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int Split2
	{
		get
		{
			if (Data.Rows[0]["Split2"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["Split2"]);
			}
			return 0;
		}
		set
		{
			Data.Rows[0]["Split2"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int Split3
	{
		get
		{
			if (Data.Rows[0]["Split3"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["Split3"]);
			}
			return 0;
		}
		set
		{
			Data.Rows[0]["Split3"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int Split4
	{
		get
		{
			if (Data.Rows[0]["Split4"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["Split4"]);
			}
			return 0;
		}
		set
		{
			Data.Rows[0]["Split4"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public string Name => Conversions.ToString(Data.Rows[0]["JobName"]);

	public string LaserFile
	{
		get
		{
			if (Data.Rows[0]["LaserFile"] != DBNull.Value)
			{
				return Conversions.ToString(Data.Rows[0]["LaserFile"]);
			}
			return "";
		}
		set
		{
			Data.Rows[0]["LaserFile"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public string GetSqlCodes
	{
		get
		{
			string Sql = "Select TOP " + Records + " * from Codes where Sent=0 and (";
			checked
			{
				int num = FilesImported.Rows.Count - 1;
				for (int i = 0; i <= num; i++)
				{
					Sql = ((i >= FilesImported.Rows.Count - 1) ? (Sql + string.Format("(Id >= {0} and Id <= {1})", RuntimeHelpers.GetObjectValue(FilesImported.Rows[i]["FromRecord"]), RuntimeHelpers.GetObjectValue(FilesImported.Rows[i]["ToRecord"]))) : (Sql + string.Format("(Id >= {0} and Id <= {1}) or ", RuntimeHelpers.GetObjectValue(FilesImported.Rows[i]["FromRecord"]), RuntimeHelpers.GetObjectValue(FilesImported.Rows[i]["ToRecord"]))));
				}
				return Sql + ")";
			}
		}
	}

	public object UserFields
	{
		get
		{
			checked
			{
				int UFields = default(int);
				if (Operators.ConditionalCompareObjectGreater(Data.Rows[0]["split1"], 0, TextCompare: false))
				{
					UFields++;
				}
				if (Operators.ConditionalCompareObjectGreater(Data.Rows[0]["split2"], 0, TextCompare: false))
				{
					UFields++;
				}
				if (Operators.ConditionalCompareObjectGreater(Data.Rows[0]["split3"], 0, TextCompare: false))
				{
					UFields++;
				}
				if (Operators.ConditionalCompareObjectGreater(Data.Rows[0]["split4"], 0, TextCompare: false))
				{
					UFields++;
				}
				return UFields;
			}
		}
	}

	public string CodesDb
	{
		get
		{
			if (Data.Rows[0]["CodesDb"] != DBNull.Value)
			{
				return Path.GetFileNameWithoutExtension(Conversions.ToString(Data.Rows[0]["CodesDb"]));
			}
			return "";
		}
		set
		{
			Data.Rows[0]["CodesDb"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int DatamatrixType
	{
		get
		{
			if (Data.Rows[0]["Datamatrix"] != DBNull.Value)
			{
				return Conversions.ToInteger(Data.Rows[0]["Datamatrix"]);
			}
			return -1;
		}
		set
		{
			Data.Rows[0]["Datamatrix"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public bool CodesDbAttached
	{
		get
		{
			if (Data.Rows[0]["IsAttached"] != DBNull.Value)
			{
				return Conversions.ToBoolean(Data.Rows[0]["IsAttached"]);
			}
			return false;
		}
		set
		{
			Data.Rows[0]["IsAttached"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	/// <summary>
	/// Capas
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public int Layers
	{
		get
		{
			return Conversions.ToInteger(Data.Rows[0]["Layers"]);
		}
		set
		{
			Data.Rows[0]["Layers"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	/// <summary>
	/// Elementos del Ciclo
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public int CycleElements
	{
		get
		{
			return Conversions.ToInteger(Data.Rows[0]["CycleElements"]);
		}
		set
		{
			Data.Rows[0]["CycleElements"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	/// <summary>
	/// Absoluta o relativa
	/// </summary>
	/// <value></value>
	/// <returns></returns>
	/// <remarks></remarks>
	public bool IsAbsolute
	{
		get
		{
			return Conversions.ToBoolean(Data.Rows[0]["IsAbsolute"]);
		}
		set
		{
			Data.Rows[0]["IsAbsolute"] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int LayerQty
	{
		get
		{
			return Conversions.ToInteger(Data.Rows[0]["LayerQty" + checked(IdLayer + 1)]);
		}
		set
		{
			Data.Rows[0]["LayerQty" + checked(IdLayer + 1)] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public int LayerUseCodes
	{
		get
		{
			return Conversions.ToInteger(Data.Rows[0]["LayerUseCodes" + checked(IdLayer + 1)]);
		}
		set
		{
			Data.Rows[0]["LayerUseCodes" + checked(IdLayer + 1)] = value;
			AppCSIUser.Db.UpdateTable(Data);
		}
	}

	public void Load(string Name)
	{
		Data = AppCSIUser.Db.GetSqlTable("Select * from Jobs where Jobname='" + Name + "'", "Jobs");
		Id = Conversions.ToInteger(Data.Rows[0]["JobId"]);
		FilesImported = AppCSIUser.Db.GetSqlTable("Select * from CodesIndex where IdJob=" + Id, "CodesIndex");
		Artworks = AppCSIUser.Db.GetSqlTable("Select * from Artworks where IdJob=" + Id, "Artworks");
		Artworks.PrimaryKey = new DataColumn[2]
		{
			Artworks.Columns[0],
			Artworks.Columns[1]
		};
	}
}
