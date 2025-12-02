# 🔌 TCPIPver31 - Documentación Técnica Completa

## 📋 Índice
- [Descripción General](#descripción-general)
- [Estructura de Carpetas](#estructura-de-carpetas)
- [Arquitectura de Comunicación](#arquitectura-de-comunicación)
- [SocketCommNet (Wrapper .NET)](#socketcommnet-wrapper-net)
- [SocketCommDll.dll (DLL Nativa)](#socketcommdlldll-dll-nativa)
- [Flujo de Llamadas](#flujo-de-llamadas)
- [Configuración de Proyectos](#configuración-de-proyectos)
- [Ejemplos de Código](#ejemplos-de-código)
- [Troubleshooting](#troubleshooting)

---

## 🎯 Descripción General

**TCPIPver31** es el protocolo y biblioteca de comunicación TCP/IP para impresoras láser MACSA. Consiste en:

1. **SocketCommDll.dll** - DLL nativa (C/C++) que maneja la comunicación TCP/IP de bajo nivel
2. **SocketCommNet** - Wrapper .NET que expone funciones de la DLL de forma type-safe
3. **Documentación** - PDFs con especificaciones del protocolo

---

## 📁 Estructura de Carpetas

```
TCPIPver31/
├── libs/                          # 📚 Bibliotecas nativas (DLLs)
│   ├── x64/                       # Versión 64-bit
│   │   ├── SocketCommDll.dll      # ⭐ DLL principal (64-bit)
│   │   ├── SocketCommDll.lib      # Librería de importación
│   │   ├── SocketCommDll.h        # Header C/C++
│   │   └── SocketCommDll.pdb      # Símbolos de debug
│   │
│   └── x86/                       # Versión 32-bit
│       ├── SocketCommDll.dll      # DLL principal (32-bit)
│       ├── SocketCommDll.lib
│       ├── SocketCommDll.h
│       └── SocketCommDll.pdb
│
├── SocketCommNet/                 # 🔷 Wrapper .NET
│   ├── SocketComm.cs              # ⭐ Clase principal (wrapper)
│   ├── SocketCommNet.csproj       # Proyecto .NET
│   └── bin/                       # Binarios compilados
│       └── Debug/
│           └── net8.0/
│               └── SocketCommNet.dll
│
├── doc/                           # 📖 Documentación
│   ├── tcpprotocol.pdf           # Protocolo TCP/IP
│   ├── alarmcodes.pdf            # Códigos de alarma
│   ├── configuration_file.pdf    # Configuración
│   └── ...
│
└── [Proyectos de prueba]          # 🧪 Tests y ejemplos
    ├── NetDllTest/                # Test en C#
    ├── NetDllTestVB/              # Test en VB.NET
    └── SocketCommDllTest/         # Test completo
```

---

## 🏗️ Arquitectura de Comunicación

### Capas de Comunicación

```
┌─────────────────────────────────────────────────────────┐
│           APLICACIÓN (LaserMacsaUser)                  │
│  LaserService.cs → SocketComm.CS_Init()                │
└───────────────────────┬─────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────┐
│         SOCKETCOMMNET (Wrapper .NET)                   │
│  SocketComm.cs                                          │
│  - CS_Init() → Convierte String a UInt16[]             │
│  - Llama a MInit() (DllImport)                         │
└───────────────────────┬─────────────────────────────────┘
                        │
                        │ [DllImport("SocketCommDll.dll")]
                        │ P/Invoke (Platform Invoke)
                        ▼
┌─────────────────────────────────────────────────────────┐
│      SOCKETCOMMDLL.DLL (DLL Nativa C/C++)              │
│  Funciones exportadas:                                  │
│  - MInit()                                              │
│  - MStartClient()                                       │
│  - MLaser_Status()                                      │
│  - etc.                                                 │
└───────────────────────┬─────────────────────────────────┘
                        │
                        │ Socket TCP/IP
                        │ Puerto: (configurado en láser)
                        ▼
┌─────────────────────────────────────────────────────────┐
│         IMPRESORA LÁSER MACSA                          │
│  IP: 192.168.0.180 (configurable)                      │
│  Protocolo: TCP/IP v3.1                                │
└─────────────────────────────────────────────────────────┘
```

---

## 🔷 SocketCommNet (Wrapper .NET)

### Ubicación
**Carpeta**: `TCPIPver31/SocketCommNet/`

**Archivo principal**: `SocketComm.cs`

**Namespace**: `SocketCommNet`

### Propósito
Wrapper .NET que:
1. Expone funciones de la DLL nativa de forma type-safe
2. Convierte tipos .NET (String) a tipos nativos (UInt16[])
3. Maneja marshalling de estructuras
4. Proporciona métodos públicos con nombres más claros (CS_*)

### Estructura del Código

#### 1. Declaraciones DllImport

```csharp
// En SocketComm.cs (líneas 156-226)

// Importar función de la DLL nativa
[DllImport("SocketCommDll.dll")]
private static extern void MInit(
    ref Int32 p,           // Puntero/handle de conexión
    UInt16[] name,          // Nombre de conexión (UTF-16)
    UInt16[] ip,            // IP del láser (UTF-16)
    UInt16[] path           // Ruta local (UTF-16)
);

[DllImport("SocketCommDll.dll")]
private static extern Int32 MStartClient(Int32 p);

[DllImport("SocketCommDll.dll")]
private static extern Int32 MLaser_Status(
    Int32 p, 
    out PStatus status      // Estructura de estado
);
```

**Explicación**:
- `[DllImport]` indica que la función está en una DLL externa
- `"SocketCommDll.dll"` es el nombre de la DLL (se busca en PATH o directorio de ejecución)
- `extern` significa que la implementación está en la DLL
- `static` porque son funciones de C (no métodos de clase)

#### 2. Métodos Públicos Wrapper

```csharp
// Método público que convierte String a UInt16[] y llama a la DLL
public void CS_Init(ref Int32 p, String name, String ip, String path)
{
    // Convertir String a UInt16[] (UTF-16)
    UInt16[] aname = new UInt16[name.Length];
    for (int i = 0; i < name.Length; i++)
    {
        aname[i] = name[i];
    }
    
    UInt16[] aip = new UInt16[ip.Length];
    for (int i = 0; i < ip.Length; i++)
    {
        aip[i] = ip[i];
    }
    
    UInt16[] apath = new UInt16[path.Length];
    for (int i = 0; i < path.Length; i++)
    {
        apath[i] = path[i];
    }
    
    // Llamar a la función nativa
    MInit(ref p, aname, aip, apath);
}
```

**Por qué se necesita**:
- La DLL nativa espera arrays de `UInt16` (UTF-16)
- .NET usa `String` (UTF-16 internamente, pero diferente representación)
- El wrapper convierte automáticamente

#### 3. Estructuras Marshalled

```csharp
// Estructura privada para la DLL (exacta a C/C++)
[StructLayout(LayoutKind.Sequential)]
private struct PStatus
{
    public UInt32 d_counter;    // Contador OK
    public UInt32 s_counter;    // Contador NOK
    public Byte Start;          // Estado (0=imprimiendo, 1=detenido)
    // ... más campos
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public Byte[] name;         // Nombre de archivo (8 bytes)
}

// Estructura pública para .NET (más amigable)
[StructLayout(LayoutKind.Sequential)]
public struct CSStatus
{
    public UInt32 d_counter;
    public UInt32 s_counter;
    public Byte Start;
    public String name;         // String en lugar de Byte[]
    // ...
}
```

**Marshalling**:
- `[StructLayout(LayoutKind.Sequential)]` asegura que los campos estén en el mismo orden que en C
- `[MarshalAs]` especifica cómo convertir tipos (Byte[] → String)

---

## 📚 SocketCommDll.dll (DLL Nativa)

### Ubicación Física

**Desarrollo**:
```
TCPIPver31/libs/x64/SocketCommDll.dll    # 64-bit
TCPIPver31/libs/x86/SocketCommDll.dll    # 32-bit
```

**Runtime (después de compilar)**:
```
LaserMacsaUser/bin/Debug/net8.0-windows/SocketCommDll.dll
MacsaLaserTest/bin/Debug/SocketCommDll.dll
```

### ¿Cómo se Copia la DLL?

#### En SocketCommNet.csproj

```xml
<ItemGroup>
  <!-- DLL 64-bit -->
  <None Include="..\libs\x64\SocketCommDll.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>SocketCommDll.dll</Link>
  </None>
  
  <!-- DLL 32-bit (opcional, para compatibilidad) -->
  <None Include="..\libs\x86\SocketCommDll.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>x86\SocketCommDll.dll</Link>
  </None>
</ItemGroup>
```

**Explicación**:
- `Include="..\libs\x64\SocketCommDll.dll"` - Ruta relativa al archivo fuente
- `CopyToOutputDirectory="PreserveNewest"` - Copia al directorio de salida si es más nuevo
- `Link="SocketCommDll.dll"` - Nombre en el proyecto (puede ser diferente al archivo fuente)

#### En LaserMacsaUser.csproj

```xml
<ItemGroup>
  <None Include="..\TCPIPver31\libs\x64\SocketCommDll.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>SocketCommDll.dll</Link>
  </None>
</ItemGroup>
```

**Resultado**: Al compilar, la DLL se copia automáticamente a `bin/Debug/net8.0-windows/`

### Búsqueda de la DLL en Runtime

Cuando se ejecuta `[DllImport("SocketCommDll.dll")]`, .NET busca la DLL en este orden:

1. **Directorio de la aplicación ejecutante**
   ```
   LaserMacsaUser/bin/Debug/net8.0-windows/SocketCommDll.dll
   ```

2. **Directorio del sistema** (Windows/System32)

3. **PATH del sistema**

4. **Directorio de trabajo actual**

**⚠️ IMPORTANTE**: La DLL debe estar en el mismo directorio que el ejecutable o en el PATH.

---

## 🔄 Flujo de Llamadas Completo

### Ejemplo: Inicializar Conexión

```csharp
// 1. EN LA APLICACIÓN (LaserService.cs)
using SocketCommNet;

var socketComm = new SocketComm();
Int32 handle = 0;
string ip = "192.168.0.180";
string name = "MyConnection";
string path = ".\\";

// 2. LLAMAR AL WRAPPER
socketComm.CS_Init(ref handle, name, ip, path);
//     │
//     └─→ SocketComm.CS_Init() (método público)
//         │
//         ├─→ Convierte String → UInt16[]
//         │
//         └─→ MInit(ref handle, aname, aip, apath)
//             │
//             └─→ [DllImport] busca SocketCommDll.dll
//                 │
//                 └─→ Carga DLL desde:
//                     - bin/Debug/net8.0-windows/SocketCommDll.dll
//                     - O PATH del sistema
//                     │
//                     └─→ Ejecuta función nativa MInit()
//                         │
//                         └─→ Crea socket TCP/IP
//                             │
//                             └─→ Se conecta a 192.168.0.180
```

### Ejemplo: Obtener Estado

```csharp
// 1. EN LA APLICACIÓN
SocketComm.CSStatusExt status = new SocketComm.CSStatusExt();
Int32 result = socketComm.CS_StatusExt(handle, ref status);

// 2. FLUJO INTERNO
socketComm.CS_StatusExt()
  │
  ├─→ Convierte CSStatusExt → PStatusExt (estructura privada)
  │
  └─→ MLaser_StatusExt(handle, out pStatusExt)
      │
      └─→ [DllImport] → SocketCommDll.dll
          │
          └─→ Función nativa MLaser_StatusExt()
              │
              ├─→ Envía comando TCP/IP al láser
              │
              ├─→ Recibe respuesta
              │
              └─→ Llena estructura PStatusExt
                  │
                  └─→ Wrapper convierte PStatusExt → CSStatusExt
                      │
                      └─→ Retorna a la aplicación
```

---

## ⚙️ Configuración de Proyectos

### Referencia a SocketCommNet

#### En LaserMacsaUser.csproj

```xml
<ItemGroup>
  <!-- Referencia al proyecto SocketCommNet -->
  <ProjectReference Include="..\TCPIPver31\SocketCommNet\SocketCommNet.csproj" />
</ItemGroup>
```

**Qué hace**:
- Agrega `SocketCommNet.dll` como dependencia
- Compila SocketCommNet si es necesario
- Copia `SocketCommNet.dll` al directorio de salida

#### En MacsaLaserTest.csproj

```xml
<ItemGroup>
  <!-- Referencia a DLL compilada (no proyecto) -->
  <Reference Include="SocketCommNet">
    <HintPath>..\TCPIPver31\libs\x86\SocketCommNet.dll</HintPath>
    <Private>True</Private>
    <CopyLocal>True</CopyLocal>
  </Reference>
</ItemGroup>
```

**Diferencia**:
- `ProjectReference`: Referencia a proyecto (compila junto)
- `Reference`: Referencia a DLL ya compilada

### Copia de SocketCommDll.dll

#### Opción 1: Desde SocketCommNet (Recomendado)

Si `SocketCommNet.csproj` ya copia la DLL, se copiará automáticamente cuando se referencia el proyecto.

#### Opción 2: Copia Manual

```xml
<!-- En LaserMacsaUser.csproj -->
<ItemGroup>
  <None Include="..\TCPIPver31\libs\x64\SocketCommDll.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>SocketCommDll.dll</Link>
  </None>
</ItemGroup>
```

---

## 💻 Ejemplos de Código

### Ejemplo 1: Conexión Básica

```csharp
using SocketCommNet;

// 1. Crear instancia
SocketComm socketComm = new SocketComm();
Int32 handle = 0;

// 2. Inicializar
string name = "MyConnection";
string ip = "192.168.0.180";  // IP del láser
string path = ".\\";           // Ruta local (cualquier ruta válida)

socketComm.CS_Init(ref handle, name, ip, path);

// 3. Verificar errores de inicialización
string errorMsg = "";
Int32 errorCode = socketComm.CS_GetLastError(handle, ref errorMsg);
if (errorCode != 0)
{
    Console.WriteLine($"Error: {errorMsg}");
    return;
}

// 4. Conectar
Int32 result = socketComm.CS_StartClient(handle);
if (result != 0)
{
    errorMsg = "";
    socketComm.CS_GetLastError(handle, ref errorMsg);
    Console.WriteLine($"Error al conectar: {errorMsg}");
    return;
}

// 5. Verificar conexión
Int32 isConnected = socketComm.CS_IsConnected(handle);
if (isConnected == 1)
{
    Console.WriteLine("¡Conectado exitosamente!");
}

// 6. Cerrar conexión
socketComm.CS_Knockout(handle);  // Notificar al láser
socketComm.CS_Finish(handle);    // Cerrar socket
```

### Ejemplo 2: Obtener Estado

```csharp
// Obtener estado extendido
SocketComm.CSStatusExt status = new SocketComm.CSStatusExt();
Int32 result = socketComm.CS_StatusExt(handle, ref status);

if (result == 0)
{
    Console.WriteLine($"Archivo activo: {status.messagename}");
    Console.WriteLine($"Contador OK: {status.d_counter}");
    Console.WriteLine($"Contador NOK: {status.s_counter}");
    Console.WriteLine($"Total: {status.t_counter}");
    Console.WriteLine($"Estado: {(status.Start == 0 ? "Imprimiendo" : "Detenido")}");
}
```

### Ejemplo 3: Enviar Mensaje de Usuario

```csharp
// Enviar texto a campo 0
string message = "Hola desde C#";
Int32 result = socketComm.CS_FastASCIIUsermessage(handle, 0, message);

if (result == 0)
{
    Console.WriteLine("Mensaje enviado");
}
else
{
    string error = "";
    socketComm.CS_GetLastError(handle, ref error);
    Console.WriteLine($"Error: {error}");
}
```

### Ejemplo 4: Iniciar Impresión

```csharp
// Iniciar impresión con archivo específico
string filename = "mi_archivo";  // Sin extensión
int copies = 1;                   // Número de copias

Int32 result = socketComm.CS_Start(handle, filename, copies);

if (result == 0)
{
    Console.WriteLine("Impresión iniciada");
}
```

---

## 🔍 Troubleshooting

### Error: "No se puede cargar la DLL 'SocketCommDll.dll'"

**Causas posibles**:
1. La DLL no está en el directorio de ejecución
2. Arquitectura incorrecta (x86 vs x64)
3. Dependencias faltantes (Visual C++ Redistributable)

**Solución**:
```bash
# Verificar que la DLL esté en el directorio de salida
dir bin\Debug\net8.0-windows\SocketCommDll.dll

# Verificar arquitectura
dumpbin /headers SocketCommDll.dll | findstr "machine"
```

### Error: "BadImageFormatException"

**Causa**: Incompatibilidad de arquitectura (x86 vs x64)

**Solución**:
- Asegurar que la aplicación y la DLL usen la misma arquitectura
- Para .NET Framework: Configurar `PlatformTarget` en `.csproj`
- Para .NET Core/8: Usar la DLL correcta (x64 o x86)

### La DLL no se copia al directorio de salida

**Verificar**:
1. Que el archivo existe en `TCPIPver31/libs/x64/`
2. Que el `.csproj` tiene la configuración correcta:
   ```xml
   <None Include="..\TCPIPver31\libs\x64\SocketCommDll.dll">
     <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
   </None>
   ```
3. Recompilar el proyecto

---

## 📖 Documentación Adicional

### Archivos PDF en `doc/`

- **tcpprotocol.pdf** - Especificación completa del protocolo TCP/IP
- **alarmcodes.pdf** - Códigos de alarma y sus significados
- **configuration_file.pdf** - Formato de archivos de configuración
- **Dynamic .NET library for TCP.pdf** - Documentación de la biblioteca

### Headers C/C++

**Ubicación**: `TCPIPver31/libs/x64/SocketCommDll.h`

**Contiene**: Declaraciones de funciones en C/C++

**Uso**: Para desarrolladores que quieran usar la DLL directamente desde C/C++

---

## 🔧 Mantenimiento y Actualización

### Actualizar la DLL

1. **Reemplazar archivo**:
   ```
   TCPIPver31/libs/x64/SocketCommDll.dll  (nuevo)
   TCPIPver31/libs/x86/SocketCommDll.dll  (nuevo)
   ```

2. **Recompilar proyectos**:
   ```bash
   dotnet build TCPIPver31/SocketCommNet/SocketCommNet.csproj
   dotnet build LaserMacsaUser/LaserMacsaUser.csproj
   ```

3. **Verificar versión**:
   ```csharp
   int version = socketComm.CS_GetDllVersion();
   Console.WriteLine($"Versión DLL: {version}");
   ```

### Agregar Nueva Función

Si se agrega una nueva función a la DLL:

1. **Agregar DllImport en SocketComm.cs**:
   ```csharp
   [DllImport("SocketCommDll.dll")]
   private static extern Int32 MLaser_NewFunction(Int32 p, Int32 param);
   ```

2. **Crear método público wrapper**:
   ```csharp
   public Int32 CS_NewFunction(Int32 p, Int32 param)
   {
       return MLaser_NewFunction(p, param);
   }
   ```

3. **Documentar en este README**

---

## 📝 Resumen de Ubicaciones

| Componente | Ubicación Desarrollo | Ubicación Runtime |
|------------|----------------------|-------------------|
| **SocketCommDll.dll (x64)** | `TCPIPver31/libs/x64/` | `bin/Debug/net8.0-windows/` |
| **SocketCommDll.dll (x86)** | `TCPIPver31/libs/x86/` | `bin/Debug/net8.0-windows/x86/` |
| **SocketCommNet.dll** | `TCPIPver31/SocketCommNet/bin/` | `bin/Debug/net8.0-windows/` |
| **SocketComm.cs** | `TCPIPver31/SocketCommNet/` | (compilado en SocketCommNet.dll) |

---

## 🔗 Referencias

- [LaserMacsaUser/README.md](../LaserMacsaUser/README.md) - Arquitectura de la aplicación
- [LaserMacsaUser/Services/README.md](../LaserMacsaUser/Services/README.md) - Uso de LaserService
- [MacsaLaserTest/README.md](../MacsaLaserTest/README.md) - Tests de conectividad

---

**Última actualización**: 2025-11-23  
**Versión del Protocolo**: TCP/IP v3.1

