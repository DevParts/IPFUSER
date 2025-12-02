using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

namespace MacsaDevicesNet.Applicators;

public class MLP : Magnetiq
{
	public enum ID_TIMERS
	{
		TIMER_T1,
		TIMER_T2,
		TIMER_T3,
		TIMER_T4,
		TIMER_T5,
		TIMER_T6,
		TIMER_T7,
		TIMER_T8,
		TIMER_T9,
		TIMER_T10,
		TIMER_T13,
		TIMER_RFU1,
		TIMER_RFU2,
		TIMER_T15,
		TIMER_T16,
		TIMER_TOE4,
		TIMER_TIE1,
		TIMER_TVacio,
		TIMER_TInhibitIE1,
		TIMER_TInhibitIE2
	}

	public enum IO_INPUT_LIST
	{
		IE1_PRESENCIA1,
		IE2_PRESENCIA2,
		IE3_APLICACION,
		IE4_REIMPRESION,
		IE5_DETECCION_PAD,
		IE6_PARO_EMERGENCIA,
		IE7_REARME_MARCHA,
		IE8_TBD,
		IE9_ENTRADA1,
		IE10_ENTRADA2,
		IE11_ENTRADA3,
		IE12_NUM_ETIQ_BCD1,
		IE13_NUM_ETIQ_BCD2,
		IE14_NUM_ETIQ_BCD3,
		IE15_NUM_ETIQ_BCD4,
		IE16_TBD,
		IE17_PROCEDENCIA_LINEA_BCD1,
		IE18_PROCEDENCIA_LINEA_BCD2,
		IE19_PROCEDENCIA_LINEA_BCD3,
		IE20_PROCEDENCIA_LINEA_BCD4,
		IE21_PICO,
		IE22_STROBE_DATOS,
		IE23_TBD,
		IE24_TBD,
		II1_LABEL_ENDING,
		II2_RIBBON_ENDING,
		II3_PAPEL_ONPAD,
		II4_SENSOR_HALL1,
		II5_SENSOR_HALL2,
		II6_COMPENSADOR_DESBOBINADOR,
		II7_CILINDRO1_MEDIO_RECORRIDO,
		II8_CILINDRO1_HOMEI,
		II9_CILINDRO2_HOME,
		II10_CILINDRO3_HOME,
		II11_PAPEL_SOPORTE_RECOGIDO,
		II12_PRESENCIA_ETIQUETAS,
		II13_CILINDRO1_TOPE,
		II14_CILINDRO2_TOPE,
		II15_CILINDRO3_TOPE,
		II16_CILINDRO4_TOPE,
		II17_CILINDRO5_TOPE,
		II18_CILINDRO4_HOME,
		II19_CILINDRO5_HOME,
		II20_SENSOR_PAD,
		II21_PUERTA_CABINA_ABIERTA,
		II22_LECTURA_IMPRESION_OK,
		II23_LIBRE,
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
		OE2_DATOS_COLA_IMPRESION,
		OE3_CILINDROS_HOME,
		OE4_FINAL_CICLO,
		OE5_LABELS_RIBBON_ENDING,
		OE6_ERROR_MLP_PRINTER,
		OE7_LIBRE,
		OE8_SALIDA_X,
		OE9_SALIDA_Y,
		OE10_SALIDA_11,
		OE11_MODOAVERIA_ACTIVO,
		OE12_ALARMA_ACTIVA,
		OI1_TUBLO_SOPLADO,
		OI2_VACIO,
		OI3_EYECCION,
		OI4_CILINDRO1_BOBINA_I,
		OI5_CILINDRO1_BOBINA_II,
		OI6_CILINDRO2,
		OI7_CILINDRO3,
		OI8_CILINDRO4,
		OI9_CILINDRO5,
		OI10_STOPPER,
		OI11_LIBRE,
		OI12_LIBRE,
		OI13_MOTOR_CORREAS,
		OI14_AUXILIAR1,
		OI15_AUXILIAR2,
		OI16_BALIZA1_VERDE,
		OI17_BALIZA1_AZUL,
		OI18_BALIZA1_ROJO,
		OI19_ACTIVACION_CILINDRO_PUERTA,
		OI20_ACTIVACION_SCANNER,
		OI21_LIBRE,
		OI22_LIBRE,
		MIO1_ORDEN_IMPRESION,
		MIO2_CALCULO_LONG_ETIQUETA,
		MIO3_PAUSE,
		MIO4_REIMPRESION,
		MIO5_INDEFINIDA
	}

	public struct MLPPARAMSDATA
	{
		public byte Idioma;

