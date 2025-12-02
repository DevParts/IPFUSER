using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Applicators;

public class MLA : Magnetiq
{
	public enum IO_INPUT_LIST
	{
		IE1_DISPENSACION_IMPRESION,
		IE2_IMPRESION2_PUSHER,
		IE3_APLICACION,
		IE4_PARO_EMERGENCIA,
		IE5_RESET_GENERAL,
		IE6_TBD,
		IE7_INHIBICION_SIGCILO,
		II1_CILINDRO1_HOME,
		II2_GAP_ETIQUETA,
		II3_CILINDRO2_HOME,
		II4_CORREAS_PALA_REBPAPEL,
		II5_SENSOR_PALA_CIL1TOPE,
		II6_BUFFER_VACIO,
		II7_SCANNERREAD_COL2TOPE,
		II8_BUFFER_LLENO,
		II9_SENSORREBARRIBA_GAP,
		II10_SENSORREBABAJO,
		II11_LABELS_ENDING_REBOLLENO,
		II12_ENCODER1_ERRORMASTER,
		II13_ENCODER1FASE_INCIDENCIAMASTER,
		MII1_RIBBON_END,
		MII2_RIBBON_ENDING,
		MII3_END_OF_PRINT,
		MII4_LABELS_END,
		MII5_ERROR,
		MII6_ONLINE,
		MII7_POWER_ON,
		MII8_RFID_NO_READ
	}

	public enum IO_OUTPUT_LIST
	{
		OE1_MLP_ON,
		OE2_CILINDROS_HOME,
		OE3_TBD,
		OE4_ERROR_MLA_PRINTER,
		OE5_ALARMA,
		OE6_FINALCICLO,
		OE7_MODOAVERIA_ACTIVO,
		OE8_TBD,
		OI1_CILINDRO1,
		OI2_CILINDRO2,
		OI3_SOPADOR,
		OI4_VACIO,
		OI5_EYECCION,
		OI6_BALIZA_VERDE,
		OI7_BALIZA_AZUL,
		OI8_BALIZA_ROJO,
		OI9_MOTOR_FANCORREAS,
		OI10_TBD,
		OI11_DISPARO_SCANNER,
		OI12_STOPPER_PUERTA,
		MIO1_ORDEN_IMPRESION,
		MIO2_CALCULO_LONG_ETIQUETA,
		MIO3_PAUSE,
		MIO4_REIMPRESION,
		MIO5_INDEFINIDA
	}

	public struct _S_MOTORMULTI
	{
		public uint Velocidad;

		public ushort PasosAcel;

		public ushort PasosDecel;

		public ushort FreqInicial;

		public ushort FreqFinal;

		public uint mmPaso;

		public ushort Offset;

		public ushort RFU;
	}

	public struct _S_MOTORREBOBINADO
	{
		public byte Enabled;

		public byte ManoEquipo;

		public ushort PasosPorVuelta;

		public uint EspesorPalet;

		public uint Reduccion;

		public ushort DistSensRebo;

		public ushort RFU;
	}

	public struct MLAPARAMSDATA
	{
		public byte Idioma;

		public byte ManoEquipo;

		public ushort PasosPorVuelta;

		public uint EspesorPapel;

		public uint Reduccion;

		public ushort DistFotocelulaArista;

		public byte AlarmLowLevelLabels;

		public byte AlarmLowLevelRibbon;

		public byte ModuloImpresor;

		public byte ImpDots;

		public byte ImprAnchoCabezal;

		public byte Imp_BTN;

		public ushort PrintSpeed;

		public ushort LongEtiqueta;

		public byte TipoAplicador;

		public byte Scanner;

		public byte More_IE1;

		public byte More_IE2;

		public byte More_IE3;

		public byte Reimpresion;

		public byte Encoder;

		public byte PasosEncoder;

		public byte More_IE17_IE22;

		public byte More_II3;

		public byte More_FinalCiclo;

		public byte PuertaCabina;

		public byte Vacio_Impr;

		public byte Stopper;

		public byte Gap;

		public byte BufferEtiquetas;

		public ushort[] Timer;

		public uint[] Ports;

		public PORT_ETHERNET PortEthernet;

		public byte NoEtiquetas;

		public byte NoEtiquetasValue;

		public ushort L;

		public ushort G;

		public byte Teaching;

		public byte MotorStep;

		public uint RelPasmm;

		public byte IsSlave;

		public byte EtiquetasSlave;

		public byte RebobinadoExt;

		public byte RfuAlign;

		public ushort FrecuenciaTeaching;

		public byte UseCylinders;

		public byte ModoTrabajo;

		public byte ModoCiclo;

		public byte RfuAlign2;

		public ushort EncResolucion;

		public _S_MOTORMULTI Motor;

		public _S_MOTORREBOBINADO MotorRebo;

		public uint EncDiamRulina;

		public byte ModeHighSpeed;

		public byte LaserCfg;

		public byte ModoProactivo;

		public byte RFU2;

		public ushort[] RFU;

		public ushort RfuAlign3;

		public ushort Chk;
	}

	public struct MLASTATUSDATA
	{
		public byte ConfigVer;

		public byte ConfigRev;

		public byte EtiVer;

		public byte EtiRev;

		public uint ContadorParcial;

		public uint ContadorAcum;

		public uint ContadorTotal;

