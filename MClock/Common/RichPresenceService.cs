using System;
using System.Threading;
using System.Windows;
using DiscordRPC;
using MClock.Types;

namespace MClock.Common;

public class RichPresenceService
{
    private readonly MainWindow _mainWindow;
    private readonly AppSettings _appSettings;
    private DiscordRpcClient _discordRpcClient;
    private bool _isEnabledOverride = false;

    public RichPresenceService(MainWindow mainWindow, AppSettings appSettings)
    {
        _mainWindow = mainWindow;
        _appSettings = appSettings;
        _discordRpcClient = new DiscordRpcClient("1099310581112119316");
    }

    public void OverrideRichPresence()
    {
        if (!_isEnabledOverride)
        {
            _isEnabledOverride = true;
            SetDiscordLogoVisibility();
            StopRichPresenceIfEnabled();
        }
        else
        {
            _discordRpcClient = new DiscordRpcClient("1099310581112119316");
            _isEnabledOverride = false;
            StartRichPresenceIfEnabled();
        }
    }
    
    public string GetTimeLeftString()
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);

        if (TimeHelper.IsDuringWork())
        {
            var timeLeft = TimeHelper.GetEndTime() - currentTime;

            var hoursLeft = Math.Floor((double) timeLeft.Hours);
            var minutesLeft = Math.Floor((double) timeLeft.Minutes);
            var secondsLeft = Math.Floor((double) timeLeft.Seconds);

            var timeLeftString = $"{hoursLeft}h:{minutesLeft}m:{secondsLeft}s left";

            if (hoursLeft < 0)
                timeLeftString = $"{minutesLeft}m:{secondsLeft}s left";

            return timeLeftString;
        }
        else
        {
            var timeSince = currentTime - TimeHelper.GetEndTime();

            var hoursLeft = Math.Floor((double) timeSince.Hours);
            var minutesLeft = Math.Floor((double) timeSince.Minutes);
            var secondsLeft = Math.Floor((double) timeSince.Seconds);

            var timeLeftString = $"{hoursLeft}h:{minutesLeft}m:{secondsLeft}s since";

            if (hoursLeft < 0)
                timeLeftString = $"{minutesLeft}m:{secondsLeft}s since";

            return timeLeftString;
        }
    }

    public string GetStateString()
    {
        if (TimeHelper.IsKaizenTime())
           return "It's Kaizen time, I'm doing a learn";

        if (TimeHelper.IsLunchTime())
            return "Lunchtime 🍕";

        if (TimeHelper.IsAfterWork())
            return "I'm finished 🎉 yet app's still open";

        return "Working away in the code mines";
    }

    public string GetSmallImageKey()
    {
        return "company_logo";
    }
    
    public string GetLargeImageKey()
    {
        if (TimeHelper.IsKaizenTime())
            return "kaizen";
        
        return "nootnoot";
    }
    
    private bool IsDiscordRichPresenceEnabled()
    {
        if (!_appSettings.DiscordRichPresenceSettings.EnableRichPresence)
            return false;

        if (TimeHelper.IsWeekend() && _appSettings.DiscordRichPresenceSettings.EnabledOnWeekends == false)
            return false;

        return true;
    }
    
    public void StartRichPresenceIfEnabled()
    {
        if (IsDiscordRichPresenceEnabled())
        {
            _discordRpcClient.Initialize();
            SetDiscordLogoVisibility();
        }
    }

    private void SetDiscordLogoVisibility()
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            if (IsDiscordRichPresenceEnabled() && !_isEnabledOverride)
            {
                _mainWindow.DiscordConnected.Visibility = Visibility.Visible;
                _mainWindow.DiscordConnectedDisabled.Visibility = Visibility.Hidden;
            }
            else
            {
                _mainWindow.DiscordConnected.Visibility = Visibility.Hidden;
                _mainWindow.DiscordConnectedDisabled.Visibility = Visibility.Visible;
            }
        });
    }

    public void StopRichPresenceIfEnabled()
    {
        if(IsDiscordRichPresenceEnabled())
            _discordRpcClient.Dispose();
    }

    public void UpdateRichPresenceIfEnabled()
    {
        if (IsDiscordRichPresenceEnabled() && !_isEnabledOverride)
        {
            _discordRpcClient.SetPresence(new RichPresence
            {
                Details = GetTimeLeftString(),
                State = GetStateString(),
                Assets = new Assets
                {
                    SmallImageKey = GetSmallImageKey(),
                    LargeImageKey = GetLargeImageKey(),
                    LargeImageText = "Everything is fine",
                    SmallImageText = "Codeweaving"
                }
            });
        }
    }
}