		public byte ManoEquipo;

		public ushort PasosPorVuelta;

		public uint EspesorPapel;

		public uint Reduccion;

		public ushort DistSensorRebob;

		public byte AlarmLowLabel;

		public byte AlarmLowRibbon;

		public byte ModuloImpresor;

		public byte Dots;

		public byte PrintheadWidth;

		public byte ImpresoraBTN;

		public ushort PrintSpeed;

		public ushort LongEtiqueta;

		public byte Applicator;

		public byte Scanner;

		public byte MORE_IE1;

		public byte MORE_IE2;

		public byte RFU;

		public byte ReImpresion;

		public byte MORE_IE9_IE11;

		public byte MORE_IE12_IE15;

		public byte MORE_IE17_IE22;

		public byte MORE_II3;

		public byte MORE_OE4;

		public byte PuertaCabina;

		public byte VacioDuranteImpresion;

		public byte Stopper;

		public byte ManipuladorParadas;

		public byte TipoManipulador;

		public ushort[] Timer;

		public uint[] Ports;

		public uint PortImpresora;

		public PORT_ETHERNET PortEthernet;

		public byte Cintas;

		public byte More_MII6;

		public byte ModoTrabajo;

		public byte OpcionesProducto;

		public byte ErrorParo;

		public byte ModoCiclo;

		public byte IsSlave;

		public byte EtiquetasSlave;

		public byte Lineal3;

		public byte SelectFrontalLineal;

		public byte Proactivo;

		public byte[] RFU2;

		public ushort[] RFU3;

		public ushort Chk;
	}

	public struct MLPSTATUSDATA
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

		public byte PrioridadErrores;

		public byte SM130Ver;

		public byte Offline;

		public byte PrioridadIncid;

		public byte DisplayReset;

		public byte RFU;

