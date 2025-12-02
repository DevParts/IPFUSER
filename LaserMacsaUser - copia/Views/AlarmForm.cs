using System;
using System.Windows.Forms;
using LaserMacsaUser.Services;

namespace LaserMacsaUser.Views
{
    /// <summary>
    /// Formulario de alarma que se puede cerrar automáticamente o manualmente
    /// </summary>
    public partial class AlarmForm : Form
    {
        private readonly LaserAlarmService.AlarmInfo _alarm;
        private readonly System.Timers.Timer _autoCloseTimer;
        private bool _wasClosedManually = false;
        private const int AUTO_CLOSE_SECONDS = 5; // Cerrar automáticamente después de 5 segundos
        
        public bool WasClosedManually => _wasClosedManually;
        public string AlarmKey { get; }
        
        public AlarmForm(LaserAlarmService.AlarmInfo alarm)
        {
            _alarm = alarm;
            AlarmKey = $"{alarm.Code}_{alarm.Type}";
            
            InitializeComponent();
            SetupForm();
            
            // Timer para cerrar automáticamente
            _autoCloseTimer = new System.Timers.Timer(AUTO_CLOSE_SECONDS * 1000);
            _autoCloseTimer.Elapsed += (s, e) => 
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => CloseAutomatically()));
                }
                else
                {
                    CloseAutomatically();
                }
            };
            _autoCloseTimer.AutoReset = false;
            _autoCloseTimer.Start();
        }
        
        private void SetupForm()
        {
            // Configurar formulario
            Text = $"Laser Alarm - {_alarm.Severity}";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true; // Siempre visible
            
            // Configurar icono según severidad
            Icon = _alarm.Severity switch
            {
                LaserAlarmService.AlarmSeverity.Info => SystemIcons.Information,
                LaserAlarmService.AlarmSeverity.Warning => SystemIcons.Warning,
                LaserAlarmService.AlarmSeverity.Error => SystemIcons.Error,
                LaserAlarmService.AlarmSeverity.Critical => SystemIcons.Error,
                _ => SystemIcons.Warning
            };
            
            // Crear panel principal
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            
            // Crear label de mensaje
            var lblMessage = new Label
            {
                Text = $"{_alarm.Code}: {_alarm.Description}\n\n" +
                       $"Solución: {_alarm.Solution}\n\n" +
                       $"Timestamp: {_alarm.Timestamp:yyyy-MM-dd HH:mm:ss}\n\n" +
                       $"(Se cerrará automáticamente en {AUTO_CLOSE_SECONDS} segundos si persiste)",
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.TopLeft,
                Padding = new Padding(0, 0, 0, 10)
            };
            
            // Crear botón cerrar
            var btnClose = new Button
            {
                Text = "Cerrar (No volver a mostrar)",
                DialogResult = DialogResult.OK,
                Dock = DockStyle.Bottom,
                Height = 35,
                UseVisualStyleBackColor = true
            };
            btnClose.Click += (s, e) => 
            {
                _wasClosedManually = true;
                _autoCloseTimer?.Stop();
            };
            
            panel.Controls.Add(lblMessage);
            panel.Controls.Add(btnClose);
            Controls.Add(panel);
            
            // Ajustar tamaño del formulario
            Width = 500;
            Height = 300;
        }
        
        private void CloseAutomatically()
        {
            if (!_wasClosedManually)
            {
                _wasClosedManually = false; // Cierre automático
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _autoCloseTimer?.Stop();
            _autoCloseTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

