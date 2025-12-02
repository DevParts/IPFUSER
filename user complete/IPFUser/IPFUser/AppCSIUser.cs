using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows.Forms;
using DataBases.SqlServer;
using IPFUser.My;
using MacsaDevicesNet;
using Microsoft.VisualBasic.CompilerServices;

namespace IPFUser;

[StandardModule]
internal sealed class AppCSIUser
{
	public const string VERSION = "3.0.0.2";

	public static SQLSeverDBMS Db = new SQLSeverDBMS();

	public static SQLSeverDBMS DbCodes = new SQLSeverDBMS();

	public static string DbPath;

	public static bool DbConnected = false;

	public static bool DbConfigured = false;

	public static int PossibleArtwork;

	public static Promotion Promo;

	public static string Pedido;

	public static string Artwork;

	public static string Drive;

	public static ResourceManager Rm;

	public static string LastJobId;

	public const int QUEUE_SIZE = 50;

	[field: AccessedThroughProperty("oPLC")]
	public static PLC oPLC
	{
		get; [MethodImpl(MethodImplOptions.Synchronized)]
		set;
	} = new PLC();

	public static void ConfigureDataBase()
	{
		if (!DbConfigured)
		{
			DbConfigured = true;
			Db.CreateAdapter("Jobs");
			Db.CreateAdapter("CodesIndex");
			Db.CreateAdapter("Artworks");
			Db.CreateAdapter("Historico");
		}
	}

