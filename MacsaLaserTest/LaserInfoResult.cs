using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// Modelo de datos que contiene toda la información obtenida de la impresora
    /// Estructura modular para facilitar el acceso a los datos
    /// </summary>
    public class LaserInfoResult
    {
        // Información básica
        public bool Exito { get; set; }
        public string MensajeError { get; set; }

        // Información de versión
        public int VersionDll { get; set; }
        public UInt16 VersionProtocolo { get; set; }
        public string VersionFirmware { get; set; }

        // Estado de la impresora
        public string MensajeActivo { get; set; }
        public string EventHandler { get; set; }
        public uint ContadorTotal { get; set; }
        public uint ContadorOK { get; set; }
        public uint ContadorNOK { get; set; }
        public uint ContadorCopias { get; set; }
        public string EstadoImpresion { get; set; }  // "Imprimiendo" o "Detenido"
        public uint CodigoAlarma { get; set; }

        // Información del sistema
        public float HorasTrabajo { get; set; }
        public ulong ContadorTotalLargo { get; set; }
        public uint TemperaturaCPU { get; set; }  // En 1/1000 Celsius

        // Espacio en disco
        public ulong EspacioTotalDisco { get; set; }      // bytes
        public ulong EspacioDisponibleDisco { get; set; } // bytes
        public ulong EspacioTotalRAM { get; set; }        // bytes
        public ulong EspacioDisponibleRAM { get; set; }    // bytes

        // Información de temperatura y hardware
        public uint TemperaturaBoard { get; set; }  // En 1/1000 Celsius
        public uint Humedad { get; set; }           // En 1/1000 porcentaje
        public uint Voltaje5V { get; set; }         // En 1/1000 Volt
        public uint Voltaje3_3V { get; set; }       // En 1/1000 Volt
        public uint TemperaturaFanLocal { get; set; }    // En 1/10 Celsius
        public uint TemperaturaFanRemoto { get; set; }  // En 1/10 Celsius
        public uint VelocidadFan { get; set; }      // Tacho en cps

        // Lista de archivos
        public string[] Archivos { get; set; }

        // Datos de conexión
        public byte LeadingByte { get; set; }
        public byte ControlByte { get; set; }
        public UInt16 FoundMask { get; set; }

        public LaserInfoResult()
        {
            Exito = false;
            MensajeError = "";
            Archivos = new string[0];
        }

        /// <summary>
        /// Muestra toda la información en formato legible
        /// </summary>
        public void MostrarInformacion()
        {
            Console.WriteLine("\n==========================================");
            Console.WriteLine("  INFORMACIÓN DE LA IMPRESORA");
            Console.WriteLine("==========================================");

            if (!Exito)
            {
                Console.WriteLine($"Error: {MensajeError}");
                return;
            }

            Console.WriteLine("\n--- VERSIÓN ---");
            Console.WriteLine($"  DLL: {VersionDll}");
            Console.WriteLine($"  Protocolo: {VersionProtocolo}");
            Console.WriteLine($"  Firmware: {VersionFirmware}");

            Console.WriteLine("\n--- ESTADO ACTUAL ---");
            Console.WriteLine($"  Mensaje Activo: {MensajeActivo}");
            Console.WriteLine($"  Event Handler: {EventHandler}");
            Console.WriteLine($"  Estado: {EstadoImpresion}");
            Console.WriteLine($"  Copias a imprimir: {ContadorCopias}");

            Console.WriteLine("\n--- CONTADORES ---");
            Console.WriteLine($"  Total: {ContadorTotal:N0}");
            Console.WriteLine($"  OK: {ContadorOK:N0}");
            Console.WriteLine($"  NOK: {ContadorNOK:N0}");
            Console.WriteLine($"  Total (largo): {ContadorTotalLargo:N0}");

            if (CodigoAlarma != 0)
            {
                Console.WriteLine($"  Advertencia - Código de Alarma: 0x{CodigoAlarma:X8}");
            }

            Console.WriteLine("\n--- SISTEMA ---");
            Console.WriteLine($"  Horas de Trabajo: {HorasTrabajo:F2}");
            Console.WriteLine($"  Temperatura CPU: {TemperaturaCPU / 1000.0:F2} °C");

            Console.WriteLine("\n--- ALMACENAMIENTO ---");
            Console.WriteLine($"  Disco - Total: {EspacioTotalDisco / (1024.0 * 1024.0):F2} MB");
            Console.WriteLine($"  Disco - Disponible: {EspacioDisponibleDisco / (1024.0 * 1024.0):F2} MB");
            Console.WriteLine($"  RAM - Total: {EspacioTotalRAM / (1024.0 * 1024.0):F2} MB");
            Console.WriteLine($"  RAM - Disponible: {EspacioDisponibleRAM / (1024.0 * 1024.0):F2} MB");

            Console.WriteLine("\n--- HARDWARE ---");
            Console.WriteLine($"  Temp. Board: {TemperaturaBoard / 1000.0:F2} °C");
            Console.WriteLine($"  Humedad: {Humedad / 1000.0:F2} %");
            Console.WriteLine($"  Voltaje 5V: {Voltaje5V / 1000.0:F3} V");
            Console.WriteLine($"  Voltaje 3.3V: {Voltaje3_3V / 1000.0:F3} V");
            Console.WriteLine($"  Temp. Fan Local: {TemperaturaFanLocal / 10.0:F1} °C");
            Console.WriteLine($"  Temp. Fan Remoto: {TemperaturaFanRemoto / 10.0:F1} °C");
            Console.WriteLine($"  Velocidad Fan: {VelocidadFan} cps");

            if (Archivos != null && Archivos.Length > 0)
            {
                Console.WriteLine("\n--- ARCHIVOS EN LA IMPRESORA ---");
                foreach (var archivo in Archivos)
                {
                    if (!string.IsNullOrWhiteSpace(archivo))
                    {
                        Console.WriteLine($"  - {archivo}");
                    }
                }
            }

            Console.WriteLine("\n--- DATOS DE CONEXIÓN ---");
            Console.WriteLine($"  Leading Byte: 0x{LeadingByte:X2}");
            Console.WriteLine($"  Control Byte: 0x{ControlByte:X2}");
            Console.WriteLine($"  Found Mask: 0x{FoundMask:X4}");

            Console.WriteLine("\n==========================================\n");
        }
    }
}

