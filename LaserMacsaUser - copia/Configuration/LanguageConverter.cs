using System;
using System.ComponentModel;
using System.Globalization;

namespace LaserMacsaUser.Configuration
{
    /// <summary>
    /// TypeConverter para mostrar Language como un ComboBox en PropertyGrid
    /// </summary>
    public class LanguageConverter : StringConverter
    {
        private static readonly string[] _languages = { "English", "Español" };

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
        {
            return true; // Solo permite valores de la lista
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            return new StandardValuesCollection(_languages);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string stringValue)
            {
                // Normalizar el valor para que coincida con los valores estándar
                if (stringValue.Equals("Español", StringComparison.OrdinalIgnoreCase) || 
                    stringValue.Equals("Spanish", StringComparison.OrdinalIgnoreCase) ||
                    stringValue.Equals("ES", StringComparison.OrdinalIgnoreCase))
                {
                    return "Español";
                }
                if (stringValue.Equals("English", StringComparison.OrdinalIgnoreCase) || 
                    stringValue.Equals("EN", StringComparison.OrdinalIgnoreCase))
                {
                    return "English";
                }
                // Si no coincide, retornar el valor original
                return stringValue;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}

