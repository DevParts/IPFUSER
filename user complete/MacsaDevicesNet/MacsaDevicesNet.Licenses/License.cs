using System;
using System.Collections;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Licenses;

public class License
{
	public enum KEY_TYPES
	{
		K_INFINITE,
		K_30DAYS,
		K_90DAYS,
		K_180DAYS,
		K_360DAYS
	}

	private ArrayList hdLista;

	private string PcKey;

	private KEY_TYPES KeyType;

	private string LicPath;

	private string MD5Value;

	public string GetPcKey => PcKey;

	public KEY_TYPES GetKeyType => KeyType;

	public string LicensePath => LicPath;

	public string LicenseCrc => MD5Value;

	public License(string AppPath)
	{
		hdLista = new ArrayList();
		LicPath = AppPath;
		string text = "";
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		int num = 0;
		if (num < allNetworkInterfaces.Length)
		{
			NetworkInterface networkInterface = allNetworkInterfaces[num];
			text = networkInterface.GetPhysicalAddress().ToString();
		}
		if (Operators.CompareString(text, "", TextCompare: false) == 0)
		{
			text = "óKLAKjskeiijsóöïsdkksj";
		}
		checked
		{
			if (AppPath.Length > 5)
			{
				text += AppPath.Substring(AppPath.Length - 5);
			}
			if (AppPath.Length > 10)
			{
				text = AppPath.Substring(AppPath.Length - 10, 5) + text;
			}
			PcKey = "";
			int num2 = text.Length - 1;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 <= num5)
				{
					PcKey += Conversion.Hex(Strings.Asc(text[num3])).PadLeft(2, '0');
					if ((unchecked(num3 % 2) == 1) & (num3 < text.Length - 1))
					{
						PcKey += "-";
					}
					num3++;
					continue;
				}
				break;
			}
		}
	}

	private bool ValidateDurantionKey(string AppKey, KEY_TYPES Duration)
	{
		byte[] array = new byte[24]
		{
			51, 34, 250, 21, 24, 152, 51, 24, 48, 16,
			25, 113, 51, 34, 250, 21, 24, 152, 193, 24,
			181, 16, 25, 160
		};
		int num = default(int);
		MACTripleDES mACTripleDES;
		switch ((int)Duration)
		{
		case 1:
			num = 30;
			array[0] = 135;
			mACTripleDES = new MACTripleDES(array);
			break;
		case 2:
			num = 90;
			array[0] = 66;
			mACTripleDES = new MACTripleDES(array);
			break;
		case 3:
			num = 180;
			array[0] = 33;
			mACTripleDES = new MACTripleDES(array);
			break;
		case 4:
			num = 360;
			array[0] = 9;
			mACTripleDES = new MACTripleDES(array);
			break;
		default:
			mACTripleDES = new MACTripleDES(array);
			break;
		}
		byte[] array2 = mACTripleDES.ComputeHash(Encoding.Default.GetBytes(PcKey));
		string text = "";
		checked
		{
			int num2 = array2.Length - 1;
			int num3 = 0;
			while (true)
			{
				int num4 = num3;
				int num5 = num2;
				if (num4 > num5)
				{
					break;
				}
				text += Conversion.Hex(array2[num3]).PadLeft(2, '0');
				num3++;
			}
			if (Operators.CompareString(text, AppKey, TextCompare: false) == 0)
			{
				KeyType = Duration;
				if (Duration == KEY_TYPES.K_INFINITE)
				{
					if (File.Exists(LicPath + "\\License.dat"))
					{
						File.Delete(LicPath + "\\License.dat");
					}
				}
				else if (!File.Exists(LicPath + "\\License.dat"))
				{
					FileStream output = new FileStream(LicPath + "\\License.dat", FileMode.Create, FileAccess.ReadWrite);
					BinaryWriter binaryWriter = new BinaryWriter(output);
					binaryWriter.Write(num);
					binaryWriter.Write(0);
					binaryWriter.Write(DateTime.Now.ToString());
					binaryWriter.Write(DateTime.Now.AddDays(num).ToString());
					binaryWriter.Write(DateTime.Now.ToString());
					binaryWriter.Seek(0, SeekOrigin.Begin);
					MD5Value = Encoding.Default.GetString(MD5.Create().ComputeHash(binaryWriter.BaseStream));
					binaryWriter.Close();
				}
				else
				{
					FileStream input = new FileStream(LicPath + "\\License.dat", FileMode.Open, FileAccess.Read);
					BinaryReader binaryReader = new BinaryReader(input);
					int num6 = binaryReader.ReadInt32();
					int num7 = binaryReader.ReadInt32();
					DateTime t = Conversions.ToDate(binaryReader.ReadString());
					DateTime t2 = Conversions.ToDate(binaryReader.ReadString());
					DateTime dateTime = Conversions.ToDate(binaryReader.ReadString());
					binaryReader.Close();
					if (DateTime.Compare(DateTime.Now, dateTime) < 0)
					{
						num7++;
						dateTime = DateTime.Now;
					}
					else if (DateAndTime.DateDiff(DateInterval.Day, dateTime, DateTime.Now) >= 1)
					{
						num7++;
						dateTime = DateTime.Now;
					}
					if ((DateTime.Compare(DateTime.Now, t2) > 0) | (DateTime.Compare(DateTime.Now, t) < 0))
					{
						return false;
					}
					if (num7 > num6)
					{
						return false;
					}
					input = new FileStream(LicPath + "\\License.dat", FileMode.Create, FileAccess.ReadWrite);
					BinaryWriter binaryWriter2 = new BinaryWriter(input);
					binaryWriter2.Write(num);
					binaryWriter2.Write(num7);
					binaryWriter2.Write(t.ToString());
					binaryWriter2.Write(t2.ToString());
					binaryWriter2.Write(dateTime.ToString());
					binaryWriter2.Seek(0, SeekOrigin.Begin);
					MD5Value = Encoding.Default.GetString(MD5.Create().ComputeHash(binaryWriter2.BaseStream));
					binaryWriter2.Close();
				}
				return true;
			}
			return false;
		}
	}

	public bool ValidateAppKey(string AppKey)
	{
		if (ValidateDurantionKey(AppKey, KEY_TYPES.K_INFINITE))
		{
			return true;
		}
		if (ValidateDurantionKey(AppKey, KEY_TYPES.K_30DAYS))
		{
			return true;
		}
		if (ValidateDurantionKey(AppKey, KEY_TYPES.K_90DAYS))
		{
			return true;
		}
		if (ValidateDurantionKey(AppKey, KEY_TYPES.K_180DAYS))
		{
			return true;
		}
		if (ValidateDurantionKey(AppKey, KEY_TYPES.K_360DAYS))
		{
			return true;
		}
		return false;
	}

	public string GetAppKey(string SerialKey, KEY_TYPES Duration = KEY_TYPES.K_INFINITE)
	{
		byte[] array = new byte[24]
		{
			51, 34, 250, 21, 24, 152, 51, 24, 48, 16,
			25, 113, 51, 34, 250, 21, 24, 152, 193, 24,
			181, 16, 25, 160
		};
		switch ((int)Duration)
		{
		case 1:
			array[0] = 135;
			break;
		case 2:
			array[0] = 66;
			break;
		case 3:
			array[0] = 33;
			break;
		case 4:
			array[0] = 9;
			break;
		}
		MACTripleDES mACTripleDES = new MACTripleDES(array);
		byte[] array2 = mACTripleDES.ComputeHash(Encoding.Default.GetBytes(SerialKey));
		string text = "";
		checked
		{
			int num = array2.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				text += Conversion.Hex(array2[num2]).PadLeft(2, '0');
				num2++;
			}
			return text;
		}
	}

	public bool VerifyLicenseFile(string Crc)
	{
		FileStream input = new FileStream(LicPath + "\\License.dat", FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(input);
		MD5Value = Encoding.Default.GetString(MD5.Create().ComputeHash(binaryReader.BaseStream));
		binaryReader.Close();
		if (Operators.CompareString(MD5Value, Crc, TextCompare: false) != 0)
		{
			return false;
		}
		return true;
	}
}
