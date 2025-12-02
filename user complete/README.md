# IPFUser - Sistema de Marcado Láser Industrial

## 📋 Descripción General

**IPFUser** es una aplicación Windows Forms desarrollada en C# (.NET Framework 4.7.2) que gestiona el marcado láser industrial con integración de bases de datos SQL Server, comunicación con PLCs mediante Modbus TCP, y control de dispositivos Macsa. El sistema está diseñado para gestionar promociones de códigos, controlar la producción de marcado láser, y mantener un historial completo de operaciones.

**Versión:** 3.0.0.2  
**Framework:** .NET Framework 4.7.2  
**Tipo:** Aplicación Windows Forms

---

## 🏗️ Arquitectura del Sistema

### Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    IPFUser (Aplicación Principal)            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  frmMain     │  │  frmSetup    │  │  frmSetArt   │      │
│  │  (UI Principal)│  │  (Config)   │  │  (Artworks)  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
         ├────────────────────┼────────────────────┤
         │                    │                    │
    ┌────▼────┐         ┌────▼────┐         ┌────▼────┐
    │  CLaser │         │   PLC   │         │  DataBases│
    │ (Control│         │(ModbusTCP)│       │(SQL Server)│
    │  Láser) │         │         │         │           │
    └─────────┘         └─────────┘         └───────────┘
         │                    │                    │
         └────────────────────┼────────────────────┘
                              │
                    ┌─────────▼─────────┐
                    │  MacsaDevicesNet   │
                    │  (Dispositivos)    │
                    └───────────────────┘
