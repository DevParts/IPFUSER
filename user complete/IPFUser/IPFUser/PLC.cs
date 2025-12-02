#define TRACE
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using IPFUser.My;
using MacsaDevicesNet;
using Microsoft.VisualBasic.CompilerServices;
using ModbusTCP;

namespace IPFUser;

public class PLC
{
	public enum PLC_COMMANDS
	{
		STATE,
		PROGRAM_PARAMETERS
	}

	public struct PLC_READ_CONTROL
	{
		public bool Rearmed;

		public bool Running;

		public bool Stopped;

		public bool GeneralError;

		public bool Life;

		public uint[] PrintedLayersQty;
	}

	public struct PLC_WRITE_CONTROL
	{
		public bool GoRearm;

		public bool GoRun;

		public bool GoStop;

		public bool GeneralError;

		public bool Life;

		public uint CycleElements;

		public ushort CycleLayers;

		public uint[] CycleLayersQty;

		public ushort CurrentLayer;

		public uint CurrentLayerConsumedItems;
	}

	public delegate void OnStateChangedEventHandler(object Sender, string State, bool IsError);

	public delegate void OnModbusExceptionEventHandler(object Sender, byte IdFunction, byte Exception);

	public delegate void OnNewDataReceivedEventHandler(object Sender);

	public delegate void OnNewDataSentEventHandler(object Sender);

	public const ushort MAX_LAYERS = 25;

	private bool _IsConnected;

	public PLC_READ_CONTROL RData;

	public PLC_WRITE_CONTROL WData;

	private Thread thPLCState;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[AccessedThroughProperty("oModBus")]
	private Master _oModBus;

	private PLC_COMMANDS Command;

