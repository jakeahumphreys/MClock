using System;
using System.Windows.Shapes;
using MClock.Types;
using Microsoft.Toolkit.Uwp.Notifications;
using Path = System.IO.Path;

namespace MClock.Common;

public sealed class NotificationService
{
    private readonly AppSettings _appSettings;

    public NotificationService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    public void HandleNotifications()
    {
        if (!_appSettings.EnableNotifications)
            return;
        
        if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime())
        {
            new ToastContentBuilder()
                .AddText("Work day has finished, remember to patch uncommitted work!")
                .AddHeroImage(new Uri($"file:///{Path.GetFullPath("images/sunset.jpg")}"))
                .Show();
        }

        if (TimeHelper.GetCurrentTime() == TimeHelper.GetLunchStartTime())
        {
            new ToastContentBuilder().AddText("It's lunchtime!").Show();
        }
        
        if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime())
        {
            new ToastContentBuilder().AddText("Lunchtime's over!").Show();
        }
    }
}