		public byte[] Errores;

		public byte[] Incidencias;

		public ushort Timer11;

		public ushort Timer12;

		public ushort Timer13;

		public ushort Timer14;

		public ushort[] TimerCiclos;

		public byte PrioridadErrores;

		public byte FamiliaActivaHS;

		public byte RFU1;

		public byte Offline;

		public byte PrioridadIncid;

		public byte FamiliaActivaRebobinado;

		public byte FamiliaActivaMultietiquetas;

		public byte DisplayReset;

		public ushort CodigoErrorImpresora;

		public byte[] MensajeImpresor;

		public ushort VelocidadEncoder;

		public ushort Chk;
	}

	public struct FAMILIA
	{
		public uint Velocidad;

		public ushort PasosRampaInicial;

		public ushort PasosRampaFinal;

		public ushort FreqInicial;

		public ushort FreqFinal;

		public uint InhibicionFotocelula;

		public uint NumEtiquetas;

		public uint PeelOff;

		public uint LongEtiqueta;

		public byte Index;

		public byte Activa;

		public ushort PasosRampaIniImpresion;

		public ushort T16RetrasoInicioCiclo;

		public ushort InhibicionGap;

		public ushort L;

		public ushort X;

		public ushort EncNumEtiquetas;

		public ushort EncMMPriEti;

		public ushort EncMMSigEti;

		public ushort EncMMINiFoto;

		public byte Tramo;

		public byte RFU2;

		public ushort[] RFU;

		public ushort Chk;
	}

	public struct FAMILY_MULTI
	{
		public byte Activa;

		public byte NumEtisLinea;

		public byte NumLineas;

		public byte Reverso;

		public byte Index;

		public byte RFU;

		public ushort SeparacionLineas;

		public ushort ColaCentrado;

		public ushort[] RFU2;

		public ushort Chk;
	}

	public struct FAMILA_HS
	{
		public byte Index;

		public byte Activa;

		public ushort RFU;

		public ushort DistMacula;

		public ushort DistMaculaAplicacion;

		public ushort RetardEV;

		public ushort[] RFU2;

		public ushort Chk;
	}

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnStoppedEventHandler(object sender);

	public delegate void OnRunningEventHandler(object sender);

	public delegate void OnInputChangedEventHandler(object sender, IO_INPUT_LIST Input, ML_IO_STATE State);

	public delegate void OnOutputChangedEventHandler(object sender, IO_OUTPUT_LIST Output, ML_IO_STATE State);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const int CONF_NUM_TIMERS_PARAMETROS = 20;

	private const int CONF_NUM_PORTS = 4;

	public MLASTATUSDATA Status;

	public MLAPARAMSDATA Parameters;

	public FAMILIA Family;

	public FAMILY_MULTI FamilyMulti;

	public FAMILA_HS FamilyHS;

	private int SizeofStatus;

	private int SizeofParameters;

	private int SizeofFamily;

	private int SizeofFamilyMulti;

	private int SizeofFamilyHs;

	public uint GetCounter
	{
		get
		{
			uint num = default(uint);
			return (int)Counter switch
			{
				1 => Status.ContadorAcum, 
				0 => Status.ContadorParcial, 
				2 => Status.ContadorTotal, 
				_ => num, 
			};
		}
	}

	[method: DebuggerNonUserCode]
	public event OnLineEventHandler OnLine;

	[method: DebuggerNonUserCode]
	public event OnStoppedEventHandler OnStopped;

	[method: DebuggerNonUserCode]
	public event OnRunningEventHandler OnRunning;

	[method: DebuggerNonUserCode]
	public event OnInputChangedEventHandler OnInputChanged;

	[method: DebuggerNonUserCode]
	public event OnOutputChangedEventHandler OnOutputChanged;

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

	public MLA()
	{
		base.OnFamilyRead += MLAFamilyRead;
		base.OnFamilyMultiRead += MLAFamilyMultiRead;
		base.OnFamilyHSRead += MLAFamilyHSRead;
		__ENCAddToList(this);
		InitStructStatus();
		InitStructParameters();
		InitStructFamily();
		InitStructFamilyMulti();
		InitStructFamilyHS();
	}

