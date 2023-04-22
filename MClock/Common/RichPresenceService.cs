using System;
using DiscordRPC;
using MClock.Types;

namespace MClock.Common;

public class RichPresenceService
{
    private readonly AppSettings _appSettings;
    private readonly DiscordRpcClient _discordRpcClient;

    public RichPresenceService(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _discordRpcClient = new DiscordRpcClient("1099310581112119316");
    }
    
    public string GetTimeLeftString()
    {
        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        var timeLeft = TimeHelper.GetEndTime() - currentTime;

        var hoursLeft = Math.Floor((double) timeLeft.Hours);
        var minutesLeft = Math.Floor((double) timeLeft.Minutes);
        var secondsLeft = Math.Floor((double) timeLeft.Seconds);

        var timeLeftString = $"{hoursLeft}h:{minutesLeft}m:{secondsLeft}s left";

        if (hoursLeft < 0)
            timeLeftString = $"{minutesLeft}m:{secondsLeft}s left";

        return timeLeftString;
    }

    public string GetStateString()
    {
        var statusString = "Working away in the code mines";

        if (TimeHelper.IsKaizenTime())
            statusString = "It's Kaizen time, I'm doing a learn";

        if (TimeHelper.IsLunchTime())
            statusString = "Lunchtime 🍕";

        return statusString;
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
        if(IsDiscordRichPresenceEnabled())
            _discordRpcClient.Initialize();
    }

    public void StopRichPresenceIfEnabled()
    {
        if(IsDiscordRichPresenceEnabled())
            _discordRpcClient.Dispose();
    }

    public void UpdateRichPresenceIfEnabled()
    {
        if (IsDiscordRichPresenceEnabled())
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