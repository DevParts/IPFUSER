# LaserMacsaUser - Sistema de Marcado Láser Industrial (.NET 8)

## 📋 Descripción General

**LaserMacsaUser** es una aplicación Windows Forms desarrollada en C# (.NET 8) que gestiona el marcado láser industrial con integración de bases de datos SQL Server y control de dispositivos Macsa. Esta es una modernización y refactorización de la aplicación original **IPFUser** (VB.NET .NET Framework 4.7.2).

**Versión:** 1.0.0  
**Framework:** .NET 8  
**Tipo:** Aplicación Windows Forms

---

## 🎯 Estado de Implementación

### ✅ Funcionalidades Implementadas

#### 1. **Arquitectura Base**
- ✅ Migración a .NET 8 C# Windows Forms
- ✅ Arquitectura modular con servicios (DatabaseService, LaserService, QueueService, PromotionService, HistoryService)
- ✅ Separación de responsabilidades (Models, Services, Views)
- ✅ Interfaces para todos los servicios principales

#### 2. **Gestión de Base de Datos**
- ✅ Conexión a SQL Server con `Microsoft.Data.SqlClient`
- ✅ Gestión de bases de datos adjuntas (Attach/Detach)
- ✅ Búsqueda automática de `IPFEu.mdf` en unidades locales
- ✅ Operaciones CRUD en tablas principales (Jobs, CodesIndex, Artworks, Historico)
- ✅ Conexión a bases de datos de códigos dinámicas
- ✅ Manejo seguro de valores `DBNull`
- ✅ Configuración SSL/TLS para conexiones seguras

#### 3. **Gestión de Promociones**
- ✅ Carga de promociones desde base de datos
- ✅ Selección de artwork con validación
- ✅ Validación de artwork (archivo láser, RecordLength, Splits)
- ✅ Carga de información de `CodesIndex`
- ✅ Cálculo de campos de usuario (UserFields 1-4)
- ✅ Gestión de múltiples archivos de códigos
- ✅ Confirmación de promoción antes de iniciar

