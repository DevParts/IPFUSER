using System.Collections.Generic;

namespace LaserMacsaUser.Models
{
    /// <summary>
    /// Resultado de la validación de códigos de la base de datos
    /// </summary>
    public class CodeValidationResult
    {
        public bool IsValid { get; set; }
        public int TotalCodesChecked { get; set; }
        public int ValidCodes { get; set; }
        public int InvalidCodes { get; set; }
        public List<CodeValidationDetail> Details { get; set; } = new List<CodeValidationDetail>();
        public string Summary { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detalle de validación de un código individual
    /// </summary>
    public class CodeValidationDetail
    {
        public int CodeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> SplitParts { get; set; } = new List<string>();
    }
}

