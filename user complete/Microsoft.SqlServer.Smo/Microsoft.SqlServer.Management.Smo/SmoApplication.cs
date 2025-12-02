using System;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public class SmoApplication
{
	public delegate void ObjectCreatedEventHandler(object sender, ObjectCreatedEventArgs e);

	public delegate void ObjectDroppedEventHandler(object sender, ObjectDroppedEventArgs e);

	public delegate void ObjectRenamedEventHandler(object sender, ObjectRenamedEventArgs e);

	public delegate void ObjectAlteredEventHandler(object sender, ObjectAlteredEventArgs e);

	public delegate void AnyObjectEventHandler(object sender, SmoEventArgs e);

	public delegate void DatabaseEventHandler(object sender, SmoEventArgs e);

	internal static readonly uint trL1 = 1u;

	internal static readonly uint trL2 = 2u;

	internal static readonly uint trErr = 1073741824u;

	internal static readonly uint trWarn = 536870912u;

	internal static readonly uint trAlways = 268435456u;

	internal static readonly SmoApplicationEventsSingleton eventsSingleton = new SmoApplicationEventsSingleton();

	internal static readonly string ModuleName = "Smo";

	public static ISmoApplicationEvents EventsSingleton => eventsSingleton;

	internal static string Namespace => typeof(SmoApplication).Namespace;

	public static CultureInfo DefaultCulture => CultureInfo.InvariantCulture;

	internal static int ConvertUInt32ToInt32(uint value)
	{
		uint[] src = new uint[1] { value };
		int[] array = new int[1];
		Buffer.BlockCopy(src, 0, array, 0, 4);
		return array[0];
	}

	internal static uint ConvertInt32ToUInt32(int value)
	{
		int[] src = new int[1] { value };
		uint[] array = new uint[1];
		Buffer.BlockCopy(src, 0, array, 0, 4);
		return array[0];
	}

	public static DataTable EnumAvailableSqlServers()
	{
		try
		{
			return Enumerator.GetData(null, "AvailableSqlServer");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAvailableSqlServers, null, ex);
		}
	}

	public static DataTable EnumAvailableSqlServers(bool localOnly)
	{
		try
		{
			if (localOnly)
			{
				return Enumerator.GetData(null, "AvailableSqlServer[@IsLocal = true()]");
			}
			return Enumerator.GetData(null, "AvailableSqlServer");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAvailableSqlServers, null, ex);
		}
	}

	public static DataTable EnumAvailableSqlServers(string name)
	{
		try
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return Enumerator.GetData(null, "AvailableSqlServer[@Name = '" + Urn.EscapeString(name) + "']");
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAvailableSqlServers, null, ex);
		}
	}
}
