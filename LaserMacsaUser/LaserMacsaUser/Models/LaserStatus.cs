using System.Collections.Generic;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Estado del láser (basado en CSStatusExt de SocketCommNet)
    /// </summary>
    public class LaserStatus
    {
        /// <summary>
        /// Contador de impresiones OK (reseteado cuando cambia el mensaje)
        /// </summary>
        public uint OkCounter { get; set; }

        /// <summary>
        /// Contador de impresiones NOK (reseteado cuando cambia el mensaje)
        /// </summary>
        public uint NokCounter { get; set; }

        /// <summary>
        /// Contador total de impresiones
        /// </summary>
        public uint TotalCounter { get; set; }

        /// <summary>
        /// Estado de impresión: 0 = imprimiendo, 1 = detenido
        /// BIT0: printing loop (preparado para imprimir)
        /// BIT1: printing (estamos marcando actualmente)
        /// </summary>
        public byte Start { get; set; }

        /// <summary>
        /// Nombre del archivo activo (máximo 16 caracteres)
        /// </summary>
        public string MessageName { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del event handler activo
        /// </summary>
        public string EventHandler { get; set; } = string.Empty;

        /// <summary>
        /// Número de puerto de mensaje actual
        /// </summary>
        public uint MessagePort { get; set; }

        /// <summary>
        /// Número de copias a imprimir
        /// </summary>
        public uint Copies { get; set; }

        /// <summary>
        /// Código de alarma
        /// </summary>
        public uint AlarmCode { get; set; }

        /// <summary>
        /// Máscara de alarmas 1
        /// </summary>
        public uint AlarmMask1 { get; set; }

        /// <summary>
        /// Máscara de alarmas 2 (alarmas extendidas)
        /// </summary>
        public uint AlarmMask2 { get; set; }

        /// <summary>
        /// Estado de señales IO
        /// </summary>
        public uint SignalState { get; set; }

        /// <summary>
        /// Información extra (uso dinámico de scanfield, modo dinámico)
        /// </summary>
        public uint Extra { get; set; }

        /// <summary>
        /// Número de elementos en el buffer del láser
        /// </summary>
        public int BufferCount { get; set; }

        /// <summary>
        /// Indica si el láser está conectado
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Lista de códigos de alarma activos
        /// </summary>
        public List<int> AlarmCodes { get; set; } = new List<int>();

        /// <summary>
        /// Indica si el sistema está imprimiendo
        /// </summary>
        public bool IsPrinting => Start == 0;

        /// <summary>
        /// Indica si el sistema está detenido
        /// </summary>
        public bool IsStopped => Start == 1;
    }
}