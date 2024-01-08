using System;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Net;

namespace FrenzyBot.Structures.User
{
    public static class Authenticator
    {
        private const string ProductId = "5efe8873e44d9d6f87d69a33";
        private static JObject HandleResponse(HttpResponseMessage msg)
        {
            var AuthRespJson = JObject.Parse(msg.Content.ReadAsStringAsync().Result);
            return AuthRespJson;
        }

        public static bool ValidateLicense(bool InitialCheck = false)
        {

            Settings _Settings = SettingsUtil.LoadSettings();

            if(_Settings.LicenseKey == null)
            {
                Console.WriteLine("Enter your license key:");
                _Settings.LicenseKey = Console.ReadLine().Trim();
            }

            SettingsUtil.SaveSettings(_Settings);
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
            Client.DefaultRequestHeaders.Add("Accept", "*/*");
            Client.DefaultRequestHeaders.Add("Auth", _Settings.LicenseKey);
            const string API_ENDPOINT = "https://api.projectindustries.gg";

            var AuthResp = Client.GetAsync
                ($"{API_ENDPOINT}/licenseKeys/{ProductId}/authenticate?sessionId={WebUtility.UrlEncode(HWID.generateHWID())}").Result;

            var AuthRespJson = HandleResponse(AuthResp) ?? null;
            
            if((bool)AuthRespJson["success"])
            {
                if (InitialCheck)
                {
                    var VersionResp = new HttpClient().GetAsync($"{API_ENDPOINT}/product/project-frenzy").Result;
                    var VersionRespContent = JObject.Parse(VersionResp.Content.ReadAsStringAsync().Result);

                    if (!Versions.program_version.Equals(VersionRespContent["version"].ToString()))
                    {
                        Logger.Log($"You are running version {Versions.program_version}. Update to version {VersionRespContent}", ConsoleColor.Yellow);
                        Console.Read();
                        Environment.Exit(0);
                    }
                }
                return true;
            }
            else
            {
                if (AuthRespJson["message"].ToString() != "")
                {
                    if (InitialCheck)
                    {
                        Logger.Log(AuthRespJson["message"].ToString(), ConsoleColor.White);
                    }
                }

                return false;
            }
        }
    }
}
