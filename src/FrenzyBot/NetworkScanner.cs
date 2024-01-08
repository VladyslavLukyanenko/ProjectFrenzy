using FrenzyBot.Structures.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FrenzyBot
{
    public static class NetworkScanner
    {
        public static string Emulator;

        public static bool Scan(string ip)
        {
            for (int retries = 0; retries < 5; retries++)
            {
                try
                {

                    HttpClient Client = new HttpClient
                    {
                        Timeout = TimeSpan.FromSeconds(1)
                    };

                    var Resp = Client.GetAsync(ip + "/ping").Result;
                    var Content = Resp.Content.ReadAsStringAsync().Result;
                    var JsonResp = JObject.Parse(Program.AndroidEncryption.Decrypt(Content));
                    if (JsonResp["version"].ToString() == Versions.server_version)
                    {
                        Emulator = ip;
                        return true;
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
