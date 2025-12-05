# ✅ CHECKLIST - Funcionalidades del Software de Láser

Este documento contiene una lista de verificación completa de todas las funcionalidades necesarias para el correcto funcionamiento del software conectado al láser Macsa, basado en la documentación TCP/IP v3.1 y el código de referencia.

**Estado de verificación:**
- ✅ = Implementado y verificado
- ⚠️ = Parcialmente implementado (necesita mejoras)
- ❌ = No implementado
- 🔄 = En progreso

---

## 1. CONEXIÓN Y INICIALIZACIÓN

### 1.1 Conexión TCP/IP Básica
- [✅] Inicialización de socket principal (`CS_Init` + `CS_StartClient`)
- [✅] Inicialización de socket secundario para envío de códigos
- [✅] Verificación de conexión (`CS_IsConnected`)
- [✅] Cierre correcto de sockets (`CS_Finish`)
- [✅] Manejo de errores de conexión con excepciones personalizadas
- [⚠️] Configuración de timeout de socket (`CS_SetTimeout`) - **FALTA IMPLEMENTAR**
- [❌] Verificación de versión de DLL (`CS_GetDllVersion`) - **FALTA IMPLEMENTAR**
- [❌] Obtención de versión del láser (`CS_GetVersion`, `CS_GetVersionString`) - **FALTA IMPLEMENTAR**
- [❌] Obtención de datos de conexión (`CS_GetConnectionData`) - **FALTA IMPLEMENTAR**

### 1.2 Configuración de Buffer
- [✅] Configuración de buffer para campos de usuario (`CS_EnableBufferedUMExt` con set=0)
- [✅] Obtención de estado del buffer (`CS_EnableBufferedUMExt` con get=1)
- [✅] Reset de buffer (`CS_EnableBufferedUMExt` con set=2)
- [✅] Configuración automática durante inicialización
- [✅] Buffer size configurable desde AppSettings
- [⚠️] Verificación de buffer lleno antes de enviar - **MEJORAR LÓGICA**

---

## 2. ENVÍO DE CÓDIGOS AL LÁSER

### 2.1 Envío de Mensajes de Usuario
- [✅] Envío de códigos UTF-8 (`CS_FastUsermessage`)
- [✅] Soporte para múltiples campos de usuario (1-4 campos)
- [✅] División de códigos según Split1, Split2, Split3, Split4
- [✅] Manejo de código de error 8 (buffer lleno)
- [✅] Reintentos automáticos cuando el buffer está lleno
- [✅] Validación de códigos vacíos antes de enviar - **IMPLEMENTADO**
- [✅] Validación de partes vacías después de dividir códigos - **IMPLEMENTADO**
- [⚠️] Verificación de código cargado con `GetFastUsermessage` - **FALTA IMPLEMENTAR VERIFICACIÓN DESPUÉS DE ENVIAR**
- [❌] Envío de códigos ASCII (`CS_FastASCIIUsermessage`) - **OPCIONAL**
- [❌] Envío múltiple de códigos (`CS_MultipleUsermessage`) - **OPCIONAL, PARA OPTIMIZACIÓN**

### 2.2 Verificación de Códigos Enviados
- [✅] Método `GetFastUsermessage` implementado
- [❌] Verificación automática después de cada envío - **CRÍTICO: IMPLEMENTAR**
- [❌] Comparación de código enviado vs código leído - **CRÍTICO: IMPLEMENTAR**
- [❌] Reintento si la verificación falla - **CRÍTICO: IMPLEMENTAR**
- [❌] Logging de códigos que no se cargaron correctamente - **IMPORTANTE**

### 2.3 Envío de DataString (Alternativa)
- [❌] Envío de DataString (`CS_FastDataString`) - **OPCIONAL, PARA CÓDIGOS BINARIOS**
- [❌] Lectura de DataString (`CS_GetFastDataString`) - **OPCIONAL**
- [❌] Buffer para DataString (`CS_EnableBufferedDataString`) - **OPCIONAL**

---

## 3. ESTADO Y MONITOREO DEL LÁSER

### 3.1 Obtención de Estado
- [✅] Obtención de estado extendido (`CS_StatusExt`)
- [✅] Procesamiento de AlarmMask1 y AlarmMask2
- [✅] Extracción de códigos de alarma activos
- [✅] Contadores (OK, NOK, Total)
- [✅] Estado de impresión (Start/Stop)
- [✅] Nombre de mensaje activo
- [✅] Estado de señales IO (`SignalState`)
- [✅] Información extra (uso de scanfield, modo dinámico)
- [⚠️] Monitoreo periódico del estado - **FALTA TIMER EN Form1**
- [❌] Obtención de estado básico (`CS_Status`) - **OPCIONAL, YA SE USA StatusExt**