	public string GetMostPriorityError(byte[] bytesError, byte bytePrioridad)
	{
		string text = "";
		string text2 = "";
		byte b = (byte)((uint)(bytePrioridad & 0xF0) >> 4);
		byte b2 = (byte)(bytePrioridad & 0xF);
		switch (b)
		{
		case 0:
			return b2 switch
			{
				0 => Common.Rm.GetString("MLA1"), 
				1 => Common.Rm.GetString("MLA2"), 
				2 => Common.Rm.GetString("MLA3"), 
				3 => Common.Rm.GetString("MLA4"), 
				4 => Common.Rm.GetString("MLA5"), 
				5 => Common.Rm.GetString("MLA6"), 
				6 => Common.Rm.GetString("MLA7"), 
				7 => Common.Rm.GetString("MLA8"), 
				_ => Common.Rm.GetString("MLA9"), 
			};
		case 1:
			return b2 switch
			{
				0 => Common.Rm.GetString("MLA10"), 
				1 => Common.Rm.GetString("MLA11"), 
				2 => Common.Rm.GetString("MLA12"), 
				3 => Common.Rm.GetString("MLA13"), 
				4 => Common.Rm.GetString("MLA14"), 
				5 => Common.Rm.GetString("MLA15"), 
				6 => Common.Rm.GetString("MLA16"), 
				7 => Common.Rm.GetString("MLA17"), 
				_ => Common.Rm.GetString("MLA18"), 
			};
		case 2:
			return b2 switch
			{
				0 => Common.Rm.GetString("MLA19"), 
				1 => Common.Rm.GetString("MLA20"), 
				2 => Common.Rm.GetString("MLA21"), 
				3 => Common.Rm.GetString("MLA22"), 
				4 => Common.Rm.GetString("MLA23"), 
				5 => Common.Rm.GetString("MLA24"), 
				6 => Common.Rm.GetString("MLA25"), 
				7 => Common.Rm.GetString("MLA26"), 
				_ => Common.Rm.GetString("MLA18"), 
			};
		case 3:
			if (b2 == 0)
			{
				return Common.Rm.GetString("MLA35");
			}
			return Common.Rm.GetString("MLA18");
		case 7:
			return b2 switch
			{
				0 => Common.Rm.GetString("MLA27"), 
				1 => Common.Rm.GetString("MLA28"), 
				2 => Common.Rm.GetString("MLA29"), 
				3 => Common.Rm.GetString("MLA30"), 
				4 => Common.Rm.GetString("MLA31"), 
				5 => Common.Rm.GetString("MLA32"), 
				6 => Common.Rm.GetString("MLA33"), 
				7 => Common.Rm.GetString("MLA34"), 
				_ => Common.Rm.GetString("MLA18"), 
			};
		default:
			return Common.Rm.GetString("MLA18");
		}
	}

	public string GetMostPriorityWarning(byte[] bytesWarning, byte bytePrioridad)
	{
		string text = "";
		string text2 = "";
		byte b = (byte)((uint)(bytePrioridad & 0xF0) >> 4);
		byte b2 = (byte)(bytePrioridad & 0xF);
		if (b == 0)
		{
			return b2 switch
			{
				0 => Common.Rm.GetString("MLA36"), 
				1 => Common.Rm.GetString("MLA37"), 
				2 => Common.Rm.GetString("MLA38"), 
				3 => Common.Rm.GetString("MLA39"), 
				4 => Common.Rm.GetString("MLA40"), 
				5 => Common.Rm.GetString("MLA41"), 
				6 => Common.Rm.GetString("MLA42"), 
				7 => Common.Rm.GetString("MLA43"), 
				_ => Common.Rm.GetString("MLA9"), 
			};
		}
		return Common.Rm.GetString("MLA9");
	}

