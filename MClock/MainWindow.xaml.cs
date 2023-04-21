using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MClock.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Uwp.Notifications;

namespace MClock
{
    public partial class MainWindow : Window
    {
        private readonly IConfiguration _configuration;
        private const int StartHour = 8;
        private const int EndHour = 17;
        private const int EndHourMinutes = 30;
        public const int KaizenTimeStartHour = 13;
        public const int KaizenTimeStartMinutes = 30;
        private readonly AppSettings _appSettings;

        public bool IsKaizenTime = false;

        public MainWindow(IConfiguration configuration)
        {
            _configuration = configuration;
            InitializeComponent();
            _appSettings = CreateSettings();
            Timer.Loaded += Timer_Loaded;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            if (_appSettings.InvertColours)
            {
               SetTimelineColour(Colors.Green);
               SetBacklineColour(Colors.Red);
            }
            
            ChangeColoursIfKaizenTime();
        }

        private AppSettings CreateSettings()
        {
            return new AppSettings
            {
                AutoStartWorkApps = Convert.ToBoolean(_configuration["AutoStartWorkApps"]),
                EnableNotifications = Convert.ToBoolean(_configuration["EnableNotifications"]),
                InvertColours = Convert.ToBoolean(_configuration["InvertColours"])
            };
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
            ChangeColoursIfKaizenTime();
            ShowTime();            
        }

        private void SetTimelineColour(Color colour)
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
            {
                TimeLine.Fill = new SolidColorBrush(colour);
            });
        }
        
        private void SetBacklineColour(Color colour)
        {
            Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
            {
                BackLine.Fill = new SolidColorBrush(colour);
            });
        }

        private void ChangeColoursIfKaizenTime()
        {
            if (_appSettings.EnableKaizenTimeColours)
            {
                var currentTimeOfDay = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                var KaizenTimeStart = new TimeOnly(KaizenTimeStartHour, KaizenTimeStartMinutes, 0);
                var KaizenTimeEnd = new TimeOnly(EndHour, EndHourMinutes, 0);

                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && currentTimeOfDay > KaizenTimeStart && currentTimeOfDay < KaizenTimeEnd && !IsKaizenTime)
                {
                    IsKaizenTime = true;
                    SetTimelineColour(Colors.Purple);
                }
            }
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