```

### Capas del Sistema

1. **Capa de Presentación (UI)**
   - Windows Forms
   - Formularios de configuración y control
   - Interfaz de usuario bilingüe (Español/Inglés)

2. **Capa de Lógica de Negocio**
   - Gestión de promociones
   - Control de producción
   - Gestión de códigos y lotes

3. **Capa de Acceso a Datos**
   - SQL Server Management Objects (SMO)
   - DataBases (Abstracción de acceso a datos)
   - Gestión de bases de datos adjuntas

4. **Capa de Comunicación**
   - Modbus TCP (PLC)
   - Protocolo láser (LaserDLL)
   - Dispositivos Macsa

---

## 📦 Componentes Principales

### 1. **IPFUser** (Proyecto Principal)

#### Archivos Clave:

**`frmMain.cs`** - Formulario Principal
- **Propósito:** Interfaz principal de la aplicación
- **Funciones principales:**
  - Control de inicio/parada de producción
  - Gestión de colas de códigos (Queue1, Queue2, QueueDataString1, QueueDataString2)
  - Sincronización con el láser mediante `SincroMarkState` timer
  - Visualización de progreso (barras de progreso, contadores)
  - Gestión de advertencias de bajo nivel de códigos
  - Comunicación con PLC para promociones multicapa
  - Generación de histórico de producción

- **Colas de Datos:**
  - `Queue1/Queue2`: Colas para códigos de texto simple
  - `QueueDataString1/QueueDataString2`: Colas para códigos DataMatrix
  - Sistema de doble cola para evitar bloqueos durante la producción

- **Hilos de Trabajo:**
  - `QueueFiller`: Llena las colas desde la base de datos
  - `QueueConsumer`: Consume códigos y los envía al láser
  - `QueueFillerDataString`: Similar para DataMatrix
  - `QueueConsumerDataString`: Consumidor para DataMatrix

**`AppCSIUser.cs`** - Módulo de Aplicación
- **Propósito:** Clase estática central que gestiona el estado global
- **Funciones:**
  - `Db`, `DbCodes`: Instancias de conexión a bases de datos
  - `Promo`: Instancia de la promoción actual
  - `oPLC`: Instancia del controlador PLC
  - `GenerateHistoric()`: Genera registros históricos de producción
  - `LoadCodes()`: Carga y adjunta la base de datos de códigos
  - `InitCulture()`: Inicializa el sistema de internacionalización

**`CLaser.cs`** - Controlador de Láser
- **Propósito:** Abstracción para controlar el dispositivo láser
- **Estados:**
  - `STOPPED`: Láser detenido
  - `MARKING`: Láser marcando
  - `ERRORS`: Estado de error
- **Funciones principales:**
  - `Run()`: Inicia el láser y configura el mensaje
  - `RunThread()`: Establece socket secundario para envío de códigos
  - `GetState()`: Obtiene el estado actual y detecta errores
  - `InBufferCount()`: Cuenta elementos en el buffer del láser
  - `ResetBuffer()`: Resetea el buffer del láser
  - `Stop()`: Detiene el láser

**`PLC.cs`** - Controlador PLC
- **Propósito:** Comunicación con PLC mediante Modbus TCP
- **Estructuras:**
  - `PLC_READ_CONTROL`: Datos leídos del PLC (estados, cantidades impresas)
  - `PLC_WRITE_CONTROL`: Datos escritos al PLC (comandos, parámetros de ciclo)
- **Funciones principales:**
  - `Init()`: Inicializa conexión Modbus TCP
  - `FillCycle()`: Rellena información de ciclo para el PLC
  - `Start()`: Envía comando de inicio
  - `Stop()`: Envía comando de parada
  - `Rearm()`: Envía comando de rearme
  - `PLCState()`: Hilo que gestiona comunicación bidireccional
- **Registros Modbus:**
  - Lectura: Registro 55000 (104 registros) - Estado del PLC
  - Escritura: Registro 55100 (60 registros) - Parámetros de programa

**`Promotion.cs`** - Gestión de Promociones
- **Propósito:** Representa una promoción de marcado
- **Propiedades clave:**
  - `TotalCodes`: Total de códigos disponibles
  - `ConsumedCodes`: Códigos consumidos
  - `Layers`: Número de capas (1-25)
  - `CycleElements`: Elementos por ciclo
  - `UserFields`: Número de campos de usuario (1-4)
  - `DatamatrixType`: Tipo de DataMatrix (-1 si no aplica)
  - `Split1-4`: Longitudes de división de código
- **Funciones:**
  - `Load()`: Carga promoción desde base de datos
  - `GetSqlCodes`: Genera SQL para obtener códigos disponibles
  - `get_LayerQty()`: Obtiene cantidad de códigos por capa

**`SetupAplicacio.cs`** - Configuración
- **Propósito:** Clase de configuración con propiedades localizables
- **Configuraciones:**
  - Base de datos (servidor, catálogo, autenticación)
  - Láser (IP, tamaño de buffer, tiempos de espera)
  - PLC (IP, puerto)
  - Advertencias de bajo nivel de códigos
  - Idioma

**`Smo.cs`** - SQL Server Management Objects
- **Propósito:** Gestión de bases de datos SQL Server
- **Funciones:**
  - `AttachDb()`: Adjunta una base de datos
  - `DetachDb()`: Desadjunta una base de datos
  - `IsAttached()`: Verifica si una BD está adjunta
  - `CreateDb()`: Crea una nueva base de datos desde script
  - `ShrinkDb()`: Reduce el tamaño de una base de datos

**`LicenseManager.cs`** - Gestión de Licencias
- **Propósito:** Validación de licencias de hardware/software
- **Funciones:**
  - `IsLicenseValid()`: Valida archivo de licencia

**`HardwareLicense.cs`** - Licencia de Hardware
- **Propósito:** Gestión de licencias basadas en hardware

**`frmSetArtwork.cs`** - Selección de Artwork
- **Propósito:** Permite seleccionar el artwork (diseño) para la producción

**`frmConfirmPromotion.cs`** - Confirmación de Promoción
- **Propósito:** Confirmación antes de iniciar producción

**`frmSetupViewer.cs`** - Visor de Configuración
- **Propósito:** Interfaz para configurar la aplicación

**`frmPassword.cs`** - Autenticación
- **Propósito:** Solicita contraseña para acceder a configuración

**`EZCode.cs`** - Generador de Códigos DataMatrix
- **Propósito:** Calcula códigos DataMatrix EZCode
- **Función:**
  - `CalculateCode()`: Genera el código DataMatrix desde bytes

---

### 2. **DataBases** - Capa de Acceso a Datos

**`Comu1.cs`** - Utilidades de Base de Datos
- **Propósito:** Funciones comunes de acceso a datos
- **Funciones:**
  - `GetLibraryVersion()`: Obtiene versión de la librería

**Estructura de Bases de Datos:**
- **IPFEu**: Base de datos principal
  - Tabla `Jobs`: Promociones/trabajos
  - Tabla `CodesIndex`: Índice de archivos de códigos importados
  - Tabla `Artworks`: Artworks asociados a trabajos
  - Tabla `Historico`: Historial de producción

- **Bases de Datos de Códigos**: Una por promoción
  - Tabla `Codes`: Códigos individuales con campos:
    - `Id`: Identificador único
    - `Code`: Código de texto
    - `Sent`: Flag de envío (0/1)
    - `TimeStamp`: Fecha/hora de envío

**Módulos de Base de Datos:**
- `DataBases.SqlServer`: Implementación para SQL Server
- `DataBases.My`: Implementación para MySQL
- `DataBases.OracleDb`: Implementación para Oracle
- `DataBases.Sybase`: Implementación para Sybase
- `DataBases.Access`: Implementación para Access

---

### 3. **ModbusTCP** - Protocolo Modbus TCP

**Propósito:** Implementación del protocolo Modbus TCP para comunicación con PLCs

**Funciones:**
- Conexión TCP/IP con dispositivos Modbus
- Lectura/Escritura de registros
- Manejo de excepciones Modbus
- Eventos para respuestas y excepciones

---

### 4. **MacsaDevicesNet** - Integración de Dispositivos Macsa

**Propósito:** Librería para comunicación con dispositivos Macsa (láseres, etiquetadoras, etc.)

**Componentes principales:**
- `Common.cs`: Funciones comunes y logging
- `MacsaDevice.cs`: Clase base para dispositivos
- `Etiquetadora.cs`: Control de etiquetadoras
- `Inyector.cs`: Control de inyectores
- `Magnetiq.cs`: Control de dispositivos Magnetiq
- `LinxMessageManager.cs`: Gestor de mensajes Linx

---

### 5. **Speedway** - Integración RFID

**Propósito:** Integración con lectores RFID Speedway

**Componentes:**
- `Speedway.Mach1`: Implementación para modelos Mach1

---

### 6. **Advantech** - Dispositivos Advantech

**Propósito:** Integración con dispositivos Advantech (Adam series)

**Componentes:**
- `Advantech.Adam`: Control de módulos Adam
- `Advantech.Common`: Funciones comunes
- `Advantech.Protocol`: Protocolos Modbus RTU y TCP

---

### 7. **Microsoft.SqlServer.*** - SQL Server Management Objects

**Propósito:** Librerías de Microsoft para gestión de SQL Server

**Componentes:**
- `Microsoft.SqlServer.Smo`: SQL Server Management Objects
- `Microsoft.SqlServer.SmoExtended`: Extensiones SMO
- `Microsoft.SqlServer.Management.Sdk.Sfc`: Framework de gestión
- `Microsoft.SqlServer.ConnectionInfo`: Información de conexión
- `Microsoft.SqlServer.SqlEnum`: Enumeraciones
- `Microsoft.SqlServer.SqlClrProvider`: Proveedor CLR

---

### 8. **NLog** - Sistema de Logging

**Propósito:** Sistema de logging estructurado

**Uso:**
- Registro de eventos de aplicación
- Trazabilidad de operaciones
- Depuración y diagnóstico

---

### 9. **Newtonsoft.Json** - Serialización JSON

**Propósito:** Serialización y deserialización JSON

**Uso:**
- Configuración
- Comunicación con servicios
- Persistencia de datos

---

## 🔄 Flujos de Trabajo Principales

### 1. Inicio de Aplicación

```
1. main_Load()
   ├─ Verifica instancia previa
   ├─ Valida licencia
   ├─ Crea directorio Tmp
   ├─ Muestra splash screen
   ├─ PrepareDataBase()
   │  └─ Configura conexión SQL Server
   ├─ SearchDb()
   │  ├─ Busca base de datos IPFEu en unidades
   │  ├─ DetachDb("IPFEu")
   │  └─ AttachDb("IPFEu", ...)
   └─ GetArtwork()
      ├─ frmSetArtwork (selección de artwork)
      ├─ frmConfirmPromotion (confirmación)
      └─ Inicializa PLC si Layers > 1
