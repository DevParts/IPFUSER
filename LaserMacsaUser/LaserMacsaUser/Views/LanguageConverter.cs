using System;
using System.ComponentModel;

namespace LaserMacsaUser.Views.AppInfo
{
    internal class LanguageConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            // Habilita lista desplegable (ComboBox)
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        {
            // Solo permite elegir de la lista (no escribir texto)
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            return new StandardValuesCollection(new string[]
            {
                "English",
                "Español"
            });
        }
    }
}
