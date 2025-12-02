using System;
using System.Collections.Generic;
using LaserMacsaUser.Models;

namespace LaserMacsaUser.Services
{
    /// <summary>
    /// Servicio para manejo de alarmas del láser según manual alarmcodes.pdf
    /// </summary>
    public class LaserAlarmService
    {
        /// <summary>
        /// Tipos de alarmas según el manual
        /// </summary>
        public enum AlarmType
        {
            None = 0,
            CommunicationError = 2,
            HardwareError = 3,
            BufferFull = 9,
            HighTemperature = 10,
            FileError = 16,
            Unknown = 999
        }

        /// <summary>
        /// Severidad de la alarma
        /// </summary>
        public enum AlarmSeverity
        {
            Info,
            Warning,
            Error,
            Critical
        }

        /// <summary>
        /// Información de alarma
        /// </summary>
        public class AlarmInfo
        {
            public AlarmType Type { get; set; }
            public AlarmSeverity Severity { get; set; }
            public string Code { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Solution { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
        }

        private static readonly Dictionary<ushort, AlarmInfo> AlarmDefinitions = new()
        {
            // Errores críticos
            {
                2,
                new AlarmInfo
                {
                    Type = AlarmType.CommunicationError,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_002",
                    Description = "Error de comunicación con el láser",
                    Solution = "Verificar conexión de red, IP del láser y que el láser esté encendido"
                }
            },
            {
                3,
                new AlarmInfo
                {
                    Type = AlarmType.HardwareError,
                    Severity = AlarmSeverity.Critical,
                    Code = "ERR_003",
                    Description = "Error de hardware en el láser",
                    Solution = "Revisar hardware del láser, contactar soporte técnico"
                }
            },
            {
                4,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_004",
                    Description = "Error del láser (código 4)",
                    Solution = "Consultar manual de alarmas o contactar soporte técnico"
                }
            },
            {
                5,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_005",
                    Description = "Error del láser (código 5)",
                    Solution = "Consultar manual de alarmas o contactar soporte técnico"
                }
            },
            {
                6,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_006",
                    Description = "Error del láser (código 6)",
                    Solution = "Consultar manual de alarmas o contactar soporte técnico"
                }
            },
            {
                7,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_007",
                    Description = "Error del láser (código 7)",
                    Solution = "Consultar manual de alarmas o contactar soporte técnico"
                }
            },
            {
                8,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_008",
                    Description = "Error del láser (código 8)",
                    Solution = "Consultar manual de alarmas o contactar soporte técnico"
                }
            },
            // Advertencias
            {
                9,
                new AlarmInfo
                {
                    Type = AlarmType.BufferFull,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_009",
                    Description = "Buffer del láser lleno",
                    Solution = "Esperar a que el buffer se vacíe o aumentar tamaño del buffer"
                }
            },
            {
                10,
                new AlarmInfo
                {
                    Type = AlarmType.HighTemperature,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_010",
                    Description = "Temperatura alta en el láser",
                    Solution = "Verificar sistema de refrigeración, permitir que el láser se enfríe"
                }
            },
            {
                11,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_011",
                    Description = "Advertencia del láser (código 11)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                12,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_012",
                    Description = "Advertencia del láser (código 12)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                13,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_013",
                    Description = "Advertencia del láser (código 13)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            // Errores de archivo
            {
                16,
                new AlarmInfo
                {
                    Type = AlarmType.FileError,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_016",
                    Description = "Error en archivo de mensaje",
                    Solution = "Verificar que el archivo .msf existe y es válido"
                }
            },
            {
                17,
                new AlarmInfo
                {
                    Type = AlarmType.FileError,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_017",
                    Description = "Error en archivo (código 17)",
                    Solution = "Verificar archivos del láser"
                }
            },
            {
                18,
                new AlarmInfo
                {
                    Type = AlarmType.FileError,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_018",
                    Description = "Error en archivo (código 18)",
                    Solution = "Verificar archivos del láser"
                }
            },
            {
                20,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_020",
                    Description = "Error del láser (código 20)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                21,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_021",
                    Description = "Error del láser (código 21)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                22,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_022",
                    Description = "Error del láser (código 22)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                37,
                new AlarmInfo
                {
                    Type = AlarmType.HardwareError,
                    Severity = AlarmSeverity.Critical,
                    Code = "ERR_037",
                    Description = "Error de hardware en el láser",
                    Solution = "Revisar hardware del láser, contactar soporte técnico"
                }
            },
            {
                38,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_038",
                    Description = "Error del láser (código 38)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                48,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_048",
                    Description = "Advertencia del láser (código 48)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                50,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_050",
                    Description = "Advertencia del láser (código 50)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                51,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_051",
                    Description = "Fallo en creación de código de barras",
                    Solution = "Verificar configuración de código de barras"
                }
            },
            {
                52,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_052",
                    Description = "Advertencia del láser (código 52)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                53,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_053",
                    Description = "Advertencia del láser (código 53)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                54,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_054",
                    Description = "Advertencia del láser (código 54)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                55,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_055",
                    Description = "Advertencia del láser (código 55)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                56,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_056",
                    Description = "Advertencia del láser (código 56)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                57,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_057",
                    Description = "Advertencia del láser (código 57)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                65,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_065",
                    Description = "Error del láser (código 65)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                66,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_066",
                    Description = "Error del láser (código 66)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                67,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_067",
                    Description = "Error del láser (código 67)",
                    Solution = "Consultar manual de alarmas"
                }
            },
            {
                68,
                new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Error,
                    Code = "ERR_068",
                    Description = "Error del láser (código 68)",
                    Solution = "Consultar manual de alarmas"
                }
            }
        };

        /// <summary>
        /// Procesa un código de error del láser y retorna información de alarma
        /// Basado en el manual alarmcodes.pdf y código original CLaser.cs
        /// </summary>
        public static AlarmInfo ProcessErrorCode(uint errorCode)
        {
            // El errorCode tiene estructura: upper WORD = último error activo, lower WORD = error actual
            ushort lowerWord = (ushort)(errorCode & 0xFFFF);
            ushort upperWord = (ushort)((errorCode >> 16) & 0xFFFF);

            // Si no hay error
            if (lowerWord == 0)
            {
                return new AlarmInfo
                {
                    Type = AlarmType.None,
                    Severity = AlarmSeverity.Info,
                    Code = "OK",
                    Description = "Sin errores",
                    Solution = "",
                    Timestamp = DateTime.Now
                };
            }

            // Buscar en diccionario de definiciones
            if (AlarmDefinitions.TryGetValue(lowerWord, out AlarmInfo? alarm))
            {
                return new AlarmInfo
                {
                    Type = alarm.Type,
                    Severity = alarm.Severity,
                    Code = alarm.Code,
                    Description = alarm.Description,
                    Solution = alarm.Solution,
                    Timestamp = DateTime.Now
                };
            }

            // Error desconocido
            return new AlarmInfo
            {
                Type = AlarmType.Unknown,
                Severity = AlarmSeverity.Error,
                Code = $"ERR_{lowerWord:X4}",
                Description = $"Error desconocido: {lowerWord} (0x{lowerWord:X4})",
                Solution = "Consultar manual de alarmas (alarmcodes.pdf) o contactar soporte técnico",
                Timestamp = DateTime.Now
            };
        }

        /// <summary>
        /// Procesa el estado del láser y retorna alarmas activas
        /// </summary>
        public static List<AlarmInfo> ProcessLaserStatus(LaserStatus status)
        {
            var alarms = new List<AlarmInfo>();

            if (status.ErrorCode != 0)
            {
                var alarm = ProcessErrorCode(status.ErrorCode);
                if (alarm.Type != AlarmType.None)
                {
                    alarms.Add(alarm);
                }
            }

            // Verificar estado adicional
            if (!string.IsNullOrEmpty(status.ErrorDescription))
            {
                alarms.Add(new AlarmInfo
                {
                    Type = AlarmType.Unknown,
                    Severity = AlarmSeverity.Warning,
                    Code = "WARN_DESC",
                    Description = status.ErrorDescription,
                    Solution = "Revisar descripción del error",
                    Timestamp = DateTime.Now
                });
            }

            return alarms;
        }

        /// <summary>
        /// Verifica si una alarma requiere detener la producción
        /// </summary>
        public static bool ShouldStopProduction(AlarmInfo alarm)
        {
            return alarm.Severity == AlarmSeverity.Error || 
                   alarm.Severity == AlarmSeverity.Critical;
        }
    }
}

