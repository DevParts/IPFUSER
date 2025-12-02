using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace DataBases;

[StandardModule]
public sealed class Comu1
{
	public static string GetLibraryVersion()
	{
		return Assembly.GetExecutingAssembly().GetName().Version.ToString();
	}
}
