# Guía de Ejecución - MacsaLaserTest

Esta guía explica cómo ejecutar cada programa de prueba y qué resultados esperar.

## Configuración Inicial

### 1. Configurar la IP de la Impresora

**IMPORTANTE**: Antes de ejecutar cualquier prueba, debes editar la IP de la impresora en cada archivo.

Abre cada archivo de prueba y busca la línea:
```csharp
string ipImpresora = "192.168.16.180";  // CAMBIA ESTA IP
```

Cambia `192.168.16.180` por la IP real de tu impresora laser Macsa.

### 2. Verificar DLLs

Asegúrate de que estas DLLs estén en `bin\Debug\`:
- `SocketCommNet.dll`
- `SocketCommDll.dll`

Si faltan, cópialas desde `..\TCPIPver31\libs\x86\`

## Cómo Ejecutar Cada Prueba

### Método 1: Cambiar StartupObject (Recomendado)

1. Abre `MacsaLaserTest.csproj` en un editor de texto
2. Busca la línea:
   ```xml
   <StartupObject>MacsaLaserTest.ProgramInfoTest</StartupObject>
   ```
3. Cámbiala por el nombre de la prueba que quieres ejecutar:
   - `MacsaLaserTest.ProgramInfoTest` (Test 1 - Información)
   - `MacsaLaserTest.Test02_UserMessages` (Test 2)
   - `MacsaLaserTest.Test03_ControlImpresion` (Test 3)
   - `MacsaLaserTest.Test04_EnviarArchivos` (Test 4)
   - `MacsaLaserTest.Test05_GestionarArchivos` (Test 5)
   - `MacsaLaserTest.Test06_Contadores` (Test 6)
   - `MacsaLaserTest.Test07_Configuracion` (Test 7)
   - `MacsaLaserTest.Test08_Monitoreo` (Test 8)
   - `MacsaLaserTest.Test09_Alarmas` (Test 9)
4. Guarda el archivo
5. Compila y ejecuta (F5 en Visual Studio)

### Método 2: Desde Línea de Comandos

```powershell
# Compilar
msbuild MacsaLaserTest.csproj /p:Configuration=Debug

# Ejecutar (cambia el StartupObject primero)
.\bin\Debug\MacsaLaserTest.exe
```

---

## Test 1: Información Completa

**Archivo**: `ProgramInfoTest.cs`  
**StartupObject**: `MacsaLaserTest.ProgramInfoTest`

### Qué Hace
Obtiene toda la información disponible de la impresora:
- Versión de DLL, protocolo y firmware
- Estado actual (mensaje activo, impresión)
- Contadores (total, OK, NOK)
- Información del sistema (disco, RAM, horas de trabajo)
- Hardware (temperatura, voltajes, ventiladores)
- Lista de archivos
- Datos de conexión

### Qué Esperar

**Si la conexión es exitosa:**
```
==========================================
  PRUEBA DE INFORMACIÓN - MACSA LASER
  Protocolo TCP/IP v3.1
==========================================

Configuración:
  IP Impresora: 192.168.16.180
  Ruta Local: .\

PASO 1: Creando conexión...
PASO 2: Inicializando...
  Inicialización exitosa
PASO 3: Conectando...
  Conexión establecida
PASO 4: Verificando conexión...
   Conexión verificada

PASO 5: Obteniendo información de la impresora...
  (Esto puede tardar unos segundos...)

PASO 6: Resultados:

==========================================
  INFORMACIÓN DE LA IMPRESORA
==========================================

--- VERSIÓN ---
  DLL: [número]
  Protocolo: [número]
  Firmware: [versión]

--- ESTADO ACTUAL ---
  Mensaje Activo: [nombre]
  Event Handler: [nombre]
  Estado: [Imprimiendo/Detenido]
  Copias a imprimir: [número]

--- CONTADORES ---
  Total: [número]
  OK: [número]
  NOK: [número]
  Total (largo): [número]

--- SISTEMA ---
  Horas de Trabajo: [número]
  Temperatura CPU: [temperatura] °C

