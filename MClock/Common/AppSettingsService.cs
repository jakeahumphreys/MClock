using System;
using MClock.Settings;
using MClock.Settings.Types;
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
            GeneralSettings = new GeneralSettings
            {
                CloseAppOnWeekends = Convert.ToBoolean(_configuration.GetSection(SectionKeys.GENERAL_SETTINGS)[SettingKeys.CLOSE_APP_ON_WEEKENDS])
            },
            ColourSettings = new ColourSettings
            {
                InvertColours = Convert.ToBoolean(_configuration.GetSection(SectionKeys.COLOUR_SETTINGS)[SettingKeys.INVERT_COLOURS]),
                EnableKaizenTimeColours = Convert.ToBoolean(_configuration.GetSection(SectionKeys.COLOUR_SETTINGS)[SettingKeys.ENABLE_KAIZEN_TIME_COLOURS]),
            },
            TimeSettings = new TimeSettings
            {
                WorkStartTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.WORK_START_TIME],
                WorkEndTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.WORK_END_TIME],
                LunchStartTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.LUNCH_START_TIME],
                LunchEndTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.LUNCH_END_TIME],
                KaizenStartTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.KAIZEN_START_TIME],
                DemosStartTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.DEMOS_START_TIME],
                DemosEndTime = _configuration.GetSection(SectionKeys.TIME_SETTINGS)[SettingKeys.DEMOS_END_TIME]
            },
            DiscordRichPresenceSettings = new DiscordRichPresenceSettings
            {
                EnableRichPresence = Convert.ToBoolean(_configuration.GetSection(SectionKeys.DISCORD_SETTINGS)[SettingKeys.ENABLE_RICH_PRESENCE]),
                EnabledOnWeekends = Convert.ToBoolean(_configuration.GetSection(SectionKeys.DISCORD_SETTINGS)[SettingKeys.ENABLED_ON_WEEKENDS])
            }
        };
    }
}