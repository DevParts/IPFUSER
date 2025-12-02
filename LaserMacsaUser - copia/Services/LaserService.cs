using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LaserMacsaUser.Models;
using SocketCommNet;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio de comunicación con el láser usando SocketCommNet (TCPIPver31)
    /// 
    /// FLUJO DE CONEXIÓN:
    /// 1. Initialize() -> CS_Init() -> CS_StartClient() -> Conecta a SocketCommDll.dll
    /// 2. SocketCommDll.dll se comunica con el láser vía TCP/IP
    /// 3. Todas las operaciones pasan por SocketCommNet (wrapper) -> SocketCommDll.dll (nativa)
    /// 
    /// UBICACIÓN:
    /// - Servicio: LaserMacsaUser/Services/LaserService.cs
    /// - Wrapper: TCPIPver31/SocketCommNet/SocketComm.cs
    /// - DLL Nativa: TCPIPver31/libs/x64/SocketCommDll.dll
    /// </summary>
    public class LaserService : ILaserService
    {
        private SocketComm? _socketComm;
        private int _socketHandle;
        private bool _isInitialized = false;
        private System.Timers.Timer? _alarmCheckTimer;
        
        // Registro de alarmas para limpieza cuando se resuelven
        private readonly HashSet<string> _activeAlarmKeys = new HashSet<string>();
        
        public bool IsConnected => _isInitialized && _socketComm != null && 
                                   _socketComm.CS_IsConnected(_socketHandle) != 0;
        
        public event EventHandler<LaserAlarmEventArgs>? AlarmDetected;
        
        public bool Initialize(string ipAddress, string messagePath)
        {
            try
            {
                _socketComm = new SocketComm();
                _socketHandle = 0;
                
                // Según el código original (CLaser.cs línea 231-235):
                // name = "anyword" (cualquier palabra, no se usa realmente)
                // path = ".\\\\" (doble backslash) - ruta local, no se usa para archivos
                // El path en MInit es solo para configuración interna, no para archivos
                string name = "anyword";
                string path = ".\\"; // Ruta local por defecto (como en código original)
                
                // Inicializar socket
                _socketComm.CS_Init(ref _socketHandle, name, ipAddress, path);
                
                // Conectar al láser
                int result = _socketComm.CS_StartClient(_socketHandle);
                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    throw new Exception($"Error al conectar con el láser: {error}");
                }
                
                _isInitialized = true;
                
                // Iniciar timer para verificar alarmas periódicamente
                StartAlarmMonitoring();
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en Initialize: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Inicia monitoreo de alarmas cada 3 segundos (reducido para no sobrecargar)
        /// </summary>
        private void StartAlarmMonitoring()
        {
            _alarmCheckTimer = new System.Timers.Timer(3000); // Verificar cada 3 segundos (no cada segundo)
            _alarmCheckTimer.Elapsed += AlarmCheckTimer_Elapsed;
            _alarmCheckTimer.Start();
        }
        
        /// <summary>
        /// Detiene monitoreo de alarmas
        /// </summary>
        private void StopAlarmMonitoring()
        {
            _alarmCheckTimer?.Stop();
            _alarmCheckTimer?.Dispose();
            _alarmCheckTimer = null;
        }
        
        /// <summary>
        /// Verifica alarmas periódicamente (cada 3 segundos)
        /// </summary>
        private void AlarmCheckTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_isInitialized) return;
            
            try
            {
                var status = GetStatus();
                var alarms = LaserAlarmService.ProcessLaserStatus(status);
                
                // Disparar evento por cada alarma activa
                // La UI se encargará de manejar si se muestra o no según si fue cerrada manualmente
                foreach (var alarm in alarms)
                {
                    // Disparar evento - la UI decidirá si mostrar o no
                    AlarmDetected?.Invoke(this, new LaserAlarmEventArgs
                    {
                        Alarm = alarm,
                        Status = status
                    });
                }
                
                // Limpiar alarmas resueltas del registro cuando ya no están activas
                CleanupResolvedAlarms(alarms);
            }
            catch
            {
                // Ignorar errores en el timer para no bloquear
            }
        }
        
        /// <summary>
        /// Limpia el registro de alarmas que ya no están activas
        /// </summary>
        private void CleanupResolvedAlarms(List<LaserAlarmService.AlarmInfo> currentAlarms)
        {
            // Crear conjunto de alarmas activas actuales
            var currentAlarmKeys = new HashSet<string>();
            foreach (var alarm in currentAlarms)
            {
                string alarmKey = $"{alarm.Code}_{alarm.Type}";
                currentAlarmKeys.Add(alarmKey);
            }
            
            // Actualizar registro de alarmas activas
            _activeAlarmKeys.Clear();
            foreach (var key in currentAlarmKeys)
            {
                _activeAlarmKeys.Add(key);
            }
        }
        
        public bool CopyMessageFile(string messageName, string messagePath)
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                // Según el código original (Laser.cs línea 602-604):
                // - Si messageName es una ruta completa, extraer solo el nombre del archivo
                // - messagePath debe terminar con "\\"
                // - El nombre del archivo debe incluir la extensión
                
                string filename = messageName;
                string path = messagePath;
                
                // Si messageName es una ruta completa, extraer solo el nombre del archivo
                if (Path.IsPathRooted(messageName))
                {
                    filename = Path.GetFileName(messageName);
                    // Si no se proporcionó path, extraerlo de messageName
                    if (string.IsNullOrEmpty(messagePath) || messagePath == ".\\" || messagePath == "./")
                    {
                        path = Path.GetDirectoryName(messageName) ?? string.Empty;
                    }
                }
                
                // Asegurar que path termine con "\\" (según documentación de CS_CopyFile)
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.EndsWith("\\") && !path.EndsWith("/"))
                    {
                        path += "\\";
                    }
                }
                else
                {
                    path = ".\\"; // Ruta por defecto
                }
                
                // Verificar que el archivo existe localmente antes de copiar
                string fullLocalPath = Path.IsPathRooted(messageName) ? messageName : Path.Combine(path, filename);
                if (!File.Exists(fullLocalPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Advertencia: Archivo no encontrado localmente: '{fullLocalPath}'. Intentando copiar de todas formas.");
                }
                
                byte option = 0; // 0 = harddisk, 1 = ramdisk
                int result = _socketComm.CS_CopyFile(_socketHandle, filename, path, option);
                
                if (result != 0)
                {
                    // Obtener error para diagnóstico
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    System.Diagnostics.Debug.WriteLine($"Error en CopyMessageFile: {error}. Archivo: '{filename}', Ruta: '{path}'");
                    return false;
                }
                
                // Pequeña espera después de copiar para asegurar que el archivo esté disponible en el láser
                System.Threading.Thread.Sleep(100);
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción en CopyMessageFile: {ex.Message}");
                return false;
            }
        }
        
        public bool SetDefaultMessage(string messageName)
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                // Según el código original (CLaser.cs línea 245 y Laser.cs línea 637):
                // SetDefault recibe el nombre del archivo CON extensión (el mismo que se usó en CopyFile)
                // La documentación dice "without extension", pero el código original pasa el nombre con extensión
                // y funciona correctamente. El láser internamente maneja ambos casos.
                
                // Extraer solo el nombre del archivo (con extensión) si viene como ruta completa
                string filename = Path.IsPathRooted(messageName) 
                    ? Path.GetFileName(messageName)  // Con extensión
                    : messageName;
                
                // Intentar primero con el nombre completo (con extensión) como en el código original
                int result = _socketComm.CS_SetDefault(_socketHandle, filename);
                
                if (result == 0)
                {
                    return true;
                }
                
                // Si falla, intentar sin extensión (según documentación)
                string nameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                result = _socketComm.CS_SetDefault(_socketHandle, nameWithoutExt);
                
                if (result == 0)
                {
                    return true;
                }
                
                // Si aún falla, obtener el error para diagnóstico
                string error = string.Empty;
                _socketComm.CS_GetLastError(_socketHandle, ref error);
                System.Diagnostics.Debug.WriteLine($"Error en SetDefaultMessage: {error}. Intentado: '{filename}' y '{nameWithoutExt}'");
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción en SetDefaultMessage: {ex.Message}");
                return false;
            }
        }
        
        public bool EnableBufferedUserFields(int userFields, int bufferSize, bool isDataString)
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                int actSize = 0;
                int fillStatus = 0;
                
                for (int i = 0; i < userFields; i++)
                {
                    int field = i;
                    
                    if (!isDataString)
                    {
                        // Buffer para texto UTF-8
                        int result = _socketComm.CS_EnableBufferedUMExt(
                            _socketHandle, 0, ref actSize, ref field, ref fillStatus, bufferSize);
                        if (result != 0) return false;
                    }
                    else
                    {
                        // Buffer para DataMatrix
                        int result = _socketComm.CS_EnableBufferedDataString(
                            _socketHandle, 0, ref actSize, ref field, ref fillStatus, bufferSize);
                        if (result != 0) return false;
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool StartPrinting(string messageName, int copies = 0)
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                // CS_Start acepta el nombre con o sin extensión según ENVIO_ARCHIVOS_LASER_COMPLETO.md
                // Usar el nombre tal cual (puede venir con o sin extensión)
                int result = _socketComm.CS_Start(_socketHandle, messageName, copies);
                
                if (result != 0)
                {
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    System.Diagnostics.Debug.WriteLine($"Error al iniciar marcado: {error}");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción en StartPrinting: {ex.Message}");
                return false;
            }
        }
        
        public bool StopPrinting(int timeout = 2000)
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                _socketComm.CS_Stop(_socketHandle, timeout);
                _socketComm.CS_Knockout(_socketHandle); // Limpiar buffer
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public int SendCode(string code, int field = 0)
        {
            if (!_isInitialized || _socketComm == null) return -1;
            
            try
            {
                // Enviar como UTF-8
                int result = _socketComm.CS_FastUsermessage(_socketHandle, field, code);
                return result;
                // 0 = éxito, 8 = buffer lleno, otro = error
            }
            catch
            {
                return -1;
            }
        }
        
        public int SendDataMatrix(byte[] dataMatrix, int field = 0)
        {
            if (!_isInitialized || _socketComm == null) return -1;
            
            try
            {
                int result = _socketComm.CS_FastDataString(_socketHandle, field, dataMatrix, dataMatrix.Length);
                return result;
            }
            catch
            {
                return -1;
            }
        }
        
        /// <summary>
        /// Envía un código dividido según los splits de la promoción
        /// Divide el código según Split1, Split2, Split3, Split4 y envía cada parte al campo correspondiente (0, 1, 2, 3)
        /// </summary>
        public int SendCodeWithSplits(string code, int split1, int split2, int split3, int split4, bool isDataString = false)
        {
            if (!_isInitialized || _socketComm == null) return -1;
            if (string.IsNullOrEmpty(code)) return -1;
            
            try
            {
                int result = 0;
                int currentPosition = 0;
                int field = 0;
                
                // Enviar cada parte del código según los splits
                if (split1 > 0 && currentPosition < code.Length)
                {
                    int length = Math.Min(split1, code.Length - currentPosition);
                    string part1 = code.Substring(currentPosition, length);
                    
                    if (!isDataString)
                    {
                        result = _socketComm.CS_FastUsermessage(_socketHandle, field, part1);
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(part1);
                        result = _socketComm.CS_FastDataString(_socketHandle, field, data, data.Length);
                    }
                    
                    if (result != 0) return result; // Error al enviar
                    currentPosition += length;
                    field++;
                }
                
                if (split2 > 0 && currentPosition < code.Length)
                {
                    int length = Math.Min(split2, code.Length - currentPosition);
                    string part2 = code.Substring(currentPosition, length);
                    
                    if (!isDataString)
                    {
                        result = _socketComm.CS_FastUsermessage(_socketHandle, field, part2);
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(part2);
                        result = _socketComm.CS_FastDataString(_socketHandle, field, data, data.Length);
                    }
                    
                    if (result != 0) return result;
                    currentPosition += length;
                    field++;
                }
                
                if (split3 > 0 && currentPosition < code.Length)
                {
                    int length = Math.Min(split3, code.Length - currentPosition);
                    string part3 = code.Substring(currentPosition, length);
                    
                    if (!isDataString)
                    {
                        result = _socketComm.CS_FastUsermessage(_socketHandle, field, part3);
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(part3);
                        result = _socketComm.CS_FastDataString(_socketHandle, field, data, data.Length);
                    }
                    
                    if (result != 0) return result;
                    currentPosition += length;
                    field++;
                }
                
                if (split4 > 0 && currentPosition < code.Length)
                {
                    int length = Math.Min(split4, code.Length - currentPosition);
                    string part4 = code.Substring(currentPosition, length);
                    
                    if (!isDataString)
                    {
                        result = _socketComm.CS_FastUsermessage(_socketHandle, field, part4);
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(part4);
                        result = _socketComm.CS_FastDataString(_socketHandle, field, data, data.Length);
                    }
                    
                    if (result != 0) return result;
                }
                
                return 0; // Éxito
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en SendCodeWithSplits: {ex.Message}");
                return -1;
            }
        }
        
        public LaserStatus GetStatus()
        {
            var status = new LaserStatus { State = LaserState.Stopped };
            
            if (!_isInitialized || _socketComm == null) return status;
            
            try
            {
                SocketComm.CSStatus csStatus = default;
                int result = _socketComm.CS_Status(_socketHandle, ref csStatus);
                
                if (result == 0)
                {
                    status.OkPrints = csStatus.d_counter;
                    status.NokPrints = csStatus.s_counter;
                    status.TotalPrints = csStatus.t_counter;
                    status.MessageName = csStatus.name;
                    status.IsPrinting = csStatus.Start == 0; // 0 = printing, 1 = stopped
                    status.ErrorCode = csStatus.err;
                    
                    // Procesar código de error para obtener descripción
                    if (status.ErrorCode != 0)
                    {
                        var alarm = LaserAlarmService.ProcessErrorCode(status.ErrorCode);
                        status.ErrorDescription = $"{alarm.Code}: {alarm.Description}";
                        
                        // Determinar estado según severidad
                        status.State = alarm.Severity == LaserAlarmService.AlarmSeverity.Warning ? 
                            LaserState.Warning : LaserState.Errors;
                    }
                    else
                    {
                        status.State = status.IsPrinting ? LaserState.Marking : LaserState.Stopped;
                    }
                }
                else
                {
                    // Error al obtener estado
                    string error = string.Empty;
                    _socketComm.CS_GetLastError(_socketHandle, ref error);
                    status.ErrorDescription = $"Error al obtener estado: {error}";
                    status.State = LaserState.Errors;
                }
            }
            catch (Exception ex)
            {
                status.ErrorDescription = $"Excepción al obtener estado: {ex.Message}";
                status.State = LaserState.Errors;
            }
            
            return status;
        }
        
        public int GetBufferCount(bool isDataString = false)
        {
            if (!_isInitialized || _socketComm == null) return 0;
            
            try
            {
                int actSize = 0;
                int field = 0;
                int fillStatus = 0;
                int defSize = 100;
                
                if (!isDataString)
                {
                    _socketComm.CS_EnableBufferedUMExt(
                        _socketHandle, 1, ref actSize, ref field, ref fillStatus, defSize);
                }
                else
                {
                    _socketComm.CS_EnableBufferedDataString(
                        _socketHandle, 1, ref actSize, ref field, ref fillStatus, defSize);
                }
                
                return fillStatus;
            }
            catch
            {
                return 0;
            }
        }
        
        public bool ClearBuffer()
        {
            if (!_isInitialized || _socketComm == null) return false;
            
            try
            {
                _socketComm.CS_Knockout(_socketHandle);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public void Disconnect()
        {
            // Detener monitoreo de alarmas
            StopAlarmMonitoring();
            
            // Limpiar registro de alarmas activas
            _activeAlarmKeys.Clear();
            
            if (!_isInitialized || _socketComm == null) return;
            
            try
            {
                _socketComm.CS_Knockout(_socketHandle);
                _socketComm.CS_ShutdownClient(_socketHandle);
                _socketComm.CS_Finish(_socketHandle);
            }
            catch { }
            finally
            {
                _isInitialized = false;
                _socketHandle = 0;
            }
        }
    }
}