--- ALMACENAMIENTO ---
  Disco - Total: [MB]
  Disco - Disponible: [MB]
  RAM - Total: [MB]
  RAM - Disponible: [MB]

--- HARDWARE ---
  Temp. Board: [temperatura] °C
  Humedad: [porcentaje] %
  Voltaje 5V: [voltaje] V
  Voltaje 3.3V: [voltaje] V
  ...

Información obtenida exitosamente
```

**Si hay error de conexión:**
```
Error: [mensaje de error]
```

### Tiempo Estimado
5-10 segundos

---

## Test 2: Mensajes de Usuario

**Archivo**: `Test02_UserMessages.cs`  
**StartupObject**: `MacsaLaserTest.Test02_UserMessages`

### Qué Hace
- Envía texto a campos de la impresora
- Lee mensajes actuales
- Prueba múltiples campos

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 2: MENSAJES DE USUARIO
  Envío de texto dinámico a la impresora
==========================================

Conectando...
Conexión establecida

PRUEBA 1: Enviar mensaje simple a campo 0
  Mensaje enviado: 'Hola desde C#'

PRUEBA 2: Leer mensaje del campo 0
  Mensaje leído: 'Hola desde C#'

PRUEBA 3: Enviar mensajes a múltiples campos
  Campo 0: 'Campo 0' - Enviado
  Campo 1: 'Campo 1' - Enviado
  Campo 2: 'Campo 2' - Enviado

PRUEBA 4: Leer todos los campos
  Campo 0: 'Campo 0'
  Campo 1: 'Campo 1'
  Campo 2: 'Campo 2'

Prueba de mensajes de usuario completada
```

**Nota**: Los mensajes enviados se reflejarán en la impresora si está configurada para usar esos campos.

### Tiempo Estimado
2-3 segundos

---

## Test 3: Control de Impresión

**Archivo**: `Test03_ControlImpresion.cs`  
**StartupObject**: `MacsaLaserTest.Test03_ControlImpresion`

### Qué Hace
- Detiene la impresión actual
- Recarga el archivo activo
- Dispara impresión por software
- Inicia impresión con archivo específico

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 3: CONTROL DE IMPRESIÓN
  Start, Stop, Trigger, Reload
==========================================

Conectando...
Conexión establecida

Estado actual de la impresora:
  Mensaje activo: [nombre]
  Estado: [Imprimiendo/Detenido]

PRUEBA 1: Detener impresión
  Impresión detenida

PRUEBA 2: Recargar archivo actual
  Archivo recargado

PRUEBA 3: Trigger de impresión (disparo software)
  Trigger enviado

PRUEBA 4: Iniciar impresión
  Ingresa el nombre del archivo (sin extensión) o Enter para omitir: [tu entrada]
  Impresión iniciada con archivo: [nombre]
  (o Prueba omitida si presionas Enter)

Estado final de la impresora:
  Estado: [Imprimiendo/Detenido]

Prueba de control de impresión completada
```

**Nota**: La impresora puede cambiar de estado físicamente si está imprimiendo.

### Tiempo Estimado
5-10 segundos (depende si ingresas nombre de archivo)

---

## Test 4: Enviar Archivos

**Archivo**: `Test04_EnviarArchivos.cs`  
**StartupObject**: `MacsaLaserTest.Test04_EnviarArchivos`

### Qué Hace
- Copia un archivo .msf desde tu PC a la impresora
- Permite elegir destino (RAM o disco duro)

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 4: ENVIAR ARCHIVOS
  Copiar archivos .msf a la impresora
==========================================

Conectando...
Conexión establecida

PRUEBA: Enviar archivo a la impresora
  Opciones de destino:
    0 = RAM Disk
    1 = Hard Disk

  Ingresa la ruta completa del archivo .msf: [ruta]
  Destino (0=RAM, 1=HD): [0 o 1]
  Enviando '[nombre].msf' desde '[directorio]'...
  Archivo enviado exitosamente a RAM Disk (o Hard Disk)

Verificando archivos en la impresora...
  Archivos encontrados:
    - [archivo1].msf
    - [archivo2].msf
    - [tu archivo].msf

Prueba de envío de archivos completada
```

