namespace MClock.Settings.Types;

public sealed class AppSettings
{
    public ColourSettings ColourSettings { get; set; }
    public TimeSettings TimeSettings { get; set; }
    public DiscordRichPresenceSettings DiscordRichPresenceSettings { get; set; }

    public AppSettings()
    {
        TimeSettings = new TimeSettings();
    }
}

public sealed class ColourSettings
{
    public bool InvertColours { get; set; }
    public bool EnableKaizenTimeColours { get; set; }
    public bool DisableSeparateColoursOnWeekends { get; set; }
}

public sealed class TimeSettings
{
    public string WorkStartTime { get; set; } = null!;
    public string WorkEndTime { get; set; } = null!;
    public string LunchStartTime { get; set; }
    public string LunchEndTime { get; set; }
    public string KaizenStartTime { get; set; } = null!;
    public string DemosStartTime { get; set; }
    public string DemosEndTime { get; set; }
}

public sealed class DiscordRichPresenceSettings
{
    public bool EnableRichPresence { get; set; }
    public bool EnabledOnWeekends { get; set; }
}