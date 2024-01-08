using Newtonsoft.Json;
using System;
using System.IO;

namespace FrenzyBot.Structures.User
{
    public static class SettingsUtil
    {
        public static void SaveSettings(Settings _Settings)
        {
            var SettingsString = JsonConvert.SerializeObject(_Settings, Formatting.Indented);
            try
            {
                File.WriteAllText(Path.Combine(Program.MainDir, "settings.json"), SettingsString);
            }
            catch
            {
                Logger.Log("Failed to save settings. Press any key to quit.", ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(-1);
            }
            Program.FrenzySettings = _Settings;
        }

        public static Settings LoadSettings()
        {
            Settings _Settings;
            try
            {
                _Settings = JsonConvert.DeserializeObject<Settings>
                (File.ReadAllText(Path.Combine(Program.MainDir, "settings.json")));
            }
            catch
            {
                _Settings = new Settings
                {
                    LicenseKey = null,
                    DiscordWebhookUrl = null,
                    EmulatorIp = null
                };
            }
            return _Settings;
        }
    }
}