	private virtual Master oModBus
	{
		[CompilerGenerated]
		get
		{
			return _oModBus;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		[CompilerGenerated]
		set
		{
			Master.ExceptionData value2 = oPLC_OnException;
			Master.ResponseData value3 = oPLC_OnResponseData;
			Master master = _oModBus;
			if (master != null)
			{
				master.OnException -= value2;
				master.OnResponseData -= value3;
			}
			_oModBus = value;
			master = _oModBus;
			if (master != null)
			{
				master.OnException += value2;
				master.OnResponseData += value3;
			}
		}
	}

	public object IsConnected
	{
		get
		{
			bool b = (oModBus != null) & oModBus.connected;
			if (b & _IsConnected)
			{
				return true;
			}
			return false;
		}
	}

	public event OnStateChangedEventHandler OnStateChanged;

	public event OnModbusExceptionEventHandler OnModbusException;

	public event OnNewDataReceivedEventHandler OnNewDataReceived;

	public event OnNewDataSentEventHandler OnNewDataSent;

	public PLC()
	{
		oModBus = new Master();
		Command = PLC_COMMANDS.STATE;
	}

	public bool Init()
	{
		bool Init;
		try
		{
			RData.PrintedLayersQty = new uint[25];
			WData.CycleLayersQty = new uint[25];
			WData.GoRearm = false;
			WData.GoStop = false;
			WData.GoRun = false;
			if (MyProject.Computer.Network.Ping(MySettingsProperty.Settings.PLC[0], 300))
			{
				try
				{
					oModBus.timeout = 1000;
					oModBus.connect(MySettingsProperty.Settings.PLC[0], Conversions.ToInteger(MySettingsProperty.Settings.PLC[1]));
					OnStateChanged?.Invoke(this, AppCSIUser.Rm.GetString("String93"), IsError: false);
					StartPLCState();
					Init = true;
				}
				catch (Exception ex)
				{
					ProjectData.SetProjectError(ex);
					Exception ex2 = ex;
					MessageBox.Show(AppCSIUser.Rm.GetString("String94") + ": " + ex2.Message);
					Init = false;
					ProjectData.ClearProjectError();
				}
			}
			else
			{
				OnStateChanged?.Invoke(this, AppCSIUser.Rm.GetString("String94"), IsError: true);
				Init = false;
			}
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			OnStateChanged?.Invoke(this, AppCSIUser.Rm.GetString("String94"), IsError: true);
			Init = false;
			ProjectData.ClearProjectError();
		}
		return Init;
	}

	/// <summary>
	/// Rellena la información de ciclo para el PLC
	/// </summary>
	/// <remarks></remarks>
	public void FillCycle(int QueuesElements)
	{
		checked
		{
			WData.CycleElements = (uint)AppCSIUser.Promo.CycleElements;
			WData.CycleLayers = (ushort)AppCSIUser.Promo.Layers;
			int num = AppCSIUser.Promo.Layers - 1;
			for (int i = 0; i <= num; i++)
			{
				WData.CycleLayersQty[i] = (uint)AppCSIUser.Promo.get_LayerQty(i);
			}
			uint ConsumedCycle = Conversions.ToUInteger(Operators.SubtractObject(Operators.SubtractObject(AppCSIUser.Promo.ConsumedCodes, QueuesElements), Operators.MultiplyObject(Operators.IntDivideObject(Operators.SubtractObject(AppCSIUser.Promo.ConsumedCodes, QueuesElements), AppCSIUser.Promo.CycleElements), AppCSIUser.Promo.CycleElements)));
			Trace.WriteLine("Punt Entrada Codis DB " + ConsumedCycle);
			Trace.WriteLine("Punt Entrada Codis en Cues " + QueuesElements);
			Trace.WriteLine("Punt Entrada Codis disponibles " + ConsumedCycle);
			uint Suma = 0u;
			int num2 = AppCSIUser.Promo.Layers - 1;
			for (int j = 0; j <= num2; j++)
			{
				Suma = (uint)(Suma + AppCSIUser.Promo.get_LayerQty(j));
				if (ConsumedCycle <= Suma)
				{
					WData.CurrentLayer = (ushort)j;
					WData.CurrentLayerConsumedItems = (uint)(ConsumedCycle - (Suma - AppCSIUser.Promo.get_LayerQty(j)));
					break;
				}
			}
			Trace.WriteLine("Layer: " + WData.CurrentLayer + " ConsumedItems: " + WData.CurrentLayerConsumedItems);
		}
	}

	public void Start()
	{
		WData.GoRun = true;
		WData.GoStop = false;
	}

	public void Stop()
	{
		WData.GoRun = false;
		WData.GoStop = true;
	}

	public void Rearm()
	{
		WData.GoRearm = true;
	}

	public void CloseConnection()
	{
		StopPLCState();
	}

	public void StartPLCState()
	{
		if (thPLCState == null)
		{
			_IsConnected = true;
			thPLCState = new Thread(PLCState);
			thPLCState.Start();
		}
	}

	public void StopPLCState()
	{
		try
		{
			if (thPLCState != null && thPLCState.IsAlive)
			{
				thPLCState.Interrupt();
				while (thPLCState.IsAlive)
				{
					thPLCState.Abort();
				}
			}
			_IsConnected = false;
			oModBus.disconnect();
			thPLCState = null;
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Common.MACSALog("Error on closing PLC Thread: " + ex2.Message, TraceEventType.Error);
			ProjectData.ClearProjectError();
		}
	}

	public void Reverse(byte[] Data, int StartIndex, int Length)
	{
		checked
		{
			byte[] Tmp = new byte[Length - 1 + 1];
			int num = Length - 1;
			for (int i = 0; i <= num; i++)
			{
				Tmp[i] = Data[StartIndex + i];
			}
			Array.Reverse(Tmp);
			int num2 = Length - 1;
			for (int j = 0; j <= num2; j++)
			{
				Data[StartIndex + j] = Tmp[j];
			}
		}
	}

	public void PLCState()
	{
		checked
		{
			while (true)
			{
				try
				{
					switch (Command)
					{
					case PLC_COMMANDS.STATE:
						oModBus.ReadHoldingRegister(1, 55000, 104);
						break;
					case PLC_COMMANDS.PROGRAM_PARAMETERS:
					{
						byte[] Data = new byte[120];
						int Index = 0;
						ushort Value = 0;
						uint Value32 = 0u;
						if (WData.GoRearm)
						{
							Value |= 1;
						}
						if (WData.GoRun)
						{
							Value |= 2;
						}
						if (WData.GoStop)
						{
							Value |= 4;
						}
						if (WData.GeneralError)
						{
							Value |= 8;
						}
						if (WData.Life)
						{
							Value |= 0x10;
						}
						byte[] Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value = 0;
						Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value32 = WData.CycleElements;
						Dummy = BitConverter.GetBytes((ushort)(unchecked((long)Value32) & 0xFFFFL));
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Dummy = BitConverter.GetBytes((ushort)((Value32 & -65536) >> 16));
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value = WData.CycleLayers;
						Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value = 0;
						Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						int i = 0;
						do
						{
							Value32 = WData.CycleLayersQty[i];
							Dummy = BitConverter.GetBytes((ushort)(unchecked((long)Value32) & 0xFFFFL));
							Reverse(Dummy, 0, 2);
							Array.Copy(Dummy, 0, Data, Index, 2);
							Index += 2;
							Dummy = BitConverter.GetBytes((ushort)((Value32 & -65536) >> 16));
							Reverse(Dummy, 0, 2);
							Array.Copy(Dummy, 0, Data, Index, 2);
							Index += 2;
							i++;
						}
						while (i <= 24);
						Value = WData.CurrentLayer;
						Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value = 0;
						Dummy = BitConverter.GetBytes(Value);
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Value32 = WData.CurrentLayerConsumedItems;
						Dummy = BitConverter.GetBytes((ushort)(unchecked((long)Value32) & 0xFFFFL));
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						Dummy = BitConverter.GetBytes((ushort)((Value32 & -65536) >> 16));
						Reverse(Dummy, 0, 2);
						Array.Copy(Dummy, 0, Data, Index, 2);
						Index += 2;
						oModBus.WriteMultipleRegister(2, 55100, 60, Data);
						break;
					}
					}
					Thread.Sleep(500);
				}
				catch (ThreadInterruptedException ex)
				{
					ProjectData.SetProjectError(ex);
					ThreadInterruptedException ex2 = ex;
					ProjectData.ClearProjectError();
					break;
				}
				catch (ThreadAbortException ex3)
				{
					ProjectData.SetProjectError(ex3);
					ThreadAbortException ex4 = ex3;
					ProjectData.ClearProjectError();
					break;
				}
				catch (Exception ex5)
				{
					ProjectData.SetProjectError(ex5);
					Exception ex6 = ex5;
					Common.MACSALog("Error on PLC Sending Threading", TraceEventType.Error);
					_IsConnected = false;
					ProjectData.ClearProjectError();
				}
			}
		}
	}

	private void oPLC_OnException(int id, byte IdFunction, byte exception)
	{
		StopPLCState();
		OnModbusException?.Invoke(this, IdFunction, exception);
	}

	private void oPLC_OnResponseData(int id, byte IdFunction, byte[] data)
	{
		checked
		{
			switch (id)
			{
			case 1:
			{
				Reverse(data, 0, 2);
				ushort Value = BitConverter.ToUInt16(data, 0);
				if ((Value & 1) == 1)
				{
					RData.Rearmed = true;
				}
				else
				{
					RData.Rearmed = false;
				}
				if ((Value & 2) == 2)
				{
					RData.Running = true;
				}
				else
				{
					RData.Running = false;
				}
				if ((Value & 4) == 4)
				{
					RData.Stopped = true;
				}
				else
				{
					RData.Stopped = false;
				}
				if ((Value & 8) == 8)
				{
					RData.GeneralError = true;
				}
				else
				{
					RData.GeneralError = false;
				}
				if ((Value & 0x10) == 16)
				{
					RData.Life = true;
				}
				else
				{
					RData.Life = false;
				}
				int i = 0;
				do
				{
					Reverse(data, 4 + i * 4, 2);
					RData.PrintedLayersQty[i] = BitConverter.ToUInt16(data, 4 + i * 4);
					Reverse(data, 6 + i * 4, 2);
					ref uint reference = ref RData.PrintedLayersQty[i];
					reference = (uint)(reference + unchecked((long)BitConverter.ToUInt16(data, checked(6 + i * 4))) * 65536L);
					i++;
				}
				while (i <= 24);
				if (RData.Life == WData.Life)
				{
					WData.Life = !WData.Life;
				}
				Command = PLC_COMMANDS.PROGRAM_PARAMETERS;
				OnNewDataReceived?.Invoke(this);
				break;
			}
			case 2:
				Command = PLC_COMMANDS.STATE;
				OnNewDataSent?.Invoke(this);
				break;
			}
		}
	}
}
