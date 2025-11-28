# Proyecto de Prueba - Conexión Macsa Laser TCP/IP

Este proyecto es una implementación básica paso a paso para conectar con impresoras laser Macsa usando el protocolo TCP/IP v3.1.

## Requisitos

- .NET Framework 4.7.2 o superior
- Visual Studio 2017 o superior (o cualquier IDE compatible)
- Las DLLs de SocketCommNet deben estar en la ruta: `..\TCPIPver31\libs\x86\`
- La DLL nativa `SocketCommDll.dll` debe estar en el mismo directorio que el ejecutable o en el PATH del sistema

## Configuración

Antes de ejecutar el proyecto, edita el archivo `Program.cs` y cambia estos valores:

```csharp
string ipImpresora = "192.168.16.180";  // Cambia por la IP de tu impresora
string rutaLocal = "C:\\Fly";            // Ruta local (puede ser cualquier ruta)
```

## Estructura del Proyecto

El proyecto está organizado en pasos claros:

1. **PASO 1**: Configuración básica de conexión
2. **PASO 2**: Inicialización de la conexión
3. **PASO 3**: Establecer conexión con la impresora
4. **PASO 4**: Verificar estado de la conexión
5. **PASO 5**: Obtener información básica de la impresora
6. **PASO 6**: Cerrar la conexión correctamente

## Compilación

1. Abre el proyecto en Visual Studio
2. Asegúrate de que la referencia a `SocketCommNet.dll` esté correcta
3. Compila el proyecto (F6)
4. Ejecuta el programa (F5)

## Solución de Problemas

### Error de conexión
- Verifica que la IP de la impresora sea correcta
- Asegúrate de que la impresora esté encendida y en la red
- Verifica que no haya un firewall bloqueando la conexión
- Comprueba que el puerto TCP/IP esté disponible

### Error de DLL no encontrada
- Asegúrate de que `SocketCommDll.dll` esté en el directorio de salida (`bin\Debug\` o `bin\Release\`)
- O coloca la DLL en una ruta que esté en el PATH del sistema

## Próximos Pasos

Una vez que la conexión básica funcione, puedes agregar:
- Envío de mensajes de usuario (User Messages)
- Control de impresión (Start/Stop)
- Lectura de contadores
- Y más funcionalidades según tus necesidades

