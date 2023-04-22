﻿using System;
using MClock.Types;

namespace MClock.Common;

public class TimeHelper
{
    private static AppSettings _appSettings;

    public TimeHelper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    public static TimeOnly GetStartTime()
    {
        var startTime = _appSettings.TimeSettings.WorkStartTime;
        var hour = Convert.ToInt32(startTime.Substring(0, 2));
        var minutes = Convert.ToInt32(startTime.Substring(3, 2));
        return new TimeOnly(hour, minutes, 0);
    }

    public static TimeOnly GetEndTime()
    {
        var endTime = _appSettings.TimeSettings.WorkEndTime;
        var hour = Convert.ToInt32(endTime.Substring(0, 2));
        var minutes = Convert.ToInt32(endTime.Substring(3, 2));
        return new TimeOnly(hour, minutes, 0);
    }
    
    public static TimeOnly GetLunchStartTime()
    {
        var startTime = _appSettings.TimeSettings.LunchStartTime;
        var hour = Convert.ToInt32(startTime.Substring(0, 2));
        var minutes = Convert.ToInt32(startTime.Substring(3, 2));
        return new TimeOnly(hour, minutes, 0);
    }
    
    public static TimeOnly GetLunchEndTime()
    {
        var endTime = _appSettings.TimeSettings.LunchEndTime;
        var hour = Convert.ToInt32(endTime.Substring(0, 2));
        var minutes = Convert.ToInt32(endTime.Substring(3, 2));
        return new TimeOnly(hour, minutes, 0);
    }

    public static TimeOnly GetKaizenStartTime()
    {
        var kaizenStartTime = _appSettings.TimeSettings.KaizenStartTime;
        var hour = Convert.ToInt32(kaizenStartTime.Substring(0, 2));
        var minutes = Convert.ToInt32(kaizenStartTime.Substring(3, 2));
        return new TimeOnly(hour, minutes, 0);
    }
        
    public static TimeOnly GetMidnight()
    {
        return new TimeOnly(0, 0, 0);
    }

    public static TimeOnly GetCurrentTime()
    {
        return TimeOnly.FromDateTime(DateTime.Now);
    }

    public static bool IsKaizenTime()
    {
        return DateTime.Today.DayOfWeek == DayOfWeek.Friday && GetCurrentTime() > GetKaizenStartTime() && GetCurrentTime() < GetEndTime();
    }

    public static bool IsLunchTime()
    {
        return GetCurrentTime() >= GetStartTime() && GetCurrentTime() < GetLunchEndTime();
    }
}