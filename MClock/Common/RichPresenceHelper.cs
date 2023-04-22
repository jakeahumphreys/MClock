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

        var timeLeftString = $"Finishing in {hoursLeft} hours {minutesLeft} minutes and {secondsLeft} seconds";

        if (hoursLeft < 0)
            timeLeftString = $"Finishing in {minutesLeft} minutes and {secondsLeft} seconds.";

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
}