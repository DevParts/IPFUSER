using System;
using System.ComponentModel;

namespace LaserMacsaUser.Views.AppInfo
{
    internal class AppSettings
    {
        // ======================
        // GLOBAL
        // ======================
        [Category("Global")]
        [Description("Application Version")]
        public string AppVersion { get; set; } = "1.0.0.0";

        [Category("Global")]
        [Description("Selected language")]
        [TypeConverter(typeof(LanguageConverter))]
        public string Language
        {
            get => Properties.Settings.Default.Language;
            set => Properties.Settings.Default.Language = value;
        }


        // ======================
        // SECURITY
        // ======================
        [Category("Security")]
        [PasswordPropertyText(false)]
        [Description("Password used to access configuration screens.")]
        public string AppPassword
        {
            get => Properties.Settings.Default.AppPassword;
            set => Properties.Settings.Default.AppPassword = value;
        }


        // ======================
        // CODES
        // ======================
        [Category("Codes")]
        [Description("Low warning threshold level.")]
        public int LowLevelWarning { get; set; } = 50;

        [Category("Codes")]
        [Description("Show low warning alerts.")]
        public bool ShowLowLevels { get; set; } = true;

        [Category("Codes")]
        [Description("Show very low warning alerts.")]
        public bool ShowVeryLowLevels { get; set; } = true;

        [Category("Codes")]
        [Description("Very low warning threshold.")]
        public int VeryLowLevelWarning { get; set; } = 25;


        // ======================
        // DATABASE
        // ======================
        [Category("Database")]
        [Description("Database catalog.")]
        public string Catalog { get; set; } = "IPFEu";

        [Category("Database")]
        [Description("Full connection string.")]
        public string ConnectionString { get; set; } = "";

        [Category("Database")]
        [Description("SQL Server data source.")]
        public string DataSource { get; set; } = "(local)\\sqlexpress";

        [Category("Database")]
        [Description("Database username.")]
        public string User { get; set; } = "";

        [Category("Database")]
        [PasswordPropertyText(true)]
        [Description("Database password.")]
        public string Password { get; set; } = "";

        [Category("Database")]
        [Description("Use Windows Authentication.")]
        public bool UseWindowsAuthentication { get; set; } = true;


        // ======================
        // LASER  (AGREGADO AQUÍ)
        // ======================
        [Category("Laser")]
        [Description("Laser IP Address.")]
        public string LaserIP
        {
            get => Properties.Settings.Default.LaserIP;
            set => Properties.Settings.Default.LaserIP = value;
        }

        [Category("Laser")]
        [Description("Laser buffer size (número de códigos que puede almacenar el buffer del láser). Valores recomendados: 50-200. Por defecto: 100.")]
        public int LaserBufferSize
        {
            get => Properties.Settings.Default.LaserBufferSize;
            set => Properties.Settings.Default.LaserBufferSize = value;
        }


        // ======================
        // TIMING
        // ======================
        [Category("Timing")]
        [Description("Seconds to wait before retry.")]
        public int WaitTime { get; set; } = 5;

        [Category("Timing")]
        [Description("Tiempo de espera (ms) cuando el buffer del láser está lleno. Por defecto: 50ms.")]
        public int WaitTimeBufferFull
        {
            get => Properties.Settings.Default.WaitTimeBufferFull;
            set => Properties.Settings.Default.WaitTimeBufferFull = value;
        }


        // ======================
        protected string myVersion = "1.0.0.0";

        // ======================
        // APP VERSION (READ ONLY)
        // ======================
        [Category("AppVersion")]
        [ReadOnly(true)]
        public string ApplicationVersion => myVersion;
    }
}
