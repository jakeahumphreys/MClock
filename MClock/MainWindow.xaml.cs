using System;
using System.Threading;
using System.Windows;
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
                InvertColours = Convert.ToBoolean(_configuration["InvertColours"]),
                EnableKaizenTimeColours = Convert.ToBoolean(_configuration["EnableKaizenTimeColours"]),
                TimeSettings = new TimeSettings
                {
                    WorkStartTime = _configuration.GetSection("TimeSettings")["WorkStartTime"],
                    WorkEndTime = _configuration.GetSection("TimeSettings")["WorkEndTime"],
                    KaizenStartTime = _configuration.GetSection("TimeSettings")["KaizenStartTime"]
                }
            };
        }

        private static void Current_DispatcherUnhandledException (object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private void HandleNotifications()
        {
            var timeOfDay = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            
            if (timeOfDay == GetEndTime())
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

        // private void SetTimelineToRainbow()
        // {
        //     var gradientBrush = new LinearGradientBrush();
        //     gradientBrush.StartPoint = new Point(0,0);
        //     gradientBrush.EndPoint = new Point(1,1);
        //     gradientBrush.GradientStops.Add(
        //         new GradientStop(Colors.Yellow, 0.0));
        //     gradientBrush.GradientStops.Add(
        //         new GradientStop(Colors.Red, 0.25));
        //     gradientBrush.GradientStops.Add(
        //         new GradientStop(Colors.Blue, 0.75));
        //     gradientBrush.GradientStops.Add(
        //         new GradientStop(Colors.LimeGreen, 1.0));
        //     
        //     Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        //     {
        //         TimeLine.Fill = gradientBrush;
        //     });
        // }

        private void ChangeColoursIfKaizenTime()
        {
            if (_appSettings.EnableKaizenTimeColours)
            {
                var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && currentTime > GetKaizenStartTime() && currentTime < GetEndTime() && !IsKaizenTime)
                {
                    IsKaizenTime = true;
                    SetTimelineColour(Colors.Purple);
                }
            }
        }

        private TimeOnly GetStartTime()
        {
            var startTime = _appSettings.TimeSettings.WorkStartTime;
            var hour = Convert.ToInt32(startTime.Substring(0, 2));
            var minutes = Convert.ToInt32(startTime.Substring(3, 2));
            return new TimeOnly(hour, minutes, 0);
        }

        private TimeOnly GetEndTime()
        {
            var endTime = _appSettings.TimeSettings.WorkEndTime;
            var hour = Convert.ToInt32(endTime.Substring(0, 2));
            var minutes = Convert.ToInt32(endTime.Substring(3, 2));
            return new TimeOnly(hour, minutes, 0);
        }

        private TimeOnly GetKaizenStartTime()
        {
            var kaizenStartTime = _appSettings.TimeSettings.KaizenStartTime;
            var hour = Convert.ToInt32(kaizenStartTime.Substring(0, 2));
            var minutes = Convert.ToInt32(kaizenStartTime.Substring(3, 2));
            return new TimeOnly(hour, minutes, 0);
        }
        
        public TimeOnly GetMidnight()
        {
            return new TimeOnly(0, 0, 0);
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
            var currentTime = new TimeOnly(time.Hour, time.Minute, time.Second);
            
            var allDay = OuterGrid.ActualWidth;
            var newWidth = GetNewWidth(time, allDay);
            if (BeforeWork(currentTime))
            {
                TimeLine.Width = 0;
                BackLine.Width = allDay;
                NightLine.Width = allDay - newWidth;
                NightLine.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (DuringWork(currentTime))
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

        private bool DuringWork(TimeOnly currentTime)
        {
            return currentTime< GetEndTime();
        }

        private bool BeforeWork(TimeOnly currentTime)
        {
            return currentTime < GetStartTime();
        }

        private double GetNewWidth(DateTime now, double fullWidth)
        {
            var currentTime = new TimeOnly(now.Hour, now.Minute, now.Second);
            if (BeforeWork(currentTime))
            {
                var partialDay = (now.Hour) * 60 + now.Minute;
                var fraction = GetFraction(GetStartTime().Hour, partialDay);
                return  fullWidth * fraction;
            }
            else if (DuringWork(currentTime))
            {
                var partialDay = ((currentTime - GetStartTime()).TotalMinutes) * 60 + now.Minute;
                var fraction = GetFraction((GetEndTime() - GetStartTime()).TotalMinutes, partialDay);
                return fullWidth * fraction;
            }
            else
            {
                var partialDay = ((currentTime - GetEndTime()).TotalMinutes) * 60 + now.Minute;
                var fraction = GetFraction((GetMidnight() - GetEndTime()).TotalMinutes, partialDay);
                return fullWidth * fraction;
            }
        }

        private double GetFraction(double startHour, double partialDay)
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
