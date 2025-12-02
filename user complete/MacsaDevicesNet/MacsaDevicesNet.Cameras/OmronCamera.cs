using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MacsaDevicesNet.ScanReaders;

namespace MacsaDevicesNet.Cameras;

public class OmronCamera : Lector
{
	public enum MODELS
	{
		FQCR15100NM
	}

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private int _ItemInspection;

	private int ExternalReference;

	public MODELS Model
	{
		[DebuggerNonUserCode]
		get;
		[DebuggerNonUserCode]
		set;
	}

	public int ItemInspection
	{
		get
		{
			return _ItemInspection;
		}
		set
		{
			_ItemInspection = value;
			ExternalReference = checked(202 + 3 * _ItemInspection);
		}
	}

	public OmronCamera()
	{
		__ENCAddToList(this);
		_ItemInspection = 0;
		ExternalReference = 202;
		MODELS model = MODELS.FQCR15100NM;
		Model = model;
	}

	[DebuggerNonUserCode]
	private static void __ENCAddToList(object value)
	{
		checked
		{
			lock (__ENCList)
			{
				if (__ENCList.Count == __ENCList.Capacity)
				{
					int num = 0;
					int num2 = __ENCList.Count - 1;
					int num3 = 0;
					while (true)
					{
						int num4 = num3;
						int num5 = num2;
						if (num4 > num5)
						{
							break;
						}
						WeakReference weakReference = __ENCList[num3];
						if (weakReference.IsAlive)
						{
							if (num3 != num)
							{
								__ENCList[num] = __ENCList[num3];
							}
							num++;
						}
						num3++;
					}
					__ENCList.RemoveRange(num, __ENCList.Count - num);
					__ENCList.Capacity = __ENCList.Count;
				}
				__ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
			}
		}
	}

	public void SendCmd(string Data)
	{
		string data = "";
		if (Model == MODELS.FQCR15100NM)
		{
			data = "ITEMDATA2 " + ItemInspection.ToString("D2") + " " + ExternalReference.ToString("D3") + " " + Data + "\r";
		}
		base.SendCmd(data, WAIT_TYPE.NoWait, 0L);
	}
}