**Si el archivo no existe:**
```
  Archivo no encontrado. Prueba omitida.
```

**Nota**: Necesitas tener un archivo .msf válido en tu PC para probar.

### Tiempo Estimado
10-30 segundos (depende del tamaño del archivo)

---

## Test 5: Gestionar Archivos

**Archivo**: `Test05_GestionarArchivos.cs`  
**StartupObject**: `MacsaLaserTest.Test05_GestionarArchivos`

### Qué Hace
- Lista archivos .msf en la impresora
- Establece archivo por defecto
- Elimina archivos (con confirmación)

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 5: GESTIONAR ARCHIVOS
  Listar, establecer por defecto, eliminar
==========================================

Conectando...
Conexión establecida

PRUEBA 1: Listar archivos .msf
  Archivos encontrados (3):
    1. archivo1.msf
    2. archivo2.msf
    3. archivo3.msf

PRUEBA 2: Establecer archivo por defecto
  Ingresa el nombre del archivo (sin extensión) o Enter para omitir: [nombre]
  Archivo '[nombre]' establecido como por defecto

PRUEBA 3: Verificar archivo actual
  Mensaje activo: [nombre]

PRUEBA 4: Eliminar archivo
  Ingresa el nombre del archivo a eliminar (con extensión) o Enter para omitir: [nombre.msf]
  ¿Estás seguro de eliminar '[nombre.msf]'? (s/n): s
  Archivo '[nombre.msf]' eliminado
  (o Eliminación cancelada si respondes 'n')
```

**Nota**: Ten cuidado al eliminar archivos, la acción es permanente.

### Tiempo Estimado
5-15 segundos (depende de tus respuestas)

---

## Test 6: Contadores

**Archivo**: `Test06_Contadores.cs`  
**StartupObject**: `MacsaLaserTest.Test06_Contadores`

### Qué Hace
- Lee contadores del estado
- Lee/establece contadores globales
- Establece contadores privados
- Resetea contadores

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 6: CONTADORES
  Leer y establecer contadores
==========================================

Conectando...
Conexión establecida

PRUEBA 1: Leer contadores del estado
  Contador Total: 1,234
  Contador OK: 1,200
  Contador NOK: 34

PRUEBA 2: Leer contador global
  Ingresa el número de campo (0-255) o Enter para campo 0: [número]
  Contador global campo 0: '[valor]'

PRUEBA 3: Establecer contador global
  Ingresa el valor del contador (o Enter para omitir): [valor]
  Contador global campo 0 establecido a: '[valor]'
  Valor verificado: '[valor]'

PRUEBA 4: Establecer contador privado
  Ingresa el número de campo (0-255) o Enter para campo 0: [número]
  Ingresa número de repeticiones (o Enter para 1): [número]
  Ingresa número de impresiones (o Enter para 1): [número]
  Contador privado campo 0 establecido:
    Repeticiones: [número]
    Impresiones: [número]

PRUEBA 5: Resetear contadores (d_counter y s_counter)
  ¿Deseas resetear los contadores? (s/n): s
  Contadores reseteados
  Contador OK después del reset: 0
  Contador NOK después del reset: 0
```

**Nota**: Los contadores se actualizan en tiempo real según la producción.

### Tiempo Estimado
10-20 segundos (depende de tus respuestas)

---

## Test 7: Configuración

**Archivo**: `Test07_Configuracion.cs`  
**StartupObject**: `MacsaLaserTest.Test07_Configuracion`

### Qué Hace
- Lee/establece modo de impresión
- Lee/establece modo estático/dinámico
- Establece offset (X, Y)
- Establece defocus (Z)
- Lee/establece powerscale

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 7: CONFIGURACIÓN
  Modo, Offset, Defocus, Powerscale
==========================================

Conectando...
Conexión establecida

PRUEBA 1: Modo de impresión
  0 = Default, 1 = UMT, 2 = Solo leer, 4 = Batchjob
  Modo actual: 0 (0=Default, 1=UMT, 4=Batchjob)

