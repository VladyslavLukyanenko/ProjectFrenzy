using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FrenzyBot.Structures.User
{
    public static class ProxyManager
    {
        public static List<WebProxy> UnusedProxies = new List<WebProxy>();
        public static List<WebProxy> UsedProxies = new List<WebProxy>();
        private static Random random = new Random();

        public static void ReloadProxies()
        {
            UnusedProxies = new List<WebProxy>();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Drag your proxy file onto this window:");
            Console.ResetColor();
            var ProxyList = File.ReadAllLines(Console.ReadLine());
            
            foreach (var ProxyString in ProxyList)
            {
                try
                {
                    string[] Data = ProxyString.Split(':');
                    if (Data.Length == 2) //proxy is ip:port
                    {

                        string Ip = Data[0];
                        if (!int.TryParse(Data[1], out int Port))
                        {

                        }
                        else
                        {
                            var Proxy = new WebProxy(Ip, Port);
                            UnusedProxies.Add(Proxy);
                        }
                    }
                    else if (Data.Length == 4) //proxy is ip:port:user:pass
                    {
                        string Ip = Data[0];
                        string Port = Data[1];
                        string User = Data[2];
                        string Pass = Data[3];

                        var Proxy = new WebProxy(new Uri("http://" + Ip + ":" + Port), false, new string[0], new NetworkCredential(User, Pass));
                        UnusedProxies.Add(Proxy);
                    }
                }
                catch { }
            }

            Logger.Log($"Loaded {UnusedProxies.Count} proxies", ConsoleColor.Cyan);
        }

        public static WebProxy GetUnusedProxy()
        {
            if (UnusedProxies.Count == 0)
                return null;

            int ProxyIndex = random.Next(UnusedProxies.Count - 1);

            var Proxy = UnusedProxies[ProxyIndex];
            UsedProxies.Add(Proxy);
            UnusedProxies.RemoveAt(ProxyIndex);

            return Proxy;
        }
    }
}