### 3.2 Información del Sistema
- [❌] Información del sistema (`CS_Sysinfo`) - **FALTA IMPLEMENTAR**
  - Temperatura CPU
  - Espacio en disco duro
  - Espacio en RAM disk
  - Espacio en RAM font
  - Espacio en log drive
  - Horas de trabajo
  - Contador total de impresiones
- [❌] Temperatura del núcleo (`CS_Coretemp`) - **FALTA IMPLEMENTAR**
  - Temperatura CPU y board
  - Humedad
  - Voltajes (5V, 3.3V)
  - Estado de ventiladores
- [❌] Estado de señales IO (`CS_Signalstate`) - **PARCIALMENTE (solo lectura en StatusExt)**

---

## 4. MANEJO DE ALARMAS

### 4.1 Detección de Alarmas
- [✅] Mapeo completo de códigos de alarma (según alarmcodes.pdf)
- [✅] Procesamiento de AlarmMask1 (bits 0-31)
- [✅] Procesamiento de AlarmMask2 (bits 32-63)
- [✅] Clasificación de alarmas críticas vs warnings
- [✅] Evento `AlarmDetected` implementado
- [✅] Detección automática de alarmas en `GetStatus`
- [✅] Mapeo del código 0x848 (2120) - Mensaje vacío - **IMPLEMENTADO**
- [✅] Código 0x848 marcado como alarma crítica - **IMPLEMENTADO**
- [⚠️] Verificación periódica de alarmas - **FALTA TIMER**

### 4.2 Acciones ante Alarmas
- [✅] Detención automática de producción para alarmas críticas
- [✅] Notificación al usuario (MessageBox)
- [✅] Logging de alarmas
- [✅] Prevención de alarmas duplicadas - **IMPLEMENTADO: Cada alarma se muestra solo una vez**
- [✅] Limpieza automática de alarmas resueltas - **IMPLEMENTADO**
- [❌] Reintento automático después de alarma resuelta - **FALTA IMPLEMENTAR**
- [❌] Historial de alarmas - **OPCIONAL**

### 4.3 Alarmas Críticas Implementadas
- [✅] 0x02 - Laser OFF (interlock open)
- [✅] 0x06 - Q-switch error
- [✅] 0x16 - Overtemperature
- [✅] 0x24 - Warmup cycle still active
- [✅] 0x25 - Shutter closed
- [✅] 0x26 - Laser not ready
- [✅] 0x28 - Power off
- [✅] 0x41 - Scanner X alarm
- [✅] 0x42 - Scanner Y alarm
- [✅] 0x44 - Initialization alarm
- [✅] 0x46 - Z scanner error
- [✅] 0x47 - Laser not armed
- [✅] 0x61 - Watchdog
- [✅] 0x62 - DSP paused
- [✅] 0x63 - FPGA failure
- [✅] 0x848 (2120) - Mensaje vacío - **IMPLEMENTADO**

---

## 5. CONTROL DE IMPRESIÓN

### 5.1 Inicio y Detención
- [✅] Inicio de impresión (`CS_Start`)
- [✅] Detención de impresión (`CS_Stop`)
- [✅] Limpieza de buffer antes de detener (`CS_Knockout`)
- [❌] Recarga de mensaje (`CS_Reload`) - **FALTA IMPLEMENTAR**
- [❌] Trigger de impresión software (`CS_TriggerPrint`) - **OPCIONAL**

### 5.2 Gestión de Archivos de Mensaje
- [✅] Copia de archivo al láser (`CS_CopyFile`)
- [✅] Establecimiento de mensaje por defecto (`CS_SetDefault`)
- [❌] Eliminación de archivo del láser (`CS_Delete`) - **FALTA IMPLEMENTAR**
- [❌] Obtención de lista de archivos (`CS_GetFilenames`) - **FALTA IMPLEMENTAR**
- [❌] Almacenamiento de configuración (`CS_Store`) - **OPCIONAL**

### 5.3 Modos de Impresión
- [❌] Modo de impresión (`CS_PrintMode`) - **FALTA IMPLEMENTAR**
  - Modo por defecto
  - Modo UMT (User Message Table)
  - Modo BatchJob
