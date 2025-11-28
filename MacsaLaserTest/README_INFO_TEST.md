# Programa de Prueba de Información - Macsa Laser

Este programa obtiene toda la información disponible de la impresora laser Macsa de forma modular.

## Estructura Modular

El proyecto está dividido en módulos independientes para facilitar el debugging:

### 1. `LaserConnection.cs`
**Responsabilidad**: Manejar la conexión TCP/IP con la impresora
- Inicializar conexión
- Conectar/Desconectar
- Verificar estado de conexión
- Manejo de errores de conexión

**Ventaja**: Si falla la conexión, el error está aislado en este archivo.

### 2. `LaserInfo.cs`
**Responsabilidad**: Obtener información de la impresora
- Estado extendido (contadores, mensaje activo, etc.)
- Información del sistema (disco, RAM, horas de trabajo)
- Información de hardware (temperatura, voltajes, ventiladores)
- Lista de archivos
- Datos de conexión

**Ventaja**: Si falla al obtener información, el error está aislado en este archivo.

### 3. `LaserInfoResult.cs`
**Responsabilidad**: Modelo de datos con toda la información
- Almacena toda la información obtenida
- Método `MostrarInformacion()` para mostrar los datos de forma legible

**Ventaja**: Estructura clara de datos, fácil de extender.

### 4. `ProgramInfoTest.cs`
**Responsabilidad**: Programa principal de prueba
- Orquesta todos los módulos
- Maneja el flujo de ejecución
- Muestra resultados

**Ventaja**: Lógica simple, fácil de entender y modificar.

## Cómo Usar

### 1. Configurar la IP
Edita `ProgramInfoTest.cs` línea ~18:
```csharp
string ipImpresora = "192.168.16.180";  // ⚠️ CAMBIA ESTA IP
```

### 2. Compilar
```powershell
msbuild MacsaLaserTest.csproj /p:Configuration=Debug
```

### 3. Ejecutar
```powershell
.\bin\Debug\MacsaLaserTest.exe
```

## Información que se Obtiene

### Versión
- Versión de la DLL
- Versión del protocolo
- Versión del firmware

### Estado Actual
- Mensaje activo
- Event handler
- Estado de impresión (Imprimiendo/Detenido)
- Copias a imprimir
- Código de alarma

### Contadores
- Contador total
- Contador OK (impresiones exitosas)
- Contador NOK (impresiones fallidas)
- Contador total largo

### Sistema
- Horas de trabajo
- Temperatura CPU

### Almacenamiento
- Espacio total y disponible en disco
- Espacio total y disponible en RAM

### Hardware
- Temperatura del board
- Humedad
- Voltajes (5V y 3.3V)
- Temperatura de ventiladores (local y remoto)
- Velocidad del ventilador

### Archivos
- Lista de archivos .msf en la impresora

### Conexión
- Leading byte
- Control byte
- Found mask

## Ventajas de la Estructura Modular

1. **Fácil Debugging**: Si algo falla, sabes exactamente en qué archivo está el problema
2. **Reutilizable**: Puedes usar `LaserConnection` y `LaserInfo` en otros proyectos
3. **Mantenible**: Cada clase tiene una responsabilidad clara
4. **Extensible**: Fácil agregar nuevas funcionalidades sin afectar el resto

## Solución de Problemas

### Error de conexión
- Verifica la IP de la impresora
- Asegúrate de que la impresora esté encendida
- Verifica que no haya firewall bloqueando

### Error al obtener información
- Revisa `LaserInfo.cs` - cada método está separado
- Si falla un método específico, los demás seguirán funcionando
- Revisa los mensajes de error en `LaserInfoResult.MensajeError`

### DLL no encontrada
- Asegúrate de que `SocketCommDll.dll` esté en `bin\Debug\`
- Verifica que `SocketCommNet.dll` también esté presente

## Próximos Pasos

Una vez que la obtención de información funcione, puedes:
- Agregar más información (si la impresora la soporta)
- Crear una interfaz gráfica que use estos módulos
- Guardar la información en un archivo
- Monitorear cambios en tiempo real

