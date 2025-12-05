using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Helper estático para validar códigos de la base de datos
    /// </summary>
    public static class CodeValidator
    {
        /// <summary>
        /// Valida una muestra de códigos de la base de datos para verificar que se extraen y dividen correctamente
        /// </summary>
        /// <param name="promotion">Promoción con configuración de splits</param>
        /// <param name="databaseService">Servicio de base de datos</param>
        /// <param name="sampleSize">Número de códigos a validar (por defecto 10)</param>
        /// <returns>Resultado de la validación con detalles</returns>
        public static CodeValidationResult ValidateCodes(Promotion promotion, IDatabaseService databaseService, int sampleSize = 10)
        {
            var result = new CodeValidationResult();

            try
            {
                // Verificar que la promoción tenga configuración válida
                if (promotion == null)
                {
                    result.IsValid = false;
                    result.Summary = "Error: No hay promoción seleccionada.";
                    return result;
                }

                // Verificar que la base de datos esté conectada
                if (databaseService == null)
                {
                    result.IsValid = false;
                    result.Summary = "Error: Servicio de base de datos no disponible.";
                    return result;
                }

                // ⬇️ AGREGAR ESTA VERIFICACIÓN ⬇️
                // Verificar que la base de datos de códigos esté conectada antes de intentar obtener códigos
                if (!databaseService.IsCodesDatabaseConnected())
                {
                    result.IsValid = false;
                    result.Summary = "Error: La base de datos de códigos no está conectada. Por favor, conéctese primero usando ConnectCodesDatabase.";
                    System.Diagnostics.Debug.WriteLine($"CodeValidator: IsCodesDatabaseConnected retorna false. No se puede validar.");
                    return result;
                }
                System.Diagnostics.Debug.WriteLine($"CodeValidator: Verificación de conexión OK. Obteniendo códigos...");
                // ⬆️ FIN VERIFICACIÓN ⬆️

                // 1. Obtener muestra de códigos de la BD
                string sql = promotion.GetSqlCodes(sampleSize);
                DataTable table = databaseService.GetCodes(sql);

                result.TotalCodesChecked = table.Rows.Count;

                if (table.Rows.Count == 0)
                {
                    result.IsValid = false;
                    result.Summary = "No se encontraron códigos en la base de datos para validar.";
                    return result;
                }

                // 2. Validar cada código
                foreach (DataRow row in table.Rows)
                {
                    var detail = new CodeValidationDetail
                    {
                        CodeId = Convert.ToInt32(row["Id"]),
                        Code = row["Code"]?.ToString() ?? string.Empty
                    };

                    // Validar código
                    string validationError = ValidateSingleCode(detail.Code, promotion);

                    if (string.IsNullOrEmpty(validationError))
                    {
                        detail.IsValid = true;
                        result.ValidCodes++;

                        // Mostrar cómo se divide el código
                        detail.SplitParts = GetSplitParts(detail.Code, promotion);
                    }
                    else
                    {
                        detail.IsValid = false;
                        detail.ErrorMessage = validationError;
                        result.InvalidCodes++;
                    }

                    result.Details.Add(detail);
                }

                // 3. Generar resumen
                result.IsValid = result.InvalidCodes == 0;
                result.Summary = GenerateValidationSummary(result, promotion);

                return result;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Summary = $"Error al validar códigos: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Valida un código individual según la configuración de la promoción
        /// </summary>
        private static string ValidateSingleCode(string code, Promotion promo)
        {
            // 1. Verificar que el código no esté vacío
            if (string.IsNullOrWhiteSpace(code))
            {
                return "Código vacío o solo espacios en blanco";
            }

            string trimmedCode = code.Trim();

            // 2. Verificar longitud mínima según UserFields
            int minLength = GetMinimumCodeLength(promo);
            if (trimmedCode.Length < minLength)
            {
                return $"Código demasiado corto. Longitud: {trimmedCode.Length}, Mínimo requerido: {minLength}";
            }

            // 3. Verificar que las partes divididas no queden vacías
            if (promo.UserFields == 1)
            {
                // Un solo campo - solo verificar que no esté vacío
                if (string.IsNullOrEmpty(trimmedCode))
                {
                    return "Código vacío después de trim";
                }
            }
            else if (promo.UserFields == 2)
            {
                if (trimmedCode.Length < promo.Split1 + promo.Split2)
                {
                    return $"Código demasiado corto para dividir en 2 partes. Longitud: {trimmedCode.Length}, Requerido: {promo.Split1 + promo.Split2}";
                }

                string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();

                if (string.IsNullOrEmpty(part1))
                    return $"Parte 1 está vacía después de dividir (Split1={promo.Split1})";
                if (string.IsNullOrEmpty(part2))
                    return $"Parte 2 está vacía después de dividir (Split2={promo.Split2})";
            }
            else if (promo.UserFields == 3)
            {
                int totalLength = promo.Split1 + promo.Split2 + promo.Split3;
                if (trimmedCode.Length < totalLength)
                {
                    return $"Código demasiado corto para dividir en 3 partes. Longitud: {trimmedCode.Length}, Requerido: {totalLength}";
                }

                string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();
                string part3 = trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3).Trim();

                if (string.IsNullOrEmpty(part1))
                    return $"Parte 1 está vacía después de dividir (Split1={promo.Split1})";
                if (string.IsNullOrEmpty(part2))
                    return $"Parte 2 está vacía después de dividir (Split2={promo.Split2})";
                if (string.IsNullOrEmpty(part3))
                    return $"Parte 3 está vacía después de dividir (Split3={promo.Split3})";
            }
            else if (promo.UserFields == 4)
            {
                int totalLength = promo.Split1 + promo.Split2 + promo.Split3 + promo.Split4;
                if (trimmedCode.Length < totalLength)
                {
                    return $"Código demasiado corto para dividir en 4 partes. Longitud: {trimmedCode.Length}, Requerido: {totalLength}";
                }

                string part1 = trimmedCode.Substring(0, promo.Split1).Trim();
                string part2 = trimmedCode.Substring(promo.Split1, promo.Split2).Trim();
                string part3 = trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3).Trim();
                string part4 = trimmedCode.Substring(promo.Split1 + promo.Split2 + promo.Split3, promo.Split4).Trim();

                if (string.IsNullOrEmpty(part1))
                    return $"Parte 1 está vacía después de dividir (Split1={promo.Split1})";
                if (string.IsNullOrEmpty(part2))
                    return $"Parte 2 está vacía después de dividir (Split2={promo.Split2})";
                if (string.IsNullOrEmpty(part3))
                    return $"Parte 3 está vacía después de dividir (Split3={promo.Split3})";
                if (string.IsNullOrEmpty(part4))
                    return $"Parte 4 está vacía después de dividir (Split4={promo.Split4})";
            }

            return string.Empty; // Sin errores
        }

        /// <summary>
        /// Obtiene la longitud mínima requerida según UserFields y Splits
        /// </summary>
        private static int GetMinimumCodeLength(Promotion promo)
        {
            return promo.UserFields switch
            {
                1 => 1, // Mínimo 1 carácter
                2 => promo.Split1 + promo.Split2,
                3 => promo.Split1 + promo.Split2 + promo.Split3,
                4 => promo.Split1 + promo.Split2 + promo.Split3 + promo.Split4,
                _ => 1
            };
        }

        /// <summary>
        /// Obtiene las partes divididas de un código para mostrar en el reporte
        /// </summary>
        private static List<string> GetSplitParts(string code, Promotion promo)
        {
            var parts = new List<string>();
            string trimmedCode = code.Trim();

            if (promo.UserFields == 1)
            {
                parts.Add($"Campo 0: '{trimmedCode}' (código completo)");
            }
            else if (promo.UserFields == 2)
            {
                if (trimmedCode.Length >= promo.Split1 + promo.Split2)
                {
                    parts.Add($"Campo 0: '{trimmedCode.Substring(0, promo.Split1)}' (Split1={promo.Split1})");
                    parts.Add($"Campo 1: '{trimmedCode.Substring(promo.Split1, promo.Split2)}' (Split2={promo.Split2})");
                }
            }
            else if (promo.UserFields == 3)
            {
                if (trimmedCode.Length >= promo.Split1 + promo.Split2 + promo.Split3)
                {
                    parts.Add($"Campo 0: '{trimmedCode.Substring(0, promo.Split1)}' (Split1={promo.Split1})");
                    parts.Add($"Campo 1: '{trimmedCode.Substring(promo.Split1, promo.Split2)}' (Split2={promo.Split2})");
                    parts.Add($"Campo 2: '{trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3)}' (Split3={promo.Split3})");
                }
            }
            else if (promo.UserFields == 4)
            {
                if (trimmedCode.Length >= promo.Split1 + promo.Split2 + promo.Split3 + promo.Split4)
                {
                    parts.Add($"Campo 0: '{trimmedCode.Substring(0, promo.Split1)}' (Split1={promo.Split1})");
                    parts.Add($"Campo 1: '{trimmedCode.Substring(promo.Split1, promo.Split2)}' (Split2={promo.Split2})");
                    parts.Add($"Campo 2: '{trimmedCode.Substring(promo.Split1 + promo.Split2, promo.Split3)}' (Split3={promo.Split3})");
                    parts.Add($"Campo 3: '{trimmedCode.Substring(promo.Split1 + promo.Split2 + promo.Split3, promo.Split4)}' (Split4={promo.Split4})");
                }
            }

            return parts;
        }

        /// <summary>
        /// Genera un resumen legible de la validación
        /// </summary>
        private static string GenerateValidationSummary(CodeValidationResult result, Promotion promotion)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== VALIDACIÓN DE CÓDIGOS ===");
            sb.AppendLine($"Promoción: {promotion.JobName}");
            sb.AppendLine($"UserFields: {promotion.UserFields}");
            sb.AppendLine($"Splits: Split1={promotion.Split1}, Split2={promotion.Split2}, Split3={promotion.Split3}, Split4={promotion.Split4}");
            sb.AppendLine();
            sb.AppendLine($"Total de códigos validados: {result.TotalCodesChecked}");
            sb.AppendLine($"Códigos válidos: {result.ValidCodes}");
            sb.AppendLine($"Códigos inválidos: {result.InvalidCodes}");
            sb.AppendLine();

            if (result.InvalidCodes > 0)
            {
                sb.AppendLine("=== CÓDIGOS INVÁLIDOS ===");
                foreach (var detail in result.Details.Where(d => !d.IsValid))
                {
                    sb.AppendLine($"ID: {detail.CodeId}, Código: '{detail.Code}'");
                    sb.AppendLine($"  Error: {detail.ErrorMessage}");
                    sb.AppendLine();
                }
            }

            if (result.ValidCodes > 0)
            {
                sb.AppendLine("=== EJEMPLOS DE CÓDIGOS VÁLIDOS ===");
                int count = 0;
                foreach (var detail in result.Details.Where(d => d.IsValid))
                {
                    if (count >= 10) break; // Mostrar solo los primeros 5
                    sb.AppendLine($"ID: {detail.CodeId}");
                    sb.AppendLine($"Código completo: '{detail.Code}' (Longitud: {detail.Code.Trim().Length})");
                    foreach (var part in detail.SplitParts)
                    {
                        sb.AppendLine($"  • {part}");
                    }
                    sb.AppendLine();
                    count++;
                }
            }

            return sb.ToString();
        }
    }
}

