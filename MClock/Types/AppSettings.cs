namespace MClock.Types;

public sealed class AppSettings
{
    public bool AutoStartWorkApps { get; set; }
    public bool EnableNotifications { get; set; }
    public bool InvertColours { get; set; }
    public bool EnableKaizenTimeColours { get; set; }
    public TimeSettings TimeSettings { get; set; }

    public AppSettings()
    {
        TimeSettings = new TimeSettings();
    }
}

public sealed class TimeSettings
{
    public string WorkStartTime { get; set; }
    public string WorkEndTime { get; set; }
    public string KaizenStartTime { get; set; }
}