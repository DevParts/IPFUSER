using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SocketCommNet;
using LaserMacsaUser.Configuration;

namespace LaserMacsaUser.Views
{
    public partial class AppConfigForm : Form
    {
        private AppSettings _settingsBefore = null!;

        public AppConfigForm()
        {
            InitializeComponent();
            
            // Configurar fuente consistente para el PropertyGrid
            // Esto asegura que todas las letras se vean igual
            if (propertyGridConfig != null)
            {
                propertyGridConfig.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            }
            
            LoadSettings();
            btnOk.Click += BtnOk_Click;
            btnTestConnection.Click += BtnTestConnection_Click;
            btnRunTests.Click += BtnRunTests_Click;
        }

        private void LoadSettings()
        {
            // Clonar valores actuales para detectar cambios
            // AppSettings ahora lee directamente de Settings, así que creamos una copia
            _settingsBefore = new AppSettings()
            {
                AppPassword = Properties.Settings.Default.AppPassword,
                Laser_IP = Properties.Settings.Default.Laser_IP,
                LaserBufferSize = Properties.Settings.Default.LaserBufferSize
            };

            // Mostrar en el PropertyGrid (AppSettings ahora lee/escribe directamente de Settings)
            propertyGridConfig.SelectedObject = new AppSettings();
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            // Obtener valores modificados
            var updated = (AppSettings)propertyGridConfig.SelectedObject;

            bool passwordChanged = updated.AppPassword != _settingsBefore.AppPassword;
            bool ipChanged = updated.Laser_IP != _settingsBefore.Laser_IP;

            // Guardar configuraciones
            // AppSettings ya guarda automáticamente en Settings cuando se modifica
            // Solo necesitamos guardar explícitamente
            Properties.Settings.Default.Save();

            // Si la contraseña cambió, reiniciar
            if (passwordChanged)
            {
                var result = MessageBox.Show(
                    "La contraseña se ha modificado.\nSe necesita reiniciar la aplicación para aplicar los cambios.\n\n¿Desea reiniciar ahora?",
                    "Reinicio necesario",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    Application.Restart();
                    Environment.Exit(0);
                }
            }

            this.Close();
        }

