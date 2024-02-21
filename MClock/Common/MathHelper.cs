using System;
using MClock.Types;

namespace MClock.Common;

public class MathHelper
{
    private readonly TimeHelper _timeHelper;

    public MathHelper(AppSettings appSettings)
    {
        _timeHelper = new TimeHelper(appSettings);
    }
    
    private double GetFraction(double startHour, double partialDay)
    {
        return partialDay / (startHour * 60.0);
    }
    
    public double GetNewWidth(DateTime now, double fullWidth)
    {
        var currentTime = new TimeOnly(now.Hour, now.Minute, now.Second);
        if (_timeHelper.IsBeforeWork())
        {
            var partialDay = (now.Hour) * 60 + now.Minute;
            var fraction = GetFraction(_timeHelper.GetStartTime().Hour, partialDay);
            return  fullWidth * fraction;
        }
        if (_timeHelper.IsDuringWork())
        {
            var partialDay = ((currentTime - _timeHelper.GetStartTime()).TotalMinutes) * 60 + now.Minute;
            var fraction = GetFraction((_timeHelper.GetEndTime() - _timeHelper.GetStartTime()).TotalMinutes, partialDay);
            return fullWidth * fraction;
        }
        else
        {
            var partialDay = ((currentTime - _timeHelper.GetEndTime()).TotalMinutes) * 60 + now.Minute;
            var fraction = GetFraction((_timeHelper.GetMidnight() - _timeHelper.GetEndTime()).TotalMinutes, partialDay);
            return fullWidth * fraction;
        }
    }
}