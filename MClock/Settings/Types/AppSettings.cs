namespace MClock.Settings.Types;

public sealed class AppSettings
{
    public GeneralSettings GeneralSettings { get; set; }
    public ColourSettings ColourSettings { get; set; }
    public TimeSettings TimeSettings { get; set; }
    public DiscordRichPresenceSettings DiscordRichPresenceSettings { get; set; }
}

public sealed class GeneralSettings
{
    public bool CloseAppOnWeekends { get; set; }
}

public sealed class ColourSettings
{
    public bool InvertColours { get; set; }
    public bool EnableKaizenTimeColours { get; set; }
}

public sealed class TimeSettings
{
    public string? WorkStartTime { get; set; }
    public string? WorkEndTime { get; set; }
    public string? LunchStartTime { get; set; }
    public string? LunchEndTime { get; set; }
    public string? KaizenStartTime { get; set; }
    public string? DemosStartTime { get; set; }
    public string? DemosEndTime { get; set; }
}

public sealed class DiscordRichPresenceSettings
{
    public bool EnableRichPresence { get; set; }
}