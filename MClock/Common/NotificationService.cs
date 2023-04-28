using System;
using System.Collections.Generic;
using System.Linq;
using MClock.Types;
using Microsoft.Toolkit.Uwp.Notifications;
using Path = System.IO.Path;

namespace MClock.Common;

public sealed class NotificationService
{
    private readonly AppSettings _appSettings;
    private List<NotificationEvent> _notificationEvents;

    public NotificationService(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _notificationEvents = new List<NotificationEvent>();
    }
    
    public void HandleNotifications()
    {
        if (!_appSettings.EnableNotifications)
            return;
        
        if (TimeHelper.GetCurrentTime() >= TimeHelper.GetEndTime())
        {
            if (ShouldNotify(NotificationEventType.WorkEnd))
            {
                new ToastContentBuilder()
                    .AddText("Work day has finished, remember to patch uncommitted work!")
                    .AddHeroImage(new Uri($"file:///{Path.GetFullPath("images/sunset.jpg")}"))
                    .Show();
                _notificationEvents.Add(new NotificationEvent(NotificationEventType.WorkEnd, TimeOnly.FromDateTime(DateTime.Now), false));
            }
            
        }

        if (TimeHelper.IsDemos())
        {
            if (ShouldNotify(NotificationEventType.DemosStart))
            {
                new ToastContentBuilder().AddText("Demos has started").Show();
                _notificationEvents.Add(new NotificationEvent(NotificationEventType.DemosStart, TimeOnly.FromDateTime(DateTime.Now), false));
            }
        }

        if (TimeHelper.IsLunchTime())
        {
            if (ShouldNotify(NotificationEventType.LunchTimeStart))
            {
                new ToastContentBuilder().AddText("It's lunchtime!").Show();
                _notificationEvents.Add(new NotificationEvent(NotificationEventType.LunchTimeStart, TimeOnly.FromDateTime(DateTime.Now), false));
            }
        }
        
        if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime())
        {
            if (ShouldNotify(NotificationEventType.LunchTimeEnd))
            {
                new ToastContentBuilder()
                    .AddText("Lunchtime's over, back to work!")
                    .AddHeroImage(new Uri($"file:///{Path.GetFullPath("images/lunchTimeOver.png")}"))
                    .Show();
                _notificationEvents.Add(new NotificationEvent(NotificationEventType.LunchTimeEnd, TimeOnly.FromDateTime(DateTime.Now), false));
            }
        }
    }

    private bool ShouldNotify(NotificationEventType eventType)
    {
        var fiveMinutesAgo = new TimeOnly(TimeHelper.GetCurrentTime().Hour, TimeHelper.GetCurrentTime().Minute - 5, TimeHelper.GetCurrentTime().Second);
        var recentEvent = _notificationEvents.SingleOrDefault(x =>
            x.NotificationEventType == eventType &&
            x.OccurredAt > fiveMinutesAgo);

        if (recentEvent == null)
            return true;

        if (recentEvent.CanNotifyAgain)
            return true;

        return false;
    }
}