- [❌] Modo estático/dinámico (`CS_Mode`) - **FALTA IMPLEMENTAR**
- [❌] Configuración dinámica (`CS_SetDynamic`, `CS_GetDynamic`) - **OPCIONAL**

---

## 6. CONTADORES Y ESTADÍSTICAS

### 6.1 Contadores del Láser
- [✅] Lectura de contadores desde estado (d_counter, s_counter, t_counter)
- [❌] Reset de contadores (`CS_CounterReset`) - **FALTA IMPLEMENTAR**
- [❌] Contador global (`CS_SetGlobalCounter`, `CS_GetGlobalCounter`) - **FALTA IMPLEMENTAR**
- [❌] Contador privado (`CS_SetPrivateCounter`) - **FALTA IMPLEMENTAR**
- [❌] Obtención de contador desde interfaz (`GetCounter`) - **FALTA IMPLEMENTAR EN ILaserService**
- [❌] Establecimiento de contador (`SetCounter`) - **FALTA IMPLEMENTAR EN ILaserService**

### 6.2 Estadísticas de Producción
- [✅] Contadores de códigos producidos en QueueService
- [✅] Actualización de consumos en base de datos
- [⚠️] Sincronización con contadores del láser - **MEJORAR**

---

## 7. CONFIGURACIÓN Y CALIBRACIÓN

### 7.1 Offset y Posicionamiento
- [❌] Offset X/Y (`CS_Offset`) - **FALTA IMPLEMENTAR**
- [❌] Defocus Z (`CS_Defocus`) - **FALTA IMPLEMENTAR**
- [❌] Shift, Rotate (`CS_ShiftRotate`) - **FALTA IMPLEMENTAR**

### 7.2 Escala de Potencia
- [❌] Escala de potencia (`CS_Powerscale`) - **FALTA IMPLEMENTAR**

### 7.3 Configuración ASCII
- [❌] Configuración ASCII (`CS_AsciiConfig`) - **OPCIONAL**

### 7.4 Tiempo del Sistema
- [❌] Sincronización de tiempo (`CS_Settime`) - **FALTA IMPLEMENTAR**
- [❌] Establecimiento de tiempo de usuario (`CS_Setusertime`) - **OPCIONAL**

---

## 8. GESTIÓN DE COLAS Y BUFFER

### 8.1 Sistema de Colas
- [✅] Sistema de doble cola (producer-consumer)
- [✅] Alternancia automática de colas
- [✅] Manejo de buffer lleno con espera
- [✅] Reintentos automáticos
- [⚠️] Verificación de código después de enviar - **CRÍTICO: IMPLEMENTAR**
- [❌] Verificación periódica del estado durante envío - **IMPORTANTE**

### 8.2 Optimizaciones
- [❌] Envío múltiple de códigos (`CS_MultipleUsermessage`) - **OPCIONAL, PARA MEJOR RENDIMIENTO**
- [❌] Lectura de FIFO (`CS_GetFifofield`) - **OPCIONAL**
- [❌] Dump de FIFO (`CS_FifoDump`) - **OPCIONAL, PARA DEBUG**

---

## 9. CONFIGURACIÓN Y PERSISTENCIA

### 9.1 Configuración de Red
- [✅] IP del láser configurable y persistente
- [✅] Buffer size configurable y persistente
- [✅] Tiempo de espera cuando buffer lleno configurable
- [❌] Timeout de socket configurable - **FALTA IMPLEMENTAR**
- [❌] Puerto TCP configurable - **NO APLICABLE (usa puerto por defecto)**

### 9.2 Configuración de Aplicación
- [✅] Settings guardados en Properties.Settings
- [✅] Persistencia automática al cambiar valores
- [✅] Carga de valores al iniciar aplicación

---

## 10. MANEJO DE ERRORES Y EXCEPCIONES

### 10.1 Excepciones Personalizadas
- [✅] `LaserCommunicationException` implementada
- [✅] `DatabaseConnectionException` implementada
- [✅] Mensajes de error descriptivos
- [✅] Información de contexto (IP, operación, código de error)

### 10.2 Manejo de Errores
- [✅] Captura de excepciones en puntos críticos
- [✅] Logging de errores
- [✅] Notificación al usuario
- [⚠️] Reintentos automáticos - **MEJORAR LÓGICA**
- [❌] Recuperación automática de conexión perdida - **IMPORTANTE**

---

## 11. INTERFAZ DE USUARIO