	public byte[] GetBinParameters()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		memoryStream.Position = 0L;
		binaryWriter.Write(Parameters.Idioma);
		binaryWriter.Write(Parameters.ManoEquipo);
		binaryWriter.Write(Parameters.PasosPorVuelta);
		binaryWriter.Write(Parameters.EspesorPapel);
		binaryWriter.Write(Parameters.Reduccion);
		binaryWriter.Write(Parameters.DistFotocelulaArista);
		binaryWriter.Write(Parameters.AlarmLowLevelLabels);
		binaryWriter.Write(Parameters.AlarmLowLevelRibbon);
		binaryWriter.Write(Parameters.ModuloImpresor);
		binaryWriter.Write(Parameters.ImpDots);
		binaryWriter.Write(Parameters.ImprAnchoCabezal);
		binaryWriter.Write(Parameters.Imp_BTN);
		binaryWriter.Write(Parameters.PrintSpeed);
		binaryWriter.Write(Parameters.LongEtiqueta);
		binaryWriter.Write(Parameters.TipoAplicador);
		binaryWriter.Write(Parameters.Scanner);
		binaryWriter.Write(Parameters.More_IE1);
		binaryWriter.Write(Parameters.More_IE2);
		binaryWriter.Write(Parameters.More_IE3);
		binaryWriter.Write(Parameters.Reimpresion);
		binaryWriter.Write(Parameters.Encoder);
		binaryWriter.Write(Parameters.PasosEncoder);
		binaryWriter.Write(Parameters.More_IE17_IE22);
		binaryWriter.Write(Parameters.More_II3);
		binaryWriter.Write(Parameters.More_FinalCiclo);
		binaryWriter.Write(Parameters.PuertaCabina);
		binaryWriter.Write(Parameters.Vacio_Impr);
		binaryWriter.Write(Parameters.Stopper);
		binaryWriter.Write(Parameters.Gap);
		binaryWriter.Write(Parameters.BufferEtiquetas);
		int num = 0;
		checked
		{
			int num2;
			int num3;
			do
			{
				binaryWriter.Write(Parameters.Timer[num]);
				num++;
				num2 = num;
				num3 = 19;
			}
			while (num2 <= num3);
			num = 0;
			int num4;
			do
			{
				binaryWriter.Write(Parameters.Ports[num]);
				num++;
				num4 = num;
				num3 = 3;
			}
			while (num4 <= num3);
			binaryWriter.Write(Parameters.PortEthernet.Ip);
			binaryWriter.Write(Parameters.PortEthernet.NetMask);
			binaryWriter.Write(Parameters.PortEthernet.GateWay);
			binaryWriter.Write(Parameters.PortEthernet.Enable);
			binaryWriter.Write(Parameters.PortEthernet.RFU);
			binaryWriter.Write(Parameters.PortEthernet.Port);
			binaryWriter.Write(Parameters.NoEtiquetas);
			binaryWriter.Write(Parameters.NoEtiquetasValue);
			binaryWriter.Write(Parameters.L);
			binaryWriter.Write(Parameters.G);
			binaryWriter.Write(Parameters.Teaching);
			binaryWriter.Write(Parameters.MotorStep);
			binaryWriter.Write(Parameters.RelPasmm);
			binaryWriter.Write(Parameters.IsSlave);
			binaryWriter.Write(Parameters.EtiquetasSlave);
			binaryWriter.Write(Parameters.RebobinadoExt);
			binaryWriter.Write(Parameters.RfuAlign);
			binaryWriter.Write(Parameters.FrecuenciaTeaching);
			binaryWriter.Write(Parameters.UseCylinders);
			binaryWriter.Write(Parameters.ModoTrabajo);
			binaryWriter.Write(Parameters.ModoCiclo);
			binaryWriter.Write(Parameters.RfuAlign2);
			binaryWriter.Write(Parameters.EncResolucion);
			binaryWriter.Write(Parameters.Motor.Velocidad);
			binaryWriter.Write(Parameters.Motor.PasosAcel);
			binaryWriter.Write(Parameters.Motor.PasosDecel);
			binaryWriter.Write(Parameters.Motor.FreqInicial);
			binaryWriter.Write(Parameters.Motor.FreqFinal);
			binaryWriter.Write(Parameters.Motor.mmPaso);
			binaryWriter.Write(Parameters.Motor.Offset);
			binaryWriter.Write(Parameters.Motor.RFU);
			binaryWriter.Write(Parameters.MotorRebo.Enabled);
			binaryWriter.Write(Parameters.MotorRebo.ManoEquipo);
			binaryWriter.Write(Parameters.MotorRebo.PasosPorVuelta);
			binaryWriter.Write(Parameters.MotorRebo.EspesorPalet);
			binaryWriter.Write(Parameters.MotorRebo.Reduccion);
			binaryWriter.Write(Parameters.MotorRebo.DistSensRebo);
			binaryWriter.Write(Parameters.MotorRebo.RFU);
			binaryWriter.Write(Parameters.EncDiamRulina);
			binaryWriter.Write(Parameters.ModeHighSpeed);
			binaryWriter.Write(Parameters.LaserCfg);
			binaryWriter.Write(Parameters.ModoProactivo);
			binaryWriter.Write(Parameters.RFU2);
			num = 0;
			int num5;
			do
			{
				binaryWriter.Write(Parameters.RFU[num]);
				num++;
				num5 = num;
				num3 = 17;
			}
			while (num5 <= num3);
			binaryWriter.Write(Parameters.RfuAlign3);
			binaryWriter.Write(Parameters.Chk);
			byte[] array = new byte[(int)(memoryStream.Position - 1) + 1];
			Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Position);
			ushort num6 = 2;
			int num7 = array.Length - 3;
			num = 0;
			while (true)
			{
				int num8 = num;
				num3 = num7;
				if (num8 > num3)
				{
					break;
				}
				num6 = unchecked((ushort)(num6 ^ BitConverter.ToUInt16(array, num)));
				num += 2;
			}
			Parameters.Chk = num6;
			byte[] bytes = BitConverter.GetBytes(Parameters.Chk);
			Array.Copy(bytes, 0, array, array.Length - 2, 2);
			return array;
		}
	}

	public bool WriteCounter(uint Value, COUNTER_TYPES CounterType)
	{
		uint value = Status.ContadorParcial;
		uint value2 = Status.ContadorAcum;
		switch ((int)CounterType)
		{
		case 0:
			value = Value;
			break;
		case 1:
			value2 = Value;
			break;
		case 3:
			value = Value;
			value2 = Value;
			break;
		}
		byte[] array = new byte[9] { 139, 0, 0, 0, 0, 0, 0, 0, 0 };
		Array.Copy(BitConverter.GetBytes(value), 0, array, 1, 4);
		Array.Copy(BitConverter.GetBytes(value2), 0, array, 5, 4);
		return SendCommand(array, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetParameters(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] array = new byte[checked(GetBinParameters().Length + 1)];
		array[0] = 130;
		Array.Copy(GetBinParameters(), 0, array, 1, GetBinParameters().Length);
		return SendCommand(array, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetFamily(byte IdFamily, WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		return SendCommand(new byte[2] { 12, IdFamily }, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetFamily(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] array = new byte[checked(GetBinFamily().Length + 1)];
		array[0] = 140;
		Array.Copy(GetBinFamily(), 0, array, 1, GetBinFamily().Length);
		return SendCommand(array, 1, WAIT_TYPE.Thread, 4000L);
	}

	public bool GetFamilyMulti(byte IdFamily, WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		return SendCommand(new byte[2] { 14, IdFamily }, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetFamilyMulti(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] array = new byte[checked(GetBinFamilyMulti().Length + 1)];
		array[0] = 142;
		Array.Copy(GetBinFamilyMulti(), 0, array, 1, GetBinFamilyMulti().Length);
		return SendCommand(array, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool GetFamilyHS(byte IdFamily, WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		return SendCommand(new byte[2] { 15, IdFamily }, 1, WAIT_TYPE.Thread, 1000L);
	}

	public bool SetFamilyHS(WAIT_TYPE tWaitType = WAIT_TYPE.Thread, long lTimeToWait = 1000L)
	{
		byte[] array = new byte[checked(GetBinFamilyMulti().Length + 1)];
		array[0] = 147;
		Array.Copy(GetBinFamilyMulti(), 0, array, 1, GetBinFamilyMulti().Length);
		return SendCommand(array, 1, WAIT_TYPE.Thread, 1000L);
	}

	public void MLAFamilyRead(object Sender, byte[] Data)
	{
		MapFamily(Data);
	}

	public void MLAFamilyMultiRead(object Sender, byte[] Data)
	{
		MapFamilyMulti(Data);
	}

	public void MLAFamilyHSRead(object Sender, byte[] Data)
	{
		MapFamilyHS(Data);
	}

	private void InitStructStatus()
	{
		Status.Errores = new byte[8];
		Status.Incidencias = new byte[4];
		Status.TimerCiclos = new ushort[9];
		Status.MensajeImpresor = new byte[20];
	}

	public override void MapStatus(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			SizeofStatus = bBuff.Length - 2;
			try
			{
				Status.ConfigVer = binaryReader.ReadByte();
				Status.ConfigRev = binaryReader.ReadByte();
				Status.EtiVer = binaryReader.ReadByte();
				Status.EtiRev = binaryReader.ReadByte();
				Status.ContadorParcial = binaryReader.ReadUInt32();
				Status.ContadorAcum = binaryReader.ReadUInt32();
				Status.ContadorTotal = binaryReader.ReadUInt32();
				Array.Copy(binaryReader.ReadBytes(8), Status.Errores, 8);
				Array.Copy(binaryReader.ReadBytes(4), Status.Incidencias, 4);
				Status.Timer11 = binaryReader.ReadUInt16();
				Status.Timer12 = binaryReader.ReadUInt16();
				Status.Timer13 = binaryReader.ReadUInt16();
				Status.Timer14 = binaryReader.ReadUInt16();
				int num = 0;
				int num2;
				int num3;
				do
				{
					Status.TimerCiclos[num] = binaryReader.ReadUInt16();
					num++;
					num2 = num;
					num3 = 8;
				}
				while (num2 <= num3);
				Status.PrioridadErrores = binaryReader.ReadByte();
				Status.FamiliaActivaHS = binaryReader.ReadByte();
				Status.RFU1 = binaryReader.ReadByte();
				Status.Offline = binaryReader.ReadByte();
				Status.PrioridadIncid = binaryReader.ReadByte();
				Status.FamiliaActivaRebobinado = binaryReader.ReadByte();
				Status.FamiliaActivaMultietiquetas = binaryReader.ReadByte();
				Status.DisplayReset = binaryReader.ReadByte();
				Status.CodigoErrorImpresora = binaryReader.ReadUInt16();
				Array.Copy(binaryReader.ReadBytes(20), Status.MensajeImpresor, 8);
				Status.VelocidadEncoder = binaryReader.ReadUInt16();
				if (Status.Offline == 1)
				{
					OnStopped?.Invoke(this);
				}
				else if (Status.Offline == 0)
				{
					OnRunning?.Invoke(this);
				}
				bool flag = false;
				int num4 = Status.Errores.Length - 1;
				num = 0;
				while (true)
				{
					int num5 = num;
					num3 = num4;
					if (num5 > num3)
					{
						break;
					}
					if (Status.Errores[num] != 0)
					{
						flag = true;
						break;
					}
					num++;
				}
				if (flag)
				{
					string mostPriorityError = GetMostPriorityError(Status.Errores, Status.PrioridadErrores);
					if (Operators.CompareString(mostPriorityError, "", TextCompare: false) != 0)
					{
						EventError(this, Status.PrioridadErrores.ToString(), mostPriorityError, Common.ERROR_TYPE.Error);
					}
					return;
				}
				int num6 = Status.Incidencias.Length - 1;
				num = 0;
				while (true)
				{
					int num7 = num;
					num3 = num6;
					if (num7 > num3)
					{
						break;
					}
					if (Status.Incidencias[num] != 0)
					{
						flag = true;
						break;
					}
					num++;
				}
				if (flag)
				{
					string mostPriorityWarning = GetMostPriorityWarning(Status.Incidencias, Status.PrioridadIncid);
					if (Operators.CompareString(mostPriorityWarning, "", TextCompare: false) != 0)
					{
						EventError(this, Status.PrioridadIncid.ToString(), mostPriorityWarning, Common.ERROR_TYPE.Warning);
					}
				}
				else
				{
					OnLine?.Invoke(this);
				}
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog("MLA: Error al procesar trama de STATUS de la MLA.", TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	private void InitStructParameters()
	{
		Parameters.Timer = new ushort[20];
		Parameters.Ports = new uint[4];
		Parameters.RFU = new ushort[18];
	}

	public override void MapParameters(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			try
			{
				SizeofParameters = bBuff.Length - 2;
				Parameters.Idioma = binaryReader.ReadByte();
				Parameters.ManoEquipo = binaryReader.ReadByte();
				Parameters.PasosPorVuelta = binaryReader.ReadUInt16();
				Parameters.EspesorPapel = binaryReader.ReadUInt32();
				Parameters.Reduccion = binaryReader.ReadUInt32();
				Parameters.DistFotocelulaArista = binaryReader.ReadUInt16();
				Parameters.AlarmLowLevelLabels = binaryReader.ReadByte();
				Parameters.AlarmLowLevelRibbon = binaryReader.ReadByte();
				Parameters.ModuloImpresor = binaryReader.ReadByte();
				Parameters.ImpDots = binaryReader.ReadByte();
				Parameters.ImprAnchoCabezal = binaryReader.ReadByte();
				Parameters.Imp_BTN = binaryReader.ReadByte();
				Parameters.PrintSpeed = binaryReader.ReadUInt16();
				Parameters.LongEtiqueta = binaryReader.ReadUInt16();
				Parameters.TipoAplicador = binaryReader.ReadByte();
				Parameters.Scanner = binaryReader.ReadByte();
				Parameters.More_IE1 = binaryReader.ReadByte();
				Parameters.More_IE2 = binaryReader.ReadByte();
				Parameters.More_IE3 = binaryReader.ReadByte();
				Parameters.Reimpresion = binaryReader.ReadByte();
				Parameters.Encoder = binaryReader.ReadByte();
				Parameters.PasosEncoder = binaryReader.ReadByte();
				Parameters.More_IE17_IE22 = binaryReader.ReadByte();
				Parameters.More_II3 = binaryReader.ReadByte();
				Parameters.More_FinalCiclo = binaryReader.ReadByte();
				Parameters.PuertaCabina = binaryReader.ReadByte();
				Parameters.Vacio_Impr = binaryReader.ReadByte();
				Parameters.Stopper = binaryReader.ReadByte();
				Parameters.Gap = binaryReader.ReadByte();
				Parameters.BufferEtiquetas = binaryReader.ReadByte();
				int num = 0;
				int num2;
				int num3;
				do
				{
					Parameters.Timer[num] = binaryReader.ReadUInt16();
					num++;
					num2 = num;
					num3 = 19;
				}
				while (num2 <= num3);
				num = 0;
				int num4;
				do
				{
					Parameters.Ports[num] = binaryReader.ReadUInt32();
					num++;
					num4 = num;
					num3 = 3;
				}
				while (num4 <= num3);
				Parameters.PortEthernet.Ip = binaryReader.ReadUInt32();
				Parameters.PortEthernet.NetMask = binaryReader.ReadUInt32();
				Parameters.PortEthernet.GateWay = binaryReader.ReadUInt32();
				Parameters.PortEthernet.Enable = binaryReader.ReadByte();
				Parameters.PortEthernet.RFU = binaryReader.ReadByte();
				Parameters.PortEthernet.Port = binaryReader.ReadUInt16();
				Parameters.NoEtiquetas = binaryReader.ReadByte();
				Parameters.NoEtiquetasValue = binaryReader.ReadByte();
				Parameters.L = binaryReader.ReadUInt16();
				Parameters.G = binaryReader.ReadUInt16();
				Parameters.Teaching = binaryReader.ReadByte();
				Parameters.MotorStep = binaryReader.ReadByte();
				Parameters.RelPasmm = binaryReader.ReadUInt32();
				Parameters.IsSlave = binaryReader.ReadByte();
				Parameters.EtiquetasSlave = binaryReader.ReadByte();
				Parameters.RebobinadoExt = binaryReader.ReadByte();
				Parameters.RfuAlign = binaryReader.ReadByte();
				Parameters.FrecuenciaTeaching = binaryReader.ReadUInt16();
				Parameters.UseCylinders = binaryReader.ReadByte();
				Parameters.ModoTrabajo = binaryReader.ReadByte();
				Parameters.ModoCiclo = binaryReader.ReadByte();
				Parameters.RfuAlign2 = binaryReader.ReadByte();
				Parameters.EncResolucion = binaryReader.ReadUInt16();
				Parameters.Motor.Velocidad = binaryReader.ReadUInt32();
				Parameters.Motor.PasosAcel = binaryReader.ReadUInt16();
				Parameters.Motor.PasosDecel = binaryReader.ReadUInt16();
				Parameters.Motor.FreqInicial = binaryReader.ReadUInt16();
				Parameters.Motor.FreqFinal = binaryReader.ReadUInt16();
				Parameters.Motor.mmPaso = binaryReader.ReadUInt32();
				Parameters.Motor.Offset = binaryReader.ReadUInt16();
				Parameters.Motor.RFU = binaryReader.ReadUInt16();
				Parameters.MotorRebo.Enabled = binaryReader.ReadByte();
				Parameters.MotorRebo.ManoEquipo = binaryReader.ReadByte();
				Parameters.MotorRebo.PasosPorVuelta = binaryReader.ReadUInt16();
				Parameters.MotorRebo.EspesorPalet = binaryReader.ReadUInt32();
				Parameters.MotorRebo.Reduccion = binaryReader.ReadUInt32();
				Parameters.MotorRebo.DistSensRebo = binaryReader.ReadUInt16();
				Parameters.MotorRebo.RFU = binaryReader.ReadUInt16();
				Parameters.EncDiamRulina = binaryReader.ReadUInt32();
				Parameters.ModeHighSpeed = binaryReader.ReadByte();
				Parameters.LaserCfg = binaryReader.ReadByte();
				Parameters.ModoProactivo = binaryReader.ReadByte();
				Parameters.RFU2 = binaryReader.ReadByte();
				num = 0;
				int num5;
				do
				{
					Parameters.RFU[num] = binaryReader.ReadUInt16();
					num++;
					num5 = num;
					num3 = 17;
				}
				while (num5 <= num3);
				Parameters.RfuAlign3 = binaryReader.ReadUInt16();
				Parameters.Chk = binaryReader.ReadUInt16();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}
	}

	public override void DetectIOChanges()
	{
		checked
		{
			int num = IO.DIQuantity - 1;
			int num2 = 0;
			while (true)
			{
				int num3 = num2;
				int num4 = num;
				if (num3 > num4)
				{
					break;
				}
				if (IO.DI[num2] != LastIO.DI[num2])
				{
					OnInputChanged?.Invoke(this, unchecked((IO_INPUT_LIST)num2), IO.DI[num2]);
				}
				num2++;
			}
			int num5 = IO.DOQuantity - 1;
			num2 = 0;
			while (true)
			{
				int num6 = num2;
				int num4 = num5;
				if (num6 <= num4)
				{
					if (IO.DO[num2] != LastIO.DO[num2])
					{
						OnOutputChanged?.Invoke(this, unchecked((IO_OUTPUT_LIST)num2), IO.DO[num2]);
					}
					num2++;
					continue;
				}
				break;
			}
		}
	}

	public void InitStructFamily()
	{
		Family.RFU = new ushort[6];
	}

	public void MapFamily(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			try
			{
				SizeofFamily = bBuff.Length - 2;
				Family.Velocidad = binaryReader.ReadUInt32();
				Family.PasosRampaInicial = binaryReader.ReadUInt16();
				Family.PasosRampaFinal = binaryReader.ReadUInt16();
				Family.FreqInicial = binaryReader.ReadUInt16();
				Family.FreqFinal = binaryReader.ReadUInt16();
				Family.InhibicionFotocelula = binaryReader.ReadUInt32();
				Family.NumEtiquetas = binaryReader.ReadUInt32();
				Family.PeelOff = binaryReader.ReadUInt32();
				Family.LongEtiqueta = binaryReader.ReadUInt32();
				Family.Index = binaryReader.ReadByte();
				Family.Activa = binaryReader.ReadByte();
				Family.PasosRampaIniImpresion = binaryReader.ReadUInt16();
				Family.T16RetrasoInicioCiclo = binaryReader.ReadUInt16();
				Family.InhibicionGap = binaryReader.ReadUInt16();
				Family.L = binaryReader.ReadUInt16();
				Family.X = binaryReader.ReadUInt16();
				Family.EncNumEtiquetas = binaryReader.ReadUInt16();
				Family.EncMMPriEti = binaryReader.ReadUInt16();
				Family.EncMMSigEti = binaryReader.ReadUInt16();
				Family.EncMMINiFoto = binaryReader.ReadUInt16();
				Family.Tramo = binaryReader.ReadByte();
				Family.RFU2 = binaryReader.ReadByte();
				int num = 0;
				int num2;
				int num3;
				do
				{
					Family.RFU[num] = binaryReader.ReadUInt16();
					num++;
					num2 = num;
					num3 = 5;
				}
				while (num2 <= num3);
				Family.Chk = binaryReader.ReadUInt16();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}
	}

	public byte[] GetBinFamily()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		memoryStream.Position = 0L;
		binaryWriter.Write(Family.Velocidad);
		binaryWriter.Write(Family.PasosRampaInicial);
		binaryWriter.Write(Family.PasosRampaFinal);
		binaryWriter.Write(Family.FreqInicial);
		binaryWriter.Write(Family.FreqFinal);
		binaryWriter.Write(Family.InhibicionFotocelula);
		binaryWriter.Write(Family.NumEtiquetas);
		binaryWriter.Write(Family.PeelOff);
		binaryWriter.Write(Family.LongEtiqueta);
		binaryWriter.Write(Family.Index);
		binaryWriter.Write(Family.Activa);
		binaryWriter.Write(Family.PasosRampaIniImpresion);
		binaryWriter.Write(Family.T16RetrasoInicioCiclo);
		binaryWriter.Write(Family.InhibicionGap);
		binaryWriter.Write(Family.L);
		binaryWriter.Write(Family.X);
		binaryWriter.Write(Family.EncNumEtiquetas);
		binaryWriter.Write(Family.EncMMPriEti);
		binaryWriter.Write(Family.EncMMSigEti);
		binaryWriter.Write(Family.EncMMINiFoto);
		binaryWriter.Write(Family.Tramo);
		binaryWriter.Write(Family.RFU2);
		int num = 0;
		checked
		{
			int num2;
			int num3;
			do
			{
				binaryWriter.Write(Family.RFU[num]);
				num++;
				num2 = num;
				num3 = 5;
			}
			while (num2 <= num3);
			binaryWriter.Write(Family.Chk);
			byte[] array = new byte[(int)(memoryStream.Position - 1) + 1];
			Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Position);
			ushort num4 = 2;
			int num5 = array.Length - 3;
			num = 0;
			while (true)
			{
				int num6 = num;
				num3 = num5;
				if (num6 > num3)
				{
					break;
				}
				num4 = unchecked((ushort)(num4 ^ BitConverter.ToUInt16(array, num)));
				num += 2;
			}
			Parameters.Chk = num4;
			byte[] bytes = BitConverter.GetBytes(Parameters.Chk);
			Array.Copy(bytes, 0, array, array.Length - 2, 2);
			return array;
		}
	}

	public void InitStructFamilyMulti()
	{
		FamilyMulti.RFU2 = new ushort[4];
	}

	public void MapFamilyMulti(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			try
			{
				SizeofFamilyMulti = bBuff.Length - 2;
				FamilyMulti.Activa = binaryReader.ReadByte();
				FamilyMulti.NumEtisLinea = binaryReader.ReadByte();
				FamilyMulti.NumLineas = binaryReader.ReadByte();
				FamilyMulti.Reverso = binaryReader.ReadByte();
				FamilyMulti.Index = binaryReader.ReadByte();
				FamilyMulti.RFU = binaryReader.ReadByte();
				FamilyMulti.SeparacionLineas = binaryReader.ReadUInt16();
				FamilyMulti.ColaCentrado = binaryReader.ReadUInt16();
				int num = 0;
				int num2;
				int num3;
				do
				{
					FamilyMulti.RFU2[num] = binaryReader.ReadUInt16();
					num++;
					num2 = num;
					num3 = 3;
				}
				while (num2 <= num3);
				FamilyMulti.Chk = binaryReader.ReadUInt16();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}
	}

	public byte[] GetBinFamilyMulti()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		memoryStream.Position = 0L;
		binaryWriter.Write(FamilyMulti.Activa);
		binaryWriter.Write(FamilyMulti.NumEtisLinea);
		binaryWriter.Write(FamilyMulti.NumLineas);
		binaryWriter.Write(FamilyMulti.Reverso);
		binaryWriter.Write(FamilyMulti.Index);
		binaryWriter.Write(FamilyMulti.RFU);
		binaryWriter.Write(FamilyMulti.SeparacionLineas);
		binaryWriter.Write(FamilyMulti.ColaCentrado);
		int num = 0;
		checked
		{
			int num2;
			int num3;
			do
			{
				binaryWriter.Write(FamilyMulti.RFU2[num]);
				num++;
				num2 = num;
				num3 = 3;
			}
			while (num2 <= num3);
			binaryWriter.Write(FamilyMulti.Chk);
			byte[] array = new byte[(int)(memoryStream.Position - 1) + 1];
			Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Position);
			ushort num4 = 2;
			int num5 = array.Length - 3;
			num = 0;
			while (true)
			{
				int num6 = num;
				num3 = num5;
				if (num6 > num3)
				{
					break;
				}
				num4 = unchecked((ushort)(num4 ^ BitConverter.ToUInt16(array, num)));
				num += 2;
			}
			Parameters.Chk = num4;
			byte[] bytes = BitConverter.GetBytes(Parameters.Chk);
			Array.Copy(bytes, 0, array, array.Length - 2, 2);
			return array;
		}
	}

	public void InitStructFamilyHS()
	{
		FamilyHS.RFU2 = new ushort[4];
	}

	public void MapFamilyHS(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			try
			{
				SizeofFamilyHs = bBuff.Length - 2;
				FamilyHS.Index = binaryReader.ReadByte();
				FamilyHS.Activa = binaryReader.ReadByte();
				FamilyHS.RFU = binaryReader.ReadUInt16();
				FamilyHS.DistMacula = binaryReader.ReadUInt16();
				FamilyHS.DistMaculaAplicacion = binaryReader.ReadUInt16();
				FamilyHS.RetardEV = binaryReader.ReadUInt16();
				int num = 0;
				int num2;
				int num3;
				do
				{
					FamilyHS.RFU2[num] = binaryReader.ReadUInt16();
					num++;
					num2 = num;
					num3 = 3;
				}
				while (num2 <= num3);
				FamilyHS.Chk = binaryReader.ReadUInt16();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				ProjectData.ClearProjectError();
			}
		}
	}

	public byte[] GetBinFamilyHS()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		memoryStream.Position = 0L;
		binaryWriter.Write(FamilyHS.Index);
		binaryWriter.Write(FamilyHS.Activa);
		binaryWriter.Write(FamilyHS.RFU);
		binaryWriter.Write(FamilyHS.DistMacula);
		binaryWriter.Write(FamilyHS.DistMaculaAplicacion);
		binaryWriter.Write(FamilyHS.RetardEV);
		int num = 0;
		checked
		{
			int num2;
			int num3;
			do
			{
				binaryWriter.Write(FamilyHS.RFU2[num]);
				num++;
				num2 = num;
				num3 = 3;
			}
			while (num2 <= num3);
			binaryWriter.Write(FamilyHS.Chk);
			byte[] array = new byte[(int)(memoryStream.Position - 1) + 1];
			Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Position);
			ushort num4 = 2;
			int num5 = array.Length - 3;
			num = 0;
			while (true)
			{
				int num6 = num;
				num3 = num5;
				if (num6 > num3)
				{
					break;
				}
				num4 = unchecked((ushort)(num4 ^ BitConverter.ToUInt16(array, num)));
				num += 2;
			}
			Parameters.Chk = num4;
			byte[] bytes = BitConverter.GetBytes(Parameters.Chk);
			Array.Copy(bytes, 0, array, array.Length - 2, 2);
			return array;
		}
	}
}
