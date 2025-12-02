using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Aladdin.Hasp;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Licenses;

public class AladdinHasp
{
	private int passw1;

	private int passw2;

	private int seedCode;

	private int lptNum;

	private string _HaspKeyFilePath;

	private const string sSecretKey = "aliCIAPg";

	private int ConnectedKeys;

	public string HaspKeyFilePath
	{
		get
		{
			return _HaspKeyFilePath;
		}
		set
		{
			_HaspKeyFilePath = value;
		}
	}

	public AladdinHasp()
	{
		passw1 = 31151;
		passw2 = 4830;
		seedCode = 0;
		lptNum = 0;
		_HaspKeyFilePath = "";
	}

	public bool HaspIsPresent()
	{
		int num = 0;
		int num2 = 0;
		object obj = num;
		object obj2 = num;
		object obj3 = num2;
		HaspKey.Hasp((HaspService)1, seedCode, lptNum, 0, 0, RuntimeHelpers.GetObjectValue(obj), RuntimeHelpers.GetObjectValue(obj2), RuntimeHelpers.GetObjectValue(obj3), (object)null);
		num = Conversions.ToInteger(obj);
		num2 = Conversions.ToInteger(obj3);
		ConnectedKeys = Conversions.ToInteger(obj2);
		if (num2 != 0)
		{
			return false;
		}
		if (0 == num)
		{
			return false;
		}
		return 0 < num;
	}

