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
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly TimeHelper _timeHelper;
        private readonly DiscordRpcClient _discordRpcClient;

        public bool IsKaizenTime = false;

        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();

            _configuration = configuration;
            _discordRpcClient = new DiscordRpcClient("1099310581112119316");
            _appSettings = CreateSettings();
            _timeHelper = new TimeHelper(_appSettings);
            
            Timer.Loaded += Timer_Loaded;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Application.Current.MainWindow.Closing += OnWindowClosing;
            

            StartDiscordRichPresence();
            HandleAppColours();
        }

        private void HandleAppColours()
        {
            if (TimeHelper.IsWeekend() && _appSettings.ColourSettings.DisableSeparateColoursOnWeekends)
            {
                SetTimelineColour(Colors.Green);
                SetBacklineColour(Colors.Green);
            }
            else
            {
                if (_appSettings.ColourSettings.InvertColours)
                {
                    SetTimelineColour(Colors.Green);
                    SetBacklineColour(Colors.Red);
                }
            
                ChangeColoursIfKaizenTime();
            }
        }

        private bool IsDiscordRichPresenceEnabled()
        {
            if (!_appSettings.DiscordRichPresenceSettings.EnableRichPresence)
                return false;

            if (TimeHelper.IsWeekend() && _appSettings.DiscordRichPresenceSettings.EnabledOnWeekends == false)
                return false;

            return true;
        }

        private void StartDiscordRichPresence()
        {
            if(IsDiscordRichPresenceEnabled())
                _discordRpcClient.Initialize();
        }

        private void SetDiscordRichPresenceTimeLeft()
        {
            if (IsDiscordRichPresenceEnabled())
            {
                _discordRpcClient.SetPresence(new RichPresence
                {
                    Details = RichPresenceHelper.GetTimeLeftString(),
                    State = RichPresenceHelper.GetStateString(),
                    Assets = new Assets
                    {
                        SmallImageKey = RichPresenceHelper.GetSmallImageKey(),
                        LargeImageKey = RichPresenceHelper.GetLargeImageKey(),
                        LargeImageText = "Everything is fine",
                        SmallImageText = "Codeweaving"
                    }
                });
            }
        }

        private AppSettings CreateSettings()
        {
            return new AppSettings
            {
                AutoStartWorkApps = Convert.ToBoolean(_configuration["AutoStartWorkApps"]),
                EnableNotifications = Convert.ToBoolean(_configuration["EnableNotifications"]),
                ColourSettings = new ColourSettings
                {
                    InvertColours = Convert.ToBoolean(_configuration["InvertColours"]),
                    EnableKaizenTimeColours = Convert.ToBoolean(_configuration["EnableKaizenTimeColours"]),
                    DisableSeparateColoursOnWeekends = Convert.ToBoolean(_configuration["DisableOnWeekends"]),
                },
                TimeSettings = new TimeSettings
                {
                    WorkStartTime = _configuration.GetSection("TimeSettings")["WorkStartTime"],
                    WorkEndTime = _configuration.GetSection("TimeSettings")["WorkEndTime"],
                    LunchStartTime = _configuration.GetSection("TimeSettings")["LunchStartTime"],
                    LunchEndTime = _configuration.GetSection("TimeSettings")["LunchEndTime"],
                    KaizenStartTime = _configuration.GetSection("TimeSettings")["KaizenStartTime"]
                },
                DiscordRichPresenceSettings = new DiscordRichPresenceSettings
                {
                    EnableRichPresence = Convert.ToBoolean(_configuration.GetSection("DiscordRichPresenceSettings")["EnableRichPresence"]),
                    EnabledOnWeekends = Convert.ToBoolean(_configuration.GetSection("DiscordRichPresenceSettings")["EnabledOnWeekends"])
                }
            };
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
            HandleNotifications();
            SetDiscordRichPresenceTimeLeft();
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
            if (_appSettings.ColourSettings.EnableKaizenTimeColours)
            {
                var currentTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && currentTime > TimeHelper.GetKaizenStartTime() && currentTime < TimeHelper.GetEndTime() && !IsKaizenTime)
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
            _discordRpcClient.Dispose();
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