	public static void GenerateHistoric(int Init, int Final)
	{
		if (Final == 0 || Init == 0)
		{
			return;
		}
		bool WritePLCData = true;
		checked
		{
			int num = Promo.FilesImported.Rows.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!Operators.ConditionalCompareObjectGreaterEqual(Init, Promo.FilesImported.Rows[i]["FromRecord"], TextCompare: false) || Operators.ConditionalCompareObjectGreaterEqual(Init, Promo.FilesImported.Rows[i]["ToRecord"], TextCompare: false))
				{
					continue;
				}
				if (Operators.ConditionalCompareObjectLessEqual(Final, Promo.FilesImported.Rows[i]["ToRecord"], TextCompare: false))
				{
					AddHistorico(Conversions.ToString(Promo.FilesImported.Rows[i]["FileName"]), Conversions.ToInteger(Operators.AddObject(Operators.SubtractObject(Init, Promo.FilesImported.Rows[i]["FromRecord"]), 1)), Conversions.ToInteger(Operators.AddObject(Operators.SubtractObject(Final, Promo.FilesImported.Rows[i]["FromRecord"]), 1)), WritePLCData);
					WritePLCData = false;
					continue;
				}
				AddHistorico(Conversions.ToString(Promo.FilesImported.Rows[i]["FileName"]), Conversions.ToInteger(Operators.AddObject(Operators.SubtractObject(Init, Promo.FilesImported.Rows[i]["FromRecord"]), 1)), Conversions.ToInteger(Operators.AddObject(Operators.SubtractObject(Promo.FilesImported.Rows[i]["toRecord"], Promo.FilesImported.Rows[i]["FromRecord"]), 1)), WritePLCData);
				WritePLCData = false;
				if (i < Promo.FilesImported.Rows.Count - 1)
				{
					Init = Conversions.ToInteger(Promo.FilesImported.Rows[i + 1]["FromRecord"]);
				}
			}
		}
	}

	public static void AddHistorico(string Fichero, int Desde, int Hasta, bool WritePLCData)
	{
		DataTable Tb = Db.GetSqlTable("Select * from Historico", "Historico");
		DataRow Row = Tb.NewRow();
		Row["Pedido"] = Pedido;
		Row["Artwork"] = Artwork;
		Row["Fichero"] = Fichero;
		Row["Desde"] = Desde;
		Row["Hasta"] = Hasta;
		Row["Volumen"] = new DriveInfo(Drive).VolumeLabel;
		Row["Timestamp"] = DateTime.Now;
		Row["Sesion"] = MySettingsProperty.Settings.Session;
		Row["Layers"] = Promo.Layers;
		int Ordinal = Tb.Columns["LayerQty1"].Ordinal;
		checked
		{
			if (unchecked(Promo.Layers > 1 && WritePLCData))
			{
				int i = 0;
				do
				{
					Row[Ordinal + i] = oPLC.RData.PrintedLayersQty[i];
					i++;
				}
				while (i <= 24);
			}
			else
			{
				int i2 = 0;
				do
				{
					Row[Ordinal + i2] = 0;
					i2++;
				}
				while (i2 <= 24);
			}
			Tb.Rows.Add(Row);
			Db.UpdateTable(Tb);
		}
	}

	public static bool DbAttached(string DbName)
	{
		DataTable Tb = Db.GetListAttachedDb();
		foreach (DataRow row in Tb.Rows)
		{
			if (Operators.ConditionalCompareObjectEqual(row["Name"], Path.GetFileNameWithoutExtension(DbName), TextCompare: false))
			{
				return true;
			}
		}
		return false;
	}

	public static void LoadCodes()
	{
		if (!Smo.IsAttached(Promo.CodesDb))
		{
			try
			{
				Common.MACSALog("Attach Db " + Promo.CodesDb, TraceEventType.Information);
				Smo.AttachDb(Promo.CodesDb, DbPath + "\\" + Promo.CodesDb + ".mdf", DbPath + "\\" + Promo.CodesDb + "_log.ldf");
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog("ERROR Attach Db " + Promo.CodesDb + " " + ex2.Message, TraceEventType.Error);
				ProjectData.ClearProjectError();
			}
		}
		if (DbCodes != null)
		{
			DbCodes.CloseConnection();
		}
		DbCodes = new SQLSeverDBMS();
		DbCodes.DbName = Promo.CodesDb;
		DbCodes.DataSource = MySettingsProperty.Settings.DataServer;
		DbCodes.UseWindowsAuthentication = MySettingsProperty.Settings.UseWindowsAuthentication;
		DbCodes.User = MySettingsProperty.Settings.SqlUser;
		DbCodes.Password = MySettingsProperty.Settings.SqlPassword;
		try
		{
			DbCodes.CreateAdapter("Codes");
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			Common.MACSALog("Reintenta al crear el adaptados de CODES: " + ex4.Message, TraceEventType.Information);
			ProjectData.ClearProjectError();
		}
	}

	public static void InitCulture()
	{
		Rm = new ResourceManager("IPFUser.Idiomas", Assembly.GetExecutingAssembly());
		if (Operators.CompareString(MySettingsProperty.Settings.Language, "English", TextCompare: false) == 0)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			MyProject.Application.ChangeCulture("en-US");
			Common.Language = "EN";
		}
		else
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
			MyProject.Application.ChangeCulture("es-ES");
			Common.Language = "ES";
		}
	}

	public static void AddDirectorySecurity(string FolderName, string Account, FileSystemRights Rights, AccessControlType controlType)
	{
		checked
		{
			try
			{
				Common.MACSALog("Otorga permisos " + Rights.ToString() + " a carpeta " + FolderName + " al grupo " + Account, TraceEventType.Information);
				DirectoryInfo dir = new DirectoryInfo(FolderName);
				DirectorySecurity sec = dir.GetAccessControl();
				sec.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, controlType));
				sec.AddAccessRule(new FileSystemAccessRule(Account, Rights, controlType));
				dir.SetAccessControl(sec);
				string[] Files = Directory.GetFiles(FolderName);
				int num = Files.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if ((Operators.CompareString(Path.GetExtension(Files[i]).ToLower(), ".mdf", TextCompare: false) == 0) | (Operators.CompareString(Path.GetExtension(Files[i]).ToLower(), ".ldf", TextCompare: false) == 0))
					{
						FileInfo oFile = new FileInfo(Files[i]);
						FileSecurity fSec = oFile.GetAccessControl();
						fSec.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, controlType));
						fSec.AddAccessRule(new FileSystemAccessRule(Account, Rights, controlType));
						oFile.SetAccessControl(fSec);
					}
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog("Error en setting de permisos para el usuario " + Account, TraceEventType.Error);
				MessageBox.Show(ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}
}