### 11.1 Monitoreo en Tiempo Real
- [✅] Actualización de contadores
- [✅] Indicador de estado de conexión
- [✅] Mostrar último código enviado
- [❌] Timer para verificación periódica de estado - **CRÍTICO: IMPLEMENTAR**
- [❌] Panel de estado del láser (temperatura, voltajes, etc.) - **OPCIONAL**
- [❌] Visualización de alarmas activas - **REMOVIDO (a petición del usuario)**

### 11.2 Configuración
- [✅] Formulario de configuración principal
- [✅] Formulario de configuración de prueba
- [✅] PropertyGrid para edición de settings
- [✅] Guardado automático de cambios

---

## 12. FUNCIONALIDADES AVANZADAS (Opcionales)

### 12.1 Event Handlers
- [❌] Configuración de event handler (`CS_Eventhandler`) - **OPCIONAL**

### 12.2 Batch Jobs
- [❌] Inicio extendido para batch jobs (`CS_StartExtended`) - **OPCIONAL**
- [❌] Tabla de mensajes (`CS_MTable`) - **OPCIONAL**

### 12.3 Exportación y Debug
- [❌] Dump SVG (`CS_DumpSVG`, `CS_DumpSVGExt`) - **OPCIONAL, PARA DEBUG**
- [❌] Test pointer (`CS_TestPointer`) - **OPCIONAL**

### 12.4 Sesiones de Impresión
- [❌] Inicio de sesión de impresión (`CS_StartPrintSession`) - **OPCIONAL**
- [❌] Fin de sesión de impresión (`CS_EndPrintSession`) - **OPCIONAL**

### 12.5 Shutdown y Reinicio
- [❌] Shutdown del servidor (`CS_ServerShutdown`) - **OPCIONAL, PARA ADMINISTRACIÓN**
- [❌] Shutdown del cliente (`CS_ShutdownClient`) - **OPCIONAL**

---

## 13. PRIORIDADES DE IMPLEMENTACIÓN

### 🔴 CRÍTICO (Implementar HOY)
1. **Verificación de códigos después de enviar** - Usar `GetFastUsermessage` para confirmar que el código se cargó correctamente
2. **Timer de verificación periódica de estado** - Monitorear estado del láser cada 2-5 segundos
3. **Verificación periódica de alarmas** - Detectar alarmas críticas durante la producción

### 🟡 IMPORTANTE (Implementar pronto)
4. **Configuración de timeout de socket** - `CS_SetTimeout` para evitar timeouts largos
5. **Recuperación automática de conexión** - Reconectar automáticamente si se pierde la conexión
6. **Información del sistema** - `CS_Sysinfo` y `CS_Coretemp` para monitoreo de salud del láser
7. **Reset de contadores** - `CS_CounterReset` para sincronizar contadores
8. **Recarga de mensaje** - `CS_Reload` después de enviar códigos

### 🟢 OPCIONAL (Mejoras futuras)
9. Envío múltiple de códigos para mejor rendimiento
10. Modos de impresión (UMT, BatchJob)
11. Offset y calibración
12. Exportación SVG para debug

---

## 14. NOTAS DE IMPLEMENTACIÓN

### Verificación de Códigos
- Después de cada `SendUserMessage`, llamar a `GetFastUsermessage` para verificar
- Comparar el código enviado con el código leído
- Si no coinciden, reintentar el envío
- Loggear los casos donde la verificación falla

### Monitoreo Periódico
- Implementar un `Timer` en `Form1` que llame a `GetStatus()` cada 2-5 segundos
- Verificar alarmas críticas y detener producción si es necesario
- Verificar conexión y reconectar si se perdió
- Actualizar UI con información del estado

### Timeout de Socket
- Llamar a `CS_SetTimeout` después de `CS_StartClient`
- Valor recomendado: 5000-10000 ms (5-10 segundos)
- Configurable desde AppSettings

### Recuperación de Conexión
- Detectar cuando `IsConnected` es false durante producción
- Intentar reconectar automáticamente
- Pausar envío de códigos durante reconexión
- Reanudar después de reconexión exitosa

---

## 15. REFERENCIAS

- **Documentación TCP/IP v3.1**: `TCPIPver31/doc/`
- **Código de referencia**: `user complete/IPFUser/`
- **SocketCommNet**: `TCPIPver31/SocketCommNet/SocketComm.cs`
- **Alarm codes**: `TCPIPver31/doc/alarmcodes.pdf`

---

**Última actualización**: [Fecha de última modificación]
**Versión del software**: [Versión actual]
**Estado general**: ⚠️ Funcional pero requiere mejoras críticas

