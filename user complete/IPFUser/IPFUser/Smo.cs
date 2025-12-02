using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Windows.Forms;
using IPFUser.My;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

public class Smo
{
	private static Server oServer;

	private static List<string> lstServers = new List<string>();

	/// <summary>
	/// Realiza el Attach de una base de datos
	/// </summary>
	/// <param name="DbFile"></param>
	/// <param name="DbLog"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool AttachDb(string DbName, string DbFile, string DbLog)
	{
		bool DbExists = false;
		ServerConnection oConnection = new ServerConnection(MySettingsProperty.Settings.DataServer);
		oServer = new Server(oConnection);
		bool AttachDb;
		try
		{
			foreach (Database Db in oServer.Databases)
			{
				if (Operators.CompareString(Db.Name, DbName, TextCompare: false) == 0)
				{
					DbExists = true;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			MessageBox.Show(AppCSIUser.Rm.GetString("String79") + " " + ex2.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			AttachDb = false;
			ProjectData.ClearProjectError();
			goto IL_027b;
		}
		if (!DbExists)
		{
			if (Operators.CompareString(DbFile, "", TextCompare: false) != 0 && File.Exists(DbFile))
			{
				try
				{
					StringCollection sc = new StringCollection();
					sc.Add(DbFile);
					sc.Add(DbLog);
					oServer.AttachDatabase(DbName, sc);
				}
				catch (Exception ex3)
				{
					ProjectData.SetProjectError(ex3);
					Exception ex4 = ex3;
					ProjectData.ClearProjectError();
				}
			}
			else
			{
				Database Db2 = new Database(oServer, DbName);
				Db2.FileGroups.Add(new FileGroup(Db2, "PRIMARY"));
				DataFile dtPrimary = new DataFile(Db2.FileGroups["PRIMARY"], DbName + "_Data");
				dtPrimary.FileName = Application.StartupPath + "\\Db\\" + DbName + ".mdf";
				Db2.FileGroups["PRIMARY"].Files.Add(dtPrimary);
				dtPrimary.Size = 3072.0;
				dtPrimary.GrowthType = FileGrowthType.KB;
				dtPrimary.Growth = 1024.0;
				LogFile logFile = new LogFile(Db2, DbName + "_Log");
				Db2.LogFiles.Add(logFile);
				logFile.FileName = Application.StartupPath + "\\Db\\" + DbName + "_Log.ldf";
				logFile.Size = 3072.0;
				logFile.GrowthType = FileGrowthType.Percent;
				logFile.Growth = 10.0;
				Db2.Create();
				Db2.Refresh();
				oServer.ConnectionContext.ExecuteNonQuery(GetScript("IPromo.sql"));
			}
		}
		AttachDb = true;
		goto IL_027b;
		IL_027b:
		return AttachDb;
	}

	/// <summary>
	/// Realiza el Detach de una base de datos existente
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool DetachDb(string DbName)
	{
		ServerConnection oConnection = new ServerConnection(MySettingsProperty.Settings.DataServer);
		oServer = new Server(oConnection);
		bool DetachDb = default(bool);
		try
		{
			foreach (Database Db in oServer.Databases)
			{
				if (Operators.CompareString(Db.Name, DbName, TextCompare: false) == 0)
				{
					oServer.KillAllProcesses(DbName);
					oServer.DetachDatabase(DbName, updateStatistics: true);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			MessageBox.Show(AppCSIUser.Rm.GetString("String79") + " " + ex2.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			DetachDb = false;
			ProjectData.ClearProjectError();
		}
		return DetachDb;
	}

	/// <summary>
	/// Crea y enlaza una BD a partir del Script de DDL y el nombre deseado
	/// </summary>
	/// <param name="Script"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool CreateDb(string Script, string Path)
	{
		string Data = GetScript(Script);
		Data = Data.Replace("<PATH>", Path);
		oServer.ConnectionContext.ExecuteNonQuery(Data);
		bool CreateDb = default(bool);
		return CreateDb;
	}

	/// <summary>
	/// Crea y enlaza una BD a partir del Script de DDL y el nombre deseado
	/// </summary>
	/// <param name="Script"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool CreatePromoDb(string Script, string Path, string Name)
	{
		string Data = GetScript(Script);
		Data = Data.Replace("<PATH>", Path);
		Data = Data.Replace("<PROMONAME>", System.IO.Path.GetFileNameWithoutExtension(Name));
		oServer.ConnectionContext.ExecuteNonQuery(Data);
		bool CreatePromoDb = default(bool);
		return CreatePromoDb;
	}

	/// <summary>
	/// Determina si la BD está enlazada
	/// </summary>
	/// <param name="DbName"></param>
	/// <returns></returns>
	/// <remarks></remarks>
	public static bool IsAttached(string DbName)
	{
		ServerConnection oConnection = new ServerConnection(MySettingsProperty.Settings.DataServer);
		oServer = new Server(oConnection);
		try
		{
			foreach (Database Db in oServer.Databases)
			{
				if (Operators.CompareString(Db.Name, DbName, TextCompare: false) == 0)
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			MessageBox.Show(AppCSIUser.Rm.GetString("String79") + " " + ex2.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			ProjectData.ClearProjectError();
		}
		return false;
	}

	public static object ShrinkDb(string DbName)
	{
		if (IsAttached(DbName))
		{
			ServerConnection oConnection = new ServerConnection(MySettingsProperty.Settings.DataServer);
			oServer = new Server(oConnection);
			Database oDb = oServer.Databases[DbName];
			oDb.Shrink(30, ShrinkMethod.NoTruncate);
			return true;
		}
		return false;
	}

	public static object ValidVersion(int Major, int Minor)
	{
		ServerConnection oConnection = new ServerConnection(MySettingsProperty.Settings.DataServer);
		object ValidVersion;
		try
		{
			oServer = new Server(oConnection);
			ValidVersion = ((!((oServer.VersionMajor != Major) | (oServer.VersionMinor != Minor))) ? ((object)true) : ((object)false));
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ValidVersion = false;
			ProjectData.ClearProjectError();
		}
		return ValidVersion;
	}

	private static string GetScript(string ScriptName)
	{
		StreamReader Sr = new StreamReader(Application.StartupPath + "\\BlankDb\\" + ScriptName);
		string Data = Sr.ReadToEnd();
		Sr.Close();
		return Data;
	}

	/// <summary>
	/// Determina el conjunt de servidors Locals!!
	/// </summary>
	/// <returns></returns>
	/// <remarks></remarks>
	private static int GetServers()
	{
		DataTable dt = SmoApplication.EnumAvailableSqlServers(localOnly: true);
		if (dt.Rows.Count > 0)
		{
			foreach (DataRow dr in dt.Rows)
			{
				string servername = Conversions.ToString(dr["Name"]);
				if (!lstServers.Contains(servername))
				{
					lstServers.Add(servername);
				}
			}
		}
		return lstServers.Count;
	}
}
