using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json.Linq;

namespace IPFUser;

[StandardModule]
internal sealed class LicenseManager
{
	private static readonly string[] DefaultLicensePaths = new string[2]
	{
		Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MiEmpresa", "MiProducto", "license.lic"),
		Path.Combine(Application.StartupPath, "license.lic")
	};

	private static readonly string PublicXmlPath = Path.Combine(Application.StartupPath, "PublicKey.xml");

	private static readonly string PublicKeyXml = string.Empty;

	/// <summary>
	/// Comprueba la licencia. Si licensePath es Nothing, buscará en rutas por defecto.
	/// </summary>
	public static bool IsLicenseValid(string licensePath = null)
	{
		bool IsLicenseValid;
		try
		{
			string licFile = licensePath;
			if (string.IsNullOrEmpty(licFile))
			{
				string[] defaultLicensePaths = DefaultLicensePaths;
				foreach (string p in defaultLicensePaths)
				{
					if (File.Exists(p))
					{
						licFile = p;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(licFile) || !File.Exists(licFile))
			{
				MessageBox.Show("No se ha encontrado la licencia (license.lic).", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				IsLicenseValid = false;
			}
			else
			{
				string licText = File.ReadAllText(licFile, Encoding.UTF8);
				JObject licObj = JObject.Parse(licText);
				string payload = licObj["payload"]?.ToString();
				string signatureB64 = licObj["signature"]?.ToString();
				if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signatureB64))
				{
					MessageBox.Show("Formato de licencia inválido.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					IsLicenseValid = false;
				}
				else
				{
					string pubXml = null;
					if (!string.IsNullOrEmpty(PublicKeyXml))
					{
						pubXml = PublicKeyXml;
					}
					else
					{
						try
						{
						}
						catch (Exception ex)
						{
							ProjectData.SetProjectError(ex);
							Exception ex2 = ex;
							ProjectData.ClearProjectError();
						}
						if (string.IsNullOrEmpty(pubXml) && File.Exists(PublicXmlPath))
						{
							pubXml = File.ReadAllText(PublicXmlPath, Encoding.UTF8);
						}
					}
					if (string.IsNullOrEmpty(pubXml))
					{
						MessageBox.Show("Clave pública no disponible para verificar la licencia.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						IsLicenseValid = false;
					}
					else if (!VerifySignature(payload, signatureB64, pubXml))
					{
						MessageBox.Show("Firma de la licencia inválida o modificada.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						IsLicenseValid = false;
					}
					else
					{
						JObject payloadObj = JObject.Parse(payload);
						string hwInLicense = payloadObj["hardwareId"]?.ToString();
						if (string.IsNullOrEmpty(hwInLicense))
						{
							MessageBox.Show("La licencia no contiene hardwareId.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
							IsLicenseValid = false;
						}
						else
						{
							string hwActual = new HardwareLicense().GetHardwareId();
							if (string.IsNullOrEmpty(hwActual))
							{
								MessageBox.Show("No se pudo calcular el hardwareId del equipo.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
								IsLicenseValid = false;
							}
							else if (!string.Equals(hwInLicense, hwActual, StringComparison.OrdinalIgnoreCase))
							{
								MessageBox.Show("La licencia no pertenece a este equipo.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
								IsLicenseValid = false;
							}
							else
							{
								IsLicenseValid = true;
							}
						}
					}
				}
			}
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			MessageBox.Show("Error al validar la licencia: " + ex4.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			IsLicenseValid = false;
			ProjectData.ClearProjectError();
		}
		return IsLicenseValid;
	}

	private static bool VerifySignature(string data, string signatureB64, string publicXml)
	{
		bool VerifySignature;
		try
		{
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);
			byte[] sigBytes = Convert.FromBase64String(signatureB64);
			using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.FromXmlString(publicXml);
			VerifySignature = rsa.VerifyData(dataBytes, CryptoConfig.MapNameToOID("SHA256"), sigBytes);
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			VerifySignature = false;
			ProjectData.ClearProjectError();
		}
		return VerifySignature;
	}
}