		public ushort Chk;
	}

	public delegate void OnLineEventHandler(object Sender);

	public delegate void OnStoppedEventHandler(object sender);

	public delegate void OnRunningEventHandler(object sender);

	public delegate void OnInputChangedEventHandler(object sender, IO_INPUT_LIST Input, ML_IO_STATE State);

	public delegate void OnOutputChangedEventHandler(object sender, IO_OUTPUT_LIST Output, ML_IO_STATE State);

	private static List<WeakReference> __ENCList = new List<WeakReference>();

	private const int CONF_NUM_TIMERS_PARAMETROS = 20;

	private const int CONF_NUM_PORTS = 6;

	public MLPSTATUSDATA Status;

	public MLPPARAMSDATA Parameters;

	private int SizeOfParameters;

	private int SizeOfStatus;

	private object m_oMLParameters;

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
		binaryWriter.Write(Parameters.DistSensorRebob);
		binaryWriter.Write(Parameters.AlarmLowLabel);
		binaryWriter.Write(Parameters.AlarmLowRibbon);
		binaryWriter.Write(Parameters.ModuloImpresor);
		binaryWriter.Write(Parameters.Dots);
		binaryWriter.Write(Parameters.PrintheadWidth);
		binaryWriter.Write(Parameters.ImpresoraBTN);
		binaryWriter.Write(Parameters.PrintSpeed);
		binaryWriter.Write(Parameters.LongEtiqueta);
		binaryWriter.Write(Parameters.Applicator);
		binaryWriter.Write(Parameters.Scanner);
		binaryWriter.Write(Parameters.MORE_IE1);
		binaryWriter.Write(Parameters.MORE_IE2);
		binaryWriter.Write(Parameters.RFU);
		binaryWriter.Write(Parameters.ReImpresion);
		binaryWriter.Write(Parameters.MORE_IE9_IE11);
		binaryWriter.Write(Parameters.MORE_IE12_IE15);
		binaryWriter.Write(Parameters.MORE_IE17_IE22);
		binaryWriter.Write(Parameters.MORE_II3);
		binaryWriter.Write(Parameters.MORE_OE4);
		binaryWriter.Write(Parameters.PuertaCabina);
		binaryWriter.Write(Parameters.VacioDuranteImpresion);
		binaryWriter.Write(Parameters.Stopper);
		binaryWriter.Write(Parameters.ManipuladorParadas);
		binaryWriter.Write(Parameters.TipoManipulador);
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
				num3 = 5;
			}
			while (num4 <= num3);
			binaryWriter.Write(Parameters.PortImpresora);
			binaryWriter.Write(Parameters.PortEthernet.Ip);
			binaryWriter.Write(Parameters.PortEthernet.NetMask);
			binaryWriter.Write(Parameters.PortEthernet.GateWay);
			binaryWriter.Write(Parameters.PortEthernet.Enable);
			binaryWriter.Write(Parameters.PortEthernet.RFU);
			binaryWriter.Write(Parameters.PortEthernet.Port);
			binaryWriter.Write(Parameters.Cintas);
			binaryWriter.Write(Parameters.More_MII6);
			binaryWriter.Write(Parameters.ModoTrabajo);
			binaryWriter.Write(Parameters.OpcionesProducto);
			binaryWriter.Write(Parameters.ErrorParo);
			binaryWriter.Write(Parameters.ModoCiclo);
			binaryWriter.Write(Parameters.IsSlave);
			binaryWriter.Write(Parameters.EtiquetasSlave);
			binaryWriter.Write(Parameters.Lineal3);
			binaryWriter.Write(Parameters.SelectFrontalLineal);
			binaryWriter.Write(Parameters.Proactivo);
			num = 0;
			int num5;
			do
			{
				binaryWriter.Write(Parameters.RFU2[num]);
				num++;
				num5 = num;
				num3 = 2;
			}
			while (num5 <= num3);
			num = 0;
			int num6;
			do
			{
				binaryWriter.Write(Parameters.RFU3[num]);
				num++;
				num6 = num;
				num3 = 51;
			}
			while (num6 <= num3);
			binaryWriter.Write(Parameters.Chk);
			byte[] array = new byte[(int)(memoryStream.Position - 1) + 1];
			Array.Copy(memoryStream.GetBuffer(), array, memoryStream.Position);
			ushort num7 = 2;
			int num8 = array.Length - 3;
			num = 0;
			while (true)
			{
				int num9 = num;
				num3 = num8;
				if (num9 > num3)
				{
					break;
				}
				num7 = unchecked((ushort)(num7 ^ BitConverter.ToUInt16(array, num)));
				num += 2;
			}
			Parameters.Chk = num7;
			byte[] bytes = BitConverter.GetBytes(Parameters.Chk);
			Array.Copy(bytes, 0, array, array.Length - 2, 2);
			return array;
		}
	}

	public MLP()
	{
		__ENCAddToList(this);
		InitStructSts();
		InitStructParameters();
	}

	public string GetMostPriorityError(byte[] bytesError, byte bytePrioridad)
	{
		string result = "";
		string text = "";
		byte b = (byte)((uint)(bytePrioridad & 0xF0) >> 4);
		byte b2 = (byte)(bytePrioridad & 0xF);
		switch (b)
		{
		case 0:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP1"), 
				1 => Common.Rm.GetString("MLP2"), 
				2 => Common.Rm.GetString("MLP3"), 
				3 => Common.Rm.GetString("MLP4"), 
				4 => Common.Rm.GetString("MLP5"), 
				5 => Common.Rm.GetString("MLP6"), 
				6 => Common.Rm.GetString("MLP7"), 
				7 => Common.Rm.GetString("MLP8"), 
				_ => Common.Rm.GetString("MLP39"), 
			};
			break;
		case 1:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP9"), 
				1 => Common.Rm.GetString("MLP10"), 
				2 => Common.Rm.GetString("MLP11"), 
				3 => Common.Rm.GetString("MLP12"), 
				4 => Common.Rm.GetString("MLP13"), 
				5 => Common.Rm.GetString("MLP14"), 
				6 => Common.Rm.GetString("MLP15"), 
				7 => Common.Rm.GetString("MLP16"), 
				_ => Common.Rm.GetString("MLP40"), 
			};
			break;
		case 2:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP17"), 
				1 => Common.Rm.GetString("MLP18"), 
				2 => Common.Rm.GetString("MLP19"), 
				3 => Common.Rm.GetString("MLP20"), 
				4 => Common.Rm.GetString("MLP21"), 
				5 => Common.Rm.GetString("MLP22"), 
				6 => Common.Rm.GetString("MLP23"), 
				7 => Common.Rm.GetString("MLP24"), 
				_ => Common.Rm.GetString("MLP40"), 
			};
			break;
		case 3:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP25"), 
				1 => Common.Rm.GetString("MLP26"), 
				2 => Common.Rm.GetString("MLP27"), 
				3 => Common.Rm.GetString("MLP28"), 
				4 => Common.Rm.GetString("MLP29"), 
				5 => Common.Rm.GetString("MLP30"), 
				6 => Common.Rm.GetString("MLP31"), 
				7 => Common.Rm.GetString("MLP32"), 
				_ => Common.Rm.GetString("MLP40"), 
			};
			break;
		case 4:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP33"), 
				1 => Common.Rm.GetString("MLP34"), 
				2 => Common.Rm.GetString("MLP35"), 
				3 => Common.Rm.GetString("MLP36"), 
				4 => Common.Rm.GetString("MLP37"), 
				5 => Common.Rm.GetString("MLP38"), 
				6 => Common.Rm.GetString("MLP46"), 
				7 => Common.Rm.GetString("MLP47"), 
				_ => Common.Rm.GetString("MLP39"), 
			};
			break;
		case 5:
			result = b2 switch
			{
				0 => Common.Rm.GetString("MLP48"), 
				1 => Common.Rm.GetString("MLP49"), 
				_ => Common.Rm.GetString("MLP39"), 
			};
			break;
		case 6:
			result = Common.Rm.GetString("MLP39");
			break;
		case 7:
			if (b2 == 0)
			{
				result = Common.Rm.GetString("MLP41");
			}
			else if (b2 == 1)
			{
				result = Common.Rm.GetString("MLP42");
			}
			else if (b2 == 2)
			{
				result = Common.Rm.GetString("MLP43");
			}
			else if (b2 == 3)
			{
				result = Common.Rm.GetString("MLP44");
			}
			else if (b2 == 4)
			{
				result = Common.Rm.GetString("MLP45");
			}
			else if (b2 > 5)
			{
				result = Common.Rm.GetString("MLP39");
			}
			break;
		}
		return result;
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
				0 => Common.Rm.GetString("MLP58"), 
				1 => Common.Rm.GetString("MLP59"), 
				2 => Common.Rm.GetString("MLP60"), 
				3 => Common.Rm.GetString("MLP61"), 
				4 => Common.Rm.GetString("MLP62"), 
				5 => Common.Rm.GetString("MLP63"), 
				6 => Common.Rm.GetString("MLP64"), 
				7 => Common.Rm.GetString("MLP65"), 
				_ => Common.Rm.GetString("MLP39"), 
			};
		}
		return Common.Rm.GetString("MLP39");
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
				SizeOfParameters = bBuff.Length - 2;
				Parameters.Idioma = binaryReader.ReadByte();
				Parameters.ManoEquipo = binaryReader.ReadByte();
				Parameters.PasosPorVuelta = binaryReader.ReadUInt16();
				Parameters.EspesorPapel = binaryReader.ReadUInt32();
				Parameters.Reduccion = binaryReader.ReadUInt32();
				Parameters.DistSensorRebob = binaryReader.ReadUInt16();
				Parameters.AlarmLowLabel = binaryReader.ReadByte();
				Parameters.AlarmLowRibbon = binaryReader.ReadByte();
				Parameters.ModuloImpresor = binaryReader.ReadByte();
				Parameters.Dots = binaryReader.ReadByte();
				Parameters.PrintheadWidth = binaryReader.ReadByte();
				Parameters.ImpresoraBTN = binaryReader.ReadByte();
				Parameters.PrintSpeed = binaryReader.ReadUInt16();
				Parameters.LongEtiqueta = binaryReader.ReadUInt16();
				Parameters.Applicator = binaryReader.ReadByte();
				Parameters.Scanner = binaryReader.ReadByte();
				Parameters.MORE_IE1 = binaryReader.ReadByte();
				Parameters.MORE_IE2 = binaryReader.ReadByte();
				Parameters.RFU = binaryReader.ReadByte();
				Parameters.ReImpresion = binaryReader.ReadByte();
				Parameters.MORE_IE9_IE11 = binaryReader.ReadByte();
				Parameters.MORE_IE12_IE15 = binaryReader.ReadByte();
				Parameters.MORE_IE17_IE22 = binaryReader.ReadByte();
				Parameters.MORE_II3 = binaryReader.ReadByte();
				Parameters.MORE_OE4 = binaryReader.ReadByte();
				Parameters.PuertaCabina = binaryReader.ReadByte();
				Parameters.VacioDuranteImpresion = binaryReader.ReadByte();
				Parameters.Stopper = binaryReader.ReadByte();
				Parameters.ManipuladorParadas = binaryReader.ReadByte();
				Parameters.TipoManipulador = binaryReader.ReadByte();
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
					num3 = 5;
				}
				while (num4 <= num3);
				Parameters.PortImpresora = binaryReader.ReadUInt32();
				Parameters.PortEthernet.Ip = binaryReader.ReadUInt32();
				Parameters.PortEthernet.NetMask = binaryReader.ReadUInt32();
				Parameters.PortEthernet.GateWay = binaryReader.ReadUInt32();
				Parameters.PortEthernet.Enable = binaryReader.ReadByte();
				Parameters.PortEthernet.RFU = binaryReader.ReadByte();
				Parameters.PortEthernet.Port = binaryReader.ReadUInt16();
				Parameters.Cintas = binaryReader.ReadByte();
				Parameters.More_MII6 = binaryReader.ReadByte();
				Parameters.ModoTrabajo = binaryReader.ReadByte();
				Parameters.OpcionesProducto = binaryReader.ReadByte();
				Parameters.ErrorParo = binaryReader.ReadByte();
				Parameters.ModoCiclo = binaryReader.ReadByte();
				Parameters.IsSlave = binaryReader.ReadByte();
				Parameters.EtiquetasSlave = binaryReader.ReadByte();
				Parameters.Lineal3 = binaryReader.ReadByte();
				Parameters.SelectFrontalLineal = binaryReader.ReadByte();
				Parameters.Proactivo = binaryReader.ReadByte();
				num = 0;
				int num5;
				do
				{
					Parameters.RFU2[num] = binaryReader.ReadByte();
					num++;
					num5 = num;
					num3 = 2;
				}
				while (num5 <= num3);
				num = 0;
				int num6;
				do
				{
					Parameters.RFU3[num] = binaryReader.ReadUInt16();
					num++;
					num6 = num;
					num3 = 51;
				}
				while (num6 <= num3);
				Parameters.Chk = binaryReader.ReadUInt16();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				Exception ex2 = ex;
				Common.MACSALog("ML: Error al procesar PARAMETROS.", TraceEventType.Error, ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
	}

	public override void MapStatus(byte[] bBuff)
	{
		MemoryStream input = new MemoryStream(bBuff);
		BinaryReader binaryReader = new BinaryReader(input);
		binaryReader.ReadByte();
		binaryReader.ReadByte();
		checked
		{
			SizeOfStatus = bBuff.Length - 2;
			try
			{
				Status.ConfigVer = binaryReader.ReadByte();
				Status.ConfigRev = binaryReader.ReadByte();
				Status.EtiVer = binaryReader.ReadByte();
				Status.EtiRev = binaryReader.ReadByte();
				Status.ContadorParcial = binaryReader.ReadUInt32();
				Status.ContadorAcum = binaryReader.ReadUInt32();
				Status.ContadorTotal = binaryReader.ReadUInt32();
				int num = 0;
				int num2;
				int num3;
				do
				{
					Status.Errores[num] = binaryReader.ReadByte();
					num++;
					num2 = num;
					num3 = 7;
				}
				while (num2 <= num3);
				num = 0;
				int num4;
				do
				{
					Status.Incidencias[num] = binaryReader.ReadByte();
					num++;
					num4 = num;
					num3 = 3;
				}
				while (num4 <= num3);
				Status.Timer11 = binaryReader.ReadUInt16();
				Status.Timer12 = binaryReader.ReadUInt16();
				Status.Timer13 = binaryReader.ReadUInt16();
				Status.Timer14 = binaryReader.ReadUInt16();
				Status.PrioridadErrores = binaryReader.ReadByte();
				Status.SM130Ver = binaryReader.ReadByte();
				Status.Offline = binaryReader.ReadByte();
				Status.PrioridadIncid = binaryReader.ReadByte();
				Status.DisplayReset = binaryReader.ReadByte();
				Status.RFU = binaryReader.ReadByte();
				Status.Chk = binaryReader.ReadUInt16();
				bool flag = false;
				if (Status.Offline == 1)
				{
					OnStopped?.Invoke(this);
				}
				else if (Status.Offline == 0)
				{
					OnRunning?.Invoke(this);
				}
				int num5 = Status.Errores.Length - 1;
				num = 0;
				while (true)
				{
					int num6 = num;
					num3 = num5;
					if (num6 > num3)
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
				int num7 = Status.Incidencias.Length - 1;
				num = 0;
				while (true)
				{
					int num8 = num;
					num3 = num7;
					if (num8 > num3)
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
				Common.MACSALog("MLP: Error al procesar trama de STATUS de la MLP.", TraceEventType.Error, ex2.Message);
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

	private void InitStructSts()
	{
		Status.Errores = new byte[8];
		Status.Incidencias = new byte[4];
	}

	private void InitStructParameters()
	{
		Parameters.Timer = new ushort[20];
		Parameters.Ports = new uint[6];
		Parameters.RFU2 = new byte[3];
		Parameters.RFU3 = new ushort[52];
	}

	public ushort GetTimerValue(ID_TIMERS eTimerId)
	{
		return Parameters.Timer[(int)eTimerId];
	}
}
