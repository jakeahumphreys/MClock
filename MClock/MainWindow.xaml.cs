using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Clock
{
    public partial class MainWindow : Window
    {
        private const int StartHour = 8;
        private const int EndHour = 17;
        private const int EndHourMinutes = 30;

        public MainWindow()
        {
            InitializeComponent();
            Timer.Loaded += Timer_Loaded;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }
        
        private static void Current_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private void HandleNotifications()
        {
            var timeOfDay = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            
            if (timeOfDay == new TimeOnly(EndHour, EndHourMinutes, 0))
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
            HandleNotifications();
            ShowTime();            
        }

        private void ShowTime()
        {
            this.Dispatcher.Invoke(() =>
            {
                Timer.Text = FormatTime();
                SetTimeline();
            });
        }

        private static string FormatTime()
        {
            return DateTime.Now.ToString("ddd dd MMM\r\nHH:mm:ss");
        }

        private void SetTimeline()
        {
            var time = DateTime.Now;
            var allDay = OuterGrid.ActualWidth;
            var newWidth = GetNewWidth(time, allDay);
            if (BeforeWork(time.Hour))
            {
                TimeLine.Width = 0;
                BackLine.Width = allDay;
                NightLine.Width = allDay - newWidth;
                NightLine.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (DuringWork(time.Hour))
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

        private bool DuringWork(int hour)
        {
            return hour < EndHour;
        }

        private bool BeforeWork(int hour)
        {
            return hour < StartHour;
        }

        private double GetNewWidth(DateTime now, double fullWidth)
        {
            if (BeforeWork(now.Hour))
            {
                var partialDay = (now.Hour) * 60 + now.Minute;
                var fraction = GetFraction(StartHour, partialDay);
                return  fullWidth * fraction;
            }
            else if (DuringWork(now.Hour))
            {
                var partialDay = (now.Hour - StartHour) * 60 + now.Minute;
                var fraction = GetFraction(EndHour - StartHour, partialDay);
                return fullWidth * fraction;
            }
            else
            {
                var partialDay = (now.Hour - EndHour) * 60 + now.Minute;
                var fraction = GetFraction(24 - EndHour, partialDay);
                return fullWidth * fraction;
            }
        }

        private double GetFraction(int startHour, int partialDay)
        {
            return partialDay / (startHour * 60.0);
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
