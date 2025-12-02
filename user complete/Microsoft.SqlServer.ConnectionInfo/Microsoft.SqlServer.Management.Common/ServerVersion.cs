using System;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public class ServerVersion
{
	private int m_nMajor;

	private int m_nMinor;

	private int m_nBuildNumber;

	public int Major => m_nMajor;

	public int Minor => m_nMinor;

	public int BuildNumber => m_nBuildNumber;

	public ServerVersion(int major, int minor)
	{
		m_nMajor = major;
		m_nMinor = minor;
	}

	public ServerVersion(int major, int minor, int buildNumber)
	{
		m_nMajor = major;
		m_nMinor = minor;
		m_nBuildNumber = buildNumber;
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", new object[3] { Major, Minor, BuildNumber });
	}

	public static explicit operator Version(ServerVersion serverVersion)
	{
		return new Version(serverVersion.Major, serverVersion.Minor, serverVersion.BuildNumber);
	}
}