PRUEBA 2: Modo estático/dinámico
  0 = Estático, 1 = Dinámico estándar, 2 = Dinámico distancia, 3 = Dinámico-estático, 8 = Solo leer
  Modo actual: Estático

PRUEBA 3: Establecer offset
  Offset X en micrones (o Enter para 0): [número]
  Offset Y en micrones (o Enter para 0): [número]
  Offset establecido: X=100 μm, Y=50 μm

PRUEBA 4: Establecer defocus (Z)
  Defocus Z en micrones (o Enter para 0): [número]
  Defocus establecido: Z=25 μm

PRUEBA 5: Powerscale
  Member: 0-3 (diferentes escalas)
  Member (0-3) o Enter para 0: [número]
  Powerscale member 0 actual: [valor]
  ¿Deseas establecer un nuevo valor? (s/n): s
  Nuevo valor (o Enter para mantener): [valor]
  Powerscale member 0 establecido a: [valor]
```

**Nota**: Los cambios de configuración afectan la impresión. Úsalos con cuidado.

### Tiempo Estimado
15-30 segundos (depende de tus respuestas)

---

## Test 8: Monitoreo en Tiempo Real

**Archivo**: `Test08_Monitoreo.cs`  
**StartupObject**: `MacsaLaserTest.Test08_Monitoreo`

### Qué Hace
- Monitorea estado continuamente
- Muestra contadores en tiempo real
- Muestra temperatura
- Detecta cambios

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 8: MONITOREO EN TIEMPO REAL
  Monitoreo continuo de estado y contadores
==========================================

Conectando...
Conexión establecida

  Intervalo de actualización en segundos (default 2): [número]
  Número de actualizaciones (default 10, 0 = infinito): [número]

Iniciando monitoreo...
Presiona cualquier tecla para detener

--- Actualización 1 - 14:30:15 ---
  Estado: Imprimiendo
  Mensaje: mensaje1
  Contador Total: 1,234
  Contador OK: 1,200 (+5)
  Contador NOK: 34
  Copias: 1
  Temp. CPU: 45.23 °C

--- Actualización 2 - 14:30:17 ---
  Estado: Imprimiendo
  Mensaje: mensaje1
  Contador Total: 1,235
  Contador OK: 1,201 (+1)
  Contador NOK: 34
  Copias: 1
  Temp. CPU: 45.25 °C

...

Monitoreo detenido
```

**Nota**: El programa se ejecuta hasta que presiones una tecla o se alcance el número de actualizaciones.

### Tiempo Estimado
Variable (depende del intervalo y número de actualizaciones)

---

## Test 9: Manejo de Alarmas

**Archivo**: `Test09_Alarmas.cs`  
**StartupObject**: `MacsaLaserTest.Test09_Alarmas`

### Qué Hace
- Lee códigos de alarma
- Analiza alarm masks
- Monitorea cambios de alarma

### Qué Esperar

**Si funciona correctamente:**
```
==========================================
  PRUEBA 9: MANEJO DE ALARMAS
  Leer y analizar códigos de alarma
==========================================

Conectando...
Conexión establecida

PRUEBA 1: Leer códigos de alarma del estado
  Código de alarma (err): 0x00000000 (0)
  Alarm mask 1: 0x00000000
  Alarm mask 2: 0x00000000
  Signal state: 0x00000000
  Estado: Sin alarmas activas

(O si hay alarma:)
  Código de alarma (err): 0x00010001 (65537)
  Alarm mask 1: 0x00000001
  Alarm mask 2: 0x00000000
  Signal state: 0x00000000
  Estado: ALARMA ACTIVA
    Upper WORD (última alarma): 0x0001 (1)
    Lower WORD (estado): 0x0001 (1)
    Alarma actualmente activa

PRUEBA 2: Monitoreo continuo de alarmas
  ¿Deseas monitorear alarmas cada 2 segundos? (s/n): s
  Monitoreando... (Presiona cualquier tecla para detener)

[14:30:15] Cambio de alarma detectado:
  Código anterior: 0x00000000
  Código actual: 0x00010001
  Estado: ALARMA DETECTADA

Monitoreo detenido

Prueba de alarmas completada

Nota: Consulta el archivo alarmcodes.pdf para interpretar los códigos de alarma específicos
```