#### 4. **Comunicación con Láser**
- ✅ Integración con `SocketCommNet` (wrapper C#) y `SocketCommDll.dll` (DLL nativa)
- ✅ Inicialización de láser (CS_Init)
- ✅ Copia de archivos `.msf` al láser (CS_CopyFile)
- ✅ Establecimiento de mensaje por defecto (CS_SetDefaultMessage)
- ✅ Envío de códigos mediante Fast User Message
- ✅ Obtención de estado del láser en tiempo real
- ✅ Detección y manejo de alarmas del láser
- ✅ Gestión de buffer del láser
- ✅ Control de inicio/parada de marcado

#### 5. **Sistema de Colas (Producer-Consumer)**
- ✅ Sistema de doble cola para códigos de texto
- ✅ Hilos separados para llenado y consumo de colas
- ✅ Llenado de colas desde base de datos (hasta 50 códigos por lote)
- ✅ Marcado de códigos como enviados (`Sent=1`)
- ✅ Actualización de `TimeStamp` en códigos enviados
- ✅ División de códigos según `UserFields` (1-4 campos)
- ✅ Actualización automática de `Consumed` en `CodesIndex`
- ✅ Manejo de errores y eventos

#### 6. **Interfaz de Usuario**
- ✅ Formulario principal con controles de producción
- ✅ Selección de artwork (ArtworkSelection)
- ✅ Confirmación de promoción (PromotionConfirmation)
- ✅ Campos de información de promoción (no editables)
- ✅ ComboBox para selección de archivos de códigos
- ✅ Barras de progreso (lote y total)
- ✅ Contadores en tiempo real (OK, NOK, Total)
- ✅ Visualización de último código enviado
- ✅ Advertencias de bajo nivel de códigos
- ✅ Panel de alarmas del láser
- ✅ Formulario de configuración (AppConfigForm)
- ✅ Formulario de login para configuración (LoginForm)

#### 7. **Control de Producción**
- ✅ Inicio de producción con validaciones
- ✅ Parada de producción
- ✅ Sincronización en tiempo real (timer cada 2 segundos)
- ✅ Actualización de contadores y barras de progreso
- ✅ Detección de finalización de producción
- ✅ Generación de histórico de producción

#### 8. **Histórico de Producción**
- ✅ Generación de registros históricos
- ✅ Registro de información por archivo
- ✅ Información de sesión
- ✅ Timestamps y volúmenes

#### 9. **Configuración**
- ✅ Sistema de configuración con `AppSettings`
- ✅ Configuración de base de datos (servidor, catálogo, autenticación)
- ✅ Configuración de láser (IP, buffer, tiempos)
- ✅ Configuración de advertencias de códigos

---

### ❌ Funcionalidades Pendientes (Excluyendo PLC)


#### 0.1 **revisar por que no se mata el proceso*** 
- ❌ Matar proceso de laserMacsa
  
#### 1. **Sistema de Licencias**
- ❌ Validación de licencias (`LicenseManager`)
- ❌ Verificación de archivo `license.lic`
- ❌ Bloqueo de aplicación si licencia inválida

#### 2. **Pantalla de Inicio (Splash Screen)** HECHO POR L.ARIAS
- ✅ Formulario `frmSplash` con barra de progreso
- ✅ Mensajes de carga durante inicialización
- ✅ Animación de inicio

#### 3. **Sistema de Internacionalización**
- ❌ Sistema de recursos multiidioma (Español/Inglés)
- ❌ `ResourceManager` para cadenas localizadas
- ❌ `AppCSIUser.InitCulture()` para inicialización de cultura
- ❌ Cambio dinámico de idioma

#### 4. **Sistema de Logging**
- ❌ Integración con NLog o sistema de logging estructurado
- ❌ Registro de eventos de aplicación
- ❌ Trazabilidad de operaciones
- ❌ Archivos de log rotativos

#### 5. **Verificación de Instancia Única**
- ❌ `Common.PrevInstance()` para evitar múltiples instancias
- ❌ Mensaje de advertencia si ya hay una instancia ejecutándose

#### 6. **Gestión de Directorio Temporal**
- ❌ Creación automática de directorio `Tmp` en startup
- ❌ Limpieza de archivos temporales

#### 7. **Soporte DataMatrix**
- ❌ Colas especiales para DataMatrix (`QueueDataString1`, `QueueDataString2`)
- ❌ Clase `EZCode` para cálculo de códigos DataMatrix
- ❌ Hilos `QueueFillerDataString` y `QueueConsumerDataString`
- ❌ Envío de datos binarios al láser para DataMatrix
- ❌ Soporte para `DatamatrixType` en promociones

#### 8. **Indicadores Visuales (LEDs)**
- ❌ LEDs de estado (pbLedLife, pbLedPLC)
- ❌ Indicadores visuales de conexión
- ❌ Panel de estado con iconos

#### 9. **Mejoras de UI/UX**
- ❌ Mensajes de estado más descriptivos
- ❌ Tooltips informativos
- ❌ Validación visual de campos
- ❌ Mejoras en diseño visual

#### 10. **Manejo de Errores Avanzado**
- ❌ Sistema de reintentos automáticos
- ❌ Recuperación de errores de conexión
- ❌ Logging detallado de excepciones
- ❌ Mensajes de error más informativos

#### 11. **Optimizaciones de Rendimiento**
- ❌ Caché de códigos frecuentes
- ❌ Optimización de consultas SQL
- ❌ Gestión mejorada de memoria
- ❌ Pool de conexiones

---

## 🏗️ Arquitectura del Sistema

### Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│              LaserMacsaUser (Aplicación Principal)            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Form1      │  │ AppConfigForm│  │ArtworkSelection│     │
│  │  (UI Principal)│  │  (Config)   │  │  (Artworks)   │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
         │                    │                    │
         ├────────────────────┼────────────────────┤
         │                    │                    │
    ┌────▼────┐         ┌────▼────┐         ┌────▼────┐
    │ LaserService │     │ QueueService │   │DatabaseService│
    │ (Control    │     │ (Colas)      │   │(SQL Server)   │
    │  Láser)     │     │              │   │               │
    └─────────┘         └─────────┘         └───────────┘
         │                    │                    │
         └────────────────────┼────────────────────┘
                              │
                    ┌─────────▼─────────┐
                    │  SocketCommNet    │
                    │  (TCP/IP Laser)   │
                    └───────────────────┘
```

### Capas del Sistema

1. **Capa de Presentación (UI)**
   - Windows Forms (.NET 8)
   - Formularios modulares
   - Controles personalizados

2. **Capa de Lógica de Negocio**
   - Servicios especializados (PromotionService, HistoryService)
   - Gestión de estado de producción
   - Validaciones de negocio

3. **Capa de Acceso a Datos**
   - `DatabaseService` con `Microsoft.Data.SqlClient`
   - Gestión de bases de datos adjuntas
   - Operaciones CRUD optimizadas

4. **Capa de Comunicación**
   - `LaserService` para comunicación TCP/IP con láser
   - `QueueService` para gestión de colas
   - Protocolo Fast User Message

---

## 📦 Componentes Principales

### 1. **Models**

**`Promotion.cs`**
- Representa una promoción/trabajo
- Propiedades: JobId, JobName, LaserFile, CodesDb, Layers, UserFields, etc.
- Método `GetSqlCodes()` para generar consultas SQL

**`CodeFileInfo.cs`**
- Información de archivos de códigos importados
- Propiedades: Id, JobId, FileName, FromRecord, ToRecord, TotalCodes, Consumed

**`LaserStatus.cs`**
- Estado del láser en tiempo real
- Propiedades: OkCounter, NokCounter, TotalCounter, AlarmCode, BufferCount, etc.

**`ProductionHistory.cs`**
- Registros históricos de producción

### 2. **Services**

**`DatabaseService.cs`**
- Implementa `IDatabaseService`
- Gestión de conexiones SQL Server
- Operaciones CRUD
- Attach/Detach de bases de datos
- Método `UpdateConsumos()` para actualizar `CodesIndex`

**`LaserService.cs`**
- Implementa `ILaserService`
- Comunicación TCP/IP con láser
- Inicialización, copia de archivos, envío de códigos
- Obtención de estado y detección de alarmas

**`QueueService.cs`**
- Implementa `IQueueService`
- Sistema de doble cola (producer-consumer)
- Llenado y consumo de códigos
- Actualización de base de datos

**`PromotionService.cs`**
- Implementa `IPromotionService`
- Carga y validación de promociones
- Gestión de artworks
- Carga de `CodesIndex`

**`HistoryService.cs`**
- Implementa `IHistoryService`
- Generación de registros históricos

### 3. **Views**

**`Form1.cs`**
- Formulario principal
- Control de producción
- Sincronización en tiempo real
- Gestión de UI

**`ArtworkSelection.cs`**
- Selección de artwork

**`PromotionConfirmation.cs`**
- Confirmación de promoción

**`AppConfigForm.cs`**
- Configuración de aplicación

**`LoginForm.cs`**
- Autenticación para configuración

---

## 🔄 Flujos de Trabajo Principales

### 1. Inicio de Aplicación

```
1. Form1_Load()
   ├─ PrepareDataBase()
   │  └─ Configura conexión SQL Server
   ├─ SearchDb()
   │  ├─ Busca IPFEu.mdf en unidades
   │  ├─ DetachDb("IPFEu")
   │  └─ AttachDb("IPFEu", ...)
   └─ GetArtwork()
      ├─ ArtworkSelection (selección)
      ├─ Validación de artwork
      ├─ ArtworkSelectionRepeat (confirmación)
      └─ PromotionConfirmation
```

### 2. Inicio de Producción

```
1. btnStart_Click()
   ├─ Valida cantidad y pedido
   ├─ AttachCodesDatabase()
   ├─ LaserService.Initialize()
   ├─ LaserService.CopyMessageFile() - Copia .msf al láser
   ├─ LaserService.SetDefaultMessage() - Establece mensaje activo
   ├─ LaserService.StartPrint()
   ├─ QueueService.Start() - Inicia colas
   └─ SyncTimer.Start() - Inicia sincronización
```

### 3. Proceso de Producción (Colas)

```
QueueService.FillQueue():
├─ Obtiene hasta 50 códigos de BD
├─ Marca como Sent=1
├─ Actualiza TimeStamp
└─ UpdateConsumos() - Actualiza CodesIndex.Consumed

QueueService.ConsumerLoop():
├─ Toma código de cola activa
├─ Divide según UserFields (1-4)
├─ LaserService.SendUserMessage() - Envía al láser
└─ Alterna entre colas
```

### 4. Sincronización (Timer)

```
Cada 2 segundos:
├─ Actualiza contadores (OK, NOK, Total)
├─ Actualiza barras de progreso
├─ Actualiza porcentaje de códigos consumidos
├─ Actualiza último código enviado
├─ Verifica errores de láser
├─ Verifica si producción completa
└─ WarningLowLevelCodes() - Advertencias
```

### 5. Parada de Producción

```
btnStop_Click():
├─ Detiene SyncTimer
├─ QueueService.Stop()
├─ LaserService.Stop()
├─ HistoryService.GenerateHistory()
└─ Habilita controles
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
- UserFields
- Split1, Split2, Split3, Split4
- RecordLength
- DatamatrixType
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

### Archivo: AppSettings.cs

```csharp
public class AppSettings
{
    // Base de Datos
    public string DataSource { get; set; } = "(local)\\SQLEXPRESS";
    public string Catalog { get; set; } = "IPFEu";
    public bool UseWindowsAuthentication { get; set; } = true;
    
    // Láser
    public string LaserIP { get; set; } = "192.168.0.180";
    public int LaserBufferSize { get; set; } = 100;
    public int WaitTimeOnLaserQueueFull { get; set; } = 50;
    
    // Advertencias
    public int LowCodes { get; set; } = 50;
    public int VeryLowCodes { get; set; } = 25;
    public bool ShowLowCodes { get; set; } = true;
    public bool ShowVeryLowCodes { get; set; } = true;
}
```

---

## 🔌 Dependencias y Referencias

### Paquetes NuGet:
- **Microsoft.Data.SqlClient** (5.2.0): Acceso a SQL Server
- **SocketCommNet**: Comunicación TCP/IP con láser (proyecto local)

### DLLs Externas:
- **SocketCommDll.dll**: DLL nativa para control de láser
- **SocketCommNet.dll**: Wrapper C# para SocketCommDll

---

## 🚀 Funcionalidades Clave Implementadas

### 1. Gestión de Promociones ✅
- Carga de promociones desde base de datos
- Selección de artwork con validación
- Gestión de múltiples archivos de códigos
- Cálculo de campos de usuario

### 2. Control de Producción ✅
- Inicio/parada de producción
- Control de cantidad a producir
- Seguimiento en tiempo real
- Barras de progreso (lote y total)

### 3. Gestión de Códigos ✅
- Sistema de doble cola para alta disponibilidad
- Soporte para códigos de texto (1-4 campos)
- Marcado automático de códigos enviados
- Actualización automática de `Consumed` en `CodesIndex`

### 4. Comunicación Láser ✅
- Inicialización y configuración
- Copia de archivos `.msf` al láser
- Envío de códigos mediante Fast User Message
- Detección de alarmas
- Gestión de buffer

### 5. Histórico y Trazabilidad ✅
- Registro completo de producción
- Información por archivo
- Datos de sesión
- Timestamps

### 6. Advertencias y Alertas ✅
- Advertencia de bajo nivel de códigos
- Alertas de errores de láser
- Panel informativo visual

---

## 📝 Notas de Implementación

### Sistema de Colas:
- **Doble cola**: Permite llenar una mientras se consume la otra
- **Thread-safe**: Uso de locks para sincronización
- **Tamaño de lote**: 50 códigos por lote

### Comunicación Láser:
- **Dos sockets**: Uno para control, otro para envío de códigos
- **Buffer**: Tamaño configurable (default 100)
- **Fast User Message**: Protocolo optimizado para alta velocidad

### Base de Datos:
- **Attach/Detach**: Bases de datos se adjuntan dinámicamente
- **Búsqueda automática**: Busca IPFEu en todas las unidades
- **SSL/TLS**: Conexiones seguras con certificados

### Actualización de Consumos:
- **Automática**: Se actualiza cada vez que se procesan códigos
- **Multiarchivo**: Maneja códigos que abarcan múltiples archivos
- **Sincronización**: UI muestra valores actualizados desde BD

---

## 🔧 Próximos Pasos de Desarrollo

### Prioridad Alta:
1. **Sistema de Licencias**: Implementar validación de licencias
2. **Sistema de Logging**: Integrar NLog o similar
3. **Splash Screen**: Agregar pantalla de inicio
4. **Verificación de Instancia Única**: Evitar múltiples instancias

### Prioridad Media:
5. **Soporte DataMatrix**: Implementar colas y cálculo de códigos DataMatrix
6. **Internacionalización**: Sistema multiidioma
7. **Indicadores Visuales**: LEDs y paneles de estado
8. **Mejoras de UI/UX**: Tooltips, validaciones visuales

### Prioridad Baja:
9. **Optimizaciones**: Caché, pool de conexiones
10. **Manejo de Errores Avanzado**: Reintentos, recuperación automática
11. **Gestión de Directorio Temporal**: Limpieza automática

---

## 🎯 Casos de Uso

### Caso 1: Producción Simple (1 Capa) ✅
1. Usuario selecciona artwork
2. Ingresa pedido (11 caracteres)
3. Ingresa cantidad a producir
4. Sistema carga códigos de BD
5. Láser marca códigos secuencialmente
6. Sistema registra histórico

### Caso 2: Producción con Múltiples Archivos ✅
1. Similar a Caso 1
2. Sistema carga múltiples archivos de códigos
3. Usuario puede seleccionar archivo en ComboBox
4. Sistema muestra progreso por archivo
5. Actualización automática de consumos

### Caso 3: Códigos DataMatrix ❌
1. Sistema calcula código DataMatrix (PENDIENTE)
2. Usa colas especiales (PENDIENTE)
3. Envía datos binarios al láser (PENDIENTE)
4. Láser genera DataMatrix (PENDIENTE)

---

## 🔐 Seguridad

- ✅ **Autenticación**: Soporte Windows Authentication y SQL Authentication
- ✅ **SSL/TLS**: Conexiones seguras con certificados
- ❌ **Licencias**: Validación de licencia (PENDIENTE)
- ✅ **Contraseñas**: Protección de configuración con login

---

## 📚 Referencias Técnicas

- **.NET 8**: Framework base
- **SQL Server**: Base de datos (Microsoft.Data.SqlClient)
- **Windows Forms**: UI framework
- **Threading**: System.Threading para hilos
- **SocketCommNet**: Comunicación TCP/IP con láser

---

## 📞 Soporte y Contacto

Para futuras actualizaciones y mantenimiento:
- Código fuente completo en este repositorio
- Documentación de componentes en comentarios XML
- Logs en archivos de registro (PENDIENTE)

---

**Última actualización:** Diciembre 2024  
**Versión documentada:** 1.0.0  
**Estado:** En desarrollo activo



