using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DiscordRPC;
using MClock.Common;
using MClock.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MClock
{
    public partial class MainWindow : Window
    {
        private readonly TimeHelper _timeHelper;
        private readonly RichPresenceService _richPresenceService;
        private readonly ColourManager _colourManager;

        public MainWindow(AppSettings appSettings)
        {
            InitializeComponent();

            _timeHelper = new TimeHelper(appSettings);
            
            _colourManager = new ColourManager(this, appSettings);
            _colourManager.UpdateAppColours();
            
            _richPresenceService = new RichPresenceService(appSettings);
            _richPresenceService.StartRichPresenceIfEnabled();

            Timer.Loaded += Timer_Loaded;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Application.Current.MainWindow.Closing += OnWindowClosing;
        }

        private static void Current_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private void HandleNotifications()
        {
            if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime())
            {
                new ToastContentBuilder().AddText("Work day has finished, remember to patch uncommitted work!").Show();
            }
        }

        private void Timer_Loaded(object sender, RoutedEventArgs e)
        {
            ShowTime();
            var timer = new System.Timers.Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += ShowTick;
            timer.Enabled = true;
        }

        private void ShowTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _richPresenceService.UpdateRichPresenceIfEnabled();
            _colourManager.UpdateAppColours();
            HandleNotifications();
            ShowTime();            
        }
        
        private void ShowTime()
        {
            this.Dispatcher.Invoke(() =>
            {
                Timer.Text = DateTime.Now.ToString("ddd dd MMM\r\nHH:mm:ss");
                SetTimeline();
            });
        }

        private void SetTimeline()
        {
            var time = DateTime.Now;
            var allDay = OuterGrid.ActualWidth;
            var newWidth = GetNewWidth(time, allDay);
            if (TimeHelper.IsBeforeWork())
            {
                TimeLine.Width = 0;
                BackLine.Width = allDay;
                NightLine.Width = allDay - newWidth;
                NightLine.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (TimeHelper.IsDuringWork())
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
        
        private double GetNewWidth(DateTime now, double fullWidth)
        {
            var currentTime = new TimeOnly(now.Hour, now.Minute, now.Second);
            if (TimeHelper.IsBeforeWork())
            {
                var partialDay = (now.Hour) * 60 + now.Minute;
                var fraction = MathHelper.GetFraction(TimeHelper.GetStartTime().Hour, partialDay);
                return  fullWidth * fraction;
            }
            else if (TimeHelper.IsDuringWork())
            {
                var partialDay = ((currentTime - TimeHelper.GetStartTime()).TotalMinutes) * 60 + now.Minute;
                var fraction = MathHelper.GetFraction((TimeHelper.GetEndTime() - TimeHelper.GetStartTime()).TotalMinutes, partialDay);
                return fullWidth * fraction;
            }
            else
            {
                var partialDay = ((currentTime - TimeHelper.GetEndTime()).TotalMinutes) * 60 + now.Minute;
                var fraction = MathHelper.GetFraction((TimeHelper.GetMidnight() - TimeHelper.GetEndTime()).TotalMinutes, partialDay);
                return fullWidth * fraction;
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _richPresenceService.StopRichPresenceIfEnabled();
        }
        

        private void WindowDeactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }
    }
}
