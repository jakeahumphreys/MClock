using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MClock.Common;
using MClock.Settings.Types;

namespace MClock
{
    public partial class MainWindow : Window
    {
        private readonly TimeHelper _timeHelper;
        private readonly RichPresenceService _richPresenceService;
        private readonly ColourManager _colourManager;
        private readonly MathHelper _mathHelper;

        public MainWindow(AppSettings appSettings)
        {
            InitializeComponent();

            _timeHelper = new TimeHelper(appSettings);
            
            _colourManager = new ColourManager(this, appSettings);
            _colourManager.UpdateAppColours();
            _richPresenceService = new RichPresenceService(this, appSettings);
            _richPresenceService.StartRichPresenceIfEnabled();
            _mathHelper = new MathHelper(appSettings);

            Timer.Loaded += Timer_Loaded;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Application.Current.MainWindow.Closing += OnWindowClosing;
            
            if (_timeHelper.IsWeekend() && appSettings.GeneralSettings.CloseAppOnWeekends)
            {
                Close();
            }
        }

        private static void Current_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private void Timer_Loaded(object sender, RoutedEventArgs e)
        {
            SetTimeLabel();
            var timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += ShowTick;
            timer.Enabled = true;
        }

        private void ShowTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _richPresenceService.UpdateRichPresenceIfEnabled();
            _colourManager.UpdateAppColours();
            SetTimeLabel();            
        }
        
        private void SetTimeLabel()
        {
            Dispatcher.Invoke(() =>
            {
                Timer.Text = DateTime.Now.ToString("ddd dd MMM\r\nHH:mm:ss");
                SetTimeline();
            });
        }

        private void SetTimeline()
        {
            var time = DateTime.Now;
            var allDay = OuterGrid.ActualWidth;
            var newWidth = _mathHelper.GetNewWidth(time, allDay);
            if (_timeHelper.IsBeforeWork())
            {
                TimeLine.Width = 0;
                BackLine.Width = allDay;
                NightLine.Width = allDay - newWidth;
                NightLine.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (_timeHelper.IsDuringWork())
            {
                TimeLine.Width = newWidth;
                BackLine.Width = allDay;
                NightLine.Width = 0;
            }
            else
            {
                TimeLine.Width = allDay;
                BackLine.Width = allDay;
                NightLine.Width = newWidth;
                NightLine.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            _richPresenceService.StopRichPresenceIfEnabled();
        }

        private void DiscordConnectedImage_mouseDown(object sender, EventArgs e)
        {
            _richPresenceService.OverrideRichPresence();
        }
        
        private void WindowDeactivated(object sender, EventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }

        private void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }
    }
}
