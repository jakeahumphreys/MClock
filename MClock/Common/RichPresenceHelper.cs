using System;

namespace MClock.Common;

public static class RichPresenceHelper
{
    public static string GetTimeLeftString()
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

    public static string GetStateString()
    {
        var statusString = "Working away in the code mines";

        if (TimeHelper.IsKaizenTime())
            statusString = "It's Kaizen time, I'm doing a learn";

        if (TimeHelper.IsLunchTime())
            statusString = "Lunchtime 🍕";

        return statusString;
    }

    public static string GetSmallImageKey()
    {
        return "company_logo";
    }
    
    public static string GetLargeImageKey()
    {
        return "nootnoot";
    }
}