**Nota**: Consulta `alarmcodes.pdf` en `TCPIPver31\doc\` para interpretar códigos específicos.

### Tiempo Estimado
5-10 segundos (o más si activas monitoreo continuo)

---

## Manejo de Errores Comunes

### Error: "No se puede cargar el archivo o ensamblado 'SocketCommNet'"

**Causa**: La DLL no está en el directorio de salida.

**Solución**:
1. Verifica que `SocketCommNet.dll` esté en `bin\Debug\`
2. Si no está, cópiala desde `..\TCPIPver31\libs\x86\`
3. Recompila el proyecto

### Error: "Error al conectar: Código [número]"

**Causas posibles**:
- IP incorrecta
- Impresora apagada o no en la red
- Firewall bloqueando
- Puerto TCP/IP no disponible

**Solución**:
1. Verifica la IP de la impresora
2. Asegúrate de que la impresora esté encendida
3. Verifica conectividad con `ping [IP]`
4. Revisa configuración de firewall

### Error: "Error al obtener estado"

**Causa**: La impresora no responde o hay un problema de comunicación.

**Solución**:
1. Verifica que la conexión esté activa
2. Intenta desconectar y volver a conectar
3. Verifica que la impresora esté funcionando correctamente

### Error: "Archivo no encontrado" (Test 4)

**Causa**: La ruta del archivo es incorrecta o el archivo no existe.

**Solución**:
1. Verifica que la ruta sea correcta
2. Usa ruta absoluta completa (ej: `C:\Archivos\mensaje.msf`)
3. Verifica que el archivo tenga extensión `.msf`

### Error: "Error al obtener información" (Test 1)

**Causa**: Alguna función no está disponible o hay un problema de comunicación.

**Solución**:
1. Revisa qué información específica falló (está en el mensaje de error)
2. Algunas impresoras pueden no soportar todas las funciones
3. Verifica la versión del firmware de la impresora

### El programa se cuelga o no responde

**Causa**: Timeout o la impresora no responde.

**Solución**:
1. Presiona Ctrl+C para cancelar
2. Verifica que la impresora esté funcionando
3. Intenta aumentar el timeout (si está disponible en la configuración)

---

## Orden Recomendado de Pruebas

1. **Test 1** (ProgramInfoTest) - Verificar conexión básica primero
2. **Test 2** (UserMessages) - Funcionalidad simple
3. **Test 3** (ControlImpresion) - Control básico
4. **Test 4** (EnviarArchivos) - Si necesitas subir archivos
5. **Test 5** (GestionarArchivos) - Administración
6. **Test 6** (Contadores) - Si trabajas con contadores
7. **Test 7** (Configuracion) - Ajustes avanzados
8. **Test 8** (Monitoreo) - Monitoreo continuo
9. **Test 9** (Alarmas) - Manejo de errores

---

## Próximos Pasos

Una vez que todas las pruebas funcionen correctamente:

1. **Integración**: Combina las funcionalidades en clases modulares
2. **Interfaz Gráfica**: Crea una aplicación con Windows Forms o WPF
3. **Manejo de Errores Robusto**: Implementa try-catch y logging
4. **Configuración Persistente**: Guarda IP y configuraciones
5. **Logging**: Registra todas las operaciones para debugging

---

## Notas Importantes

- **Siempre cierra la conexión correctamente**: Cada prueba lo hace automáticamente
- **No ejecutes múltiples pruebas simultáneamente**: Solo una conexión a la vez
- **Algunas funciones pueden no estar disponibles**: Depende del modelo y firmware
- **Los cambios son en tiempo real**: Afectan la impresora inmediatamente
- **Ten cuidado con eliminar archivos**: La acción es permanente

---

## Soporte

Si encuentras errores:
1. Anota el mensaje de error exacto
2. Indica qué prueba estabas ejecutando
3. Verifica la versión del firmware de la impresora
4. Consulta la documentación en `TCPIPver31\doc\`

