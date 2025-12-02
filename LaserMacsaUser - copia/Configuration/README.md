# ⚙️ Configuration - Documentación de Configuración

## 📋 Índice
- [Descripción General](#descripción-general)
- [Archivos de Configuración](#archivos-de-configuración)
- [AppSettings.cs](#appsettingscs)
- [Program.cs](#programcs)
- [Configuración Persistente](#configuración-persistente)

---

## 📖 Descripción General

La carpeta `Configuration/` contiene los archivos relacionados con la configuración de la aplicación y el punto de entrada.

---

## 📁 Archivos de Configuración

### Estructura
```
Configuration/
├── AppSettings.cs        # Clase de configuración (PropertyGrid)
└── Program.cs            # Punto de entrada (Main)
```

---

## 📝 AppSettings.cs

**Namespace**: `LaserMacsaUser.Configuration`

**Propósito**: Define las propiedades configurables de la aplicación que se muestran en el PropertyGrid del formulario de configuración.

### Propiedades Disponibles

```csharp
public class AppSettings
{
    // Global
    public string AppVersion { get; set; }
    public string Language { get; set; }
    
    // Security
    public string AppPassword { get; set; }
    
    // Codes
    public int LowLevelWarning { get; set; }
    public bool ShowLowLevels { get; set; }
    public int VeryLowLevelWarning { get; set; }
    
    // Database
    public string Catalog { get; set; }
    public string DataSource { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public bool UseWindowsAuthentication { get; set; }
    
    // Laser
    public string Laser_IP { get; set; }        // ⭐ IP del láser (dinámica)
    public int LaserBufferSize { get; set; }
    
    // Timing
    public int WaitTime { get; set; }
    public int WaitTimeBufferFull { get; set; }
}
```

### Uso en AppConfigForm

```csharp
// Cargar configuración
var settings = new AppSettings
{
    AppPassword = Properties.Settings.Default.AppPassword,
    Laser_IP = Properties.Settings.Default.Laser_IP
};

// Mostrar en PropertyGrid
propertyGridConfig.SelectedObject = settings;

// Guardar configuración
Properties.Settings.Default.Laser_IP = settings.Laser_IP;
Properties.Settings.Default.Save();
```

### Configuración de IP Dinámica

La IP del láser (`Laser_IP`) es **dinámica** y se puede cambiar desde la interfaz:

1. **Ubicación en UI**: `Views/AppConfigForm.cs` → Categoría "Laser"
2. **Almacenamiento**: `Properties/Settings.settings` → `Laser_IP`
3. **Uso en código**:
   ```csharp
   string ip = Properties.Settings.Default.Laser_IP;
   _laserService.Initialize(ip, ".\\");
   ```

---

## 🚀 Program.cs

**Namespace**: `LaserMacsaUser.Configuration`

**Propósito**: Punto de entrada de la aplicación. Contiene el método `Main()`.

### Código

```csharp
using LaserMacsaUser.Views;

namespace LaserMacsaUser.Configuration
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configuración de alta DPI y fuente por defecto
            ApplicationConfiguration.Initialize();
            
            // Ejecutar formulario principal
            Application.Run(new Form1());
        }
    }
}
```

### Configuración en .csproj

El punto de entrada se configura automáticamente cuando `Program.cs` está en la raíz o en `Configuration/`:

```xml
<!-- No es necesario especificar StartupObject si Program.cs está en Configuration/ -->
```

---

## 💾 Configuración Persistente

### Settings.settings

**Ubicación**: `Properties/Settings.settings`

**Formato**: XML

```xml
<?xml version='1.0' encoding='utf-8'?>
<SettingsFile>
  <Settings>
    <Setting Name="AppPassword" Type="System.String" Scope="User">
      <Value Profile="(Default)">mlaser</Value>
    </Setting>
    <Setting Name="Laser_IP" Type="System.String" Scope="User">
      <Value Profile="(Default)">192.168.0.180</Value>
    </Setting>
  </Settings>
</SettingsFile>
```

### Settings.Designer.cs

**Ubicación**: `Properties/Settings.Designer.cs`

**Generado automáticamente** por Visual Studio. Proporciona acceso type-safe a la configuración:

```csharp
namespace LaserMacsaUser.Properties
{
    internal sealed partial class Settings
    {
        public static Settings Default { get; }
        
        [UserScopedSetting]
        [DefaultSettingValue("192.168.0.180")]
        public string Laser_IP
        {
            get { return ((string)(this["Laser_IP"])); }
            set { this["Laser_IP"] = value; }
        }
    }
}
```

### App.config

**Ubicación**: `App.config` (raíz del proyecto)

**Contiene**: Configuración de runtime y valores por defecto

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <userSettings>
    <LaserMacsaUser.Properties.Settings>
      <setting name="Laser_IP" serializeAs="String">
        <value>192.168.0.180</value>
      </setting>
    </LaserMacsaUser.Properties.Settings>
  </userSettings>
</configuration>
```

### Acceso desde Código

```csharp
using LaserMacsaUser.Properties;

// Leer
string ip = Settings.Default.Laser_IP;
string password = Settings.Default.AppPassword;

// Escribir
Settings.Default.Laser_IP = "192.168.1.100";
Settings.Default.AppPassword = "nuevaPassword";

// Guardar (importante!)
Settings.Default.Save();
```

### Ubicación Física de Settings

Los settings de usuario se guardan en:
```
%USERPROFILE%\AppData\Local\LaserMacsaUser\LaserMacsaUser.exe_Url_[hash]\[version]\user.config
```

---

## 🔧 Agregar Nueva Configuración

### Paso 1: Agregar a Settings.settings

```xml
<Setting Name="MyNewSetting" Type="System.String" Scope="User">
  <Value Profile="(Default)">defaultValue</Value>
</Setting>
```

### Paso 2: Agregar a AppSettings.cs

```csharp
[Category("MyCategory")]
[Description("Description of my setting")]
public string MyNewSetting { get; set; } = Settings.Default.MyNewSetting;
```

### Paso 3: Usar en Código

```csharp
// Leer
string value = Settings.Default.MyNewSetting;

// Escribir
Settings.Default.MyNewSetting = "newValue";
Settings.Default.Save();
```

---

## 🔗 Referencias

- [README Principal](../README.md) - Arquitectura general
- [Views/AppConfigForm.cs](../Views/AppConfigForm.cs) - Formulario de configuración
- [Properties/Settings.settings](../Properties/Settings.settings) - Archivo de configuración

---

**Última actualización**: 2025-11-23