	public int[] GetID()
	{
		if (!HaspIsPresent())
		{
			return new int[1] { 0 };
		}
		checked
		{
			int[] array = new int[ConnectedKeys - 1 + 1];
			int num = ConnectedKeys - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				object obj = num5;
				object obj2 = num6;
				object obj3 = num7;
				HaspKey.Hasp((HaspService)6, seedCode, 201 + num2, passw1, passw2, RuntimeHelpers.GetObjectValue(obj), RuntimeHelpers.GetObjectValue(obj2), RuntimeHelpers.GetObjectValue(obj3), (object)null);
				if (Conversions.ToInteger(obj3) != 0)
				{
					return new int[1] { 0 };
				}
				num5 = Conversions.ToInteger(obj);
				num6 = Conversions.ToInteger(obj2);
				if (32767 < num6)
				{
					num6 -= 65536;
				}
				num6 *= 65536;
				array[num2] = num6 + num5;
				num2++;
			}
			return array;
		}
	}

	public bool KeyIsRegistered()
	{
		bool result;
		checked
		{
			if (Operators.CompareString(_HaspKeyFilePath, "", TextCompare: false) == 0)
			{
				result = false;
			}
			else if (!File.Exists(_HaspKeyFilePath + "\\Keys.kf"))
			{
				result = false;
			}
			else
			{
				DecryptFile(_HaspKeyFilePath + "\\Keys.kf", _HaspKeyFilePath + "\\Keys.dat", "aliCIAPg");
				if (!File.Exists(_HaspKeyFilePath + "\\Keys.dat"))
				{
					result = false;
				}
				else
				{
					int[] iD = GetID();
					bool flag = false;
					FileStream fileStream = new FileStream(_HaspKeyFilePath + "\\Keys.dat", FileMode.Open, FileAccess.Read);
					StreamReader streamReader = new StreamReader(fileStream);
					try
					{
						if (Operators.CompareString(streamReader.ReadLine(), "MACSA KEYS FILE", TextCompare: false) != 0)
						{
							streamReader.Close();
							result = false;
							goto IL_0196;
						}
					}
					catch (Exception ex)
					{
						ProjectData.SetProjectError(ex);
						Exception ex2 = ex;
						fileStream.Close();
						result = false;
						ProjectData.ClearProjectError();
						goto IL_0196;
					}
					while (!streamReader.EndOfStream)
					{
						int num = iD.Length - 1;
						int num2 = 0;
						while (true)
						{
							int num3 = num2;
							int num4 = num;
							if (num3 > num4)
							{
								break;
							}
							if (Operators.CompareString(streamReader.ReadLine(), iD[num2].ToString("X"), TextCompare: false) != 0)
							{
								num2++;
								continue;
							}
							goto IL_014c;
						}
						continue;
						IL_014c:
						flag = true;
						break;
					}
					streamReader.Close();
					File.Delete(_HaspKeyFilePath + "\\Keys.dat");
					result = flag;
				}
			}
			goto IL_0196;
		}
		IL_0196:
		return result;
	}

	public bool RegisterKey()
	{
		if (Operators.CompareString(_HaspKeyFilePath, "", TextCompare: false) == 0)
		{
			return false;
		}
		int[] iD = GetID();
		StreamWriter streamWriter;
		if (File.Exists(_HaspKeyFilePath + "\\Keys.kf"))
		{
			DecryptFile(_HaspKeyFilePath + "\\Keys.kf", _HaspKeyFilePath + "\\Keys.dat", "aliCIAPg");
			FileStream stream = new FileStream(_HaspKeyFilePath + "\\Keys.dat", FileMode.Append, FileAccess.Write);
			streamWriter = new StreamWriter(stream);
		}
		else
		{
			FileStream stream = new FileStream(_HaspKeyFilePath + "\\Keys.dat", FileMode.Create, FileAccess.Write);
			streamWriter = new StreamWriter(stream);
			streamWriter.WriteLine("MACSA KEYS FILE");
		}
		checked
		{
			int num = iD.Length - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				streamWriter.WriteLine(iD[num2].ToString("X"));
				num2++;
			}
			streamWriter.Close();
			EncryptFile(_HaspKeyFilePath + "\\Keys.dat", _HaspKeyFilePath + "\\Keys.kf", "aliCIAPg");
			File.Delete(_HaspKeyFilePath + "\\Keys.dat");
			return true;
		}
	}

	public void UnregisterKey(string Key)
	{
		DecryptFile(_HaspKeyFilePath + "\\Keys.kf", _HaspKeyFilePath + "\\Keys.dat", "aliCIAPg");
		FileStream fileStream = new FileStream(_HaspKeyFilePath + "\\Keys.dat", FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		try
		{
			if (Operators.CompareString(streamReader.ReadLine(), "MACSA KEYS FILE", TextCompare: false) != 0)
			{
				streamReader.Close();
				return;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			fileStream.Close();
			ProjectData.ClearProjectError();
			return;
		}
		FileStream stream = new FileStream(_HaspKeyFilePath + "\\KeysN.dat", FileMode.Create, FileAccess.Write);
		StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.WriteLine("MACSA KEYS FILE");
		while (!streamReader.EndOfStream)
		{
			string text = streamReader.ReadLine();
			if (Operators.CompareString(text, Key, TextCompare: false) != 0)
			{
				streamWriter.WriteLine(text);
			}
		}
		streamWriter.Close();
		streamReader.Close();
		File.Delete(_HaspKeyFilePath + "\\Keys.dat");
		EncryptFile(_HaspKeyFilePath + "\\KeysN.dat", _HaspKeyFilePath + "\\Keys.kf", "aliCIAPg");
		File.Delete(_HaspKeyFilePath + "\\KeysN.dat");
	}

	public string[] GetInfoFile()
	{
		DecryptFile(_HaspKeyFilePath + "\\Keys.kf", _HaspKeyFilePath + "\\Keys.dat", "aliCIAPg");
		FileStream fileStream = new FileStream(_HaspKeyFilePath + "\\Keys.dat", FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream);
		string[] result;
		try
		{
			if (Operators.CompareString(streamReader.ReadLine(), "MACSA KEYS FILE", TextCompare: false) != 0)
			{
				streamReader.Close();
				result = null;
				goto IL_0122;
			}
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			fileStream.Close();
			result = null;
			ProjectData.ClearProjectError();
			goto IL_0122;
		}
		checked
		{
			string[] array = default(string[]);
			int num = default(int);
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				array = (string[])Utils.CopyArray(array, new string[num + 1]);
				array[num] = text;
				num++;
			}
			streamReader.Close();
			File.Delete(_HaspKeyFilePath + "\\Keys.dat");
			if (array == null)
			{
				array = new string[1] { "<EMPTY>" };
			}
			result = array;
			goto IL_0122;
		}
		IL_0122:
		return result;
	}

	private void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
	{
		FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
		FileStream stream = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
		dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
		ICryptoTransform transform = dESCryptoServiceProvider.CreateEncryptor();
		CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write);
		byte[] array = new byte[checked((int)fileStream.Length - 1 + 1)];
		fileStream.Read(array, 0, array.Length);
		cryptoStream.Write(array, 0, array.Length);
		cryptoStream.Close();
		fileStream.Close();
	}

	private void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
	{
		DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
		dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
		dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
		FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
		ICryptoTransform transform = dESCryptoServiceProvider.CreateDecryptor();
		CryptoStream stream = new CryptoStream(fileStream, transform, CryptoStreamMode.Read);
		StreamWriter streamWriter = new StreamWriter(sOutputFilename);
		streamWriter.Write(new StreamReader(stream).ReadToEnd());
		streamWriter.Flush();
		streamWriter.Close();
		fileStream.Close();
	}
}