```

### 2. Inicio de Producción

```
1. btnStart_Click()
   ├─ Valida cantidad a producir
   ├─ Valida pedido (11 caracteres)
   ├─ Si Layers > 1:
   │  ├─ Inicializa PLC
   │  ├─ Rearm PLC
   │  └─ FillCycle() - Rellena parámetros de ciclo
   ├─ Laser.Run() - Inicia láser
   ├─ Laser.RunThread() - Socket secundario
   ├─ Inicia hilos:
   │  ├─ QueueFiller / QueueFillerDataString
   │  └─ QueueConsumer / QueueConsumerDataString
   ├─ MLaser_Start() - Inicia marcado
   └─ SincroMarkState.Start() - Timer de sincronización
```

### 3. Proceso de Producción (Colas)

```
QueueFiller (Hilo):
├─ Mientras Laser.State == MARKING:
│  ├─ Si Queue1 vacía:
│  │  └─ FillQueue(Queue1)
│  │     ├─ Obtiene hasta 50 códigos de BD
│  │     ├─ Marca como Sent=1
│  │     ├─ Actualiza TimeStamp
│  │     └─ UpdateConsumos()
│  └─ Si Queue2 vacía:
│     └─ FillQueue(Queue2)

QueueConsumer (Hilo):
├─ Mientras Laser.State == MARKING:
│  ├─ Si ActiveQueue == Queue1:
│  │  ├─ Toma código de Queue1
│  │  ├─ Divide según UserFields (1-4)
│  │  ├─ MLaser_FastUTF8Usermessage() - Envía al láser
│  │  └─ Si éxito: Dequeue()
│  └─ Alterna entre Queue1 y Queue2
```

### 4. Sincronización (Timer SincroMarkState)

```
Cada 2 segundos:
├─ Actualiza contadores (producidos, pendientes)
├─ Actualiza barras de progreso
├─ Actualiza último código enviado
├─ Verifica errores de láser
├─ Verifica errores de PLC
├─ Si producción completa:
│  └─ btnStop_Click()
└─ WarningLowLevelCodes() - Advertencias
```

### 5. Parada de Producción

```
btnStop_Click():
├─ Detiene timer SincroMarkState
├─ Laser.Stop()
├─ Si Layers > 1: PLC.Stop()
├─ Actualiza contadores finales
├─ GenerateHistoric() - Genera histórico
└─ Habilita controles
```

### 6. Comunicación PLC (Modbus TCP)

```
PLCState (Hilo):
├─ Loop continuo:
│  ├─ Comando STATE:
│  │  └─ ReadHoldingRegister(1, 55000, 104)
│  │     └─ oPLC_OnResponseData()
│  │        ├─ Parsea estados (Rearmed, Running, Stopped, Error)
│  │        ├─ Parsea PrintedLayersQty[25]
│  │        └─ Cambia a comando PROGRAM_PARAMETERS
│  └─ Comando PROGRAM_PARAMETERS:
│     ├─ Construye array de 120 bytes
│     ├─ Incluye: comandos, CycleElements, Layers, LayerQty[25]
│     └─ WriteMultipleRegister(2, 55100, 60, Data)
│        └─ oPLC_OnResponseData()
│           └─ Cambia a comando STATE
```

---

## 🗄️ Estructura de Base de Datos

### Base de Datos Principal: IPFEu

#### Tabla: Jobs
```sql
- JobId (PK)
- JobName
- LaserFile
- CodesDb
- Layers
- CycleElements
- Split1, Split2, Split3, Split4
- RecordLength
- Datamatrix
- IsAbsolute
- LayerQty1 - LayerQty25
- LayerUseCodes1 - LayerUseCodes25
```

#### Tabla: CodesIndex
```sql
- Id (PK)
- IdJob (FK)
- FileName
- FromRecord
- ToRecord
- TotalCodes
- Consumed
```

#### Tabla: Artworks
```sql
- IdJob (PK, FK)
- Artwork (PK)
- ...
```

#### Tabla: Historico
```sql
- Id (PK)
- Pedido
- Artwork
- Fichero
- Desde
- Hasta
- Volumen
- Timestamp
- Sesion
- Layers
- LayerQty1 - LayerQty25
```

### Base de Datos de Códigos (por promoción)

#### Tabla: Codes
```sql
- Id (PK)
- Code (texto del código)
- Sent (0/1)
- TimeStamp
```

---

## ⚙️ Configuración

### Archivo: app.config

```xml
<userSettings>
  <IPFUser.My.MySettings>
    <!-- Base de Datos -->
    <setting name="Catalog">IPFEu</setting>
    <setting name="DataServer">(local)\sqlexpress</setting>
    <setting name="UseWindowsAuthentication">True</setting>
    
    <!-- Láser -->
    <setting name="LaserIP">192.168.0.180</setting>
    <setting name="LaserBufferSize">100</setting>
    <setting name="WaitTimeOnLaserQueueFull">50</setting>
    <setting name="WaitTime">5</setting>
    
    <!-- PLC -->
    <setting name="PLC">
      <ArrayOfString>
        <string>192.168.1.100</string>
        <string>502</string>
      </ArrayOfString>
    </setting>
    
    <!-- Advertencias -->
    <setting name="LowCodes">50</setting>
    <setting name="VeryLowCodes">25</setting>
    <setting name="ShowLowCodes">True</setting>
    <setting name="ShowVeryLowCodes">True</setting>
    
    <!-- Idioma -->
    <setting name="Language">English</setting>
  </IPFUser.My.MySettings>
