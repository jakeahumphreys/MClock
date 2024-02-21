using System;

namespace MClock.Common;

public static class MathHelper
{
    private static double GetFraction(double startHour, double partialDay)
    {
        return partialDay / (startHour * 60.0);
    }
    
    public static double GetNewWidth(DateTime now, double fullWidth)
    {
        var currentTime = new TimeOnly(now.Hour, now.Minute, now.Second);
        if (TimeHelper.IsBeforeWork())
        {
            var partialDay = (now.Hour) * 60 + now.Minute;
            var fraction = GetFraction(TimeHelper.GetStartTime().Hour, partialDay);
            return  fullWidth * fraction;
        }
        if (TimeHelper.IsDuringWork())
        {
            var partialDay = ((currentTime - TimeHelper.GetStartTime()).TotalMinutes) * 60 + now.Minute;
            var fraction = GetFraction((TimeHelper.GetEndTime() - TimeHelper.GetStartTime()).TotalMinutes, partialDay);
            return fullWidth * fraction;
        }
        else
        {
            var partialDay = ((currentTime - TimeHelper.GetEndTime()).TotalMinutes) * 60 + now.Minute;
            var fraction = GetFraction((TimeHelper.GetMidnight() - TimeHelper.GetEndTime()).TotalMinutes, partialDay);
            return fullWidth * fraction;
        }
    }
}