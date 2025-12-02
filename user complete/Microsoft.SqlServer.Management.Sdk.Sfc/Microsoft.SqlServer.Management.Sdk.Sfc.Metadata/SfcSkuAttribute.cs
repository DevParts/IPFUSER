using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcSkuAttribute : Attribute
{
	private string[] m_skus;

	private bool m_exclusive;

	public string[] SkuNames => m_skus;

	public bool Exclusive => m_exclusive;

	public SfcSkuAttribute(string skuName, bool exclusive)
	{
		m_skus = new string[1];
		m_skus[0] = skuName;
		m_exclusive = exclusive;
	}

	public SfcSkuAttribute(string[] skuNames, bool exclusive)
	{
		m_skus = skuNames;
		m_exclusive = exclusive;
	}
}
