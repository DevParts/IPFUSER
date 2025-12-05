# Cambios en la Arquitectura de Base de Datos de Códigos

## Resumen

Se modificó la arquitectura de almacenamiento de códigos para seguir el diseño original de la base de datos. Ahora cada promoción tiene su propia base de datos de códigos separada en lugar de almacenar todos los códigos en una tabla centralizada en `IPFEu`.

## Estructura Anterior (Incorrecta)

```
IPFEu (Base de datos principal)
├── Jobs
├── CodesIndex
├── Artworks
└── Codes (tabla centralizada con JobId) ❌ INCORRECTO
```

Todos los códigos se almacenaban en una sola tabla `Codes` dentro de `IPFEuo`, usand un campo `JobId` para identificar a qué promoción pertenecían.

## Estructura Nueva (Correcta)

```
IPFEu (Base de datos principal)
├── Jobs (con campo CodesDB que indica el nombre de la BD de códigos)
├── CodesIndex (índice de archivos, con FromRecord/ToRecord)
├── Artworks
└── Historico

PROMOCION01 (Base de datos separada)
└── Codes (Id, Code, Consumed)

PROMOCION02 (Base de datos separada)
└── Codes (Id, Code, Consumed)

... hasta PROMOCION25
```

Cada promoción tiene su propia base de datos de códigos (`PROMOCION01` a `PROMOCION25`). El nombre de la base de datos se almacena en el campo `Jobs.CodesDB`.

## Tabla Codes (en cada base de datos de promoción)

| Campo     | Tipo          | Descripción                              |
|-----------|---------------|------------------------------------------|
| Id        | INT IDENTITY  | Identificador único del código           |
| Code      | NVARCHAR(MAX) | El código en sí                          |
| Consumed  | BIT           | Indica si el código ha sido consumido (0=No, 1=Sí) |

**Nota:** El campo `Consumed` se agregó para poder rastrear qué códigos han sido utilizados.

## Archivos Modificados

### 1. `LaserMacsa/Helpers/PromotionCodesHelper.cs`
- **Cambio:** Agregado campo `Consumed` a la tabla `Codes`.
- **Método afectado:** `CreateCodesTable()`

```csharp
CREATE TABLE [dbo].[Codes] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Code] NVARCHAR(MAX) NOT NULL,
    [Consumed] BIT NOT NULL DEFAULT 0  // ← NUEVO
);
```

### 2. `LaserMacsa/DataAccess/DatabaseHelper.cs`
- **Cambio:** Eliminada la creación de la tabla `Codes` en `IPFEu`.
- **Cambio:** Método `EnsureCodesTableExists()` marcado como obsoleto (no hace nada).

### 3. `LaserMacsa/Helpers/CodeImportHelper.cs`
- **Cambio completo:** Reescrito para importar códigos a la base de datos de la promoción.
- Ahora obtiene el nombre de la BD desde `Jobs.CodesDB`.
- Se conecta a la base de datos de la promoción (no a `IPFEu`).
- Inserta códigos con campos `Code` y `Consumed` (sin `JobId`).

### 4. `LaserMacsa/Views/NewPromotion/NewPromotion.cs`
- **Cambio:** Al crear una promoción, ahora también crea su base de datos de códigos.
- Obtiene un nombre disponible (`PROMOCION01` a `PROMOCION25`).
- Crea la base de datos usando `PromotionCodesHelper.CreateCodesDatabase()`.
- Guarda el nombre en `Jobs.CodesDB`.

### 5. `LaserMacsa/Views/Main/Form1.cs`
- **Cambio:** Actualizado `LoadFileParameters()` para calcular `RecordLength` desde la base de datos de códigos correcta.
- Se conecta a la base de datos de la promoción para obtener la longitud del código.

### 6. `LaserMacsa/Views/Database/DbFinder.cs`
- **Cambio:** Eliminadas llamadas obsoletas a `EnsureCodesTableExists()`.

## Flujo de Creación de Promoción

1. Usuario ingresa nombre de la promoción.
2. Sistema obtiene un nombre disponible para la BD de códigos (`PROMOCION01`, etc.).
3. Sistema crea la base de datos de códigos con la tabla `Codes`.
4. Sistema inserta la promoción en `Jobs` con el nombre de la BD en `CodesDB`.

## Flujo de Importación de Códigos

1. Usuario selecciona un archivo de códigos.
2. Sistema obtiene el nombre de la BD de códigos desde `Jobs.CodesDB`.
3. Sistema verifica/adjunta la base de datos de códigos.
4. Sistema inserta los códigos en la tabla `Codes` de esa BD.
5. Sistema registra el archivo en `CodesIndex` de `IPFEu`.

## Límites

- Máximo **25 promociones** con bases de datos de códigos (`PROMOCION01` a `PROMOCION25`).
- Si se alcanza el límite, se debe eliminar alguna promoción existente.

## Ubicación de Archivos

Las bases de datos de códigos se crean en la misma carpeta que la base de datos principal (`C:\IPFEu\`):

```
C:\IPFEu\
├── IPF.mdf              (Base de datos principal)
├── IPF_log.ldf
├── IPFAdmin.mdf         (Histórico)
├── IPFAdmin_log.ldf
├── PROMOCION01.mdf      (Códigos de promoción 1)
├── PROMOCION01_log.ldf
├── PROMOCION02.mdf      (Códigos de promoción 2)
├── PROMOCION02_log.ldf
└── ...
```

## Compatibilidad

- Las promociones creadas con el sistema anterior (con códigos en `IPFEu`) no serán compatibles.
- Se recomienda crear nuevas promociones con el nuevo sistema.
- La tabla `Codes` en `IPFEu` (si existe) será ignorada.

---

*Documentación creada el 5 de Diciembre de 2025*

