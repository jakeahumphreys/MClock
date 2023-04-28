using System;
using MClock.Types;
using Microsoft.Extensions.Configuration;

namespace MClock.Common;

public sealed class AppSettingsService
{
    private readonly IConfiguration _configuration;

    public AppSettingsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AppSettings GetSettings()
    {
        return new AppSettings
        {
            AutoStartWorkApps = Convert.ToBoolean(_configuration["AutoStartWorkApps"]),
            EnableNotifications = Convert.ToBoolean(_configuration["EnableNotifications"]),
            ColourSettings = new ColourSettings
            {
                InvertColours = Convert.ToBoolean(_configuration.GetSection("ColourSettings")["InvertColours"]),
                EnableKaizenTimeColours = Convert.ToBoolean(_configuration.GetSection("ColourSettings")["EnableKaizenTimeColours"]),
                DisableSeparateColoursOnWeekends = Convert.ToBoolean(_configuration.GetSection("ColourSettings")["DisableSeparateColoursOnWeekends"]),
            },
            TimeSettings = new TimeSettings
            {
                WorkStartTime = _configuration.GetSection("TimeSettings")["WorkStartTime"],
                WorkEndTime = _configuration.GetSection("TimeSettings")["WorkEndTime"],
                LunchStartTime = _configuration.GetSection("TimeSettings")["LunchStartTime"],
                LunchEndTime = _configuration.GetSection("TimeSettings")["LunchEndTime"],
                KaizenStartTime = _configuration.GetSection("TimeSettings")["KaizenStartTime"],
                DemosStartTime = _configuration.GetSection("TimeSettings")["DemosStartTime"],
                DemosEndTime = _configuration.GetSection("TimeSettings")["DemosEndTime"]
            },
            DiscordRichPresenceSettings = new DiscordRichPresenceSettings
            {
                EnableRichPresence = Convert.ToBoolean(_configuration.GetSection("DiscordRichPresenceSettings")["EnableRichPresence"]),
                EnabledOnWeekends = Convert.ToBoolean(_configuration.GetSection("DiscordRichPresenceSettings")["EnabledOnWeekends"])
            }
        };
    }
}