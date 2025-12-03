using LaserMacsaUser.Views;
using LaserMacsaUser.Views.AppInfo;

namespace LaserMacsaUser
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Cargar idioma guardado
            string savedLanguage = Properties.Settings.Default.Language;

            // Aplicar cultura
            if (savedLanguage == "Español")
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es");
            else
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");

            Application.Run(new SplashForm());
        }
    }
}
