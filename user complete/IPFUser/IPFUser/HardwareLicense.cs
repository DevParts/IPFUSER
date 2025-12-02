using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace IPFUser;

public class HardwareLicense
{
	/// <summary>
	/// Devuelve un identificador de hardware único basado en CPU, disco, MAC y placa base.
	/// Si alguno falla, usa MachineGuid del registro como respaldo.
	/// </summary>
	public string GetHardwareId()
	{
		string cpu = GetWmiProperty("Win32_Processor", "ProcessorId");
		string disk = GetWmiProperty("Win32_DiskDrive", "SerialNumber");
		string mac = GetWmiProperty("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled", "TRUE");
		string mb = GetWmiProperty("Win32_BaseBoard", "SerialNumber");
		if (string.IsNullOrEmpty(cpu) || string.IsNullOrEmpty(disk) || string.IsNullOrEmpty(mac) || string.IsNullOrEmpty(mb))
		{
			try
			{
				using RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Cryptography");
				string machineGuid = regKey.GetValue("MachineGuid", "").ToString();
				if (!string.IsNullOrEmpty(machineGuid))
				{
					return ComputeSha256Hash(machineGuid);
				}
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				ProjectData.ClearProjectError();
			}
		}
		string combined = $"{cpu}|{disk}|{mac}|{mb}";
		return ComputeSha256Hash(combined);
	}

	/// <summary>
	/// Obtiene una propiedad WMI de una clase dada (con opción de condición).
	/// </summary>
	private string GetWmiProperty(string wmiClass, string prop, string condProp = null, string condVal = null)
	{
		try
		{
			ManagementClass mc = new ManagementClass(wmiClass);
			foreach (ManagementObject mo in mc.GetInstances())
			{
				if ((condProp == null || (mo[condProp] != null && Operators.CompareString(mo[condProp].ToString(), condVal, TextCompare: false) == 0)) && mo[prop] != null)
				{
					return mo[prop].ToString().Trim();
				}
			}
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			ProjectData.ClearProjectError();
		}
		return string.Empty;
	}

	/// <summary>
	/// Calcula el hash SHA-256 de una cadena de texto.
	/// </summary>
	private string ComputeSha256Hash(string input)
	{
		using SHA256 sha = SHA256.Create();
		byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder sb = new StringBuilder();
		byte[] array = hash;
		foreach (byte b in array)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}
}
