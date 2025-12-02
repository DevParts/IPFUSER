using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcVersionAttribute : Attribute
{
	private Version m_begin;

	private Version m_end;

	public Version BeginVersion => m_begin;

	public Version EndVersion => m_end;

	public SfcVersionAttribute(int beginMajor, int beginMinor, int beginBuild, int beginRevision, int endMajor, int endMinor, int endBuild, int endRevision)
	{
		m_begin = new Version(beginMajor, beginMinor, beginBuild, beginRevision);
		m_end = new Version(endMajor, endMinor, endBuild, endRevision);
	}

	public SfcVersionAttribute(int beginMajor, int beginMinor, int beginBuild, int beginRevision)
	{
		m_begin = new Version(beginMajor, beginMinor, beginBuild, beginRevision);
	}

	public SfcVersionAttribute(int beginMajor, int endMajor)
	{
		m_begin = new Version(beginMajor, 0);
		m_end = new Version(endMajor, 0);
	}

	public SfcVersionAttribute(int beginMajor)
	{
		m_begin = new Version(beginMajor, 0);
	}
}
