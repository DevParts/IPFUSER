# 🖼️ Views - Documentación de Formularios

## 📋 Índice
- [Descripción General](#descripción-general)
- [Estructura de Carpetas](#estructura-de-carpetas)
- [Formularios Disponibles](#formularios-disponibles)
- [Convenciones](#convenciones)
- [Cómo Agregar un Nuevo Formulario](#cómo-agregar-un-nuevo-formulario)

---

## 📖 Descripción General

La carpeta `Views/` contiene todos los formularios (ventanas) de la aplicación Windows Forms. Cada formulario está compuesto por tres archivos:

1. **`[Nombre]Form.cs`** - Código C# del formulario (lógica de UI)
2. **`Designers/[Nombre]Form.Designer.cs`** - Código generado por el diseñador de Visual Studio
3. **`Resources/[Nombre]Form.resx`** - Recursos del formulario (imágenes, textos, etc.)

---

## 📁 Estructura de Carpetas

```
Views/
├── Form1.cs                      # Formulario principal
├── LoginForm.cs                  # Formulario de login
├── AppConfigForm.cs              # Formulario de configuración
├── ArtworkSelection.cs           # Selección de artwork
├── ConfirmPromotionForm.cs       # Confirmación de promoción
│
├── Designers/                    # Archivos Designer.cs
│   ├── Form1.Designer.cs
│   ├── LoginForm.Designer.cs
│   ├── AppConfigForm.Designer.cs
│   ├── ArtworkSelection.Designer.cs
│   └── ConfirmPromotionForm.Designer.cs
│
└── Resources/                    # Archivos .resx
    ├── Form1.resx
    ├── LoginForm.resx
    ├── AppConfigForm.resx
    ├── ArtworkSelection.resx
    └── ConfirmPromotionForm.resx
```

---

## 📝 Formularios Disponibles

### 1. Form1.cs
**Namespace**: `LaserMacsaUser.Views`

**Propósito**: Formulario principal de la aplicación. Gestiona la producción, muestra estado del láser y permite controlar la impresión.

**Características principales**:
- Control de producción (Iniciar/Detener)
- Visualización de estado del láser
- Gestión de artworks
- Manejo de promociones
- Monitoreo de alarmas

**Dependencias**:
- `ProductionController`
- `ArtworkController`
- `PromotionController`
- `LaserService`

**Ejemplo de uso**:
```csharp
// En Program.cs
Application.Run(new Form1());
```

---

### 2. LoginForm.cs
**Namespace**: `LaserMacsaUser.Views`

**Propósito**: Autenticación de usuario antes de acceder a la aplicación.

**Características**:
- Validación de contraseña
- Retorna resultado de autenticación

**Uso**:
```csharp
var loginForm = new LoginForm();
if (loginForm.ShowDialog() == DialogResult.OK)
{
    // Usuario autenticado
    string password = loginForm.Password;
}
```

---

### 3. AppConfigForm.cs
**Namespace**: `LaserMacsaUser.Views`

**Propósito**: Configuración de la aplicación, incluyendo IP del láser y otras opciones.

**Características principales**:
- Configuración de IP del láser (dinámica)
- Configuración de contraseña de aplicación
- Botón "Probar Conexión" para verificar conectividad
- Botón "Ejecutar Tests" para abrir MacsaLaserTest

**Configuración de IP**:
```csharp
// La IP se guarda automáticamente en Settings
Properties.Settings.Default.Laser_IP = ipAddress;
Properties.Settings.Default.Save();
```

**Prueba de conexión**:
- Usa `SocketComm` directamente para probar la conexión
- Muestra mensajes de éxito/error al usuario

**Abrir desde código**:
```csharp
var configForm = new AppConfigForm();
configForm.ShowDialog();
```

---

### 4. ArtworkSelection.cs
**Namespace**: `LaserMacsaUser.Views`

**Propósito**: Permite al usuario seleccionar un artwork para activar o repetir.

**Modos de operación**:
- `Mode.Activate`: Activar un nuevo artwork
- `Mode.Repeat`: Repetir un artwork existente

**Uso**:
```csharp
// Activar artwork
var form = new ArtworkSelection(ArtworkSelection.Mode.Activate);
if (form.ShowDialog() == DialogResult.OK)
{
    int artworkId = form.SelectedArtworkId;
    // Usar artworkId...
}

// Repetir artwork
var repeatForm = new ArtworkSelection(ArtworkSelection.Mode.Repeat);
```

---

### 5. ConfirmPromotionForm.cs
**Namespace**: `LaserMacsaUser.Views`

**Propósito**: Confirmar una promoción antes de aplicarla.

**Uso**:
```csharp
var confirmForm = new ConfirmPromotionForm(artworkId, promotionName);
if (confirmForm.ShowDialog() == DialogResult.OK)
{
    Promotion? promotion = confirmForm.ConfirmedPromotion;
    // Usar promoción confirmada...
}
```

---

## 📐 Convenciones

### Nombres de Archivos
- Formularios: `[Nombre]Form.cs` (ej: `LoginForm.cs`)
- Designers: `Designers/[Nombre]Form.Designer.cs`
- Resources: `Resources/[Nombre]Form.resx`

### Namespace
Todos los formularios usan: `LaserMacsaUser.Views`

### Estructura de Clase
```csharp
namespace LaserMacsaUser.Views
{
    public partial class [Nombre]Form : Form
    {
        // Propiedades públicas
        public string SomeProperty { get; set; }
        
        // Constructor
        public [Nombre]Form()
        {
            InitializeComponent();
            // Inicialización adicional
        }
        
        // Eventos
        private void BtnAction_Click(object? sender, EventArgs e)
        {
            // Lógica del evento
        }
    }
}
```

### Separación de Responsabilidades
- **Views NO deben**:
  - Acceder directamente a base de datos
  - Contener lógica de negocio compleja
  - Conocer detalles de implementación de servicios

- **Views DEBEN**:
  - Delegar acciones a Controllers
  - Mostrar datos al usuario
  - Capturar eventos del usuario
  - Validar entrada básica (formato, campos requeridos)

---

## ➕ Cómo Agregar un Nuevo Formulario

### Paso 1: Crear el Formulario

1. **Crear archivo principal**:
   ```
   Views/MyNewForm.cs
   ```

2. **Código inicial**:
   ```csharp
   using System.Windows.Forms;
   
   namespace LaserMacsaUser.Views
   {
       public partial class MyNewForm : Form
       {
           public MyNewForm()
           {
               InitializeComponent();
           }
       }
   }
   ```

### Paso 2: Crear el Designer

1. **Crear archivo Designer**:
   ```
   Views/Designers/MyNewForm.Designer.cs
   ```

2. **Código inicial**:
   ```csharp
   namespace LaserMacsaUser.Views
   {
       partial class MyNewForm
       {
           private System.ComponentModel.IContainer components = null;
           
           protected override void Dispose(bool disposing)
           {
               if (disposing && (components != null))
                   components.Dispose();
               base.Dispose(disposing);
           }
           
           private void InitializeComponent()
           {
               this.SuspendLayout();
               // Configuración del formulario
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.ClientSize = new System.Drawing.Size(400, 300);
               this.Text = "Mi Nuevo Formulario";
               this.ResumeLayout(false);
           }
       }
   }
   ```

### Paso 3: Crear el Resource File

1. **Crear archivo .resx**:
   ```
   Views/Resources/MyNewForm.resx
   ```

2. **Contenido mínimo** (XML):
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <root>
     <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
       <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
       <xsd:element name="root" msdata:IsDataSet="true">
         <xsd:complexType>
           <xsd:choice maxOccurs="unbounded">
             <xsd:element name="metadata">
               <xsd:complexType>
                 <xsd:sequence>
                   <xsd:element name="value" type="xsd:string" minOccurs="0" />
                 </xsd:sequence>
                 <xsd:attribute name="name" use="required" type="xsd:string" />
                 <xsd:attribute name="type" type="xsd:string" />
                 <xsd:attribute name="mimetype" type="xsd:string" />
               </xsd:complexType>
             </xsd:element>
             <xsd:element name="assembly">
               <xsd:complexType>
                 <xsd:attribute name="alias" type="xsd:string" />
                 <xsd:attribute name="name" type="xsd:string" />
               </xsd:complexType>
             </xsd:element>
             <xsd:element name="data">
               <xsd:complexType>
                 <xsd:sequence>
                   <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                   <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
                 </xsd:sequence>
                 <xsd:attribute name="name" type="xsd:string" use="required" />
                 <xsd:attribute name="type" type="xsd:string" />
                 <xsd:attribute name="mimetype" type="xsd:string" />
               </xsd:complexType>
             </xsd:element>
             <xsd:element name="resheader">
               <xsd:complexType>
                 <xsd:sequence>
                   <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                 </xsd:sequence>
                 <xsd:attribute name="name" type="xsd:string" use="required" />
               </xsd:complexType>
             </xsd:element>
           </xsd:choice>
         </xsd:complexType>
       </xsd:element>
     </xsd:schema>
     <resheader name="resmimetype">
       <value>text/microsoft-resx</value>
     </resheader>
     <resheader name="version">
       <value>2.0</value>
     </resheader>
     <resheader name="reader">
       <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
     </resheader>
     <resheader name="writer">
       <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
     </resheader>
   </root>
   ```

### Paso 4: Actualizar el Proyecto

El archivo `.csproj` detecta automáticamente los archivos `.cs`, pero asegúrate de que las relaciones estén configuradas:

```xml
<Compile Update="Views\Designers\MyNewForm.Designer.cs">
  <DependentUpon>MyNewForm.cs</DependentUpon>
</Compile>

<EmbeddedResource Update="Views\Resources\MyNewForm.resx">
  <DependentUpon>MyNewForm.cs</DependentUpon>
</EmbeddedResource>
```

### Paso 5: Usar el Formulario

```csharp
// Mostrar como diálogo modal
var form = new MyNewForm();
if (form.ShowDialog() == DialogResult.OK)
{
    // Procesar resultado
}

// Mostrar como ventana no modal
var form = new MyNewForm();
form.Show();
```

---

## 🔗 Referencias

- [README Principal](../README.md) - Arquitectura general
- [Controllers/README.md](../Controllers/README.md) - Cómo comunicarse con controladores
- [Services/README.md](../Services/README.md) - Servicios disponibles

---

**Última actualización**: 2025-11-23

