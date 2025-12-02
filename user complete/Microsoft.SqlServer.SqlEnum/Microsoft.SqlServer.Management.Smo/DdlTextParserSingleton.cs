using System.Text.RegularExpressions;

namespace Microsoft.SqlServer.Management.Smo;

internal class DdlTextParserSingleton
{
	internal bool hasParanthesis;

	internal string returnTableVariableName;

	internal Regex regex;

	internal Regex m_r_end;
}
