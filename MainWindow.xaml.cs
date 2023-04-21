using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Clock
{
    public partial class MainWindow : Window
    {
        private const int Morning = 8;
        private const int Evening = 17;

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
            return hour < Evening;
        }

        private bool BeforeWork(int hour)
        {
            return hour < Morning;
        }

        private double GetNewWidth(DateTime now, double fullWidth)
        {
            if (BeforeWork(now.Hour))
            {
                var partialDay = (now.Hour) * 60 + now.Minute;
                var fraction = GetFraction(Morning, partialDay);
                return  fullWidth * fraction;
            }
            else if (DuringWork(now.Hour))
            {
                var partialDay = (now.Hour - Morning) * 60 + now.Minute;
                var fraction = GetFraction(Evening - Morning, partialDay);
                return fullWidth * fraction;
            }
            else
            {
                var partialDay = (now.Hour - Evening) * 60 + now.Minute;
                var fraction = GetFraction(24 - Evening, partialDay);
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
