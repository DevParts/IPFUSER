using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace IPFUser.My.Resources;

/// <summary>
///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
/// </summary>
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class Idiomas
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	/// <summary>
	///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(resourceMan, null))
			{
				ResourceManager temp = new ResourceManager("IPFUser.Idiomas", typeof(Idiomas).Assembly);
				resourceMan = temp;
			}
			return resourceMan;
		}
	}

	/// <summary>
	///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
	///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	/// <summary>
	///   Busca una cadena traducida similar a Laser is Off.
	/// </summary>
	internal static string String1 => ResourceManager.GetString("String1", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Program check not correct.
	/// </summary>
	internal static string String10 => ResourceManager.GetString("String10", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Off-Line.
	/// </summary>
	internal static string String100 => ResourceManager.GetString("String100", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a PLC is not ready. Can't start promotion control process..
	/// </summary>
	internal static string String101 => ResourceManager.GetString("String101", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a PLC General Error.
	/// </summary>
	internal static string String102 => ResourceManager.GetString("String102", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a figure-types not valid in file.
	/// </summary>
	internal static string String11 => ResourceManager.GetString("String11", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Not available memory.
	/// </summary>
	internal static string String12 => ResourceManager.GetString("String12", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a File does not exist.
	/// </summary>
	internal static string String13 => ResourceManager.GetString("String13", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Water refrigeration failure.
	/// </summary>
	internal static string String14 => ResourceManager.GetString("String14", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Interlock.
	/// </summary>
	internal static string String15 => ResourceManager.GetString("String15", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser Failure.
	/// </summary>
	internal static string String16 => ResourceManager.GetString("String16", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Invalid Source.
	/// </summary>
	internal static string String17 => ResourceManager.GetString("String17", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Overheat.
	/// </summary>
	internal static string String18 => ResourceManager.GetString("String18", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser Not ready.
	/// </summary>
	internal static string String19 => ResourceManager.GetString("String19", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Shutter in closed.
	/// </summary>
	internal static string String2 => ResourceManager.GetString("String2", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Overspeed.
	/// </summary>
	internal static string String20 => ResourceManager.GetString("String20", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a File without printing permission.
	/// </summary>
	internal static string String21 => ResourceManager.GetString("String21", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Barcode creation failure.
	/// </summary>
	internal static string String22 => ResourceManager.GetString("String22", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a No license for barcode.
	/// </summary>
	internal static string String23 => ResourceManager.GetString("String23", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a No Barcode Library.
	/// </summary>
	internal static string String24 => ResourceManager.GetString("String24", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Trigger Signal.
	/// </summary>
	internal static string String25 => ResourceManager.GetString("String25", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Database Alarm.
	/// </summary>
	internal static string String26 => ResourceManager.GetString("String26", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Distance Alarm.
	/// </summary>
	internal static string String27 => ResourceManager.GetString("String27", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Min Distance Alarm.
	/// </summary>
	internal static string String28 => ResourceManager.GetString("String28", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Scanner X Failure.
	/// </summary>
	internal static string String29 => ResourceManager.GetString("String29", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a DC Power Failure.
	/// </summary>
	internal static string String3 => ResourceManager.GetString("String3", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Scanner Y Failure.
	/// </summary>
	internal static string String30 => ResourceManager.GetString("String30", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Empty message.
	/// </summary>
	internal static string String31 => ResourceManager.GetString("String31", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Initialization.
	/// </summary>
	internal static string String32 => ResourceManager.GetString("String32", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser Error:.
	/// </summary>
	internal static string String33 => ResourceManager.GetString("String33", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must indicate SQLServer catalog.
	/// </summary>
	internal static string String34 => ResourceManager.GetString("String34", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must restart application to apply changes.
	/// </summary>
	internal static string String35 => ResourceManager.GetString("String35", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must indicate SQLServer datasource.
	/// </summary>
	internal static string String36 => ResourceManager.GetString("String36", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Execution copy is yet running.
	/// </summary>
	internal static string String37 => ResourceManager.GetString("String37", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Invalid Key. Application Aborted.
	/// </summary>
	internal static string String38 => ResourceManager.GetString("String38", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Starting System....
	/// </summary>
	internal static string String39 => ResourceManager.GetString("String39", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Amplifier overtemperature.
	/// </summary>
	internal static string String4 => ResourceManager.GetString("String4", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Data Access....
	/// </summary>
	internal static string String40 => ResourceManager.GetString("String40", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Restoring State....
	/// </summary>
	internal static string String41 => ResourceManager.GetString("String41", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Codes detected.
	/// </summary>
	internal static string String42 => ResourceManager.GetString("String42", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a No database found.
	/// </summary>
	internal static string String43 => ResourceManager.GetString("String43", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Be sure than HD is present and restart application.
	/// </summary>
	internal static string String44 => ResourceManager.GetString("String44", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Stopper number must be greater than 0.
	/// </summary>
	internal static string String45 => ResourceManager.GetString("String45", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Not enogh available codes. You can indicate as maximum.
	/// </summary>
	internal static string String46 => ResourceManager.GetString("String46", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must indicate an order of 11 digits.
	/// </summary>
	internal static string String47 => ResourceManager.GetString("String47", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Continue.
	/// </summary>
	internal static string String48 => ResourceManager.GetString("String48", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser Error.
	/// </summary>
	internal static string String49 => ResourceManager.GetString("String49", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Q-switch overheat.
	/// </summary>
	internal static string String5 => ResourceManager.GetString("String5", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Communication Failure.
	/// </summary>
	internal static string String50 => ResourceManager.GetString("String50", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Continue with other promotion.
	/// </summary>
	internal static string String51 => ResourceManager.GetString("String51", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Production order is completed.
	/// </summary>
	internal static string String52 => ResourceManager.GetString("String52", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Start.
	/// </summary>
	internal static string String53 => ResourceManager.GetString("String53", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Remains.
	/// </summary>
	internal static string String54 => ResourceManager.GetString("String54", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a of codes.
	/// </summary>
	internal static string String55 => ResourceManager.GetString("String55", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Invalid Password.
	/// </summary>
	internal static string String56 => ResourceManager.GetString("String56", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must indicate an artwork number.
	/// </summary>
	internal static string String57 => ResourceManager.GetString("String57", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a The Artwork number.
	/// </summary>
	internal static string String58 => ResourceManager.GetString("String58", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a is not created. Tell to supervisor.
	/// </summary>
	internal static string String59 => ResourceManager.GetString("String59", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a High reverse power.
	/// </summary>
	internal static string String6 => ResourceManager.GetString("String6", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Bad artwork number. Try again..
	/// </summary>
	internal static string String60 => ResourceManager.GetString("String60", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Artwork Number to Activate.
	/// </summary>
	internal static string String61 => ResourceManager.GetString("String61", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Repeat Artwork Number.
	/// </summary>
	internal static string String62 => ResourceManager.GetString("String62", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Are you sure you want to exit from application.
	/// </summary>
	internal static string String63 => ResourceManager.GetString("String63", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Application Version.
	/// </summary>
	internal static string String64 => ResourceManager.GetString("String64", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a  Global.
	/// </summary>
	internal static string String65 => ResourceManager.GetString("String65", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Database.
	/// </summary>
	internal static string String66 => ResourceManager.GetString("String66", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a ConnectionString.
	/// </summary>
	internal static string String67 => ResourceManager.GetString("String67", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a SqlServer Catalog Name.
	/// </summary>
	internal static string String68 => ResourceManager.GetString("String68", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a SqlServer Dataserver.
	/// </summary>
	internal static string String69 => ResourceManager.GetString("String69", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a low forward power.
	/// </summary>
	internal static string String7 => ResourceManager.GetString("String7", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a SqlServer User Name.
	/// </summary>
	internal static string String70 => ResourceManager.GetString("String70", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a SqlServer Password.
	/// </summary>
	internal static string String71 => ResourceManager.GetString("String71", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Use Windows Authentication.
	/// </summary>
	internal static string String72 => ResourceManager.GetString("String72", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Timing.
	/// </summary>
	internal static string String73 => ResourceManager.GetString("String73", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Wait time between sent blocks.
	/// </summary>
	internal static string String74 => ResourceManager.GetString("String74", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Wait time on full queue.
	/// </summary>
	internal static string String75 => ResourceManager.GetString("String75", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser.
	/// </summary>
	internal static string String76 => ResourceManager.GetString("String76", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser IP.
	/// </summary>
	internal static string String77 => ResourceManager.GetString("String77", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Laser queue memory size. Maximum number in memory.
	/// </summary>
	internal static string String78 => ResourceManager.GetString("String78", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Codes.
	/// </summary>
	internal static string String79 => ResourceManager.GetString("String79", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a YAG RS232 error.
	/// </summary>
	internal static string String8 => ResourceManager.GetString("String8", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Percent to show  first warning on low codes level .
	/// </summary>
	internal static string String80 => ResourceManager.GetString("String80", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Percent to show  second warning on low codes level.
	/// </summary>
	internal static string String81 => ResourceManager.GetString("String81", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Show  first warning on low codes level.
	/// </summary>
	internal static string String82 => ResourceManager.GetString("String82", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Show  second warning on low codes level.
	/// </summary>
	internal static string String83 => ResourceManager.GetString("String83", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Set the application language.
	/// </summary>
	internal static string String84 => ResourceManager.GetString("String84", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a You must restart application to apply changes.
	/// </summary>
	internal static string String85 => ResourceManager.GetString("String85", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a The selected promotion is bad configured. Execution no possible. Tell to administrator..
	/// </summary>
	internal static string String86 => ResourceManager.GetString("String86", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Windows User Group to set folder permissions (default may be 'Everyone').
	/// </summary>
	internal static string String87 => ResourceManager.GetString("String87", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Fatal Error: Can't attach DB as Read_Write. Application will be exited. Restart again. If problem persists call administrator..
	/// </summary>
	internal static string String88 => ResourceManager.GetString("String88", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Dongle license key is missing. System stopped..
	/// </summary>
	internal static string String89 => ResourceManager.GetString("String89", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Conveyor Stopped.
	/// </summary>
	internal static string String9 => ResourceManager.GetString("String9", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Selected Data Server is not SQLServer 2008 R2 Version. Select the correct one..
	/// </summary>
	internal static string String90 => ResourceManager.GetString("String90", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a IPAddress.
	/// </summary>
	internal static string String91 => ResourceManager.GetString("String91", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a TCP Port.
	/// </summary>
	internal static string String92 => ResourceManager.GetString("String92", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a PLC Off-Line.
	/// </summary>
	internal static string String93 => ResourceManager.GetString("String93", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Communication Fail.
	/// </summary>
	internal static string String94 => ResourceManager.GetString("String94", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a PLC Connection Failed. Program can't continue. Review connections and setup communication parameters..
	/// </summary>
	internal static string String95 => ResourceManager.GetString("String95", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Running.
	/// </summary>
	internal static string String96 => ResourceManager.GetString("String96", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a Stopped.
	/// </summary>
	internal static string String97 => ResourceManager.GetString("String97", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a On-Line.
	/// </summary>
	internal static string String98 => ResourceManager.GetString("String98", resourceCulture);

	/// <summary>
	///   Busca una cadena traducida similar a General Error.
	/// </summary>
	internal static string String99 => ResourceManager.GetString("String99", resourceCulture);

	internal Idiomas()
	{
	}
}
