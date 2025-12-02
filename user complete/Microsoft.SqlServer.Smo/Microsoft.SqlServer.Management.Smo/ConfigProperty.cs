using System;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ConfigProperty
{
	private ConfigurationBase m_configbase;

	private int m_iNumber;

	public string DisplayName => (string)m_configbase.GetConfigProperty(m_iNumber, "Name");

	public int Number => GetIntProperty("Number");

	public int Minimum => GetIntProperty("Minimum");

	public int Maximum => GetIntProperty("Maximum");

	public bool IsDynamic => GetBoolProperty("Dynamic");

	public bool IsAdvanced => GetBoolProperty("Advanced");

	public string Description => (string)m_configbase.GetConfigProperty(m_iNumber, "Description");

	public int RunValue => GetIntProperty("RunValue");

	public int ConfigValue
	{
		get
		{
			return GetIntProperty("Value");
		}
		set
		{
			m_configbase.SetConfigProperty(m_iNumber, value);
		}
	}

	internal ConfigProperty(ConfigurationBase configbase, int number)
	{
		m_configbase = configbase;
		m_iNumber = number;
	}

	private int GetIntProperty(string propertyName)
	{
		object configProperty = m_configbase.GetConfigProperty(m_iNumber, propertyName);
		if (configProperty == null || typeof(DBNull) == configProperty.GetType())
		{
			return 0;
		}
		return (int)configProperty;
	}

	private bool GetBoolProperty(string propertyName)
	{
		object configProperty = m_configbase.GetConfigProperty(m_iNumber, propertyName);
		if (configProperty == null || typeof(DBNull) == configProperty.GetType())
		{
			return false;
		}
		return (bool)configProperty;
	}
}
