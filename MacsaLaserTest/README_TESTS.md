# Guía de Pruebas Modulares - Macsa Laser

Este documento explica cómo usar cada prueba independiente para verificar las funcionalidades de la impresora laser Macsa.

## Estructura de Pruebas

Cada prueba está en un archivo separado y es completamente independiente. Esto permite:
- Probar cada funcionalidad de forma aislada
- Identificar fácilmente dónde falla algo
- Integrar las funcionalidades que funcionan en una aplicación completa

## Configuración Común

**IMPORTANTE**: Antes de ejecutar cualquier prueba, edita la IP de la impresora en cada archivo:

```csharp
string ipImpresora = "192.168.16.180";  // CAMBIA ESTA IP
```

## Cómo Ejecutar Cada Prueba

### Opción 1: Cambiar StartupObject en el proyecto

Edita `MacsaLaserTest.csproj` y cambia la línea:
```xml
<StartupObject>MacsaLaserTest.ProgramInfoTest</StartupObject>
```

Por el nombre de la prueba que quieres ejecutar:
- `MacsaLaserTest.Test02_UserMessages`
- `MacsaLaserTest.Test03_ControlImpresion`
- etc.

### Opción 2: Compilar cada prueba como ejecutable separado

Cada prueba tiene su propio método `Main()`, así que puedes compilarlas individualmente.

## Lista de Pruebas

### Test 1: Información Completa (Ya implementado)
**Archivo**: `ProgramInfoTest.cs`
**Funcionalidad**: Obtiene toda la información disponible de la impresora
**Uso**: Ejecutar para verificar conexión y obtener datos del sistema

---

### Test 2: Mensajes de Usuario
**Archivo**: `Test02_UserMessages.cs`
**Funcionalidad**: 
- Enviar texto dinámico a campos de la impresora
- Leer mensajes actuales
- Trabajar con múltiples campos

**Qué prueba**:
1. Enviar mensaje simple a campo 0
2. Leer mensaje del campo 0
3. Enviar múltiples mensajes a diferentes campos
4. Leer todos los campos enviados

**Cuándo usar**: Cuando necesites enviar texto dinámico a la impresora (fechas, números de serie, etc.)

---

### Test 3: Control de Impresión
**Archivo**: `Test03_ControlImpresion.cs`
**Funcionalidad**:
- Detener impresión
- Recargar archivo actual
- Trigger de impresión (disparo software)
- Iniciar impresión con archivo específico

**Qué prueba**:
1. Detener impresión actual
2. Recargar archivo
3. Disparar impresión por software
4. Iniciar impresión con archivo específico

**Cuándo usar**: Para controlar el proceso de impresión (start/stop)

---

### Test 4: Enviar Archivos
**Archivo**: `Test04_EnviarArchivos.cs`
**Funcionalidad**:
- Copiar archivos .msf desde tu PC a la impresora
- Elegir destino (RAM o disco duro)

**Qué prueba**:
1. Enviar archivo .msf a la impresora
2. Verificar que el archivo se copió correctamente
3. Listar archivos en la impresora

**Cuándo usar**: Cuando necesites subir nuevos diseños a la impresora

---

### Test 5: Gestionar Archivos
**Archivo**: `Test05_GestionarArchivos.cs`
**Funcionalidad**:
- Listar archivos en la impresora
- Establecer archivo por defecto
- Eliminar archivos

**Qué prueba**:
1. Listar todos los archivos .msf
2. Establecer un archivo como por defecto
3. Verificar archivo actual
4. Eliminar archivo (con confirmación)

**Cuándo usar**: Para administrar los archivos almacenados en la impresora

---

### Test 6: Contadores
**Archivo**: `Test06_Contadores.cs`
**Funcionalidad**:
- Leer contadores del estado
- Leer/establecer contadores globales
- Establecer contadores privados
- Resetear contadores

**Qué prueba**:
1. Leer contadores del estado (total, OK, NOK)
2. Leer contador global de un campo
3. Establecer contador global
4. Establecer contador privado (repeticiones/impresiones)
5. Resetear contadores

**Cuándo usar**: Para trabajar con contadores de producción

---

### Test 7: Configuración
**Archivo**: `Test07_Configuracion.cs`
**Funcionalidad**:
- Modo de impresión (default, UMT, batchjob)
- Modo estático/dinámico
- Offset (X, Y)
- Defocus (Z)
- Powerscale

**Qué prueba**:
1. Leer/establecer modo de impresión
2. Leer/establecer modo estático/dinámico
3. Establecer offset en X e Y
4. Establecer defocus en Z
5. Leer/establecer powerscale

**Cuándo usar**: Para ajustar parámetros de impresión y calibración

---

### Test 8: Monitoreo en Tiempo Real
**Archivo**: `Test08_Monitoreo.cs`
**Funcionalidad**:
- Monitoreo continuo del estado
- Seguimiento de contadores
- Monitoreo de temperatura
- Detección de cambios

**Qué prueba**:
1. Monitoreo continuo cada X segundos
2. Mostrar estado, contadores y temperatura
3. Detectar cambios en contadores
4. Detectar alarmas

**Cuándo usar**: Para monitorear la impresora durante la producción

---

### Test 9: Manejo de Alarmas
**Archivo**: `Test09_Alarmas.cs`
**Funcionalidad**:
- Leer códigos de alarma
- Analizar alarm masks
- Monitoreo continuo de alarmas
- Detectar cambios de estado de alarma

**Qué prueba**:
1. Leer códigos de alarma del estado
2. Analizar estructura del código de alarma
3. Leer alarm masks
4. Monitoreo continuo de cambios de alarma

**Cuándo usar**: Para detectar y manejar problemas en la impresora

**Nota**: Consulta `alarmcodes.pdf` para interpretar los códigos específicos

---

## Orden Recomendado de Pruebas

1. **Test 1** (ProgramInfoTest) - Verificar conexión básica
2. **Test 2** (UserMessages) - Envío de texto simple
3. **Test 3** (ControlImpresion) - Control básico
4. **Test 4** (EnviarArchivos) - Si necesitas subir archivos
5. **Test 5** (GestionarArchivos) - Administración de archivos
6. **Test 6** (Contadores) - Si trabajas con contadores
7. **Test 7** (Configuracion) - Ajustes avanzados
8. **Test 8** (Monitoreo) - Monitoreo continuo
9. **Test 9** (Alarmas) - Manejo de errores

## Integración

Una vez que hayas probado y verificado que cada funcionalidad funciona:

1. Crea clases modulares para cada funcionalidad (similar a `LaserConnection` y `LaserInfo`)
2. Integra las clases en una aplicación principal
3. Crea una interfaz gráfica que use todas las funcionalidades

## Solución de Problemas

### Error de conexión
- Verifica la IP de la impresora
- Asegúrate de que la impresora esté encendida
- Verifica que no haya firewall bloqueando

### Error en una prueba específica
- Revisa el código de esa prueba
- Verifica que la funcionalidad esté soportada por tu modelo de impresora
- Consulta la documentación del protocolo TCP/IP

### DLL no encontrada
- Asegúrate de que `SocketCommDll.dll` esté en `bin\Debug\`
- Verifica que `SocketCommNet.dll` también esté presente

## Próximos Pasos

Una vez que todas las pruebas funcionen:
1. Crea clases modulares para cada funcionalidad
2. Integra todo en una aplicación principal
3. Crea interfaz gráfica (Windows Forms o WPF)
4. Agrega manejo de errores robusto
5. Implementa logging