        private void BtnTestConnection_Click(object? sender, EventArgs e)
        {
            // Obtener la IP actual del PropertyGrid
            var currentSettings = (AppSettings)propertyGridConfig.SelectedObject;
            string ipToTest = currentSettings.Laser_IP;

            if (string.IsNullOrWhiteSpace(ipToTest))
            {
                MessageBox.Show(
                    "Por favor, ingrese una dirección IP válida antes de probar la conexión.",
                    "IP no válida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Deshabilitar el botón mientras se prueba la conexión
            btnTestConnection.Enabled = false;
            btnTestConnection.Text = "Probando...";
            Application.DoEvents();

            try
            {
                // Crear conexión de prueba usando SocketComm directamente
                SocketComm socketComm = new SocketComm();
                Int32 puntero = 0;
                string nombreConexion = "TestConnection";
                string rutaLocal = ".\\";
                string errorMsg = "";

                // Inicializar
                socketComm.CS_Init(ref puntero, nombreConexion, ipToTest, rutaLocal);
                
                // Verificar errores de inicialización
                Int32 resultado = socketComm.CS_GetLastError(puntero, ref errorMsg);
                if (resultado != 0 && !string.IsNullOrWhiteSpace(errorMsg))
                {
                    MessageBox.Show(
                        $"Error al inicializar la conexión:\n{errorMsg}",
                        "Error de Inicialización",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Intentar conectar
                resultado = socketComm.CS_StartClient(puntero);
                if (resultado != 0)
                {
                    errorMsg = "";
                    socketComm.CS_GetLastError(puntero, ref errorMsg);
                    MessageBox.Show(
                        $"No se pudo establecer la conexión con la IP {ipToTest}:\n{errorMsg}\n\nPosibles causas:\n- La IP es incorrecta\n- La impresora no está encendida\n- Hay un firewall bloqueando la conexión",
                        "Error de Conexión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    
                    // Cerrar conexión
                    try
                    {
                        socketComm.CS_Knockout(puntero);
                        socketComm.CS_Finish(puntero);
                    }
                    catch { }
                    return;
                }

                // Verificar conexión
                Int32 estadoConexion = socketComm.CS_IsConnected(puntero);
                if (estadoConexion != 1)
                {
                    MessageBox.Show(
                        $"La conexión se estableció pero no está activa.\nIP: {ipToTest}",
                        "Advertencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    
                    // Cerrar conexión
                    try
                    {
                        socketComm.CS_Knockout(puntero);
                        socketComm.CS_Finish(puntero);
                    }
                    catch { }
                    return;
                }

                // Éxito
                MessageBox.Show(
                    $"¡Conexión exitosa!\n\nIP: {ipToTest}\nLa impresora está conectada y lista para usar.",
                    "Conexión Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Cerrar conexión de prueba
                try
                {
                    socketComm.CS_Knockout(puntero);
                    socketComm.CS_Finish(puntero);
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error inesperado al probar la conexión:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                // Restaurar el botón
                btnTestConnection.Enabled = true;
                btnTestConnection.Text = "Probar Conexión";
            }
        }

        private void BtnRunTests_Click(object? sender, EventArgs e)
        {
            try
            {
                // Guardar la IP actual antes de abrir los tests
                var currentSettings = (AppSettings)propertyGridConfig.SelectedObject;
                if (!string.IsNullOrWhiteSpace(currentSettings.Laser_IP))
                {
                    Properties.Settings.Default.Laser_IP = currentSettings.Laser_IP;
                    Properties.Settings.Default.Save();
                }

                // Buscar el ejecutable MacsaLaserTest.exe
                string testExePath = FindMacsaLaserTestExe();
                
                if (string.IsNullOrEmpty(testExePath) || !System.IO.File.Exists(testExePath))
                {
                    // Intentar compilar el proyecto si no existe
                    var compileResult = TryCompileMacsaLaserTest();
                    if (compileResult.Success)
                    {
                        // Buscar nuevamente después de compilar
                        testExePath = FindMacsaLaserTestExe();
                    }
                    
                    if (string.IsNullOrEmpty(testExePath) || !System.IO.File.Exists(testExePath))
                    {
                        string expectedPath = GetExpectedTestPath();
                        string message = "No se encontró el ejecutable MacsaLaserTest.exe.\n\n";
                        
                        if (!compileResult.Success)
                        {
                            message += $"Error al compilar: {compileResult.ErrorMessage}\n\n";
                        }
                        
                        message += "Por favor, compile el proyecto MacsaLaserTest manualmente.\n\n";
                        message += "Ruta esperada:\n" + expectedPath;
                        
                    MessageBox.Show(
                            message,
                        "Tests no encontrados",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                    }
                }

                // Ejecutar el menú de tests
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = testExePath,
                    UseShellExecute = true,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(testExePath)
                };

                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al ejecutar los tests:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private string FindMacsaLaserTestExe()
        {
            // Obtener la raíz del workspace (directorio padre de LaserMacsaUser)
            string? assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string? baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Intentar encontrar la raíz del workspace
            string? workspaceRoot = null;
            
            // Método 1: Desde la ubicación del ensamblado
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                string? assemblyDir = Path.GetDirectoryName(assemblyLocation);
                if (!string.IsNullOrEmpty(assemblyDir))
                {
                    // Navegar hacia arriba desde bin\Debug\net8.0-windows hasta la raíz
                    var dir = new DirectoryInfo(assemblyDir);
                    for (int i = 0; i < 4 && dir != null; i++)
                    {
                        dir = dir.Parent;
                    }
                    if (dir != null && Directory.Exists(Path.Combine(dir.FullName, "MacsaLaserTest")))
                    {
                        workspaceRoot = dir.FullName;
                    }
                }
            }
            
            // Método 2: Desde BaseDirectory
            if (string.IsNullOrEmpty(workspaceRoot) && !string.IsNullOrEmpty(baseDir))
            {
                var dir = new DirectoryInfo(baseDir);
                for (int i = 0; i < 4 && dir != null; i++)
                {
                    dir = dir.Parent;
                }
                if (dir != null && Directory.Exists(Path.Combine(dir.FullName, "MacsaLaserTest")))
                {
                    workspaceRoot = dir.FullName;
                }
            }
            
            // Método 3: Buscar desde el directorio actual
            if (string.IsNullOrEmpty(workspaceRoot))
            {
                string currentDir = Directory.GetCurrentDirectory();
                var dir = new DirectoryInfo(currentDir);
                
                // Buscar hacia arriba hasta encontrar MacsaLaserTest
                while (dir != null)
                {
                    string testPath = Path.Combine(dir.FullName, "MacsaLaserTest");
                    if (Directory.Exists(testPath))
                    {
                        workspaceRoot = dir.FullName;
                        break;
                    }
                    dir = dir.Parent;
                }
            }
            
            // Si encontramos la raíz, construir la ruta
            if (!string.IsNullOrEmpty(workspaceRoot))
            {
            string[] posiblesRutas = new string[]
            {
                    Path.Combine(workspaceRoot, "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe"),
                    Path.Combine(workspaceRoot, "MacsaLaserTest", "bin", "Release", "MacsaLaserTest.exe"),
                };
                
                foreach (string ruta in posiblesRutas)
                {
                    try
                    {
                        string rutaNormalizada = Path.GetFullPath(ruta);
                        if (File.Exists(rutaNormalizada))
                        {
                            return rutaNormalizada;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            
            // Fallback: rutas relativas desde diferentes ubicaciones
            string[] fallbackRutas = new string[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe"),
                Path.Combine(Directory.GetCurrentDirectory(), "..", "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe"),
                Path.Combine(Directory.GetCurrentDirectory(), "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe"),
            };

            foreach (string ruta in fallbackRutas)
            {
                try
                {
                    string rutaNormalizada = Path.GetFullPath(ruta);
                    if (File.Exists(rutaNormalizada))
                    {
                        return rutaNormalizada;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return string.Empty;
        }
        
        private (bool Success, string ErrorMessage) TryCompileMacsaLaserTest()
        {
            try
            {
                // Buscar el archivo .csproj
                string? workspaceRoot = null;
                string? assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                
                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    string? assemblyDir = Path.GetDirectoryName(assemblyLocation);
                    if (!string.IsNullOrEmpty(assemblyDir))
                    {
                        var dir = new DirectoryInfo(assemblyDir);
                        for (int i = 0; i < 4 && dir != null; i++)
                        {
                            dir = dir.Parent;
                        }
                        if (dir != null && Directory.Exists(Path.Combine(dir.FullName, "MacsaLaserTest")))
                        {
                            workspaceRoot = dir.FullName;
                        }
                    }
                }
                
                if (string.IsNullOrEmpty(workspaceRoot))
                {
                    return (false, "No se pudo encontrar la raíz del workspace");
                }
                
                string csprojPath = Path.Combine(workspaceRoot, "MacsaLaserTest", "MacsaLaserTest.csproj");
                if (!File.Exists(csprojPath))
                {
                    return (false, $"No se encontró el archivo del proyecto: {csprojPath}");
                }
                
                // Intentar compilar usando MSBuild (para .NET Framework)
                // Buscar MSBuild en ubicaciones comunes
                string[] msbuildPaths = new string[]
                {
                    @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
                    @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe",
                    @"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe",
                    "msbuild.exe" // Intentar en PATH
                };
                
                string? msbuildPath = null;
                foreach (string path in msbuildPaths)
                {
                    if (path == "msbuild.exe")
                    {
                        // Intentar encontrar en PATH
                        try
                        {
                            var psi = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "where.exe",
                                Arguments = "msbuild.exe",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            };
                            using (var proc = System.Diagnostics.Process.Start(psi))
                            {
                                if (proc != null)
                                {
                                    string? result = proc.StandardOutput.ReadLine();
                                    proc.WaitForExit();
                                    if (!string.IsNullOrEmpty(result) && File.Exists(result))
                                    {
                                        msbuildPath = result;
                                        break;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                    else if (File.Exists(path))
                    {
                        msbuildPath = path;
                        break;
                    }
                }
                
                if (string.IsNullOrEmpty(msbuildPath))
                {
                    return (false, "No se encontró MSBuild. Por favor, compile el proyecto MacsaLaserTest manualmente desde Visual Studio.");
                }
                
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = msbuildPath,
                    Arguments = $"\"{csprojPath}\" /p:Configuration=Debug /p:Platform=AnyCPU /t:Build /nologo /verbosity:minimal",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(csprojPath)
                };
                
                using (var process = System.Diagnostics.Process.Start(processInfo))
                {
                    if (process == null)
                    {
                        return (false, "No se pudo iniciar MSBuild.");
                    }
                    
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    
                    if (process.ExitCode == 0)
                    {
                        return (true, string.Empty);
                    }
                    else
                    {
                        return (false, $"Error de compilación. Salida: {output}\nErrores: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private string GetExpectedTestPath()
        {
            // Intentar encontrar la raíz del workspace
            string? assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                string? assemblyDir = Path.GetDirectoryName(assemblyLocation);
                if (!string.IsNullOrEmpty(assemblyDir))
                {
                    var dir = new DirectoryInfo(assemblyDir);
                    for (int i = 0; i < 4 && dir != null; i++)
                    {
                        dir = dir.Parent;
                    }
                    if (dir != null)
                    {
                        return Path.Combine(dir.FullName, "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe");
                    }
                }
            }
            
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..",
                "MacsaLaserTest", "bin", "Debug", "MacsaLaserTest.exe"
            );
        }
    }
}
