using System;
using System.Windows.Shapes;
using MClock.Types;
using Microsoft.Toolkit.Uwp.Notifications;
using Path = System.IO.Path;

namespace MClock.Common;

public sealed class NotificationService
{
    private readonly AppSettings _appSettings;
    private bool _lunchStartNotified = false;
    private bool _lunchEndNotified = false;

    public NotificationService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    
    public void HandleNotifications()
    {
        if (!_appSettings.EnableNotifications)
            return;

        var currentTime = TimeHelper.GetCurrentTime();
        var lunchStartTime = TimeHelper.GetLunchStartTime();

        var timeDifference = currentTime - lunchStartTime;
        var threshold = TimeSpan.FromSeconds(1);

        if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime())
        {
            new ToastContentBuilder()
                .AddText("Work day has finished, remember to patch uncommitted work!")
                .AddHeroImage(new Uri($"file:///{Path.GetFullPath("images/sunset.jpg")}"))
                .Show();
        }

        if (timeDifference.Duration() <= threshold && !_lunchStartNotified)
        {
            _lunchEndNotified = true;
            new ToastContentBuilder().AddText("It's lunchtime!").Show();
        }
        
        if (TimeHelper.GetCurrentTime() == TimeHelper.GetEndTime() && !_lunchEndNotified)
        {
            _lunchEndNotified = true;
            new ToastContentBuilder()
                .AddText("Lunchtime's over, back to work!")
                .AddHeroImage(new Uri($"file:///{Path.GetFullPath("images/lunchTimeOver.png")}"))
                .Show();
        }
    }
}