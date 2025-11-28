using System;
using SocketCommNet;

namespace MacsaLaserTest
{
    /// <summary>
    /// Clase modular para obtener información de la impresora laser Macsa
    /// Aísla toda la lógica de obtención de información para facilitar el debugging
    /// </summary>
    public class LaserInfo
    {
        private LaserConnection _conexion;

        public LaserInfo(LaserConnection conexion)
        {
            if (conexion == null)
                throw new ArgumentNullException(nameof(conexion));
            
            _conexion = conexion;
        }

        /// <summary>
        /// Obtiene toda la información disponible de la impresora
        /// </summary>
        /// <returns>Objeto LaserInfoResult con toda la información</returns>
        public LaserInfoResult ObtenerTodaLaInformacion()
        {
            var resultado = new LaserInfoResult();

            try
            {
                // Verificar conexión
                if (!_conexion.IsConnected)
                {
                    resultado.Exito = false;
                    resultado.MensajeError = "No hay conexión con la impresora";
                    return resultado;
                }

                Int32 puntero = _conexion.Puntero;
                SocketComm socketComm = _conexion.SocketComm;

                // 1. Obtener versión de la DLL
                resultado.VersionDll = socketComm.CS_GetDllVersion();

                // 2. Obtener versión del protocolo
                resultado.VersionProtocolo = socketComm.CS_GetVersion(puntero);

                // 3. Obtener versión del firmware
                string versionFirmware = "";
                Int32 err = socketComm.CS_GetVersionString(puntero, ref versionFirmware, 0);
                if (err == 0)
                {
                    resultado.VersionFirmware = versionFirmware.Trim();
                }

                // 4. Obtener estado extendido
                ObtenerEstadoExtendido(socketComm, puntero, resultado);

                // 5. Obtener información del sistema
                ObtenerInformacionSistema(socketComm, puntero, resultado);

                // 6. Obtener información de temperatura y hardware
                ObtenerInformacionHardware(socketComm, puntero, resultado);

                // 7. Obtener lista de archivos
                ObtenerListaArchivos(socketComm, puntero, resultado);

                // 8. Obtener datos de conexión
                ObtenerDatosConexion(socketComm, puntero, resultado);

                resultado.Exito = true;
                resultado.MensajeError = "";
            }
            catch (Exception ex)
            {
                resultado.Exito = false;
                resultado.MensajeError = $"Excepción al obtener información: {ex.Message}";
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el estado extendido de la impresora
        /// </summary>
        private void ObtenerEstadoExtendido(SocketComm socketComm, Int32 puntero, LaserInfoResult resultado)
        {
            try
            {
                SocketComm.CSStatusExt estado = new SocketComm.CSStatusExt();
                Int32 err = socketComm.CS_StatusExt(puntero, ref estado);

                if (err == 0)
                {
                    resultado.MensajeActivo = estado.messagename ?? "";
                    resultado.EventHandler = estado.eventhandler ?? "";
                    resultado.ContadorTotal = estado.t_counter;
                    resultado.ContadorOK = estado.d_counter;
                    resultado.ContadorNOK = estado.s_counter;
                    resultado.ContadorCopias = estado.m_copies;
                    resultado.CodigoAlarma = estado.err;
                    
                    // Estado de impresión
                    if ((estado.Start & 0x01) != 0)
                        resultado.EstadoImpresion = "Preparado para imprimir";
                    else if ((estado.Start & 0x02) != 0)
                        resultado.EstadoImpresion = "Imprimiendo";
                    else
                        resultado.EstadoImpresion = "Detenido";
                }
                else
                {
                    string errorMsg = "";
                    socketComm.CS_GetLastError(puntero, ref errorMsg);
                    throw new Exception($"Error al obtener estado extendido: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeError += $"\nEstado extendido: {ex.Message}";
            }
        }

        /// <summary>
        /// Obtiene información del sistema (disco, RAM, horas de trabajo, etc.)
        /// </summary>
        private void ObtenerInformacionSistema(SocketComm socketComm, Int32 puntero, LaserInfoResult resultado)
        {
            try
            {
                SocketComm.CSSysinfo sysinfo = new SocketComm.CSSysinfo();
                Int32 err = socketComm.CS_Sysinfo(puntero, ref sysinfo);

                if (err == 0)
                {
                    resultado.HorasTrabajo = sysinfo.hours;
                    resultado.ContadorTotalLargo = sysinfo.longcounter;
                    resultado.TemperaturaCPU = sysinfo.cputemp;
                    resultado.EspacioTotalDisco = sysinfo.size0;
                    resultado.EspacioDisponibleDisco = sysinfo.avail0;
                    resultado.EspacioTotalRAM = sysinfo.size1;
                    resultado.EspacioDisponibleRAM = sysinfo.avail1;
                }
                else
                {
                    string errorMsg = "";
                    socketComm.CS_GetLastError(puntero, ref errorMsg);
                    throw new Exception($"Error al obtener información del sistema: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeError += $"\nInformación del sistema: {ex.Message}";
            }
        }

        /// <summary>
        /// Obtiene información de temperatura y hardware
        /// </summary>
        private void ObtenerInformacionHardware(SocketComm socketComm, Int32 puntero, LaserInfoResult resultado)
        {
            try
            {
                SocketComm.CSCoretemp coretemp = new SocketComm.CSCoretemp();
                Int32 err = socketComm.CS_Coretemp(puntero, ref coretemp);

                if (err == 0)
                {
                    resultado.TemperaturaBoard = coretemp.boardtemp;
                    resultado.Humedad = coretemp.humidity;
                    resultado.Voltaje5V = coretemp.voltage1;
                    resultado.Voltaje3_3V = coretemp.voltage2;
                    resultado.TemperaturaFanLocal = coretemp.fanlocaltemp;
                    resultado.TemperaturaFanRemoto = coretemp.fanremotetemp;
                    resultado.VelocidadFan = coretemp.fantacho;
                }
                else
                {
                    string errorMsg = "";
                    socketComm.CS_GetLastError(puntero, ref errorMsg);
                    throw new Exception($"Error al obtener información de hardware: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeError += $"\nInformación de hardware: {ex.Message}";
            }
        }

        /// <summary>
        /// Obtiene la lista de archivos en la impresora
        /// </summary>
        private void ObtenerListaArchivos(SocketComm socketComm, Int32 puntero, LaserInfoResult resultado)
        {
            try
            {
                string filenames = "";
                Int32 err = socketComm.CS_GetFilenames(puntero, "msf", 0, ref filenames);

                if (err == 0 && !string.IsNullOrWhiteSpace(filenames))
                {
                    // Los archivos vienen separados por caracteres nulos o espacios
                    resultado.Archivos = filenames.Split(new char[] { '\0', ' ', '\n', '\r' }, 
                        StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    resultado.Archivos = new string[0];
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeError += $"\nLista de archivos: {ex.Message}";
                resultado.Archivos = new string[0];
            }
        }

        /// <summary>
        /// Obtiene datos internos de la conexión
        /// </summary>
        private void ObtenerDatosConexion(SocketComm socketComm, Int32 puntero, LaserInfoResult resultado)
        {
            try
            {
                byte leading = 0;
                byte control = 0;
                UInt16 foundmask = 0;
                Int32 err = socketComm.CS_GetConnectionData(puntero, ref leading, ref control, ref foundmask);

                if (err == 0)
                {
                    resultado.LeadingByte = leading;
                    resultado.ControlByte = control;
                    resultado.FoundMask = foundmask;
                }
            }
            catch (Exception ex)
            {
                resultado.MensajeError += $"\nDatos de conexión: {ex.Message}";
            }
        }
    }
}