</userSettings>
```

---

## 🔌 Dependencias y Referencias

### Referencias de Proyecto:
- **DataBases**: Acceso a datos
- **MacsaDevicesNet**: Dispositivos Macsa
- **ModbusTCP**: Protocolo Modbus
- **Speedway**: RFID
- **Advantech.Adam**: Dispositivos Advantech
- **Microsoft.SqlServer.Smo**: Gestión SQL Server
- **NLog**: Logging
- **Newtonsoft.Json**: JSON

### DLLs Externas:
- **LaserDLL**: DLL nativa para control de láser (no incluida en código fuente)

---

## 🚀 Funcionalidades Clave

### 1. Gestión de Promociones
- Carga de promociones desde base de datos
- Selección de artwork
- Gestión de múltiples capas (hasta 25)
- Cálculo de ciclos y elementos

### 2. Control de Producción
- Inicio/parada de producción
- Control de cantidad a producir
- Seguimiento en tiempo real
- Barras de progreso (lote y total)

### 3. Gestión de Códigos
- Sistema de doble cola para alta disponibilidad
- Soporte para códigos de texto y DataMatrix
- División de códigos en múltiples campos (1-4)
- Marcado automático de códigos enviados

### 4. Integración PLC
- Comunicación Modbus TCP
- Sincronización de estados
- Gestión de múltiples capas
- Control de ciclo de producción

### 5. Histórico y Trazabilidad
- Registro completo de producción
- Información por archivo
- Datos de sesión
- Información de capas impresas

### 6. Advertencias y Alertas
- Advertencia de bajo nivel de códigos
- Alertas de errores de láser
- Alertas de errores de PLC
- Panel informativo visual

---

## 🔧 Mantenimiento y Actualizaciones Futuras

### Áreas de Mejora Identificadas:

1. **Arquitectura:**
   - Migrar a arquitectura más modular
   - Separar lógica de negocio de UI
   - Implementar patrón Repository para acceso a datos

2. **Manejo de Errores:**
   - Implementar sistema de reintentos
   - Mejorar logging estructurado
   - Manejo de excepciones más robusto

3. **Rendimiento:**
   - Optimizar consultas SQL
   - Implementar caché de códigos
   - Mejorar gestión de hilos

4. **Configuración:**
   - Migrar a archivo de configuración más estructurado
   - Implementar validación de configuración
   - Configuración por entorno

5. **Testing:**
   - Implementar pruebas unitarias
   - Pruebas de integración
   - Pruebas de carga

### Puntos de Atención:

- **Thread Safety**: Las colas usan locks, pero revisar concurrencia
- **Database Connections**: Verificar cierre adecuado de conexiones
- **Memory Management**: Revisar gestión de DataTables grandes
- **Error Recovery**: Implementar recuperación automática de errores

---

## 📝 Notas de Implementación

### Sistema de Colas:
- **Doble cola**: Permite llenar una mientras se consume la otra
- **Thread-safe**: Uso de locks para sincronización
- **Tamaño de lote**: 50 códigos por lote (configurable)

### Comunicación Láser:
- **Dos sockets**: Uno para control, otro para envío de códigos
- **Buffer**: Tamaño configurable (default 100)
- **Fast User Message**: Protocolo optimizado para alta velocidad

### Comunicación PLC:
- **Modbus TCP**: Puerto 502 (configurable)
- **Polling**: Cada 500ms
- **Registros**: 
  - Lectura: 55000-55103 (104 registros)
  - Escritura: 55100-55159 (60 registros)

### Base de Datos:
- **Attach/Detach**: Bases de datos se adjuntan dinámicamente
- **Búsqueda automática**: Busca IPFEu en todas las unidades
- **Permisos**: Configura permisos de archivos automáticamente

---

## 🎯 Casos de Uso

### Caso 1: Producción Simple (1 Capa)
1. Usuario selecciona artwork
2. Ingresa pedido (11 caracteres)
3. Ingresa cantidad a producir
4. Sistema carga códigos de BD
5. Láser marca códigos secuencialmente
6. Sistema registra histórico

### Caso 2: Producción Multicapa (2+ Capas)
1. Similar a Caso 1
2. Sistema inicializa PLC
3. PLC controla cambio de capas
4. Sistema sincroniza con PLC
5. Registra cantidades por capa

### Caso 3: Códigos DataMatrix
1. Sistema calcula código DataMatrix
2. Usa colas especiales (QueueDataString)
3. Envía datos binarios al láser
4. Láser genera DataMatrix

---

## 🔐 Seguridad

- **Licencias**: Validación de licencia al inicio
- **Contraseñas**: Protección de configuración
- **Permisos**: Gestión de permisos de archivos
- **Autenticación**: Soporte Windows Authentication y SQL Authentication

---

## 📚 Referencias Técnicas

- **.NET Framework 4.7.2**: Framework base
- **SQL Server**: Base de datos (SMO)
- **Modbus TCP**: Protocolo industrial estándar
- **Windows Forms**: UI framework
- **Threading**: System.Threading para hilos
- **NLog**: Sistema de logging

---

## 📞 Soporte y Contacto

Para futuras actualizaciones y mantenimiento, referirse a:
- Código fuente completo en este repositorio
- Documentación de componentes en comentarios XML

- Logs en archivos de registro (NLog)
---

**Última actualización:** Análisis completo del código base  
**Versión documentada:** 3.0.0.2

